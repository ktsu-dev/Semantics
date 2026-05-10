# Physics Quantities by Domain

A user-facing tour of the physics quantities currently declared in `Semantics.SourceGenerators/Metadata/dimensions.json` (62 dimensions, plus 83 semantic overloads). The architecture lives in `strategy-unified-vector-quantities.md`; this document just shows what's available and how it groups by physics area.

> The dimension set evolves with the metadata. If a dimension is here but you can't find a generated type, run `dotnet build` and look under `Semantics.Quantities/Generated/Semantics.SourceGenerators/Semantics.SourceGenerators.QuantitiesGenerator/`.

## Vector forms cheat sheet

| Form | Meaning | Generated type pattern |
|---|---|---|
| `IVector0` | non-negative magnitude | `Speed<T>`, `Mass<T>`, `Energy<T>`, … |
| `IVector1` | signed 1D | `Velocity1D<T>`, `Force1D<T>`, `Temperature<T>` |
| `IVector2` | 2D directional | `Velocity2D<T>`, `Force2D<T>`, `Acceleration2D<T>` |
| `IVector3` | 3D directional | `Velocity3D<T>`, `Force3D<T>`, `Position3D<T>` |
| `IVector4` | 4D directional | `Displacement4D<T>` and similar (relativistic / spacetime) |

Forms a dimension *doesn't* declare aren't generated. For example, `Mass` is V0 only; `Energy` is V0 only; `Velocity` declares all of V0..V4.

## Base SI

Seven base dimensions:

- `Length` — V0..V4 with overloads `Width`, `Height`, `Depth`, `Radius`, `Diameter`, `Distance`, `Altitude`, `Wavelength`, `Thickness`, `Perimeter`, `Offset`, `Position3D`, `Translation3D`. Diameter ↔ Radius is a generated relationship.
- `Mass` — V0 with overload `AtomicMass`.
- `Time` — V0 with overloads `Duration`, `Period`, `Lifetime`, `RiseTime`, `FallTime`, `DelayTime`.
- `ElectricCurrent` — V0/V1/V3.
- `Temperature` — V0 with overloads `KineticTemperature`, `ColorTemperature`; V1 base for signed deltas.
- `AmountOfSubstance` — V0.
- `LuminousIntensity` — V0.

```csharp
var height   = Height<double>.FromMeters(1.75);
var atomic   = AtomicMass<double>.FromKilograms(1.66e-27);
var lifetime = Lifetime<double>.FromSeconds(3600);
var temp     = Temperature<double>.FromKelvins(298.15);
```

## Geometry and kinematics

- `Area` — V0 with overloads `CrossSection`, `SurfaceArea`.
- `Volume` — V0 with overload `MolarVolume`.
- `Velocity` / `Acceleration` / `Jerk` / `Snap` — full V0..V4.
- `Frequency` — V0 with overloads `SamplingRate`, `BitRate`, `RefreshRate`, `RotationalSpeed`.

```csharp
var velocity3d = Velocity3D<double>.FromMetersPerSecond(3.0, 4.0, 0.0);
var speed      = velocity3d.Magnitude();          // 5.0 (Speed<double>)
var sampling   = SamplingRate<double>.FromHertz(48_000);
```

## Angular mechanics

- `AngularDisplacement` — V0/V1/V3 with overloads (`Phase`, `RotationAngle`, `Heading`, `Pitch`, `Roll`, `Yaw`).
- `AngularVelocity`, `AngularAcceleration`, `AngularJerk` — V0/V1/V3.
- `Torque` — V0/V1/V3.
- `AngularMomentum` — V0/V1/V3.
- `MomentOfInertia` — V0.

```csharp
var heading = Heading<double>.FromRadians(Math.PI / 2);     // V0 overload of AngularDisplacement
var omega3d = AngularVelocity3D<double>.FromRadianPerSecond(0.0, 0.0, 1.5);
```

## Dynamics

- `Force` — V0..V4 with overloads (`Weight`, `Drag`, `Thrust`, `Lift`, `Friction`, `Tension`, `Compression`, `Normal`, `Shear`).
- `Momentum` — V0..V4.
- `Pressure` — V0 with overloads (`AtmosphericPressure`, `GaugePressure`, `AbsolutePressure`, `VaporPressure`, `OsmoticPressure`, `Stress`).
- `Energy` — V0 with overloads (`KineticEnergy`, `PotentialEnergy`, `ThermalEnergy`, `ChemicalEnergy`, `WorkDone`).
- `Power` — V0 with overload `RadiantPower`.
- `Density` — V0.

```csharp
var weight = Weight<double>.From(ForceMagnitude<double>.FromNewtons(686.0));
var drag   = Drag<double>.FromNewtons(20.0);
var pe     = PotentialEnergy<double>.FromJoules(500.0);
```

## Thermal

- `Entropy` — V0 with overload `MolarEntropy`.
- `SpecificHeat` — V0 with overload `MolarHeatCapacity`.
- `ThermalConductivity` — V0.
- `HeatTransferCoefficient` — V0.
- `ThermalExpansion` — V0.

Heat itself is currently expressed via `Energy` (and its `ThermalEnergy` overload) rather than a separate dimension.

## Electrical and magnetic

- `ElectricCharge` — V0/V1.
- `ElectricPotential` — V0/V1 with overloads (`Voltage`, `EMF`).
- `ElectricField` — V0/V1/V2/V3.
- `ElectricResistance`, `ElectricConductance` (overload `Admittance`), `ElectricCapacitance`, `Inductance` — V0.
- `MagneticFluxDensity` — V0/V3.
- `MagneticFlux` — V0.

```csharp
var v   = Voltage<double>.FromVolts(12.0);
var i   = ElectricCurrent<double>.FromAmperes(2.0);
var r   = v / i;                                  // ElectricResistance<double>
var p   = v * i;                                  // Power<double>
```

## Optical and radiometric

- `LuminousFlux` — V0.
- `Illuminance` — V0 with overload `Luminance`.
- `OpticalPower` — V0.
- `Irradiance` — V0 with overloads `RadiantExitance`, `Radiance`, `RadiantIntensity`.

```csharp
var flux  = LuminousFlux<double>.FromLumens(800.0);
var lux   = flux / Area<double>.FromSquareMeters(4.0);   // Illuminance<double>
```

## Acoustic

- `AcousticImpedance` — V0.
- Most acoustic quantities reuse other dimensions: `Frequency` (with `SamplingRate`/`BitRate` overloads), `Pressure` for sound pressure, `Power` for acoustic power, `Length` for `Wavelength`, etc.

```csharp
var f = Frequency<double>.FromHertz(440.0);              // A4 note
var T = f.Period();                                      // Time<double>
```

## Fluid dynamics

- `KinematicViscosity` — V0 with overload `MomentumDiffusivity`.
- `DynamicViscosity` — V0.
- `VolumetricFlowRate` — V0.
- `MassFlowRate` — V0.
- `SurfaceTension` — V0.

`ReynoldsNumber`, `MachNumber`, `SpecificGravity`, `RefractiveIndex` are V0 overloads of `Dimensionless`.

## Chemical

- `Concentration` — V0.
- `MolarMass` — V0.
- `CatalyticActivity` — V0 with overload `EnzymeActivity`.
- `ReactionRate` — V0.
- `MolarEnergy` — V0 with overloads `ActivationEnergy`, `EnthalpyOfReaction`.

```csharp
var n     = AmountOfSubstance<double>.FromMoles(0.5);
var V     = Volume<double>.FromCubicMeters(0.002);          // 2 L
var M     = n / V;                                         // Concentration<double>
```

## Nuclear and radiation

- `RadioactiveActivity` — V0.
- `AbsorbedDose` — V0.
- `EquivalentDose` — V0.
- `Exposure` — V0.
- `NuclearCrossSection` — V0.

## Dimensionless

- `Dimensionless` — V0 base `Ratio` (with overloads `RefractiveIndex`, `ReynoldsNumber`, `SpecificGravity`, `MachNumber`); V1 base `SignedRatio` for signed quantities such as differential ratios.

## Common patterns

### Magnitude extraction

```csharp
Velocity3D<double> v = Velocity3D<double>.FromMetersPerSecond(3.0, 4.0, 0.0);
Speed<double>      s = v.Magnitude();   // 5.0, always >= 0
```

### Cross product (V3 only)

```csharp
Force3D<double>        F = Force3D<double>.FromNewtons(0.0, 10.0, 0.0);
Displacement3D<double> r = Displacement3D<double>.FromMeters(0.5, 0.0, 0.0);
Torque3D<double>       τ = F.Cross(r);
```

### Dot product

```csharp
Energy<double> work = Force3D<double>.FromNewtons(10.0, 0.0, 0.0)
                          .Dot(Displacement3D<double>.FromMeters(2.0, 0.0, 0.0));   // 20 J
```

### Semantic overload narrowing

```csharp
ForceMagnitude<double> raw    = ForceMagnitude<double>.FromNewtons(686.0);
Weight<double>         weight = Weight<double>.From(raw);    // explicit narrow
ForceMagnitude<double> back   = weight;                       // implicit widen
```

### Physical constants in expressions

```csharp
var R    = PhysicalConstants.Generic.GasConstant<double>();          // J/(mol·K)
var n    = AmountOfSubstance<double>.FromMoles(1.0);
var T    = Temperature<double>.FromKelvins(273.15);
var P    = Pressure<double>.FromPascals(101_325.0);

// PV = nRT  →  V = nRT / P
// (constants flow into operators because everything stores SI base units)
```

## Adding a new dimension or relationship

Edit `Semantics.SourceGenerators/Metadata/dimensions.json` and rebuild. The full schema and a worked example are in `physics-generator.md`.
