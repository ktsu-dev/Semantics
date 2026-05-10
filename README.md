# ktsu.Semantics

[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![Build Status](https://github.com/ktsu-dev/Semantics/workflows/CI/badge.svg)](https://github.com/ktsu-dev/Semantics/actions)

A .NET library for replacing primitive obsession with strongly-typed, self-validating domain models. Three pillars:

- **Semantic Strings** — type-safe wrappers like `EmailAddress`, `UserId`, `BlogSlug` with attribute-driven validation.
- **Semantic Paths** — polymorphic `IPath` hierarchy for files, directories, absolute, relative, and combinations.
- **Physics Quantities** — a metadata-generated, type-safe physics system with a unified `IVector0..IVector4` model covering 62 physical dimensions and ~195 generated types.

Targets `net10.0`, `net9.0`, `net8.0`, `net7.0`.

## Install

```bash
dotnet add package ktsu.Semantics
```

## Semantic strings

```csharp
using ktsu.Semantics.Strings;

[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[StartsWith("USER_"), HasNonWhitespaceContent]
public sealed record UserId : SemanticString<UserId> { }

// Direct construction — no generic params needed
var email  = EmailAddress.Create("user@example.com");
var userId = UserId.Create("USER_12345");

// Span-based and char[] overloads exist too
var email2 = EmailAddress.Create("user@example.com".AsSpan());

// Safe creation
if (EmailAddress.TryCreate("maybe@invalid", out EmailAddress? safe)) { /* … */ }

// Compile-time safety
public void SendWelcomeEmail(EmailAddress to, UserId userId) { /* … */ }
// SendWelcomeEmail(userId, email);   // ❌ compiler error
```

### Combining attributes

```csharp
// All must pass (default)
[IsEmailAddress, EndsWith(".com")]
public sealed record DotComEmail : SemanticString<DotComEmail> { }

// Any can pass
[ValidateAny]
[IsEmailAddress, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

The full attribute catalogue (text, format, casing, first-class .NET types, paths) lives in [`docs/validation-reference.md`](docs/validation-reference.md).

## Semantic paths

```csharp
using ktsu.Semantics.Paths;

var configFile = AbsoluteFilePath.Create(@"C:\app\config.json");

configFile.FileName;       // config.json
configFile.FileExtension;  // .json
configFile.DirectoryPath;  // C:\app
configFile.Exists;         // bool

// Polymorphic collections
List<IPath> all = [
    AbsoluteFilePath.Create(@"C:\data.txt"),
    RelativeDirectoryPath.Create(@"logs\app"),
    FilePath.Create(@"document.pdf"),
];

var files     = all.OfType<IFilePath>().ToList();
var absolutes = all.OfType<IAbsolutePath>().ToList();
```

Conversions: `AsAbsolute()`, `AsAbsolute(baseDirectory)`, `AsRelative(baseDirectory)`.

## Physics quantities

Every quantity is a vector. Direction-space dimensionality is part of the type:

| Form | Sign | Examples |
|---|---|---|
| `IVector0` (magnitude) | `>= 0` | `Speed`, `Mass`, `Energy`, `Distance`, `Area` |
| `IVector1` (signed 1D) | signed | `Velocity1D`, `Force1D`, `Temperature` |
| `IVector2` (2D) | per-component | `Velocity2D`, `Force2D`, `Acceleration2D` |
| `IVector3` (3D) | per-component | `Velocity3D`, `Force3D`, `Position3D` |
| `IVector4` (4D) | per-component | reserved (relativistic / spacetime) |

```csharp
using ktsu.Semantics.Quantities;

// V0 magnitudes — From{Unit} factories use the plural form (#49)
var speed    = Speed<double>.FromMetersPerSecond(15.0);
var mass     = Mass<double>.FromKilograms(10.0);
var distance = Distance<double>.FromMeters(5.0);

// V3 directional — object-initializer syntax (X/Y/Z components)
var force3d  = new Force3D<double> { X = 0.0, Y = 0.0, Z = -9.8 };
var disp3d   = new Displacement3D<double> { X = 3.0, Y = 4.0, Z = 0.0 };

// Operators flow from dimensions.json
var work     = ForceMagnitude<double>.FromNewtons(10.0) * distance;    // F·d = Energy
var power    = work / Duration<double>.FromSeconds(2.0);               // W/t = Power

// Vector ops
var workDot  = force3d.Dot(disp3d);                                    // Energy
var torque   = force3d.Cross(disp3d);                                  // Torque3D
var mag      = disp3d.Magnitude();                                     // Length (always >= 0)

// Construction-time invariants (#50, #51)
// Speed.FromMetersPerSecond(-1.0)        // ArgumentException — V0 must be non-negative
// Wavelength<double>.FromMeters(0.0)     // ArgumentException — strict-positive overload

// Type safety
// var nope = force3d + speed;            // ❌ compiler error
```

Semantic overloads (e.g. `Weight` over `ForceMagnitude`, `Diameter` ↔ `Radius`):

```csharp
var raw    = ForceMagnitude<double>.FromNewtons(686.0);
var weight = Weight<double>.From(raw);   // explicit narrow
ForceMagnitude<double> back = weight;    // implicit widen

var radius   = Radius<double>.FromMeters(2.0);
var diameter = radius.ToDiameter();       // 4 m via metadata-defined relationship
```

Physical constants are exposed in two shapes:

```csharp
// Domain-grouped, exact PreciseNumber values:
PhysicalConstants.Fundamental.SpeedOfLight;    // 299_792_458 m/s as PreciseNumber
PhysicalConstants.Chemistry.GasConstant;       // 8.31446... J/(mol·K)

// Generic accessors — materialise into any T : INumber<T>:
var c = PhysicalConstants.Generic.SpeedOfLight<double>();
var R = PhysicalConstants.Generic.GasConstant<decimal>();
```

The unified vector model and its rationale: [`docs/strategy-unified-vector-quantities.md`](docs/strategy-unified-vector-quantities.md).
A per-domain tour: [`docs/physics-domains-guide.md`](docs/physics-domains-guide.md).
How the source generator turns `dimensions.json` into types: [`docs/physics-generator.md`](docs/physics-generator.md).

## Dependency injection

```csharp
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();

public class UserService(ISemanticStringFactory<EmailAddress> emails)
{
    public Task<User> CreateUserAsync(string raw) =>
        emails.TryCreate(raw, out var email)
            ? Task.FromResult(new User(email))
            : throw new ArgumentException("invalid email");
}
```

## Architecture

The physics system is **metadata-driven**. The single source of truth is
`Semantics.SourceGenerators/Metadata/dimensions.json` (with `units.json`, `magnitudes.json`, `conversions.json`, and `domains.json` alongside it), and a Roslyn incremental generator emits:

- One record per quantity (Vector0/1/2/3/4 base + semantic overloads).
- A `From{Unit}` factory per declared unit, with built-in unit conversion to the SI base unit and a `Vector0Guards` enforce-non-negative (or strict-positive) check on V0 types.
- Cross-dimensional `*`, `/`, `Dot`, `Cross` operators driven by `integrals` / `derivatives` / `dotProducts` / `crossProducts` declarations.
- `PhysicalConstants` with both domain-grouped `PreciseNumber` fields and generic `T.CreateChecked`-backed accessors.

Generator diagnostics catch metadata problems at build time:

- **SEM001** — relationship references an unknown dimension name.
- **SEM002** — schema-level metadata issue (missing fields, duplicate type names, etc).
- **SEM003** — relationship's `forms` list references a vector form not declared on a participating dimension.

Generated output is committed to `Semantics.Quantities/Generated/` so the project compiles without first running the generator.

The string and path systems use the same building blocks: an attribute → strategy → rule → factory pipeline. See [`docs/architecture.md`](docs/architecture.md).

## Documentation

- [Complete library guide](docs/complete-library-guide.md) — start here for a feature tour.
- [Architecture (strings/paths/validation)](docs/architecture.md)
- [Architecture (physics — unified vector model)](docs/strategy-unified-vector-quantities.md)
- [Source-generator workflow](docs/physics-generator.md)
- [Physics domains tour](docs/physics-domains-guide.md)
- [Validation attributes reference](docs/validation-reference.md)
- [Advanced usage patterns](docs/advanced-usage.md)

## Contributing

Contributions are welcome — please open an issue first for major changes so we can discuss the direction. The branch's open work items are tracked as GitHub issues.

## License

MIT — see [`LICENSE.md`](LICENSE.md).
