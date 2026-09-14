# ktsu.Semantics.Quantities.Precise

> Storage-type aliases that bind every `ktsu.Semantics.Quantities` type to `ktsu.PreciseNumber.PreciseNumber`, so you write `Mass` instead of `Mass<PreciseNumber>`, project-wide.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Quantities.Precise?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Precise)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Quantities.Precise?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Precise)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Quantities.Precise?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Precise)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Quantities.Precise` is a satellite of [`ktsu.Semantics.Quantities`](../Semantics.Quantities/README.md) in the [ktsu.Semantics](../README.md) family. Read the core package README first for the quantity API itself.

## Introduction

Every quantity in `ktsu.Semantics.Quantities` is generic over its numeric storage type, so you normally write `Mass<PreciseNumber>`, `Speed<PreciseNumber>`, and so on. If a project uses one storage type throughout, that generic argument is noise.

This package is props-only. It ships no assembly, just a `buildTransitive` props file that injects one C# global-using alias per quantity, binding each open generic type to [`ktsu.PreciseNumber.PreciseNumber`](https://nuget.org/packages/ktsu.PreciseNumber). Reference it and you can write `Mass`, `Speed`, `Force3D` with no generic argument, and every quantity resolves to its `PreciseNumber` form. The aliases are real `Mass<PreciseNumber>` (and so on), so they interoperate with the entire API with no conversion.

Installing this package also pulls in the matching version of `ktsu.Semantics.Quantities` and `ktsu.PreciseNumber` as dependencies, so it is the only reference you need. The core quantities package does not depend on `ktsu.PreciseNumber`; that dependency arrives only with this package.

## Why this storage type

`PreciseNumber` is an arbitrary-precision type — a `BigInteger` significand with a decimal exponent — so a unit factor is applied at the precision the value carries rather than at the 15 to 17 significant digits a `double` holds. Conversions that terminate come out exact:

```csharp
Length.FromFoot(PreciseNumber.One).In(Units.Inch)   // exactly 12
1.0 * 0.3048 / 0.0254                               // 12.000000000000002 in double
```

Roots are computed in the storage type's own arithmetic rather than through a `double` round trip, so a vector length keeps its digits too:

```csharp
Velocity3D.One.Length()   // 1.73205080756887729352744634150587236694280525381038
```

This costs speed and allocation against `double`, so it suits work where the error budget matters more than throughput — long-horizon integration, unit-conversion chains, and reference values to check a faster pipeline against.

Two limits worth knowing. The logarithmic scales (`Decibels`, `Cents`, `PH`, and the rest) still compute through `double` whatever the storage type. And a quotient that does not terminate is rounded to a precision derived from its operands, not carried forever.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Quantities.Precise
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Quantities.Precise
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Quantities.Precise" Version="x.y.z" />
```

## Usage Example

```csharp
using ktsu.Semantics.Quantities;   // types resolve to their PreciseNumber form via the injected aliases
using ktsu.PreciseNumber;

Mass mass = Mass.FromKilogram(10.ToPreciseNumber());
Speed speed = Speed.FromMeterPerSecond(15.ToPreciseNumber());
Mass total = mass + Mass.FromKilogram(2.ToPreciseNumber());   // still a Mass<PreciseNumber>, full identity

Force3D f = new() { X = 3.ToPreciseNumber(), Y = 4.ToPreciseNumber(), Z = PreciseNumber.Zero };
ForceMagnitude mag = f.Magnitude();                           // exactly 5
```

Under the hood the injected alias looks like this (one line per quantity, roughly 220 in total):

```xml
<Using Include="ktsu.Semantics.Quantities.Mass&lt;ktsu.PreciseNumber.PreciseNumber&gt;" Alias="Mass" />
```

## Use exactly one storage-type alias package per project

The aliases are project-wide global usings keyed on the bare type name (`Mass`, `Speed`, ...). Referencing two flavor packages in the same project would define the same alias name twice (one bound to `PreciseNumber`, one to another type), which is a compile error. So reference exactly one of:

- [`ktsu.Semantics.Quantities.Double`](../Semantics.Quantities.Double/README.md)
- [`ktsu.Semantics.Quantities.Float`](../Semantics.Quantities.Float/README.md)
- [`ktsu.Semantics.Quantities.Decimal`](../Semantics.Quantities.Decimal/README.md)
- `ktsu.Semantics.Quantities.Precise` (this package)

A project that genuinely needs mixed storage types should skip the alias packages and reference `ktsu.Semantics.Quantities` directly, writing the closed generic (`Mass<PreciseNumber>`) explicitly.

The alias lists are generated from the quantity catalogue by `scripts/Generate-AliasProps.ps1` and validated in CI, so they stay in lockstep with the quantities the core package emits.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
