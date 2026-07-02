# ktsu.Semantics.Quantities.Decimal

> Storage-type aliases that bind every `ktsu.Semantics.Quantities` type to `decimal`, so you write `Mass` instead of `Mass<decimal>`, project-wide.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Quantities.Decimal?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Decimal)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Quantities.Decimal?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Decimal)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Quantities.Decimal?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Quantities.Decimal)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Quantities.Decimal` is a satellite of [`ktsu.Semantics.Quantities`](../Semantics.Quantities/README.md) in the [ktsu.Semantics](../README.md) family. Read the core package README first for the quantity API itself.

## Introduction

Every quantity in `ktsu.Semantics.Quantities` is generic over its numeric storage type, so you normally write `Mass<decimal>`, `Speed<decimal>`, and so on. If a project uses one storage type throughout, that generic argument is noise.

This package is props-only. It ships no assembly, just a `buildTransitive` props file that injects one C# global-using alias per quantity, binding each open generic type to `decimal`. Reference it and you can write `Mass`, `Speed`, `Force3D` with no generic argument, and every quantity resolves to its `decimal` form. The aliases are real `Mass<decimal>` (and so on), so they interoperate with the entire API with no conversion.

Installing this package also pulls in the matching version of `ktsu.Semantics.Quantities` as a dependency, so it is the only reference you need.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Quantities.Decimal
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Quantities.Decimal
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Quantities.Decimal" Version="x.y.z" />
```

## Usage Example

```csharp
using ktsu.Semantics.Quantities;   // types resolve to their decimal form via the injected aliases

Mass mass = Mass.FromKilogram(10.0m);
Speed speed = Speed.FromMeterPerSecond(15.0m);
Mass total = mass + Mass.FromKilogram(2.0m);   // still a Mass<decimal>, full identity

Force3D f = new() { X = 3.0m, Y = 4.0m, Z = 0.0m };
ForceMagnitude mag = f.Magnitude();            // 5.0
```

Under the hood the injected alias looks like this (one line per quantity, roughly 220 in total):

```xml
<Using Include="ktsu.Semantics.Quantities.Mass&lt;decimal&gt;" Alias="Mass" />
```

## Use exactly one storage-type alias package per project

The aliases are project-wide global usings keyed on the bare type name (`Mass`, `Speed`, ...). Referencing two flavor packages in the same project would define the same alias name twice (one bound to `decimal`, one to another type), which is a compile error. So reference exactly one of:

- [`ktsu.Semantics.Quantities.Double`](../Semantics.Quantities.Double/README.md)
- [`ktsu.Semantics.Quantities.Float`](../Semantics.Quantities.Float/README.md)
- `ktsu.Semantics.Quantities.Decimal` (this package)

A project that genuinely needs mixed storage types should skip the alias packages and reference `ktsu.Semantics.Quantities` directly, writing the closed generic (`Mass<decimal>`) explicitly.

The alias lists are generated from the quantity catalogue by `scripts/Generate-AliasProps.ps1` and validated in CI, so they stay in lockstep with the quantities the core package emits.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
