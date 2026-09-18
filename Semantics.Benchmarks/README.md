# Semantics Benchmarks

A [BenchmarkDotNet](https://benchmarkdotnet.org) suite covering the quantity system: building a
quantity from a unit, reading it back out in one, the generated physics operators, the
componentwise vector operations, and comparison.

## Quantities

### The axis that matters here is the storage type

Every generated quantity is a `readonly record struct` over a storage type — `Length<double>`,
`Length<decimal>`, `Length<PreciseNumber>` — and it does almost nothing of its own. A value is held
in the dimension's SI base unit, so an operator is the storage type's arithmetic and a struct
initialiser, and a unit factory is that plus one multiplication. What a quantity costs is therefore
mostly what its `T` costs, and a change to this library shows up differently per `T`.

So every class here is generic and carries `[GenericTypeArguments]` for `double`, `float`,
`decimal` and `PreciseNumber`, and the summary is read **across** those four rather than down one
of them. The four are chosen to span the interesting ground: two binary floating point types the
hardware does in a register, one decimal type the runtime does in software, and one arbitrary
precision type from another package.

Operands are **parsed from text**, never converted from a `double`. A `decimal` or a
`PreciseNumber` seeded through a double would be measured carrying a double's worth of digits,
which is the opposite of why those types are in the list.

### What each class is for

| Class | What it isolates |
|---|---|
| `ConstructionBenchmarks` | `Create` is the floor — a struct initialiser. `FromMeter` adds the magnitude guard alone. `FromKilometer` and `FromNauticalMile` add a factor read from a `Values<T>` holder, which is what 5.2.0 introduced and what gives a `decimal` quantity its full precision. |
| `UnitConversionBenchmarks` | The way back out, through `In(unit)`. `InCelsius` is the one affine case, carrying an offset as well as a factor. |
| `OperatorBenchmarks` | The generated physics relationships. `Add` is the control, being the one operation that changes no dimension. |
| `VectorBenchmarks` | Componentwise arithmetic, and the square root two of the operations need. `LengthSquared` is `Length` without the root, so the gap between them is the root alone — which for `decimal` and `PreciseNumber` is a Newton refinement rather than a hardware instruction. |
| `ComparisonBenchmarks` | The two comparison routes. The operators are the storage type's own; `CompareTo` and the `IPhysicalQuantity<T>` overload of `Equals` box the argument and check dimensions first, which is what buys the ability to refuse a length against a mass. |
| `AbstractionCostBenchmarks` | The same arithmetic twice, once on the bare storage type and once on quantities over it, paired so BenchmarkDotNet reports the ratio. See below. |
| `BaselineBenchmarks` | Touches none of this library. It exists so that timings taken in different CI jobs can be compared; see its remarks, and do not edit its body. |

#### What the quantity types cost over the bare storage type

`AbstractionCostBenchmarks` is the one that answers the question the rest of the suite only implies:
a quantity is a `readonly record struct` holding one value in the SI base unit, so an operator on it
should be the storage type's own arithmetic and a struct initialiser, and nothing more once the JIT
has inlined it. The C++ projection has always been held to that against bare floats. This holds the
.NET side to it the same way.

Each pair runs identical arithmetic on `T` and on quantities over `T`, with the bare one marked
`Baseline = true`, so the answer is the `Ratio` column rather than two rows divided by hand:

| storage | `Add` | `Multiply` (to `Area`) | allocation |
|---|---|---|---|
| `float` | 1.00 | 0.94 | none either side |
| `double` | 1.02 | 0.94 | none either side |
| `decimal` | 1.04 | 0.90 | none either side |
| `PreciseNumber` | 1.03 | 1.03 | 193 B either side, ratio 1.00 |

The wrapper is free. Read the ratios below 1.00 as noise and code layout rather than as the quantity
being faster than the number inside it — the spread across three short-run iterations covers that
much, and there is no mechanism by which it could be.

The `PreciseNumber` row is the one that says it most precisely, because it is the only storage type
here that allocates at all: **the allocation ratio is exactly 1.00**. Every byte belongs to the
`BigInteger` inside, and the quantity adds none of its own.

**These are loops, and that is deliberate.** A single operator over operands that do not change is
loop-invariant and the JIT hoists it out, which is exactly what makes `OperatorBenchmarks` report
ZeroMeasurement for `double` and `float` — and a ratio between two hoisted methods would mean
nothing. Here each iteration feeds the next, so there is nothing to hoist and both sides of a pair
are measurable for every storage type.

**The loop's own cost biases toward 1.00, not away from it.** Both sides pay the same counter and
branch; it is a dependency chain, so most of that overlaps the arithmetic, and whatever does not is
added equally to numerator and denominator. So a ratio at 1.00 is the claim kept, and a ratio above
it is a floor on the real cost rather than the whole of it.

**The operands stay bounded on purpose.** `PreciseNumber` carries as many digits as the arithmetic
produces, so a chain that compounds its operand would measure digit growth instead of the operation.
Both loops accumulate rather than compound.

#### An operator on a `double` is below the floor

`OperatorBenchmarks` over `double` and `float` comes back with a ZeroMeasurement warning: the
method is indistinguishable from an empty one. That is not a broken benchmark, it is the answer.
A relationship operator on a binary floating point type is one machine instruction over operands
the loop does not change, so the JIT hoists it out entirely.

It cannot be fixed without measuring the fix instead of the operator — an array of operands adds a
load, a mutated field adds a store, and either would swamp the instruction being asked about. So
those rows are read as "below what the harness resolves" rather than as numbers, the `decimal` and
`PreciseNumber` rows in the same table are the ones that mean something, and the release chart
draws no operator panel at all.

## Running

From the repository root:

```bash
# Pick benchmarks from an interactive list
dotnet run -c Release --project Semantics.Benchmarks

# Run everything
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*'

# One class across all four storage types, or one storage type across all classes
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*VectorBenchmarks*'
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*<Decimal>*'

# What the quantity types cost over the bare storage type
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*AbstractionCostBenchmarks*'
```

The suite covers three libraries, and `--filter` is how one is picked:

```bash
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*String*Benchmarks*'
dotnet run -c Release --project Semantics.Benchmarks -- --filter '*Path*Benchmarks*'
```

## Measuring a published release

Set `BenchmarkAgainstVersion` and the suite measures that package instead of the working copy:

```bash
BenchmarkAgainstVersion=5.2.0 dotnet run -c Release --project Semantics.Benchmarks -- --filter '*<Decimal>*'
```

Set it **in the environment, not with `-p:`**. BenchmarkDotNet generates and builds a project of
its own for each run, and a property passed on the command line does not reach that project — it
would build the benchmark assembly against the version you asked for and the harness against the
one pinned centrally, which fails to compile if a type changed shape between them. MSBuild reads
environment variables as properties in every project, so the environment form reaches both.
`BenchmarkAgainstVersion` swaps all four shipped packages, `Quantities`, `Strings`,
`Strings.Identifiers` and `Paths`, at the one version, which is correct because this repository
ships one version across every package.

This switch is how `docs/benchmarks/` is filled. No tag in this repository carries a benchmark
project, so there is no older source to check out and run; and measuring packages is the better
comparison anyway, because every version is timed by identical benchmark code rather than by
whatever each tag happened to ship. A version whose API the current benchmarks cannot express is
reported and skipped rather than failing the backfill — 4.0 made every quantity a record struct and
5.0 removed four operators, so reaching back far enough eventually finds a version this suite
cannot ask.

That skip is also shared across subjects in a way worth naming, because it is the reason the strings
and paths charts start at 4.0.0 with no 3.3.1 point. All three subjects live in one project, so a
compile error in any one file blocks every subject's run regardless of `--filter`. Against packages
older than 4.0.0, `AbstractionCostBenchmarks.cs` — a quantities-only file — fails to build, because
it declares a `Length<T>` field with no initializer, which is only an error once `Length<T>` becomes
a `readonly record struct`; before that migration it was a reference type and the field was valid.
`docs/benchmarks/history.json` still carries a 3.3.1 entry from before that file existed in its
current form, but the current suite cannot regenerate it for any subject.

It is also why nothing here touches an internal member: the `InternalsVisibleTo` that would expose
one names the test assembly, and a benchmark built on internals could only ever measure the
working copy.

## Strings

A suite covering three things: building a semantic string across a ladder of validators, what a
value costs once it exists, and what the type costs over the code a caller would otherwise write.

### What each class is for

| Class | What it isolates |
|---|---|
| `StringCreationBenchmarks` | Creation across a ladder of validators. `Unvalidated` is the reflection floor alone (`Activator.CreateInstance`, a `PropertyInfo.SetValue`, and a strategy lookup that finds nothing), and `CharsetRegex`, `FormatRegex`, `Checksum` and `Mod97` each add one validator on top of it. `TryCreateRejects` and `CreateThrows` are both failure paths, measured side by side. |
| `StringOperationBenchmarks` | What a value costs after creation: equality, ordering, hashing, and two calls that look like field access at a call site but are a full creation in disguise, `AsConversion` and `WithSuffix`. |
| `StringAbstractionCostBenchmarks` | The same operation written twice, once by hand at a boundary and once through the type, paired so BenchmarkDotNet reports the ratio directly. See below. |

Four structurally different validators land within about 340 ns of each other on a floor of roughly
1,650 ns. A Luhn pass, a character-set regular expression, a mod-97 pass and a format regular
expression cost 0.93, 1.06, 1.26 and 1.27 microseconds on top of that floor, so the validator is a
minor term and the reflection machinery is the bill. The last two are a near-tie, worth naming
because the mod-97 check was expected to dominate going in, and it does not.

### What the type costs over the code a caller would otherwise write

| pair | ratio | allocation, bare | allocation, semantic |
|---|---|---|---|
| Validate at a boundary | 12.94 | 0 B | 928 B |
| Reject without throwing | 164.98 | 0 B | 1424 B |
| Equality | 2.53 | 0 B | 0 B |
| Ordering | 1.11 | 0 B | 0 B |

The Reject ratio is the most actionable number in the table. `SemanticString.TryFromString` is
implemented as a `Create` call wrapped in a `try`/`catch` for `ArgumentException`, so a rejection
pays a full .NET exception where the hand-written pattern match returns a plain `false`. At a
boundary that rejects often, `TryCreate` should be read as costing an exception every time, not as
the cheap option its name suggests.

Equality and ordering are a fair pair only once each baseline is pinned to the rule the semantic side
actually follows. Record equality on a semantic string uses `EqualityComparer<string>.Default`,
which is ordinal, while `CompareTo` forwards to `string.CompareTo(string)`, which is
current-culture collation. Two values can therefore compare equal under `==` and still sort by a
different rule. Nothing here changes that behavior. It is reported because it is easy to assume the
two follow the same rule.

## Paths

A suite covering building each path type from a string, operating on a path once it exists, and what
the type costs over `System.IO.Path` doing the same work.

### What each class is for

| Class | What it isolates |
|---|---|
| `PathCreationBenchmarks` | Building each path type from a well-formed string. A path type is a semantic string whose validator asks the runtime a question about the shape of the value, so each row is the reflection floor plus one such question. |
| `PathOperationBenchmarks` | What a path costs once it exists. `FileNameWithoutExtension` caches into a field and reads it back; `FileName` rebuilds and revalidates on every read. Both are properties and both look like field access at a call site. |
| `PathAbstractionCostBenchmarks` | The same operation written twice, once against `System.IO.Path` and once through the type, paired so BenchmarkDotNet reports the ratio directly. See below. |

### What the type costs over `System.IO.Path`

| pair | ratio | allocation, bare | allocation, semantic |
|---|---|---|---|
| FileName | 130.22 | 48 B | 944 B |
| AsAbsolute | 15.69 | 256 B | 1944 B |
| AsRelative | 10.35 | 80 B | 1728 B |
| Create | 1,810.28 | 0 B | 1816 B |

The Create ratio needs its own framing stated next to it, because on its own the number overstates
what changed. It is large because its baseline is cheap, a single boolean check, not because
creation itself is unusually expensive: the semantic side costs roughly the same 4.7 microseconds
across all four categories, and almost all of the spread between the ratios comes from what each one
happens to be divided by. The baseline also deliberately leaves out the separator concatenation
`IsAbsolutePathAttribute` performs before asking the same question, because a baseline in this class
means the code a caller would otherwise write, and a caller checking whether a path is absolute
writes the plain check rather than the library's own way of handling edge cases. That one choice
accounts for roughly a fourteenfold difference in the reported ratio on its own.

## Reading the results

Allocation is reported alongside time and matters just as much. A quantity is a value type, so
anything allocated was allocated by the storage type or by boxing — a `double` or `decimal`
quantity should show 0 B for arithmetic and comparison, a `PreciseNumber` one should show its
`BigInteger` digit arrays, and the interface comparison routes should show a box on every storage
type.
