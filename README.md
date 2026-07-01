# ktsu.Semantics

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

A .NET library for replacing primitive obsession with strongly-typed, self-validating domain models. Four pillars:

- **Semantic Strings** — type-safe wrappers like `EmailAddress`, `UserId`, `BlogSlug` with attribute-driven validation, plus a batteries-included `Semantics.Strings.Identifiers` package (`Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`).
- **Semantic Paths** — polymorphic `IPath` hierarchy for files, directories, absolute, relative, and combinations.
- **Semantic Quantities** — a metadata-generated, type-safe quantity system with a unified `IVector0..IVector4` model covering 60+ physical dimensions and 200+ generated types. Optional per-storage-type alias packages let you write `Mass` instead of `Mass<double>`.
- **Semantic Music** — immutable musical value types: `PitchClass`, `Pitch`, `Interval`, `Mode`/`Scale`, `Chord`, `Key`, rational `Duration`, and `TimeSignature`, with chord-symbol parsing and voicing, plus a harmonic/structural **analysis** layer (progressions, cadences, key inference, sections, forms).

Targets `net8.0`–`net10.0`. Semantic Strings, Paths, and Music additionally target `netstandard2.0`/`netstandard2.1`; Semantic Quantities is `net8.0`+ (it requires `INumber<T>`).

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

### Batteries-included identifiers

Defining your own types (as above) is the framework's core use, but the `ktsu.Semantics.Strings.Identifiers` package ships ready-made identifier types so common cases need no boilerplate:

```csharp
using ktsu.Semantics.Strings.Identifiers;

Uuid id   = Uuid.Create("123E4567-E89B-12D3-A456-426614174000"); // canonicalised to lowercase
Ulid ulid = Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAV");
CreditCardNumber card = CreditCardNumber.Create("4111 1111 1111 1111"); // separators stripped, Luhn-checked
Isbn isbn = Isbn.Create("978-0-306-40615-7");                     // ISBN-10 or ISBN-13, check-digit validated
Iban iban = Iban.Create("GB82 WEST 1234 5698 7654 32");           // whitespace stripped, mod-97 validated

Uuid.Create("not-a-uuid"); // ❌ throws ArgumentException
```

Each type normalises on creation and validates with real check-digit maths where applicable (`Iban`, `Isbn`, `CreditCardNumber`) or structural/format rules (`Uuid`, `Ulid`, `JwtToken`). Validation is pragmatic with documented limits rather than RFC-exhaustive — for example, `JwtToken` checks the three-segment structure and that the header and payload decode to JSON objects, but does **not** verify the signature.

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

## Semantic quantities

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

// V0 magnitudes — From{Unit} factories use the singular lemma (#49)
var speed    = Speed<double>.FromMeterPerSecond(15.0);
var mass     = Mass<double>.FromKilogram(10.0);
var distance = Distance<double>.FromMeter(5.0);

// V3 directional — object-initializer syntax (X/Y/Z components)
var force3d  = new Force3D<double> { X = 0.0, Y = 0.0, Z = -9.8 };
var disp3d   = new Displacement3D<double> { X = 3.0, Y = 4.0, Z = 0.0 };

// Operators flow from dimensions.json
var work     = ForceMagnitude<double>.FromNewton(10.0) * distance;    // F·d = Energy
var power    = work / Duration<double>.FromSecond(2.0);               // W/t = Power

// Vector ops
var workDot  = force3d.Dot(disp3d);                                    // Energy
var torque   = force3d.Cross(disp3d);                                  // Torque3D
var mag      = disp3d.Magnitude();                                     // Length (always >= 0)

// Construction-time invariants (#50, #51)
// Speed.FromMeterPerSecond(-1.0)        // ArgumentException — V0 must be non-negative
// Wavelength<double>.FromMeter(0.0)     // ArgumentException — strict-positive overload

// Type safety
// var nope = force3d + speed;            // ❌ compiler error
```

Semantic overloads (e.g. `Weight` over `ForceMagnitude`, `Diameter` ↔ `Radius`):

```csharp
var raw    = ForceMagnitude<double>.FromNewton(686.0);
var weight = Weight<double>.From(raw);   // explicit narrow
ForceMagnitude<double> back = weight;    // implicit widen

var radius   = Radius<double>.FromMeter(2.0);
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

### Storage-type alias packages

Every quantity is generic over its storage type (`Mass<double>`, `Speed<float>`, …). If a project uses one storage type throughout, reference a satellite package and drop the generic argument entirely:

```xml
<PackageReference Include="ktsu.Semantics.Quantities.Double" Version="x.y.z" />
```

```csharp
using ktsu.Semantics.Quantities;

// No <double> anywhere — the package injects global-using aliases for every quantity.
Mass mass = Mass.FromKilogram(10.0);
Speed speed = Speed.FromMeterPerSecond(15.0);
Mass total = mass + Mass.FromKilogram(2.0);   // still a Mass<double>, full identity
```

The aliases are real `Mass<double>` (etc.), so they interoperate with the whole API with no conversion. Packages exist for `Double`, `Float`, and `Decimal` — reference exactly one per project to pick the storage type. The alias lists are generated from the quantity catalogue (`scripts/Generate-AliasProps.ps1`) and validated in CI.

The unified vector model and its rationale: [`docs/strategy-unified-vector-quantities.md`](docs/strategy-unified-vector-quantities.md).
A per-domain tour: [`docs/physics-domains-guide.md`](docs/physics-domains-guide.md).
How the source generator turns `dimensions.json` into types: [`docs/physics-generator.md`](docs/physics-generator.md).

## Semantic music

Immutable, validated musical value types under `ktsu.Semantics.Music`. Pure logic, no I/O. The pitch convention is MIDI 60 = C4.

```csharp
using ktsu.Semantics.Music;

// Pitches and intervals
Pitch middleC = Pitch.FromName("C4");          // MIDI 60
Pitch g4      = middleC.Transpose(7);          // a perfect fifth up
Interval fifth = Interval.Between(middleC, g4); // +7 semitones

// Scales and modes (diatonic, jazz, symmetric, pentatonic, blues)
Scale dDorian = Scale.Create(PitchClass.Create(2), Mode.Dorian);
bool hasF = dDorian.Contains(PitchClass.Create(5));   // true

// Chord-symbol parsing → tones and voicings
Chord cmaj7 = Chord.Parse("Cmaj7#11");
IReadOnlyList<int> tones = cmaj7.ChordTones();        // 0,4,7,11,18
IReadOnlyList<Pitch> voicing = Chord.Parse("C/G").Voice(4); // G3, C4, E4, G4

// Inversions and transposition
IReadOnlyList<Pitch> firstInv = Chord.Parse("C").Voice(4, 1);   // E4, G4, C5
Chord dmaj7 = Chord.Parse("Cmaj7").Transpose(2);                // up a tone

// Roman-numeral function within a key — and back again
Key cMajor = Key.Create(PitchClass.Create(0), Mode.Major);
string fn   = cMajor.RomanNumeralOf(Chord.Parse("Dm7"));        // "ii7"
Chord ii    = cMajor.ChordFromRomanNumeral("ii7");              // Dm7

// Rational durations, time signatures (exact for tuplets), and real time
Duration tripletEighth = Duration.Create(1, 12);               // three fill a quarter
Duration barOf7_8 = TimeSignature.Create(7, 8).BarDuration;
Note a4 = Note.Create(Pitch.FromName("A4"), Duration.Half, Velocity.Forte);
double seconds = a4.Seconds(Tempo.Create(120));                // 1.0
double hz = a4.Pitch.FrequencyHz;                              // 440.0
```

The chord parser covers triads, sixths, sevenths (including half-diminished `m7b5` and minor-major `mmaj7`), extensions and altered tensions (`9`/`11`/`13`, `b9`/`#9`/`#11`/`b13`), suspensions, power chords, omit voicings (`no3`/`no5`), and slash bass. Beyond the value types there are score primitives (`Note`, `Rest`, `Velocity`, `Tempo`) that convert rhythm to real time, equal-tempered `Pitch`↔frequency (A440), chord inversions, `Transpose` on `Chord`/`Scale`/`Key`, and roman-numeral parsing as the inverse of `RomanNumeralOf`.

### Analysis: progressions, sections, and forms

Above the single-event types is an analysis layer that models harmony nested inside structure. A `Progression` is an ordered chord sequence with bar-based harmonic rhythm; it computes roman numerals, functional roles, cadences, an inferred key, and chromatic classifications. `Section`s group progressions into labelled units, an `Arrangement` orders them into a piece, and `Form` extracts the structural letter-pattern and names it.

```csharp
using ktsu.Semantics.Music;

// A chord progression with bar-based harmonic rhythm ("|" = barline)
Progression prog = Progression.Parse("Dm7 | G7 | Cmaj7");
Key key = prog.InferKey()!;                                     // C major (quality-weighted fit)

IReadOnlyList<string> roman = prog.RomanNumerals(key);          // "ii7", "V7", "Imaj7"
IReadOnlyList<HarmonicFunction> fns = prog.Functions(key);      // Predominant, Dominant, Tonic
IReadOnlyList<CadenceInstance> cadences = prog.Cadences(key);   // Authentic (V→I) at the resolution

// Chromatic analysis: secondary dominants, borrowed chords, Neapolitan
IReadOnlyList<ChromaticAnalysis> chromatic =
    Progression.Parse("C | D7 | G7 | C").ChromaticChords(key);  // D7 → "V/V" (SecondaryDominant)

// Structure: sections → arrangement → form
Section verse  = Section.Create(SectionType.Verse,  Progression.Parse("C | Am | F | G"));
Section bridge = Section.Create(SectionType.Bridge, Progression.Parse("F | G | Em | Am"));
Arrangement song = Arrangement.Create(key, [verse, verse, bridge, verse]);
Form form = song.Form;                                          // Pattern "AABA", Name ThirtyTwoBarAABA
```

Cadences are classified by scale-degree motion (authentic, plagal, half, deceptive); key inference is quality-weighted so it distinguishes a key from its relative/parallel neighbours; `Form` recognizes named forms including AABA, ternary, binary, rondo, strophic, and the 12-bar blues progression template.

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
- **SEM004** — `availableUnits` references a unit not declared in `units.json`.

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
