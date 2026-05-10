# CLAUDE.md

Guidance for Claude Code working in this repository. Read this together with `docs/strategy-unified-vector-quantities.md` (the architecture spec for the physics system) and `docs/physics-generator.md` (the metadata workflow).

## Build commands

This is a multi-target .NET library (`net10.0;net9.0;net8.0;net7.0`) using ktsu MSBuild SDKs.

- **Build**: `dotnet build`
- **Test**: `dotnet test`
- **Test (verbose)**: `dotnet test --logger "console;verbosity=detailed"`
- **Clean**: `dotnet clean`
- **Restore**: `dotnet restore`
- **Format**: `dotnet format`

Tests use MSTest. Generator output is emitted to `Semantics.Quantities/Generated/` (committed) so the project can be inspected without first running the generator.

## Project layout

| Project | Responsibility |
|---|---|
| `Semantics.Strings` | Strongly-typed string wrappers (`SemanticString<T>`) and validation attributes/strategies. |
| `Semantics.Paths` | Polymorphic file system path types (`IPath`, `IFilePath`, `IDirectoryPath`, …). |
| `Semantics.Quantities` | Hand-written runtime types (`PhysicalQuantity<TSelf, T>`, `IVector0`..`IVector4`, `UnitSystem`) plus generator output under `Generated/`. |
| `Semantics.SourceGenerators` | Roslyn incremental generators that emit quantity types, units, conversions, magnitudes, physical constants, and storage-type helpers from metadata. |
| `Semantics.Test` | MSTest project covering all of the above. |

## Physics quantities architecture (the unified vector model)

The physics system is **metadata-driven**. The single source of truth is
`Semantics.SourceGenerators/Metadata/dimensions.json`, which lists every physical dimension and the vector forms it supports.

Every quantity is a vector. Dimensionality of the *direction space* is part of the type:

| Form | Meaning | Sign | Examples |
|---|---|---|---|
| `IVector0<TSelf, T>` | Magnitude only | Always `>= 0` | `Speed`, `Mass`, `Energy`, `Distance`, `Area` |
| `IVector1<TSelf, T>` | Signed 1D | Signed | `Velocity1D`, `Force1D`, `Temperature`, `ElectricCharge` |
| `IVector2<TSelf, T>` | 2D directional | Per-component | `Velocity2D`, `Force2D`, `Acceleration2D` |
| `IVector3<TSelf, T>` | 3D directional | Per-component | `Velocity3D`, `Force3D`, `Position3D` |
| `IVector4<TSelf, T>` | 4D directional | Per-component | (reserved for relativistic / spacetime) |

`IVectorN.Magnitude()` (for N >= 1) returns the corresponding `IVector0`.

All generated types are generic over a numeric storage type: `where T : struct, INumber<T>`.

### Resolved design decisions

These are now baked into the generator and enforced by tests. **Do not reopen without an architecture discussion.**

1. **`V0 - V0` returns the same `V0` of `T.Abs(a - b)`.** Magnitude subtraction stays non-negative; signed subtraction must use the V1 form explicitly.
2. **Dimensionless and angular quantities have both `Ratio` (V0) and `SignedRatio` (V1) bases.** Ratios that semantically must be non-negative (e.g. `RefractiveIndex`, `MachNumber`, `SpecificGravity`) are V0 overloads of `Ratio`.
3. **Semantic overloads widen implicitly to their base, narrow explicitly from it.** A `Weight` is implicitly a `ForceMagnitude`; the reverse requires `Weight.From(forceMagnitude)` or an explicit cast.
4. **Physical constraints are enforced structurally via the V0 (magnitude) form.** `Vector0` factories run `Vector0Guards.EnsureNonNegative` and throw `ArgumentException` on a negative value. That covers absolute zero (Temperature is V0, so Kelvin must be ≥ 0), non-negative frequency, non-negative absolute pressure, etc. Strict-positive or upper-bound constraints are not yet declared in metadata (tracked separately).

### Physical constants

`PhysicalConstants` is **generated** from `domains.json`. Public surface:

```csharp
// Domain-grouped PreciseNumber values:
PhysicalConstants.Fundamental.SpeedOfLight
PhysicalConstants.Fundamental.PlanckConstant
PhysicalConstants.AngularMechanics.DegreesPerRadian

// Generic accessors that materialise into any T : INumber<T>:
PhysicalConstants.Generic.SpeedOfLight<T>()
PhysicalConstants.Generic.PlanckConstant<T>()
PhysicalConstants.Generic.DegreesPerRadian<T>()
```

Backing values are stored as `PreciseNumber` and converted with `T.CreateChecked` per call.

### Operators and physics relationships

Cross-dimensional relationships are also declared in `dimensions.json` (`integrals`, `derivatives`, `dotProducts`, `crossProducts`). The generator emits operators like:

```csharp
public static Energy<T> operator *(Force1D<T> f, Length<T> d) =>
    Energy<T>.Create(f.Value * d.Value);
```

All values are stored in SI base units, so operators read `.Value` directly. Suppress `CA2225` on physics operators because "named alternates" (`Add`, `Multiply`) don't carry the dimensional meaning:

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage", "CA2225:Operator overloads have named alternates",
    Justification = "Physics relationship operators represent fundamental equations.")]
```

## Code standards

### File headers

```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.
```

Generator-emitted files additionally carry `// <auto-generated />`.

### Validation and error handling

- Throw `ArgumentException` for validation failures (not `FormatException`).
- Throw `DivideByZeroException` when dividing by zero in `DivideToStorage`.
- Use the most specific exception type available.

### Testing

- Use explicit types (no `var`) in test bodies.
- Pre-create fixtures outside measurement loops in performance tests.
- Mark OS-specific tests with `[TestCategory("OS-Specific")]`.
- Use 259-character path limit for cross-platform path tests.
- Force GC before memory measurements: `GC.Collect(); GC.WaitForPendingFinalizers();`

### XML documentation

- `/// <summary>Gets the physical dimension of <quantity> [<symbol>].</summary>` style for dimension properties.
- Include `<param>`, `<returns>`, `<exception>`, and `<see cref="">` tags on public APIs.

## Important implementation notes

### Semantic string creation

```csharp
var email = EmailAddress.Create("user@example.com");
var userId = UserId.Create("USER_123");

// Extension method conversion
var email2 = "user@example.com".As<EmailAddress>();

// Cross-type conversion
var converted = sourceString.As<SourceType, TargetType>();
```

### Path conversion

- `AsAbsolute()` — convert to absolute using current working directory.
- `AsAbsolute(baseDirectory)` — convert to absolute using a specific base.
- `AsRelative(baseDirectory)` — convert to relative against a specific base.

### Working with the source generator

- Edit `Semantics.SourceGenerators/Metadata/dimensions.json` to add a dimension, vector form, semantic overload, or relationship.
- Rebuild `Semantics.SourceGenerators` and the consuming `Semantics.Quantities` project; emitted files appear in `Semantics.Quantities/Generated/Semantics.SourceGenerators/<GeneratorName>/`.
- Treat generator output as committed source. Diff it before commit so accidental regressions are visible.
- Factory names are **plural by convention** (#49). Each entry in `units.json` carries a `factoryName` field — the generator emits `From{factoryName}` (e.g. `Length.FromMeters`, `Mass.FromKilograms`, `Speed.FromMetersPerSecond`, `Length.FromFeet`, `Frequency.FromHertz`). Set it explicitly on every new unit; the generator falls back to `name + "s"` if absent, which is wrong for irregulars and "Per" compounds.
- Generator diagnostics:
  - **SEM001** — a relationship in `dimensions.json` references a dimension that does not exist (typo or rename). The operator is silently dropped.
  - **SEM002** — schema-level validation issue (missing `name`/`symbol`, empty `availableUnits`, duplicate type names, no vector forms declared).
  - **SEM003** — a relationship's explicit `forms` list references a vector form not declared on a participating dimension. Use `forms` to constrain a relationship to specific vector forms (e.g. `crossProducts: [{ "other": "Length", "result": "Torque", "forms": [3] }]`); when omitted, the legacy "emit at every common form" behaviour is preserved.
- See `docs/physics-generator.md` for the full schema and an end-to-end "add a dimension" walk-through.

This file is the entry point. For deeper material:

- `docs/strategy-unified-vector-quantities.md` — architecture spec for the unified vector model.
- `docs/physics-generator.md` — generator + `dimensions.json` schema.
- `docs/architecture.md` — semantic strings/paths/validation architecture (SOLID, design patterns).
- `docs/complete-library-guide.md` — user-facing guide to all components.
- `docs/validation-reference.md` — list of validation attributes.
- `docs/advanced-usage.md` — advanced patterns for strings/paths.
