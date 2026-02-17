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

## Cross-Dimensional Relationships

This is the core of the type system: when you multiply or divide quantities of different physical dimensions, the result must be the correct semantic type at the correct vector form.

### Three Kinds of Cross-Dimensional Operations

**1. Scalar operations** (`*`, `/`) - one or both operands are V0:

```
V0 * V0 → V0       Speed * Duration       = Length
V0 * VN → VN       Duration * Acceleration3D = Velocity3D
VN * V0 → VN       Velocity3D * Duration   = Displacement3D
VN / V0 → VN       Displacement3D / Duration = Velocity3D
V0 / V0 → V0       Length / Duration       = Speed
```

**2. Dot product** (`.Dot()`) - two VN operands of the same N, result is always V0:

```
VN · VN → V0       Force3D · Displacement3D = Energy
                    Velocity3D · Velocity3D  = Speed² (not a named type)
```

**3. Cross product** (`.Cross()`) - two V3 operands, result is V3:

```
V3 × V3 → V3       Displacement3D × Force3D = Torque3D
                    AngularVelocity3D × Displacement3D = Velocity3D
```

### Vector Form Propagation Rules

When the source generator encounters a dimensional relationship like `Velocity * Time = Displacement`, it must generate operators for every valid combination of vector forms across both operands.

**Rule: The result vector form equals the highest vector form among the operands, provided the result dimension supports that form.**

| Self form | Other form | Result form | Condition | Example |
|-----------|-----------|-------------|-----------|---------|
| V0 | V0 | V0 | Always valid | `Speed * Duration = Length` |
| V1 | V0 | V1 | Result has V1 | `Velocity1D * Duration = Displacement1D` |
| V0 | V1 | V1 | Result has V1 | `Duration * Velocity1D = Displacement1D` |
| V2 | V0 | V2 | Result has V2 | `Velocity2D * Duration = Displacement2D` |
| V0 | V2 | V2 | Result has V2 | `Duration * Velocity2D = Displacement2D` |
| V3 | V0 | V3 | Result has V3 | `Velocity3D * Duration = Displacement3D` |
| V0 | V3 | V3 | Result has V3 | `Duration * Velocity3D = Displacement3D` |
| V4 | V0 | V4 | Result has V4 | `Velocity4D * Duration = Displacement4D` |
| V0 | V4 | V4 | Result has V4 | `Duration * Velocity4D = Displacement4D` |
| VN | VN | - | **Not a `*` operator** | Use `.Dot()` or `.Cross()` instead |

**If the result dimension does not have the required vector form, the operator is not generated.** For example, `Force * Length = Energy` only generates `ForceMagnitude * Length = Energy` (V0 * V0 = V0), because Energy is scalar-only. `Force3D * Length` is not Energy3D - it's a scaled Force3D (same-dimension scalar multiplication, not a cross-dimensional relationship).

### Relationship Types in dimensions.json

The current `integrals` and `derivatives` lists are supplemented with `dotProducts` and `crossProducts`:

```json
{
  "name": "Force",
  "symbol": "M L T⁻²",
  "dimensionalFormula": { "mass": 1, "length": 1, "time": -2 },
  "quantities": {
    "vector0": { "base": "ForceMagnitude" },
    "vector1": { "base": "Force1D" },
    "vector2": { "base": "Force2D" },
    "vector3": { "base": "Force3D" },
    "vector4": { "base": "Force4D" }
  },
  "integrals": [
    { "other": "Length", "result": "Energy" },
    { "other": "Time", "result": "Momentum" }
  ],
  "derivatives": [],
  "dotProducts": [
    { "other": "Length", "result": "Energy" }
  ],
  "crossProducts": [
    { "other": "Length", "result": "Torque" }
  ]
}
```

**How the generator uses each relationship type:**

- **`integrals`** (`Self * Other = Result`): Generates `*` operators following the vector form propagation rules above. Only V0-V0 and VN-V0 combinations.
- **`derivatives`** (`Self / Other = Result`): Generates `/` operators following the same propagation rules.
- **`dotProducts`** (`Self · Other = Result`): Generates `.Dot()` methods on VN types (N >= 1) where both self and other have that VN form. Result is always V0 of the result dimension.
- **`crossProducts`** (`Self × Other = Result`): Generates `.Cross()` methods only on V3 types where both self and other have V3 forms. Result is V3 of the result dimension.

### Complete Example: Velocity Dimension

Given:

```json
{
  "name": "Velocity",
  "quantities": {
    "vector0": { "base": "Speed" },
    "vector1": { "base": "Velocity1D" },
    "vector2": { "base": "Velocity2D" },
    "vector3": { "base": "Velocity3D" },
    "vector4": { "base": "Velocity4D" }
  },
  "integrals": [
    { "other": "Time", "result": "Length" }
  ],
  "derivatives": [
    { "other": "Time", "result": "Acceleration" }
  ]
}
```

The generator produces these `*` operators (from `integrals`):

```csharp
// V0 * V0 → V0
public static Length operator *(Speed left, Duration right);
public static Length operator *(Duration left, Speed right);

// V1 * V0 → V1 (Length has V1 = Displacement1D)
public static Displacement1D operator *(Velocity1D left, Duration right);
public static Displacement1D operator *(Duration left, Velocity1D right);

// V2 * V0 → V2 (Length has V2 = Displacement2D)
public static Displacement2D operator *(Velocity2D left, Duration right);
public static Displacement2D operator *(Duration left, Velocity2D right);

// V3 * V0 → V3 (Length has V3 = Displacement3D)
public static Displacement3D operator *(Velocity3D left, Duration right);
public static Displacement3D operator *(Duration left, Velocity3D right);

// V4 * V0 → V4 (Length has V4 = Displacement4D)
public static Displacement4D operator *(Velocity4D left, Duration right);
public static Displacement4D operator *(Duration left, Velocity4D right);
```

And these `/` operators (from `derivatives`):

```csharp
// V0 / V0 → V0
public static AccelerationMagnitude operator /(Speed left, Duration right);

// V1 / V0 → V1
public static Acceleration1D operator /(Velocity1D left, Duration right);

// V2 / V0 → V2
public static Acceleration2D operator /(Velocity2D left, Duration right);

// V3 / V0 → V3
public static Acceleration3D operator /(Velocity3D left, Duration right);

// V4 / V0 → V4
public static Acceleration4D operator /(Velocity4D left, Duration right);
```

### Complete Example: Force Dimension (with dot/cross)

Given:

```json
{
  "name": "Force",
  "quantities": {
    "vector0": { "base": "ForceMagnitude" },
    "vector1": { "base": "Force1D" },
    "vector3": { "base": "Force3D" }
  },
  "integrals": [
    { "other": "Length", "result": "Energy" },
    { "other": "Time", "result": "Momentum" }
  ],
  "dotProducts": [
    { "other": "Length", "result": "Energy" }
  ],
  "crossProducts": [
    { "other": "Length", "result": "Torque" }
  ]
}
```

**Scalar operators from `integrals`:**

```csharp
// Force * Length = Energy (V0 * V0 → V0 only, because Energy is scalar-only)
public static Energy operator *(ForceMagnitude left, Length right);
public static Energy operator *(Length left, ForceMagnitude right);

// Force * Time = Momentum (propagates across all matching forms)
public static MomentumMagnitude operator *(ForceMagnitude left, Duration right);  // V0*V0→V0
public static Momentum1D operator *(Force1D left, Duration right);               // V1*V0→V1
public static Momentum3D operator *(Force3D left, Duration right);               // V3*V0→V3
// (plus commutative versions)
```

Note: `Force1D * Length` does NOT generate an Energy1D operator because Energy has no V1 form. That combination is simply not available as a `*` operator.

**Dot product methods from `dotProducts`:**

```csharp
// Force · Length = Energy (dot product: VN · VN → V0)
public Energy Force1D.Dot(Displacement1D other);     // V1·V1 → V0
public Energy Force3D.Dot(Displacement3D other);     // V3·V3 → V0
```

This is how `Force3D * Displacement3D = Energy` is expressed: not as a `*` operator (which would be ambiguous) but as an explicit `.Dot()` call that always returns V0.

**Cross product methods from `crossProducts`:**

```csharp
// Force × Length = Torque (cross product: V3 × V3 → V3)
public Torque3D Force3D.Cross(Displacement3D other);
```

### How Semantic Overloads Participate

Semantic overloads participate in cross-dimensional operations through implicit widening to their base type. The resolution chain:

```csharp
// User writes:
Weight w = Weight.FromNewtons(9.8);       // V0 overload of ForceMagnitude
Height h = Height.FromMeters(10.0);       // V0 overload of Length
Energy e = w * h;                         // How does this work?

// Resolution:
// 1. Weight implicitly converts to ForceMagnitude (base)
// 2. Height implicitly converts to Length (base)
// 3. ForceMagnitude * Length = Energy (generated operator)
// 4. Result: Energy
```

```csharp
// Vector overloads:
WeightVector wv = WeightVector.Create(0, -9.8, 0);  // V3 overload of Force3D
Displacement3D d = Displacement3D.Create(5, 0, 0);

// Dot product:
Energy e = wv.Dot(d);
// 1. WeightVector widens to Force3D
// 2. Force3D.Dot(Displacement3D) = Energy
// 3. Result: Energy

// Cross product:
Torque3D t = wv.Cross(d);
// 1. WeightVector widens to Force3D
// 2. Force3D.Cross(Displacement3D) = Torque3D
// 3. Result: Torque3D
```

**The result of a cross-dimensional operation is always a base type**, never a semantic overload. This is consistent with rule 5 from the semantic overloads section. If the user wants to assert the result is a specific overload, they use explicit narrowing:

```csharp
Energy e = w * h;                     // base type result
Work work = Work.From(e);             // explicit narrowing to semantic overload
// or: Work work = (Work)(w * h);     // cast syntax
```

### Inverse Relationships

Every integral generates a corresponding derivative, and vice versa. The generator automatically creates the inverse operators:

```
If:    A * B = C     (integral)
Then:  C / B = A     (auto-generated derivative)
And:   C / A = B     (auto-generated derivative)
```

Example: `Force * Time = Momentum` generates:

```csharp
// Forward (integral):
MomentumMagnitude = ForceMagnitude * Duration;
Momentum3D = Force3D * Duration;

// Inverse (auto-derived):
ForceMagnitude = MomentumMagnitude / Duration;
Force3D = Momentum3D / Duration;
Duration = MomentumMagnitude / ForceMagnitude;
// (Duration is V0-only, so the VN/VN case uses scalar division:)
// Momentum3D / Force3D is NOT generated as a Duration operator.
// Instead: Duration = Momentum3D.Magnitude() / Force3D.Magnitude()
```

**VN / VN across different dimensions is never generated as a `/` operator** because the result would be ambiguous (component-wise division is not physically meaningful). Users must either:

- Use magnitudes: `Duration d = velocity3D.Magnitude() / acceleration3D.Magnitude()`
- Use dot product where appropriate
- Work at V0 level

### Same-Dimension Operators vs Cross-Dimension Operators

To avoid confusion, there are two distinct categories of operators:

**Same-dimension operators** (always present, defined on the type itself):

```csharp
// Addition/subtraction (same type)
Speed + Speed → Speed
Velocity3D + Velocity3D → Velocity3D

// Scalar multiplication/division (by raw T, not another quantity)
Velocity3D * T → Velocity3D
Speed / T → Speed

// Same-type division → raw T
Speed / Speed → T
```

**Cross-dimension operators** (generated from relationships):

```csharp
// Integral: Velocity * Time → Displacement
Speed * Duration → Length
Velocity3D * Duration → Displacement3D

// Derivative: Velocity / Time → Acceleration
Speed / Duration → AccelerationMagnitude
Velocity3D / Duration → Acceleration3D

// Dot product:
Force3D.Dot(Displacement3D) → Energy

// Cross product:
Force3D.Cross(Displacement3D) → Torque3D
```

These are distinct because same-dimension `*` by `T` is always available, while cross-dimension `*` by another quantity type is only available when a relationship is declared.

### Rules for Operator Dimensionality (Summary)

| Operation | Left | Right | Result form | Generated from |
|-----------|------|-------|-------------|----------------|
| `*` | V0(A) | V0(B) | V0(C) | `integrals` |
| `*` | VN(A) | V0(B) | VN(C) if C has VN | `integrals` |
| `*` | V0(A) | VN(B) | VN(C) if C has VN | `integrals` (commutative) |
| `/` | VN(A) | V0(B) | VN(C) if C has VN | `derivatives` |
| `/` | V0(A) | V0(B) | V0(C) | `derivatives` |
| `.Dot()` | VN(A) | VN(B) | V0(C) | `dotProducts` |
| `.Cross()` | V3(A) | V3(B) | V3(C) | `crossProducts` |
| `+` / `-` | VN(A) | VN(A) | VN(A) | Same dimension |
| `*` / `/` | VN(A) | T | VN(A) | Same dimension (scalar) |
| `Magnitude()` | VN(A) | - | V0(A) | Structural |

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

## Proposed Base Types

This section defines the base type name for every physical dimension at each supported vector form. These are the canonical types that carry the full operator suite and cross-dimensional relationships. Semantic overloads (Width, Weight, etc.) derive from these.

### Naming Patterns

Three patterns emerge naturally based on whether the dimension has an established magnitude name:

**Pattern A** - Distinct magnitude name exists in physics:

| V0 | V1 | V2 | V3 | V4 |
|----|----|----|----|----|
| Speed | Velocity1D | Velocity2D | Velocity3D | Velocity4D |

**Pattern B** - Dimension is inherently non-directional (V0 only):

| V0 |
|----|
| Mass |

**Pattern C** - Directional but no distinct magnitude name:

| V0 | V1 | V2 | V3 | V4 |
|----|----|----|----|----|
| ForceMagnitude | Force1D | Force2D | Force3D | Force4D |

### Vector Form Applicability

Not all dimensions support all vector forms. Three categories:

- **Scalar-only** (V0): Quantities with no meaningful directional interpretation. Mass, Time, Energy, Pressure, etc.
- **Linear** (V0, V1, V2, V3, V4): Quantities that can point in any spatial direction. Velocity, Force, Acceleration, etc.
- **Rotational** (V0, V1, V3): Pseudovectors that are scalar in 2D and axial vectors in 3D. Angular velocity, Torque, etc. No V2 or V4 forms because angular quantities in 2D reduce to a signed scalar (V1), not a 2-component vector.

### Complete Base Type Catalog

#### Base SI Dimensions

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Dimensionless | 1 | Ratio | SignedRatio | - | - | - | Scalar + signed |
| Length | L | Length | Displacement1D | Displacement2D | Displacement3D | Displacement4D | Linear |
| Mass | M | Mass | - | - | - | - | Scalar-only |
| Time | T | Duration | - | - | - | - | Scalar-only |
| ElectricCurrent | I | CurrentMagnitude | Current1D | - | Current3D | - | Linear (partial) |
| Temperature | Θ | Temperature | TemperatureDelta | - | - | - | Scalar + signed |
| AmountOfSubstance | N | AmountOfSubstance | - | - | - | - | Scalar-only |
| LuminousIntensity | J | LuminousIntensity | - | - | - | - | Scalar-only |

#### Geometry

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Area | L² | Area | - | - | - | - | Scalar-only |
| Volume | L³ | Volume | - | - | - | - | Scalar-only |
| NuclearCrossSection | L² | NuclearCrossSection | - | - | - | - | Scalar-only |

#### Linear Motion

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Velocity | L T⁻¹ | Speed | Velocity1D | Velocity2D | Velocity3D | Velocity4D | Linear |
| Acceleration | L T⁻² | AccelerationMagnitude | Acceleration1D | Acceleration2D | Acceleration3D | Acceleration4D | Linear |
| Jerk | L T⁻³ | JerkMagnitude | Jerk1D | Jerk2D | Jerk3D | Jerk4D | Linear |
| Snap | L T⁻⁴ | SnapMagnitude | Snap1D | Snap2D | Snap3D | Snap4D | Linear |

#### Rotational Motion

| Dimension | Formula | V0 | V1 | V3 | Category |
|-----------|---------|----|----|-----|----------|
| AngularDisplacement | 1 | Angle | SignedAngle | AngularDisplacement3D | Rotational |
| AngularVelocity | T⁻¹ | AngularSpeed | AngularVelocity1D | AngularVelocity3D | Rotational |
| AngularAcceleration | T⁻² | AngularAccelerationMagnitude | AngularAcceleration1D | AngularAcceleration3D | Rotational |
| AngularJerk | T⁻³ | AngularJerkMagnitude | AngularJerk1D | AngularJerk3D | Rotational |
| Torque | M L² T⁻² | TorqueMagnitude | Torque1D | Torque3D | Rotational |
| AngularMomentum | M L² T⁻¹ | AngularMomentumMagnitude | AngularMomentum1D | AngularMomentum3D | Rotational |
| MomentOfInertia | M L² | MomentOfInertia | - | - | Scalar-only |

#### Mechanics

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Force | M L T⁻² | ForceMagnitude | Force1D | Force2D | Force3D | Force4D | Linear |
| Momentum | M L T⁻¹ | MomentumMagnitude | Momentum1D | Momentum2D | Momentum3D | Momentum4D | Linear |
| Pressure | M L⁻¹ T⁻² | Pressure | - | - | - | - | Scalar-only |
| Energy | M L² T⁻² | Energy | - | - | - | - | Scalar-only |
| Power | M L² T⁻³ | Power | - | - | - | - | Scalar-only |
| Density | M L⁻³ | Density | - | - | - | - | Scalar-only |

#### Frequency and Rates

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Frequency | T⁻¹ | Frequency | - | - | - | - | Scalar-only |
| RadioactiveActivity | T⁻¹ | RadioactiveActivity | - | - | - | - | Scalar-only |

#### Electrical

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| ElectricCharge | I T | ChargeMagnitude | Charge | - | - | - | Scalar + signed |
| ElectricPotential | M L² T⁻³ I⁻¹ | VoltageMagnitude | Voltage | - | - | - | Scalar + signed |
| ElectricField | M L T⁻³ I⁻¹ | ElectricFieldMagnitude | ElectricField1D | ElectricField2D | ElectricField3D | - | Linear (partial) |
| ElectricResistance | M L² T⁻³ I⁻² | Resistance | - | - | - | - | Scalar-only |
| ElectricCapacitance | M⁻¹ L⁻² T⁴ I² | Capacitance | - | - | - | - | Scalar-only |

#### Radiation

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| AbsorbedDose | L² T⁻² | AbsorbedDose | - | - | - | - | Scalar-only |
| EquivalentDose | L² T⁻² | EquivalentDose | - | - | - | - | Scalar-only |

#### Optical

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| LuminousFlux | J | LuminousFlux | - | - | - | - | Scalar-only |
| Illuminance | J L⁻² | Illuminance | - | - | - | - | Scalar-only |
| OpticalPower | L⁻¹ | OpticalPower | - | - | - | - | Scalar-only |

#### Chemical

| Dimension | Formula | V0 | V1 | V2 | V3 | V4 | Category |
|-----------|---------|----|----|----|----|-----|----------|
| Concentration | N L⁻³ | Concentration | - | - | - | - | Scalar-only |

### Naming Rationale

**Names chosen over alternatives:**

| Chosen | Alternative considered | Rationale |
|--------|----------------------|-----------|
| `Duration` (V0 of Time) | `Time` | "Time" is the dimension name; "Duration" is the magnitude concept (a span of time, always >= 0). Avoids ambiguity when the dimension and the type share a name. |
| `Ratio` (V0 of Dimensionless) | `Dimensionless` | Clearer intent. A ratio is always a non-negative magnitude. The dimension is "Dimensionless"; the V0 base type is "Ratio". |
| `SignedRatio` (V1 of Dimensionless) | `Dimensionless1D` | More readable than appending "1D" to "Dimensionless". Conveys that it's a signed value. |
| `Angle` (V0 of AngularDisplacement) | `AngularDisplacementMagnitude` | "Angle" is the universally understood magnitude name. No one says "angular displacement magnitude". |
| `SignedAngle` (V1 of AngularDisplacement) | `AngularDisplacement1D` | Idiomatic. Matches how game engines and graphics APIs name the signed variant. |
| `AngularSpeed` (V0 of AngularVelocity) | `AngularVelocityMagnitude` | Parallels Speed/Velocity. "Angular speed" is the established physics term for the magnitude. |
| `Length` (V0 of Length dimension) | `Distance` | "Length" is the most general term. "Distance" is a semantic overload (separation between two points). |
| `CurrentMagnitude` (V0 of ElectricCurrent) | `ElectricCurrentMagnitude` | Shorter while unambiguous. "Current" alone is too common a word. |
| `VoltageMagnitude` (V0 of ElectricPotential) | `ElectricPotentialMagnitude` | "Voltage" is far more commonly used than "electric potential" in practice. |
| `Voltage` (V1 of ElectricPotential) | `ElectricPotential` | Same reasoning. V1 because voltage/potential difference is naturally signed (above or below reference). |
| `Charge` (V1 of ElectricCharge) | `ElectricCharge` | Shorter. V1 because charge is naturally signed (positive/negative). |
| `Resistance` (V0 of ElectricResistance) | `ElectricResistance` | Shorter while unambiguous in context. |
| `Capacitance` (V0 of ElectricCapacitance) | `ElectricCapacitance` | Same reasoning. |

**The `{Name}Magnitude` pattern** is used when:
- The dimension is fundamentally directional (Force, Acceleration, Momentum)
- No distinct magnitude name exists in common physics terminology
- The unqualified name is more naturally associated with the directional form

**Type counts by vector form:**

| Form | Count | Notes |
|------|-------|-------|
| V0 | 39 | Every dimension has a V0 form |
| V1 | 17 | Signed scalars and 1D directional |
| V2 | 8 | 2D linear quantities only |
| V3 | 16 | 3D linear + rotational pseudovectors |
| V4 | 8 | 4D linear quantities only |
| **Total** | **88** | Base types only, before semantic overloads |

### Magnitude Relationships

Every V1+ type has a structural `Magnitude()` method returning its V0 base:

| From (VN base) | Magnitude() returns (V0 base) |
|-----------------|-------------------------------|
| Velocity1D/2D/3D/4D | Speed |
| Acceleration1D/2D/3D/4D | AccelerationMagnitude |
| Force1D/2D/3D/4D | ForceMagnitude |
| Momentum1D/2D/3D/4D | MomentumMagnitude |
| Displacement1D/2D/3D/4D | Length |
| Jerk1D/2D/3D/4D | JerkMagnitude |
| Snap1D/2D/3D/4D | SnapMagnitude |
| SignedRatio | Ratio |
| TemperatureDelta | Temperature |
| Charge | ChargeMagnitude |
| Voltage | VoltageMagnitude |
| ElectricField1D/2D/3D | ElectricFieldMagnitude |
| Current1D/3D | CurrentMagnitude |
| SignedAngle | Angle |
| AngularVelocity1D/3D | AngularSpeed |
| AngularAcceleration1D/3D | AngularAccelerationMagnitude |
| AngularJerk1D/3D | AngularJerkMagnitude |
| Torque1D/3D | TorqueMagnitude |
| AngularMomentum1D/3D | AngularMomentumMagnitude |

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
