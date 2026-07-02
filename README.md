# ktsu.Semantics

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

A .NET family of libraries for replacing primitive obsession with strongly-typed, self-validating domain models. Each pillar ships as its own NuGet package, so you take only what you need.

- **Semantic Strings**: type-safe wrappers like `EmailAddress`, `UserId`, `BlogSlug` with attribute-driven validation, plus a batteries-included identifiers package (`Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`).
- **Semantic Paths**: a polymorphic `IPath` hierarchy for files, directories, absolute, relative, and combinations.
- **Semantic Quantities**: a metadata-generated, type-safe quantity system with a unified `IVector0..IVector4` model covering 60+ physical dimensions and 200+ generated types. Optional per-storage-type alias packages let you write `Mass` instead of `Mass<double>`.
- **Semantic Music**: immutable musical value types (`Pitch`, `Interval`, `Scale`, `Chord`, `Key`, `Duration`, `TimeSignature`), with chord-symbol parsing and voicing, plus a harmonic and structural analysis layer.
- **Semantic Color**: a physically-grounded `Color` type with linear-RGB math, perceptual Oklab operations, and built-in WCAG accessibility checks.

## Packages

Each package has its own README with the full API surface, examples, and reference tables. This document is the family overview.

| Package | What it provides | Details |
|---------|------------------|---------|
| [`ktsu.Semantics.Strings`](Semantics.Strings/README.md) | `SemanticString<T>` framework and validation attributes | [README](Semantics.Strings/README.md) |
| [`ktsu.Semantics.Strings.Identifiers`](Semantics.Strings.Identifiers/README.md) | Ready-made identifier types (`Uuid`, `Iban`, `Isbn`, ...) | [README](Semantics.Strings.Identifiers/README.md) |
| [`ktsu.Semantics.Paths`](Semantics.Paths/README.md) | Polymorphic, typed file system path types | [README](Semantics.Paths/README.md) |
| [`ktsu.Semantics.Quantities`](Semantics.Quantities/README.md) | Generated, dimensionally-safe physical quantities | [README](Semantics.Quantities/README.md) |
| [`ktsu.Semantics.Quantities.Double`](Semantics.Quantities.Double/README.md) | `double` storage-type aliases | [README](Semantics.Quantities.Double/README.md) |
| [`ktsu.Semantics.Quantities.Float`](Semantics.Quantities.Float/README.md) | `float` storage-type aliases | [README](Semantics.Quantities.Float/README.md) |
| [`ktsu.Semantics.Quantities.Decimal`](Semantics.Quantities.Decimal/README.md) | `decimal` storage-type aliases | [README](Semantics.Quantities.Decimal/README.md) |
| [`ktsu.Semantics.Music`](Semantics.Music/README.md) | Musical value types and harmonic analysis | [README](Semantics.Music/README.md) |
| [`ktsu.Semantics.Color`](Semantics.Color/README.md) | Linear/perceptual color with accessibility tooling | [README](Semantics.Color/README.md) |

All packages target `net8.0`–`net10.0`. Strings, Identifiers, Paths, Music, and Color additionally target `netstandard2.0`/`netstandard2.1`. Quantities and its alias packages are `net8.0`+ (they require `INumber<T>`).

## Semantic strings

Define a string-shaped domain type, attach validation attributes, and get a compile-time-distinct type that validates on construction.

```csharp
using ktsu.Semantics.Strings;

[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[StartsWith("USER_"), HasNonWhitespaceContent]
public sealed record UserId : SemanticString<UserId> { }

EmailAddress email = EmailAddress.Create("user@example.com");
UserId userId = UserId.Create("USER_12345");

public void SendWelcomeEmail(EmailAddress to, UserId who) { /* ... */ }
// SendWelcomeEmail(userId, email);   // does not compile
```

The [`ktsu.Semantics.Strings.Identifiers`](Semantics.Strings.Identifiers/README.md) package adds ready-made identifier types with real check-digit and structural validation:

```csharp
using ktsu.Semantics.Strings.Identifiers;

Uuid id = Uuid.Create("123E4567-E89B-12D3-A456-426614174000"); // canonicalised to lowercase
Iban iban = Iban.Create("GB82 WEST 1234 5698 7654 32");        // whitespace stripped, mod-97 validated
```

Full detail: [Semantics.Strings README](Semantics.Strings/README.md) and [Semantics.Strings.Identifiers README](Semantics.Strings.Identifiers/README.md).

## Semantic paths

A path is a type that encodes whether it names a file or a directory and whether it is absolute or relative. Compose them with `/`, decompose with typed properties, and hold mixed paths behind `IPath`.

```csharp
using ktsu.Semantics.Paths;

AbsoluteDirectoryPath projectDir = AbsoluteDirectoryPath.Create(@"C:\repos\app");
AbsoluteFilePath source = projectDir / RelativeFilePath.Create(@"src\Program.cs");

FileName name = source.FileName;              // Program.cs
FileExtension ext = source.FileExtension;     // .cs
RelativeFilePath rel = source.AsRelative(projectDir);
```

Full detail: [Semantics.Paths README](Semantics.Paths/README.md).

## Semantic quantities

Every quantity is a vector whose direction-space dimensionality is part of the type. Operators flow from a metadata definition, so `Force · Displacement` yields `Energy` at compile time.

```csharp
using ktsu.Semantics.Quantities;

Mass<double> m = Mass<double>.FromKilogram(2.0);
Speed<double> v = Speed<double>.FromMeterPerSecond(3.0);
MomentumMagnitude<double> p = m * v;          // Mass * Speed -> MomentumMagnitude

Force3D<double> f = new() { X = 3.0, Y = 4.0, Z = 0.0 };
ForceMagnitude<double> mag = f.Magnitude();   // 5.0
```

If a project uses one storage type throughout, reference an alias package and drop the generic argument entirely:

```csharp
using ktsu.Semantics.Quantities;   // with ktsu.Semantics.Quantities.Double referenced

Mass mass = Mass.FromKilogram(10.0);
Speed speed = Speed.FromMeterPerSecond(15.0);
```

Full detail: [Semantics.Quantities README](Semantics.Quantities/README.md). The unified vector model and generator are documented in [`docs/strategy-unified-vector-quantities.md`](docs/strategy-unified-vector-quantities.md) and [`docs/physics-generator.md`](docs/physics-generator.md).

## Semantic music

Immutable, validated musical value types plus an analysis layer that models harmony nested inside structure. Pure logic, no I/O. The pitch convention is MIDI 60 = C4.

```csharp
using ktsu.Semantics.Music;

Progression prog = Progression.Parse("| Dm7 | G7 | Cmaj7 |");
Key key = prog.InferKey()!;                             // C major
IReadOnlyList<string> roman = prog.RomanNumerals(key);  // ii7, V7, Imaj7

Chord cmaj7 = Chord.Parse("Cmaj7");
IReadOnlyList<Pitch> voicing = cmaj7.Voice(octave: 4);  // C4, E4, G4, B4
```

Full detail: [Semantics.Music README](Semantics.Music/README.md).

## Semantic color

The canonical `Color` stores linear RGBA, so mixing, interpolation, and luminance are physically correct. Perceptual work happens in Oklab, and WCAG accessibility tooling is built in.

```csharp
using ktsu.Semantics.Color;

Color text = Color.FromHex("#777777");
Color background = NamedColors.White;

if (text.AccessibilityLevelAgainst(background) < AccessibilityLevel.AA)
{
    text = text.AdjustForContrast(background, AccessibilityLevel.AA);
}

IReadOnlyList<Color> ramp = NamedColors.Red.Gradient(NamedColors.Blue, 5); // Oklab gradient
```

Full detail: [Semantics.Color README](Semantics.Color/README.md).

## Dependency injection

```csharp
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();

public class UserService(ISemanticStringFactory<EmailAddress> emails)
{
    public User CreateUser(string raw) =>
        emails.TryFromString(raw, out EmailAddress? email)
            ? new User(email!)
            : throw new ArgumentException("invalid email");
}
```

## Architecture

The quantity system is metadata-driven. The single source of truth is `Semantics.SourceGenerators/Metadata/dimensions.json` (with `units.json`, `magnitudes.json`, `conversions.json`, `domains.json`, and `logarithmic.json` alongside it), and a Roslyn incremental generator emits the quantity records, unit-conversion factories, cross-dimensional operators, and physical constants. Generated output is committed to `Semantics.Quantities/Generated/` so the project compiles without first running the generator.

The string and path systems share an attribute → strategy → rule → factory validation pipeline. See [`docs/architecture.md`](docs/architecture.md).

## Documentation

- [Complete library guide](docs/complete-library-guide.md) (start here for a feature tour).
- [Architecture (strings/paths/validation)](docs/architecture.md)
- [Architecture (physics, unified vector model)](docs/strategy-unified-vector-quantities.md)
- [Source-generator workflow](docs/physics-generator.md)
- [Physics domains tour](docs/physics-domains-guide.md)
- [Validation attributes reference](docs/validation-reference.md)
- [Advanced usage patterns](docs/advanced-usage.md)

Per-package API detail lives in each package's own README, linked from the [Packages](#packages) table above.

## Contributing

Contributions are welcome. Please open an issue first for major changes so we can discuss the direction. The branch's open work items are tracked as GitHub issues.

## License

MIT. See [`LICENSE.md`](LICENSE.md).
