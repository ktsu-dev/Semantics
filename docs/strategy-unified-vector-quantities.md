# Strategy: Unified Vector Representation for Quantities

## Motivation

The current quantity system draws a hard line between "scalar" quantities and "vector" quantities, treating them as fundamentally different types. This creates several problems:

1. **Redundant type hierarchies** - Scalar quantities and vector quantities share the same dimensional relationships but have separate code paths for generation, operators, and conversions.
2. **Ambiguous "scalar" semantics** - The word "scalar" conflates two distinct concepts: a magnitude-only value (always non-negative, like speed) and a signed one-dimensional value (like velocity along a single axis).
3. **No clean magnitude extraction** - There's no unified way to go from a directional quantity to its magnitude. The relationship between `Velocity` (scalar) and `VelocityVector3` is implicit, not structural.
4. **Dimensional inconsistency** - A `Dimensionless` scalar and a magnitude-only quantity are represented identically, even though they have different physical meaning.

## Core Idea

Represent all quantities as vectors, eliminating the scalar/vector distinction. The dimensionality of the direction space becomes a property of the quantity rather than a categorical difference.

| Type | Direction Dimensions | Value Space | Sign | Examples |
|------|---------------------|-------------|------|----------|
| **Vector0** | 0 (none) | Non-negative magnitude | Unsigned | Speed, Distance, Mass, Energy, Area, Volume |
| **Vector1** | 1 | Signed scalar | Signed | Displacement (1D), Velocity (1D), Force (1D), Temperature delta, Electric charge |
| **VectorN** | N (2, 3, 4...) | N-component | Per-component signed | Position, Velocity, Acceleration, Force in 2D/3D |

### Key Insight

This framing makes the distinction between **magnitude-only** and **signed dimensionless** quantities explicit at the type level:

- `Speed<T>` is a `Vector0` - a magnitude with no directional component, always >= 0
- `Velocity1D<T>` is a `Vector1` - a signed value along one axis, can be negative
- `Velocity3D<T>` is a `Vector3` - three signed components

The magnitude of any VectorN is always a Vector0 of the same physical dimension. This relationship is structural, not coincidental.

## Type Hierarchy

```
IQuantity<T>                          // Root: all quantities
├── IVector0<TSelf, T>                // Magnitude only (unsigned, >= 0)
│   ├── Speed<T>
│   ├── Distance<T>
│   ├── Mass<T>
│   ├── Energy<T>
│   ├── Area<T>
│   └── ...
├── IVector1<TSelf, T>                // Signed 1D quantity
│   ├── Displacement1D<T>
│   ├── Velocity1D<T>
│   ├── Force1D<T>
│   ├── Temperature<T>               // Signed because deltas can be negative
│   └── ...
├── IVector2<TSelf, T>                // 2D directional
│   ├── Position2D<T>
│   ├── Velocity2D<T>
│   └── ...
├── IVector3<TSelf, T>                // 3D directional
│   ├── Position3D<T>
│   ├── Velocity3D<T>
│   └── ...
└── IVector4<TSelf, T>                // 4D directional
    └── ...
```

## Semantic Relationships

### Magnitude Extraction

Every VectorN (N >= 1) has a corresponding Vector0 that represents its magnitude:

```csharp
Velocity3D<double> v = Velocity3D.Create(3.0, 4.0, 0.0);
Speed<double> s = v.Magnitude();  // returns Speed with value 5.0

Force1D<double> f = Force1D.FromNewtons(-9.8);
ForceMagnitude<double> m = f.Magnitude();  // returns 9.8 (always non-negative)
```

This is a fundamental structural relationship, not a convenience method. The magnitude of a vector quantity is always a well-defined quantity of the same physical dimension.

### Vector0: Non-negative Magnitude

Vector0 quantities enforce non-negativity as a type-level invariant:

```csharp
Speed<double> s = Speed.FromMetersPerSecond(5.0);   // OK
Speed<double> s = Speed.FromMetersPerSecond(-1.0);   // throws: magnitude cannot be negative

// Arithmetic respects the invariant
Speed<double> a = Speed.FromMetersPerSecond(3.0);
Speed<double> b = Speed.FromMetersPerSecond(5.0);
Speed<double> sum = a + b;    // 8.0 - OK
// a - b would need to return absolute value or throw, since magnitudes can't go negative
```

**Design decision needed**: What happens when subtracting two Vector0 values would produce a negative result?

Options:
1. Return the absolute value of the difference (magnitude of difference)
2. Throw an exception
3. Don't define subtraction on Vector0 at all (force users to convert to Vector1 first)
4. Return a Vector1 (the signed difference) and let the user extract magnitude if needed

### Vector1: Signed One-Dimensional

Vector1 quantities are signed values along a single axis:

```csharp
Velocity1D<double> v = Velocity1D.FromMetersPerSecond(-3.0);  // negative = moving backward
Speed<double> s = v.Magnitude();  // 3.0, always non-negative

// Negation is natural
Velocity1D<double> reversed = -v;  // +3.0
```

### VectorN: Multi-dimensional Direction

VectorN quantities (N >= 2) have N components, each signed:

```csharp
Velocity3D<double> v = Velocity3D.Create(3.0, 4.0, 0.0);
Speed<double> s = v.Magnitude();      // 5.0
Velocity3D<double> n = v.Normalize(); // (0.6, 0.8, 0.0)
```

## Dimensional Relationships Across Vector Types

Physical relationships (integrals and derivatives) must be defined per-vector-dimensionality. The dimension of the direction space is preserved through operations:

```
Vector0 * Vector0 → Vector0     (Speed * Time = Distance)
Vector0 * Vector1 → Vector1     (Speed * Direction1D = Velocity1D)
Vector1 / Vector0 → Vector1     (Velocity1D / Time = Acceleration1D)
Vector3 / Vector0 → Vector3     (Velocity3D / Time = Acceleration3D)
Vector3 * Vector0 → Vector3     (Force3D * Distance = ... depends on context)

// Dot product: VectorN . VectorN → Vector0
Vector3 . Vector3 → Vector0     (Force3D . Displacement3D = Work/Energy as magnitude)

// Cross product: Vector3 x Vector3 → Vector3
Vector3 x Vector3 → Vector3     (Velocity3D x MagneticField3D = Force3D direction)
```

### Rules for Operator Dimensionality

| Operation | Left | Right | Result | Notes |
|-----------|------|-------|--------|-------|
| `*` (scalar multiply) | Vector0 | Vector0 | Vector0 | Magnitude times magnitude |
| `*` (scale vector) | VectorN | Vector0 | VectorN | Scale a vector by a magnitude |
| `*` (scale vector) | Vector0 | VectorN | VectorN | Commutative |
| `/` (scalar divide) | VectorN | Vector0 | VectorN | Divide vector by magnitude |
| `/` (scalar divide) | Vector0 | Vector0 | Vector0 | Magnitude ratio |
| `Dot` | VectorN | VectorN | Vector0 | Dot product yields magnitude |
| `Cross` | Vector3 | Vector3 | Vector3 | Cross product yields vector |
| `+` / `-` | VectorN | VectorN | VectorN | Same dimension required |
| `Magnitude()` | VectorN | - | Vector0 | Extract magnitude |

## Impact on dimensions.json

The current `"scalar": true/false` and `"vectors": true/false` flags would be replaced with a single field describing which vector forms the quantity supports:

### Current Format
```json
{
  "name": "Velocity",
  "scalar": true,
  "vectors": true,
  ...
}
```

### Proposed Format
```json
{
  "name": "Velocity",
  "vectorForms": [0, 1, 2, 3, 4],
  ...
}
```

Or, to make the naming explicit:

```json
{
  "name": "Velocity",
  "vectorForms": {
    "vector0": { "name": "Speed" },
    "vector1": { "name": "Velocity1D" },
    "vector2": { "name": "Velocity2D" },
    "vector3": { "name": "Velocity3D" },
    "vector4": { "name": "Velocity4D" }
  },
  ...
}
```

This allows:
- Quantities that only exist as magnitudes (e.g., Mass: only vector0)
- Quantities with distinct names for each form (Speed vs Velocity)
- Quantities where some dimensions don't make physical sense (e.g., Temperature might only have vector0 and vector1, not vector3)

## Impact on Source Generator

The `QuantitiesGenerator` currently generates two separate categories:

1. Scalar types: `public record Velocity<T> : SemanticQuantity<Velocity<T>, T>`
2. Vector types: `public record VelocityVector2<T> : IVector2<VelocityVector2<T>, T>`

Under the unified model, the generator would produce:

1. `Speed<T>` implementing `IVector0<Speed<T>, T>` - the magnitude form
2. `Velocity1D<T>` implementing `IVector1<Velocity1D<T>, T>` - signed 1D form
3. `Velocity2D<T>` implementing `IVector2<Velocity2D<T>, T>` - 2D form
4. `Velocity3D<T>` implementing `IVector3<Velocity3D<T>, T>` - 3D form
5. `Velocity4D<T>` implementing `IVector4<Velocity4D<T>, T>` - 4D form

Each generated type includes:
- A `Magnitude()` method returning the corresponding Vector0 type (for N >= 1)
- Operators that respect the vector dimensionality rules above
- Factory methods from the appropriate units

### IVector0 Interface

A new `IVector0<TSelf, T>` interface would be needed:

```csharp
public interface IVector0<TSelf, T>
    where TSelf : IVector0<TSelf, T>
    where T : struct, INumber<T>
{
    /// <summary>Gets the magnitude value (always non-negative).</summary>
    T Value { get; }

    /// <summary>Gets a quantity with value zero.</summary>
    static abstract TSelf Zero { get; }

    // Arithmetic
    static abstract TSelf operator +(TSelf left, TSelf right);
    static abstract TSelf operator *(TSelf quantity, T scalar);
    static abstract TSelf operator *(T scalar, TSelf quantity);
    static abstract TSelf operator /(TSelf quantity, T scalar);
}
```

### IVector1 Interface

A new `IVector1<TSelf, T>` interface:

```csharp
public interface IVector1<TSelf, T>
    where TSelf : IVector1<TSelf, T>
    where T : struct, INumber<T>
{
    /// <summary>Gets the signed value along the single axis.</summary>
    T Value { get; }

    /// <summary>Gets a quantity with value zero.</summary>
    static abstract TSelf Zero { get; }

    // Arithmetic
    static abstract TSelf operator +(TSelf left, TSelf right);
    static abstract TSelf operator -(TSelf left, TSelf right);
    static abstract TSelf operator *(TSelf quantity, T scalar);
    static abstract TSelf operator *(T scalar, TSelf quantity);
    static abstract TSelf operator /(TSelf quantity, T scalar);
    static abstract TSelf operator -(TSelf value);  // Negation
}
```

## Naming Conventions

Where vector forms have established distinct names in physics, use them:

| Physical Dimension | Vector0 (Magnitude) | Vector1 (1D Signed) | VectorN (Multi-D) |
|-------------------|---------------------|---------------------|--------------------|
| Length/Displacement | Distance | Displacement1D | Displacement2D/3D |
| Velocity | Speed | Velocity1D | Velocity2D/3D |
| Force | ForceMagnitude | Force1D | Force2D/3D |
| Acceleration | AccelerationMagnitude | Acceleration1D | Acceleration2D/3D |
| Momentum | MomentumMagnitude | Momentum1D | Momentum2D/3D |
| Mass | Mass | - | - |
| Time | Time | - | - |
| Energy | Energy | - | - |
| Temperature | Temperature | TemperatureDelta | - |
| Electric Charge | ChargeMagnitude | Charge | - |

Where no established distinct name exists, use the `{Name}Magnitude` / `{Name}1D` / `{Name}ND` convention.

## Quantities That Don't Span All Forms

Not every quantity makes sense at every vector dimensionality:

- **Mass**: Only Vector0. Mass has no direction.
- **Time**: Only Vector0. Time has no direction.
- **Temperature**: Vector0 (absolute) and Vector1 (delta). Not multi-dimensional.
- **Energy**: Only Vector0. Energy is a scalar in classical mechanics.
- **Velocity**: All forms. Speed (V0), Velocity1D (V1), Velocity2D/3D (VN).
- **Force**: Vector0 (magnitude), Vector1, Vector2, Vector3. Force is fundamentally directional.

The `vectorForms` metadata in dimensions.json controls which forms the source generator produces for each quantity.

## Semantic Overloads

### The Problem

A single physical dimension at a single vector form often has multiple meaningful names depending on context. These aren't different physical quantities - they share the same dimension, units, and arithmetic - but they carry distinct semantic intent.

**Length dimension, Vector0 (magnitude):**

- Length, Width, Height, Depth, Thickness
- Radius, Diameter
- Distance, Range
- Altitude, Elevation
- Wavelength, Stride, Span
- Perimeter, Circumference

**Length dimension, Vector1 (signed 1D):**

- Displacement, Offset, Shift

**Length dimension, Vector3 (3D):**

- Position, Location
- Translation, Displacement
- Normal (direction, unit vector context)

**Velocity dimension, Vector0:**

- Speed, FlowRate (contextual)

**Velocity dimension, Vector3:**

- Velocity, WindVelocity, CurrentVelocity

**Force dimension, Vector0:**

- Weight, Thrust, Drag, Lift, Tension

**Force dimension, Vector3:**

- Force, Weight (directional), Thrust (directional)

These semantic overloads exist across nearly every physical dimension. Ignoring them produces code where every length is just `Length<T>`, losing domain context. But treating them as fully independent types creates a combinatorial explosion of operator definitions.

### Design: Semantic Overloads as Typed Aliases With a Shared Base

Each physical dimension + vector form combination has one **base quantity type** (the canonical type). Semantic overloads are distinct types that share the same physical behavior but carry their own name and optional validation.

```
Length<T>        (base - Vector0 of Length dimension)
├── Width<T>     (semantic overload)
├── Height<T>    (semantic overload)
├── Depth<T>     (semantic overload)
├── Radius<T>    (semantic overload)
├── Distance<T>  (semantic overload)
├── Altitude<T>  (semantic overload)
└── Wavelength<T>(semantic overload)
```

#### Key Rules

**1. Overloads implicitly convert to their base type (widening).**

An overload IS-A base. A `Width<T>` is always a valid `Length<T>`.

```csharp
Width<double> w = Width.FromMeters(5.0);
Length<double> l = w;  // implicit - every Width is a Length
```

**2. Base types require explicit conversion to an overload (narrowing).**

Not every Length is a Width. The user asserts the semantic meaning.

```csharp
Length<double> l = Length.FromMeters(5.0);
Width<double> w = l.As<Width<double>>();   // explicit - asserting "this length is a width"
// or:
Width<double> w = Width.From(l);           // factory-style explicit conversion
```

**3. Operations between overloads of the same base return the base type.**

When you mix different semantic names of the same dimension, the result loses the specific semantic and falls back to the base.

```csharp
Width<double> w = Width.FromMeters(3.0);
Height<double> h = Height.FromMeters(4.0);
Length<double> sum = w + h;  // Width + Height = Length (base type)

Width<double> w2 = Width.FromMeters(2.0);
Width<double> sum2 = w + w2;  // Width + Width = Width (same overload preserved)
```

**4. Operations within the same overload preserve the overload type.**

```csharp
Width<double> w1 = Width.FromMeters(3.0);
Width<double> w2 = Width.FromMeters(2.0);
Width<double> sum = w1 + w2;    // Width + Width = Width
Width<double> scaled = w1 * 2;  // Width * scalar = Width
```

**5. Cross-dimensional operations use the base types for their results.**

```csharp
Width<double> w = Width.FromMeters(3.0);
Height<double> h = Height.FromMeters(4.0);
Area<double> a = w * h;  // Width * Height = Area (dimensional relationship uses bases)
```

**6. Overloads can add validation beyond the base.**

```csharp
// Radius enforces non-negative (already guaranteed by Vector0) but Diameter
// has a structural relationship:
Diameter<double> d = Diameter.FromMeters(10.0);
Radius<double> r = d.ToRadius();  // r.Value == 5.0 (semantic conversion with logic)
```

### Implementation Approach

Semantic overloads are generated as thin wrapper records around the base type:

```csharp
// Generated base type
public record Length<T> : IVector0<Length<T>, T>
    where T : struct, INumber<T>
{
    public T Value { get; init; }
    // ... full operator suite, factory methods, etc.
}

// Generated semantic overload
public record Width<T> : IVector0<Width<T>, T>
    where T : struct, INumber<T>
{
    public T Value { get; init; }

    // Implicit widening to base
    public static implicit operator Length<T>(Width<T> w) => Length<T>.Create(w.Value);

    // Explicit narrowing from base
    public static explicit operator Width<T>(Length<T> l) => new() { Value = l.Value };

    // Same operators as base, but returning Width<T> for same-type operations
    public static Width<T> operator +(Width<T> left, Width<T> right)
        => new() { Value = left.Value + right.Value };

    // Mixed-overload operations return the base type (handled via implicit conversion)
}
```

### Semantic Overloads in dimensions.json

Semantic overloads are defined within the `vectorForms` structure:

```json
{
  "name": "Length",
  "symbol": "L",
  "dimensionalFormula": { "length": 1 },
  "quantities": {
    "vector0": {
      "base": "Length",
      "overloads": [
        {
          "name": "Width",
          "description": "Horizontal extent of an object or space."
        },
        {
          "name": "Height",
          "description": "Vertical extent of an object or space."
        },
        {
          "name": "Depth",
          "description": "Extent into a surface or volume, perpendicular to its face."
        },
        {
          "name": "Radius",
          "description": "Distance from the center to the edge of a circle or sphere."
        },
        {
          "name": "Diameter",
          "description": "Distance across a circle or sphere through its center.",
          "relationships": {
            "toRadius": "Value / 2",
            "fromRadius": "Value * 2"
          }
        },
        {
          "name": "Distance",
          "description": "Separation between two points in space."
        },
        {
          "name": "Altitude",
          "description": "Height above a reference level, typically sea level."
        },
        {
          "name": "Wavelength",
          "description": "Spatial period of a periodic wave."
        },
        {
          "name": "Thickness",
          "description": "Extent of an object through its thinnest dimension."
        },
        {
          "name": "Perimeter",
          "description": "Total length of the boundary of a 2D shape."
        },
        {
          "name": "Circumference",
          "description": "Perimeter of a circle.",
          "relationships": {
            "toRadius": "Value / (2 * pi)",
            "toDiameter": "Value / pi"
          }
        }
      ]
    },
    "vector1": {
      "base": "Displacement1D",
      "overloads": [
        {
          "name": "Offset",
          "description": "Signed distance from a reference point along one axis."
        }
      ]
    },
    "vector3": {
      "base": "Position3D",
      "overloads": [
        {
          "name": "Displacement3D",
          "description": "Change in position in 3D space."
        },
        {
          "name": "Translation3D",
          "description": "Movement applied to an object in 3D space."
        }
      ]
    }
  }
}
```

### Another Example: Force

```json
{
  "name": "Force",
  "symbol": "M L T⁻²",
  "dimensionalFormula": { "mass": 1, "length": 1, "time": -2 },
  "quantities": {
    "vector0": {
      "base": "ForceMagnitude",
      "overloads": [
        { "name": "Weight", "description": "Gravitational force on an object." },
        { "name": "Thrust", "description": "Propulsive force." },
        { "name": "Drag", "description": "Resistive force opposing motion through a medium." },
        { "name": "Lift", "description": "Force perpendicular to flow direction." },
        { "name": "Tension", "description": "Pulling force along a rope, cable, or similar." }
      ]
    },
    "vector1": {
      "base": "Force1D"
    },
    "vector3": {
      "base": "Force3D",
      "overloads": [
        { "name": "WeightVector", "description": "Gravitational force vector (typically -Y or -Z)." }
      ]
    }
  }
}
```

### Source Generator Impact

The source generator treats overloads as lightweight types:

1. **Base types** get the full operator suite, factory methods, unit conversions, and cross-dimensional operators (integrals/derivatives).
2. **Overload types** get:
   - Same-type arithmetic operators (returning the overload type)
   - Implicit conversion to the base type
   - Explicit conversion from the base type
   - Factory methods for the shared units
   - Any declared `relationships` as conversion methods
3. **Cross-dimensional operators** are only generated between base types. Overloads participate via implicit conversion to their base.

This keeps the generated code manageable: if a dimension has 10 overloads and participates in 5 cross-dimensional relationships, we generate 5 operator sets (not 50).

### Comprehensive Semantic Overload Catalog

Below is a non-exhaustive list of overloads to illustrate the breadth:

| Dimension | Vector Form | Base | Overloads |
|-----------|-------------|------|-----------|
| Length | V0 | Length | Width, Height, Depth, Radius, Diameter, Distance, Altitude, Elevation, Wavelength, Thickness, Perimeter, Circumference, Stride, Span, Range, Focal Length |
| Length | V1 | Displacement1D | Offset, Shift |
| Length | V3 | Position3D | Displacement3D, Translation3D, Location3D |
| Velocity | V0 | Speed | FlowSpeed, WindSpeed, GroundSpeed, Airspeed |
| Velocity | V1 | Velocity1D | - |
| Velocity | V3 | Velocity3D | WindVelocity3D, CurrentVelocity3D |
| Force | V0 | ForceMagnitude | Weight, Thrust, Drag, Lift, Tension, NormalForce, Friction, SpringForce |
| Force | V3 | Force3D | WeightVector, ThrustVector |
| Acceleration | V0 | AccelerationMagnitude | GravitationalAcceleration |
| Acceleration | V3 | Acceleration3D | GravitationalField3D |
| Pressure | V0 | Pressure | Stress, AtmosphericPressure, GaugePressure, OsmoticPressure |
| Energy | V0 | Energy | Work, Heat, KineticEnergy, PotentialEnergy, ThermalEnergy, ElectricalEnergy |
| Power | V0 | Power | Wattage, Luminosity (radiant), HeatFlowRate |
| Mass | V0 | Mass | AtomicMass, MolarMass (different units but same dimension) |
| Time | V0 | Duration | Period, HalfLife, TimeConstant, Latency |
| Temperature | V0 | Temperature | - |
| Temperature | V1 | TemperatureDelta | TemperatureRise, TemperatureDrop |
| Angle | V0 | AngleMagnitude | FieldOfView, ApertureAngle |
| Angle | V1 | Angle | Rotation, Phase, Bearing, Heading |
| Area | V0 | Area | SurfaceArea, CrossSectionalArea, Footprint |
| Volume | V0 | Volume | Capacity, Displacement (engine) |
| Frequency | V0 | Frequency | SamplingRate, ClockSpeed, Bandwidth |
| ElectricPotential | V0 | Voltage | EMF, VoltageDrop, BackEMF |

### Open Questions (Semantic Overloads)

1. **Overload depth**: Should overloads be allowed to have their own overloads? (e.g., `Weight` as an overload of `ForceMagnitude`, then `BodyWeight` as an overload of `Weight`). Probably not - keep it flat.

2. **Cross-overload operations**: When `Width + Height` returns `Length`, should there be a way to annotate that the result could be semantically promoted? (e.g., `Perimeter` = `Width + Width + Height + Height`). Probably out of scope - that's application logic.

3. **Unit scoping**: Should overloads restrict available units? (e.g., `Wavelength` might typically use nanometers/micrometers rather than miles). This could be a display hint rather than a hard restriction.

4. **Overload-specific validation**: Beyond the shared base validation, can overloads add constraints? (e.g., `Radius >= 0` is already guaranteed by Vector0, but `Altitude` might have a domain-specific minimum).

## Migration Strategy

### Phase 1: Add IVector0 and IVector1 interfaces
- Define `IVector0<TSelf, T>` alongside existing `IVector2`, `IVector3`, `IVector4`
- Define `IVector1<TSelf, T>`
- Both interfaces coexist with the current `SemanticQuantity` base

### Phase 2: Update dimensions.json schema
- Replace `"scalar"` / `"vectors"` booleans with `"vectorForms"` object
- Populate with appropriate names for each quantity at each dimensionality

### Phase 3: Update source generator
- Generate Vector0 types where configured (replacing current scalar generation)
- Generate Vector1 types where configured (new)
- Keep Vector2/3/4 generation as-is but implement `Magnitude()` returning the Vector0 type
- Generate cross-dimensionality operators (VectorN * Vector0, etc.)

### Phase 4: Update SemanticQuantity base
- Evaluate whether `SemanticQuantity<TSelf, TStorage>` becomes the base for Vector0/Vector1
- Or whether the IVector interfaces stand alone with generated record types

## Open Questions

1. **Subtraction on Vector0**: What should `Distance(5) - Distance(8)` return? Options: absolute value, throw, return Vector1, or don't define it.

2. **Implicit conversions**: Should `Speed<T>` implicitly convert to `Velocity1D<T>` (promoting magnitude to positive signed value)? Should `Velocity1D<T>` implicitly yield `Speed<T>` via `.Magnitude()`?

3. **Naming**: For quantities with no established magnitude name (e.g., Jerk), is `JerkMagnitude` acceptable or is there a better convention?

4. **Base class vs interface-only**: Should Vector0/Vector1 types inherit from `SemanticQuantity<TSelf, TStorage>` (giving them the existing arithmetic infrastructure) or should they be standalone records implementing only the IVector interfaces?

5. **Dimensionless quantities**: Is `Dimensionless` a Vector0 (always positive ratio) or do we need both `Dimensionless` (Vector0) and `SignedDimensionless` (Vector1)?

6. **Angular quantities**: Are angles Vector0 (magnitude) or Vector1 (signed, since clockwise vs counterclockwise matters)?

---

*This document defines the strategic direction. Implementation details for each phase will be tracked in separate design documents as work proceeds.*
