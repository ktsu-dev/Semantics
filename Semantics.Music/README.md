# ktsu.Semantics.Music

> Immutable, validated musical value types plus a harmonic and structural analysis layer, all pure logic with no I/O.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Music?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Music)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Music?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Music)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Music?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Music)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Music` is one package in the [ktsu.Semantics](../README.md) family. Start at the [root README](../README.md) for the family overview.

## Introduction

`ktsu.Semantics.Music` models music theory as immutable value types. Pitches, intervals, scales, modes, chords, keys, rational durations, and time signatures are all first-class, validate on creation, and return new instances on every operation. The pitch convention is MIDI 60 = C4, and equal temperament is anchored at A4 = 440 Hz.

Above the single-event types sits an analysis layer that models harmony nested inside structure. A `Progression` is an ordered chord sequence with bar-based harmonic rhythm that computes roman numerals, functional roles, cadences, an inferred key, and chromatic classifications. `Section`s group progressions, an `Arrangement` orders them into a piece, and `Form` names the structural pattern.

## Features

- **Pitch and interval types**: `PitchClass`, `Pitch` (MIDI, with name and frequency conversion), `Interval` (signed semitones, cents, folding).
- **Scales and modes**: `Mode` with roughly 29 presets (diatonic, jazz, symmetric, pentatonic, blues), `Scale` rooting a mode at a pitch class, with `Contains` and `DegreeOf`.
- **Chord-symbol parsing**: `Chord.Parse` handles triads, sixths, sevenths (including `m7b5` and `mmaj7`), extensions and altered tensions (`9`/`11`/`13`, `b9`/`#9`/`#11`/`b13`), suspensions, power chords, omissions (`no3`/`no5`), and slash bass.
- **Chord realization**: `ChordTones()` and `Voice(octave)` / `Voice(octave, inversion)`, plus `Transpose`.
- **Roman-numeral analysis both directions**: `Key.RomanNumeralOf(chord)` and `Key.ChordFromRomanNumeral(numeral)`.
- **Rhythm and real time**: rational `Duration`, `TimeSignature`, `Tempo`, and `Note` / `Rest` / `ChordEvent` events that convert to seconds.
- **Analysis layer**: `Progression` (roman numerals, functions, cadences, key inference, chromatic chords), `Section`, `Arrangement`, and `Form` with named-form recognition.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Music
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Music
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Music" Version="x.y.z" />
```

## Usage Examples

### Pitches, intervals, scales

```csharp
using ktsu.Semantics.Music;

Pitch middleC = Pitch.Parse("C4");              // MIDI 60
Pitch cSharp4 = Pitch.Create(NoteLetter.C, Accidental.Sharp, octave: 4); // type-safe, no parsing
Pitch g4 = middleC.Transpose(7);                // a perfect fifth up
Interval fifth = Interval.Between(middleC, g4); // +7 semitones
double hz = Pitch.Parse("A4").FrequencyHz;      // 440.0

Scale dDorian = Scale.Create(PitchClass.Create(2), Mode.Dorian);
bool hasF = dDorian.Contains(PitchClass.Create(5));   // true
```

### Chord parsing, voicing, and rhythm

```csharp
using ktsu.Semantics.Music;

Chord cmaj7 = Chord.Parse("Cmaj7");
IReadOnlyList<Pitch> voicing = cmaj7.Voice(octave: 4);   // C4, E4, G4, B4
IReadOnlyList<int> tones = Chord.Parse("Cmaj7#11").ChordTones();
Chord up = cmaj7.Transpose(2);                           // Dmaj7

Tempo tempo = Tempo.Create(120.0);                       // quarter = 120 bpm
double halfNoteSeconds = tempo.Seconds(Duration.Half);   // 1.0 s
Note a4 = Note.Create(Pitch.Parse("A4"), Duration.Quarter, Velocity.Forte);
double noteSeconds = a4.Seconds(tempo);                  // 0.5 s
```

### Roman numerals, both directions

```csharp
using ktsu.Semantics.Music;

Key cMajor = Key.Create(PitchClass.Create(0), Mode.Major);

string label = cMajor.RomanNumeralOf(Chord.Parse("Dm7"));  // "ii7"
Chord five = cMajor.ChordFromRomanNumeral("V7");           // G7
```

### Analysis: progressions, cadences, key inference, and form

```csharp
using ktsu.Semantics.Music;

// Chart style: a leading time signature, bars separated by "|", and a beat slash "/"
// extends the preceding chord by one beat (so "Cmaj7 / / /" is a whole bar of 4/4).
Progression prog = Progression.Parse("4/4  Dm7 / / / | G7 / / / | Cmaj7 / / / | Cmaj7 / / /");

Key key = prog.InferKey()!;                                   // C major (quality-weighted fit)
IReadOnlyList<string> roman = prog.RomanNumerals(key);        // ii7, V7, Imaj7, Imaj7
IReadOnlyList<HarmonicFunction> fns = prog.Functions(key);    // Predominant, Dominant, Tonic, Tonic
IReadOnlyList<CadenceInstance> cadences = prog.Cadences(key); // Authentic at the resolution

// Chromatic analysis: secondary dominants, borrowed chords, Neapolitan
IReadOnlyList<ChromaticAnalysis> chromatic =
    Progression.Parse("4/4  C / / / | A7 / / / | Dm / / / | G7 / / /").ChromaticChords(key); // A7 -> secondary dominant

// Structure: sections -> arrangement -> form
Progression verse  = Progression.Parse("4/4  C / / / | G / / / | Am / / / | F / / /");
Progression bridge = Progression.Parse("4/4  F / / / | C / / / | G / / / | C / / /");
Arrangement song = Arrangement.Create(key,
[
    Section.Create(SectionType.Verse,  verse,  "Verse 1"),
    Section.Create(SectionType.Verse,  verse,  "Verse 2"),
    Section.Create(SectionType.Bridge, bridge, "Bridge"),
    Section.Create(SectionType.Verse,  verse,  "Verse 3"),
]);
Form form = song.Form;   // Pattern "AABA", Name NamedForm.ThirtyTwoBarAABA
```

### Type-safe construction and round-trippable text

```csharp
using ktsu.Semantics.Music;

// Compiler-enforced construction — no strings to mistype
Pitch p = Pitch.Create(NoteLetter.C, Accidental.Sharp, octave: 4); // C#4

// Every in-scope type has a canonical ToString() and a Parse/TryParse that inverts it
Chord c = Chord.Parse("Cmaj7");
Chord same = Chord.Parse(c.ToString());   // c == same

// Progressions read like a lead sheet and round-trip through their chart form
Progression prog = Progression.Parse("4/4  Dm7 / G7 / | Cmaj7 / / /");
Progression reparsed = Progression.Parse(prog.ToString());   // prog == reparsed

// TryParse never throws; Parse throws FormatException on malformed text
bool ok = Interval.TryParse("-5", out Interval? descendingFourth);
```

## API Reference

### Core value types

| Type | Description | Key factories / members |
|------|-------------|-------------------------|
| `PitchClass` | One of twelve pitch classes, folded to 0..11. | `Create(int)`, `Create(NoteLetter, Accidental)`, `Parse`/`TryParse`, `Value`, `Name` |
| `Pitch` | MIDI pitch 0..127 (60 = C4). | `Create(int)`, `Create(NoteLetter, Accidental, int)`, `Parse`/`TryParse`, `FromFrequency(double)`, `Transpose(int)`, `FrequencyHz`, `Octave` |
| `Interval` | Signed interval in semitones. | `Create(int)`, `Between(Pitch, Pitch)`, `Parse`/`TryParse`, `Semitones`, `Cents`, `Folded` |
| `Mode` | Scale shape by semitone offsets. | `Parse`/`TryParse`, presets (`Major`, `Dorian`, `HarmonicMinor`, `WholeTone`, `MajorPentatonic`, ...), `Intervals` |
| `Scale` | A mode rooted at a pitch class. | `Create(PitchClass, Mode)`, `Parse`/`TryParse`, `Contains`, `DegreeOf`, `Transpose` |
| `Chord` | Chord parsed from a symbol. | `Parse`/`TryParse`, `ChordTones()`, `Voice(octave)`, `Voice(octave, inversion)`, `Transpose` |
| `Key` | Tonic in a mode, resolves function. | `Create(PitchClass, Mode)`, `Parse`/`TryParse`, `RomanNumeralOf(Chord)`, `ChordFromRomanNumeral(string)`, `FunctionOf`, `Scale` |
| `Duration` | Exact rational fraction of a whole note. | `Create(int, int)`, `Parse`/`TryParse`, presets (`Whole`..`Sixteenth`), `Dotted()`, `Add`, `Multiply`, `AsWholeNotes` |
| `TimeSignature` | Bar and beat lengths. | `Create(int, int)`, `Parse`/`TryParse`, `BarDuration`, `BeatDuration` |
| `Note` / `Rest` / `ChordEvent` | Timed musical events (`IMusicalEvent`). | `Create(...)`, `Parse`/`TryParse`, `Duration`, `Seconds(Tempo)` |
| `Velocity` | MIDI velocity 0..127. | `Create(int)`, `Parse`/`TryParse`, presets (`Piano`..`Fortissimo`) |
| `Tempo` | Beats per minute with a beat unit. | `Create(double)`, `Parse`/`TryParse`, `Seconds(Duration)`, `SecondsPerBeat` |

### Analysis layer

| Type | Description | Key members |
|------|-------------|-------------|
| `Progression` | Chord sequence with bar-based harmonic rhythm. | `Parse(string)`, `Create(...)`, `RomanNumerals(Key)`, `Functions(Key)`, `Cadences(Key)`, `InferKey()`, `InferKeys()`, `ChromaticChords(Key)` |
| `Section` | Labeled structural unit. | `Create(SectionType, Progression, label?, key?)`, `Parse`/`TryParse`, `IsSameStructure(Section)` |
| `Arrangement` | Sections in performance order. | `Create(Key, IEnumerable<Section>)`, `Parse`/`TryParse`, `Form`, `TotalBars` |
| `Form` | Structural pattern and its name. | `Of(Arrangement)`, `Parse`/`TryParse`, `Pattern`, `Name` |
| `HarmonicFunction` | enum: Tonic, Predominant, Dominant, Chromatic. | |
| `CadenceInstance` | A cadence at a resolution index. | `Index`, `Type` (`Cadence` enum: Authentic, Plagal, Half, Deceptive) |
| `ChromaticAnalysis` | A non-diatonic chord classification. | `Index`, `Kind` (`ChromaticKind`), `Detail` |
| `NamedForm` | enum: ThirtyTwoBarAABA, TwelveBarBlues, Binary, Ternary, Rondo, Strophic, ... | |

`InferKey()` returns `Key?` (null for all-chromatic input), while `InferKeys()` returns the full ranked `IReadOnlyList<KeyMatch>`. See the [complete library guide](../docs/complete-library-guide.md) for a wider tour.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
