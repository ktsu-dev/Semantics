# Strings and Paths Benchmark Charts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Give `Semantics.Strings` and `Semantics.Paths` the same measured, charted, per-release performance history that `Semantics.Quantities` already has.

**Architecture:** The existing pipeline is single-subject by construction. Task 1 makes the renderer subject-aware while keeping the quantities chart byte-identical. Tasks 2 to 6 add benchmark classes to the existing `Semantics.Benchmarks` project. Tasks 7 and 8 register the two new subjects with the renderer and the workflow. Tasks 9 and 10 seed the histories locally. Task 11 writes the documentation, using real numbers from the seeding run.

**Tech Stack:** .NET 10, BenchmarkDotNet, MSTest (existing suite, untouched), a file-based C# app (`scripts/benchmark-history.cs`) run by `dotnet run`, GitHub Actions.

**Spec:** `docs/superpowers/specs/2026-09-18-strings-paths-benchmarks-design.md`

## Global Constraints

Copied from the spec and from `CLAUDE.md`. Every task's requirements include these.

- **Indentation is tabs** in C# files. Line endings CRLF for C# sources, LF for generated output and markdown (`.gitattributes` sets `* text=auto eol=lf`).
- **File header on every new C# file:** `// Copyright (c) 2023-2026 ktsu-dev contributors` as the first line, followed by a blank line.
- **File-scoped namespaces.** Using directives go inside the namespace. No `this.` qualifiers. Explicit accessibility modifiers everywhere. Braces on every control flow statement.
- **Warnings are errors** (ktsu.Sdk default). `dotnet build` must be clean.
- **No global warning suppressions.** Use targeted `[SuppressMessage]` with a justification if one is genuinely needed.
- **No `var` in test bodies.** Benchmark bodies follow the same convention as the existing suite, which uses explicit types throughout.
- **The benchmark project touches no internal member.** `InternalsVisibleTo` names only `ktsu.Semantics.Test`, and a benchmark built on internals could only ever measure the working copy, never a published package.
- **Every benchmark operand is a field set in `[GlobalSetup]`,** never a literal in the benchmark body, so the JIT cannot constant-fold the work away.
- **`BenchmarkAgainstVersion` is set in the environment, never with `-p:`.** BenchmarkDotNet builds a generated project of its own that a command-line property never reaches.
- **Commit message version tag:** the final implementation commit carries `[patch]`. Intermediate commits carry no tag.
- **Do not edit `BaselineBenchmarks.ReferenceWork`.** Its body is the fixed point that makes timings from different machines comparable. Editing it silently rescales every comparison against history recorded before the edit.
- **Generated output is committed source.** `docs/benchmarks/*.json` and `docs/benchmarks/*.svg` are diffed before commit, never assumed.

---

### Task 1: Make the renderer subject-aware

The gate for everything after it. The quantities chart must come out byte-identical.

**Files:**
- Modify: `scripts/benchmark-history.cs:25` (the `Columns` constant), `:54-64` (the `Headline` array), `:391-427` (`Render`), `:429-441` (`Draw`), `:443-459` (`Preamble`), `:461-495` (`Section`)
- Test: no test file. The regression check is byte equality of the two committed SVG files, run as a shell command in Step 2 and Step 5.

**Interfaces:**
- Consumes: nothing from earlier tasks.
- Produces: `render --subject <name> --history <path> --out <path>`, where `<name>` is a key of the `Subjects` dictionary. Task 7 adds `"strings"` and `"paths"` keys. Task 8 calls this from the workflow. The two record types other tasks reference by name are `Subject(string Title, int Columns, Panel[] Headline)` and `Panel(string Key, string? Parameters, string Label)`.

- [ ] **Step 1: Record the current chart bytes as the regression baseline**

```bash
cd /c/dev/ktsu-dev/Semantics
mkdir -p "$TMPDIR/chartcheck"
cp docs/benchmarks/performance.svg "$TMPDIR/chartcheck/performance.svg.expected"
cp docs/benchmarks/performance-dark.svg "$TMPDIR/chartcheck/performance-dark.svg.expected"
```

- [ ] **Step 2: Confirm the baseline reproduces before any edit**

Proves the check is meaningful rather than vacuous. If the committed SVG files do not reproduce from the committed history *before* the refactor, stop and report that, because then byte equality cannot gate anything.

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render \
  --history docs/benchmarks/history.json --out docs/benchmarks/performance.svg
git diff --stat docs/benchmarks/
```

Expected: `git diff --stat` reports no changes.

- [ ] **Step 3: Replace the `Headline` array and `Columns` constant with a subject table**

Delete the `private const int Columns = 4;` line. Replace the whole `Headline` field, XML documentation comment included, with the following. The `<remarks>` prose is moved verbatim from the old `Headline` comment onto the `quantities` entry, because it explains that subject's panel choice and nothing else.

```csharp
	/// <summary>One drawable chart: its title, its grid width, and the panels it shows.</summary>
	/// <param name="Title">The library the chart is about, used in the heading and the aria-label.</param>
	/// <param name="Columns">Panels per row.</param>
	/// <param name="Headline">The panels, in reading order.</param>
	private sealed record Subject(string Title, int Columns, Panel[] Headline);

	/// <summary>One panel: which benchmark it draws, and what to call it.</summary>
	/// <param name="Key">The benchmark key as <c>ingest</c> stores it.</param>
	/// <param name="Parameters">The parameter case to draw, or null when the benchmark has none.</param>
	/// <param name="Label">The panel heading.</param>
	private sealed record Panel(string Key, string? Parameters, string Label);

	/// <summary>The charts this script can draw, by the name <c>--subject</c> takes.</summary>
	/// <remarks>
	/// <para>
	/// Everything measured is stored; this only decides what each picture shows, so it can change
	/// without re-running anything.
	/// </para>
	/// <para>
	/// <b>Quantities</b> is drawn as a grid of one operation per storage type rather than of every
	/// operation at one storage type. A quantity is a value type over <c>T</c> and does almost
	/// nothing of its own, so what a release changes it changes per storage type — and the same
	/// line of user code costs four different things depending on the <c>T</c> it was written
	/// against. The top row is one construction across the four, so that row reads as the
	/// comparison it is.
	/// </para>
	/// <para>
	/// None of the eight is a bare operator, although the suite measures those too. A relationship
	/// operator over a binary floating point type is a single machine instruction on values the
	/// loop does not change, so the JIT hoists it out and BenchmarkDotNet reports it as
	/// indistinguishable from an empty method. That is a true answer about the library and a
	/// useless one to plot: a panel of it would chart the harness's resolution rather than any
	/// release. What is drawn instead is the work around an operator — building a quantity from a
	/// unit, reading it back out in one, a vector length, a comparison — all of which are far
	/// enough above that floor to move when the library does.
	/// </para>
	/// </remarks>
	private static readonly Dictionary<string, Subject> Subjects = new(StringComparer.Ordinal)
	{
		["quantities"] = new("Semantics.Quantities", 4,
		[
			new("ConstructionBenchmarks<Double>.FromNauticalMile", null, "Construct (double)"),
			new("ConstructionBenchmarks<Single>.FromNauticalMile", null, "Construct (float)"),
			new("ConstructionBenchmarks<Decimal>.FromNauticalMile", null, "Construct (decimal)"),
			new("ConstructionBenchmarks<PreciseNumber>.FromNauticalMile", null, "Construct (precise)"),
			new("UnitConversionBenchmarks<Decimal>.InNauticalMile", null, "Read back (decimal)"),
			new("UnitConversionBenchmarks<PreciseNumber>.InNauticalMile", null, "Read back (precise)"),
			new("VectorBenchmarks<Decimal>.Length", null, "Vector length (decimal)"),
			new("ComparisonBenchmarks<Double>.CompareToInterface", null, "CompareTo (double)"),
		]),
	};
```

- [ ] **Step 4: Thread the subject through `Render`, `Draw`, `Preamble` and `Section`**

Four signature changes and six body edits. Nothing else in the file moves.

In `Render`, immediately after the existing `string historyPath = Required(options, "history");` line, add the lookup, then pass `subject` to `Draw`:

```csharp
		string subjectName = Required(options, "subject");
		if (!Subjects.TryGetValue(subjectName, out Subject? subject))
		{
			throw new InvalidOperationException(
				$"Unknown subject '{subjectName}'. Known: {string.Join(", ", Subjects.Keys)}");
		}
```

The existing `File.WriteAllText(path, Draw(entries, Themes[name]));` becomes:

```csharp
			File.WriteAllText(path, Draw(entries, Themes[name], subject));
```

`Draw` takes the subject and reads its columns and panel count from it:

```csharp
	private static string Draw(JsonArray entries, Theme theme, Subject subject)
	{
		string[] labels = [.. entries.Select(entry => entry!["version"]?.GetValue<string>() ?? "?")];
		int width = Left + (subject.Columns * CellWidth) + 24;
		int rows = (subject.Headline.Length + subject.Columns - 1) / subject.Columns;
		int height = 72 + (((34 + (rows * CellHeight)) * 2) + 54);

		StringBuilder svg = new();
		Preamble(svg, theme, width, height, entries, subject);

		int y = 72;
		foreach (bool isTime in (bool[])[false, true])
		{
			Section(svg, theme, entries, labels.Length, y, isTime, subject);
			y += 34 + (rows * CellHeight);
		}

		Footer(svg, entries, labels, y - 4);
		svg.AppendLine("</svg>");
		return svg.ToString();
	}
```

In `Preamble`, add `Subject subject` as the last parameter and replace the two hardcoded strings:

```csharp
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" viewBox="0 0 {width} {height}" role="img" aria-label="{Escape(subject.Title)} allocation and relative time per release">""");
```

```csharp
		svg.AppendLine(CultureInfo.InvariantCulture, $"""<text x="{Left}" y="28" class="title">{Escape(subject.Title)} performance by release</text>""");
```

In `Section`, add `Subject subject` as the last parameter and change the panel loop's three references:

```csharp
		for (int position = 0; position < subject.Headline.Length; position++)
		{
			(string key, string? parameters, string label) = subject.Headline[position];
			double?[] values = [.. entries.Select(entry => Value(entry!, key, parameters, isTime))];
			Panel(
				svg,
				Left + (position % subject.Columns * CellWidth),
				y + 26 + (position / subject.Columns * CellHeight),
				label + (parameters is null ? "" : $" ({parameters} digits)"),
				points,
				values,
				isTime,
				colour,
				theme);
		}
```

Note: the existing private method `Panel(...)` that draws a panel now shares its name with the new `Panel` record. C# resolves the method call and the type usage without ambiguity here, but if the compiler complains, rename the **record** to `PanelSpec` and update the three references in `Subject`, `Subjects` and the loop. Do not rename the drawing method, which is referenced elsewhere.

- [ ] **Step 5: Verify byte equality**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render --subject quantities \
  --history docs/benchmarks/history.json --out docs/benchmarks/performance.svg
diff docs/benchmarks/performance.svg "$TMPDIR/chartcheck/performance.svg.expected"
diff docs/benchmarks/performance-dark.svg "$TMPDIR/chartcheck/performance-dark.svg.expected"
git diff --stat docs/benchmarks/
```

Expected: both `diff` commands produce no output, and `git diff --stat` reports no changes. **If a single byte moved, the refactor is wrong. Fix it before continuing rather than accepting the new bytes.**

- [ ] **Step 6: Verify the unknown-subject error**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render --subject nosuch \
  --history docs/benchmarks/history.json --out /tmp/x.svg; echo "exit=$?"
```

Expected: `Unknown subject 'nosuch'. Known: quantities` on standard error, `exit=2`.

- [ ] **Step 7: Run the Sonar analyzers over the change**

The script is not `SonarQubeExclude`d, and CI will analyze it. Catching findings here saves a ten-minute round trip.

```powershell
cd C:\dev\ktsu-dev\Semantics
dotnet build -p:CustomAfterMicrosoftCommonProps=$PWD\.sonarlint\sonar-local.props
```

Expected: build succeeds with no new warnings.

- [ ] **Step 8: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add scripts/benchmark-history.cs
git commit -m "Make the release chart renderer subject-aware

The headline set, the grid width and the two title strings move onto a
Subject record looked up by --subject. Ingest is untouched: it never
read either, and the results directory is already per-subject because
the filter selected it.

The quantities chart renders byte-identical, which is the whole test."
```

---

### Task 2: String specimens and creation benchmarks

**Files:**
- Modify: `Semantics.Benchmarks/Semantics.Benchmarks.csproj` (both `ItemGroup`s that `BenchmarkAgainstVersion` switches between)
- Create: `Semantics.Benchmarks/Strings/StringSpecimens.cs`
- Create: `Semantics.Benchmarks/Strings/StringCreationBenchmarks.cs`
- Test: no test file. Verification is a `--job short` run whose summary shows a measurement for every method and no `NA` or `ZeroMeasurement`.

**Interfaces:**
- Consumes: nothing from earlier tasks.
- Produces: `internal static class StringSpecimens` with `internal const string` members `PlainInput`, `UuidText`, `UuidPattern`, `UuidRejected`, `UlidText`, `CardText`, `IbanText`, and the public type `PlainText`. Tasks 3 and 4 use all of them. Benchmark keys `StringCreationBenchmarks.Unvalidated`, `.CharsetRegex`, `.FormatRegex`, `.Checksum`, `.Mod97`, `.TryCreateRejects`, `.CreateThrows`, which Task 7 draws.

- [ ] **Step 1: Add the three package references to the benchmark project**

`BenchmarkAgainstVersion` switches between a project-reference group and a package-reference group. Both grow to four entries. All four are listed on both sides even though `Semantics.Paths` transitively brings `Semantics.Strings`, because the package side must name all four regardless and two lists differing in membership invite the wrong edit later.

Replace the two existing `ItemGroup` elements at the bottom of `Semantics.Benchmarks/Semantics.Benchmarks.csproj` with:

```xml
  <ItemGroup Condition="'$(BenchmarkAgainstVersion)' == ''">
    <ProjectReference Include="..\Semantics.Quantities\Semantics.Quantities.csproj" />
    <ProjectReference Include="..\Semantics.Strings\Semantics.Strings.csproj" />
    <ProjectReference Include="..\Semantics.Strings.Identifiers\Semantics.Strings.Identifiers.csproj" />
    <ProjectReference Include="..\Semantics.Paths\Semantics.Paths.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(BenchmarkAgainstVersion)' != ''">
    <PackageReference Include="ktsu.Semantics.Quantities" VersionOverride="$(BenchmarkAgainstVersion)" />
    <PackageReference Include="ktsu.Semantics.Strings" VersionOverride="$(BenchmarkAgainstVersion)" />
    <PackageReference Include="ktsu.Semantics.Strings.Identifiers" VersionOverride="$(BenchmarkAgainstVersion)" />
    <PackageReference Include="ktsu.Semantics.Paths" VersionOverride="$(BenchmarkAgainstVersion)" />
  </ItemGroup>
```

- [ ] **Step 2: Verify the project still builds both ways**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet build -c Release Semantics.Benchmarks
BenchmarkAgainstVersion=5.3.4 dotnet build -c Release Semantics.Benchmarks
```

Expected: both succeed. The second proves all four packages exist at that version on nuget.org, which the backfill in Task 10 depends on.

- [ ] **Step 3: Write the specimens file**

Create `Semantics.Benchmarks/Strings/StringSpecimens.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using ktsu.Semantics.Strings;

/// <summary>
/// A semantic string that declares no validation at all.
/// </summary>
/// <remarks>
/// The one specimen in this suite that is not a shipped type, and it has to be.
/// <c>Semantics.Strings</c> ships the validation framework and no concrete types, so nothing
/// shipped occupies the no-validation rung — and that rung is what every other measurement is read
/// against, being the reflection machinery alone with no validator behind it. Declared against the
/// public API only, like everything else here.
/// </remarks>
public sealed record PlainText : SemanticString<PlainText>;

/// <summary>
/// The input strings the string benchmarks run against.
/// </summary>
/// <remarks>
/// Every value here is valid for the type it is paired with, except <see cref="UuidRejected"/>,
/// and each is held as a constant so that a benchmark body reads as one call rather than as a
/// literal the JIT might fold. The benchmark classes copy these into fields in
/// <c>[GlobalSetup]</c> for the same reason.
/// </remarks>
internal static class StringSpecimens
{
	/// <summary>Input for the unvalidated rung. Length is in the same range as the others.</summary>
	internal const string PlainInput = "0123456789abcdef0123456789abcdef0123";

	/// <summary>A canonical lowercase RFC 4122 identifier, which <c>IsUuid</c> accepts as written.</summary>
	internal const string UuidText = "123e4567-e89b-12d3-a456-426614174000";

	/// <summary>
	/// The pattern <c>IsUuidAttribute</c> uses, repeated here for the hand-written side of the
	/// cost pairs in <c>StringAbstractionCostBenchmarks</c>. It must stay identical to the one in
	/// <c>Semantics.Strings.Identifiers</c>, or that pair stops being a comparison.
	/// </summary>
	internal const string UuidPattern =
		"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";

	/// <summary>The same identifier one character short, so the pattern rejects it.</summary>
	internal const string UuidRejected = "123e4567-e89b-12d3-a456-42661417400";

	/// <summary>A 26-character Crockford base32 identifier, which <c>IsUlid</c> accepts.</summary>
	internal const string UlidText = "01ARZ3NDEKTSV4RRFFQ69G5FAV";

	/// <summary>A 16-digit number that passes the Luhn check. Not a real card.</summary>
	internal const string CardText = "4111111111111111";

	/// <summary>The standard example account number, which passes the mod-97 check.</summary>
	internal const string IbanText = "GB82WEST12345698765432";
}
```

- [ ] **Step 4: Write the creation benchmarks**

Create `Semantics.Benchmarks/Strings/StringCreationBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using System;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures creating a semantic string, across the range of validation a type can declare.
/// </summary>
/// <remarks>
/// <para>
/// <b>The axis here is validation weight.</b> A semantic string is a record wrapping a
/// <see cref="string"/>, so its cost is concentrated at creation, and what differs between types
/// is what their attributes do. <see cref="Unvalidated"/> is the floor: the reflection machinery
/// alone, which is <c>Activator.CreateInstance</c>, a <c>GetProperty</c>, a
/// <c>PropertyInfo.SetValue</c>, and a strategy lookup that finds no attributes. Every other row
/// is that floor plus one validator, so the difference between rows is the validator.
/// </para>
/// <para>
/// The validators are shipped types rather than fixtures, so the numbers describe types a user
/// actually holds. They also happen to span the interesting ground: <see cref="CharsetRegex"/> and
/// <see cref="FormatRegex"/> are interpreted regular expressions looked up from the static cache
/// on every call and carrying a one-second timeout, <see cref="Checksum"/> is a hand-written Luhn
/// pass over the same sort of input, and <see cref="Mod97"/> a rearrange-expand-and-modulo pass.
/// </para>
/// <para>
/// <b>Both failure paths are here.</b> <see cref="TryCreateRejects"/> and
/// <see cref="CreateThrows"/> reject the same input, so the pair is the cost of choosing
/// <c>Create</c> over <c>TryCreate</c> at a boundary that sees bad input — a number rather than a
/// guess. <see cref="CreateThrows"/> catches the exception inside the benchmark on purpose: the
/// throw and the catch together are what a caller pays.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class StringCreationBenchmarks
{
	private string plain = "";
	private string uuid = "";
	private string uuidRejected = "";
	private string ulid = "";
	private string card = "";
	private string iban = "";

	/// <summary>Copies the inputs into fields, so no benchmark body holds a literal.</summary>
	[GlobalSetup]
	public void Setup()
	{
		plain = StringSpecimens.PlainInput;
		uuid = StringSpecimens.UuidText;
		uuidRejected = StringSpecimens.UuidRejected;
		ulid = StringSpecimens.UlidText;
		card = StringSpecimens.CardText;
		iban = StringSpecimens.IbanText;
	}

	/// <summary>The reflection machinery with no validator behind it.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public PlainText Unvalidated() => PlainText.Create(plain);

	/// <summary>That floor plus an interpreted regular expression over a fixed character set.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Ulid CharsetRegex() => Ulid.Create(ulid);

	/// <summary>The same, over a pattern with groups and separators.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Uuid FormatRegex() => Uuid.Create(uuid);

	/// <summary>That floor plus a hand-written Luhn pass, for regular expressions against arithmetic.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public CreditCardNumber Checksum() => CreditCardNumber.Create(card);

	/// <summary>A rearrangement, a character-to-digit expansion, and modular arithmetic.</summary>
	/// <returns>The created value.</returns>
	[Benchmark]
	public Iban Mod97() => Iban.Create(iban);

	/// <summary>The failure path that does not throw.</summary>
	/// <returns>Whether creation succeeded, which is always false here.</returns>
	[Benchmark]
	public bool TryCreateRejects() => Uuid.TryCreate(uuidRejected, out _);

	/// <summary>The failure path that does, throw and catch together.</summary>
	/// <returns>Whether the expected exception was raised, which is always true here.</returns>
	[Benchmark]
	public bool CreateThrows()
	{
		try
		{
			_ = Uuid.Create(uuidRejected);
			return false;
		}
		catch (ArgumentException)
		{
			return true;
		}
	}
}
```

- [ ] **Step 5: Run the class and read the summary**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*StringCreationBenchmarks*' --job short
```

Expected: seven rows, each with a mean in nanoseconds and an allocation figure. **Check for and report:**
- Any row reading `NA` means the benchmark threw. The most likely cause is a specimen the validator rejects. Fix the specimen, do not remove the row.
- Any `ZeroMeasurement` warning means the JIT hoisted the work. None is expected here, since every path allocates.
- `Unvalidated` should be the fastest row and `Mod97` the slowest. If that ordering does not hold, stop and report it rather than continuing, because it means the benchmark is not measuring what this task claims.

- [ ] **Step 6: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add Semantics.Benchmarks/Semantics.Benchmarks.csproj Semantics.Benchmarks/Strings/
git commit -m "Measure creating a semantic string across the validation ladder

Seven rungs from the reflection machinery alone up to the mod-97 check,
using shipped identifier types so the numbers describe types a caller
actually holds. The one fixture is the no-validation floor, which
nothing shipped occupies.

Both failure paths are measured, so the cost of Create over TryCreate at
a boundary that sees bad input is a number."
```

---

### Task 3: String operation benchmarks

**Files:**
- Create: `Semantics.Benchmarks/Strings/StringOperationBenchmarks.cs`
- Test: no test file. Verification is the `--job short` run in Step 2.

**Interfaces:**
- Consumes: `StringSpecimens.UuidText`, `StringSpecimens.PlainInput`, and the `PlainText` type from Task 2.
- Produces: benchmark keys `StringOperationBenchmarks.EqualityOperator`, `.CompareTo`, `.HashCode`, `.AsConversion`, `.WithSuffix`, `.ToStringImplicit`. Task 7 draws `.AsConversion` and `.HashCode` (`.CompareTo` was measured at 0.5714 ns and hoisted, so it is not drawn).

- [ ] **Step 1: Write the class**

Note two shapes that are not arbitrary. The equality method is named `EqualityOperator` rather than `Equals`, because a method called `Equals` on the class would hide `object.Equals` and draw a compiler warning, which is an error here. And `WithSuffix` runs on `PlainText` rather than on `Uuid`, because appending to a `Uuid` produces a value its own validator rejects and the benchmark would throw on every invocation.

Create `Semantics.Benchmarks/Strings/StringOperationBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures what a semantic string costs after it has been created.
/// </summary>
/// <remarks>
/// <para>
/// Creation is where a semantic string spends its cost, and the rest of this suite says so. This
/// class is the other half of that claim: once a value exists, an operation on it should be the
/// underlying <see cref="string"/>'s own work plus a wrapper, and the rows here are how far that
/// holds.
/// </para>
/// <para>
/// <b>Two of them are creation in disguise, which is the point of including them.</b>
/// <see cref="AsConversion"/> and <see cref="WithSuffix"/> both route back through
/// <c>Create</c> on the target type, so each pays the full reflection and validation cost a
/// <c>StringCreationBenchmarks</c> row measures. They read as ordinary member calls at a call
/// site, and they are not, and that is worth being able to point at.
/// </para>
/// <para>
/// <see cref="WithSuffix"/> runs on <see cref="PlainText"/> rather than on an identifier, because
/// appending to a <see cref="Uuid"/> produces a value its own validator rejects. Measuring the
/// throw is <c>StringCreationBenchmarks.CreateThrows</c>'s job, not this one's.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class StringOperationBenchmarks
{
	private Uuid left = null!;
	private Uuid right = null!;
	private PlainText plain = null!;
	private string suffix = "";

	/// <summary>Builds the operands once, outside the measurement.</summary>
	[GlobalSetup]
	public void Setup()
	{
		left = Uuid.Create(StringSpecimens.UuidText);
		right = Uuid.Create(StringSpecimens.UuidText);
		plain = PlainText.Create(StringSpecimens.PlainInput);
		suffix = "-suffixed";
	}

	/// <summary>Record equality over two equal values, which is the worst case for it.</summary>
	/// <returns>Whether the two are equal, which is always true here.</returns>
	[Benchmark]
	public bool EqualityOperator() => left == right;

	/// <summary>Ordering, which routes through the underlying string's comparison.</summary>
	/// <returns>The comparison result.</returns>
	[Benchmark]
	public int CompareTo() => left.CompareTo(right);

	/// <summary>Hashing, which a dictionary of semantic strings pays on every lookup.</summary>
	/// <returns>The hash code.</returns>
	[Benchmark]
	public int HashCode() => left.GetHashCode();

	/// <summary>Cross-type conversion, which is a full creation against the target type.</summary>
	/// <returns>The converted value.</returns>
	[Benchmark]
	public PlainText AsConversion() => left.As<PlainText>();

	/// <summary>Appending, which is also a full creation against the same type.</summary>
	/// <returns>The extended value.</returns>
	[Benchmark]
	public PlainText WithSuffix() => plain.WithSuffix(suffix);

	/// <summary>The implicit conversion back out, which should be a field read.</summary>
	/// <returns>The underlying string.</returns>
	[Benchmark]
	public string ToStringImplicit() => left;
}
```

- [ ] **Step 2: Run the class and read the summary**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*StringOperationBenchmarks*' --job short
```

Expected: six rows with measurements. **Check for and report:**
- `ToStringImplicit` is the row most likely to report `ZeroMeasurement`, being a field read the JIT can hoist. If it does, leave it in the class but note it, because Task 7 does not draw it.
- `EqualityOperator` and `CompareTo` are the next most likely. **If either reports `ZeroMeasurement`, Task 7 must replace the `CompareTo` panel with `HashCode`.** Record which happened, because Task 7 depends on the answer.
- `AsConversion` and `WithSuffix` should each cost roughly what a `StringCreationBenchmarks` creation row costs. If either is dramatically cheaper, the call is being optimized away and the row is not measuring what it claims.

- [ ] **Step 3: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add Semantics.Benchmarks/Strings/StringOperationBenchmarks.cs
git commit -m "Measure a semantic string after it exists

Equality, ordering, hashing and the conversion back out, plus the two
members that read as ordinary calls and are really full creations
against the target type."
```

---

### Task 4: String cost pairs

**Files:**
- Create: `Semantics.Benchmarks/Strings/StringAbstractionCostBenchmarks.cs`
- Test: no test file. Verification is the `--job short` run in Step 2, read for the `Ratio` column.

**Interfaces:**
- Consumes: `StringSpecimens.UuidText`, `.UuidPattern`, `.UuidRejected` from Task 2.
- Produces: a `Ratio` column per category, which Task 11 copies into `Semantics.Benchmarks/README.md`. Nothing later depends on the benchmark keys, because this class is never plotted.

- [ ] **Step 1: Write the class**

The pairing is the substance of this task, so the reasoning is in the class documentation rather than only in the spec.

Create `Semantics.Benchmarks/Strings/StringAbstractionCostBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Strings;

using System;
using System.Text.RegularExpressions;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Measures what the semantic string types cost over the code a caller would otherwise write.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the baseline side is, and why it is not a bare string.</b> The quantities suite pairs
/// <c>T + T</c> against <c>Length&lt;T&gt; + Length&lt;T&gt;</c>, which is fair because both sides
/// do identical work and the only question is what the wrapper adds. There is no such pairing for
/// <c>Uuid.Create</c>: the bare-string counterpart is an assignment, which is no work at all.
/// Against that, the ratio would be a large number restating only that validation is not free,
/// which needs no benchmark to establish.
/// </para>
/// <para>
/// So the baseline side here is the code a caller would otherwise have written — the same pattern
/// matched by hand, and the same throw on failure. Read that way the ratio answers the question a
/// caller actually has: <i>I was going to validate this anyway, so what does routing it through
/// the type cost me on top?</i> The answer separates into the validation both sides pay and the
/// per-call reflection only one side does.
/// </para>
/// <para>
/// <b>The pattern is duplicated on purpose.</b> <c>StringSpecimens.UuidPattern</c> is the same
/// string <c>IsUuidAttribute</c> holds. If the two ever drift the comparison stops being one, so
/// it is worth saying here: the constant exists to be kept identical, not to be tuned.
/// </para>
/// <para>
/// <b>The last two categories are fair pairs in the quantities sense</b> and are expected near
/// 1.00, because a semantic string's equality and ordering are the underlying string's own.
/// </para>
/// <para>
/// <b>Why these are loops.</b> A single call over an operand that does not change is
/// loop-invariant and the JIT hoists it, and a ratio between two hoisted methods means nothing.
/// Each iteration here feeds the next through an accumulator, so there is nothing to hoist and
/// both sides of a pair stay measurable. Both sides also pay the same counter and branch, which
/// pulls the ratio toward 1.00 rather than away from it, so a ratio above 1.00 is a floor on the
/// real cost rather than the whole of it.
/// </para>
/// </remarks>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class StringAbstractionCostBenchmarks
{
	/// <summary>
	/// Operations per invocation. Enough that the loop's own cost is a small share of the work.
	/// </summary>
	private const int Operations = 64;

	private string text = "";
	private string rejected = "";
	private string pattern = "";
	private Uuid left = null!;
	private Uuid right = null!;

	/// <summary>Prepares both sides of every pair.</summary>
	[GlobalSetup]
	public void Setup()
	{
		text = StringSpecimens.UuidText;
		rejected = StringSpecimens.UuidRejected;
		pattern = StringSpecimens.UuidPattern;
		left = Uuid.Create(StringSpecimens.UuidText);
		right = Uuid.Create(StringSpecimens.UuidText);
	}

	/// <summary>Validating at a boundary by hand, throwing on rejection.</summary>
	/// <returns>The accumulated length, returned so nothing here is dead code.</returns>
	[BenchmarkCategory("Validate")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareValidate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Regex.IsMatch(text, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				throw new ArgumentException("unreachable: the specimen is valid");
			}

			accumulator += text.Length;
		}

		return accumulator;
	}

	/// <summary>Validating at a boundary through the type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("Validate")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticValidate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Uuid.Create(text).Length;
		}

		return accumulator;
	}

	/// <summary>Rejecting by hand without throwing.</summary>
	/// <returns>The count of rejections, which is every iteration.</returns>
	[BenchmarkCategory("Reject")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareReject()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Regex.IsMatch(rejected, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Rejecting through the type without throwing.</summary>
	/// <returns>The count of rejections, which is every iteration.</returns>
	[BenchmarkCategory("Reject")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticReject()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (!Uuid.TryCreate(rejected, out _))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Ordinal equality on the bare strings.</summary>
	/// <returns>The count of matches, which is every iteration.</returns>
	[BenchmarkCategory("Equality")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareEquality()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (string.Equals(left.WeakString, right.WeakString, StringComparison.Ordinal))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Record equality on the semantic values holding those strings.</summary>
	/// <returns>The count of matches, which is every iteration.</returns>
	[BenchmarkCategory("Equality")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticEquality()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (left == right)
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Ordering on the bare strings.</summary>
	/// <returns>The accumulated comparison results.</returns>
	[BenchmarkCategory("Ordering")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareOrdering()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += left.WeakString.CompareTo(right.WeakString);
		}

		return accumulator;
	}

	/// <summary>Ordering on the semantic values.</summary>
	/// <returns>The accumulated comparison results.</returns>
	[BenchmarkCategory("Ordering")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticOrdering()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += left.CompareTo(right);
		}

		return accumulator;
	}
}
```

- [ ] **Step 2: Run the class and read the ratios**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*StringAbstractionCostBenchmarks*' --job short
```

Expected: eight rows in four categories, each category showing a `Ratio` column with the bare row at 1.00.

**Record the four ratios and the four allocation figures. Task 11 writes them into the benchmark README verbatim, so copy them rather than rounding from memory.**

Sanity expectations, to be reported if violated rather than silently accepted:
- `Validate` and `Reject` ratios above 1.00, because the semantic side does the same regular expression plus reflection.
- `Equality` and `Ordering` ratios near 1.00.
- The semantic side of `Validate` allocates and the bare side does not, because a semantic string is a reference type.

- [ ] **Step 3: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add Semantics.Benchmarks/Strings/StringAbstractionCostBenchmarks.cs
git commit -m "Pair the string types against hand-written validation

Not against a bare string: the bare counterpart of Uuid.Create is an
assignment, and a ratio against no work at all would only restate that
validation is not free. Paired against the check a caller would have
written anyway, the ratio separates the validation both sides pay from
the reflection only one side does."
```

---

### Task 5: Path benchmarks

**Files:**
- Create: `Semantics.Benchmarks/Paths/PathSpecimens.cs`
- Create: `Semantics.Benchmarks/Paths/PathCreationBenchmarks.cs`
- Create: `Semantics.Benchmarks/Paths/PathOperationBenchmarks.cs`
- Test: no test file. Verification is the `--job short` run in Step 4.

**Interfaces:**
- Consumes: the package references added in Task 2 Step 1.
- Produces: `internal static class PathSpecimens` with `internal static readonly string` members `AbsoluteFile` and `AbsoluteDirectory`, plus `internal const string` members `RelativeFile` and `FileNameOnly`. Task 6 uses all of them. Benchmark keys `PathCreationBenchmarks.AbsoluteFilePath`, `.RelativeFilePath`, `.FileNameType`, `.AbsoluteDirectoryPath`, and `PathOperationBenchmarks.FileName`, `.FileNameWithoutExtension`, `.DirectoryPath`, `.AsAbsolute`, `.AsRelative`, `.RemoveExtension`, which Task 7 draws.

- [ ] **Step 1: Write the specimens file**

The portability hazard is the substance of this step. `IsAbsolutePathAttribute` validates through `Path.IsPathFullyQualified`, which is operating-system dependent: `C:\...` is fully qualified on Windows and is a relative path on Linux. A hardcoded Windows path would make every absolute-path benchmark throw in CI while passing locally. So the root is chosen per platform.

Create `Semantics.Benchmarks/Paths/PathSpecimens.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using System;
using System.IO;

/// <summary>
/// The path strings the path benchmarks run against.
/// </summary>
/// <remarks>
/// <para>
/// <b>The absolute ones are built per platform, and they have to be.</b>
/// <c>IsAbsolutePathAttribute</c> validates through <c>Path.IsPathFullyQualified</c>, whose answer
/// depends on the operating system: <c>C:\x</c> is fully qualified on Windows and is an ordinary
/// relative path on Linux. A hardcoded Windows root would throw from every absolute benchmark on
/// the CI runner while passing on a developer's machine, which is the worst way for this to fail.
/// </para>
/// <para>
/// The two roots differ by two characters in length, so a measurement taken on Windows is not
/// exactly a measurement taken on Linux. That is smaller than the difference between CI hosts that
/// <c>BaselineBenchmarks</c> already exists to normalize, and it is why the history records a
/// baseline reading alongside every entry.
/// </para>
/// <para>
/// Nothing here touches the filesystem, and none of these paths needs to exist. The benchmarks
/// deliberately avoid <c>IsDirectory</c> and <c>IsFile</c>, which call <c>Directory.Exists</c> and
/// <c>File.Exists</c> and would measure the disk rather than the library.
/// </para>
/// </remarks>
internal static class PathSpecimens
{
	private static readonly string Root =
		OperatingSystem.IsWindows() ? @"C:\projects" : "/projects";

	/// <summary>A fully qualified file path, four segments below the root.</summary>
	internal static readonly string AbsoluteFile =
		Path.Combine(Root, "semantics", "src", "Semantics.Paths", "FilePath.cs");

	/// <summary>The directory that file sits in, used as the base for both conversions.</summary>
	internal static readonly string AbsoluteDirectory =
		Path.Combine(Root, "semantics", "src");

	/// <summary>A relative file path. Forward slashes are accepted on both platforms.</summary>
	/// <remarks>
	/// <c>const</c> rather than <c>static readonly</c>: it is a compile-time literal, and CA1802
	/// reports the latter as an error under this repository's warnings-as-errors setting. The two
	/// absolute specimens above genuinely must be <c>static readonly</c>, because they call
	/// <see cref="Path.Combine"/> against a root chosen per platform.
	/// </remarks>
	internal const string RelativeFile = "Semantics.Paths/FilePath.cs";

	/// <summary>A bare file name, with no separator in it.</summary>
	internal const string FileNameOnly = "FilePath.cs";
}
```

- [ ] **Step 2: Write the creation benchmarks**

Create `Semantics.Benchmarks/Paths/PathCreationBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures building each of the path types from a well-formed string.
/// </summary>
/// <remarks>
/// A path type is a semantic string whose validator asks the runtime a question about the shape of
/// the value, so each row is the same reflection machinery
/// <c>StringCreationBenchmarks.Unvalidated</c> measures plus one such question. The absolute rows
/// ask <c>Path.IsPathFullyQualified</c> and the relative row asks its negation, so the pair is the
/// same work in both directions rather than two different validators.
/// </remarks>
[MemoryDiagnoser]
public class PathCreationBenchmarks
{
	private string absoluteFile = "";
	private string absoluteDirectory = "";
	private string relativeFile = "";
	private string fileName = "";

	/// <summary>Copies the inputs into fields.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFile = PathSpecimens.AbsoluteFile;
		absoluteDirectory = PathSpecimens.AbsoluteDirectory;
		relativeFile = PathSpecimens.RelativeFile;
		fileName = PathSpecimens.FileNameOnly;
	}

	/// <summary>Builds a fully qualified file path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public AbsoluteFilePath AbsoluteFilePath() =>
		Semantics.Paths.AbsoluteFilePath.Create(absoluteFile);

	/// <summary>Builds a relative file path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public RelativeFilePath RelativeFilePath() =>
		Semantics.Paths.RelativeFilePath.Create(relativeFile);

	/// <summary>Builds a fully qualified directory path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public AbsoluteDirectoryPath AbsoluteDirectoryPath() =>
		Semantics.Paths.AbsoluteDirectoryPath.Create(absoluteDirectory);

	/// <summary>Builds a bare file name, whose validator checks for separators.</summary>
	/// <returns>The created file name.</returns>
	[Benchmark]
	public FileName FileNameType() => FileName.Create(fileName);
}
```

If the compiler objects to a benchmark method sharing a name with its return type, qualify the call as written above. The fully qualified form `Semantics.Paths.AbsoluteFilePath.Create` is there for exactly that reason and should be kept even if a shorter form happens to compile.

- [ ] **Step 3: Write the operation benchmarks**

Create `Semantics.Benchmarks/Paths/PathOperationBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures the operations a path type offers over the string it holds.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two rows are here to be read against each other.</b>
/// <see cref="FileNameWithoutExtension"/> caches into a field on the path, so the second read and
/// every read after it is a field read. <see cref="FileName"/> does not: it builds a fresh
/// <c>FileName</c>, validation included, on every single read. Both are properties, both read like
/// field access at a call site, and they differ by the cost of a semantic string creation. Putting
/// them side by side is what makes that visible, and it is the kind of thing a release could
/// quietly change in either direction.
/// </para>
/// <para>
/// <b>Both conversions are measured in the direction that does work.</b>
/// <c>AbsoluteFilePath.AsAbsolute()</c> returns <c>this</c> and would measure nothing, so the
/// absolute direction is taken from a relative path and the relative direction from an absolute
/// one.
/// </para>
/// <para>
/// Nothing here touches the filesystem. <c>IsDirectory</c> and <c>IsFile</c> are excluded on
/// purpose: they call <c>Directory.Exists</c> and <c>File.Exists</c>, so they would measure the
/// disk and the state of the machine rather than this library.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class PathOperationBenchmarks
{
	private AbsoluteFilePath absoluteFile = null!;
	private RelativeFilePath relativeFile = null!;
	private AbsoluteDirectoryPath baseDirectory = null!;

	/// <summary>Builds the operands once, outside the measurement.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFile = AbsoluteFilePath.Create(PathSpecimens.AbsoluteFile);
		relativeFile = RelativeFilePath.Create(PathSpecimens.RelativeFile);
		baseDirectory = AbsoluteDirectoryPath.Create(PathSpecimens.AbsoluteDirectory);
	}

	/// <summary>Reads the file name, which builds and validates a new one every time.</summary>
	/// <returns>The file name.</returns>
	[Benchmark]
	public FileName FileName() => absoluteFile.FileName;

	/// <summary>Reads the stem, which is cached into a field after the first read.</summary>
	/// <returns>The file name without its extension.</returns>
	[Benchmark]
	public FileName FileNameWithoutExtension() => absoluteFile.FileNameWithoutExtension;

	/// <summary>Reads the containing directory, which builds and validates a new path.</summary>
	/// <returns>The directory path.</returns>
	[Benchmark]
	public DirectoryPath DirectoryPath() => absoluteFile.DirectoryPath;

	/// <summary>Resolves a relative path against a base directory.</summary>
	/// <returns>The resolved absolute path.</returns>
	[Benchmark]
	public AbsoluteFilePath AsAbsolute() => relativeFile.AsAbsolute(baseDirectory);

	/// <summary>Expresses an absolute path relative to a base directory.</summary>
	/// <returns>The relative path.</returns>
	[Benchmark]
	public RelativeFilePath AsRelative() => absoluteFile.AsRelative(baseDirectory);

	/// <summary>Strips the extension, which rebuilds and revalidates the whole path.</summary>
	/// <returns>The path without its extension.</returns>
	[Benchmark]
	public AbsoluteFilePath RemoveExtension() => absoluteFile.RemoveExtension();
}
```

Three methods here share a name with their own return type (`FileName`, `DirectoryPath`). C# resolves
that, because method names do not participate in type-name lookup, but if the compiler does object,
qualify the return type as `Semantics.Paths.FileName` rather than renaming the method: Task 7 draws
these by name and a rename there would silently produce empty panels.

- [ ] **Step 4: Run both classes and read the summaries**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*PathCreationBenchmarks*' '*PathOperationBenchmarks*' --job short
```

Expected: four rows and six rows, all with measurements.

**Check for and report:**
- Any `NA` row means the benchmark threw. On a path benchmark the near-certain cause is a specimen that is not valid for its type on this operating system. Fix the specimen in `PathSpecimens`, and re-read the remarks in that file before doing so.
- `FileNameWithoutExtension` should be dramatically cheaper than `FileName`, because the first is a field read after the first call and the second is a full creation. **If they are close, the caching is not doing what the class documentation claims, which is a finding to report rather than a benchmark to adjust.**
- `FileNameWithoutExtension` is the row most likely to report `ZeroMeasurement`, being a field read. If it does, report it: Task 7 draws it, and the panel choice needs revisiting.

- [ ] **Step 5: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add Semantics.Benchmarks/Paths/
git commit -m "Measure the path types, building and operating

Absolute specimens are built per platform because IsAbsolutePath asks
Path.IsPathFullyQualified, whose answer differs between Windows and
Linux; a hardcoded Windows root would throw on every CI run and pass
locally.

The cached and uncached file name properties are measured side by side,
because both read like field access and one is a full creation."
```

---

### Task 6: Path cost pairs

**Files:**
- Create: `Semantics.Benchmarks/Paths/PathAbstractionCostBenchmarks.cs`
- Test: no test file. Verification is the `--job short` run in Step 2.

**Interfaces:**
- Consumes: `PathSpecimens.AbsoluteFile`, `.AbsoluteDirectory`, `.RelativeFile` from Task 5.
- Produces: a `Ratio` column per category, which Task 11 copies into `Semantics.Benchmarks/README.md`. Nothing later depends on the benchmark keys.

- [ ] **Step 1: Write the class**

Create `Semantics.Benchmarks/Paths/PathAbstractionCostBenchmarks.cs`:

```csharp
// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using System.IO;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures what the path types cost over <see cref="Path"/> doing the same work.
/// </summary>
/// <remarks>
/// <para>
/// This pairing needs none of the care the string one does. <see cref="Path"/> is a real API doing
/// the real work, so each category is the same operation twice and the ratio is a straight answer.
/// </para>
/// <para>
/// What the semantic side adds is a validated wrapper around the result: every one of these
/// operations returns a path type rather than a string, which means a creation, which means the
/// reflection machinery and the validator. The ratio is therefore expected well above 1.00
/// throughout, and the number is the point rather than a disappointment — it is what a caller pays
/// for a result that cannot be silently passed where a different kind of path belongs.
/// </para>
/// <para>
/// The loops exist for the reason they do everywhere in this suite: a single call over an
/// unchanging operand is loop-invariant, the JIT hoists it, and a ratio between two hoisted
/// methods means nothing.
/// </para>
/// </remarks>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PathAbstractionCostBenchmarks
{
	/// <summary>Operations per invocation.</summary>
	private const int Operations = 64;

	private string absoluteFileText = "";
	private string relativeFileText = "";
	private string baseDirectoryText = "";
	private AbsoluteFilePath absoluteFile = null!;
	private RelativeFilePath relativeFile = null!;
	private AbsoluteDirectoryPath baseDirectory = null!;

	/// <summary>Prepares both sides of every pair, holding the same values.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFileText = PathSpecimens.AbsoluteFile;
		relativeFileText = PathSpecimens.RelativeFile;
		baseDirectoryText = PathSpecimens.AbsoluteDirectory;

		absoluteFile = AbsoluteFilePath.Create(absoluteFileText);
		relativeFile = RelativeFilePath.Create(relativeFileText);
		baseDirectory = AbsoluteDirectoryPath.Create(baseDirectoryText);
	}

	/// <summary>Extracting a file name with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("FileName")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareFileName()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetFileName(absoluteFileText).Length;
		}

		return accumulator;
	}

	/// <summary>Extracting it through the path type, which validates the result.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("FileName")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticFileName()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += absoluteFile.FileName.Length;
		}

		return accumulator;
	}

	/// <summary>Resolving a relative path with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsAbsolute")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareAsAbsolute()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetFullPath(relativeFileText, baseDirectoryText).Length;
		}

		return accumulator;
	}

	/// <summary>Resolving it through the path type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsAbsolute")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticAsAbsolute()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += relativeFile.AsAbsolute(baseDirectory).Length;
		}

		return accumulator;
	}

	/// <summary>Relativizing with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsRelative")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareAsRelative()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetRelativePath(baseDirectoryText, absoluteFileText).Length;
		}

		return accumulator;
	}

	/// <summary>Relativizing through the path type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsRelative")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticAsRelative()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += absoluteFile.AsRelative(baseDirectory).Length;
		}

		return accumulator;
	}

	/// <summary>Checking a path is rooted by hand, which is what creation validates.</summary>
	/// <returns>The count of rooted paths, which is every iteration.</returns>
	[BenchmarkCategory("Create")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareCreate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (Path.IsPathFullyQualified(absoluteFileText))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Building the path type, which asks the same question and keeps the answer.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("Create")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticCreate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += AbsoluteFilePath.Create(absoluteFileText).Length;
		}

		return accumulator;
	}
}
```

- [ ] **Step 2: Run the class and read the ratios**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*PathAbstractionCostBenchmarks*' --job short
```

Expected: eight rows in four categories, each with a `Ratio` column.

**Record the four ratios and the eight allocation figures for Task 11.**

Sanity expectation: every ratio above 1.00, because the semantic side does the same work and then wraps the result in a validated type. A ratio at or below 1.00 means the semantic side is being optimized away and should be reported, not accepted.

- [ ] **Step 3: Build clean and commit**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet build -c Release Semantics.Benchmarks
git add Semantics.Benchmarks/Paths/PathAbstractionCostBenchmarks.cs
git commit -m "Pair the path types against System.IO.Path

A straight comparison, unlike the string one: Path is a real API doing
the real work. What the semantic side adds is a validated wrapper around
every result, so the ratio is what a caller pays for a path that cannot
be passed where a different kind belongs."
```

---

### Task 7: Register the two new subjects with the renderer

**Files:**
- Modify: `scripts/benchmark-history.cs` (the `Subjects` dictionary from Task 1)
- Test: no test file. Verification is a render against a stub history in Step 2, then byte equality of the quantities chart in Step 3.

**Interfaces:**
- Consumes: the `Subject` and `Panel` records and the `Subjects` dictionary from Task 1; the benchmark keys produced by Tasks 2, 3 and 5.
- Produces: `--subject strings` and `--subject paths` as valid arguments, which Task 8 calls from the workflow.

- [ ] **Step 1: Add the two entries**

**The strings contingency has already fired and is baked into the list below.** Task 3 measured
`StringOperationBenchmarks.CompareTo` at 0.5714 ns and BenchmarkDotNet reported `ZeroMeasurement` —
the JIT hoists it, so a panel of it would chart the harness's resolution rather than the library.
Position 8 is therefore `HashCode` (43.8881 ns, measured cleanly, no warning) rather than `CompareTo`.
`ToStringImplicit` was also hoisted, and is not drawn either way.

Still to check before writing the paths list: the note recorded in Task 5 Step 4. If
`FileNameWithoutExtension` reported `ZeroMeasurement`, replace the paths position 5 panel with
`new("PathCreationBenchmarks.AbsoluteDirectoryPath", null, "Create (absolute dir)")`.

Add to the `Subjects` dictionary, after the `quantities` entry:

```csharp
		["strings"] = new("Semantics.Strings", 4,
		[
			new("StringCreationBenchmarks.Unvalidated", null, "Create (no validation)"),
			new("StringCreationBenchmarks.CharsetRegex", null, "Create (charset regex)"),
			new("StringCreationBenchmarks.FormatRegex", null, "Create (format regex)"),
			new("StringCreationBenchmarks.Mod97", null, "Create (mod-97)"),
			new("StringCreationBenchmarks.TryCreateRejects", null, "TryCreate (rejects)"),
			new("StringCreationBenchmarks.CreateThrows", null, "Create (throws)"),
			new("StringOperationBenchmarks.AsConversion", null, "As<T> conversion"),
			new("StringOperationBenchmarks.HashCode", null, "GetHashCode"),
		]),
		["paths"] = new("Semantics.Paths", 4,
		[
			new("PathCreationBenchmarks.AbsoluteFilePath", null, "Create (absolute file)"),
			new("PathCreationBenchmarks.RelativeFilePath", null, "Create (relative file)"),
			new("PathCreationBenchmarks.FileNameType", null, "Create (file name)"),
			new("PathOperationBenchmarks.FileName", null, "FileName (uncached)"),
			new("PathOperationBenchmarks.FileNameWithoutExtension", null, "FileName (cached)"),
			new("PathOperationBenchmarks.AsAbsolute", null, "AsAbsolute (from relative)"),
			new("PathOperationBenchmarks.AsRelative", null, "AsRelative (from absolute)"),
			new("PathOperationBenchmarks.RemoveExtension", null, "RemoveExtension"),
		]),
```

Extend the `Subjects` XML documentation with two paragraphs, placed after the existing quantities paragraphs:

```csharp
	/// <para>
	/// <b>Strings</b> is drawn along validation weight, because that is the axis there is. A
	/// semantic string is a record wrapping a <see cref="string"/> and its cost is concentrated at
	/// creation, so the top row walks from the reflection machinery alone up through a character
	/// set check, a format check and a mod-97 check. The bottom row is what a caller pays around
	/// that: both failure paths, the cross-type conversion that is secretly another creation, and
	/// the hash a dictionary of semantic strings pays on every lookup.
	/// </para>
	/// <para>
	/// Ordering is measured and stored, and deliberately not drawn. A semantic string's
	/// <c>CompareTo</c> is the underlying string's own, over operands a loop does not change, so
	/// the JIT hoists it and BenchmarkDotNet reports it as indistinguishable from an empty method —
	/// the same reason no bare quantity operator is drawn above. A panel of it would chart the
	/// harness's resolution rather than any release.
	/// </para>
	/// <para>
	/// <b>Paths</b> is the same shape: build each kind, then operate on one. The two file name
	/// panels sit next to each other because one caches into a field and one rebuilds and
	/// revalidates on every read, and both look like field access at a call site.
	/// </para>
```

- [ ] **Step 2: Verify both subjects render from a stub history**

The real histories do not exist until Task 9, so render against a stub to prove the panel keys resolve and the layout is sound.

```bash
cd /c/dev/ktsu-dev/Semantics
cat > "$TMPDIR/stub-history.json" <<'JSON'
{
  "schemaVersion": 1,
  "entries": [
    {
      "version": "5.3.4",
      "commit": "0000000",
      "date": "2026-09-18",
      "cpu": "stub",
      "runtime": "stub",
      "baselineNs": 100.0,
      "runId": "stub",
      "benchmarks": {
        "StringCreationBenchmarks.Unvalidated": { "": { "meanNs": 100.0, "allocatedBytes": 64 } }
      }
    }
  ]
}
JSON
dotnet run scripts/benchmark-history.cs -- render --subject strings \
  --history "$TMPDIR/stub-history.json" --out "$TMPDIR/strings.svg"
dotnet run scripts/benchmark-history.cs -- render --subject paths \
  --history "$TMPDIR/stub-history.json" --out "$TMPDIR/paths.svg"
grep -c "panel-title" "$TMPDIR/strings.svg" "$TMPDIR/paths.svg"
grep -o 'class="title">[^<]*' "$TMPDIR/strings.svg"
```

Expected: both renders succeed. Each file contains 16 panel titles, which is eight panels drawn twice, once for allocation and once for time. The title line reads `Semantics.Strings performance by release`. Panels whose key is absent from the stub draw empty, which is the correct behavior for a missing measurement and is what a skipped version looks like.

- [ ] **Step 3: Re-verify the quantities chart is still byte-identical**

Adding dictionary entries must not disturb the existing one, but this is cheap and it is the invariant the whole task set rests on.

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render --subject quantities \
  --history docs/benchmarks/history.json --out docs/benchmarks/performance.svg
git diff --stat docs/benchmarks/
```

Expected: no changes.

- [ ] **Step 4: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add scripts/benchmark-history.cs
git commit -m "Draw the strings and paths charts

Strings along validation weight, which is the axis there is when the
cost is concentrated at creation. Paths the same shape, with the cached
and uncached file name panels adjacent because both read like field
access and only one is."
```

---

### Task 8: Make the workflow run three subjects

**Files:**
- Modify: `.github/workflows/benchmark-history.yml`
- Test: no test file. Verification is a `yamllint`-free parse check in Step 5 and a local dry run of the loop logic in Step 4. The workflow itself is proven by the first real dispatch, which is out of scope here.

**Interfaces:**
- Consumes: `render --subject <name>` from Tasks 1 and 7; the benchmark classes from Tasks 2 to 6.
- Produces: nothing later tasks consume. Task 9 and Task 10 run the benchmarks locally rather than through this workflow.

- [ ] **Step 1: Replace the single-subject environment block**

Replace the `HISTORY`, `CHART` and `HEADLINE_FILTER` entries in the workflow's `env:` block with a single subject table. Keep `DOTNET_VERSION`, `RUNS` and `BENCHMARK_JOB` exactly as they are.

```yaml
env:
  DOTNET_VERSION: "10.0"
  # Already ignored, and ktsu.Sdk regenerates .gitignore on build so a new entry would not last.
  RUNS: BenchmarkDotNet.Artifacts
  # Short runs: three iterations is enough for a trend line, and a release should not tie up a
  # runner for half an hour.
  BENCHMARK_JOB: short
  # One line per chart: name|history|chart|filter. An environment variable cannot hold an array,
  # and three steps need the same three triples, so this is the one place they are written.
  #
  # Each filter is one operation per panel drawn. What varies between subjects is the axis: a
  # quantity is a value type over T and does almost nothing of its own, so its release changes land
  # per storage type; a semantic string spends its cost at creation, so its axis is how much
  # validation the type declares.
  SUBJECTS: |
    quantities|docs/benchmarks/history.json|docs/benchmarks/performance.svg|*ConstructionBenchmarks*FromNauticalMile *UnitConversionBenchmarks*InNauticalMile *OperatorBenchmarks*LengthTimesLength *VectorBenchmarks*.Length *ComparisonBenchmarks*CompareToInterface
    strings|docs/benchmarks/strings-history.json|docs/benchmarks/strings-performance.svg|*StringCreationBenchmarks* *StringOperationBenchmarks*
    paths|docs/benchmarks/paths-history.json|docs/benchmarks/paths-performance.svg|*PathCreationBenchmarks* *PathOperationBenchmarks*
```

The strings and paths filters name whole classes rather than individual methods, because those classes are small and every method in them is wanted in the history. The quantities filter stays method-by-method, unchanged, because its classes are generic over four storage types and naming a whole class would multiply the run by four for panels nobody draws.

- [ ] **Step 2: Add the `subjects` dispatch input and raise the timeout**

In `workflow_dispatch.inputs`, after the existing `versions` input:

```yaml
      subjects:
        description: "Space-separated subjects to measure: quantities strings paths"
        required: false
        default: "quantities strings paths"
        type: string
```

Change `timeout-minutes: 240` to:

```yaml
    # Three subjects rather than one. The dispatch's `subjects` input is the intended way to split
    # a long backfill across runs; raising this number further is not.
    timeout-minutes: 360
```

- [ ] **Step 3: Loop the subject table in the three measuring and rendering steps**

In the release step, replace the single `dotnet run` and `ingest` pair with a loop inside the existing worktree. The worktree is created once and removed once, exactly as now:

```bash
          set -euo pipefail
          version="${TAG#v}"
          work="${RUNNER_TEMP}/bench-$version"
          git worktree add --detach "$work" "$TAG"

          while IFS='|' read -r subject history chart filter; do
            [ -n "$subject" ] || continue
            echo "::group::$subject $version"
            (cd "$work" && dotnet run -c Release --project Semantics.Benchmarks -- \
              --filter $filter \
              --job "$BENCHMARK_JOB" \
              --artifacts "$GITHUB_WORKSPACE/$RUNS/$subject/$version")

            dotnet run scripts/benchmark-history.cs -- ingest \
              --history "$history" \
              --results "$RUNS/$subject/$version" \
              --version "$version" \
              --commit "$(git rev-parse --short "$TAG^{commit}")" \
              --date "$(git log -1 --format=%cs "$TAG")" \
              --run-id "${{ github.run_id }}" \
              --baseline-ns "${{ steps.baseline.outputs.ns }}"
            echo "::endgroup::"
          done <<< "$SUBJECTS"

          git worktree remove --force "$work"
```

In the backfill step, nest the subject loop inside the existing version loop, so one `BenchmarkAgainstVersion` build serves all three subjects at that version. Keep both existing skip paths, now scoped per subject per version rather than per version, which is what lets the strings chart reach back further than the quantities one:

```bash
          set -euo pipefail
          read -ra versions <<< "$VERSIONS"
          read -ra wanted <<< "$SUBJECTS_WANTED"

          for version in "${versions[@]}"; do
            while IFS='|' read -r subject history chart filter; do
              [ -n "$subject" ] || continue
              # Skip a subject the dispatch did not ask for.
              case " ${wanted[*]} " in *" $subject "*) ;; *) continue ;; esac

              echo "::group::$subject $version"
              # Through the environment rather than a -p: switch, because BenchmarkDotNet generates
              # and builds a project of its own per run, which a property passed on the command line
              # does not reach. MSBuild reads environment variables as properties in every project.
              if ! BenchmarkAgainstVersion="$version" dotnet run -c Release --project Semantics.Benchmarks -- \
                   --filter $filter \
                   --job "$BENCHMARK_JOB" \
                   --artifacts "$GITHUB_WORKSPACE/$RUNS/$subject/$version"; then
                echo "::warning::$subject $version could not be benchmarked by the current suite; skipping"
                echo "::endgroup::"
                continue
              fi

              tag="v$version"
              commit=""
              date=""
              if git rev-parse -q --verify "$tag^{commit}" >/dev/null; then
                commit="$(git rev-parse --short "$tag^{commit}")"
                date="$(git log -1 --format=%cs "$tag")"
              fi

              # Skipped here too: a package can build against these benchmarks and still throw from
              # every one of them at run time, which BenchmarkDotNet reports as a table of NA rather
              # than as a failure. Ingest refuses such a run, and the backfill carries on.
              if ! dotnet run scripts/benchmark-history.cs -- ingest \
                   --history "$history" \
                   --results "$RUNS/$subject/$version" \
                   --version "$version" \
                   --commit "$commit" \
                   --date "$date" \
                   --run-id "${{ github.run_id }}" \
                   --baseline-ns "$BASELINE_NS"; then
                echo "::warning::$subject $version produced no usable measurement; skipping"
              fi
              echo "::endgroup::"
            done <<< "$SUBJECTS"
          done
```

Add `SUBJECTS_WANTED` to that step's `env:` block alongside the existing `VERSIONS` and `BASELINE_NS`:

```yaml
          SUBJECTS_WANTED: ${{ inputs.subjects }}
```

Replace the render step's single command with a loop:

```yaml
      - name: Redraw the charts
        shell: bash
        run: |
          set -euo pipefail
          while IFS='|' read -r subject history chart filter; do
            [ -n "$subject" ] || continue
            if [ ! -f "$history" ]; then
              echo "No history for $subject yet; nothing to draw."
              continue
            fi
            dotnet run scripts/benchmark-history.cs -- render \
              --subject "$subject" --history "$history" --out "$chart"
          done <<< "$SUBJECTS"
```

- [ ] **Step 4: Widen the commit step's staging**

The current step stages one history and one chart stem. It stages all of them:

```bash
          git add docs/benchmarks/
```

The whole directory rather than an enumerated list, because the enumeration would be a fourth place the subject table is written down and would silently drop a chart if it fell out of step. Nothing else lives in that directory.

- [ ] **Step 5: Verify the loop logic locally**

The workflow cannot be run locally, but the parsing can, which is where the real risk is. This catches a mistyped separator or a filter that splits wrongly.

```bash
cd /c/dev/ktsu-dev/Semantics
SUBJECTS=$(sed -n '/^  SUBJECTS: |$/,/^[a-zA-Z]/p' .github/workflows/benchmark-history.yml \
  | sed '1d;$d' | sed 's/^    //')
while IFS='|' read -r subject history chart filter; do
  [ -n "$subject" ] || continue
  echo "subject=[$subject] history=[$history] chart=[$chart]"
  echo "  filter=[$filter]"
done <<< "$SUBJECTS"
```

Expected: three lines, each with a non-empty subject, a history path under `docs/benchmarks/`, a chart path under `docs/benchmarks/`, and a filter containing at least one `*`. If any field is empty or a filter has leaked into the chart field, a separator is wrong.

- [ ] **Step 6: Verify the workflow file parses as YAML**

```bash
cd /c/dev/ktsu-dev/Semantics
python3 -c "import yaml,sys; yaml.safe_load(open('.github/workflows/benchmark-history.yml')); print('ok')"
```

Expected: `ok`. If `python3` is unavailable, use `gh workflow view benchmark-history.yml` or push the branch and let GitHub's own parser report, but do not skip the check.

- [ ] **Step 7: Update the workflow's header comment**

The comment block at the top of the file describes a single-subject pipeline. Extend it rather than rewriting it, keeping every existing paragraph, and add:

```yaml
# Three subjects share one job: quantities, strings, and paths. One job rather than a matrix so
# that the reference workload is measured once and stamped on every entry the run produces -- which
# is what lets a strings point and a quantities point from the same run be compared at all. It also
# keeps the results to a single push.
#
# The cost is wall clock, and `subjects` on the dispatch is the answer to that: a long backfill is
# split by subject across runs rather than by raising the timeout.
```

- [ ] **Step 8: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add .github/workflows/benchmark-history.yml
git commit -m "Measure three subjects per run

One job rather than a matrix, so the reference workload is read once and
stamped on every entry: that is what makes a strings point and a
quantities point from the same run comparable, and it keeps the results
to one push.

The dispatch gains a subjects input, which is how a long backfill gets
split rather than by raising the timeout."
```

---

### Task 9: Seed one version and check the numbers

The gate before the long run. Its purpose is to find out whether the measurements mean anything *before* spending hours producing more of them.

**Files:**
- Create: `docs/benchmarks/strings-history.json`, `docs/benchmarks/paths-history.json`
- Create: `docs/benchmarks/strings-performance.svg`, `docs/benchmarks/strings-performance-dark.svg`, `docs/benchmarks/paths-performance.svg`, `docs/benchmarks/paths-performance-dark.svg`
- Test: no test file. The check is the expectations table in Step 4.

**Interfaces:**
- Consumes: everything from Tasks 1 to 7.
- Produces: the two history files Task 10 appends to.

- [ ] **Step 1: Measure the reference workload**

Every entry needs a baseline reading from the same machine, exactly as the workflow takes one.

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*BaselineBenchmarks.ReferenceWork' --job short \
  --artifacts "$PWD/BenchmarkDotNet.Artifacts/baseline"
dotnet run scripts/benchmark-history.cs -- baseline --results BenchmarkDotNet.Artifacts/baseline
```

Expected: a single number in nanoseconds. **Record it. Every ingest in this task and Task 10 passes the same value**, because they all run on this one machine.

- [ ] **Step 2: Measure both subjects at the working copy**

The working copy is 5.3.4 plus this branch's changes, and none of those changes touch the libraries being measured, so it is a fair stand-in for the released 5.3.4.

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*StringCreationBenchmarks*' '*StringOperationBenchmarks*' --job short \
  --artifacts "$PWD/BenchmarkDotNet.Artifacts/strings/5.3.4"
dotnet run -c Release --project Semantics.Benchmarks -- \
  --filter '*PathCreationBenchmarks*' '*PathOperationBenchmarks*' --job short \
  --artifacts "$PWD/BenchmarkDotNet.Artifacts/paths/5.3.4"
```

- [ ] **Step 3: Ingest both**

Substitute the baseline number from Step 1 for `<NS>`.

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- ingest \
  --history docs/benchmarks/strings-history.json \
  --results BenchmarkDotNet.Artifacts/strings/5.3.4 \
  --version 5.3.4 --commit "$(git rev-parse --short v5.3.4^{commit})" \
  --date "$(git log -1 --format=%cs v5.3.4)" --run-id local-seed --baseline-ns <NS>

dotnet run scripts/benchmark-history.cs -- ingest \
  --history docs/benchmarks/paths-history.json \
  --results BenchmarkDotNet.Artifacts/paths/5.3.4 \
  --version 5.3.4 --commit "$(git rev-parse --short v5.3.4^{commit})" \
  --date "$(git log -1 --format=%cs v5.3.4)" --run-id local-seed --baseline-ns <NS>
```

Expected: each prints `ingested 5.3.4: N benchmarks, baseline <NS> ns, cpu ...`.

- [ ] **Step 4: Check the numbers against expectations stated in advance**

**This is the gate. Do not proceed to Task 10 until every line holds, and report any that does not rather than adjusting the expectation to fit the number.**

| Expectation | Why it must hold |
|---|---|
| `StringCreationBenchmarks.Unvalidated` is the fastest creation row | It is the reflection machinery with no validator. Anything faster means another row is not running its validator. |
| The four validator rows all land within ~340 ns of each other, on a floor of ~1,650 ns | Measured in Task 9. The validator is a minor term and the reflection machinery is the bill. `Mod97` and `FormatRegex` are a tie within noise, so do not expect a stable ordering between them. **The failure mode to watch for is a validator row at or near the unvalidated floor**, which would mean its validator is not running. |
| `CreateThrows` and `TryCreateRejects` cost about the same, and both cost far more than any success row | Established by measurement in Task 2, and it is a property of the library rather than of the benchmark: `SemanticString.TryFromString` is implemented as `try { Create(...) } catch (ArgumentException) { return false; }`, so `TryCreate` throws and catches internally on every rejection. Both rows therefore pay a full .NET exception. **If instead either row is cheap and close to a success row, the specimen is being accepted and the rejection is not happening** — that is the failure mode to watch for. |
| Every string creation row allocates more than 0 bytes | A semantic string is a reference type. A zero here means the allocation is not being counted and `[MemoryDiagnoser]` is missing or the row did not run. |
| `PathOperationBenchmarks.FileNameWithoutExtension` is far cheaper than `.FileName` | One caches into a field, the other rebuilds and revalidates. |
| Every path row allocates more than 0 bytes except `FileNameWithoutExtension` | Same reasoning, and the cached one returns an existing reference. |

```bash
cd /c/dev/ktsu-dev/Semantics
python3 - <<'PY'
import json
for name in ("strings", "paths"):
    with open(f"docs/benchmarks/{name}-history.json") as handle:
        entry = json.load(handle)["entries"][-1]
    print(f"--- {name} {entry['version']} baseline {entry['baselineNs']} ns ---")
    for key, cases in sorted(entry["benchmarks"].items()):
        for _, measurement in cases.items():
            print(f"  {key:60s} {measurement['meanNs']:12.2f} ns  {measurement['allocatedBytes']:6d} B")
PY
```

- [ ] **Step 5: Render both charts and look at them**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render --subject strings \
  --history docs/benchmarks/strings-history.json --out docs/benchmarks/strings-performance.svg
dotnet run scripts/benchmark-history.cs -- render --subject paths \
  --history docs/benchmarks/paths-history.json --out docs/benchmarks/paths-performance.svg
ls -la docs/benchmarks/
```

Expected: six SVG files in the directory, the two new light charts and their two dark counterparts alongside the two quantities ones. A one-point chart draws a single marker per panel, which is correct and will fill in as Task 10 adds versions.

- [ ] **Step 6: Commit the seed**

```bash
cd /c/dev/ktsu-dev/Semantics
git add docs/benchmarks/
git status --short
git commit -m "Seed the strings and paths histories at 5.3.4

Measured on one machine with one reference reading, recorded as
local-seed the way the quantities history was. baselineNs is what lets
these sit alongside the CI points that follow."
```

---

### Task 10: Backfill the remaining versions

**Files:**
- Modify: `docs/benchmarks/strings-history.json`, `docs/benchmarks/paths-history.json`, and the four new SVG files
- Test: no test file. Verification is the entry count and the per-version skip report in Step 3.

**Interfaces:**
- Consumes: the histories from Task 9, and the baseline nanosecond figure recorded in Task 9 Step 1.
- Produces: filled histories and charts that Task 11 describes in the README.

- [ ] **Step 1: Run the backfill**

The same version list the quantities workflow defaults to, minus 5.3.4 which Task 9 already seeded. Substitute the Task 9 baseline figure for `<NS>`. This is the long step. Leave the machine otherwise idle.

```bash
cd /c/dev/ktsu-dev/Semantics
BASELINE_NS=<NS>
for version in 3.3.1 4.0.0 4.1.0 4.2.0 4.3.2 5.0.0 5.1.0 5.2.0 5.2.4 5.3.2; do
  for subject in strings paths; do
    case "$subject" in
      strings) filter="*StringCreationBenchmarks* *StringOperationBenchmarks*" ;;
      paths)   filter="*PathCreationBenchmarks* *PathOperationBenchmarks*" ;;
    esac
    echo "=== $subject $version ==="
    if ! BenchmarkAgainstVersion="$version" dotnet run -c Release --project Semantics.Benchmarks -- \
         --filter $filter --job short \
         --artifacts "$PWD/BenchmarkDotNet.Artifacts/$subject/$version"; then
      echo "SKIP: $subject $version could not be benchmarked by the current suite"
      continue
    fi
    if ! dotnet run scripts/benchmark-history.cs -- ingest \
         --history "docs/benchmarks/$subject-history.json" \
         --results "BenchmarkDotNet.Artifacts/$subject/$version" \
         --version "$version" \
         --commit "$(git rev-parse --short "v$version^{commit}" 2>/dev/null || echo '')" \
         --date "$(git log -1 --format=%cs "v$version" 2>/dev/null || echo '')" \
         --run-id local-seed --baseline-ns "$BASELINE_NS"; then
      echo "SKIP: $subject $version produced no usable measurement"
    fi
  done
done
```

- [ ] **Step 2: Render both charts**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet run scripts/benchmark-history.cs -- render --subject strings \
  --history docs/benchmarks/strings-history.json --out docs/benchmarks/strings-performance.svg
dotnet run scripts/benchmark-history.cs -- render --subject paths \
  --history docs/benchmarks/paths-history.json --out docs/benchmarks/paths-performance.svg
```

- [ ] **Step 3: Report what was measured and what was skipped**

```bash
cd /c/dev/ktsu-dev/Semantics
python3 - <<'PY'
import json
for name in ("strings", "paths"):
    with open(f"docs/benchmarks/{name}-history.json") as handle:
        entries = json.load(handle)["entries"]
    versions = [e["version"] for e in entries]
    print(f"{name}: {len(entries)} entries -> {', '.join(versions)}")
PY
```

**Report the skipped versions and their reasons.** A version skipped because the older package's API cannot express today's benchmarks is expected and fine, and is exactly why the workflow warns rather than fails. `Semantics.Strings` has been more stable than the quantities package, so the strings history may well reach further back than `docs/benchmarks/history.json` does, which is a result worth stating in Task 11 rather than hiding.

- [ ] **Step 4: Commit**

```bash
cd /c/dev/ktsu-dev/Semantics
git add docs/benchmarks/
git commit -m "Backfill the strings and paths histories

Measured as published packages by today's benchmarks, which is the
better comparison than checking out each tag: every version is timed by
identical code rather than by whatever each tag shipped."
```

---

### Task 11: Documentation

**Files:**
- Modify: `README.md` (the `## Performance` section, currently at lines 148-176)
- Modify: `Semantics.Benchmarks/README.md` (headings throughout, plus two new sections)
- Modify: `CLAUDE.md` (the project layout table, plus a new subsection)
- Test: no test file. Verification is the link and image check in Step 5.

**Interfaces:**
- Consumes: the ratio tables recorded in Task 4 Step 2 and Task 6 Step 2; the version coverage reported in Task 10 Step 3.
- Produces: nothing. This is the last task.

- [ ] **Step 1: Restructure the README performance section**

Replace the whole `## Performance` section. The two paragraphs about how to read the charts are stated once above the three subsections rather than three times.

```markdown
## Performance

Every release measures a fixed set of benchmarks and adds a point to a chart per library. The numbers
behind them are in [`docs/benchmarks/`](docs/benchmarks/), and the suite is
[`Semantics.Benchmarks`](Semantics.Benchmarks/README.md).

Read the two halves of every chart differently. **Allocation is exact** — the same code allocates the
same bytes on any machine, so a step in the top row is always a real change. **Time is measured on
shared CI runners**, where the host a job happens to land on varies more than most releases do, so
each time is divided by a reference workload measured in the same job. That cancels most of the
difference between machines; what is left is indicative rather than precise.

### Quantities

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/benchmarks/performance-dark.svg">
  <img alt="Allocated bytes per operation, and time relative to a fixed reference workload, for each Semantics.Quantities release" src="docs/benchmarks/performance.svg">
</picture>

The grid is one operation per storage type rather than every operation at one storage type. A quantity
is a `readonly record struct` over its `T` and does almost nothing of its own — a value is held in the
SI base unit, so an operator is the storage type's arithmetic and a struct initialiser — so the same
line of user code costs different things depending on the `T` it was written against, and a release
changes it per `T`.

### Strings

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/benchmarks/strings-performance-dark.svg">
  <img alt="Allocated bytes per operation, and time relative to a fixed reference workload, for each Semantics.Strings release" src="docs/benchmarks/strings-performance.svg">
</picture>

The axis here is validation weight, because that is where a semantic string spends. `Create` goes
through `Activator.CreateInstance`, a `PropertyInfo.SetValue`, and a reflective walk of the type's
validation attributes on every call, so the top row walks from that machinery alone up through a
character set check, a format check, and a mod-97 check. The bottom row is what surrounds it: both
failure paths, the cross-type conversion that is a full creation in disguise, and an ordering that is
the underlying string's own.

This is a different answer from the quantities one, and worth stating plainly rather than leaving to
be inferred from a chart: a quantity's wrapper is free, and a semantic string's is not. What it buys
is that an invalid value cannot exist, checked once at the boundary instead of everywhere the value
is used.

### Paths

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/benchmarks/paths-performance-dark.svg">
  <img alt="Allocated bytes per operation, and time relative to a fixed reference workload, for each Semantics.Paths release" src="docs/benchmarks/paths-performance.svg">
</picture>

Building each kind of path, then operating on one. The two file name panels sit next to each other
deliberately: `FileNameWithoutExtension` caches into a field, `FileName` rebuilds and revalidates on
every read, and both look like field access at a call site.
```

- [ ] **Step 2: Restructure the benchmark suite README**

Three edits, in order.

First, insert a `## Quantities` heading immediately before the existing `## The axis that matters here is the storage type` section, and demote that section and everything down to the end of `### An operator on a double is below the floor` by one heading level, so `##` becomes `###` and `###` becomes `####`. The `## Running`, `## Measuring a published release` and `## Reading the results` sections stay at `##` and stay where they are, because they are shared.

Second, after the quantities material, add `## Strings` and `## Paths` sections. Write them from the class documentation in `StringCreationBenchmarks`, `StringAbstractionCostBenchmarks`, `PathOperationBenchmarks` and `PathAbstractionCostBenchmarks`, and include the two ratio tables using **the real figures recorded in Task 4 Step 2 and Task 6 Step 2**. The tables take this shape:

```markdown
| pair | ratio | allocation, bare | allocation, semantic |
|---|---|---|---|
| Validate at a boundary | N.NN | 0 B | NNN B |
| Reject without throwing | N.NN | 0 B | NNN B |
| Equality | N.NN | 0 B | 0 B |
| Ordering | N.NN | 0 B | 0 B |
```

Each table is followed by a sentence saying what it means, written after reading the numbers rather than predicted here.

Third, extend the two shared sections. In `## Running`, add a paragraph reading "The suite covers
three libraries, and `--filter` is how one is picked:" followed by a fenced `bash` block containing
these two lines:

````text
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*String*Benchmarks*'
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*Path*Benchmarks*'
````

In `## Measuring a published release`, note that `BenchmarkAgainstVersion` swaps all four packages — `Quantities`, `Strings`, `Strings.Identifiers` and `Paths` — at the one version, which is correct because this repository ships one version across every package.

- [ ] **Step 3: Add the benchmark project to the CLAUDE.md layout table**

In the project layout table, after the `Semantics.Quantities.{Double,Float,Decimal,Precise}` row, add:

```markdown
| `Semantics.Benchmarks` | BenchmarkDotNet suite covering quantities, strings and paths. Not shipped and not covered by tests, so it carries `SonarQubeExclude`. Feeds the per-release charts in `docs/benchmarks/`. |
```

- [ ] **Step 4: Add the CLAUDE.md benchmarks subsection**

Add after the "Conversion factors and square roots at the storage type's precision" section:

```markdown
### Benchmarks and the release charts

`Semantics.Benchmarks` measures three libraries. `.github/workflows/benchmark-history.yml` runs a
fixed set once per release, appends to a history file per subject under `docs/benchmarks/`, and
redraws the chart the README shows. Three things about it are not guessable from the code:

- **`BenchmarkAgainstVersion` must be set in the environment, never with `-p:`.** BenchmarkDotNet
  generates and builds a project of its own for each run, which a property passed on the command line
  never reaches: the benchmark assembly would build against the version asked for and the harness
  against the one pinned centrally, which fails to compile if a type changed shape between them.
  MSBuild reads environment variables as properties in every project, so the environment form reaches
  both. It swaps all four shipped packages at once, which is correct because this repository ships one
  version across every package.
- **The histories and the SVG files are committed output that a bot pushes.** A local `render` must be
  diffed before commit rather than assumed. The quantities chart in particular is a regression test:
  a change to the renderer that moves a byte in it has broken something.
- **Nothing here touches an internal member.** The `InternalsVisibleTo` that would expose one names
  only the test assembly, and a benchmark built on internals could only ever measure the working copy,
  never a published package — which would make the release history impossible to backfill.

`BaselineBenchmarks.ReferenceWork` measures a fixed workload that touches none of this library, so
timings from different CI runners can be compared. **Its body must never change.** Editing it silently
rescales every comparison drawn against history recorded before the edit.
```

- [ ] **Step 5: Verify every link and image resolves**

```bash
cd /c/dev/ktsu-dev/Semantics
for target in $(grep -o 'src="docs/benchmarks/[^"]*"\|srcset="docs/benchmarks/[^"]*"' README.md \
  | sed 's/.*="//;s/"//'); do
  [ -f "$target" ] && echo "ok   $target" || echo "MISSING $target"
done
grep -n "docs/benchmarks/" README.md | head -20
```

Expected: six `ok` lines, no `MISSING`.

- [ ] **Step 6: Full build, full test, Sonar check**

```bash
cd /c/dev/ktsu-dev/Semantics
dotnet build
dotnet test
```

```powershell
cd C:\dev\ktsu-dev\Semantics
dotnet build -p:CustomAfterMicrosoftCommonProps=$PWD\.sonarlint\sonar-local.props
```

Expected: build clean with no warnings, all tests pass, no new Sonar findings. Note that `dotnet test` is not passed `--nologo`, because this repository uses Microsoft.Testing.Platform, where that flag makes the run discover zero tests and exit 5.

- [ ] **Step 7: Final commit with the version tag**

```bash
cd /c/dev/ktsu-dev/Semantics
git add README.md Semantics.Benchmarks/README.md CLAUDE.md
git commit -m "Document the strings and paths benchmarks [patch]

The README gains a chart per library, with the way to read a chart
stated once above all three rather than three times. The strings section
says plainly what the chart shows: a quantity's wrapper is free and a
semantic string's is not, and what that buys.

CLAUDE.md gains the benchmark project, which it never listed, and the
three things about the pipeline that are not guessable from the code."
```

---

## Wrap-up

- [ ] **Push the branch and open a pull request**

```bash
cd /c/dev/ktsu-dev/Semantics
git push -u origin claude/benchmarks-strings-paths-252
gh pr create --fill --base main
```

The pull request body must disclose that it was written by Claude, as the first line, per the outward-facing communication rules in the global instructions. It should state which versions the backfill reached for each subject and name any that were skipped, with reasons.

- [ ] **Raise the deferred issue**

The spec defers a `verify-charts` workflow that would re-render from committed history and fail a pull request on drift, guarding the committed SVG files the way `verify-generated` guards the generated sources. Open it as its own issue, referencing this one.
