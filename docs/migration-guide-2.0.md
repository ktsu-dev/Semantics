# Migrating from Semantics 1.x to 2.0

Semantics 2.0 replaces the hand-written physical-quantity library with a
metadata-driven source generator built around the **unified vector model**:
every quantity declares the dimensionality of its direction space in its type
(`Speed` is a magnitude, `Velocity3D` is a 3-vector), all values are stored in
SI base units, and cross-dimensional physics relationships are generated as
operators. The `Semantics.Strings` and `Semantics.Paths` packages are
unaffected apart from additions — this guide is about
`ktsu.Semantics.Quantities`.

For the architecture behind these changes see
`docs/strategy-unified-vector-quantities.md`; for the generator workflow see
`docs/physics-generator.md`.

## Quick checklist

1. Update the namespace: quantities moved from `ktsu.Semantics` to
   `ktsu.Semantics.Quantities` (unit singletons live in
   `ktsu.Semantics.Quantities.Units`). The audio-engineering types
   (`Decibels`, `Gain`, `NormalizedParameter`, …) remain in `ktsu.Semantics`.
2. Rename vector-capable quantities to their explicit form — `Force<double>`
   becomes `ForceMagnitude<double>` (or `Force1D`/`Force2D`/`Force3D` if you
   were tracking sign or direction). The full table is below.
3. Replace uses of the `Units`/`BootstrapUnits` registries and
   `UnitExtensions` with the generated `From{Unit}` factories and the typed
   `In(unit)` method.
4. Move logarithmic quantities (`SoundPressureLevel`, `pH`, …) to the new
   self-contained companion types — they are no longer `PhysicalQuantity`
   subclasses.
5. Audit any code that relies on negative magnitudes or non-SI storage —
   Vector0 quantities now reject negative values, and `Concentration` /
   `NuclearCrossSection` storage moved to true SI base units.

## Namespace changes

| 1.x | 2.0 |
|---|---|
| `ktsu.Semantics` (quantity types) | `ktsu.Semantics.Quantities` |
| `ktsu.Semantics` (unit registry) | `ktsu.Semantics.Quantities.Units` |
| `ktsu.Semantics` (audio engineering: `Decibels`, `Gain`, `Cents`, `Semitones`, `Percent`, `QFactor`, `Ratio` (audio), `NormalizedParameter`, `ParameterTaper`) | unchanged |
| `ktsu.Semantics.Strings`, `ktsu.Semantics.Paths` | unchanged |

## Quantity type renames

Quantities whose 1.x type covered both magnitude and direction are now split
by vector form. The magnitude (V0) form is always non-negative; the V1 form is
a signed scalar; V2/V3/V4 are per-component-signed vectors.

| 1.x type | 2.0 magnitude (V0) | 2.0 signed / vector forms |
|---|---|---|
| `Time` | `Duration` | — |
| `Velocity` | `Speed` | `Velocity1D`/`2D`/`3D`/`4D` |
| `Acceleration` | `AccelerationMagnitude` | `Acceleration1D`/`2D`/`3D`/`4D` |
| `Force` | `ForceMagnitude` | `Force1D`/`2D`/`3D`/`4D` |
| `Momentum` | `MomentumMagnitude` | `Momentum1D`/`2D`/`3D`/`4D` |
| `Torque` | `TorqueMagnitude` | `Torque1D`/`3D` |
| `AngularVelocity` | `AngularSpeed` | `AngularVelocity1D`/`3D` |
| `AngularAcceleration` | `AngularAccelerationMagnitude` | `AngularAcceleration1D`/`3D` |
| `ElectricCurrent` | `CurrentMagnitude` | `Current1D`/`3D` |
| `ElectricCharge` | `ChargeMagnitude` | `Charge` (V1) |
| `ElectricPotential` | `VoltageMagnitude` | `Voltage` (V1) |
| `ElectricField` | `ElectricFieldMagnitude` | `ElectricField1D`/`2D`/`3D` |
| `ElectricResistance` | `Resistance` | — |
| `ElectricCapacitance` | `Capacitance` | — |
| `ImpedanceAC` | `Impedance` (overload of `Resistance`) | — |
| `ThermalExpansion` | `ThermalExpansionCoefficient` | — |

`Magnitude()` on any V≥1 form returns the corresponding V0 type:
`velocity3d.Magnitude()` is a `Speed<T>`.

## Behavioral changes

### Vector0 quantities are non-negative

All V0 factories guard against negative values and throw `ArgumentException`.
This structurally enforces physical floors — Kelvin ≥ 0, non-negative
frequency, non-negative mass. A few quantities (`Wavelength`, `Period`,
`HalfLife`) additionally reject zero.

`V0 - V0` returns the same V0 of the **absolute** difference. If you need a
signed difference, use the V1 form explicitly:

```csharp
// 1.x: could go negative
Force diff = brake - thrust;

// 2.0: magnitude subtraction stays non-negative…
ForceMagnitude<double> diff = brake - thrust;   // |brake − thrust|

// …signed subtraction is the V1 form
Force1D<double> net = thrustSigned - brakeSigned;
```

### Semantic overloads widen implicitly, narrow explicitly

A `Weight<T>` is implicitly a `ForceMagnitude<T>`; converting back requires
`Weight<T>.From(force)` or an explicit cast. Cross-dimensional operators are
defined on the base types and apply to overloads through widening — Ohm's law
works with an `Impedance` because it widens to `Resistance`.

### Storage-unit fixes (silent value changes!)

Three dimensions stored non-SI values in 1.x. If you read `.Value` directly or
serialized raw values, these are the places to audit:

| Quantity | 1.x storage | 2.0 storage |
|---|---|---|
| `Concentration` | mol/L | mol/m³ (`FromMolars(1)` now stores `1000`) |
| `NuclearCrossSection` | barn | m² (`FromBarns(1)` now stores `1e-28`) |
| `MolarMass` via `Dalton` | 1.66×10⁻²⁷ (a per-particle mass — wrong) | 10⁻³ kg/mol (1 Da ≡ 1 g/mol) |

`Temperature.FromFahrenheit` was inverted in early 2.0 previews and computed
the K→F transform; it now correctly maps 32 °F → 273.15 K, and `Rankine` is
supported.

### Factory naming is plural

`From{Unit}` factories use the plural unit form: `Length.FromMeters(…)`,
`Mass.FromKilograms(…)`, `Energy.FromBtus(…)`, `Speed.FromKnots(…)`. Mass
nouns stay invariant (`Frequency.FromHertz`, `Temperature.FromCelsius`), and
"Per" compounds keep the singular form
(`ThermalResistance.FromKelvinPerWatt`,
`Density.FromGramPerCubicCentimeter`).

### Unit conversion is `In(unit)`

`UnitExtensions` and `PhysicalDimensionExtensions` are gone. Conversion out of
SI storage is the dimension-typed `In` method, which fails at compile time for
a unit of the wrong dimension:

```csharp
Length<double> l = Length<double>.FromMeters(10_000);
double km = l.In(Units.Kilometer);       // 10.0
double k  = temperature.In(Units.Celsius);
// l.In(Units.Kilogram) — does not compile
```

`Units` (in `ktsu.Semantics.Quantities.Units`) exposes a generated singleton
per unit; `Unit`, `BootstrapUnit`, and `BootstrapUnits` no longer exist.

## Logarithmic scales are companion types

Decibel and pH scales don't obey linear arithmetic, so they are no longer
`PhysicalQuantity` dimensions. Each is a self-contained
`readonly record struct` that converts to and from its linear counterpart:

| 1.x type | 2.0 companion | Linear counterpart |
|---|---|---|
| `SoundPressureLevel` | `SoundPressureLevel<T>` | `SoundPressure<T>` (Pa, overload of `Pressure`) |
| `SoundIntensityLevel` | `SoundIntensityLevel<T>` | `SoundIntensity<T>` (W/m²) |
| `SoundPowerLevel` | `SoundPowerLevel<T>` | `SoundPower<T>` (W, overload of `Power`) |
| `DirectionalityIndex` | `DirectionalityIndex<T>` | intensity `Ratio<T>` |
| `PH` | `PH<T>` | hydrogen-ion `Concentration<T>` |

```csharp
SoundPressureLevel<double> spl =
    SoundPressureLevel<double>.FromSoundPressure(SoundPressure<double>.FromPascals(0.02)); // 60 dB
SoundPressure<double> p = spl.ToSoundPressure();

PH<double> ph = PH<double>.FromHydrogenConcentration(Concentration<double>.FromMolars(1e-7)); // pH 7
```

Dropped with no replacement: `SoundPressureLevel.AWeighted()` (the 1.x
implementation was a no-op placeholder), `EquivalentLevel`, and the
`PH.CommonValues` novelty list (use `PH<T>.Neutral` for 7.0).

## Physical constants

`PhysicalConstants` is generated from `domains.json`. Domain groupings were
renamed, and the conversion/math helper groups were removed because their
values are now baked into the generated unit factories:

| 1.x group | 2.0 group |
|---|---|
| `Fundamental` | `Fundamental` |
| `Mechanical` | `ClassicalMechanics` |
| `Chemical` | `Chemistry` |
| `Acoustic` | `Acoustics` |
| `Optical` | `Optics` |
| `Nuclear` | `NuclearPhysics` |
| `FluidDynamics` | `FluidMechanics` |
| `Temperature`, `Time`, `Conversion`, `Mathematical` | removed — unit conversions are inside `From{Unit}`/`In(unit)`; use literals for math fractions |

Values are stored as `PreciseNumber`; `PhysicalConstants.Generic.X<T>()`
materializes any constant into your storage type.

## Dimension metadata

`PhysicalDimension` is replaced by the generated `DimensionInfo` record
(name, symbol, dimensional formula, and the quantity types belonging to it),
still exposed per-dimension on the static `PhysicalDimensions` class and per
quantity via `IPhysicalQuantity<T>.Dimension`. Comparing quantities of
different dimensions throws `ArgumentException`; equality across dimensions is
`false`.

## Removed APIs and their replacements

| 1.x API | 2.0 replacement |
|---|---|
| `Units` static registry, `Unit`, `BootstrapUnit(s)` | generated `Units` singletons (`ktsu.Semantics.Quantities.Units`) |
| `UnitExtensions`, `PhysicalDimensionExtensions` | `quantity.In(unit)` |
| `PhysicalDimension` | `DimensionInfo` |
| `MolesPerSecond` unit on `ReactionRate` | none — `ReactionRate` is volumetric (mol/(m³·s)); the 1.x unit was mislabelled |
| `PhysicalConstants.Conversion.*` | conversions are baked into factories; raw factors live in `conversions.json` |
| `SoundPressureLevel.AWeighted()` / `EquivalentLevel` | none (1.x implementations were placeholders) |

## What didn't change

- `Semantics.Strings` and `Semantics.Paths` — same namespaces, same types;
  2.0 only adds validation attributes.
- The audio-engineering types added late in 1.x (`Decibels`, `Gain`,
  `NormalizedParameter`, `ParameterTaper`, …) ship unchanged in
  `ktsu.Semantics`.
- Target frameworks: `net7.0` through `net10.0`.
- Quantities remain generic over the numeric storage type
  (`where T : struct, INumber<T>`).
- `UnitSystem` and `UnitConversionException` keep their names and meaning.
