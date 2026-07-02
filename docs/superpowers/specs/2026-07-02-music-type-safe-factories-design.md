# Music type-safe factories, Parse/TryParse convention, and canonical round-trippable ToString

Date: 2026-07-02
Status: design approved, pending spec review

## Problem

Two related weaknesses in `Semantics.Music`:

1. **Naked primitives in construction.** Note letters `A`–`G` and accidentals (`#`, `b`, `♯`, `♭`) are parsed as `char` inside `Pitch.FromName` / `Chord.ParseRoot` / `Key.ChordFromRomanNumeral`, with the letter→pitch-class `switch` duplicated. Neither concept has a type, so the compiler cannot enforce valid values.
2. **Inconsistent, non-round-tripping string surface.** String entry points are named `FromName` / `FromPattern` rather than the .NET `Parse`/`TryParse` convention, and most value types have no canonical string form at all (they inherit the record default `ToString`), so there is no `Parse(x.ToString()) == x` guarantee anywhere.

This is a very new library (2.0-era, per-package split), so breaking renames are acceptable and there is no compatibility burden.

## Goals

1. Introduce `NoteLetter` and `Accidental` enums for the two concepts with no type.
2. Add compiler-enforced construction (`Create`) alongside the string parsers.
3. Give **every in-scope music type** a canonical `ToString()` and a matching `Parse`/`TryParse`, with a tested **canonical-output round-trip**: `Parse(x.ToString()) == x`.
4. Rename the string entry points to the `Parse`/`TryParse` convention (hard rename, no shims).
5. Remove the duplicated char-parsing by routing everything through the new enums.

## In scope (which types get canonical ToString + Parse/TryParse)

Confirmed with the user:

- **Core value types:** `PitchClass`, `Pitch`, `Interval`, `Duration`, `TimeSignature`, `Velocity`, `Tempo`, `Mode`, `Scale`, `Key`, `Chord`.
- **Score composites:** `Note`, `Rest`, `ChordEvent`.
- **Analysis aggregates:** `Progression`, `Section`, `Arrangement`, `Form`.

## Out of scope / non-goals

- **Analysis result records** (`ScaleDegree`, `KeyMatch`, `CadenceInstance`, `ChromaticAnalysis`): analysis outputs, not parsed from text. No canonical `Parse`/round-trip.
- **No `ModeName` enum.** `Mode` already exposes 30 static constant properties (`Mode.Dorian`, …) — those are the compiler-enforced path.
- **No typed roman-numeral factory.** `Key.ChordFromRomanNumeral(string)` stays a parser (though its char-scan is refactored onto `Accidental`).
- **No `IParsable<T>`** (net7+ only; Music targets down to netstandard2.0 and the repo avoids conditional compilation). Plain static `Parse`/`TryParse` only.
- **No melodic-sequence aggregate.** A `Note`/`Rest`/`ChordEvent` sequence type (melodic counterpart to `Progression`) was raised as a future idea and explicitly deferred.
- **No changes to numeric range semantics** — `Pitch.Create(int midi)`, `Interval.Create(int semitones)`, etc. keep their integer factories; the new work is additive.

## New enums

Enum values are chosen so a cast does the arithmetic — no lookup table.

```csharp
/// <summary>The seven natural note letters. The underlying value is the natural pitch class.</summary>
public enum NoteLetter { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 }

/// <summary>A pitch alteration. The underlying value is the semitone offset.</summary>
public enum Accidental { DoubleFlat = -2, Flat = -1, Natural = 0, Sharp = 1, DoubleSharp = 2 }
```

A pitch class is then `(int)letter + (int)accidental`, folded mod 12.

## Typed factories

```csharp
// PitchClass.cs
public static PitchClass Create(NoteLetter letter, Accidental accidental) =>
    Create((int)letter + (int)accidental);

// Pitch.cs (keeps existing Create(int midi))
public static Pitch Create(NoteLetter letter, Accidental accidental, int octave) =>
    Create(((octave + 1) * 12) + (int)letter + (int)accidental);
```

Chords keep object-initializer + `Parse` construction (a full typed factory would have an unwieldy parameter list); the new `PitchClass` overload makes hand-built chord roots compiler-enforced.

## Renames to the Parse/TryParse convention

| Current | Becomes |
|---|---|
| `Pitch.FromName(string)` | `Pitch.Parse` + `Pitch.TryParse(string, out Pitch)` |
| `Mode.FromName(string)` | `Mode.Parse` + `Mode.TryParse(string, out Mode)` |
| `Form.FromPattern(string)` | `Form.Parse` + `Form.TryParse(string, out Form)` |
| `Chord.Parse(string)` | unchanged name; add `Chord.TryParse(string, out Chord)` |

Hard rename, no `[Obsolete]` shims. `Create` = compiler-enforced construction; `Parse` = from text; `ToString` = back to text. Every in-scope type gets `Parse` + `TryParse`. Parse cores are factored into a private `bool TryParseCore(...)` that `Parse` (throws on false) and `TryParse` (returns the bool) both call, so `TryParse` is never throw-driven.

## Canonical string forms

### Leaf value types (single-line scalars)

| Type | Canonical `ToString()` | Example | Notes |
|---|---|---|---|
| `PitchClass` | sharp-spelled name | `C#` | `Parse` accepts flats/double accidentals, emits sharps |
| `Pitch` | pitch class + octave | `F#3` | keep the existing `Name` property; `ToString` returns it |
| `Interval` | signed semitones | `7`, `-5` | the type is defined by semitones |
| `Duration` | reduced `n/d` | `3/8` | |
| `TimeSignature` | `beats/unit` | `6/8` | |
| `Velocity` | integer | `96` | |
| `Tempo` | `{bpm}bpm@{beat}` | `120bpm@1/4` | beat always encoded (invariant-culture, round-trippable `bpm`) |
| `Mode` | canonical lower name | `harmonic_minor` | |
| `Scale` | `{root} {mode}` | `C dorian` | single-space split |
| `Key` | `{tonic} {mode}` | `A aeolian` | same shape as `Scale`, distinct type/`Parse` |
| `Chord` | canonical symbol | `Cmaj7`, `C/G` | see round-trip caveat below |
| `Form` | `Pattern` | `AABA` | already round-trips |

### Score composites (single-line scalars, context-free explicit durations)

| Type | Canonical `ToString()` | Example |
|---|---|---|
| `ChordEvent` | `{chord}:{duration}` | `Cmaj7:1/4` |
| `Note` | `{pitch}:{duration}:v{velocity}` | `C4:1/4:v80` |
| `Rest` | `R:{duration}` | `R:1/4` |

`:` is safe as the top separator: chord symbols use `/` (slash chords) but never `:`.

### Analysis aggregates (hybrid lead-sheet / chart style)

Structure is carried by chart idioms (bracket section headers, bar lines, blank lines), not YAML-style keys.

**`Progression` — one line.** Time signature up front, bars separated by ` | `, chords left to right. Duration is carried by **beat slashes**: a chord sounds for one beat, and each following whole-token `/` extends it one more beat.

```
4/4  Dm7 / G7 / | Cmaj7 / / /
```

(Dm7 two beats, G7 two beats, Cmaj7 a full 4/4 bar.) All-one-beat is bare chords: `4/4  Am7 Dm7 | E7 Am7`.

- A beat slash is a whole `/` token (space-delimited), so it never collides with a slash chord `C/G` or a duration `1/4`.
- Bar lines ` | ` are cosmetic: `ToString` inserts them at bar boundaries; `Parse` derives duration from the slashes and treats `|` as validation.
- Beat = `1/BeatUnit`. Round-trips exactly whenever each chord's duration is a whole number of beats (the normal case). Sub-beat durations use an explicit escape `Chord@n/d` (e.g. `Cmaj7@1/8`) so the format is total.
- `Progression.ToString` owns this chart rendering; it does **not** concatenate `ChordEvent.ToString()` (which is context-free and has no time signature to count beats against).

**`Section` — bracket header, then its progression.** Section type in brackets, optional quoted label, optional key in parens:

```
[Verse] "Verse 1" (C major)
4/4  Dm7 / G7 / | Cmaj7 / / /
```

Minimal (no label, no local key): `[Intro]` then the progression line. Header grammar: `[<SectionType>]` optionally followed by ` "<label>"` then ` (<key>)`; both optional and omitted when null.

**`Arrangement` — home key line, then blank-line-separated sections.**

```
A minor

[Intro]
4/4  Am / / /

[Verse] "Verse 1"
4/4  Am7 / Dm7 / | E7 / / /

[Bridge] (C major)
4/4  C / / / | G / / / | Am / / / | F / / /
```

First non-empty line is the home key (bare). Each section is a bracket header (optional `"label"` / `(key)`) followed by its one-line progression. `Parse` splits on blank lines / `[` headers.

## Chord round-trip caveat

`Chord.ToString()` is a canonical symbol formatter defined as the inverse of `Chord.Parse`, so `Parse(chord.ToString()) == chord` holds for any chord obtainable from `Parse` (the canonical corpus). A chord hand-built via object initializer with a field combination `Parse` cannot produce (e.g. `Augmented` quality together with a `b5`) is not guaranteed to round-trip. The formatter must emit spellings that reproduce the exact structure (e.g. a `Diminished`+`Dominant` chord spells as `m7b5`, not `dim7`, because `dim7` parses to a `Diminished` seventh). This is the highest-risk piece and is covered by enumerating a broad symbol corpus in tests.

## ToString override note

This overrides the auto-generated record `ToString` (e.g. `Pitch { Midi = 60 }` becomes `C4`). Audit tests and callers for reliance on the default record form before switching.

## Exception behavior (judgment call)

`Parse` throws `FormatException` on unparseable text, matching the BCL `Parse` convention the rename aligns to (`int.Parse`, `DateTime.Parse`); this takes precedence over CLAUDE.md's general "prefer `ArgumentException`" guidance, which targets argument validation rather than text parsing. `null` input throws `ArgumentNullException` (via `Ensure.NotNull`). `TryParse` throws nothing and returns `false`.

## Internal parse routing

`Pitch.Parse`, `Chord.ParseRoot`, and `Key.ChordFromRomanNumeral` are refactored to map the leading char to a `NoteLetter` via one shared helper and accumulate accidental chars into an `Accidental`/int offset, then delegate to the typed `Create`. This removes the two duplicated `C => 0, D => 2, …` switches and the three duplicated `#`/`b` scans.

## Mode literal centralization

The 30 canonical mode names currently appear twice (as `Shapes` dictionary keys and as string literals in the 30 static properties). Centralize so they cannot drift (single source of the name strings). Public surface (30 constants + `Parse`/`TryParse`) is unchanged.

## Blast radius (renames)

19 call sites of `FromName`/`FromPattern` across 8 files: `Chord.cs` (2, internal `Pitch.FromName` in `Voice`), tests (`PitchTests`, `ModeTests`, `FormTests`, `FrequencyBridgeTests`, `ScorePrimitivesTests`), and docs (`Semantics.Music/README.md`, the analysis-aggregate plan doc). All updated to the new names; README examples also show the typed `Create` factories and the canonical string forms.

## Testing

- **Round-trip** `Parse(x.ToString()) == x` for every in-scope type across representative values: pitch classes (naturals, sharps, flats, double accidentals), pitches, intervals (0, positive, negative, > octave), durations, time signatures, velocities, tempos (default and explicit beat), all mode families, scales, keys, a broad chord symbol corpus, score composites, and aggregates (progressions with whole-beat and sub-beat durations, sections with/without label/key, multi-section arrangements including repeated sections).
- **Typed-factory equivalence:** `Pitch.Create(NoteLetter.C, Accidental.Sharp, 4) == Pitch.Parse("C#4")`; the `PitchClass` overload against known values.
- **Enum-cast invariants:** `(int)NoteLetter.X` equals the natural pitch class; `(int)Accidental.Y` equals the semitone offset.
- **TryParse** returns `false` (no throw) on malformed input; `Parse` throws `FormatException` for the same input and `ArgumentNullException` for `null`.
- Existing Music tests pass after the rename.
