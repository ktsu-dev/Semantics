# Music analysis aggregate layer — design

Status: approved (2026-07-01). Phase 1 of `docs/roadmap-semantic-domains.md`, scoped to **analysis**.
Read alongside the existing `Semantics.Music` primitives and `CLAUDE.md`.

## Motivation

`Semantics.Music` ships a deep set of primitives — `Pitch`, `PitchClass`, `Interval`, `Scale`/`Mode`,
`Key` (with roman-numeral ↔ chord resolution), `Chord` (rich parse/voice/transpose), `Duration`,
`TimeSignature`, `Tempo`, `Velocity`, and `Note`/`Rest` as `IMusicalEvent`s — but nothing above the
single event. There is no way to hold a chord *progression*, label the *sections* of a piece, describe
its *arrangement*, or name its *form*.

The roadmap's Phase 1 sketched a playback/notation container layer (`Score → Track → Measure → Voice`,
plus `Tuning`, `Clef`, `Articulation`, `Dynamic`). This spec deliberately reframes Phase 1 around
**analysis** instead: the container layer models *harmony nested inside structure* and computes
functional-harmony and formal analysis. Playback and notation concerns (tempo/seconds timing, tuning
systems, clefs, articulations, dynamics) are **out of scope** for this work.

## Scope

In scope — four container types plus their analyses, added to `Semantics.Music`:

```
Arrangement          the piece's roadmap: an ordered realization of sections (repeats allowed)
  └─ Section         a labeled unit — Intro / Verse / Chorus / Bridge / Solo / Coda …
       └─ Progression   the harmonic content: an ordered chord sequence with harmonic rhythm
Form                 the abstract pattern extracted from the sections — AABA, 12-bar blues, ternary …
```

The five analyses live on `Progression`:

1. Batch **roman-numeral** labeling relative to a `Key`.
2. **Functional classification** (tonic / predominant / dominant / chromatic).
3. **Cadence detection** (authentic / plagal / half / deceptive).
4. **Key inference** (rank candidate keys by diatonic fit).
5. **Chromatic identification** (secondary dominant / borrowed / Neapolitan / chromatic).

Out of scope: real-time (tempo/seconds) timing, tuning/temperament systems, clefs, key signatures as
engraving objects, articulations, dynamics, `Score`/`Track`/`Measure`/`Voice` performance containers,
and any MIDI/MusicXML I/O.

## Conventions

- All types added to the existing `Semantics.Music` package (namespace `ktsu.Semantics.Music`); the
  target matrix is unchanged (`net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`). No new package,
  no new dependency — the analysis layer builds only on the existing primitives.
- Every new type is an immutable `sealed record` with static `Create`/`Parse` factories and
  `Ensure.NotNull` guards, matching the existing Music primitives.
- Error handling follows the local Music convention: `FormatException` for parse failures,
  `ArgumentOutOfRangeException` for out-of-range values, `ArgumentException` for other validation.
  Empty collections are rejected (a `Progression`, `Section`, and `Arrangement` must each hold at
  least one element).
- File headers, XML documentation on all public APIs, tabs, CRLF + UTF-8-no-BOM + trailing newline.
- Tests follow repo conventions: MSTest, `Assert.ThrowsExactly<T>`, explicit types (no `var`), no
  `using System;`/MSTest usings in test files (global usings cover them), semantic asserts.

## Layer 1 — `Progression`

### `ChordEvent`

```csharp
public sealed record ChordEvent : IMusicalEvent
{
    public Chord Chord { get; }
    public Duration Duration { get; }   // harmonic rhythm — how long this chord sounds
    public static ChordEvent Create(Chord chord, Duration duration);
}
```

Implements the existing `IMusicalEvent` (which requires only `Duration`), so a chord change slots into
the same rhythmic contract as `Note`/`Rest`.

### `Progression`

```csharp
public sealed record Progression
{
    public IReadOnlyList<ChordEvent> Chords { get; }
    public TimeSignature TimeSignature { get; }   // default 4/4 — interprets durations as bars/beats
    public Duration TotalDuration { get; }
    public double TotalBars { get; }

    public static Progression Create(IEnumerable<ChordEvent> chords);
    public static Progression Create(IEnumerable<ChordEvent> chords, TimeSignature timeSignature);
    public static Progression Create(IEnumerable<Chord> chords, Duration each);   // uniform harmonic rhythm
    public static Progression Parse(string text);                                 // "Dm7 | G7 | Cmaj7"
    public static Progression Parse(string text, TimeSignature timeSignature);
}
```

`Parse` is the ergonomic analysis entry point: `|` is a barline, spaces separate chords within a bar,
and a bar's chords split its `BarDuration` evenly. Each token is parsed by the existing `Chord.Parse`.
`"Dm7 | G7 | Cmaj7"` in 4/4 yields three whole-bar chords; `"C G | Am F"` yields four half-bar chords.

### Analysis API on `Progression`

```csharp
public IReadOnlyList<string> RomanNumerals(Key key);
public IReadOnlyList<HarmonicFunction> Functions(Key key);
public IReadOnlyList<CadenceInstance> Cadences(Key key);
public IReadOnlyList<KeyMatch> InferKeys();
public Key? InferKey();
public IReadOnlyList<ChromaticAnalysis> ChromaticChords(Key key);
```

**Roman numerals** — one per `ChordEvent`, delegating to the existing `Key.RomanNumeralOf(Chord)`.

**Functional classification**

```csharp
public enum HarmonicFunction { Tonic, Predominant, Dominant, Chromatic }
```

Assigned by the chord root's scale degree in `key` (via `Key.FunctionOf`):

| Degree (diatonic) | Function |
|---|---|
| I, iii, vi | Tonic |
| ii, IV | Predominant |
| V, vii° | Dominant |
| non-diatonic root | Chromatic |

**Cadence detection**

```csharp
public enum Cadence { Authentic, Plagal, Half, Deceptive }
public sealed record CadenceInstance   // Index = position of the resolution chord
{
    public int Index { get; }
    public Cadence Type { get; }
}
```

Classified from the functional motion of each adjacent chord pair (degree-based, so it does not depend
on seventh/extension colour):

| Motion (root degrees) | Cadence |
|---|---|
| V → I | Authentic |
| IV → I | Plagal |
| any → V (as the phrase target) | Half |
| V → vi | Deceptive |

`Half` is reported when the pair *ends* on V (a dominant arrival); the four types are mutually
exclusive per pair, tested in the order Authentic, Deceptive, Plagal, Half. PAC/IAC are **not**
distinguished — that needs soprano/voice-leading information the model does not carry.

**Key inference**

```csharp
public sealed record KeyMatch { public Key Key { get; } public double Score { get; } }
```

Scores every candidate `Key` — 12 tonics × {major (`Mode.Major`), natural minor (`Mode.Aeolian`)} — by
the fraction of chords whose root is diatonic to that key (ties broken by preferring a tonic-rooted
first/last chord, then major over minor). `InferKeys()` returns matches ranked by descending score;
`InferKey()` returns the single best (or `null` for a degenerate all-chromatic input).

**Chromatic identification**

```csharp
public enum ChromaticKind { SecondaryDominant, BorrowedChord, Neapolitan, AugmentedSixth, Chromatic }
public sealed record ChromaticAnalysis   // Index = chord position
{
    public int Index { get; }
    public ChromaticKind Kind { get; }
    public string? Detail { get; }        // e.g. "V/V" for a secondary dominant of the dominant
}
```

Only chords **not** diatonic to `key` are analyzed. Classification, in priority order:

- **SecondaryDominant** — a dominant-quality chord (major triad or dominant 7th) whose root is a
  perfect fifth above a diatonic degree of `key`; `Detail` names the target (e.g. `"V/V"`, `"V/ii"`).
- **Neapolitan** — a major triad on the lowered second degree (`bII`).
- **BorrowedChord** — otherwise diatonic to the *parallel* mode (major↔minor) of the same tonic;
  `Detail` names the source (e.g. `"iv (from parallel minor)"`).
- **Chromatic** — anything else (the honest fallback).

`AugmentedSixth` is declared in the enum for completeness but detection is a **stretch goal**: the
current `Chord` model stores pitch classes and quality, not the specific interval spelling
(e.g. the augmented sixth against the bass) needed to identify It/Fr/Ger sixths reliably. The initial
implementation will not emit `AugmentedSixth`; such chords fall through to `Chromatic`. This limitation
is documented rather than faked.

## Layer 2 — `Section`

```csharp
public enum SectionType
{
    Intro, Verse, PreChorus, Chorus, PostChorus, Bridge, Solo, Interlude, Refrain, Outro, Coda, Other
}

public sealed record Section
{
    public SectionType Type { get; }
    public string? Label { get; }        // human label distinguishing repeats, e.g. "Verse 1"
    public Progression Progression { get; }
    public Key? Key { get; }             // optional local key for a section that modulates
    public double Bars { get; }          // derived from Progression.TotalBars

    public static Section Create(SectionType type, Progression progression, string? label = null, Key? key = null);

    public bool IsSameStructure(Section other);   // Type + progression harmonic content; ignores Label/Key
}
```

`IsSameStructure` is what `Form` groups on: two sections share a form-letter when they have the same
`Type` and the same ordered chord content (ignoring `Label` and local `Key`). Record value-equality
still exists but includes `Label`/`Key`, so it is not used for form grouping.

## Layer 3 — `Arrangement`

```csharp
public sealed record Arrangement
{
    public Key Key { get; }                          // home key for whole-piece analysis
    public IReadOnlyList<Section> Sections { get; }   // in performance order; repeats = repeated entries
    public double TotalBars { get; }
    public Form Form { get; }                         // => Form.Of(this)

    public static Arrangement Create(Key key, IEnumerable<Section> sections);
}
```

The `Arrangement` is the top-level analysis container. Repetition and song-map semantics are expressed
by repeating `Section` entries in order (e.g. Verse, Chorus, Verse, Chorus, Bridge, Chorus). Each
section analyzes against its own `Key` if set, otherwise the arrangement `Key`.

## Layer 4 — `Form`

```csharp
public enum NamedForm
{
    ThirtyTwoBarAABA, TwelveBarBlues, VerseChorus, Binary, Ternary, Rondo, Strophic, ThroughComposed, Unknown
}

public sealed record Form
{
    public string Pattern { get; }              // e.g. "AABA"
    public IReadOnlyList<char> Letters { get; } // per-section, aligned to arrangement order
    public NamedForm? Name { get; }

    public static Form Of(Arrangement arrangement);
    public static Form FromPattern(string pattern);
}
```

`Pattern` is assigned by walking the arrangement's sections in order and giving each distinct
`IsSameStructure` group the next unused letter (A, B, C…). `NamedForm` recognition combines two kinds
of template:

- **Section-letter patterns** — `Binary` = `AB`, `Ternary` = `ABA`, `ThirtyTwoBarAABA` = `AABA`
  (with the four sections each ~8 bars), `Rondo` = `ABACA`/`ABACABA`, `Strophic` = all identical
  (`AAA…`), `ThroughComposed` = all distinct (no letter repeats), else `Unknown`.
- **Progression-template match** — `TwelveBarBlues` is recognized when a section's progression matches
  the 12-bar blues roman-numeral template (I–I–I–I–IV–IV–I–I–V–IV–I–V, allowing common variants) over
  12 bars; this is a *progression* form, so it is detected from a section's harmonic content rather
  than the section-letter pattern.

`FromPattern` builds a `Form` directly from a letter string (for callers who have a pattern but no
arrangement) and applies the section-letter recognition only.

## Testing

New test files under `Semantics.Test/Music/`, each with concrete musical examples:

- `ProgressionTests` — construction, `Parse` bar syntax, `TotalBars`, empty rejection.
- `HarmonicFunctionTests` — I/ii/V/vi function mapping in major and minor.
- `CadenceTests` — authentic (V–I), plagal (IV–I), half (→V), deceptive (V–vi) in C major.
- `KeyInferenceTests` — ii–V–I in C infers C major; a diatonic minor progression infers the minor key.
- `ChromaticAnalysisTests` — secondary dominant (D7→G7 in C = V/V), borrowed iv (Fm in C), Neapolitan
  (Db major = bII in C), and a chromatic fallback.
- `SectionTests` — `IsSameStructure` equivalence ignoring `Label`/`Key`; bar length.
- `ArrangementTests` — ordered sections, per-section key override, `TotalBars`.
- `FormTests` — `AABA` pattern + `ThirtyTwoBarAABA`, `ABA` ternary, `AB` binary, strophic, 12-bar blues
  progression-template recognition, `FromPattern`.

## Documentation

Finish with the `update-docs` skill: extend the README Music section with the aggregate/analysis layer,
add a CLAUDE.md project-layout note that `Semantics.Music` now includes the analysis containers, and
refresh DESCRIPTION/TAGS as needed.

## Implementation sequencing

One spec, one branch, milestoned in the plan:

1. `Progression` + `ChordEvent` + roman numerals + functional classification + cadence detection.
2. Key inference + chromatic identification.
3. `Section` + `Arrangement` + `Form` (section-letter recognition, then the 12-bar-blues template).

Each milestone lands with its tests green before the next begins.
