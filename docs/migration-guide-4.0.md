# Migrating from Semantics 3.x to 4.0

Semantics 4.0 makes every physical quantity a **value type**. `Length<double>`,
`Velocity3D<float>`, `Weight<decimal>` and the other 212 generated quantities were
`record` classes; they are now `readonly record struct`.

Strings, Paths, Music and Color are unaffected.

## Why

The record base built every result with `new TQuantity() with { Quantity = value }` —
two heap allocations for one number. Every operator and every unit factory went through
it, so `a + b` on a `Length<double>` allocated 48 bytes:

```
a + b            : 48.0 bytes/op      →  0.0 bytes/op
FromMeter        : 48.0 bytes/op      →  0.0 bytes/op
FromFoot         : 48.0 bytes/op      →  0.0 bytes/op
Length / Duration: 48.0 bytes/op      →  0.0 bytes/op
```

In a loop over a few thousand entities at 60 Hz that is tens of megabytes a second of
garbage, for a library whose whole purpose is to be the type a number is stored in.
`QuantityValueTypeTests` measures this now, so it cannot come back.

## Quick checklist

1. Remove null checks and `?` annotations on quantities — a struct is never null.
2. Replace `quantity is null` / `== null` with a comparison against `Zero`, if you
   meant "unset".
3. If you derived from `PhysicalQuantity<TSelf, T>`, implement
   `IPhysicalQuantity<TSelf, T>` on a struct instead.
4. Nothing to do for arithmetic, unit factories, `In(unit)`, `Dimension`, `CompareTo`,
   `Equals`, comparison operators, or `ToString` — all identical.

## 1. A quantity cannot be null

```csharp
Length<double>? maybe = null;         // now Nullable<Length<double>>, not a null reference
if (length is null) { … }             // no longer compiles for a non-nullable quantity
ArgumentNullException.ThrowIfNull(m); // now a no-op; analyzers flag it (CA2264)
```

Code that treated "no quantity" as `null` should use `Zero`, or `Nullable<>` where the
distinction between "zero" and "not supplied" genuinely matters.

## 2. `PhysicalQuantity<TSelf, T>` is gone

The abstract record base has been replaced by two things:

- **`IPhysicalQuantity<TSelf, T>`** — the interface a quantity implements, carrying
  `static abstract TSelf Create(T value)`. This is the seam that replaced the
  `where TSelf : PhysicalQuantity<TSelf, T>, new()` constraint, so generic code over
  quantities still works.
- **`PhysicalQuantityCore`** — the static class holding the validity and
  cross-dimension comparison rules a quantity used to inherit. Generated quantities
  delegate to it; you would rarely call it directly.

`SemanticQuantity<TStorage>` and `SemanticQuantity<TSelf, TStorage>` are **unchanged**.
They remain available for a reference-type semantic quantity of your own; the physical
quantities simply no longer derive from them.

## 3. `default` does not run a construction guard

This is the one behaviour given up, and it is narrow.

A Vector0 quantity is non-negative, and `default` gives it zero — which satisfies that
invariant, so nothing changes for it. But three quantities declare
`physicalConstraints.minExclusive: "0"` in `dimensions.json` — `Wavelength`, `Period`
and `HalfLife` — for which zero is unphysical:

```csharp
Wavelength<double>.FromMeter(0.0);   // still throws ArgumentException
default(Wavelength<double>).Value;   // 0.0 — the guard cannot run
```

A struct always has a zero value and no factory can intercept it. If you need to reject
an unset value, compare against `Zero` explicitly rather than relying on construction to
have refused it.

## 4. Equality and hashing

Value equality is unchanged in what it reports: two quantities are equal when they share
a type and a value, and `Equals(IPhysicalQuantity<T>)` still compares dimension and
value, returning `false` across dimensions rather than throwing. `CompareTo` still throws
`ArgumentException` across dimensions, because quantities of different dimensions are not
ordered.

What changed is that a quantity is no longer reference-comparable. `ReferenceEquals(a, b)`
on two quantities now boxes both and is always `false`; it was never meaningful and is
now visibly so.
