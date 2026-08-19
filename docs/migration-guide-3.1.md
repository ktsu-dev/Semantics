# Migrating from Semantics 3.0 to 3.1

Semantics 3.1 removes the last two third-party NuGet dependencies from the
shipping packages. `ktsu.Semantics.Strings` now has no NuGet dependencies at all
on .NET 8 and later, and `ktsu.Semantics.Quantities` has none on any target.

Both removals are breaking. Paths, Music, Color, and the storage-type alias
packages are unaffected except through their dependency on Strings.

## Quick checklist

1. If you serialize semantic strings or paths to JSON, register a converter on
   your `JsonSerializerOptions` — Semantics no longer registers one for you.
   **Do this before upgrading**; without it, serialization silently changes
   shape and deserialization of existing files throws.
2. Replace `PhysicalConstants.<Domain>.<Name>` field reads with the generic
   accessor `PhysicalConstants.<Domain>.<Name><T>()`.

## 1. `ktsu.RoundTripStringJsonConverter` is no longer a dependency

`SemanticString<TDerived>` previously carried
`[JsonConverter(typeof(RoundTripStringJsonConverterFactory))]`, which made every
semantic string and every path type serialize as a plain JSON string with no
configuration. That attribute is gone, and so is the package dependency.

Registering a converter is now the consumer's decision. This matters more than a
typical attribute removal, because the fallback is not a compile error:

```csharp
// A semantic string is an IEnumerable<char>, so with no converter registered
// System.Text.Json treats it as a collection:
JsonSerializer.Serialize(cardName);   // ["P","i","k","a","c","h","u"]

// ...and reading back a value written by 3.0 throws:
JsonSerializer.Deserialize<CardName>("\"Pikachu\"");   // JsonException
```

So an upgrade without the change below **corrupts newly written JSON and fails
to read existing JSON**. There is no deprecation window that can warn you at
compile time; add the converter first.

### What to do

Keep using the same converter — just register it yourself:

```csharp
// dotnet add package ktsu.RoundTripStringJsonConverter
using ktsu.RoundTripStringJsonConverter;

private static readonly JsonSerializerOptions JsonOptions = new()
{
    Converters = { new RoundTripStringJsonConverterFactory() },
};

string json = JsonSerializer.Serialize(card, JsonOptions);
Card restored = JsonSerializer.Deserialize<Card>(json, JsonOptions)!;
```

This produces byte-identical output to 3.0, so existing files stay readable.

Pass those options at every call site that touches a semantic string, including
nested ones — a converter registered on the options applies to the whole object
graph, but options are not global. If your app has a single serialization helper,
one registration is enough; if it calls `JsonSerializer` ad hoc, each call needs
the options.

Any converter factory that writes `ToString()` and reads back through a static
`Create`/`FromString`/`Parse` accepting a `string` will do; the ktsu package is
just a ready-made one.

### If you already registered it

Several consumers already add `RoundTripStringJsonConverterFactory` to their own
`JsonSerializerOptions`. Those are unaffected — an explicitly registered
converter took precedence over the attribute in 3.0 and behaves identically now.
Keep your own `PackageReference` to `ktsu.RoundTripStringJsonConverter`, since it
no longer arrives transitively through Semantics.

## 2. `PhysicalConstants` domain fields are now generic accessors

`ktsu.PreciseNumber` has been removed from `ktsu.Semantics.Quantities`. The
domain-grouped constants were typed `PreciseNumber`, so they have become generic
methods returning the numeric type you ask for:

```csharp
// Before
PreciseNumber c = PhysicalConstants.Fundamental.SpeedOfLight;
double cd = PhysicalConstants.Fundamental.SpeedOfLight.To<double>();

// After
double cd = PhysicalConstants.Fundamental.SpeedOfLight<double>();
```

`PhysicalConstants.Generic.<Name><T>()` is unchanged, so code already using the
generic accessors needs no edits.

Unlike the JSON change, this one is a compile error at every call site, so the
compiler will find them for you.

### Accuracy changes

Constants are now parsed from their metadata literal directly into `T` rather
than being stored in an intermediate significand/exponent representation and
converted per call. The intermediate rounded twice and mis-handled the long
CODATA literals, so several values change — all of them toward the correct
value:

| Accessor | 3.0 | 3.1 |
|---|---|---|
| `DegreesPerRadian<float>()` | `NaN` | `57.29578` |
| `DegreesPerRadian<decimal>()` | throws `OverflowException` | `57.295779513082320876798154814` |
| `DegreesPerRadian<double>()` | `57.295779513082316` | `57.29577951308232` |
| `PlanckConstant<float>()` | `6.629563E-34` | `6.62607E-34` |

The same applies to the other long-expansion constants (`Pi`, `TwoPi`,
`RadiansPerDegree`, `Ln2`). If you compensated for any of these — for example by
special-casing `float` or catching `OverflowException` around a `decimal`
conversion — remove the workaround.

Values that were already correct are unchanged, so `double` results are stable
apart from the last-ulp corrections shown above.
