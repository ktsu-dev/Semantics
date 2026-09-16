# Semantics Benchmarks

A [BenchmarkDotNet](https://benchmarkdotnet.org) suite covering the quantity system: building a
quantity from a unit, reading it back out in one, the generated physics operators, the
componentwise vector operations, and comparison.

## The axis that matters here is the storage type

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

This switch is how `docs/benchmarks/` is filled. No tag in this repository carries a benchmark
project, so there is no older source to check out and run; and measuring packages is the better
comparison anyway, because every version is timed by identical benchmark code rather than by
whatever each tag happened to ship. A version whose API the current benchmarks cannot express is
reported and skipped rather than failing the backfill — 4.0 made every quantity a record struct and
5.0 removed four operators, so reaching back far enough eventually finds a version this suite
cannot ask.

It is also why nothing here touches an internal member: the `InternalsVisibleTo` that would expose
one names the test assembly, and a benchmark built on internals could only ever measure the
working copy.

## What each class is for

| Class | What it isolates |
|---|---|
| `ConstructionBenchmarks` | `Create` is the floor — a struct initialiser. `FromMeter` adds the magnitude guard alone. `FromKilometer` and `FromNauticalMile` add a factor read from a `Values<T>` holder, which is what 5.2.0 introduced and what gives a `decimal` quantity its full precision. |
| `UnitConversionBenchmarks` | The way back out, through `In(unit)`. `InCelsius` is the one affine case, carrying an offset as well as a factor. |
| `OperatorBenchmarks` | The generated physics relationships. `Add` is the control, being the one operation that changes no dimension. |
| `VectorBenchmarks` | Componentwise arithmetic, and the square root two of the operations need. `LengthSquared` is `Length` without the root, so the gap between them is the root alone — which for `decimal` and `PreciseNumber` is a Newton refinement rather than a hardware instruction. |
| `ComparisonBenchmarks` | The two comparison routes. The operators are the storage type's own; `CompareTo` and the `IPhysicalQuantity<T>` overload of `Equals` box the argument and check dimensions first, which is what buys the ability to refuse a length against a mass. |
| `BaselineBenchmarks` | Touches none of this library. It exists so that timings taken in different CI jobs can be compared; see its remarks, and do not edit its body. |

### An operator on a `double` is below the floor

`OperatorBenchmarks` over `double` and `float` comes back with a ZeroMeasurement warning: the
method is indistinguishable from an empty one. That is not a broken benchmark, it is the answer.
A relationship operator on a binary floating point type is one machine instruction over operands
the loop does not change, so the JIT hoists it out entirely.

It cannot be fixed without measuring the fix instead of the operator — an array of operands adds a
load, a mutated field adds a store, and either would swamp the instruction being asked about. So
those rows are read as "below what the harness resolves" rather than as numbers, the `decimal` and
`PreciseNumber` rows in the same table are the ones that mean something, and the release chart
draws no operator panel at all.

## Reading the results

Allocation is reported alongside time and matters just as much. A quantity is a value type, so
anything allocated was allocated by the storage type or by boxing — a `double` or `decimal`
quantity should show 0 B for arithmetic and comparison, a `PreciseNumber` one should show its
`BigInteger` digit arrays, and the interface comparison routes should show a box on every storage
type.
