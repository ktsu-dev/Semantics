# Music type-safe factories and Parse/TryParse convention

Date: 2026-07-02
Status: design approved, pending spec review

## Problem

Several factory and construction paths in `Semantics.Music` accept "naked" primitives (`string`, `char`, `int`) drawn from a small fixed set, so the compiler cannot enforce valid values. The worst cases:

- Note letters `A`–`G` are parsed as `char` via a `switch` duplicated in `Pitch.FromName` and `Chord.ParseRoot`.
- Accidentals (`#`, `b`, `♯`, `♭`) are scanned as `char` in three places (`Pitch.FromName`, `Chord.ParseRoot`, `Key.ChordFromRomanNumeral`).
- Neither concept has a type anywhere in the project.

Separately, the string entry points are named inconsistently (`FromName`, `FromPattern`) rather than following the .NET `Parse`/`TryParse`/`ToString` convention.

This is a very new library (2.0-era, per-package split), so breaking renames are acceptable and there is no compatibility burden.

## Goals

1. Introduce enums for the two concepts with no type: note letter and accidental.
2. Add compiler-enforced construction (`Create`) alongside the existing string parsers.
3. Rename the string entry points to `Parse`/`TryParse` and give each a `ToString` that round-trips.
4. Remove the duplicated char-parsing logic by routing everything through the new enums.

## Non-goals (YAGNI / deferred)

- **No `ModeName` enum.** `Mode` already exposes 30 static constant properties (`Mode.Dorian`, …); those *are* the compiler-enforced path. Adding a parallel enum duplicates them.
- **No typed roman-numeral factory.** `Key.ChordFromRomanNumeral(string)` stays a parser. Roman numerals are compound (degree + quality + alteration) and `ScaleDegree` already structures part of it. Revisit later if wanted.
- **No `Chord.ToString()` symbol formatter.** Reversing the chord DSL into a canonical symbol is a real, lossy feature (many symbols map to one chord). Out of scope; `Chord` only gains `TryParse`.
- **No changes to genuinely numeric factories** (`Pitch.Create(int midi)`, `Interval.Create(int semitones)`, `TimeSignature.Create(int beats, int beatUnit)`, MIDI velocity, etc.). Those are ranges, not label sets.

## Design

### New enums (in `Semantics.Music`, own files, standard header)

Enum values are chosen so a cast does the arithmetic — no lookup table needed.

```csharp
/// <summary>The seven natural note letters. The underlying value is the natural pitch class.</summary>
public enum NoteLetter
{
    C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11,
}

/// <summary>A pitch alteration. The underlying value is the semitone offset.</summary>
public enum Accidental
{
    DoubleFlat = -2, Flat = -1, Natural = 0, Sharp = 1, DoubleSharp = 2,
}
```

With these, a pitch class is simply `(int)letter + (int)accidental`, folded mod 12.

### Typed factories

```csharp
// PitchClass.cs — new overload
public static PitchClass Create(NoteLetter letter, Accidental accidental) =>
    Create((int)letter + (int)accidental);

// Pitch.cs — new typed factory (keeps existing Create(int midi))
public static Pitch Create(NoteLetter letter, Accidental accidental, int octave) =>
    Create(((octave + 1) * 12) + (int)letter + (int)accidental);
```

Chord roots do not get a dedicated big factory (the parameter list would be unwieldy). Chords remain constructable via object-initializer (`new Chord { Root = PitchClass.Create(NoteLetter.C, Accidental.Natural), Quality = ChordQuality.Major }`) — which is now fully compiler-enforced — plus `Parse` for text.

### Renames to the Parse/TryParse convention

| Current | Becomes |
|---|---|
| `Pitch.FromName(string)` | `Pitch.Parse(string)` + `Pitch.TryParse(string, out Pitch)` |
| `Mode.FromName(string)` | `Mode.Parse(string)` + `Mode.TryParse(string, out Mode)` |
| `Form.FromPattern(string)` | `Form.Parse(string)` + `Form.TryParse(string, out Form)` |
| `Chord.Parse(string)` | unchanged name; add `Chord.TryParse(string, out Chord)` |

`Create` = compiler-enforced construction. `Parse` = from text. `ToString` = back to text. Hard rename, no `[Obsolete]` shims.

`TryParse` is implemented by wrapping the parse logic; on failure it returns `false` and sets `out` to `null`/`default`. To avoid throw-driven `TryParse`, the parse core is refactored into a private `bool TryParseCore(...)` that both `Parse` (throws on false) and `TryParse` (returns the bool) call.

### Internal parse routing

`Pitch.Parse` and `Chord.ParseRoot` are refactored to:
1. Map the leading char to a `NoteLetter` (single small lookup, one place — a private helper `TryNoteLetter(char, out NoteLetter)`).
2. Accumulate accidental chars into an `Accidental` (or directly into the int offset).
3. Delegate to the typed `Create`.

This removes the two duplicated `C => 0, D => 2, …` switches and the three duplicated `#/b` scans.

### ToString round-trip

Override `ToString()` to emit the canonical notation that `Parse` accepts, so `Parse(x.ToString())` reproduces `x`:

- `Pitch.ToString()` → the existing `Name` (e.g. `"C4"`, `"F#3"`). `Name` property is retained.
- `Mode.ToString()` → the canonical lower-case `Name` (e.g. `"dorian"`, `"harmonic_minor"`).
- `Form.ToString()` → the `Pattern` (e.g. `"AABA"`).

`Chord` is excluded (see non-goals).

### Mode literal centralization

Today the 30 canonical names appear twice: as `Shapes` dictionary keys and as string literals in the 30 static properties. Centralize so they cannot drift — e.g. each static property is defined in terms of a single source (private consts or a shared internal factory keyed off the dictionary). The public surface (30 constants + `Parse`/`TryParse`) is unchanged.

## Exception behavior (judgment call)

`Parse` throws `FormatException` on unparseable text, matching the BCL `Parse` convention the rename aligns to (`int.Parse`, `DateTime.Parse`). This is treated as the most specific exception type for a format-parse failure, taking precedence over the general "prefer `ArgumentException`" guidance in CLAUDE.md (which targets argument validation, not text parsing). `null` input still throws `ArgumentNullException` via `Ensure.NotNull`. `TryParse` throws nothing and returns `false`.

## Blast radius

19 call sites of `FromName`/`FromPattern` across 8 files:

- Production: `Chord.cs` (2 — `Pitch.FromName` inside `Voice`).
- Tests: `PitchTests.cs` (4), `ModeTests.cs` (3), `FormTests.cs` (2), `FrequencyBridgeTests.cs` (1), `ScorePrimitivesTests.cs` (1).
- Docs: `Semantics.Music/README.md` (3), `docs/superpowers/plans/2026-07-01-music-analysis-aggregate.md` (3).

All updated to the new names. README examples updated to also show the typed `Create` factories.

## Testing

- Round-trip tests: `Parse(x.ToString()) == x` for `Pitch`, `Mode`, `Form` across representative values (naturals, sharps, flats, all mode families, named + irregular form patterns).
- Typed-factory equivalence: `Pitch.Create(NoteLetter.C, Accidental.Sharp, 4) == Pitch.Parse("C#4")`, and the `PitchClass` overload against known values.
- `TryParse` returns `false` (no throw) for malformed input; `Parse` throws `FormatException` for the same input and `ArgumentNullException` for `null`.
- Enum-cast invariants: `(int)NoteLetter.X` equals the natural pitch class; `(int)Accidental.Y` equals the semitone offset.
- Existing Music tests continue to pass after the rename.

## Multi-targeting note

Plain static `Parse(string)` / `TryParse(string, out T)` methods only — do **not** implement `IParsable<T>` (net7+ only; Music targets down to netstandard2.0 and the repo avoids conditional compilation).
