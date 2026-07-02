# Music Type-Safe Factories & Canonical Round-Trip ToString Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Give `Semantics.Music` compiler-enforced construction (a `NoteLetter` and `Accidental` enum plus typed `Create` factories), rename the string entry points to the `Parse`/`TryParse` convention, and give every in-scope music type a canonical `ToString()` with a tested `Parse(x.ToString()) == x` round-trip.

**Architecture:** Two value-backed enums (`NoteLetter`, `Accidental`) whose integer values are the natural pitch class and the semitone offset, so casts replace lookup switches. A shared internal `Notation` helper centralizes note-letter and accidental scanning. Every type gains a canonical single-line `ToString` (leaves and score composites) or a chart-style multi-line form (the three analysis aggregates). Each `Parse` is factored so `Parse` throws and `TryParse` returns a bool from one shared core.

**Tech Stack:** C#, multi-target (`net8.0`–`net10.0` + `netstandard2.0`/`netstandard2.1`), MSTest, ktsu.Sdk, Polyfill (`Ensure.NotNull`, `[NotNullWhen]`).

## Global Constraints

- File header on every new `.cs` file: `// Copyright (c) ktsu.dev` / `// All rights reserved.` / `// Licensed under the MIT license.`
- Tabs for indentation; CRLF line endings; file-scoped namespace `ktsu.Semantics.Music`; using directives inside the namespace.
- Nullable reference types enabled; treat warnings as errors; all analyzer diagnostics are errors.
- Do NOT run `dotnet format` (corrupts multi-target files in this repo).
- Tests: MSTest, explicit types (no `var`) in test bodies, semantic asserts (`Assert.AreEqual`, `Assert.IsTrue`, `Assert.ThrowsException`), not `Assert.IsTrue(x == y)`.
- `Parse` throws `FormatException` on malformed text and `ArgumentNullException` on null (via `Ensure.NotNull`). `TryParse` never throws; returns `false` and sets the out parameter to `null`/`default`.
- Do NOT implement `IParsable<T>` (net7+ only; conditional compilation is avoided here).
- Round-trip guarantee is **canonical-output**: `Parse(x.ToString()) == x`. For `Chord` this holds over the `Parse`-reachable corpus only.
- Build (`dotnet build`) and test (`dotnet test`) from the `Semantics.Music` / `Semantics.Test` solution. Each task ends green.

---

## File map

- Create: `Semantics.Music/NoteLetter.cs`, `Semantics.Music/Accidental.cs`, `Semantics.Music/Notation.cs`
- Modify (add `Parse`/`TryParse`/`ToString`, typed `Create`, or renames): `PitchClass.cs`, `Pitch.cs`, `Mode.cs`, `Interval.cs`, `Duration.cs`, `TimeSignature.cs`, `Velocity.cs`, `Tempo.cs`, `Scale.cs`, `Key.cs`, `Chord.cs`, `ChordEvent.cs`, `Note.cs`, `Rest.cs`, `Form.cs`, `Progression.cs`, `Section.cs`, `Arrangement.cs`
- Test: `Semantics.Test/Music/*` (new `RoundTripTests.cs` plus edits to existing test files that used `FromName`/`FromPattern`)
- Docs: `Semantics.Music/README.md`, `docs/superpowers/plans/2026-07-01-music-analysis-aggregate.md`

---

### Task 1: NoteLetter and Accidental enums

**Files:**
- Create: `Semantics.Music/NoteLetter.cs`, `Semantics.Music/Accidental.cs`
- Test: `Semantics.Test/Music/EnumInvariantTests.cs`

**Interfaces:**
- Produces: `enum NoteLetter { C=0, D=2, E=4, F=5, G=7, A=9, B=11 }`, `enum Accidental { DoubleFlat=-2, Flat=-1, Natural=0, Sharp=1, DoubleSharp=2 }`

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/EnumInvariantTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class EnumInvariantTests
{
	[TestMethod]
	public void NoteLetterValuesAreNaturalPitchClasses()
	{
		Assert.AreEqual(0, (int)NoteLetter.C);
		Assert.AreEqual(4, (int)NoteLetter.E);
		Assert.AreEqual(11, (int)NoteLetter.B);
	}

	[TestMethod]
	public void AccidentalValuesAreSemitoneOffsets()
	{
		Assert.AreEqual(-1, (int)Accidental.Flat);
		Assert.AreEqual(0, (int)Accidental.Natural);
		Assert.AreEqual(2, (int)Accidental.DoubleSharp);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~EnumInvariantTests"`
Expected: FAIL (compile error — `NoteLetter`/`Accidental` do not exist).

- [ ] **Step 3: Create the enums**

`Semantics.Music/NoteLetter.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The seven natural note letters. The underlying value is the natural pitch class.</summary>
public enum NoteLetter
{
	/// <summary>C (pitch class 0).</summary>
	C = 0,

	/// <summary>D (pitch class 2).</summary>
	D = 2,

	/// <summary>E (pitch class 4).</summary>
	E = 4,

	/// <summary>F (pitch class 5).</summary>
	F = 5,

	/// <summary>G (pitch class 7).</summary>
	G = 7,

	/// <summary>A (pitch class 9).</summary>
	A = 9,

	/// <summary>B (pitch class 11).</summary>
	B = 11,
}
```

`Semantics.Music/Accidental.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A pitch alteration. The underlying value is the semitone offset.</summary>
public enum Accidental
{
	/// <summary>Double flat (−2 semitones).</summary>
	DoubleFlat = -2,

	/// <summary>Flat (−1 semitone).</summary>
	Flat = -1,

	/// <summary>Natural (no alteration).</summary>
	Natural = 0,

	/// <summary>Sharp (+1 semitone).</summary>
	Sharp = 1,

	/// <summary>Double sharp (+2 semitones).</summary>
	DoubleSharp = 2,
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~EnumInvariantTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/NoteLetter.cs Semantics.Music/Accidental.cs Semantics.Test/Music/EnumInvariantTests.cs
git commit -m "feat(music): add NoteLetter and Accidental enums"
```

---

### Task 2: Notation helper + PitchClass typed factory, Parse/TryParse, ToString

**Files:**
- Create: `Semantics.Music/Notation.cs`
- Modify: `Semantics.Music/PitchClass.cs`
- Test: `Semantics.Test/Music/PitchClassTests.cs`

**Interfaces:**
- Consumes: `NoteLetter`, `Accidental` (Task 1).
- Produces:
  - `internal static class Notation` with `bool TryReadNoteLetter(char c, out NoteLetter letter)` and `int ReadAccidentalOffset(string text, ref int index)`.
  - `PitchClass.Create(NoteLetter letter, Accidental accidental)`, `PitchClass.Parse(string)`, `PitchClass.TryParse(string?, out PitchClass?)`, `PitchClass.ToString()`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/PitchClassTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using System;
using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PitchClassTests
{
	[TestMethod]
	public void TypedCreateComposesLetterAndAccidental()
	{
		PitchClass cSharp = PitchClass.Create(NoteLetter.C, Accidental.Sharp);
		Assert.AreEqual(1, cSharp.Value);
	}

	[TestMethod]
	public void ToStringIsSharpSpelled()
	{
		Assert.AreEqual("C#", PitchClass.Create(NoteLetter.C, Accidental.Sharp).ToString());
	}

	[TestMethod]
	public void ParseAcceptsFlatsAndNormalisesToSharp()
	{
		PitchClass dFlat = PitchClass.Parse("Db");
		Assert.AreEqual(1, dFlat.Value);
		Assert.AreEqual("C#", dFlat.ToString());
	}

	[TestMethod]
	public void RoundTripOfCanonicalOutput()
	{
		PitchClass parsed = PitchClass.Parse("Db");
		Assert.AreEqual(parsed, PitchClass.Parse(parsed.ToString()));
	}

	[TestMethod]
	public void ParseThrowsFormatExceptionOnGarbage()
	{
		Assert.ThrowsException<FormatException>(() => PitchClass.Parse("H"));
	}

	[TestMethod]
	public void TryParseReturnsFalseOnGarbage()
	{
		Assert.IsFalse(PitchClass.TryParse("H", out PitchClass? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~PitchClassTests"`
Expected: FAIL (compile error — members do not exist).

- [ ] **Step 3: Create the Notation helper**

`Semantics.Music/Notation.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>Shared note-letter and accidental scanning used by pitch, chord, and roman-numeral parsing.</summary>
internal static class Notation
{
	/// <summary>Maps a note-letter character (case-insensitive) to a <see cref="NoteLetter"/>.</summary>
	internal static bool TryReadNoteLetter(char c, out NoteLetter letter)
	{
		switch (char.ToUpperInvariant(c))
		{
			case 'C': letter = NoteLetter.C; return true;
			case 'D': letter = NoteLetter.D; return true;
			case 'E': letter = NoteLetter.E; return true;
			case 'F': letter = NoteLetter.F; return true;
			case 'G': letter = NoteLetter.G; return true;
			case 'A': letter = NoteLetter.A; return true;
			case 'B': letter = NoteLetter.B; return true;
			default: letter = default; return false;
		}
	}

	/// <summary>Consumes any run of accidental characters (# b ♯ ♭) from <paramref name="index"/>, returning the net semitone offset.</summary>
	internal static int ReadAccidentalOffset(string text, ref int index)
	{
		int offset = 0;
		while (index < text.Length && (text[index] is '#' or 'b' or '♯' or '♭'))
		{
			offset += text[index] is '#' or '♯' ? 1 : -1;
			index++;
		}

		return offset;
	}
}
```

- [ ] **Step 4: Add members to PitchClass**

In `Semantics.Music/PitchClass.cs`, add `using System;` and `using System.Diagnostics.CodeAnalysis;` inside the namespace, then add these members to the record:
```csharp
	/// <summary>Creates a pitch class from a note letter and accidental.</summary>
	/// <param name="letter">The natural note letter.</param>
	/// <param name="accidental">The accidental.</param>
	/// <returns>The folded pitch class.</returns>
	public static PitchClass Create(NoteLetter letter, Accidental accidental) =>
		Create((int)letter + (int)accidental);

	/// <summary>Parses a pitch-class name such as "C", "F#", or "Bb".</summary>
	/// <param name="text">A note letter A-G with optional accidentals; no octave.</param>
	/// <returns>The parsed pitch class.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static PitchClass Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out PitchClass? result)
			? result
			: throw new FormatException($"Invalid pitch class '{text}'.");
	}

	/// <summary>Tries to parse a pitch-class name.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed pitch class, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out PitchClass? result)
	{
		result = null;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}

		int index = 0;
		if (!Notation.TryReadNoteLetter(text![index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(text, ref index);
		if (index != text.Length)
		{
			return false;
		}

		result = Create((int)letter + accidental);
		return true;
	}

	/// <summary>Returns the sharp-spelled pitch-class name.</summary>
	/// <returns>The canonical name (e.g. "C#").</returns>
	public override string ToString() => Name;
```

- [ ] **Step 5: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~PitchClassTests"`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Music/Notation.cs Semantics.Music/PitchClass.cs Semantics.Test/Music/PitchClassTests.cs
git commit -m "feat(music): typed PitchClass factory, Parse/TryParse, canonical ToString"
```

---

### Task 3: Pitch typed factory, FromName → Parse/TryParse, ToString

**Files:**
- Modify: `Semantics.Music/Pitch.cs`, `Semantics.Music/Chord.cs` (2 internal `Pitch.FromName` calls in `Voice`)
- Test: `Semantics.Test/Music/PitchTests.cs` (update existing `FromName` calls), add cases

**Interfaces:**
- Consumes: `NoteLetter`, `Accidental`, `Notation` (Tasks 1–2).
- Produces: `Pitch.Create(NoteLetter, Accidental, int octave)`, `Pitch.Parse(string)`, `Pitch.TryParse(string?, out Pitch?)`, `Pitch.ToString()`. Removes `Pitch.FromName`.

- [ ] **Step 1: Write/adjust the failing test**

In `Semantics.Test/Music/PitchTests.cs`, replace every `Pitch.FromName(` with `Pitch.Parse(` and add:
```csharp
	[TestMethod]
	public void TypedCreateMatchesParse()
	{
		Assert.AreEqual(Pitch.Parse("C#4"), Pitch.Create(NoteLetter.C, Accidental.Sharp, 4));
	}

	[TestMethod]
	public void ToStringRoundTrips()
	{
		Pitch p = Pitch.Parse("F#3");
		Assert.AreEqual("F#3", p.ToString());
		Assert.AreEqual(p, Pitch.Parse(p.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnBadOctave()
	{
		Assert.IsFalse(Pitch.TryParse("Cx", out Pitch? result));
		Assert.IsNull(result);
	}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~PitchTests"`
Expected: FAIL (compile error — `Pitch.Parse`/`Pitch.Create(NoteLetter,...)` do not exist).

- [ ] **Step 3: Rewrite Pitch factory/parse and add ToString**

In `Semantics.Music/Pitch.cs`, replace the `FromName` method (lines 41–78) with:
```csharp
	/// <summary>Creates a pitch from a note letter, accidental, and octave (MIDI 60 = C4).</summary>
	/// <param name="letter">The natural note letter.</param>
	/// <param name="accidental">The accidental.</param>
	/// <param name="octave">The octave number.</param>
	/// <returns>A new pitch.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when the resulting MIDI number is outside 0..127.</exception>
	public static Pitch Create(NoteLetter letter, Accidental accidental, int octave) =>
		Create(((octave + 1) * 12) + (int)letter + (int)accidental);

	/// <summary>Parses a note name such as "C4", "F#3", or "Bb5".</summary>
	/// <param name="name">A letter A-G, optional accidentals, then an octave integer.</param>
	/// <returns>The parsed pitch.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the name cannot be parsed.</exception>
	public static Pitch Parse(string name)
	{
		Ensure.NotNull(name);
		return TryParse(name, out Pitch? result)
			? result
			: throw new FormatException($"Invalid note name '{name}'.");
	}

	/// <summary>Tries to parse a note name.</summary>
	/// <param name="name">The text to parse.</param>
	/// <param name="result">The parsed pitch, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Pitch? result)
	{
		result = null;
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}

		int index = 0;
		if (!Notation.TryReadNoteLetter(name![index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(name, ref index);
		if (!int.TryParse(name[index..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int octave))
		{
			return false;
		}

		int midi = ((octave + 1) * 12) + (int)letter + accidental;
		if (midi is < 0 or > 127)
		{
			return false;
		}

		result = new() { Midi = midi };
		return true;
	}

	/// <summary>Returns the canonical note name (e.g. "C4").</summary>
	/// <returns>The note name.</returns>
	public override string ToString() => Name;
```

- [ ] **Step 4: Update internal callers in Chord.cs**

In `Semantics.Music/Chord.cs` `Voice`, change both `Pitch.FromName(` calls (lines 377, 382) to `Pitch.Parse(`.

- [ ] **Step 5: Run tests to verify they pass**

Run: `dotnet test --filter "FullyQualifiedName~PitchTests"`
Expected: PASS. Then `dotnet build` to confirm no remaining `Pitch.FromName` references.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Music/Pitch.cs Semantics.Music/Chord.cs Semantics.Test/Music/PitchTests.cs
git commit -m "feat(music): typed Pitch factory, rename FromName to Parse/TryParse, canonical ToString"
```

---

### Task 4: Mode FromName → Parse/TryParse, ToString, centralize name literals

**Files:**
- Modify: `Semantics.Music/Mode.cs`
- Test: `Semantics.Test/Music/ModeTests.cs` (replace `FromName` calls), add round-trip

**Interfaces:**
- Produces: `Mode.Parse(string)`, `Mode.TryParse(string?, out Mode?)`, `Mode.ToString()`. Removes `Mode.FromName`.

- [ ] **Step 1: Adjust tests**

In `Semantics.Test/Music/ModeTests.cs`, replace `Mode.FromName(` with `Mode.Parse(` and add:
```csharp
	[TestMethod]
	public void ToStringRoundTripsForAllShapes()
	{
		foreach (string name in new[] { "major", "dorian", "harmonic_minor", "octatonic_hw", "blues_major" })
		{
			Mode mode = Mode.Parse(name);
			Assert.AreEqual(name, mode.ToString());
			Assert.AreEqual(mode, Mode.Parse(mode.ToString()));
		}
	}

	[TestMethod]
	public void TryParseFailsOnUnknownMode()
	{
		Assert.IsFalse(Mode.TryParse("bogus", out Mode? result));
		Assert.IsNull(result);
	}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ModeTests"`
Expected: FAIL (compile error — `Mode.Parse`/`Mode.ToString` do not exist).

- [ ] **Step 3: Replace FromName and add ToString**

In `Semantics.Music/Mode.cs`, replace `FromName` (lines 159–170) with:
```csharp
	/// <summary>Parses a mode by name, case-insensitively.</summary>
	/// <param name="name">The mode name (e.g. "aeolian", "harmonic_minor", "octatonic_hw").</param>
	/// <returns>The matching mode.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the name is not a known mode.</exception>
	public static Mode Parse(string name)
	{
		Ensure.NotNull(name);
		return TryParse(name, out Mode? result)
			? result
			: throw new FormatException($"Unknown mode '{name}'.");
	}

	/// <summary>Tries to parse a mode by name, case-insensitively.</summary>
	/// <param name="name">The mode name.</param>
	/// <param name="result">The matching mode, or null on failure.</param>
	/// <returns><see langword="true"/> when the name is a known mode.</returns>
	public static bool TryParse(string? name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Mode? result)
	{
		if (name is not null && Shapes.ContainsKey(name))
		{
			result = new() { Name = name.ToLowerInvariant() };
			return true;
		}

		result = null;
		return false;
	}

	/// <summary>Returns the canonical lower-case mode name.</summary>
	/// <returns>The mode name.</returns>
	public override string ToString() => Name;
```

Add `using System;` and `using System.Diagnostics.CodeAnalysis;` inside the namespace if not present (the file currently has `using System;` and `using System.Collections.Generic;`).

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ModeTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Mode.cs Semantics.Test/Music/ModeTests.cs
git commit -m "feat(music): rename Mode.FromName to Parse/TryParse, canonical ToString"
```

> Note: the `Shapes` dictionary keys already serve as the single source for `Name`; the 30 static constant literals stay as-is (a `ModeName` enum was explicitly ruled out). Deep literal-deduplication is optional and not required to pass tests.

---

### Task 5: Interval canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Interval.cs`
- Test: `Semantics.Test/Music/IntervalTests.cs`

**Interfaces:**
- Produces: `Interval.Parse(string)`, `Interval.TryParse(string?, out Interval?)`, `Interval.ToString()`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/IntervalTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using System;
using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class IntervalTests
{
	[TestMethod]
	public void ToStringIsSignedSemitones()
	{
		Assert.AreEqual("7", Interval.Create(7).ToString());
		Assert.AreEqual("-5", Interval.Create(-5).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		foreach (int s in new[] { 0, 7, -5, 14 })
		{
			Interval i = Interval.Create(s);
			Assert.AreEqual(i, Interval.Parse(i.ToString()));
		}
	}

	[TestMethod]
	public void TryParseFailsOnGarbage()
	{
		Assert.IsFalse(Interval.TryParse("x", out Interval? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~IntervalTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Interval.cs`, add `using System.Globalization;` inside the namespace and add:
```csharp
	/// <summary>Parses a signed semitone count.</summary>
	/// <param name="text">The signed integer text.</param>
	/// <returns>The parsed interval.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is not an integer.</exception>
	public static Interval Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Interval? result)
			? result
			: throw new FormatException($"Invalid interval '{text}'.");
	}

	/// <summary>Tries to parse a signed semitone count.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed interval, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Interval? result)
	{
		if (text is not null && int.TryParse(text, NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int semitones))
		{
			result = Create(semitones);
			return true;
		}

		result = null;
		return false;
	}

	/// <summary>Returns the signed semitone count.</summary>
	/// <returns>The canonical interval text.</returns>
	public override string ToString() => Semitones.ToString(CultureInfo.InvariantCulture);
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~IntervalTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Interval.cs Semantics.Test/Music/IntervalTests.cs
git commit -m "feat(music): Interval canonical ToString + Parse/TryParse"
```

---

### Task 6: Duration canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Duration.cs`
- Test: `Semantics.Test/Music/DurationTests.cs`

**Interfaces:**
- Produces: `Duration.Parse(string)`, `Duration.TryParse(string?, out Duration?)`, `Duration.ToString()`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/DurationTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using System;
using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class DurationTests
{
	[TestMethod]
	public void ToStringIsReducedFraction()
	{
		Assert.AreEqual("1/4", Duration.Quarter.ToString());
		Assert.AreEqual("1/4", Duration.Create(2, 8).ToString());
	}

	[TestMethod]
	public void RoundTripOfCanonicalOutput()
	{
		Duration d = Duration.Create(2, 8);
		Assert.AreEqual(d, Duration.Parse(d.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnMissingSlashOrZeroDenominator()
	{
		Assert.IsFalse(Duration.TryParse("4", out Duration? a));
		Assert.IsNull(a);
		Assert.IsFalse(Duration.TryParse("1/0", out Duration? b));
		Assert.IsNull(b);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~DurationTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Duration.cs`, add `using System.Globalization;` inside the namespace and add:
```csharp
	/// <summary>Parses a fraction "n/d".</summary>
	/// <param name="text">The fraction text.</param>
	/// <returns>The reduced duration.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is not a valid non-zero-denominator fraction.</exception>
	public static Duration Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Duration? result)
			? result
			: throw new FormatException($"Invalid duration '{text}'.");
	}

	/// <summary>Tries to parse a fraction "n/d".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The reduced duration, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Duration? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int slash = text.IndexOf('/');
		if (slash <= 0 || slash == text.Length - 1)
		{
			return false;
		}

		if (!int.TryParse(text[..slash], NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int numerator)
			|| !int.TryParse(text[(slash + 1)..], NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int denominator)
			|| denominator == 0)
		{
			return false;
		}

		result = Create(numerator, denominator);
		return true;
	}

	/// <summary>Returns the reduced fraction (e.g. "1/4").</summary>
	/// <returns>The canonical duration text.</returns>
	public override string ToString() =>
		$"{Numerator.ToString(CultureInfo.InvariantCulture)}/{Denominator.ToString(CultureInfo.InvariantCulture)}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~DurationTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Duration.cs Semantics.Test/Music/DurationTests.cs
git commit -m "feat(music): Duration canonical ToString + Parse/TryParse"
```

---

### Task 7: TimeSignature canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/TimeSignature.cs`
- Test: `Semantics.Test/Music/TimeSignatureTests.cs`

**Interfaces:**
- Produces: `TimeSignature.Parse(string)`, `TimeSignature.TryParse(string?, out TimeSignature?)`, `TimeSignature.ToString()`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/TimeSignatureTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TimeSignatureTests
{
	[TestMethod]
	public void ToStringIsBeatsOverUnit()
	{
		Assert.AreEqual("6/8", TimeSignature.Create(6, 8).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		TimeSignature ts = TimeSignature.Create(7, 8);
		Assert.AreEqual(ts, TimeSignature.Parse(ts.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnNonPositive()
	{
		Assert.IsFalse(TimeSignature.TryParse("0/4", out TimeSignature? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~TimeSignatureTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/TimeSignature.cs`, add `using System.Globalization;` inside the namespace and add:
```csharp
	/// <summary>Parses a time signature "beats/unit".</summary>
	/// <param name="text">The time-signature text.</param>
	/// <returns>The parsed time signature.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is not a valid positive "beats/unit".</exception>
	public static TimeSignature Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out TimeSignature? result)
			? result
			: throw new FormatException($"Invalid time signature '{text}'.");
	}

	/// <summary>Tries to parse a time signature "beats/unit".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed time signature, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TimeSignature? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int slash = text.IndexOf('/');
		if (slash <= 0 || slash == text.Length - 1)
		{
			return false;
		}

		if (!int.TryParse(text[..slash], NumberStyles.Integer, CultureInfo.InvariantCulture, out int beats)
			|| !int.TryParse(text[(slash + 1)..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int beatUnit)
			|| beats <= 0 || beatUnit <= 0)
		{
			return false;
		}

		result = Create(beats, beatUnit);
		return true;
	}

	/// <summary>Returns "beats/unit" (e.g. "4/4").</summary>
	/// <returns>The canonical time-signature text.</returns>
	public override string ToString() =>
		$"{Beats.ToString(CultureInfo.InvariantCulture)}/{BeatUnit.ToString(CultureInfo.InvariantCulture)}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~TimeSignatureTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/TimeSignature.cs Semantics.Test/Music/TimeSignatureTests.cs
git commit -m "feat(music): TimeSignature canonical ToString + Parse/TryParse"
```

---

### Task 8: Velocity canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Velocity.cs`
- Test: `Semantics.Test/Music/VelocityTests.cs`

**Interfaces:**
- Produces: `Velocity.Parse(string)`, `Velocity.TryParse(string?, out Velocity?)`, `Velocity.ToString()`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/VelocityTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class VelocityTests
{
	[TestMethod]
	public void ToStringIsInteger()
	{
		Assert.AreEqual("96", Velocity.Create(96).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Velocity v = Velocity.Create(100);
		Assert.AreEqual(v, Velocity.Parse(v.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOutOfRange()
	{
		Assert.IsFalse(Velocity.TryParse("200", out Velocity? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~VelocityTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Velocity.cs`, add `using System.Globalization;` inside the namespace and add:
```csharp
	/// <summary>Parses a velocity value.</summary>
	/// <param name="text">The integer text (0..127).</param>
	/// <returns>The parsed velocity.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is not an integer in 0..127.</exception>
	public static Velocity Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Velocity? result)
			? result
			: throw new FormatException($"Invalid velocity '{text}'.");
	}

	/// <summary>Tries to parse a velocity value.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed velocity, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Velocity? result)
	{
		if (text is not null
			&& int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
			&& value is >= 0 and <= 127)
		{
			result = Create(value);
			return true;
		}

		result = null;
		return false;
	}

	/// <summary>Returns the velocity value as an integer string.</summary>
	/// <returns>The canonical velocity text.</returns>
	public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~VelocityTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Velocity.cs Semantics.Test/Music/VelocityTests.cs
git commit -m "feat(music): Velocity canonical ToString + Parse/TryParse"
```

---

### Task 9: Tempo canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Tempo.cs`
- Test: `Semantics.Test/Music/TempoTests.cs`

**Interfaces:**
- Consumes: `Duration.Parse`/`ToString` (Task 6).
- Produces: `Tempo.Parse(string)`, `Tempo.TryParse(string?, out Tempo?)`, `Tempo.ToString()`. Format `{bpm}bpm@{beat}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/TempoTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TempoTests
{
	[TestMethod]
	public void ToStringEncodesBpmAndBeat()
	{
		Assert.AreEqual("120bpm@1/4", Tempo.Create(120).ToString());
		Assert.AreEqual("90bpm@1/8", Tempo.Create(90, Duration.Eighth).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Tempo t = Tempo.Create(132.5, Duration.Eighth);
		Assert.AreEqual(t, Tempo.Parse(t.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWhenMarkerMissing()
	{
		Assert.IsFalse(Tempo.TryParse("120", out Tempo? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~TempoTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Tempo.cs`, add `using System.Globalization;` inside the namespace and add:
```csharp
	/// <summary>Parses a tempo "{bpm}bpm@{beat}" (e.g. "120bpm@1/4").</summary>
	/// <param name="text">The tempo text.</param>
	/// <returns>The parsed tempo.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Tempo Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Tempo? result)
			? result
			: throw new FormatException($"Invalid tempo '{text}'.");
	}

	/// <summary>Tries to parse a tempo "{bpm}bpm@{beat}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed tempo, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Tempo? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int marker = text.IndexOf("bpm@", StringComparison.Ordinal);
		if (marker <= 0)
		{
			return false;
		}

		if (!double.TryParse(text[..marker], NumberStyles.Float, CultureInfo.InvariantCulture, out double bpm)
			|| bpm <= 0.0
			|| !Duration.TryParse(text[(marker + 4)..], out Duration? beat))
		{
			return false;
		}

		result = Create(bpm, beat);
		return true;
	}

	/// <summary>Returns "{bpm}bpm@{beat}" (e.g. "120bpm@1/4").</summary>
	/// <returns>The canonical tempo text.</returns>
	public override string ToString() =>
		$"{BeatsPerMinute.ToString("R", CultureInfo.InvariantCulture)}bpm@{Beat}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~TempoTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Tempo.cs Semantics.Test/Music/TempoTests.cs
git commit -m "feat(music): Tempo canonical ToString + Parse/TryParse"
```

---

### Task 10: Scale canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Scale.cs`
- Test: `Semantics.Test/Music/ScaleTests.cs`

**Interfaces:**
- Consumes: `PitchClass.Parse`/`ToString` (Task 2), `Mode.Parse`/`ToString` (Task 4).
- Produces: `Scale.Parse(string)`, `Scale.TryParse(string?, out Scale?)`, `Scale.ToString()`. Format `{root} {mode}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/ScaleTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ScaleTests
{
	[TestMethod]
	public void ToStringIsRootSpaceMode()
	{
		Scale s = Scale.Create(PitchClass.Create(NoteLetter.C, Accidental.Natural), Mode.Dorian);
		Assert.AreEqual("C dorian", s.ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Scale s = Scale.Create(PitchClass.Create(NoteLetter.F, Accidental.Sharp), Mode.HarmonicMinor);
		Assert.AreEqual(s, Scale.Parse(s.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWithoutSpace()
	{
		Assert.IsFalse(Scale.TryParse("Cdorian", out Scale? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ScaleTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Scale.cs`, add:
```csharp
	/// <summary>Parses a scale "{root} {mode}" (e.g. "C dorian").</summary>
	/// <param name="text">The scale text.</param>
	/// <returns>The parsed scale.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Scale Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Scale? result)
			? result
			: throw new FormatException($"Invalid scale '{text}'.");
	}

	/// <summary>Tries to parse a scale "{root} {mode}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed scale, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Scale? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int space = text.IndexOf(' ');
		if (space <= 0 || space == text.Length - 1)
		{
			return false;
		}

		if (!PitchClass.TryParse(text[..space], out PitchClass? root)
			|| !Mode.TryParse(text[(space + 1)..], out Mode? mode))
		{
			return false;
		}

		result = Create(root, mode);
		return true;
	}

	/// <summary>Returns "{root} {mode}" (e.g. "C dorian").</summary>
	/// <returns>The canonical scale text.</returns>
	public override string ToString() => $"{Root} {Mode}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ScaleTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Scale.cs Semantics.Test/Music/ScaleTests.cs
git commit -m "feat(music): Scale canonical ToString + Parse/TryParse"
```

---

### Task 11: Key canonical ToString + Parse/TryParse, roman-numeral accidental refactor

**Files:**
- Modify: `Semantics.Music/Key.cs`
- Test: `Semantics.Test/Music/KeyTests.cs`

**Interfaces:**
- Consumes: `PitchClass.Parse`/`ToString`, `Mode.Parse`/`ToString`, `Notation.ReadAccidentalOffset`.
- Produces: `Key.Parse(string)`, `Key.TryParse(string?, out Key?)`, `Key.ToString()`. Format `{tonic} {mode}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/KeyTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class KeyTests
{
	[TestMethod]
	public void ToStringIsTonicSpaceMode()
	{
		Key k = Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian);
		Assert.AreEqual("A aeolian", k.ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Key k = Key.Create(PitchClass.Create(NoteLetter.E, Accidental.Flat), Mode.Major);
		Assert.AreEqual(k, Key.Parse(k.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnUnknownMode()
	{
		Assert.IsFalse(Key.TryParse("C bogus", out Key? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~KeyTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members and refactor the numeral accidental scan**

In `Semantics.Music/Key.cs`, add:
```csharp
	/// <summary>Parses a key "{tonic} {mode}" (e.g. "C major", "A aeolian").</summary>
	/// <param name="text">The key text.</param>
	/// <returns>The parsed key.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Key Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Key? result)
			? result
			: throw new FormatException($"Invalid key '{text}'.");
	}

	/// <summary>Tries to parse a key "{tonic} {mode}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed key, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Key? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int space = text.IndexOf(' ');
		if (space <= 0 || space == text.Length - 1)
		{
			return false;
		}

		if (!PitchClass.TryParse(text[..space], out PitchClass? tonic)
			|| !Mode.TryParse(text[(space + 1)..], out Mode? mode))
		{
			return false;
		}

		result = Create(tonic, mode);
		return true;
	}

	/// <summary>Returns "{tonic} {mode}" (e.g. "C major").</summary>
	/// <returns>The canonical key text.</returns>
	public override string ToString() => $"{Tonic} {Mode}";
```

Then in `ChordFromRomanNumeral`, replace the inline accidental loop (lines 103–108) with:
```csharp
		int index = 0;
		int alteration = Notation.ReadAccidentalOffset(text, ref index);
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~KeyTests"`
Expected: PASS. Then `dotnet test --filter "FullyQualifiedName~Music"` to confirm the roman-numeral refactor didn't regress existing key/analysis tests.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Key.cs Semantics.Test/Music/KeyTests.cs
git commit -m "feat(music): Key canonical ToString + Parse/TryParse; roman-numeral accidental via Notation"
```

---

### Task 12: Chord canonical ToString, TryParse, ParseRoot refactor

**Files:**
- Modify: `Semantics.Music/Chord.cs`
- Test: `Semantics.Test/Music/ChordRoundTripTests.cs`

**Interfaces:**
- Consumes: `Notation` (Task 2), `PitchClass`.
- Produces: `Chord.TryParse(string?, out Chord?)`, `Chord.ToString()` (canonical symbol). `Chord.Parse` name unchanged.

- [ ] **Step 1: Write the failing corpus round-trip test**

`Semantics.Test/Music/ChordRoundTripTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ChordRoundTripTests
{
	private static readonly string[] Corpus =
	[
		"C", "Cm", "Cdim", "Caug", "Csus2", "Csus4", "C5",
		"Cmaj7", "C7", "Cm7", "Cdim7", "Cm7b5", "CmMaj7",
		"C6", "Cm6", "C9", "Cm9", "C11", "C13",
		"C7b9", "C7#9", "C7#11", "C7b13", "Cadd9",
		"C/G", "Dm7/G", "F#m7b5", "Bbmaj7",
	];

	[TestMethod]
	public void CanonicalOutputRoundTrips()
	{
		foreach (string symbol in Corpus)
		{
			Chord chord = Chord.Parse(symbol);
			Chord reparsed = Chord.Parse(chord.ToString());
			Assert.AreEqual(chord, reparsed, $"round-trip failed for '{symbol}' -> '{chord}'");
		}
	}

	[TestMethod]
	public void TryParseReturnsFalseOnEmpty()
	{
		Assert.IsFalse(Chord.TryParse("", out Chord? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ChordRoundTripTests"`
Expected: FAIL (compile error — `Chord.TryParse`/`Chord.ToString` do not exist).

- [ ] **Step 3: Refactor Parse/ParseRoot to TryParse and add the formatter**

In `Semantics.Music/Chord.cs`:

(a) Replace the body of `Parse` (lines 43–89) so it delegates to a new `TryParse`, and add `TryParse`:
```csharp
	/// <summary>Parses a chord symbol.</summary>
	/// <param name="symbol">The chord symbol.</param>
	/// <returns>The parsed chord.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="symbol"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the symbol cannot be parsed.</exception>
	public static Chord Parse(string symbol)
	{
		Ensure.NotNull(symbol);
		return TryParse(symbol, out Chord? result)
			? result
			: throw new FormatException($"Invalid chord symbol '{symbol}'.");
	}

	/// <summary>Tries to parse a chord symbol.</summary>
	/// <param name="symbol">The text to parse.</param>
	/// <param name="result">The parsed chord, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Chord? result)
	{
		result = null;
		if (string.IsNullOrEmpty(symbol))
		{
			return false;
		}

		if (!TryReadRoot(symbol!, out PitchClass? bass, out string head, out int index, out PitchClass? root))
		{
			return false;
		}

		string body = new([.. head[index..].Where(c => c is not ('(' or ')'))]);
		ChordModifiers modifiers = ConsumeModifiers(ref body);
		ChordQuality quality = DetermineQuality(body, modifiers.FifthAlteration);
		SeventhType seventh = DetermineSeventh(body, quality);

		SixthType sixth = modifiers.Sixth;
		if (sixth == SixthType.None && body.Contains('6'))
		{
			sixth = SixthType.Natural;
		}

		ChordTension tensions = modifiers.Tensions;
		ApplyExtensions(ref body, modifiers.HasAdd9, ref seventh, ref tensions);

		result = new Chord
		{
			Root = root,
			Quality = quality,
			Seventh = seventh,
			Sixth = sixth,
			Tensions = tensions,
			Omissions = modifiers.Omissions,
			Bass = bass,
		};
		return true;
	}

	private static bool TryReadRoot(string symbol, out PitchClass? bass, out string head, out int index, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PitchClass? root)
	{
		bass = null;
		head = symbol;
		index = 0;
		root = null;

		int slash = symbol.IndexOf('/');
		if (slash >= 0)
		{
			int bassIndex = 0;
			if (!TryParseRoot(symbol[(slash + 1)..], ref bassIndex, out PitchClass? parsedBass))
			{
				return false;
			}

			bass = parsedBass;
			head = symbol[..slash];
		}

		return TryParseRoot(head, ref index, out root);
	}
```

(b) Replace `ParseRoot` (lines 91–119) with a `TryParseRoot` that uses `Notation`:
```csharp
	private static bool TryParseRoot(string symbol, ref int index, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PitchClass? root)
	{
		root = null;
		if (index >= symbol.Length || !Notation.TryReadNoteLetter(symbol[index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(symbol, ref index);
		root = PitchClass.Create((int)letter + accidental);
		return true;
	}
```

(c) Add the canonical formatter (place after `Voice`):
```csharp
	/// <summary>Returns the canonical chord symbol. The formatter is the inverse of <see cref="Parse"/> over the parseable corpus.</summary>
	/// <returns>The canonical chord symbol (e.g. "Cmaj7", "C/G").</returns>
	public override string ToString()
	{
		System.Text.StringBuilder sb = new();
		_ = sb.Append(Root.Name);
		AppendQualityAndSeventh(sb);
		AppendSixth(sb);
		AppendTensions(sb);
		AppendOmissions(sb);
		if (Bass is not null)
		{
			_ = sb.Append('/').Append(Bass.Name);
		}

		return sb.ToString();
	}

	private void AppendQualityAndSeventh(System.Text.StringBuilder sb)
	{
		switch (Quality)
		{
			case ChordQuality.Sus2: _ = sb.Append("sus2"); AppendPlainSeventh(sb); break;
			case ChordQuality.Sus4: _ = sb.Append("sus4"); AppendPlainSeventh(sb); break;
			case ChordQuality.Power: _ = sb.Append('5'); break;
			case ChordQuality.Augmented: _ = sb.Append("aug"); AppendPlainSeventh(sb); break;
			case ChordQuality.Minor: _ = sb.Append('m'); AppendPlainSeventh(sb); break;
			case ChordQuality.Diminished: AppendDiminished(sb); break;
			default: AppendPlainSeventh(sb); break;
		}
	}

	private void AppendPlainSeventh(System.Text.StringBuilder sb) => _ = sb.Append(Seventh switch
	{
		SeventhType.Major => "maj7",
		SeventhType.Dominant => "7",
		SeventhType.Diminished => "7",
		_ => "",
	});

	private void AppendDiminished(System.Text.StringBuilder sb) => _ = Seventh switch
	{
		SeventhType.Diminished => sb.Append("dim7"),
		SeventhType.Dominant => sb.Append("m7b5"),
		SeventhType.Major => sb.Append("dimmaj7"),
		_ => sb.Append("dim"),
	};

	private void AppendSixth(System.Text.StringBuilder sb) => _ = Sixth switch
	{
		SixthType.Natural => sb.Append('6'),
		SixthType.Flat => sb.Append("b6"),
		_ => sb,
	};

	private void AppendTensions(System.Text.StringBuilder sb)
	{
		// Natural extension stack: 13 implies 9+11+13, 11 implies 9+11. A bare 9 with no
		// seventh must be written "add9" so it does not imply a dominant seventh on reparse.
		bool hasSeventh = Seventh != SeventhType.None;
		if (Tensions.HasFlag(ChordTension.Thirteen))
		{
			_ = sb.Append("13");
		}
		else if (Tensions.HasFlag(ChordTension.Eleven))
		{
			_ = sb.Append("11");
		}
		else if (Tensions.HasFlag(ChordTension.Nine))
		{
			_ = sb.Append(hasSeventh ? "9" : "add9");
		}

		if (Tensions.HasFlag(ChordTension.FlatNine)) { _ = sb.Append("b9"); }
		if (Tensions.HasFlag(ChordTension.SharpNine)) { _ = sb.Append("#9"); }
		if (Tensions.HasFlag(ChordTension.SharpEleven)) { _ = sb.Append("#11"); }
		if (Tensions.HasFlag(ChordTension.FlatThirteen)) { _ = sb.Append("b13"); }
	}

	private void AppendOmissions(System.Text.StringBuilder sb)
	{
		if (Omissions.HasFlag(ChordOmission.Third)) { _ = sb.Append("no3"); }
		if (Omissions.HasFlag(ChordOmission.Fifth)) { _ = sb.Append("no5"); }
	}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ChordRoundTripTests"`
Expected: PASS. If a corpus entry fails, adjust the formatter (not the parser) so its output reparses to the same structure; re-run until green.

- [ ] **Step 5: Run the whole Music suite**

Run: `dotnet test --filter "FullyQualifiedName~Music"`
Expected: PASS (confirms `Voice` and analysis code still work with the refactored parser).

- [ ] **Step 6: Commit**

```bash
git add Semantics.Music/Chord.cs Semantics.Test/Music/ChordRoundTripTests.cs
git commit -m "feat(music): canonical Chord ToString, TryParse, ParseRoot via Notation"
```

---

### Task 13: ChordEvent canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/ChordEvent.cs`
- Test: `Semantics.Test/Music/ChordEventTests.cs`

**Interfaces:**
- Consumes: `Chord.Parse`/`TryParse`/`ToString` (Task 12), `Duration.Parse`/`ToString` (Task 6).
- Produces: `ChordEvent.Parse(string)`, `ChordEvent.TryParse(string?, out ChordEvent?)`, `ChordEvent.ToString()`. Format `{chord}:{duration}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/ChordEventTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ChordEventTests
{
	[TestMethod]
	public void ToStringIsChordColonDuration()
	{
		ChordEvent e = ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Quarter);
		Assert.AreEqual("Cmaj7:1/4", e.ToString());
	}

	[TestMethod]
	public void RoundTripWithSlashChord()
	{
		ChordEvent e = ChordEvent.Create(Chord.Parse("C/G"), Duration.Half);
		Assert.AreEqual(e, ChordEvent.Parse(e.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWithoutColon()
	{
		Assert.IsFalse(ChordEvent.TryParse("Cmaj7", out ChordEvent? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ChordEventTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/ChordEvent.cs`, add `using System;` inside the namespace and add:
```csharp
	/// <summary>Parses a chord event "{chord}:{duration}" (e.g. "Cmaj7:1/4").</summary>
	/// <param name="text">The chord-event text.</param>
	/// <returns>The parsed chord event.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static ChordEvent Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out ChordEvent? result)
			? result
			: throw new FormatException($"Invalid chord event '{text}'.");
	}

	/// <summary>Tries to parse a chord event "{chord}:{duration}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed chord event, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ChordEvent? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int colon = text.IndexOf(':');
		if (colon <= 0 || colon == text.Length - 1)
		{
			return false;
		}

		if (!Chord.TryParse(text[..colon], out Chord? chord)
			|| !Duration.TryParse(text[(colon + 1)..], out Duration? duration))
		{
			return false;
		}

		result = Create(chord, duration);
		return true;
	}

	/// <summary>Returns "{chord}:{duration}" (e.g. "Cmaj7:1/4").</summary>
	/// <returns>The canonical chord-event text.</returns>
	public override string ToString() => $"{Chord}:{Duration}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ChordEventTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/ChordEvent.cs Semantics.Test/Music/ChordEventTests.cs
git commit -m "feat(music): ChordEvent canonical ToString + Parse/TryParse"
```

---

### Task 14: Note canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Note.cs`
- Test: `Semantics.Test/Music/NoteTests.cs`

**Interfaces:**
- Consumes: `Pitch`, `Duration`, `Velocity` Parse/ToString.
- Produces: `Note.Parse(string)`, `Note.TryParse(string?, out Note?)`, `Note.ToString()`. Format `{pitch}:{duration}:v{velocity}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/NoteTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NoteTests
{
	[TestMethod]
	public void ToStringEncodesPitchDurationVelocity()
	{
		Note n = Note.Create(Pitch.Parse("C4"), Duration.Quarter, Velocity.Create(80));
		Assert.AreEqual("C4:1/4:v80", n.ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Note n = Note.Create(Pitch.Parse("F#3"), Duration.Eighth, Velocity.Create(100));
		Assert.AreEqual(n, Note.Parse(n.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnMissingVelocityMarker()
	{
		Assert.IsFalse(Note.TryParse("C4:1/4:80", out Note? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~NoteTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Note.cs`, add `using System;` inside the namespace and add:
```csharp
	/// <summary>Parses a note "{pitch}:{duration}:v{velocity}" (e.g. "C4:1/4:v80").</summary>
	/// <param name="text">The note text.</param>
	/// <returns>The parsed note.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Note Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Note? result)
			? result
			: throw new FormatException($"Invalid note '{text}'.");
	}

	/// <summary>Tries to parse a note "{pitch}:{duration}:v{velocity}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed note, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Note? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] parts = text.Split(':');
		if (parts.Length != 3 || parts[2].Length < 2 || parts[2][0] != 'v')
		{
			return false;
		}

		if (!Pitch.TryParse(parts[0], out Pitch? pitch)
			|| !Duration.TryParse(parts[1], out Duration? duration)
			|| !Velocity.TryParse(parts[2][1..], out Velocity? velocity))
		{
			return false;
		}

		result = Create(pitch, duration, velocity);
		return true;
	}

	/// <summary>Returns "{pitch}:{duration}:v{velocity}" (e.g. "C4:1/4:v80").</summary>
	/// <returns>The canonical note text.</returns>
	public override string ToString() => $"{Pitch}:{Duration}:v{Velocity}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~NoteTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Note.cs Semantics.Test/Music/NoteTests.cs
git commit -m "feat(music): Note canonical ToString + Parse/TryParse"
```

---

### Task 15: Rest canonical ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Rest.cs`
- Test: `Semantics.Test/Music/RestTests.cs`

**Interfaces:**
- Consumes: `Duration` Parse/ToString.
- Produces: `Rest.Parse(string)`, `Rest.TryParse(string?, out Rest?)`, `Rest.ToString()`. Format `R:{duration}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/RestTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class RestTests
{
	[TestMethod]
	public void ToStringIsRColonDuration()
	{
		Assert.AreEqual("R:1/4", Rest.Create(Duration.Quarter).ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Rest r = Rest.Create(Duration.Half);
		Assert.AreEqual(r, Rest.Parse(r.ToString()));
	}

	[TestMethod]
	public void TryParseFailsWithoutRPrefix()
	{
		Assert.IsFalse(Rest.TryParse("1/4", out Rest? result));
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~RestTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Rest.cs`, add `using System;` inside the namespace and add:
```csharp
	/// <summary>Parses a rest "R:{duration}" (e.g. "R:1/4").</summary>
	/// <param name="text">The rest text.</param>
	/// <returns>The parsed rest.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Rest Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Rest? result)
			? result
			: throw new FormatException($"Invalid rest '{text}'.");
	}

	/// <summary>Tries to parse a rest "R:{duration}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed rest, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Rest? result)
	{
		result = null;
		if (text is null || !text.StartsWith("R:", StringComparison.Ordinal))
		{
			return false;
		}

		if (!Duration.TryParse(text[2..], out Duration? duration))
		{
			return false;
		}

		result = Create(duration);
		return true;
	}

	/// <summary>Returns "R:{duration}" (e.g. "R:1/4").</summary>
	/// <returns>The canonical rest text.</returns>
	public override string ToString() => $"R:{Duration}";
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~RestTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Rest.cs Semantics.Test/Music/RestTests.cs
git commit -m "feat(music): Rest canonical ToString + Parse/TryParse"
```

---

### Task 16: Form FromPattern → Parse/TryParse, ToString

**Files:**
- Modify: `Semantics.Music/Form.cs`
- Test: `Semantics.Test/Music/FormTests.cs` (replace `FromPattern` calls), add round-trip

**Interfaces:**
- Produces: `Form.Parse(string)`, `Form.TryParse(string?, out Form?)`, `Form.ToString()`. Removes `Form.FromPattern`.

- [ ] **Step 1: Adjust tests**

In `Semantics.Test/Music/FormTests.cs`, replace `Form.FromPattern(` with `Form.Parse(` and add:
```csharp
	[TestMethod]
	public void ToStringRoundTripsPattern()
	{
		Form form = Form.Parse("AABA");
		Assert.AreEqual("AABA", form.ToString());
		Assert.AreEqual(form, Form.Parse(form.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnLowercase()
	{
		Assert.IsFalse(Form.TryParse("aaba", out Form? result));
		Assert.IsNull(result);
	}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~FormTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Replace FromPattern and add ToString**

In `Semantics.Music/Form.cs`, replace `FromPattern` (lines 62–81) with:
```csharp
	/// <summary>Parses a form from a letter-pattern string.</summary>
	/// <param name="pattern">A pattern of upper-case letters A-Z (e.g. "ABACA").</param>
	/// <returns>The form for the pattern.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="pattern"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the pattern is empty or contains a non-upper-case-letter.</exception>
	public static Form Parse(string pattern)
	{
		Ensure.NotNull(pattern);
		return TryParse(pattern, out Form? result)
			? result
			: throw new FormatException($"Invalid form pattern '{pattern}'.");
	}

	/// <summary>Tries to parse a form from a letter-pattern string.</summary>
	/// <param name="pattern">The text to parse.</param>
	/// <param name="result">The parsed form, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? pattern, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Form? result)
	{
		result = null;
		if (string.IsNullOrEmpty(pattern) || !pattern!.All(c => c is >= 'A' and <= 'Z'))
		{
			return false;
		}

		result = new() { Pattern = pattern, Letters = [.. pattern], Name = RecognizePattern(pattern) };
		return true;
	}

	/// <summary>Returns the letter-pattern (e.g. "AABA").</summary>
	/// <returns>The canonical form text.</returns>
	public override string ToString() => Pattern;
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~FormTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Form.cs Semantics.Test/Music/FormTests.cs
git commit -m "feat(music): rename Form.FromPattern to Parse/TryParse, canonical ToString"
```

---

### Task 17: Progression chart-style ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Progression.cs`
- Test: `Semantics.Test/Music/ProgressionRoundTripTests.cs`

**Interfaces:**
- Consumes: `TimeSignature`, `ChordEvent`, `Chord`, `Duration` (Tasks 6, 7, 12, 13).
- Produces: `Progression.Parse(string)`, `Progression.TryParse(string?, out Progression?)`, `Progression.ToString()`. Chart line `{timesig}  {chords with beat slashes and bar lines}`.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/ProgressionRoundTripTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ProgressionRoundTripTests
{
	[TestMethod]
	public void ToStringRendersBeatSlashesAndBarLines()
	{
		Progression p = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("Dm7"), Duration.Half),
			ChordEvent.Create(Chord.Parse("G7"), Duration.Half),
			ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Whole),
		]);
		Assert.AreEqual("4/4  Dm7 / G7 / | Cmaj7 / / /", p.ToString());
	}

	[TestMethod]
	public void RoundTripWholeBeatDurations()
	{
		Progression p = Progression.Create([Chord.Parse("Am7"), Chord.Parse("D7")], Duration.Quarter);
		Assert.AreEqual(p, Progression.Parse(p.ToString()));
	}

	[TestMethod]
	public void RoundTripSubBeatDurationUsesEscape()
	{
		Progression p = Progression.Create([ChordEvent.Create(Chord.Parse("C"), Duration.Eighth)]);
		Assert.AreEqual("4/4  C@1/8", p.ToString());
		Assert.AreEqual(p, Progression.Parse(p.ToString()));
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionRoundTripTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Progression.cs`, add `using System.Diagnostics.CodeAnalysis;` and `using System.Text;` inside the namespace and add:
```csharp
	/// <summary>Returns the chart-style progression line, e.g. "4/4  Dm7 / G7 / | Cmaj7 / / /".</summary>
	/// <returns>The canonical progression text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append(TimeSignature).Append("  ");

		int beatUnit = TimeSignature.BeatUnit;
		int beatsPerBar = TimeSignature.Beats;
		int beatCursor = 0;
		bool first = true;

		foreach (ChordEvent ev in Chords)
		{
			if (!first && beatCursor % beatsPerBar == 0)
			{
				_ = sb.Append("| ");
			}

			first = false;

			double beatsExact = ev.Duration.AsWholeNotes * beatUnit;
			int beats = (int)System.Math.Round(beatsExact);
			bool wholeBeats = beats >= 1 && System.Math.Abs(beatsExact - beats) < 1e-9;

			if (wholeBeats)
			{
				_ = sb.Append(ev.Chord);
				for (int i = 1; i < beats; i++)
				{
					_ = sb.Append(" /");
				}

				beatCursor += beats;
			}
			else
			{
				_ = sb.Append(ev.Chord).Append('@').Append(ev.Duration);
				beatCursor = 0; // sub-beat durations reset bar tracking (cosmetic only)
			}

			_ = sb.Append(' ');
		}

		return sb.ToString().TrimEnd();
	}

	/// <summary>Parses a chart-style progression line.</summary>
	/// <param name="text">The progression text.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Progression Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Progression? result)
			? result
			: throw new FormatException($"Invalid progression '{text}'.");
	}

	/// <summary>Tries to parse a chart-style progression line.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed progression, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Progression? result)
	{
		result = null;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		string[] tokens = text!.Split((char[]?)null, System.StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length < 2 || !TimeSignature.TryParse(tokens[0], out TimeSignature? ts))
		{
			return false;
		}

		int beatUnit = ts.BeatUnit;
		List<ChordEvent> events = [];
		Chord? current = null;
		int currentBeats = 0;
		Duration? explicitDuration = null;

		bool Flush()
		{
			if (current is null)
			{
				return true;
			}

			Duration duration = explicitDuration ?? Duration.Create(currentBeats, beatUnit);
			events.Add(ChordEvent.Create(current, duration));
			current = null;
			currentBeats = 0;
			explicitDuration = null;
			return true;
		}

		for (int i = 1; i < tokens.Length; i++)
		{
			string token = tokens[i];
			if (token == "|")
			{
				continue;
			}

			if (token == "/")
			{
				if (current is null)
				{
					return false;
				}

				currentBeats++;
				continue;
			}

			_ = Flush();
			int at = token.IndexOf('@');
			if (at >= 0)
			{
				if (!Chord.TryParse(token[..at], out current) || !Duration.TryParse(token[(at + 1)..], out explicitDuration))
				{
					return false;
				}
			}
			else if (!Chord.TryParse(token, out current))
			{
				return false;
			}
			else
			{
				currentBeats = 1;
			}
		}

		_ = Flush();
		if (events.Count == 0)
		{
			return false;
		}

		result = Create(events, ts);
		return true;
	}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ProgressionRoundTripTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Progression.cs Semantics.Test/Music/ProgressionRoundTripTests.cs
git commit -m "feat(music): chart-style Progression ToString + Parse/TryParse"
```

---

### Task 18: Section chart-style ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Section.cs`
- Test: `Semantics.Test/Music/SectionRoundTripTests.cs`

**Interfaces:**
- Consumes: `Progression`, `Key` Parse/ToString, `SectionType` enum.
- Produces: `Section.Parse(string)`, `Section.TryParse(string?, out Section?)`, `Section.ToString()`. Header `[Type] "label" (key)` then a newline then the progression line.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/SectionRoundTripTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class SectionRoundTripTests
{
	private static Progression SampleProgression() =>
		Progression.Create([Chord.Parse("Dm7"), Chord.Parse("G7")], Duration.Half);

	[TestMethod]
	public void ToStringWithLabelAndKey()
	{
		Section s = Section.Create(SectionType.Verse, SampleProgression(), "Verse 1", Key.Create(PitchClass.Create(NoteLetter.C, Accidental.Natural), Mode.Major));
		Assert.AreEqual("[Verse] \"Verse 1\" (C major)\n4/4  Dm7 / G7 /", s.ToString());
	}

	[TestMethod]
	public void RoundTripMinimal()
	{
		Section s = Section.Create(SectionType.Intro, SampleProgression());
		Assert.AreEqual(s, Section.Parse(s.ToString()));
	}

	[TestMethod]
	public void RoundTripWithLabelAndKey()
	{
		Section s = Section.Create(SectionType.Chorus, SampleProgression(), "Chorus", Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian));
		Assert.AreEqual(s, Section.Parse(s.ToString()));
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~SectionRoundTripTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Section.cs`, add `using System;`, `using System.Diagnostics.CodeAnalysis;`, and `using System.Text;` inside the namespace and add:
```csharp
	/// <summary>Returns the chart-style section block: a bracket header then the progression line.</summary>
	/// <returns>The canonical section text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append('[').Append(Type).Append(']');
		if (Label is not null)
		{
			_ = sb.Append(" \"").Append(Label).Append('"');
		}

		if (Key is not null)
		{
			_ = sb.Append(" (").Append(Key).Append(')');
		}

		_ = sb.Append('\n').Append(Progression);
		return sb.ToString();
	}

	/// <summary>Parses a chart-style section block.</summary>
	/// <param name="text">The section text.</param>
	/// <returns>The parsed section.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Section Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Section? result)
			? result
			: throw new FormatException($"Invalid section '{text}'.");
	}

	/// <summary>Tries to parse a chart-style section block.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed section, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Section? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] lines = text.Replace("\r", string.Empty).Split('\n');
		if (lines.Length < 2 || lines[0].Length == 0 || lines[0][0] != '[')
		{
			return false;
		}

		string header = lines[0];
		int close = header.IndexOf(']');
		if (close < 1)
		{
			return false;
		}

		if (!Enum.TryParse<SectionType>(header[1..close], out SectionType type))
		{
			return false;
		}

		string? label = null;
		int quote = header.IndexOf('"', close);
		if (quote >= 0)
		{
			int endQuote = header.IndexOf('"', quote + 1);
			if (endQuote < 0)
			{
				return false;
			}

			label = header[(quote + 1)..endQuote];
		}

		Key? key = null;
		int open = header.IndexOf('(', close);
		if (open >= 0)
		{
			int endParen = header.IndexOf(')', open + 1);
			if (endParen < 0 || !Key.TryParse(header[(open + 1)..endParen], out key))
			{
				return false;
			}
		}

		if (!Progression.TryParse(lines[1], out Progression? progression))
		{
			return false;
		}

		result = Create(type, progression, label, key);
		return true;
	}
```

> Use the explicit-type-argument form `Enum.TryParse<SectionType>(...)`, which is available on every target (including netstandard2.0). No conditional compilation is needed.

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~SectionRoundTripTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Section.cs Semantics.Test/Music/SectionRoundTripTests.cs
git commit -m "feat(music): chart-style Section ToString + Parse/TryParse"
```

---

### Task 19: Arrangement chart-style ToString + Parse/TryParse

**Files:**
- Modify: `Semantics.Music/Arrangement.cs`
- Test: `Semantics.Test/Music/ArrangementRoundTripTests.cs`

**Interfaces:**
- Consumes: `Key`, `Section` Parse/ToString.
- Produces: `Arrangement.Parse(string)`, `Arrangement.TryParse(string?, out Arrangement?)`, `Arrangement.ToString()`. Home-key line, blank line, then blank-line-separated sections.

- [ ] **Step 1: Write the failing test**

`Semantics.Test/Music/ArrangementRoundTripTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ArrangementRoundTripTests
{
	private static Arrangement Sample()
	{
		Key key = Key.Create(PitchClass.Create(NoteLetter.A, Accidental.Natural), Mode.Aeolian);
		Progression intro = Progression.Create([ChordEvent.Create(Chord.Parse("Am"), Duration.Whole)]);
		Progression verse = Progression.Create([Chord.Parse("Am7"), Chord.Parse("Dm7")], Duration.Half);
		return Arrangement.Create(key,
		[
			Section.Create(SectionType.Intro, intro),
			Section.Create(SectionType.Verse, verse, "Verse 1"),
		]);
	}

	[TestMethod]
	public void ToStringHasKeyLineThenSections()
	{
		Assert.AreEqual(
			"A aeolian\n\n[Intro]\n4/4  Am / / /\n\n[Verse] \"Verse 1\"\n4/4  Am7 / Dm7 /",
			Sample().ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Arrangement a = Sample();
		Assert.AreEqual(a, Arrangement.Parse(a.ToString()));
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test --filter "FullyQualifiedName~ArrangementRoundTripTests"`
Expected: FAIL (compile error).

- [ ] **Step 3: Add members**

In `Semantics.Music/Arrangement.cs`, add `using System.Diagnostics.CodeAnalysis;` and `using System.Text;` inside the namespace and add:
```csharp
	/// <summary>Returns the chart: a home-key line then blank-line-separated section blocks.</summary>
	/// <returns>The canonical arrangement text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append(Key);
		foreach (Section section in Sections)
		{
			_ = sb.Append("\n\n").Append(section);
		}

		return sb.ToString();
	}

	/// <summary>Parses a chart-style arrangement.</summary>
	/// <param name="text">The arrangement text.</param>
	/// <returns>The parsed arrangement.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Arrangement Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Arrangement? result)
			? result
			: throw new FormatException($"Invalid arrangement '{text}'.");
	}

	/// <summary>Tries to parse a chart-style arrangement.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed arrangement, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Arrangement? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] blocks = text.Replace("\r", string.Empty).Split(["\n\n"], System.StringSplitOptions.RemoveEmptyEntries);
		if (blocks.Length < 2 || !Key.TryParse(blocks[0].Trim(), out Key? key))
		{
			return false;
		}

		List<Section> sections = [];
		for (int i = 1; i < blocks.Length; i++)
		{
			if (!Section.TryParse(blocks[i], out Section? section))
			{
				return false;
			}

			sections.Add(section);
		}

		result = Create(key, sections);
		return true;
	}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test --filter "FullyQualifiedName~ArrangementRoundTripTests"`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add Semantics.Music/Arrangement.cs Semantics.Test/Music/ArrangementRoundTripTests.cs
git commit -m "feat(music): chart-style Arrangement ToString + Parse/TryParse"
```

---

### Task 20: Update docs and remaining references; full suite

**Files:**
- Modify: `Semantics.Music/README.md`, `docs/superpowers/plans/2026-07-01-music-analysis-aggregate.md`
- Modify (if any remain): `Semantics.Test/Music/FrequencyBridgeTests.cs`, `Semantics.Test/Music/ScorePrimitivesTests.cs`

- [ ] **Step 1: Find any leftover old-name references**

Run: search for `FromName(` and `FromPattern(` across the repo (Grep). Expected: only historical mentions in prose docs; no compiling code.

- [ ] **Step 2: Update remaining test call sites**

In `FrequencyBridgeTests.cs` and `ScorePrimitivesTests.cs`, replace any `Pitch.FromName(` with `Pitch.Parse(` (and any `Mode.FromName`/`Form.FromPattern` similarly). These may already have been updated in Tasks 3/4/16; only edit what remains.

- [ ] **Step 3: Update README examples**

In `Semantics.Music/README.md`, replace `FromName`/`FromPattern` usages with `Parse`, and add a short section showing the typed factories and canonical strings:
```markdown
## Type-safe construction and round-trippable text

```csharp
// Compiler-enforced construction
Pitch p = Pitch.Create(NoteLetter.C, Accidental.Sharp, octave: 4); // C#4

// Parse / ToString round-trip (canonical output)
Chord c = Chord.Parse("Cmaj7");
Chord same = Chord.Parse(c.ToString());   // c == same

// Progressions read like a lead sheet
Progression prog = Progression.Parse("4/4  Dm7 / G7 / | Cmaj7 / / /");
```
```

- [ ] **Step 4: Update the analysis-aggregate plan doc**

In `docs/superpowers/plans/2026-07-01-music-analysis-aggregate.md`, update the three `FromName`/`FromPattern` mentions to the new `Parse` names so the historical doc stays accurate.

- [ ] **Step 5: Run the full suite and build**

Run: `dotnet build` then `dotnet test`
Expected: build clean (warnings-as-errors satisfied), all tests PASS.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Music/README.md docs/superpowers/plans/2026-07-01-music-analysis-aggregate.md Semantics.Test/Music/FrequencyBridgeTests.cs Semantics.Test/Music/ScorePrimitivesTests.cs
git commit -m "docs(music): update examples and references for Parse/TryParse rename and canonical ToString"
```

---

## Self-Review

**Spec coverage:**
- `NoteLetter`/`Accidental` enums → Task 1. Typed factories (`PitchClass`, `Pitch`) → Tasks 2, 3. Renames (`Pitch`/`Mode`/`Form`) + `Chord.TryParse` → Tasks 3, 4, 16, 12. Internal parse routing dedup (`Notation`, `Chord.ParseRoot`, `Key` numeral) → Tasks 2, 11, 12. Canonical `ToString`+`Parse`/`TryParse` for every in-scope type → Tasks 2–19. Chart-style aggregates → Tasks 17–19. Round-trip tests → each task + the corpus test in Task 12. Docs/blast-radius → Task 20. Mode literal note → Task 4. Covered.
- Out-of-scope items (analysis result records, `ModeName` enum, roman-numeral typed factory, `IParsable<T>`, melodic-sequence aggregate) are correctly not implemented.

**Placeholder scan:** No TBD/TODO; every code step shows complete code; the `Chord` formatter is complete with the corpus test as its gate.

**Type consistency:** `TryParse(string?, out T?)` shape is uniform; `Parse` delegates to `TryParse` everywhere; `Notation.TryReadNoteLetter`/`ReadAccidentalOffset` signatures match their consumers in Tasks 2, 3, 11, 12; no conditional compilation is used (`Enum.TryParse<SectionType>` works on all targets).

One risk to watch during execution: the `Chord` canonical formatter (Task 12) may need iteration to make every corpus symbol round-trip — the test is the gate, adjust the formatter (never the parser) until green.
