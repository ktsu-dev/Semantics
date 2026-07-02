# ktsu.Semantics.Quantities

> A metadata-generated, type-safe physical quantity system built on a unified vector model, with compile-time dimensional analysis, generated unit conversions and physics operators, and centralized physical constants.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Quantities?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Quantities?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Quantities?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Quantities` is one package in the [ktsu.Semantics](../README.md) family. If you use a single numeric storage type throughout a project, the alias packages [`ktsu.Semantics.Quantities.Double`](../Semantics.Quantities.Double/README.md), [`.Float`](../Semantics.Quantities.Float/README.md), and [`.Decimal`](../Semantics.Quantities.Decimal/README.md) let you drop the generic argument and write `Mass` instead of `Mass<double>`.

## Introduction

`ktsu.Semantics.Quantities` gives you physical quantities as types, so a `Force` cannot be added to a `Speed` and multiplying a `Mass` by an `AccelerationMagnitude` yields a `ForceMagnitude` at compile time. Every quantity is generic over its numeric storage type (`Mass<double>`, `Speed<float>`, `Length<decimal>`), and all values are stored in SI base units.

The type surface is generated. A single source of truth, `dimensions.json`, drives a Roslyn incremental generator that emits the quantity records, their `From{Unit}` factories with built-in conversions, the cross-dimensional physics operators, and the physical constants. That is roughly 72 physical dimensions and over 200 generated quantity types, all committed to source so the project compiles without first running the generator.

Every quantity is a vector, and the dimensionality of its direction space is part of the type.

| Form | Meaning | Sign | Examples |
|------|---------|------|----------|
| `IVector0` | magnitude only | always `>= 0` | `Speed`, `Mass`, `Energy`, `Distance`, `Area` |
| `IVector1` | signed 1D | signed | `Velocity1D`, `Force1D`, `Temperature` |
| `IVector2` | 2D directional | per-component | `Velocity2D`, `Force2D`, `Acceleration2D` |
| `IVector3` | 3D directional | per-component | `Velocity3D`, `Force3D`, `Position3D` |
| `IVector4` | 4D directional | per-component | reserved (relativistic / spacetime) |

`IVectorN.Magnitude()` (for N >= 1) returns the matching `IVector0` quantity.

## Features

- **Compile-time dimensional safety**: illegal combinations do not compile, and cross-dimensional operators produce the correct result type.
- **Generated unit conversions**: `From{Unit}` factories per declared unit (`Mass.FromKilogram`, `Speed.FromMeterPerSecond`, `Length.FromFoot`), converting to the SI base unit on construction, with `In(unit)` to convert back.
- **Unified vector model**: `IVector0` through `IVector4`, with `Magnitude()`, `Dot`, `Cross`, `Normalize`, and typed cross-quantity results (`Force3D.Dot(Displacement3D)` returns `Energy`).
- **Construction-time invariants**: `IVector0` magnitudes are guarded non-negative, and quantities where zero is unphysical (`Wavelength`, `Period`, `HalfLife`) are guarded strictly positive.
- **Physical constants**: `PhysicalConstants` with domain-grouped `PreciseNumber` values and generic accessors that materialize into any `T : INumber<T>`.
- **Generator diagnostics**: metadata problems (`SEM001`-`SEM005`) are caught at build time.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Quantities
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Quantities
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Quantities" Version="x.y.z" />
```

For a project that uses one storage type everywhere, reference an alias package instead (or in addition) so you can omit the generic argument. See [storage-type aliases](#storage-type-aliases).

## Usage Examples

### Basic Example: magnitudes and operators

```csharp
using ktsu.Semantics.Quantities;

Mass<double> m = Mass<double>.FromKilogram(2.0);
Speed<double> v = Speed<double>.FromMeterPerSecond(3.0);

MomentumMagnitude<double> p = m * v;   // Mass * Speed -> MomentumMagnitude
double kg = m.Value;                   // stored SI-base value

// arithmetic and comparison are inherited
Speed<double> faster = v + Speed<double>.FromMeterPerSecond(5.0);
bool ok = faster > v;

// construction-time guard: a negative magnitude throws ArgumentException
// Speed<double>.FromMeterPerSecond(-1.0);
```

### Vector quantities

```csharp
using ktsu.Semantics.Quantities;

Force3D<double> f = new() { X = 3.0, Y = 4.0, Z = 0.0 };
ForceMagnitude<double> mag = f.Magnitude();     // matching Vector0 quantity, value 5.0
Force3D<double> unit = f.Normalize();

Displacement3D<double> d = new() { X = 1.0, Y = 0.0, Z = 0.0 };
Energy<double> work = f.Dot(d);                 // typed dot: Force . Displacement = Energy
Torque3D<double> torque = f.Cross(d);           // typed cross: Force x Displacement = Torque
Momentum3D<double> impulse = f * Duration<double>.Create(2.0);
```

### Physical constants

```csharp
using ktsu.Semantics.Quantities;

// domain-grouped, exact PreciseNumber values
var c = PhysicalConstants.Fundamental.SpeedOfLight;      // 299_792_458 m/s
var R = PhysicalConstants.Chemistry.GasConstant;         // 8.31446... J/(mol.K)

// generic accessors materialize into any T : INumber<T>
double g = PhysicalConstants.Generic.StandardGravity<double>();   // 9.80665
ForceMagnitude<double> weight = Mass<double>.FromKilogram(70.0) * AccelerationMagnitude<double>.Create(g);
```

## API Reference

### Hand-written runtime types

| Type | Description |
|------|-------------|
| `SemanticQuantity<TSelf, T>` | Arithmetic base. `Create(T)` factory, `Quantity` value, and inherited `+ - * / -` operators. Divide by zero throws `DivideByZeroException`. |
| `PhysicalQuantity<TSelf, T>` | Abstract base for scalar quantities. Adds `Value`, `Dimension`, `IsPhysicallyValid`, comparison operators, and cross-dimension-aware `CompareTo`/`Equals`. |
| `IVector0<TSelf, T>` | Magnitude-only marker: `Value`, static `Zero`. Non-negative by construction. |
| `IVector1<TSelf, T>` | Signed single-axis: `Value`, static `Zero`. |
| `IVector2` / `IVector3` / `IVector4` | Directional vectors with `X`/`Y`/`Z`/`W`, `Length()`, `LengthSquared()`, `Dot`, `Distance`, `Normalize`; `IVector3` adds `Cross`. |
| `Vector0Guards` | `EnsureNonNegative(value, name)` and `EnsurePositive(value, name)`, used by generated `From{Unit}` factories. |
| `UnitSystem` | enum classifying units (`SIBase`, `SIDerived`, `Metric`, `Imperial`, ...). |

### Generated quantity types

Each generated quantity is a `partial record Name<T> where T : struct, INumber<T>`. A typical `IVector0` quantity such as `Mass<T>` exposes:

| Member | Description |
|--------|-------------|
| `From{Unit}(T)` | e.g. `FromKilogram`, `FromGram`, `FromPound`. Converts to SI base and applies the guard. |
| `Create(T)` | Inherited base factory (value already in SI base units). |
| `Value` | The stored SI-base value. |
| `In(unit)` | Convert the value back to a specific unit. |
| typed operators | e.g. `Mass * AccelerationMagnitude -> ForceMagnitude`, `Mass / Volume -> Density`. |

Vector quantities (`Force3D<T>`, `Velocity2D<T>`, ...) implement the matching `IVectorN` and add `Magnitude()` (returning the corresponding `*Magnitude` Vector0 quantity), `Dot`, `Cross`, and typed cross-quantity results.

Factory names use the singular lemma of each unit name verbatim (`FromMeterPerSecond`, `FromRevolutionPerMinute`). There is no pluralization step.

### `PhysicalConstants`

- **Domain-grouped**: nested static classes returning `PreciseNumber`, for example `PhysicalConstants.Fundamental.SpeedOfLight`, `PhysicalConstants.ClassicalMechanics.StandardGravity`, `PhysicalConstants.Thermodynamics.WaterTriplePoint`. Domain groups include Acoustics, AngularMechanics, Chemistry, ClassicalMechanics, FluidMechanics, Fundamental, NuclearPhysics, Optics, and Thermodynamics.
- **Generic**: `PhysicalConstants.Generic.Name<T>()` materializes any constant into a `T : INumber<T>`.

## Architecture

The system is metadata-driven. The single source of truth is `Semantics.SourceGenerators/Metadata/dimensions.json` (alongside `units.json`, `magnitudes.json`, `conversions.json`, `domains.json`, and `logarithmic.json`). A Roslyn incremental generator emits one record per quantity, a `From{Unit}` factory per declared unit, the cross-dimensional `*` / `/` / `Dot` / `Cross` operators declared in the metadata, and the `PhysicalConstants` surface. Logarithmic-scale quantities (decibels, cents, pH) are generated separately because they do not obey linear arithmetic.

Generated output is committed to `Generated/`, so the project compiles without first running the generator. For the full design and an end-to-end "add a dimension" walk-through, see:

- [Unified vector model strategy](../docs/strategy-unified-vector-quantities.md)
- [Source-generator workflow and schema](../docs/physics-generator.md)
- [Physics domains tour](../docs/physics-domains-guide.md)

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
