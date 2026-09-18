# Design: Benchmark history and release charts for semantic strings and paths

Status: approved (2026-09-18). Implements [#252](https://github.com/ktsu-dev/Semantics/issues/252).
Read alongside `Semantics.Benchmarks/README.md` (the existing suite), `scripts/benchmark-history.cs`
(ingest and render), and `.github/workflows/benchmark-history.yml` (the release pipeline).

## Problem

`Semantics.Quantities` has a measured, charted performance history. Every release adds a point to
`docs/benchmarks/history.json`, the chart is redrawn into `docs/benchmarks/performance.svg`, and the
README shows it. `Semantics.Strings` and `Semantics.Paths` have nothing.

The gap is not only that the two packages are unmeasured. It is that the whole pipeline is
single-subject by construction: one history file, one hardcoded headline set, one chart title, one
package swapped by `BenchmarkAgainstVersion`. Adding a second subject is a change to how those pieces
fit together, not an addition alongside them.

There is also a substantive reason to want the numbers. `SemanticString.Create` goes through
`Activator.CreateInstance`, `Type.GetProperty`, and `PropertyInfo.SetValue` on every call, then
validates by walking attributes reflectively. The quantities suite's headline finding was that the
wrapper is free. The strings finding will not be that, and quantifying the difference is the most
valuable thing this work produces.

## Goals

- Measure `Semantics.Strings` and `Semantics.Paths` with the same rigor the quantities suite applies.
- Give each its own committed history file and its own chart, drawn by the same renderer.
- Keep the quantities chart byte-identical through the refactor.
- State what the wrapper costs against the code a user would otherwise have written, honestly paired.
- Record in `CLAUDE.md` what the pipeline is, which it currently does not mention at all.

## Non-goals

- No combined chart across subjects. Each subject gets its own picture.
- No change to what the quantities suite measures or how its chart looks.
- No `verify-charts` drift guard. Real gap, separate concern, its own issue (see Deferred work).
- No test harness for `scripts/benchmark-history.cs`. It is untested tooling today and stays that way.
- No optimization of the code being measured. This work reports numbers, it does not change them.

## Decisions

These were settled during design and should not be reopened without revisiting this document.

1. **One chart per subject.** Three history files, three charts, one renderer. A combined grid would
   have meant migrating the existing quantities history into a new schema for no reader's benefit.
2. **Absolute values on the chart, ratios in the suite README.** The charts draw time and allocation
   per operation, as the quantities chart does. The abstraction-cost ratios are written into
   `Semantics.Benchmarks/README.md` by a human reading a run, not redrawn by CI.
3. **One benchmark project, three subjects.** New classes join `Semantics.Benchmarks` in `Strings/`
   and `Paths/` folders. `BaselineBenchmarks` stays shared, so every subject measured in one job ties
   to the same reference reading.
4. **Subject table in C#, not in a data file.** The headline sets stay in `benchmark-history.cs` as a
   dictionary of records. The prose explaining why a benchmark is or is not drawn is the most
   valuable content in that file, and an XML doc comment holds it better than a JSON field.
5. **History and chart paths stay command-line arguments.** The script keeps knowing nothing about
   repository layout. The workflow holds the paths, which is where paths already live.
6. **Specimens are shipped types wherever one exists.** The string benchmarks use `Uuid`, `Ulid`,
   `CreditCardNumber` and `Iban` from `Semantics.Strings.Identifiers`, so the numbers describe types
   users actually hold. Exactly one fixture type is declared in the benchmark project, for the
   no-validation rung, because nothing shipped occupies it.
7. **Histories are seeded locally.** The two new histories are backfilled on a development machine
   and committed with `runId: "local-seed"`, exactly as the quantities history was. `baselineNs` is
   what makes those points comparable with CI points that follow.

## What gets measured

### Strings

The axis is validation weight. A semantic string is a record wrapping a `string`, so its cost is
concentrated at creation, and what varies between types is what their attributes do. The shipped
identifier types form a genuine ladder: each carries exactly one attribute, and those attributes span
interpreted regular expressions, hand-written checksums, and modular arithmetic.

`StringCreationBenchmarks`:

| Method | Specimen | What it isolates |
|---|---|---|
| `Unvalidated` | fixture type declared in the suite | The reflection machinery alone: `Activator.CreateInstance`, `GetProperty`, `SetValue`, then a strategy lookup finding no attributes. The floor every other row sits on. |
| `CharsetRegex` | `Ulid` | That floor plus an interpreted `Regex.IsMatch` over a fixed character set. |
| `FormatRegex` | `Uuid` | The same, over a pattern with groups and separators. |
| `Checksum` | `CreditCardNumber` | The floor plus a hand-written Luhn pass. Paired against the two above, this is regular expressions against arithmetic at comparable input lengths. |
| `Mod97` | `Iban` | The heaviest shipped validator: rearrangement, character-to-digit expansion, modular arithmetic. |
| `TryCreateRejects` | `Uuid` | The failure path that does not throw. |
| `CreateThrows` | `Uuid` | The failure path that does, so the cost of choosing `Create` over `TryCreate` at a boundary that sees bad input is a number rather than a guess. |

The one fixture type is unavoidable. `Semantics.Strings` ships the framework and no concrete types,
so nothing shipped sits on the no-validation rung. It is declared against the public API only.

`StringOperationBenchmarks` covers life after creation: `Equals`, `CompareTo`, `GetHashCode`,
`As<TDest>` (which re-runs the entire creation path against the target type, and should read as
such), `WithSuffix` (likewise), and the implicit conversion back to `string`.

### Paths

`PathCreationBenchmarks` builds `AbsoluteFilePath`, `RelativeFilePath`, `AbsoluteDirectoryPath` and
`FileName` from well-formed input.

`PathOperationBenchmarks` covers `FileName`, `FileNameWithoutExtension`, `DirectoryPath`,
`AsAbsolute(baseDirectory)`, `AsRelative(baseDirectory)` and `WithoutExtension`.

Two notes on that set. `FileNameWithoutExtension` caches into a field on `AbsoluteFilePath`, while
`FileName` on `SemanticFilePath` builds a fresh `FileName`, validation included, on every read.
Charting both makes that difference visible, and it is the kind of thing a release could quietly
change. And `IsDirectory` and `IsFile` are excluded outright, because they call `Directory.Exists` and
`File.Exists` and would measure the filesystem rather than the library.

### Headline panels

Of everything measured, these are what the two charts draw, four columns by two rows to match the
quantities grid. Everything else stays in the history file and can be promoted later without
re-running anything, because `ingest` records every benchmark in a run and only `render` selects.

Strings:

| Position | Benchmark | Label |
|---|---|---|
| 1 | `StringCreationBenchmarks.Unvalidated` | Create (no validation) |
| 2 | `StringCreationBenchmarks.CharsetRegex` | Create (charset regex) |
| 3 | `StringCreationBenchmarks.FormatRegex` | Create (format regex) |
| 4 | `StringCreationBenchmarks.Mod97` | Create (mod-97) |
| 5 | `StringCreationBenchmarks.TryCreateRejects` | TryCreate (rejects) |
| 6 | `StringCreationBenchmarks.CreateThrows` | Create (throws) |
| 7 | `StringOperationBenchmarks.AsConversion` | As&lt;T&gt; conversion |
| 8 | `StringOperationBenchmarks.CompareTo` | CompareTo |

Paths:

| Position | Benchmark | Label |
|---|---|---|
| 1 | `PathCreationBenchmarks.AbsoluteFilePath` | Create (absolute file) |
| 2 | `PathCreationBenchmarks.RelativeFilePath` | Create (relative file) |
| 3 | `PathCreationBenchmarks.FileNameType` | Create (file name) |
| 4 | `PathOperationBenchmarks.FileName` | FileName (uncached) |
| 5 | `PathOperationBenchmarks.FileNameWithoutExtension` | FileName (cached) |
| 6 | `PathOperationBenchmarks.AsAbsolute` | AsAbsolute |
| 7 | `PathOperationBenchmarks.AsRelative` | AsRelative |
| 8 | `PathOperationBenchmarks.WithoutExtension` | WithoutExtension |

Both sets are subject to the measurability check under Verification. A benchmark that reports
`ZeroMeasurement` is replaced by the next candidate from its class rather than left on the chart.

## The cost pairs

This is where strings differ from quantities in kind rather than degree.

The quantities pair is fair because both sides do identical work. `T + T` against
`Length<T> + Length<T>` is the same addition, and the only open question is what the wrapper adds.
There is no equivalent for `Uuid.Create(s)`, because the bare-string counterpart is an assignment,
which is no work at all. Paired against that, the wrapper's ratio would be a large number restating
only that validation is not free, which needs no benchmark.

So `StringAbstractionCostBenchmarks` pairs against the code a user would otherwise have written, with
the hand-written side marked `Baseline = true`:

| Pair | Baseline side | Semantic side |
|---|---|---|
| Validate at a boundary | `Regex.IsMatch(s, pattern)` then throw on failure, the same pattern the attribute uses | `Uuid.Create(s)` |
| Validate without throwing | the same match, returning a `bool` | `Uuid.TryCreate(s, out _)` |
| Equality | `string.Equals(a, b, StringComparison.Ordinal)` | `Uuid` record equality |
| Ordering | `string.CompareTo` | `Uuid.CompareTo` |

Read that way, the ratio answers the question a user actually has. *I was going to validate this
anyway, so what does routing it through the type cost me on top?* The answer separates into the
validation both sides pay and the per-call reflection only one side does. The last two rows are fair
pairs in the quantities sense and are expected near 1.00.

`PathAbstractionCostBenchmarks` needs none of that care, because `System.IO.Path` is a real API doing
the real work: `FileName` against `Path.GetFileName`, `AsAbsolute` against `Path.GetFullPath`,
`AsRelative` against `Path.GetRelativePath`, and creation against a hand-written `Path.IsPathRooted`
guard.

Both cost classes use the accumulating-loop shape the quantities cost class established, for the same
reason: a single call over an unchanging operand is loop-invariant, gets hoisted, and would make the
ratio meaningless.

Neither cost class is plotted. Their ratios go into `Semantics.Benchmarks/README.md` as tables, the
way the quantities ratios sit there today.

## The script

Every subject-specific thing in `scripts/benchmark-history.cs` is inside `render`. `ingest` never
touches the headline set or the subject name. It takes `--history` and `--results` and records
whatever the run produced, and the results directory is already per-subject-per-version because the
filter selected it. **`ingest` therefore needs no change.**

What `render` holds today, by line:

```text
line  25  Columns = 4
line  54  Headline = [ ...eight quantities entries... ]
line 424  rows computed from Headline.Length
line 444  aria-label "Semantics.Quantities allocation and relative time per release"
line 456  title      "Semantics.Quantities performance by release"
line 478  loop over Headline
```

Those become a subject:

```csharp
private sealed record Subject(string Title, int Columns, Panel[] Headline);
private sealed record Panel(string Key, string? Parameters, string Label);

private static readonly Dictionary<string, Subject> Subjects = new(StringComparer.Ordinal)
{
    ["quantities"] = new("Semantics.Quantities", 4, [ /* exactly today's eight */ ]),
    ["strings"]    = new("Semantics.Strings",    4, [ /* eight */ ]),
    ["paths"]      = new("Semantics.Paths",      4, [ /* eight */ ]),
};
```

`render` gains `--subject`, looks the subject up, and threads it through `Draw`, `Preamble` and
`Section`. The two title strings become interpolations over `subject.Title`. `Columns` moves from a
constant onto the record. Nothing else in the file moves.

The `(parameters) digits` suffix in `Section` stays untouched. It is quantities-only formatting,
reached only when a benchmark carries parameters, and none of the new ones do. Generalizing it would
be churn for no reader.

## The workflow and project wiring

**The csproj.** `BenchmarkAgainstVersion` currently swaps one `ProjectReference` for one
`PackageReference`. It grows to four pairs: `Quantities`, `Strings`, `Paths`, `Strings.Identifiers`.
That is correct because this repository ships one version across every package. `Semantics.Paths`
already references `Semantics.Strings`, so the project-reference side could omit it, but all four are
listed explicitly on both sides. The package side must name all four regardless, and two lists that
differ in membership invite the wrong edit later.

**The subject table.** Three `(name, history, chart, filter)` triples, and environment variables
cannot hold arrays. One block-scalar variable with one line per subject, parsed with
`IFS='|' read`, keeps the table in one place:

```yaml
env:
  SUBJECTS: |
    quantities|docs/benchmarks/history.json|docs/benchmarks/performance.svg|<filter>
    strings|docs/benchmarks/strings-history.json|docs/benchmarks/strings-performance.svg|<filter>
    paths|docs/benchmarks/paths-history.json|docs/benchmarks/paths-performance.svg|<filter>
```

Each of the four steps that currently does one thing loops that table, skipping any subject not named
in the dispatch's new `subjects` input (default: all three). The release path loops subjects inside
the existing worktree. The backfill loops versions and then subjects, so one `BenchmarkAgainstVersion`
build serves all three subjects at that version. The render step loops. The commit step stages three
histories and six SVG files and still makes one push.

**Unchanged deliberately.** The baseline is measured once, before the loops, and stamped on every
entry the job produces. One runner, one reading, and that is what lets a strings point and a
quantities point from the same job be compared at all. The `concurrency` group stays a single serial
queue, so two dispatches cannot race the same files.

**Changed: the timeout.** It is 240 minutes for one subject. A three-subject backfill across ten
versions is roughly triple. `timeout-minutes` goes to 360, with a comment saying the `subjects` input
is the intended way to split a long backfill rather than raising the number further.

**Failure semantics carry over unchanged.** A version whose API the current benchmarks cannot express
is warned and skipped rather than failing the run, and so is a run that builds but reports a table of
`NA`. With three subjects that matters more, not less. `Semantics.Strings` has been far more stable
than the quantities package, so a backfill will likely reach versions where the strings benchmarks run
and the quantities ones do not. Skipping per subject per version, rather than per version, is what
lets the strings chart reach back further than the quantities one.

## Documentation

**`README.md`, the `## Performance` section.** Today it is one picture and four paragraphs, and the
prose is entirely about storage types. It becomes three subsections, one per subject, each with its
own `<picture>` and a paragraph saying what its axis is and why.

The two paragraphs that are not subject-specific are lifted out and stated once above the three: that
allocation is exact and a step in it is always a real change, while time is measured on shared CI
runners and divided by a reference workload, making it indicative rather than precise.

The new prose must be honest about what the charts show. The quantities paragraph gets to say the
wrapper is free. The strings paragraph will be saying that creation goes through
`Activator.CreateInstance`, a `PropertyInfo.SetValue` and a reflective attribute walk on every call,
and that this is what the numbers are. Better the README states that plainly than a reader discovers
it from a chart with no explanation attached.

**`Semantics.Benchmarks/README.md`.** Currently framed end to end around one subject, opening "The
axis that matters here is the storage type." It grows a subject heading above that material and two
siblings:

- *Quantities*: everything there now, moved down a heading level, otherwise untouched.
- *Strings*: the validation ladder, why the specimens are shipped identifier types, why the one
  fixture exists, and the cost-pair table with its explanation of why the baseline side is
  hand-written validation rather than a bare string.
- *Paths*: the operation set, why `IsDirectory` and `IsFile` are excluded, the cached against
  uncached contrast, and the `System.IO.Path` cost-pair table.

"Running" and "Measuring a published release" stay shared and gain a line each about `--filter`
selecting a subject and about the four packages `BenchmarkAgainstVersion` now swaps.

The two ratio tables ship with real numbers from the seeding run. An empty table with a note saying
it will be filled in later is how a document starts rotting.

**`CLAUDE.md`.** `Semantics.Benchmarks` joins the project-layout table, which omits it entirely today.
A short "Benchmarks and the release charts" subsection records the three things that are non-obvious
and will otherwise be rediscovered the hard way:

- `BenchmarkAgainstVersion` must be set in the environment rather than with `-p:`, because
  BenchmarkDotNet builds a generated project of its own that a command-line property never reaches.
- The histories and SVG files are committed output that a bot pushes, so a local `render` must be
  diffed rather than assumed.
- The benchmark project deliberately touches no internal member, because the `InternalsVisibleTo`
  that would expose one names only the test assembly, and a benchmark built on internals could only
  ever measure the working copy.

## Verification

1. **Byte equality gates everything.** Before anything else, `render --subject quantities` against the
   untouched `history.json` must reproduce `performance.svg` and `performance-dark.svg` exactly.
   `git diff` empty, or the refactor is wrong.
2. **Every new benchmark is checked for being measurable.** The headline sets above are proposals, not
   commitments. Each new class runs at `--job short`, the warnings get read, and any benchmark coming
   back as `ZeroMeasurement` or `NA` is dropped or replaced before it reaches a chart. Anything cut is
   reported with its reason rather than quietly substituted. `Equals` and `CompareTo` on the strings
   operations chart are the plausible casualties, followed by the path property reads.
3. **Seeding proceeds one version first.** The full run is two new subjects across a ten-version list
   and will take hours. Version 5.3.4 is seeded alone and its numbers read against expectations
   stated in advance: the unvalidated floor below every validated rung, `Iban` slowest, allocation
   non-zero everywhere because a semantic string is a reference type. The full backfill starts only
   once those hold. If they do not hold, that is a finding to raise, not something to chart.
4. **Ordinary gates.** `dotnet build` warnings-clean, since ktsu.Sdk treats warnings as errors.
   `dotnet test` green. `Semantics.Benchmarks` carries `SonarQubeExclude` so the new benchmark classes
   are not analysed, but `scripts/benchmark-history.cs` is, so the local Sonar build documented in
   `CLAUDE.md` runs over the script change rather than findings surfacing after a push.

## Version tag

`[patch]`. No library API changes. This is tooling, CI and documentation, matching the precedent of
the commit that introduced the quantities chart.

## Deferred work

A `verify-charts` workflow, re-rendering from committed history and failing a pull request on drift,
would guard the committed SVG files the way `verify-generated` guards the generated sources. It is a
real gap, and the quantities chart has had it since the day it shipped. It is a separate concern from
this issue, and bundling it would change the CI contract for a reason the issue never raised. It
should be its own issue.
