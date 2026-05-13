# Architecture Guide

This document covers the architecture of the **semantic strings, paths, and validation** subsystems. The physics quantities subsystem is metadata-driven and is documented separately:

-   [`docs/strategy-unified-vector-quantities.md`](strategy-unified-vector-quantities.md) — the unified `IVector0..IVector4` model.
-   [`docs/physics-generator.md`](physics-generator.md) — `dimensions.json` schema and the source-generator pipeline.
-   The [Physics Quantities: Metadata-Driven Generation](#physics-quantities-metadata-driven-generation) section below provides a short orientation and links into those documents.

## Table of Contents

-   [Overview](#overview)
-   [SOLID Principles Implementation](#solid-principles-implementation)
-   [DRY Implementation](#dry-implementation)
-   [Design Patterns](#design-patterns)
-   [Class Hierarchy](#class-hierarchy)
-   [Validation System](#validation-system)
-   [Physics Quantities: Metadata-Driven Generation](#physics-quantities-metadata-driven-generation)
-   [Testing Strategy](#testing-strategy)

## Overview

The Semantics library is built around three pillars — **semantic strings**, **semantic paths**, and **physics quantities** — sharing a single philosophy: replace primitive obsession with strongly-typed, self-validating domain models. Strings and paths use a hand-authored attribute → strategy → rule → factory pipeline (described in this document). Physics quantities are emitted at compile time by a Roslyn incremental generator from declarative metadata (described in [Physics Quantities: Metadata-Driven Generation](#physics-quantities-metadata-driven-generation)). All three subsystems target `net10.0`, `net9.0`, `net8.0`, `net7.0`.

## SOLID Principles Implementation

### Single Responsibility Principle (SRP)

Each class has a single, well-defined responsibility:

#### Factory Pattern Implementation

-   **`ISemanticStringFactory<T>`**: Object creation only
-   **`SemanticStringFactory<T>`**: Concrete creation logic
-   **Purpose**: Separates construction from business logic

#### Validation Strategy Separation

-   **`IValidationStrategy`**: Defines validation processing
-   **`ValidateAllStrategy`**: "All must pass" logic
-   **`ValidateAnyStrategy`**: "Any can pass" logic
-   **`ValidationStrategyFactory`**: Strategy creation

### Open/Closed Principle (OCP)

Open for extension, closed for modification:

```csharp
// Add new validation rules without modifying existing code
public class CustomBusinessRule : ValidationRuleBase
{
    public override string RuleName => "CustomBusiness";

    protected override bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        // Custom validation logic
        return true;
    }
}
```

### Liskov Substitution Principle (LSP)

All implementations are fully substitutable through behavioral contracts:

```csharp
// Contract validation ensures LSP compliance
public static class SemanticStringContracts
{
    public static bool ValidateContracts<T>(T instance) where T : SemanticString<T>
    {
        // Validates basic contracts (reflexivity, etc.)
    }

    public static bool ValidateEqualityContracts<T>(T? first, T? second) where T : SemanticString<T>
    {
        // Validates equality behavior
    }
}
```

### Interface Segregation Principle (ISP)

Focused, client-specific interfaces:

-   **`ISemanticStringFactory<T>`**: Factory operations only
-   **`IValidationStrategy`**: Validation strategy operations only
-   **`IValidationRule`**: Individual rule operations only

### Dependency Inversion Principle (DIP)

High-level modules depend on abstractions:

```csharp
// Service depends on abstraction, not concrete implementation
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }
}
```

## DRY Implementation

### Strategy Pattern for Validation Logic

**Problem Eliminated**: Duplicated validation logic across multiple methods.

**Solution**: Centralized validation strategies with pluggable implementations.

```csharp
public interface IValidationStrategy
{
    bool Validate(IEnumerable<SemanticStringValidationAttribute> attributes, ISemanticString value);
}

// Reusable strategies eliminate duplication
public class ValidateAllStrategy : IValidationStrategy { /* implementation */ }
public class ValidateAnyStrategy : IValidationStrategy { /* implementation */ }
```

### Template Method Pattern for Validation Rules

**Problem Eliminated**: Common validation patterns repeated across attribute types.

**Solution**: Base class with template method for common functionality.

```csharp
public abstract class ValidationRuleBase : IValidationRule
{
    // Template method - common structure, specific implementation in derived classes
    public bool Validate(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        if (!IsApplicable(attribute)) return false;
        if (value?.ToString() is not string stringValue) return false;
        return ValidateCore(attribute, value);  // Specific implementation
    }

    protected abstract bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value);
}
```

## Design Patterns

### Factory Pattern

-   **Purpose**: Encapsulate object creation logic
-   **Implementation**: `ISemanticStringFactory<T>` and `SemanticStringFactory<T>`
-   **Benefits**: Consistent creation, testability, DI support

### Strategy Pattern

-   **Purpose**: Interchangeable validation algorithms
-   **Implementation**: `IValidationStrategy` with concrete strategies
-   **Benefits**: Extensibility, configurable behavior

### Template Method Pattern

-   **Purpose**: Define algorithm structure with customizable steps
-   **Implementation**: `ValidationRuleBase` with abstract methods
-   **Benefits**: Code reuse, consistent structure

## Class Hierarchy

```
ISemanticString
├── SemanticString<TDerived> (abstract base)
    ├── SemanticPath<TDerived> (path-specific base)
    │   ├── SemanticAbsolutePath<TDerived> (absolute paths base)
    │   │   └── AbsolutePath
    │   ├── SemanticRelativePath<TDerived> (relative paths base)
    │   │   └── RelativePath
    │   ├── SemanticFilePath<TDerived> (file paths base)
    │   │   ├── FilePath
    │   │   ├── AbsoluteFilePath
    │   │   └── RelativeFilePath
    │   ├── SemanticDirectoryPath<TDerived> (directory paths base)
    │   │   ├── DirectoryPath
    │   │   ├── AbsoluteDirectoryPath
    │   │   └── RelativeDirectoryPath
    │   ├── FileName
    │   └── FileExtension
    └── [Custom semantic string types]

ISemanticStringFactory<T>
└── SemanticStringFactory<T>

IValidationStrategy
├── ValidateAllStrategy
├── ValidateAnyStrategy
└── [Custom validation strategies]

IValidationRule
├── ValidationRuleBase (abstract)
│   ├── LengthValidationRule
│   ├── PatternValidationRule
│   └── [Custom validation rules]
└── [Custom validation rules]
```

### Path Interface Hierarchy

The library provides a comprehensive interface hierarchy for path types that enables polymorphism and type-safe operations:

```
IPath (base interface for all path types)
├── IAbsolutePath : IPath
├── IRelativePath : IPath
├── IFilePath : IPath
├── IDirectoryPath : IPath
├── IAbsoluteFilePath : IFilePath, IAbsolutePath
├── IRelativeFilePath : IFilePath, IRelativePath
├── IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
└── IRelativeDirectoryPath : IDirectoryPath, IRelativePath

IFileName (separate hierarchy for non-path file components)
IFileExtension (separate hierarchy for file extensions)
```

**Interface Implementation Mapping:**

```csharp
// Path types implement their corresponding interfaces
AbsolutePath : SemanticAbsolutePath<AbsolutePath>, IAbsolutePath
RelativePath : SemanticRelativePath<RelativePath>, IRelativePath
FilePath : SemanticFilePath<FilePath>, IFilePath
DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
RelativeFilePath : SemanticFilePath<RelativeFilePath>, IRelativeFilePath
AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath

// Non-path types have separate interfaces
FileName : SemanticString<FileName>, IFileName
FileExtension : SemanticString<FileExtension>, IFileExtension
```

**Polymorphic Benefits:**

1. **Type-safe Collections**: Store different path types in the same collection using common interfaces
2. **Polymorphic Methods**: Write methods that accept any path type or specific categories
3. **Interface Segregation**: Use the most specific interface needed for your use case
4. **Extensibility**: Easy to add new path types that integrate with existing polymorphic code

## Validation System

### Architecture Overview

The validation system follows a layered approach:

1. **Attribute Layer**: `SemanticStringValidationAttribute` classes
2. **Strategy Layer**: `IValidationStrategy` implementations
3. **Rule Layer**: `IValidationRule` implementations
4. **Factory Layer**: `ValidationStrategyFactory` creates strategies

### Validation Flow

```
User Creates Semantic String
           ↓
    SemanticStringFactory
           ↓
   Get Validation Attributes
           ↓
   ValidationStrategyFactory.GetStrategy()
           ↓
   IValidationStrategy.Validate()
           ↓
   For Each Attribute: IValidationRule.Validate()
           ↓
   Combine Results (All/Any/Custom)
           ↓
   Return Valid Object or Throw Exception
```

## Physics Quantities: Metadata-Driven Generation

Unlike strings and paths — which are hand-authored — every physics quantity type, factory, operator, and constant is emitted by a Roslyn incremental generator. The single source of truth is `Semantics.SourceGenerators/Metadata/`:

| File | Contents |
|---|---|
| `dimensions.json` | Every physical dimension, the vector forms it supports (`Vector0`..`Vector4`), its `availableUnits`, semantic overloads (e.g. `Weight` over `ForceMagnitude`), and cross-dimensional relationships (`integrals`, `derivatives`, `dotProducts`, `crossProducts`). |
| `units.json` | Unit declarations with `factoryName` (plural) and a base-unit conversion expression. |
| `magnitudes.json` | SI magnitude prefixes for unit derivations. |
| `conversions.json` | Conversion factors between non-SI units and the SI base. |
| `domains.json` | Domain grouping for `PhysicalConstants` (e.g. `Fundamental`, `Chemistry`, `AngularMechanics`). |

### Generator pipeline

```
Metadata/*.json
       │
       ▼
Semantics.SourceGenerators (Roslyn IIncrementalGenerator)
       │
       ├── QuantitiesGenerator       → one record per quantity (V0/V1/V2/V3/V4 + overloads)
       │                                + From{Unit} factory per declared unit
       │                                + Vector0Guards.EnsureNonNegative / EnsurePositive
       │                                + cross-dimensional *, /, Dot, Cross operators
       ├── ConversionsGenerator      → unit-to-SI conversion helpers
       ├── PhysicalConstantsGenerator → PhysicalConstants.<Domain>.* (PreciseNumber)
       │                                + PhysicalConstants.Generic.*<T>() (T.CreateChecked)
       └── StorageHelpersGenerator   → DivideToStorage with DivideByZeroException
       │
       ▼
Semantics.Quantities/Generated/   (committed source — diff before commit)
```

### Runtime contract — `IPhysicalQuantity<T>`

Every generated V0 / V1 quantity (and V0/V1 semantic overload) implements
`IPhysicalQuantity<T>` through the `PhysicalQuantity<TSelf, T>` base. The contract is
deliberately slim:

```csharp
public interface IPhysicalQuantity<T>
    : ISemanticQuantity<T>
    , IComparable<IPhysicalQuantity<T>>
    , IEquatable<IPhysicalQuantity<T>>
    where T : struct, INumber<T>
{
    T Value { get; }                  // stored in the dimension's SI base unit
    bool IsPhysicallyValid { get; }   // structural: finite, non-NaN
    DimensionInfo Dimension { get; }  // PhysicalDimensions.X (generated singleton)
}
```

Semantics (locked in #59):

1. **`Dimension`** is generated per quantity as `=> PhysicalDimensions.{Name}` so every
   instance knows what it measures without reflection.
2. **`CompareTo(IPhysicalQuantity<T>?)`** compares stored SI-base values, but throws
   `ArgumentException` when the dimensions differ — quantities of different dimensions
   are not ordered.
3. **`Equals(IPhysicalQuantity<T>?)`** is total: cross-dimension comparisons return
   `false` rather than throwing, because equality (unlike ordering) must be defined
   for every pair.

V2 / V3 / V4 vector types implement only their `IVectorN<TSelf, T>` interface — the
slim `IPhysicalQuantity<T>` contract applies to scalar-storage quantities.

### Unit conversion — typed `In(...)`

Each V0/V1 quantity emits a dimensionally-typed `In` method:

```csharp
// On generated Length<T>:
public T In(ILengthUnit unit) => unit.FromBase(Value);

// On generated Temperature<T>:
public T In(ITemperatureUnit unit) => unit.FromBase(Value);
```

`ILengthUnit`, `ITemperatureUnit`, etc. are marker interfaces generated by
`DimensionsGenerator` — each declared unit implements `IUnit` plus the marker(s) for
the dimension(s) it belongs to. Cross-dimension calls fail at compile time:

```csharp
length.In(Units.Kilogram);   // ❌ compile error — Kilogram : IMassUnit, not ILengthUnit
length.In(Units.Kilometer);  // ✓
```

The `IUnit` interface carries `Name`, `Symbol`, `Dimension`, and the affine conversion
(`base = value × ToBaseFactor + ToBaseOffset`). `ToBase<T>` / `FromBase<T>` are default
interface methods, so each concrete unit only has to declare its factor and offset.
The static `Units` catalogue exposes one singleton per declared unit.

`UnitConversionException` remains in the runtime for any future untyped-unit dispatch
path; the typed `In(I{Dim}Unit)` path is compile-time-safe and does not throw.

### Vector-form invariants

These are enforced structurally by the generated types and locked in by `Semantics.Test`:

1. `V0` magnitudes are non-negative; the SI factory throws `ArgumentException` on a negative value, and `V0 - V0` returns `T.Abs(a - b)` to preserve the invariant.
2. A V0 overload can opt into a strict-positive guard with `physicalConstraints: { "minExclusive": "0" }` (used by `Wavelength`, `Period`, `HalfLife`); `EnsurePositive` then rejects zero as well.
3. Semantic overloads widen implicitly to their base, narrow explicitly (`Weight.From(forceMagnitude)`).
4. `IVectorN.Magnitude()` for N ≥ 1 returns the corresponding `IVector0`.

### Generator diagnostics

Metadata errors fail the build rather than silently emitting wrong code:

-   **SEM001** — a relationship references a dimension that does not exist.
-   **SEM002** — schema-level metadata issue (missing `name` / `symbol`, empty `availableUnits`, duplicate type names, no vector forms declared).
-   **SEM003** — a relationship's explicit `forms` list references a vector form not declared on a participating dimension.
-   **SEM004** — a dimension's `availableUnits` array references a unit name that isn't declared in `units.json` (catches typos that would otherwise produce a wrong identity-conversion factory).

For the schema, an end-to-end "add a dimension" walk-through, and the design rationale, see [`docs/physics-generator.md`](physics-generator.md) and [`docs/strategy-unified-vector-quantities.md`](strategy-unified-vector-quantities.md).

## Testing Strategy

### Contract Testing

-   All implementations must pass contract validation
-   `SemanticStringContracts` provides standardized tests
-   Ensures LSP compliance

### Example Test Structure

```csharp
[Test]
public void EmailAddress_ShouldSatisfyContracts()
{
    var email1 = _factory.Create("user1@example.com");
    var email2 = _factory.Create("user2@example.com");
    var email3 = _factory.Create("user3@example.com");

    Assert.IsTrue(SemanticStringContracts.ValidateContracts(email1));
    Assert.IsTrue(SemanticStringContracts.ValidateEqualityContracts(email1, email2));
    Assert.IsTrue(SemanticStringContracts.ValidateComparisonContracts(email1, email2, email3));
}
```

This architecture ensures the library remains maintainable, extensible, and testable while providing excellent performance and type safety.
