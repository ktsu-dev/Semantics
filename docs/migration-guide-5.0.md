# Migrating from Semantics 4.x to 5.0

Semantics 5.0 removes twelve generated operators and three generated methods from
`ktsu.Semantics.Quantities`. Every one of them computed a result the dimensions of its
operands do not support, and each was found by the same check the C++ projection has
always run.

Strings, Paths, Music and Color are unaffected. No quantity, unit, factory, constant or
conversion changed, and nothing was renamed.

## Why

A relationship in `dimensions.json` is a claim — `Force × Length → Torque` — and the
dimensional exponents are what check it. The C++ projection multiplies the exponents out
because it has to: it writes `Result{ lhs.value() * rhs.value() }` over a
`Quantity<Dimension<…>>`, so a claim the exponents contradict does not compile. It has
refused these relationships by name since it existed.

The C# generator had no such check. It verified that a relationship's names resolved
(`SEM001`) and that the vector forms it asked for existed (`SEM003`), and then emitted the
operator. 4.3 moved the check into `Semantics.Vocabulary`, shared source compiled into
both generators, and reported it as `SEM008` while still emitting the operator — audible,
but not yet a broken package. That is what found the `Sensitivity` bug, since fixed in
4.3.1.

5.0 lets the shared vocabulary drive C# emission rather than only check it, so a
relationship it refuses is not among the ones there are to write. The two generators now
disagree about nothing.

## Quick checklist

1. Search your code for the three products and one method listed below.
2. For each, decide what you meant: the dimensions say the result is not what the
   operator claimed, so there is a real modelling question underneath every call site.
3. Nothing else to do.

## What was removed

Three products, each in the four spellings C# generated for it — the declared direction,
its commutation, and the two divisions that undo it:

| Removed | Why |
|---|---|
| `TorqueMagnitude<T> * Angle<T> -> Energy<T>` | the `r × F` versus `τ · θ` contradiction |
| `MomentOfInertia<T> * AngularSpeed<T> -> AngularMomentumMagnitude<T>` | the same contradiction |
| `MomentOfInertia<T> * AngularAccelerationMagnitude<T> -> TorqueMagnitude<T>` | the same contradiction |

and one dot product, at each of the three vector forms it reached:

| Removed | Why |
|---|---|
| `Force2D<T>.Dot(Displacement2D<T>) -> Energy<T>` | signed value, magnitude result |
| `Force3D<T>.Dot(Displacement3D<T>) -> Energy<T>` | signed value, magnitude result |
| `Force4D<T>.Dot(Displacement4D<T>) -> Energy<T>` | signed value, magnitude result |

The untyped `Dot` over two vectors of the same quantity — `Force3D.Dot(Force3D)`, which
answers with the storage type — is unaffected, as is `Displacement3D.Cross(Force3D)`.

## 1. The rotational three

`Torque * AngularDisplacement -> Energy`, `MomentOfInertia * AngularVelocity ->
AngularMomentum` and `MomentOfInertia * AngularAcceleration -> Torque` are **not** fixable
by choosing different angle exponents, and that is provable rather than a matter of taste.
`Torque * AngularDisplacement -> Energy` forces torque's angle exponent to −1;
`Force × Length -> Torque` forces it to 0. It is the classic `r × F` versus `τ · θ`
contradiction, and it is why SI keeps the radian dimensionless.

What separates a torque from an energy in this library is the *name*, not the exponents —
that is the whole job of the nominal layer. So do the arithmetic and name the result:

```csharp
// Was:
Energy<double> work = torque * angle;

// Now:
Energy<double> work = Energy<double>.FromJoule(torque.Value * angle.Value);
```

```csharp
// Was:
AngularMomentumMagnitude<double> l = inertia * spin;

// Now:
AngularMomentumMagnitude<double> l =
    AngularMomentumMagnitude<double>.FromKilogramMeterSquaredPerSecond(inertia.Value * spin.Value);
```

The inverse directions (`energy / angle`, `angularMomentum / inertia`, and so on) went
with them and come back the same way.

## 2. Force dotted with displacement

The exponents agree here — `LMT⁻² · L` is `L²MT⁻²`, which is what `Energy` is — and the
claim is still unkeepable, for a reason the vector forms surfaced. A dot product is
signed: a force opposing a displacement does negative work. `Energy` declares only a
magnitude form, and a magnitude cannot be negative, so its factories run
`Vector0Guards.EnsureNonNegative` and throw. The generated method would therefore have
thrown `ArgumentException` on an ordinary input — a braking force, a spring under
compression, anything pushing back.

If you only ever dotted aligned vectors, you were relying on that. Take the sign
explicitly:

```csharp
// Was:
Energy<double> work = force.Dot(displacement);   // threw when the two opposed each other

// Now — the signed work, as a bare number:
double work = (force.X * displacement.X) + (force.Y * displacement.Y) + (force.Z * displacement.Z);

// Or, if you want the magnitude and know you are discarding the direction:
Energy<double> magnitude = Energy<double>.FromJoule(Math.Abs(work));
```

The fix on the library's side is named rather than guessed at: `Energy` needs a `vector1`
form for a signed result to land in. That is a metadata change, and when it is made the
relationship becomes keepable and the method comes back — answering with the signed form,
not this one.

## What this does not change

- No quantity type was added, removed or renamed, so the storage-type alias packages
  (`ktsu.Semantics.Quantities.Double` and friends) are unchanged.
- `Sensitivity<T> * Pressure<T> -> VoltageMagnitude<T>` is **not** affected. It was refused
  in 4.3 and is the reason `SEM008` exists, but the metadata bug behind it — `Sensitivity`
  declared as amperes per newton while the unit beside it said `VoltPerPascal` — was fixed
  in 4.3.1. The relationship is dimensionally true now and the operator is generated.
- `SEM008` still reports each refused relationship when you build metadata of your own; it
  now says no operator is generated, where it used to say one was generated anyway.
- `UnkeepableRelationshipTests` pins the set to exactly these four and asserts that none of
  them is in the compiled surface, so a fifth fails a test rather than disappearing into
  the `NoWarn` in `Semantics.Quantities.csproj`.
