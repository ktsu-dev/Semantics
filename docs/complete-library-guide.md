# Complete Semantics Library Guide

ktsu.Semantics is a .NET library for replacing primitive obsession with strongly-typed, self-validating domain models. It has three pillars:

- **Semantic Strings** — type-safe string wrappers with attribute-driven validation.
- **Semantic Paths** — polymorphic file system path types with rich operations.
- **Physics Quantities** — a metadata-generated, type-safe physics system built on a unified vector model.

All three share a runtime philosophy: validate at construction time, fail fast with `ArgumentException`, and never let an invalid value into the type.

## Document map

| Topic | Doc |
|---|---|
| Architecture (strings/paths/validation) | `architecture.md` |
| Architecture (physics quantities) | `strategy-unified-vector-quantities.md` |
| Source-generator workflow | `physics-generator.md` |
| Validation attribute reference | `validation-reference.md` |
| Advanced patterns | `advanced-usage.md` |
| Physics quick reference by dimension | `physics-domains-guide.md` |

## Semantic strings

Define a strongly-typed string by deriving from `SemanticString<TSelf>` and decorating with validation attributes:

```csharp
[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[StartsWith("USER_"), HasNonWhitespaceContent]
public sealed record UserId : SemanticString<UserId> { }
```

Construction goes through one of:

```csharp
// Direct, type-inferred
var email  = EmailAddress.Create("user@example.com");
var userId = UserId.Create("USER_12345");

// From char span / array
var email2 = EmailAddress.Create("user@example.com".AsSpan());

// Explicit cast
var email3 = (EmailAddress)"user@example.com";

// Safe creation
if (EmailAddress.TryCreate("maybe@invalid", out EmailAddress? safeEmail)) { /* … */ }
```

Compile-time safety prevents the classic mix-up:

```csharp
public void SendWelcomeEmail(EmailAddress to, UserId userId) { … }
// SendWelcomeEmail(userId, email);  // ❌ won't compile
```

Validation runs through the strategy/rule pipeline — see `architecture.md` and `validation-reference.md`.

### Factory pattern (DI)

Use `SemanticStringFactory<T>` when you want to inject construction:

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

## Semantic paths

Paths are a separate hierarchy on top of `SemanticString<TSelf>`. Everything is a record so equality and immutability come for free.

```
IPath
├── IAbsolutePath          ├── IFilePath
├── IRelativePath          └── IDirectoryPath
├── IAbsoluteFilePath  : IFilePath, IAbsolutePath
├── IRelativeFilePath  : IFilePath, IRelativePath
├── IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
└── IRelativeDirectoryPath : IDirectoryPath, IRelativePath

IFileName, IFileExtension  // separate hierarchies for non-path components
```

```csharp
var configFile = AbsoluteFilePath.Create(@"C:\app\config.json");

configFile.FileName;       // config.json
configFile.FileExtension;  // .json
configFile.DirectoryPath;  // C:\app
configFile.Exists;         // bool

// Polymorphic collections
List<IPath> all = [
    AbsoluteFilePath.Create(@"C:\data.txt"),
    RelativeDirectoryPath.Create(@"logs\app"),
    FilePath.Create(@"document.pdf")
];

var files     = all.OfType<IFilePath>().ToList();
var absolutes = all.OfType<IAbsolutePath>().ToList();
```

### Conversion API

- `AsAbsolute()` — using current working directory.
- `AsAbsolute(baseDirectory)` — using a specific base.
- `AsRelative(baseDirectory)` — relative against a base.

## Physics quantities

The physics system is **metadata-driven**: the source of truth is `Semantics.SourceGenerators/Metadata/dimensions.json`, and the Roslyn generator emits one record per quantity into `Semantics.Quantities/Generated/`.

### The unified vector model

Every quantity is a vector. Direction-space dimensionality is part of the type:

| Form | Sign | Examples |
|---|---|---|
| `IVector0<TSelf, T>` (magnitude) | `>= 0` | `Speed`, `Mass`, `Energy`, `Distance`, `Area` |
| `IVector1<TSelf, T>` (signed 1D) | signed | `Velocity1D`, `Force1D`, `Temperature`, `ElectricCharge` |
| `IVector2<TSelf, T>` (2D) | per-component | `Velocity2D`, `Force2D`, `Acceleration2D` |
| `IVector3<TSelf, T>` (3D) | per-component | `Velocity3D`, `Force3D`, `Position3D` |
| `IVector4<TSelf, T>` | per-component | reserved (relativistic / spacetime) |

`IVectorN.Magnitude()` (for N >= 1) returns the corresponding `IVector0`.

The model and its rationale live in `strategy-unified-vector-quantities.md`. Rules of thumb:

- A `Vector0` is *always* non-negative. `Speed.Create(-1)` throws.
- `V0 - V0` returns the same `V0` of `T.Abs(a - b)` (signed subtraction must use V1 explicitly).
- A semantic overload (e.g. `Weight` over `ForceMagnitude`) implicitly widens to its base; narrowing is explicit.
- All values are stored in SI base units.

### Creating quantities

```csharp
// Vector0 — magnitudes (non-negative)
var speed     = Speed<double>.FromMetersPerSecond(15.0);
var mass      = Mass<double>.FromKilogram(10.0);
var distance  = Distance<double>.FromMeter(5.0);
var energy    = Energy<double>.FromJoule(1_000.0);

// Vector1 — signed scalar
var v1        = Velocity1D<double>.FromMetersPerSecond(-3.5);
var temp      = Temperature<double>.FromKelvin(300.0);

// Vector3 — directional
var force3d   = Force3D<double>.FromNewton(0.0, 0.0, -9.8);
var disp3d    = Displacement3D<double>.FromMeter(3.0, 4.0, 0.0);
```

### Operators and dimensional analysis

Cross-dimensional operators are declared in `dimensions.json` and emitted automatically:

```csharp
// V0 × V0 (magnitudes)
var force      = mass * Acceleration<double>.FromMeter(9.8);  // Mass × Accel = Force
var work       = ForceMagnitude<double>.FromNewton(10.0) * distance;  // F·d = Energy
var power      = work / Duration<double>.FromSecond(2.0);     // W/t = Power

// Vector ops
var workScalar = force3d.Dot(disp3d);                          // Energy
var torque     = force3d.Cross(disp3d);                        // Torque3D
var magnitude  = disp3d.Magnitude();                           // Distance

// Type safety
// var nope = force + temp;   // ❌ compiler error
```

### Semantic overloads

Several dimensions declare narrower-named overloads with implicit widening:

```csharp
var w   = Weight<double>.From(force);                   // Weight is a ForceMagnitude
var fm  = ForceMagnitude<double>.From(w);               // implicit widening also OK
var d   = Distance<double>.FromMeter(10.0);
var rad = Radius<double>.From(d);
var dia = rad.ToDiameter();                             // 20m via metadata-defined relationship
```

Overload preservation: `Weight + Weight => Weight`, but `Weight + Drag => ForceMagnitude` (narrowest-shared base).

### Physical constants

Centralised, generated, and generic over storage type:

```csharp
var c   = PhysicalConstants.Generic.SpeedOfLight<double>();         // 299_792_458 m/s
var h   = PhysicalConstants.Generic.PlanckConstant<double>();
var R   = PhysicalConstants.Generic.GasConstant<decimal>();
var ftM = PhysicalConstants.Conversion.FeetToMeters<double>();      // 0.3048
```

Backing storage is `PreciseNumber`; the accessor converts via `T.CreateChecked` per call.

### Adding new dimensions / overloads / relationships

Edit `dimensions.json` and rebuild — see `physics-generator.md` for the full schema and an end-to-end walk-through.

## Validation system

Validation is attribute-driven and pipes through a strategy + rule architecture:

1. **Attribute layer** — declarative validation on a type (`[IsEmailAddress]`, `[HasNonWhitespaceContent]`).
2. **Strategy layer** — `ValidateAllStrategy` (default), `ValidateAnyStrategy`, or a custom `IValidationStrategy`.
3. **Rule layer** — `IValidationRule` implementations selected per attribute.
4. **Factory layer** — `ValidationStrategyFactory` resolves the right strategy.

```csharp
// Default: all must pass
[HasNonWhitespaceContent, IsEmailAddress, EndsWith(".com")]
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }

// Any can pass
[ValidateAny]
[IsEmailAddress, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

The full attribute list is in `validation-reference.md`. The runtime architecture (interfaces, strategies, contracts) is in `architecture.md`.

## Performance utilities

The library is tuned for throughput-sensitive scenarios:

- `PooledStringBuilder` — pooled, disposable `StringBuilder` for hot paths.
- `InternedPathStrings` — intern frequently-used path literals.
- `SpanPathUtilities` — span-based path manipulation with no allocations.
- Validation runs once at construction and caches the verdict on the immutable record.

## Integration

### ASP.NET Core model binding

```csharp
[HttpPost]
public IActionResult CreateUser([FromBody] CreateUserRequest req)
{
    if (!EmailAddress.TryCreate(req.Email, out var email))
        return BadRequest("Invalid email");
    return Ok(new User(email));
}
```

### Entity Framework Core value conversion

```csharp
modelBuilder.Entity<User>()
    .Property(u => u.Email)
    .HasConversion(
        email => email.ToString(),
        value => EmailAddress.Create(value));
```

### Dependency injection

```csharp
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();
```

## Where to go next

- `strategy-unified-vector-quantities.md` for the physics architecture rationale and the full type hierarchy.
- `physics-generator.md` for the metadata schema and how to add new dimensions.
- `architecture.md` for SOLID/DRY patterns inside strings, paths, and validation.
- `validation-reference.md` for the complete attribute catalogue.
- `advanced-usage.md` for custom validation, contract validation, and DI patterns.
