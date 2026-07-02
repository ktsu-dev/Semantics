# Music Analysis Aggregate Layer Implementation Plan

> **Superseded (2026-07-02):** The `Pitch.FromName`, `Mode.FromName`, and `Form.FromPattern` entry points referenced below were renamed to `Parse`/`TryParse`, and the bar-delimited `Progression.Parse` format was replaced with a chart-style format, by the type-safe-factories work — see `2026-07-02-music-type-safe-factories.md`. Snippets below record the API as it was when this plan was executed.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an analysis-oriented aggregate layer to `Semantics.Music` — `Progression` (chords + functional-harmony analysis), `Section`, `Arrangement`, and `Form` — building only on the existing primitives.

**Architecture:** Four immutable `sealed record` container types in the existing `ktsu.Semantics.Music` namespace. `Progression` is a partial record whose analyses (roman numerals, functional classification, cadences, key inference, chromatic identification) are split across focused partial files. `Section` wraps a `Progression` with a role; `Arrangement` orders sections; `Form` derives the structural letter-pattern and recognizes named forms. No new package, no new dependency.

**Tech Stack:** C# (LangVersion ≥ 12 — collection expressions and list patterns are already in use), MSTest, `Ensure.NotNull` from Polyfill. Multi-target `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`.

## Global Constraints

- All types added to the existing `Semantics.Music/` project; namespace `ktsu.Semantics.Music`. No `.csproj` changes; the test project already references `Semantics.Music`.
- File header on every source file:
  ```csharp
  // Copyright (c) ktsu.dev
  // All rights reserved.
  // Licensed under the MIT license.
  ```
- File-scoped namespace; `using` directives **inside** the namespace (match existing Music files).
- Tabs for indentation; CRLF line endings; UTF-8 **no BOM**; trailing newline. (The Write tool emits LF — after writing any `.cs` file, verify it is CRLF + no-BOM: first two bytes `2f 2f`, last two bytes `0d 0a`.)
- Immutable `sealed record` with static `Create`/`Parse` factories; `Ensure.NotNull` on reference parameters; no `this.` qualifiers; explicit accessibility; nullable enabled; warnings-as-errors.
- Error handling: `FormatException` for parse failures, `ArgumentOutOfRangeException` for out-of-range values, `ArgumentException` for other validation. A `Progression`, `Section`, and `Arrangement` must each be non-empty.
- Tests: MSTest, `Assert.ThrowsExactly<T>` (never `Assert.ThrowsException`), explicit types (no `var`), **no** `using System;` or MSTest usings in test files (global usings cover them), only `using ktsu.Semantics.Music;`. Use semantic asserts (`Assert.AreEqual`, `CollectionAssert`) not `Assert.IsTrue` on equality.
- Build after each task with `dotnet build` and run tests with `dotnet test`. Multi-target builds are slow — allow generous timeouts.

---

## Milestone 1 — Progression core + roman numerals, functions, cadences

### Task 1: `ChordEvent`

**Files:**
- Create: `Semantics.Music/ChordEvent.cs`
- Test: `Semantics.Test/Music/ProgressionTests.cs` (created here, extended in later tasks)

**Interfaces:**
- Consumes: existing `Chord`, `Duration`, `IMusicalEvent`.
- Produces: `ChordEvent.Create(Chord chord, Duration duration) -> ChordEvent`; properties `Chord Chord`, `Duration Duration`.

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/ProgressionTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ProgressionTests
{
	[TestMethod]
	public void ChordEvent_BundlesChordAndDuration_AndIsMusicalEvent()
	{
		ChordEvent chordEvent = ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Half);
		Assert.AreEqual(0, chordEvent.Chord.Root.Value);
		Assert.AreEqual(Duration.Half, chordEvent.Duration);
		IMusicalEvent asEvent = chordEvent;
		Assert.AreEqual(Duration.Half, asEvent.Duration);
	}

	[TestMethod]
	public void ChordEvent_RejectsNullChord() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => ChordEvent.Create(null!, Duration.Half));
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: FAIL — `ChordEvent` does not exist (compile error).

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/ChordEvent.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>
/// A harmonic event: a chord sounding for a rhythmic duration (the harmonic rhythm).
/// </summary>
public sealed record ChordEvent : IMusicalEvent
{
	/// <summary>Gets the chord that sounds.</summary>
	public Chord Chord { get; private init; } = new();

	/// <summary>Gets the rhythmic duration for which the chord sounds.</summary>
	public Duration Duration { get; private init; } = Duration.Quarter;

	/// <summary>Creates a chord event from a chord and a duration.</summary>
	/// <param name="chord">The chord.</param>
	/// <param name="duration">The rhythmic duration.</param>
	/// <returns>A new chord event.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static ChordEvent Create(Chord chord, Duration duration)
	{
		Ensure.NotNull(chord);
		Ensure.NotNull(duration);
		return new() { Chord = chord, Duration = duration };
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

Confirm `Semantics.Music/ChordEvent.cs` is CRLF + no-BOM (first bytes `2f 2f`, last `0d 0a`).

```bash
git add Semantics.Music/ChordEvent.cs Semantics.Test/Music/ProgressionTests.cs
git commit -m "feat(music): add ChordEvent harmonic event type"
```

---

### Task 2: `Progression` core (construction, totals, empty rejection)

**Files:**
- Create: `Semantics.Music/Progression.cs`
- Test: `Semantics.Test/Music/ProgressionTests.cs` (extend)

**Interfaces:**
- Consumes: `ChordEvent`, `Chord`, `Duration`, `TimeSignature`.
- Produces (partial record `Progression`):
  - `IReadOnlyList<ChordEvent> Chords`, `TimeSignature TimeSignature`, `Duration TotalDuration`, `double TotalBars`.
  - `Create(IEnumerable<ChordEvent>) -> Progression`
  - `Create(IEnumerable<ChordEvent>, TimeSignature) -> Progression`
  - `Create(IEnumerable<Chord>, Duration each) -> Progression`

- [ ] **Step 1: Write the failing test** (append inside `ProgressionTests`)

```csharp
	[TestMethod]
	public void Progression_TotalBars_SumsHarmonicRhythmAgainstTimeSignature()
	{
		Progression progression = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("C"), Duration.Whole),
			ChordEvent.Create(Chord.Parse("G"), Duration.Whole),
		]);
		Assert.AreEqual(2.0, progression.TotalBars, 1e-9);
		Assert.AreEqual(2, progression.Chords.Count);
	}

	[TestMethod]
	public void Progression_Create_FromChordsWithUniformRhythm()
	{
		Progression progression = Progression.Create(
			[Chord.Parse("Dm7"), Chord.Parse("G7"), Chord.Parse("Cmaj7")], Duration.Whole);
		Assert.AreEqual(3.0, progression.TotalBars, 1e-9);
	}

	[TestMethod]
	public void Progression_RejectsEmpty() =>
		_ = Assert.ThrowsExactly<ArgumentException>(() => Progression.Create(System.Array.Empty<ChordEvent>()));
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: FAIL — `Progression` does not exist.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/Progression.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An ordered chord sequence with a harmonic rhythm, plus functional-harmony analysis.
/// </summary>
public sealed partial record Progression
{
	/// <summary>Gets the ordered chord events (the harmonic content).</summary>
	public IReadOnlyList<ChordEvent> Chords { get; private init; } = [];

	/// <summary>Gets the time signature used to interpret durations as bars and beats.</summary>
	public TimeSignature TimeSignature { get; private init; } = TimeSignature.Create(4, 4);

	/// <summary>Gets the total rhythmic length as a fraction of a whole note.</summary>
	public Duration TotalDuration =>
		Chords.Aggregate(Duration.Create(0, 1), (sum, chordEvent) => sum.Add(chordEvent.Duration));

	/// <summary>Gets the total length measured in bars of the time signature.</summary>
	public double TotalBars => TotalDuration.AsWholeNotes / TimeSignature.BarDuration.AsWholeNotes;

	/// <summary>Creates a progression in 4/4 from chord events.</summary>
	/// <param name="chords">The ordered chord events; must be non-empty.</param>
	/// <returns>A new progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="chords"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<ChordEvent> chords) => Create(chords, TimeSignature.Create(4, 4));

	/// <summary>Creates a progression from chord events with an explicit time signature.</summary>
	/// <param name="chords">The ordered chord events; must be non-empty.</param>
	/// <param name="timeSignature">The time signature.</param>
	/// <returns>A new progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<ChordEvent> chords, TimeSignature timeSignature)
	{
		Ensure.NotNull(chords);
		Ensure.NotNull(timeSignature);
		List<ChordEvent> list = [.. chords];
		if (list.Count == 0)
		{
			throw new ArgumentException("A progression must contain at least one chord.", nameof(chords));
		}

		return new() { Chords = list, TimeSignature = timeSignature };
	}

	/// <summary>Creates a progression from chords that all share one rhythmic duration.</summary>
	/// <param name="chords">The ordered chords; must be non-empty.</param>
	/// <param name="each">The duration applied to every chord.</param>
	/// <returns>A new progression in 4/4.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<Chord> chords, Duration each)
	{
		Ensure.NotNull(chords);
		Ensure.NotNull(each);
		return Create([.. chords.Select(chord => ChordEvent.Create(chord, each))]);
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/Progression.cs Semantics.Test/Music/ProgressionTests.cs
git commit -m "feat(music): add Progression core (construction, totals, empty rejection)"
```

---

### Task 3: `Progression.Parse` (bar-delimited chord syntax)

**Files:**
- Create: `Semantics.Music/Progression.Parse.cs`
- Test: `Semantics.Test/Music/ProgressionTests.cs` (extend)

**Interfaces:**
- Consumes: `Progression.Create`, `ChordEvent.Create`, `Chord.Parse`, `TimeSignature`, `Duration`.
- Produces: `Progression.Parse(string) -> Progression`; `Progression.Parse(string, TimeSignature) -> Progression`.

- [ ] **Step 1: Write the failing test** (append inside `ProgressionTests`)

```csharp
	[TestMethod]
	public void Parse_OneChordPerBar_FillsWholeBars()
	{
		Progression progression = Progression.Parse("Dm7 | G7 | Cmaj7");
		Assert.AreEqual(3, progression.Chords.Count);
		Assert.AreEqual(Duration.Whole, progression.Chords[0].Duration);
		Assert.AreEqual(3.0, progression.TotalBars, 1e-9);
	}

	[TestMethod]
	public void Parse_TwoChordsPerBar_SplitBarEvenly()
	{
		Progression progression = Progression.Parse("C G | Am F");
		Assert.AreEqual(4, progression.Chords.Count);
		Assert.AreEqual(Duration.Half, progression.Chords[0].Duration);
		Assert.AreEqual(2.0, progression.TotalBars, 1e-9);
	}

	[TestMethod]
	public void Parse_ToleratesLeadingAndTrailingBarlines()
	{
		Progression progression = Progression.Parse("| C | G |");
		Assert.AreEqual(2, progression.Chords.Count);
	}

	[TestMethod]
	public void Parse_RejectsEmptyBar() =>
		_ = Assert.ThrowsExactly<FormatException>(() => Progression.Parse("C || G"));

	[TestMethod]
	public void Parse_RejectsEmptyText() =>
		_ = Assert.ThrowsExactly<FormatException>(() => Progression.Parse("   "));
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: FAIL — `Parse` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/Progression.Parse.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;

public sealed partial record Progression
{
	/// <summary>Parses a bar-delimited chord progression in 4/4.</summary>
	/// <param name="text">Chord symbols with <c>|</c> as a barline and spaces separating chords in a bar.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is empty or a bar is empty.</exception>
	public static Progression Parse(string text) => Parse(text, TimeSignature.Create(4, 4));

	/// <summary>Parses a bar-delimited chord progression with an explicit time signature.</summary>
	/// <param name="text">Chord symbols with <c>|</c> as a barline and spaces separating chords in a bar.</param>
	/// <param name="timeSignature">The time signature; a bar's chords split its bar duration evenly.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is empty or a bar is empty.</exception>
	public static Progression Parse(string text, TimeSignature timeSignature)
	{
		Ensure.NotNull(text);
		Ensure.NotNull(timeSignature);

		string trimmed = text.Trim();
		if (trimmed.Length == 0)
		{
			throw new FormatException("Progression is empty.");
		}

		// Strip one leading and one trailing barline so "| C | G |" parses cleanly.
		if (trimmed[0] == '|')
		{
			trimmed = trimmed[1..];
		}

		if (trimmed.Length > 0 && trimmed[^1] == '|')
		{
			trimmed = trimmed[..^1];
		}

		Duration barDuration = timeSignature.BarDuration;
		List<ChordEvent> events = [];
		foreach (string bar in trimmed.Split('|'))
		{
			string[] tokens = bar.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0)
			{
				throw new FormatException("Progression has an empty bar.");
			}

			Duration each = Duration.Create(barDuration.Numerator, barDuration.Denominator * tokens.Length);
			foreach (string token in tokens)
			{
				events.Add(ChordEvent.Create(Chord.Parse(token), each));
			}
		}

		return Create(events, timeSignature);
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/Progression.Parse.cs Semantics.Test/Music/ProgressionTests.cs
git commit -m "feat(music): add Progression.Parse bar-delimited chord syntax"
```

---

### Task 4: Roman numerals + functional classification

**Files:**
- Create: `Semantics.Music/HarmonicFunction.cs`
- Create: `Semantics.Music/Progression.Analysis.cs`
- Test: `Semantics.Test/Music/HarmonicFunctionTests.cs`

**Interfaces:**
- Consumes: `Key.RomanNumeralOf(Chord)`, `Key.FunctionOf(PitchClass) -> ScaleDegree`, `ScaleDegree.Degree`, `ScaleDegree.Alteration`.
- Produces:
  - `enum HarmonicFunction { Tonic, Predominant, Dominant, Chromatic }`
  - `Progression.RomanNumerals(Key key) -> IReadOnlyList<string>`
  - `Progression.Functions(Key key) -> IReadOnlyList<HarmonicFunction>`

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/HarmonicFunctionTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class HarmonicFunctionTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void RomanNumerals_LabelsTwoFiveOne()
	{
		Progression progression = Progression.Parse("Dm7 | G7 | Cmaj7");
		System.Collections.Generic.IReadOnlyList<string> numerals = progression.RomanNumerals(CMajor);
		CollectionAssert.AreEqual(new[] { "ii7", "V7", "Imaj7" }, (System.Collections.Generic.List<string>)[.. numerals]);
	}

	[TestMethod]
	public void Functions_ClassifyDiatonicChords()
	{
		Progression progression = Progression.Parse("C | Dm | G | Am");
		System.Collections.Generic.IReadOnlyList<HarmonicFunction> functions = progression.Functions(CMajor);
		Assert.AreEqual(HarmonicFunction.Tonic, functions[0]);      // I
		Assert.AreEqual(HarmonicFunction.Predominant, functions[1]); // ii
		Assert.AreEqual(HarmonicFunction.Dominant, functions[2]);    // V
		Assert.AreEqual(HarmonicFunction.Tonic, functions[3]);       // vi
	}

	[TestMethod]
	public void Functions_MarksChromaticRoot()
	{
		Progression progression = Progression.Parse("C | Db");
		System.Collections.Generic.IReadOnlyList<HarmonicFunction> functions = progression.Functions(CMajor);
		Assert.AreEqual(HarmonicFunction.Chromatic, functions[1]);
	}
```

Close the class:

```csharp
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~HarmonicFunctionTests"`
Expected: FAIL — `HarmonicFunction`/`Functions`/`RomanNumerals` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/HarmonicFunction.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The functional role a chord plays within a key.</summary>
public enum HarmonicFunction
{
	/// <summary>Tonic function (scale degrees I, iii, vi).</summary>
	Tonic,

	/// <summary>Predominant / subdominant function (scale degrees ii, IV).</summary>
	Predominant,

	/// <summary>Dominant function (scale degrees V, vii).</summary>
	Dominant,

	/// <summary>A chromatic (non-diatonic) chord with no diatonic function.</summary>
	Chromatic,
}
```

```csharp
// Semantics.Music/Progression.Analysis.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	/// <summary>Returns the roman-numeral function of each chord relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One roman numeral per chord, in order.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<string> RomanNumerals(Key key)
	{
		Ensure.NotNull(key);
		return [.. Chords.Select(chordEvent => key.RomanNumeralOf(chordEvent.Chord))];
	}

	/// <summary>Returns the harmonic function of each chord relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One function per chord, in order.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<HarmonicFunction> Functions(Key key)
	{
		Ensure.NotNull(key);
		return [.. Chords.Select(chordEvent => FunctionOf(key, chordEvent.Chord))];
	}

	/// <summary>Classifies a single chord's function in a key by its root scale degree.</summary>
	private static HarmonicFunction FunctionOf(Key key, Chord chord)
	{
		ScaleDegree degree = key.FunctionOf(chord.Root);
		if (degree.Alteration != 0)
		{
			return HarmonicFunction.Chromatic;
		}

		return degree.Degree switch
		{
			1 or 3 or 6 => HarmonicFunction.Tonic,
			2 or 4 => HarmonicFunction.Predominant,
			5 or 7 => HarmonicFunction.Dominant,
			_ => HarmonicFunction.Chromatic,
		};
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~HarmonicFunctionTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/HarmonicFunction.cs Semantics.Music/Progression.Analysis.cs Semantics.Test/Music/HarmonicFunctionTests.cs
git commit -m "feat(music): add roman-numeral labeling and functional classification"
```

---

### Task 5: Cadence detection

**Files:**
- Create: `Semantics.Music/Cadence.cs`
- Create: `Semantics.Music/CadenceInstance.cs`
- Create: `Semantics.Music/Progression.Cadences.cs`
- Test: `Semantics.Test/Music/CadenceTests.cs`

**Interfaces:**
- Consumes: `Key.FunctionOf(PitchClass) -> ScaleDegree`.
- Produces:
  - `enum Cadence { Authentic, Plagal, Half, Deceptive }`
  - `CadenceInstance.Create(int index, Cadence type) -> CadenceInstance`; properties `int Index`, `Cadence Type`.
  - `Progression.Cadences(Key key) -> IReadOnlyList<CadenceInstance>`

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/CadenceTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class CadenceTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void Cadences_DetectsAuthentic()
	{
		System.Collections.Generic.IReadOnlyList<CadenceInstance> cadences =
			Progression.Parse("G | C").Cadences(CMajor);
		Assert.AreEqual(1, cadences.Count);
		Assert.AreEqual(Cadence.Authentic, cadences[0].Type);
		Assert.AreEqual(1, cadences[0].Index);
	}

	[TestMethod]
	public void Cadences_DetectsPlagalHalfAndDeceptive()
	{
		Assert.AreEqual(Cadence.Plagal, Progression.Parse("F | C").Cadences(CMajor)[0].Type);
		Assert.AreEqual(Cadence.Half, Progression.Parse("C | G").Cadences(CMajor)[0].Type);
		Assert.AreEqual(Cadence.Deceptive, Progression.Parse("G | Am").Cadences(CMajor)[0].Type);
	}

	[TestMethod]
	public void Cadences_ReportsNoneForNonCadentialMotion()
	{
		System.Collections.Generic.IReadOnlyList<CadenceInstance> cadences =
			Progression.Parse("C | Am").Cadences(CMajor);
		Assert.AreEqual(0, cadences.Count);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~CadenceTests"`
Expected: FAIL — `Cadence`/`CadenceInstance`/`Cadences` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/Cadence.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A harmonic cadence type, classified by the scale-degree motion into the final chord.</summary>
public enum Cadence
{
	/// <summary>Authentic cadence: V to I.</summary>
	Authentic,

	/// <summary>Plagal cadence: IV to I.</summary>
	Plagal,

	/// <summary>Half cadence: any chord arriving on V.</summary>
	Half,

	/// <summary>Deceptive cadence: V to vi.</summary>
	Deceptive,
}
```

```csharp
// Semantics.Music/CadenceInstance.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>A cadence located within a progression at the position of its resolution chord.</summary>
public sealed record CadenceInstance
{
	/// <summary>Gets the index of the resolution (second) chord of the cadence.</summary>
	public int Index { get; private init; }

	/// <summary>Gets the cadence type.</summary>
	public Cadence Type { get; private init; }

	/// <summary>Creates a cadence instance.</summary>
	/// <param name="index">The zero-based index of the resolution chord; must be non-negative.</param>
	/// <param name="type">The cadence type.</param>
	/// <returns>A new cadence instance.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
	public static CadenceInstance Create(int index, Cadence type)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be non-negative.");
		}

		return new() { Index = index, Type = type };
	}
}
```

```csharp
// Semantics.Music/Progression.Cadences.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;

public sealed partial record Progression
{
	/// <summary>Detects cadences from the scale-degree motion of each adjacent chord pair in a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>The cadences found, in order; empty when none are present.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<CadenceInstance> Cadences(Key key)
	{
		Ensure.NotNull(key);
		List<CadenceInstance> result = [];
		for (int i = 0; i + 1 < Chords.Count; i++)
		{
			ScaleDegree from = key.FunctionOf(Chords[i].Chord.Root);
			ScaleDegree to = key.FunctionOf(Chords[i + 1].Chord.Root);
			if (from.Alteration != 0 || to.Alteration != 0)
			{
				continue;
			}

			Cadence? cadence = Classify(from.Degree, to.Degree);
			if (cadence is Cadence value)
			{
				result.Add(CadenceInstance.Create(i + 1, value));
			}
		}

		return result;
	}

	private static Cadence? Classify(int from, int to) => (from, to) switch
	{
		(5, 1) => Cadence.Authentic,
		(5, 6) => Cadence.Deceptive,
		(4, 1) => Cadence.Plagal,
		(_, 5) => Cadence.Half,
		_ => null,
	};
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~CadenceTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/Cadence.cs Semantics.Music/CadenceInstance.cs Semantics.Music/Progression.Cadences.cs Semantics.Test/Music/CadenceTests.cs
git commit -m "feat(music): add cadence detection"
```

- [ ] **Step 6: Milestone 1 checkpoint — full build and test**

Run: `dotnet build` then `dotnet test`
Expected: solution builds clean across all targets; all tests pass.

---

## Milestone 2 — Key inference + chromatic identification

### Task 6: Key inference

**Files:**
- Create: `Semantics.Music/KeyMatch.cs`
- Create: `Semantics.Music/Progression.KeyInference.cs`
- Test: `Semantics.Test/Music/KeyInferenceTests.cs`

**Interfaces:**
- Consumes: `Key.Create(PitchClass, Mode)`, `Key.Scale`, `Scale.Contains(PitchClass)`, `Scale.PitchClasses`, `Key.Tonic`, `Key.Mode`, `Mode.Major`, `Mode.Aeolian`, `PitchClass.Create(int)`, `PitchClass.Value`, `Chord.Root`, `Chord.Quality`, `ChordQuality` (`Major`/`Minor`/`Diminished`/`Augmented`).
- Produces:
  - `KeyMatch.Create(Key key, double score) -> KeyMatch`; properties `Key Key`, `double Score`.
  - `Progression.InferKeys() -> IReadOnlyList<KeyMatch>` (ranked, best first)
  - `Progression.InferKey() -> Key?`

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/KeyInferenceTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class KeyInferenceTests
{
	[TestMethod]
	public void InferKey_TwoFiveOne_IsCMajor()
	{
		Key? key = Progression.Parse("Dm7 | G7 | Cmaj7").InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(0, key.Tonic.Value);
		Assert.AreEqual(Mode.Major, key.Mode);
	}

	[TestMethod]
	public void InferKey_DiatonicMinorProgression_IsAMinor()
	{
		Key? key = Progression.Parse("Am | Dm | Em | Am").InferKey();
		Assert.IsNotNull(key);
		Assert.AreEqual(9, key.Tonic.Value);
		Assert.AreEqual(Mode.Aeolian, key.Mode);
	}

	[TestMethod]
	public void InferKeys_RanksBestFirst_WithTwentyFourCandidates()
	{
		System.Collections.Generic.IReadOnlyList<KeyMatch> matches =
			Progression.Parse("Dm7 | G7 | Cmaj7").InferKeys();
		Assert.AreEqual(24, matches.Count);
		Assert.IsTrue(matches[0].Score >= matches[1].Score);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~KeyInferenceTests"`
Expected: FAIL — `KeyMatch`/`InferKey`/`InferKeys` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/KeyMatch.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A candidate key paired with its diatonic-fit score for a progression.</summary>
public sealed record KeyMatch
{
	/// <summary>Gets the candidate key.</summary>
	public Key Key { get; private init; } = Key.Create(PitchClass.Create(0), Mode.Major);

	/// <summary>Gets the quality-weighted diatonic-fit score in 0..1 (1.0 = every chord is diatonic with matching triad quality).</summary>
	public double Score { get; private init; }

	/// <summary>Creates a key match.</summary>
	/// <param name="key">The candidate key.</param>
	/// <param name="score">The fit score.</param>
	/// <returns>A new key match.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public static KeyMatch Create(Key key, double score)
	{
		Ensure.NotNull(key);
		return new() { Key = key, Score = score };
	}
}
```

```csharp
// Semantics.Music/Progression.KeyInference.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	/// <summary>Ranks candidate keys (12 tonics in major and natural minor) by quality-weighted diatonic fit.</summary>
	/// <returns>All 24 candidates, best first; ties break toward a tonic-rooted last then first chord, then major.</returns>
	public IReadOnlyList<KeyMatch> InferKeys()
	{
		PitchClass firstRoot = Chords[0].Chord.Root;
		PitchClass lastRoot = Chords[^1].Chord.Root;
		List<KeyMatch> matches = [];
		foreach (Mode mode in new[] { Mode.Major, Mode.Aeolian })
		{
			for (int tonic = 0; tonic < 12; tonic++)
			{
				Key key = Key.Create(PitchClass.Create(tonic), mode);
				double total = 0.0;
				foreach (ChordEvent chordEvent in Chords)
				{
					total += ScoreChord(key, chordEvent.Chord);
				}

				matches.Add(KeyMatch.Create(key, total / Chords.Count));
			}
		}

		return
		[
			.. matches
				.OrderByDescending(match => match.Score)
				.ThenByDescending(match => match.Key.Tonic.Value == lastRoot.Value)
				.ThenByDescending(match => match.Key.Tonic.Value == firstRoot.Value)
				.ThenByDescending(match => match.Key.Mode == Mode.Major)
				.ThenBy(match => match.Key.Tonic.Value),
		];
	}

	// Scores one chord's fit to a key: 1.0 when its root is diatonic AND its triad quality matches the
	// diatonic triad at that degree, 0.5 when the root is diatonic but the quality differs (or has no
	// comparable third), 0.0 when the root is not in the scale. Quality-weighting is what distinguishes
	// a key from its relative/parallel neighbours, which share the same chord roots.
	private static double ScoreChord(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		if (!scale.Contains(chord.Root))
		{
			return 0.0;
		}

		ChordQuality? diatonic = DiatonicTriadQuality(scale, chord.Root);
		bool comparable = chord.Quality is ChordQuality.Major or ChordQuality.Minor
			or ChordQuality.Diminished or ChordQuality.Augmented;
		if (!comparable || diatonic is null)
		{
			return 1.0;
		}

		return chord.Quality == diatonic.Value ? 1.0 : 0.5;
	}

	// Returns the diatonic triad quality built on a scale degree by stacking scale thirds, or null when
	// the third/fifth do not form a recognised triad.
	private static ChordQuality? DiatonicTriadQuality(Scale scale, PitchClass root)
	{
		IReadOnlyList<PitchClass> pitchClasses = scale.PitchClasses;
		int index = -1;
		for (int i = 0; i < pitchClasses.Count; i++)
		{
			if (pitchClasses[i].Value == root.Value)
			{
				index = i;
				break;
			}
		}

		if (index < 0)
		{
			return null;
		}

		int count = pitchClasses.Count;
		int third = ((((pitchClasses[(index + 2) % count].Value - root.Value) % 12) + 12) % 12);
		int fifth = ((((pitchClasses[(index + 4) % count].Value - root.Value) % 12) + 12) % 12);
		return (third, fifth) switch
		{
			(4, 7) => ChordQuality.Major,
			(3, 7) => ChordQuality.Minor,
			(3, 6) => ChordQuality.Diminished,
			(4, 8) => ChordQuality.Augmented,
			_ => null,
		};
	}

	/// <summary>Returns the single best-fitting key, or null when no chord root is diatonic to any candidate.</summary>
	/// <returns>The best key, or null for a degenerate all-chromatic input.</returns>
	public Key? InferKey()
	{
		KeyMatch best = InferKeys()[0];
		return best.Score > 0.0 ? best.Key : null;
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~KeyInferenceTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/KeyMatch.cs Semantics.Music/Progression.KeyInference.cs Semantics.Test/Music/KeyInferenceTests.cs
git commit -m "feat(music): add key inference by diatonic fit"
```

---

### Task 7: Chromatic identification

**Files:**
- Create: `Semantics.Music/ChromaticKind.cs`
- Create: `Semantics.Music/ChromaticAnalysis.cs`
- Create: `Semantics.Music/Progression.Chromatic.cs`
- Test: `Semantics.Test/Music/ChromaticAnalysisTests.cs`

**Interfaces:**
- Consumes: `Chord.ChordTones() -> IReadOnlyList<int>`, `Chord.Root`, `Chord.Quality` (`ChordQuality.Major`), `Chord.Seventh` (`SeventhType.None`/`SeventhType.Dominant`), `Key.Scale`, `Key.Tonic`, `Key.Mode`, `Scale.Create`, `Scale.Contains`, `Scale.DegreeOf`.
- Produces:
  - `enum ChromaticKind { SecondaryDominant, BorrowedChord, Neapolitan, AugmentedSixth, Chromatic }`
  - `ChromaticAnalysis.Create(int index, ChromaticKind kind, string? detail) -> ChromaticAnalysis`; properties `int Index`, `ChromaticKind Kind`, `string? Detail`.
  - `Progression.ChromaticChords(Key key) -> IReadOnlyList<ChromaticAnalysis>`

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/ChromaticAnalysisTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ChromaticAnalysisTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void ChromaticChords_SkipsDiatonicChords()
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | Dm | G7 | C").ChromaticChords(CMajor);
		Assert.AreEqual(0, analyses.Count);
	}

	[TestMethod]
	public void ChromaticChords_DetectsSecondaryDominantOfDominant()
	{
		// D7 in C is V/V (resolving toward G, the dominant).
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | D7 | G7 | C").ChromaticChords(CMajor);
		Assert.AreEqual(1, analyses.Count);
		Assert.AreEqual(ChromaticKind.SecondaryDominant, analyses[0].Kind);
		Assert.AreEqual("V/V", analyses[0].Detail);
		Assert.AreEqual(1, analyses[0].Index);
	}

	[TestMethod]
	public void ChromaticChords_DetectsNeapolitan()
	{
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | Db | G7").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.Neapolitan, analyses[0].Kind);
		Assert.AreEqual("bII", analyses[0].Detail);
	}

	[TestMethod]
	public void ChromaticChords_DetectsBorrowedMinorSubdominant()
	{
		// Fm in C major is iv borrowed from the parallel minor.
		System.Collections.Generic.IReadOnlyList<ChromaticAnalysis> analyses =
			Progression.Parse("C | Fm | C").ChromaticChords(CMajor);
		Assert.AreEqual(ChromaticKind.BorrowedChord, analyses[0].Kind);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ChromaticAnalysisTests"`
Expected: FAIL — chromatic types/method not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/ChromaticKind.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The category of a non-diatonic (chromatic) chord within a key.</summary>
public enum ChromaticKind
{
	/// <summary>A secondary dominant: a dominant-quality chord tonicizing a diatonic degree.</summary>
	SecondaryDominant,

	/// <summary>A borrowed chord: diatonic to the parallel mode of the same tonic (modal interchange).</summary>
	BorrowedChord,

	/// <summary>A Neapolitan: a major triad on the lowered second degree.</summary>
	Neapolitan,

	/// <summary>
	/// An augmented-sixth chord. Reserved: detection is not implemented because the chord model does
	/// not carry the interval spelling needed to identify Italian/French/German sixths reliably.
	/// </summary>
	AugmentedSixth,

	/// <summary>A chromatic chord with no more specific classification.</summary>
	Chromatic,
}
```

```csharp
// Semantics.Music/ChromaticAnalysis.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>The chromatic classification of a chord at a position within a progression.</summary>
public sealed record ChromaticAnalysis
{
	/// <summary>Gets the zero-based index of the chord within the progression.</summary>
	public int Index { get; private init; }

	/// <summary>Gets the chromatic category.</summary>
	public ChromaticKind Kind { get; private init; }

	/// <summary>Gets an optional human-readable detail (e.g. "V/V", "bII"), or null.</summary>
	public string? Detail { get; private init; }

	/// <summary>Creates a chromatic analysis result.</summary>
	/// <param name="index">The zero-based chord index; must be non-negative.</param>
	/// <param name="kind">The chromatic category.</param>
	/// <param name="detail">An optional descriptive detail.</param>
	/// <returns>A new chromatic analysis result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
	public static ChromaticAnalysis Create(int index, ChromaticKind kind, string? detail)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be non-negative.");
		}

		return new() { Index = index, Kind = kind, Detail = detail };
	}
}
```

```csharp
// Semantics.Music/Progression.Chromatic.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	private static readonly string[] MajorDegreeRomans = ["I", "ii", "iii", "IV", "V", "vi", "vii°"];
	private static readonly string[] MinorDegreeRomans = ["i", "ii°", "III", "iv", "v", "VI", "VII"];

	/// <summary>Classifies each non-diatonic chord in the progression relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One entry per non-diatonic chord; diatonic chords are omitted.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<ChromaticAnalysis> ChromaticChords(Key key)
	{
		Ensure.NotNull(key);
		List<ChromaticAnalysis> result = [];
		for (int i = 0; i < Chords.Count; i++)
		{
			Chord chord = Chords[i].Chord;
			if (IsDiatonic(key, chord))
			{
				continue;
			}

			(ChromaticKind kind, string? detail) = ClassifyChromatic(key, chord);
			result.Add(ChromaticAnalysis.Create(i, kind, detail));
		}

		return result;
	}

	private static bool IsDiatonic(Key key, Chord chord)
	{
		Scale scale = key.Scale;
		return chord.ChordTones().All(offset => scale.Contains(PitchClass.Create(chord.Root.Value + offset)));
	}

	private static (ChromaticKind Kind, string? Detail) ClassifyChromatic(Key key, Chord chord)
	{
		// Secondary dominant: a major triad or dominant seventh a perfect fifth above a diatonic degree.
		bool dominantQuality = chord.Quality == ChordQuality.Major
			&& chord.Seventh is SeventhType.None or SeventhType.Dominant;
		if (dominantQuality)
		{
			ScaleDegree target = key.Scale.DegreeOf(PitchClass.Create(chord.Root.Value - 7));
			if (target.Alteration == 0 && target.Degree != 1)
			{
				string[] table = key.Mode == Mode.Major ? MajorDegreeRomans : MinorDegreeRomans;
				return (ChromaticKind.SecondaryDominant, "V/" + table[(target.Degree - 1) % 7]);
			}
		}

		// Neapolitan: a major triad on the lowered second degree.
		if (chord.Quality == ChordQuality.Major
			&& chord.Root.Value == PitchClass.Create(key.Tonic.Value + 1).Value)
		{
			return (ChromaticKind.Neapolitan, "bII");
		}

		// Borrowed: diatonic to the parallel mode of the same tonic.
		Mode parallelMode = key.Mode == Mode.Major ? Mode.Aeolian : Mode.Major;
		Scale parallel = Scale.Create(key.Tonic, parallelMode);
		if (chord.ChordTones().All(offset => parallel.Contains(PitchClass.Create(chord.Root.Value + offset))))
		{
			string source = parallelMode == Mode.Aeolian ? "parallel minor" : "parallel major";
			return (ChromaticKind.BorrowedChord, "from " + source);
		}

		return (ChromaticKind.Chromatic, null);
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ChromaticAnalysisTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/ChromaticKind.cs Semantics.Music/ChromaticAnalysis.cs Semantics.Music/Progression.Chromatic.cs Semantics.Test/Music/ChromaticAnalysisTests.cs
git commit -m "feat(music): add chromatic chord identification"
```

- [ ] **Step 6: Milestone 2 checkpoint — full build and test**

Run: `dotnet build` then `dotnet test`
Expected: builds clean; all tests pass.

---

## Milestone 3 — Section, Arrangement, Form

### Task 8: `Section`

**Files:**
- Create: `Semantics.Music/SectionType.cs`
- Create: `Semantics.Music/Section.cs`
- Test: `Semantics.Test/Music/SectionTests.cs`

**Interfaces:**
- Consumes: `Progression` (with `Chords`, `TotalBars`), `ChordEvent.Chord`, `Chord` equality, `Key`.
- Produces:
  - `enum SectionType { Intro, Verse, PreChorus, Chorus, PostChorus, Bridge, Solo, Interlude, Refrain, Outro, Coda, Other }`
  - `Section.Create(SectionType type, Progression progression, string? label = null, Key? key = null) -> Section`; properties `SectionType Type`, `string? Label`, `Progression Progression`, `Key? Key`, `double Bars`.
  - `Section.IsSameStructure(Section other) -> bool`

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/SectionTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class SectionTests
{
	[TestMethod]
	public void Section_ExposesTypeProgressionAndBars()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("C | Am | F | G"), "Verse 1");
		Assert.AreEqual(SectionType.Verse, verse.Type);
		Assert.AreEqual("Verse 1", verse.Label);
		Assert.AreEqual(4.0, verse.Bars, 1e-9);
	}

	[TestMethod]
	public void IsSameStructure_IgnoresLabelAndKey()
	{
		Section a = Section.Create(SectionType.Verse, Progression.Parse("C | Am | F | G"), "Verse 1");
		Section b = Section.Create(SectionType.Verse, Progression.Parse("C | Am | F | G"), "Verse 2");
		Assert.IsTrue(a.IsSameStructure(b));
	}

	[TestMethod]
	public void IsSameStructure_FalseForDifferentTypeOrChords()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("C | Am | F | G"));
		Section chorus = Section.Create(SectionType.Chorus, Progression.Parse("C | Am | F | G"));
		Section other = Section.Create(SectionType.Verse, Progression.Parse("F | G | C | C"));
		Assert.IsFalse(verse.IsSameStructure(chorus));
		Assert.IsFalse(verse.IsSameStructure(other));
	}

	[TestMethod]
	public void Section_RejectsNullProgression() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => Section.Create(SectionType.Verse, null!));
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~SectionTests"`
Expected: FAIL — `SectionType`/`Section` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/SectionType.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The structural role of a section within a piece.</summary>
public enum SectionType
{
	/// <summary>An introduction.</summary>
	Intro,

	/// <summary>A verse.</summary>
	Verse,

	/// <summary>A pre-chorus leading into the chorus.</summary>
	PreChorus,

	/// <summary>A chorus / refrain hook.</summary>
	Chorus,

	/// <summary>A post-chorus following the chorus.</summary>
	PostChorus,

	/// <summary>A bridge providing contrast.</summary>
	Bridge,

	/// <summary>A solo section.</summary>
	Solo,

	/// <summary>An instrumental interlude.</summary>
	Interlude,

	/// <summary>A recurring refrain.</summary>
	Refrain,

	/// <summary>An outro / ending section.</summary>
	Outro,

	/// <summary>A coda / tail.</summary>
	Coda,

	/// <summary>Any other or unspecified role.</summary>
	Other,
}
```

```csharp
// Semantics.Music/Section.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;

/// <summary>A labeled structural unit of a piece: a role plus its harmonic content.</summary>
public sealed record Section
{
	/// <summary>Gets the structural role of the section.</summary>
	public SectionType Type { get; private init; } = SectionType.Other;

	/// <summary>Gets an optional human label distinguishing repeated instances (e.g. "Verse 1").</summary>
	public string? Label { get; private init; }

	/// <summary>Gets the harmonic content of the section.</summary>
	public Progression Progression { get; private init; } = new();

	/// <summary>Gets an optional local key for a section that modulates; null inherits the arrangement key.</summary>
	public Key? Key { get; private init; }

	/// <summary>Gets the length of the section in bars, derived from its progression.</summary>
	public double Bars => Progression.TotalBars;

	/// <summary>Creates a section.</summary>
	/// <param name="type">The structural role.</param>
	/// <param name="progression">The harmonic content.</param>
	/// <param name="label">An optional human label.</param>
	/// <param name="key">An optional local key.</param>
	/// <returns>A new section.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="progression"/> is null.</exception>
	public static Section Create(SectionType type, Progression progression, string? label = null, Key? key = null)
	{
		Ensure.NotNull(progression);
		return new() { Type = type, Progression = progression, Label = label, Key = key };
	}

	/// <summary>Returns whether another section has the same role and ordered chord content.</summary>
	/// <param name="other">The section to compare.</param>
	/// <returns><see langword="true"/> when the type and chord sequence match; label and key are ignored.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
	public bool IsSameStructure(Section other)
	{
		Ensure.NotNull(other);
		if (Type != other.Type)
		{
			return false;
		}

		IReadOnlyList<ChordEvent> mine = Progression.Chords;
		IReadOnlyList<ChordEvent> theirs = other.Progression.Chords;
		if (mine.Count != theirs.Count)
		{
			return false;
		}

		for (int i = 0; i < mine.Count; i++)
		{
			if (mine[i].Chord != theirs[i].Chord)
			{
				return false;
			}
		}

		return true;
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~SectionTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/SectionType.cs Semantics.Music/Section.cs Semantics.Test/Music/SectionTests.cs
git commit -m "feat(music): add Section structural unit"
```

---

### Task 9: `Arrangement` (core, without Form)

**Files:**
- Create: `Semantics.Music/Arrangement.cs`
- Test: `Semantics.Test/Music/ArrangementTests.cs`

**Interfaces:**
- Consumes: `Section` (with `Bars`), `Key`.
- Produces: `Arrangement.Create(Key key, IEnumerable<Section> sections) -> Arrangement`; properties `Key Key`, `IReadOnlyList<Section> Sections`, `double TotalBars`. (The `Form Form` property is added in Task 10.)

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/ArrangementTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ArrangementTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void Arrangement_OrdersSectionsAndSumsBars()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("C | Am | F | G"));
		Section chorus = Section.Create(SectionType.Chorus, Progression.Parse("F | G | C | C"));
		Arrangement arrangement = Arrangement.Create(CMajor, [verse, chorus, verse]);
		Assert.AreEqual(3, arrangement.Sections.Count);
		Assert.AreEqual(12.0, arrangement.TotalBars, 1e-9);
		Assert.AreEqual(SectionType.Chorus, arrangement.Sections[1].Type);
	}

	[TestMethod]
	public void Arrangement_RejectsEmpty() =>
		_ = Assert.ThrowsExactly<ArgumentException>(() => Arrangement.Create(CMajor, System.Array.Empty<Section>()));

	[TestMethod]
	public void Arrangement_RejectsNullKey() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(
			() => Arrangement.Create(null!, [Section.Create(SectionType.Verse, Progression.Parse("C"))]));
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ArrangementTests"`
Expected: FAIL — `Arrangement` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/Arrangement.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The ordered realization of a piece: a sequence of sections in performance order.</summary>
public sealed record Arrangement
{
	/// <summary>Gets the home key used for whole-piece analysis; a section may override it locally.</summary>
	public Key Key { get; private init; } = Key.Create(PitchClass.Create(0), Mode.Major);

	/// <summary>Gets the sections in performance order; repetition is expressed by repeated entries.</summary>
	public IReadOnlyList<Section> Sections { get; private init; } = [];

	/// <summary>Gets the total length of the arrangement in bars.</summary>
	public double TotalBars => Sections.Sum(section => section.Bars);

	/// <summary>Creates an arrangement.</summary>
	/// <param name="key">The home key.</param>
	/// <param name="sections">The sections in performance order; must be non-empty.</param>
	/// <returns>A new arrangement.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="sections"/> is empty.</exception>
	public static Arrangement Create(Key key, IEnumerable<Section> sections)
	{
		Ensure.NotNull(key);
		Ensure.NotNull(sections);
		List<Section> list = [.. sections];
		if (list.Count == 0)
		{
			throw new ArgumentException("An arrangement must contain at least one section.", nameof(sections));
		}

		return new() { Key = key, Sections = list };
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ArrangementTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/Arrangement.cs Semantics.Test/Music/ArrangementTests.cs
git commit -m "feat(music): add Arrangement container"
```

---

### Task 10: `Form` (pattern extraction + named-form recognition) and wire `Arrangement.Form`

**Files:**
- Create: `Semantics.Music/NamedForm.cs`
- Create: `Semantics.Music/Form.cs`
- Modify: `Semantics.Music/Arrangement.cs` (add `Form Form` property)
- Test: `Semantics.Test/Music/FormTests.cs`

**Interfaces:**
- Consumes: `Arrangement` (`Sections`, `Key`), `Section.IsSameStructure`, `Section.Bars`, `Section.Key`, `Section.Progression.Chords`, `Key.FunctionOf`, `ScaleDegree`.
- Produces:
  - `enum NamedForm { ThirtyTwoBarAABA, TwelveBarBlues, VerseChorus, Binary, Ternary, Rondo, Strophic, ThroughComposed, Unknown }`
  - `Form.Of(Arrangement arrangement) -> Form`; `Form.FromPattern(string pattern) -> Form`; properties `string Pattern`, `IReadOnlyList<char> Letters`, `NamedForm? Name`.
  - `Arrangement.Form` computed property returning `Form.Of(this)`.

**Design note (deviation from spec):** `ThirtyTwoBarAABA` is recognized from the `AABA` **letter pattern** alone; the "each section ~8 bars" length is treated as the conventional realization and is not enforced, so `Form.Of` and `Form.FromPattern` agree. `TwelveBarBlues` remains the one progression-template form (detected only in `Form.Of`, which has access to progressions and the key).

- [ ] **Step 1: Write the failing test**

```csharp
// Semantics.Test/Music/FormTests.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class FormTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	private static Section Section(SectionType type, string chords) =>
		ktsu.Semantics.Music.Section.Create(type, Progression.Parse(chords));

	[TestMethod]
	public void Of_ExtractsAABAPattern_AndNamesThirtyTwoBarForm()
	{
		Section a = Section(SectionType.Verse, "C | Am | F | G");
		Section b = Section(SectionType.Bridge, "F | G | Em | Am");
		Arrangement arrangement = Arrangement.Create(CMajor, [a, a, b, a]);
		Form form = arrangement.Form;
		Assert.AreEqual("AABA", form.Pattern);
		Assert.AreEqual(NamedForm.ThirtyTwoBarAABA, form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTernaryAndBinary()
	{
		Section a = Section(SectionType.Verse, "C | G");
		Section b = Section(SectionType.Chorus, "F | C");
		Assert.AreEqual(NamedForm.Ternary, Arrangement.Create(CMajor, [a, b, a]).Form.Name);
		Assert.AreEqual(NamedForm.Binary, Arrangement.Create(CMajor, [a, b]).Form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTwelveBarBlues()
	{
		Section blues = Section(SectionType.Verse, "C7 | C7 | C7 | C7 | F7 | F7 | C7 | C7 | G7 | F7 | C7 | C7");
		Arrangement arrangement = Arrangement.Create(CMajor, [blues]);
		Assert.AreEqual(NamedForm.TwelveBarBlues, arrangement.Form.Name);
	}

	[TestMethod]
	public void FromPattern_AppliesLetterRecognition()
	{
		Form form = Form.FromPattern("ABACA");
		Assert.AreEqual(NamedForm.Rondo, form.Name);
		Assert.AreEqual(5, form.Letters.Count);
	}

	[TestMethod]
	public void FromPattern_RejectsNonLetters() =>
		_ = Assert.ThrowsExactly<FormatException>(() => Form.FromPattern("A1B"));
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~FormTests"`
Expected: FAIL — `NamedForm`/`Form`/`Arrangement.Form` not defined.

- [ ] **Step 3: Write minimal implementation**

```csharp
// Semantics.Music/NamedForm.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A recognized named musical form.</summary>
public enum NamedForm
{
	/// <summary>32-bar AABA song form (recognized by the AABA section pattern).</summary>
	ThirtyTwoBarAABA,

	/// <summary>12-bar blues (recognized by a section's I-IV-V progression template).</summary>
	TwelveBarBlues,

	/// <summary>Verse-chorus form.</summary>
	VerseChorus,

	/// <summary>Binary form (AB).</summary>
	Binary,

	/// <summary>Ternary form (ABA).</summary>
	Ternary,

	/// <summary>Rondo form (ABACA / ABACABA).</summary>
	Rondo,

	/// <summary>Strophic form (a single repeated section).</summary>
	Strophic,

	/// <summary>Through-composed (no section repeats).</summary>
	ThroughComposed,

	/// <summary>An unrecognized pattern.</summary>
	Unknown,
}
```

```csharp
// Semantics.Music/Form.cs
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The structural form of a piece: its section letter-pattern and any recognized named form.</summary>
public sealed record Form
{
	private static readonly int[][] BluesTemplates =
	[
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 1, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 1],
		[1, 4, 1, 1, 4, 4, 1, 1, 5, 4, 1, 5],
	];

	/// <summary>Gets the section letter-pattern (e.g. "AABA").</summary>
	public string Pattern { get; private init; } = "";

	/// <summary>Gets the per-section letters, aligned to arrangement order.</summary>
	public IReadOnlyList<char> Letters { get; private init; } = [];

	/// <summary>Gets the recognized named form, or null.</summary>
	public NamedForm? Name { get; private init; }

	/// <summary>Derives the form of an arrangement from its sections.</summary>
	/// <param name="arrangement">The arrangement to analyze.</param>
	/// <returns>The derived form.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="arrangement"/> is null.</exception>
	public static Form Of(Arrangement arrangement)
	{
		Ensure.NotNull(arrangement);
		IReadOnlyList<Section> sections = arrangement.Sections;

		List<char> letters = [];
		List<Section> representatives = [];
		foreach (Section section in sections)
		{
			int index = representatives.FindIndex(representative => representative.IsSameStructure(section));
			if (index < 0)
			{
				representatives.Add(section);
				index = representatives.Count - 1;
			}

			letters.Add((char)('A' + index));
		}

		string pattern = new([.. letters]);
		NamedForm name = sections.Any(section => IsTwelveBarBlues(section, section.Key ?? arrangement.Key))
			? NamedForm.TwelveBarBlues
			: RecognizePattern(pattern);

		return new() { Pattern = pattern, Letters = letters, Name = name };
	}

	/// <summary>Builds a form directly from a letter-pattern string, applying letter recognition only.</summary>
	/// <param name="pattern">A pattern of letters A-Z (e.g. "ABACA").</param>
	/// <returns>The form for the pattern.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="pattern"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the pattern is empty or contains a non-letter.</exception>
	public static Form FromPattern(string pattern)
	{
		Ensure.NotNull(pattern);
		if (pattern.Length == 0)
		{
			throw new FormatException("Form pattern is empty.");
		}

		if (!pattern.All(char.IsUpper))
		{
			throw new FormatException($"Form pattern '{pattern}' must contain only upper-case letters.");
		}

		return new() { Pattern = pattern, Letters = [.. pattern], Name = RecognizePattern(pattern) };
	}

	private static NamedForm RecognizePattern(string pattern) => pattern switch
	{
		"AABA" => NamedForm.ThirtyTwoBarAABA,
		"AB" => NamedForm.Binary,
		"ABA" => NamedForm.Ternary,
		_ when IsRondo(pattern) => NamedForm.Rondo,
		_ when pattern.Length > 1 && pattern.All(letter => letter == pattern[0]) => NamedForm.Strophic,
		_ when pattern.Distinct().Count() == pattern.Length => NamedForm.ThroughComposed,
		_ => NamedForm.Unknown,
	};

	private static bool IsRondo(string pattern)
	{
		if (pattern.Length < 5 || pattern.Length % 2 == 0)
		{
			return false;
		}

		for (int i = 0; i < pattern.Length; i++)
		{
			bool refrainPosition = i % 2 == 0;
			if (refrainPosition != (pattern[i] == 'A'))
			{
				return false;
			}
		}

		return true;
	}

	private static bool IsTwelveBarBlues(Section section, Key key)
	{
		IReadOnlyList<ChordEvent> chords = section.Progression.Chords;
		if (chords.Count != 12)
		{
			return false;
		}

		int[] degrees = new int[12];
		for (int i = 0; i < 12; i++)
		{
			ScaleDegree degree = key.FunctionOf(chords[i].Chord.Root);
			if (degree.Alteration != 0)
			{
				return false;
			}

			degrees[i] = degree.Degree;
		}

		return BluesTemplates.Any(template => template.SequenceEqual(degrees));
	}
}
```

Then modify `Semantics.Music/Arrangement.cs` — add this property after `TotalBars`:

```csharp
	/// <summary>Gets the structural form of the arrangement.</summary>
	public Form Form => Form.Of(this);
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~FormTests"`
Expected: PASS.

- [ ] **Step 5: Verify line endings, then commit**

```bash
git add Semantics.Music/NamedForm.cs Semantics.Music/Form.cs Semantics.Music/Arrangement.cs Semantics.Test/Music/FormTests.cs
git commit -m "feat(music): add Form pattern extraction and named-form recognition"
```

---

### Task 11: Documentation and final verification

**Files:**
- Modify: `README.md` (Music section), `CLAUDE.md` (project-layout row for `Semantics.Music`), and DESCRIPTION/TAGS if present — via the `update-docs` skill.

- [ ] **Step 1: Full solution build and test**

Run: `dotnet build`
Expected: clean across `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`.

Run: `dotnet test`
Expected: all tests pass (the ~944 pre-existing plus the new analysis tests).

- [ ] **Step 2: Update documentation**

Invoke the `update-docs` skill to extend the README Music section with the aggregate/analysis layer (`Progression`, `Section`, `Arrangement`, `Form` and the analyses), add a note in `CLAUDE.md` that `Semantics.Music` now includes the analysis containers, and refresh DESCRIPTION/TAGS as needed. Show a usage snippet, e.g.:

```csharp
Progression prog = Progression.Parse("Dm7 | G7 | Cmaj7");
Key key = prog.InferKey()!;                       // C major
IReadOnlyList<string> roman = prog.RomanNumerals(key);   // ii7, V7, Imaj7
IReadOnlyList<CadenceInstance> cadences = prog.Cadences(key);
```

- [ ] **Step 3: Commit documentation**

```bash
git add README.md CLAUDE.md
git commit -m "docs(music): document the analysis aggregate layer"
```

- [ ] **Step 4: Update project memory**

Append a memory file noting the analysis layer shipped (types, the AABA-by-pattern deviation, the deferred `AugmentedSixth` detection) and add its pointer to `MEMORY.md`.

---

## Self-Review

**Spec coverage:**
- Progression + ChordEvent + harmonic rhythm → Tasks 1–3. ✓
- Roman numerals → Task 4. ✓ Functional classification → Task 4. ✓
- Cadence detection → Task 5. ✓
- Key inference → Task 6. ✓
- Chromatic identification (secondary dominant / borrowed / Neapolitan / chromatic; AugmentedSixth reserved) → Task 7. ✓
- Section (+ `IsSameStructure`, `SectionType`) → Task 8. ✓
- Arrangement → Task 9. ✓
- Form (pattern + `NamedForm` recognition; 12-bar blues template; `FromPattern`) → Task 10. ✓
- Testing (per-type test files with concrete examples) → each task. ✓
- Docs (README/CLAUDE.md/DESCRIPTION/TAGS via update-docs) → Task 11. ✓
- Milestone sequencing (Progression/roman/function/cadence → key/chromatic → Section/Arrangement/Form) → Milestones 1–3. ✓

**Deviation recorded:** `ThirtyTwoBarAABA` recognized by the AABA letter pattern without enforcing 8-bar sections (documented in Task 10) so `Of`/`FromPattern` agree — a deliberate refinement of the spec's "~8 bars each" note.

**Placeholder scan:** no TBD/TODO; every code step has complete code; no "handle edge cases" hand-waving.

**Type consistency:** `Progression` is `partial record` across all six files; analysis method names (`RomanNumerals`, `Functions`, `Cadences`, `InferKey`/`InferKeys`, `ChromaticChords`) are consistent between the interface blocks and the implementations; `ChordEvent.Chord`/`Duration`, `CadenceInstance.Index/Type`, `KeyMatch.Key/Score`, `ChromaticAnalysis.Index/Kind/Detail`, `Section.IsSameStructure`, `Form.Of`/`FromPattern`/`Pattern`/`Letters`/`Name`, and `Arrangement.Form` all match across tasks.
