# Semantic Quantities Library Design Document

## Executive Summary

This document presents the design for a strongly typed physical quantities library that provides compile-time safety, intuitive syntax, and comprehensive unit support, building upon the existing SemanticQuantity framework.

## Overview

The Semantic Quantities Library provides strongly typed quantities of physical dimensions with compile-time safety, intuitive syntax, and comprehensive unit support, building upon the existing SemanticQuantity framework.

## API Design Goals

The interface should remain clean and intuitive:

```csharp
// Primary API - clean and intuitive with compile-time safety
var lengthA = 10.Meters();              // Uses configured storage type
var lengthB = 20.5f.Feet();             // Automatic conversion to configured type
var lengthC = 30.999.Kilometers();      // Multiple units supported

var area = lengthA * lengthB;           // Length × Length → Area (compile-time validated)
var volume = area * lengthC;            // Area × Length → Volume (compile-time validated)

var time = 10.Seconds();
var velocity = lengthA / time;          // Length ÷ Time → Velocity (compile-time validated)

var combinedTime = 10.Minutes() + 30.Seconds();  // Same dimension addition
var speed = velocity.In(Units.KilometersPerHour); // Consistent unit conversion API

// Explicit generic usage when needed for specific precision
var preciseLength = 10.123m.Meters<decimal>();
var preciseArea = preciseLength * preciseLength;   // Returns Area<decimal>

// Compile-time safety: This would NOT compile
// var invalid = preciseLength + 5.Kilograms();    // Compile error: Length + Mass
// var mixed = preciseLength * 5.0.Meters<double>(); // Compile error: mixed storage types
```

## Architecture Overview

### Simplified Core Type System

```
ISemanticQuantity<T> (existing base interface)
└── IPhysicalQuantity<T> : ISemanticQuantity<T>
    ├── Length<T> : IPhysicalQuantity<T>, IMultiplyOperator<Length<T>, Area<T>>
    ├── Mass<T> : IPhysicalQuantity<T>
    ├── Time<T> : IPhysicalQuantity<T>
    ├── Area<T> : IPhysicalQuantity<T>, IMultiplyOperator<Length<T>, Volume<T>>
    ├── Velocity<T> : IPhysicalQuantity<T>, IDivideOperator<Time<T>, Acceleration<T>>
    └── [All other quantities with explicit operator interfaces...]
```

### Key Design Principles

#### 1. Simplified Interface Hierarchy
**Simplified from complex self-referencing:**
```csharp
// Clean and simple
IPhysicalQuantity<T> where T : struct, INumber<T>
```

#### 2. Explicit Operator Interfaces
**Clear relationship definitions:**
```csharp
IMultiplyOperator<Length<T>, Area<T>>     // Length * Length = Area
IDivideOperator<Time<T>, Velocity<T>>     // Length / Time = Velocity
```

#### 3. Integration with SemanticQuantity Framework

PhysicalQuantity extends the existing SemanticQuantity framework with:
- **Physical dimensional relationships** via explicit operator interfaces
- **Unit conversion capabilities** with SI base unit storage
- **Compile-time dimensional analysis** through interface contracts
- **Physical validation** (e.g., temperature above absolute zero)

#### 4. Generic Storage Type Support

The library inherits storage type flexibility from SemanticQuantity and adds physical-specific features:
- Support for the same storage types as SemanticQuantity (`float`, `double`, `decimal`, etc.)
- SI unit normalization for consistent calculations
- Physical unit conversion with precision preservation

## Domain Coverage

It should cover all base and derived SI physical dimensions, and be able to integrate and derive with other quantities from:

### Base SI Units (7 fundamental dimensions)
- **Length** (meter, m)
- **Mass** (kilogram, kg)  
- **Time** (second, s)
- **Electric Current** (ampere, A)
- **Thermodynamic Temperature** (kelvin, K)
- **Amount of Substance** (mole, mol)
- **Luminous Intensity** (candela, cd)

### Domain-Specific Derived Quantities

#### Mechanics
- Area (m²), Volume (m³)
- Velocity (m/s), Acceleration (m/s²)
- Force (N = kg⋅m/s²), Pressure (Pa = N/m²)
- Energy (J = N⋅m), Power (W = J/s)
- Momentum (kg⋅m/s), Angular Momentum (kg⋅m²/s)

#### Electromagnetism
- Electric Charge (C = A⋅s)
- Electric Potential (V = W/A)
- Electric Resistance (Ω = V/A)
- Electric Capacitance (F = C/V)
- Magnetic Field (T = Wb/m²)
- Electric Field (V/m)
- Inductance (H = Wb/A)

#### Thermodynamics
- Heat Capacity (J/K)
- Specific Heat Capacity (J/(kg⋅K))
- Thermal Conductivity (W/(m⋅K))
- Entropy (J/K)
- Heat Transfer Coefficient (W/(m²⋅K))

#### Chemistry
- Molarity (mol/L)
- Molality (mol/kg)
- Molar Mass (kg/mol)
- Reaction Rate (mol/(L⋅s))
- Gas Constant applications

#### Photometry
- Luminous Flux (lm = cd⋅sr)
- Illuminance (lx = lm/m²)
- Luminance (cd/m²)
- Luminous Efficacy (lm/W)

#### Rotational Mechanics
- Angular Velocity (rad/s)
- Angular Acceleration (rad/s²)
- Torque (N⋅m)
- Moment of Inertia (kg⋅m²)
- Angular Momentum (kg⋅m²/s)

## Core Interfaces

### Base Physical Quantity Interface
```csharp
public interface IPhysicalQuantity<T> : ISemanticQuantity<T>, 
    IEquatable<IPhysicalQuantity<T>>, IComparable<IPhysicalQuantity<T>>
    where T : struct, INumber<T>
{
    /// <summary>Physical dimension (e.g., "Length [L]", "Force [M L T⁻²]")</summary>
    string Dimension { get; }
    
    /// <summary>SI base unit for this quantity type</summary>
    IUnit BaseUnit { get; }
    
    /// <summary>Convert to specified unit</summary>
    T In(IUnit targetUnit);
    
    /// <summary>Convert to specified unit using generic unit type</summary>
    T In<TUnit>() where TUnit : IUnit, new();
    
    /// <summary>Get value with uncertainty if available</summary>
    UncertainValue<T> WithUncertainty { get; }
    
    /// <summary>Validate physical constraints (e.g., positive temperature)</summary>
    bool IsPhysicallyValid { get; }
}
```

### Simplified Operator Interfaces
```csharp
// Correctly constrained operator interfaces with proper TSelf definition
public interface IMultiplyOperator<TSelf, TOther, TResult> 
    where TSelf : IPhysicalQuantity<T>
    where TOther : IPhysicalQuantity<T>
    where TResult : IPhysicalQuantity<T>
{
    static abstract TResult operator *(TSelf left, TOther right);
}

public interface IDivideOperator<TSelf, TOther, TResult>
    where TSelf : IPhysicalQuantity<T>
    where TOther : IPhysicalQuantity<T> 
    where TResult : IPhysicalQuantity<T>
{
    static abstract TResult operator /(TSelf left, TOther right);
}

// Addition/subtraction only allowed within same dimension
public interface IAddOperator<TSelf> where TSelf : IPhysicalQuantity<T>
{
    static abstract TSelf operator +(TSelf left, TSelf right);
    static abstract TSelf operator -(TSelf left, TSelf right);
}

// Comparison operators for same dimension
public interface IComparisonOperator<TSelf> where TSelf : IPhysicalQuantity<T>
{
    static abstract bool operator >(TSelf left, TSelf right);
    static abstract bool operator <(TSelf left, TSelf right);
    static abstract bool operator >=(TSelf left, TSelf right);
    static abstract bool operator <=(TSelf left, TSelf right);
    static abstract bool operator ==(TSelf left, TSelf right);
    static abstract bool operator !=(TSelf left, TSelf right);
}
```

## Robust Unit System

### Improved Unit Interface
```csharp
public interface IUnit
{
    string Name { get; }
    string Symbol { get; }
    string Dimension { get; }           // Must match quantity dimension
    UnitSystem System { get; }
    bool IsBaseUnit { get; }
}

public interface ILinearUnit : IUnit
{
    double ToBaseFactor { get; }        // Multiplication factor to base unit
}

public interface IOffsetUnit : IUnit  
{
    double ToBaseFactor { get; }        // For temperature: K = C + 273.15
    double ToBaseOffset { get; }
}

public interface ICompoundUnit : IUnit
{
    IReadOnlyList<UnitComponent> Components { get; }  // For m/s, m²/s³, etc.
}

public record UnitComponent(IUnit Unit, int Power); // m² = (meter, 2)
```

### Unit Registry and Validation
```csharp
public static class Units
{
    // Length units
    public static readonly ILinearUnit Meter = new LinearUnit("meter", "m", "Length", 1.0, UnitSystem.SI);
    public static readonly ILinearUnit Kilometer = new LinearUnit("kilometer", "km", "Length", 1000.0, UnitSystem.SI);
    public static readonly ILinearUnit Foot = new LinearUnit("foot", "ft", "Length", 0.3048, UnitSystem.Imperial);
    
    // Temperature units with offset
    public static readonly ILinearUnit Kelvin = new LinearUnit("kelvin", "K", "Temperature", 1.0, UnitSystem.SI);
    public static readonly IOffsetUnit Celsius = new OffsetUnit("celsius", "°C", "Temperature", 1.0, 273.15, UnitSystem.SI);
    
    // Compound units
    public static readonly ICompoundUnit MetersPerSecond = new CompoundUnit("meters per second", "m/s", "Velocity",
        [new(Meter, 1), new(Second, -1)], UnitSystem.SI);
    
    // Unit validation at registration
    static Units()
    {
        ValidateUnitConsistency();
    }
}
```

## Core Design Principles

### 1. Type Safety
- Compile-time dimensional analysis
- Prevent invalid operations (e.g., adding Length + Mass)
- Return correct derived types from operations

### 2. Unit Conversion System
The system uses constants for conversion factors and offsets, internally storing values in SI units, with methods to convert to other applicable units of the same dimension, both metric and imperial.

All calculations are performed in SI units for consistency.

### 3. Physical Relationship Contracts
Physical relationships are defined through explicit operator interfaces that ensure type safety at compile time:

```csharp
// Length can multiply with itself to create Area, and divide by Time to create Velocity
public readonly struct Length<T> : IPhysicalQuantity<T>, 
    IMultiplyOperator<Length<T>, Length<T>, Area<T>>,     // Length × Length → Area
    IDivideOperator<Length<T>, Time<T>, Velocity<T>>,     // Length ÷ Time → Velocity
    IAddOperator<Length<T>>,                              // Length + Length → Length
    IComparisonOperator<Length<T>>                        // Length comparison operators
    where T : struct, INumber<T>
{
    // Implementation ensures compile-time type safety
    public static Area<T> operator *(Length<T> left, Length<T> right)
        => new Area<T>(left.Value * right.Value);
    
    public static Velocity<T> operator /(Length<T> left, Time<T> right)
        => new Velocity<T>(left.Value / right.Value);
}

// Force demonstrates multiple dimensional relationships
public readonly struct Force<T> : IPhysicalQuantity<T>,
    IDivideOperator<Force<T>, Mass<T>, Acceleration<T>>,    // Force ÷ Mass → Acceleration
    IDivideOperator<Force<T>, Area<T>, Pressure<T>>,        // Force ÷ Area → Pressure  
    IMultiplyOperator<Force<T>, Length<T>, Energy<T>>,      // Force × Length → Energy
    IAddOperator<Force<T>>,                                 // Force + Force → Force
    IComparisonOperator<Force<T>>                           // Force comparison operators
    where T : struct, INumber<T>
{
    // Only valid operations are available - compile-time safety guaranteed
}
```

### 4. Arithmetic Operations Within Same Dimension
```csharp
// Arithmetic within same dimension (generic storage type)
Length<T> a = 10.Meters<T>();
Length<T> b = 5.Meters<T>();
Length<T> sum = a + b;        // Addition
Length<T> diff = a - b;       // Subtraction
Length<T> scaled = a * T.CreateChecked(2);     // Scalar multiplication

// Comparison operators
bool isGreater = length1 > length2;
bool isEqual = length1 == length2;
```

### 5. Fluent Extension Methods
```csharp
public static class NumericExtensions
{
    public static Length<T> Meters<T>(this T value) where T : struct, INumber<T>
        => new Length<T>(value);
    
    public static Length<T> Feet<T>(this T value) where T : struct, INumber<T>
        => new Length<T>(value * T.CreateChecked(0.3048));
    
    public static Length<T> Kilometers<T>(this T value) where T : struct, INumber<T>
        => new Length<T>(value * T.CreateChecked(1000));
    
    public static Mass<T> Kilograms<T>(this T value) where T : struct, INumber<T>
        => new Mass<T>(value);
    
    public static Mass<T> Grams<T>(this T value) where T : struct, INumber<T>
        => new Mass<T>(value * T.CreateChecked(0.001));
    
    public static Mass<T> Pounds<T>(this T value) where T : struct, INumber<T>
        => new Mass<T>(value * T.CreateChecked(0.453592));
    
    public static Time<T> Seconds<T>(this T value) where T : struct, INumber<T>
        => new Time<T>(value);
    
    public static Time<T> Minutes<T>(this T value) where T : struct, INumber<T>
        => new Time<T>(value * T.CreateChecked(60));
    
    public static Time<T> Hours<T>(this T value) where T : struct, INumber<T>
        => new Time<T>(value * T.CreateChecked(3600));
    
    // Usage examples demonstrating relationship contracts:
    // var area = 10.Meters<double>() * 5.Meters<double>();          // Returns Area<double> via IIntegralOperators
    // var velocity = 100.Meters<float>() / 10.Seconds<float>();     // Returns Velocity<float> via IDerivativeOperators
    // var force = 2.Kilograms<decimal>() * 9.8m.MetersPerSecondSquared<decimal>(); // Returns Force<decimal>
}
```

## Enhanced Type Safety

### Compile-Time Relationship Validation
```csharp
// Properly constrained operator implementations with compile-time validation
public readonly struct Length<T> : IPhysicalQuantity<T>, 
    IMultiplyOperator<Length<T>, Length<T>, Area<T>>,     // Correct: TSelf, TOther, TResult
    IDivideOperator<Length<T>, Time<T>, Velocity<T>>,     // Correct: TSelf, TOther, TResult
    IAddOperator<Length<T>>,                              // Same dimension operations
    IComparisonOperator<Length<T>>                        // Comparison operations
    where T : struct, INumber<T>
{
    private readonly T _value;
    
    public Length(T value) => _value = ValidationHelper.ValidateFinite(value);
    
    public T Value => _value;
    public string Dimension => "Length [L]";
    public IUnit BaseUnit => Units.Meter;
    public bool IsPhysicallyValid => T.IsFinite(_value) && _value >= T.Zero;
    
    // Correctly typed operator implementations
    public static Area<T> operator *(Length<T> left, Length<T> right)
        => new Area<T>(left._value * right._value);
    
    public static Velocity<T> operator /(Length<T> left, Time<T> right)
        => new Velocity<T>(left._value / right.Value);
    
    public static Length<T> operator +(Length<T> left, Length<T> right)
        => new Length<T>(left._value + right._value);
    
    public static Length<T> operator -(Length<T> left, Length<T> right)
        => new Length<T>(left._value - right._value);
        
    // Comparison operators
    public static bool operator >(Length<T> left, Length<T> right)
        => left._value > right._value;
    
    public static bool operator <(Length<T> left, Length<T> right)
        => left._value < right._value;
        
    // Compile-time safety: Invalid operations like Length + Mass are impossible
    // The type system prevents such operations from compiling
}
```

### Runtime Dimensional Validation
```csharp
public class DimensionalValidator : IDimensionalValidator
{
    private readonly Dictionary<string, DimensionSignature> _knownDimensions;
    
    public bool ValidateOperation<TLeft, TRight, TResult>(string operation)
        where TLeft : IPhysicalQuantity<T>
        where TRight : IPhysicalQuantity<T> 
        where TResult : IPhysicalQuantity<T>
    {
        var leftDim = ParseDimension(typeof(TLeft));
        var rightDim = ParseDimension(typeof(TRight));
        var resultDim = ParseDimension(typeof(TResult));
        
        var expectedResult = operation switch
        {
            "*" => leftDim + rightDim,
            "/" => leftDim - rightDim,
            "+" or "-" => leftDim == rightDim ? leftDim : null,
            _ => throw new ArgumentException($"Unknown operation: {operation}")
        };
        
        return expectedResult?.Equals(resultDim) == true;
    }
}

// Dimensional signature for validation
public record DimensionSignature(
    int Length = 0, int Mass = 0, int Time = 0, int Current = 0,
    int Temperature = 0, int Substance = 0, int Luminosity = 0)
{
    public static DimensionSignature operator +(DimensionSignature left, DimensionSignature right)
        => new(left.Length + right.Length, left.Mass + right.Mass, /* ... */);
        
    public static DimensionSignature operator -(DimensionSignature left, DimensionSignature right)
        => new(left.Length - right.Length, left.Mass - right.Mass, /* ... */);
}
```

## Comprehensive Error Handling

### Robust Error Types
```csharp
public abstract class PhysicalQuantityException : Exception
{
    protected PhysicalQuantityException(string message) : base(message) { }
    protected PhysicalQuantityException(string message, Exception innerException) : base(message, innerException) { }
}

public class DimensionalMismatchException : PhysicalQuantityException
{
    public string LeftDimension { get; }
    public string RightDimension { get; }
    public string Operation { get; }
    
    public DimensionalMismatchException(string operation, string leftDim, string rightDim)
        : base($"Cannot perform {operation} between {leftDim} and {rightDim}")
    {
        Operation = operation;
        LeftDimension = leftDim;
        RightDimension = rightDim;
    }
}

public class UnitConversionException : PhysicalQuantityException
{
    public IUnit SourceUnit { get; }
    public IUnit TargetUnit { get; }
    
    public UnitConversionException(IUnit source, IUnit target, string reason)
        : base($"Cannot convert from {source.Symbol} to {target.Symbol}: {reason}")
    {
        SourceUnit = source;
        TargetUnit = target;
    }
}

public class PhysicalConstraintViolationException : PhysicalQuantityException
{
    public object Value { get; }
    public string Constraint { get; }
    
    public PhysicalConstraintViolationException(object value, string constraint)
        : base($"Value {value} violates physical constraint: {constraint}")
    {
        Value = value;
        Constraint = constraint;
    }
}
```

### Safe Operation Patterns
```csharp
// Result pattern for operations that might fail
public readonly struct OperationResult<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public PhysicalQuantityException? Error { get; }
    
    private OperationResult(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }
    
    private OperationResult(PhysicalQuantityException error)
    {
        IsSuccess = false;
        Value = default!;
        Error = error;
    }
    
    public static OperationResult<T> Success(T value) => new(value);
    public static OperationResult<T> Failure(PhysicalQuantityException error) => new(error);
}

// Safe operations
public static class SafeOperations
{
    public static OperationResult<Velocity<T>> TryDivide<T>(Length<T> distance, Time<T> time)
        where T : struct, INumber<T>
    {
        try
        {
            if (time.Value == T.Zero)
                return OperationResult<Velocity<T>>.Failure(
                    new PhysicalConstraintViolationException(time.Value, "Time cannot be zero"));
                    
            return OperationResult<Velocity<T>>.Success(distance / time);
        }
        catch (Exception ex)
        {
            return OperationResult<Velocity<T>>.Failure(new PhysicalQuantityException("Division failed", ex));
        }
    }
}
```

## Performance Optimizations

### Optimized Readonly Structs
```csharp
[StructLayout(LayoutKind.Auto)]
public readonly struct Length<T> : IPhysicalQuantity<T>
    where T : struct, INumber<T>
{
    private readonly T _value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Length(T value) => _value = value;
    
    public T Value 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Length<T> operator +(Length<T> left, Length<T> right)
        => new(left._value + right._value);
        
    // Ref return for zero-copy access in performance-critical scenarios
    public ref readonly T ValueRef => ref _value;
}
```

### Bulk Operations Support
```csharp
public static class BulkOperations
{
    // SIMD-optimized bulk conversions
    public static void ConvertUnits<T>(ReadOnlySpan<Length<T>> source, 
                                      Span<Length<T>> destination,
                                      T conversionFactor) 
        where T : struct, INumber<T>
    {
        // Can be vectorized by JIT
        for (int i = 0; i < source.Length; i++)
        {
            destination[i] = new Length<T>(source[i].Value * conversionFactor);
        }
    }
    
    // Parallel processing for large datasets
    public static async Task<Area<T>[]> CalculateAreasAsync<T>(Length<T>[] widths, Length<T>[] heights)
        where T : struct, INumber<T>
    {
        return await Task.Run(() =>
            widths.AsParallel()
                  .Zip(heights.AsParallel())
                  .Select(pair => pair.First * pair.Second)
                  .ToArray());
    }
}
```

## Testing Strategy

### Comprehensive Test Categories
```csharp
[TestFixture]
public class LengthTests
{
    [Test]
    [TestCase(10.0, ExpectedResult = 10.0)]
    [TestCase(0.0, ExpectedResult = 0.0)]
    [TestCase(double.MaxValue, ExpectedResult = double.MaxValue)]
    public double Constructor_ValidValues_StoresCorrectly(double value)
    {
        var length = new Length<double>(value);
        return length.Value;
    }
    
    [Test]
    public void Constructor_NaN_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Length<double>(double.NaN));
    }
    
    [Test]
    public void Multiply_TwoLengths_ReturnsCorrectArea()
    {
        var length1 = new Length<double>(5.0);
        var length2 = new Length<double>(4.0);
        
        var area = length1 * length2;
        
        Assert.AreEqual(20.0, area.Value);
        Assert.AreEqual("Area [L²]", area.Dimension);
    }
    
    [Test]
    public void Conversion_MetersToFeet_WithinTolerance()
    {
        var meters = 10.0.Meters();
        var feet = meters.In(Units.Foot);
        
        Assert.AreEqual(32.8084, feet, 0.0001);
    }
    
    [Test]
    public void Addition_DifferentStorageTypes_CompileError()
    {
        // This should not compile
        // var length1 = new Length<double>(5.0);
        // var length2 = new Length<float>(3.0f);
        // var sum = length1 + length2; // Compile error!
    }
}

[TestFixture]
public class DimensionalValidationTests
{
    [Test]
    public void ValidateOperation_LengthTimesLength_ReturnsTrue()
    {
        var validator = new DimensionalValidator();
        
        var isValid = validator.ValidateOperation<Length<double>, Length<double>, Area<double>>("*");
        
        Assert.IsTrue(isValid);
    }
    
    [Test]
    public void ValidateOperation_LengthPlusMass_ReturnsFalse()
    {
        var validator = new DimensionalValidator();
        
        // This would be a compile error anyway, but test the validator
        var isValid = validator.ValidateOperation<Length<double>, Mass<double>, object>("+" );
        
        Assert.IsFalse(isValid);
    }
}
```

## Improved Configuration System

### Better Dependency Injection Pattern
```csharp
// Remove static service provider anti-pattern
public interface IPhysicalQuantityFactory
{
    ILength CreateLength(double value, IUnit? unit = null);
    IMass CreateMass(double value, IUnit? unit = null);
    ITime CreateTime(double value, IUnit? unit = null);
    TQuantity Create<TQuantity>(double value, IUnit? unit = null) 
        where TQuantity : IPhysicalQuantity<T>;
}

// Configuration through options pattern
public class PhysicalQuantityOptions
{
    public Type StorageType { get; set; } = typeof(double);
    public bool ValidatePhysicalConstraints { get; set; } = true;
    public bool EnableUncertaintyPropagation { get; set; } = false;
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    public IUnit DefaultLengthUnit { get; set; } = Units.Meter;
    public IUnit DefaultMassUnit { get; set; } = Units.Kilogram;
    // ... default units for each dimension
}

// Clean service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhysicalQuantities(
        this IServiceCollection services,
        Action<PhysicalQuantityOptions>? configure = null)
    {
        services.Configure(configure ?? (_ => { }));
        services.AddSingleton<IPhysicalQuantityFactory, PhysicalQuantityFactory>();
        services.AddSingleton<IUnitRegistry, UnitRegistry>();
        services.AddSingleton<IDimensionalValidator, DimensionalValidator>();
        return services;
    }
}
```

### Intuitive Extension Methods
```csharp
// Clean, type-safe extension methods with consistent API
public static class NumericExtensions
{
    // Primary API - uses configured storage type, clean syntax
    public static ILength Meters(this double value) => CreateLength(value, Units.Meter);
    public static ILength Feet(this double value) => CreateLength(value, Units.Foot);
    public static ILength Kilometers(this double value) => CreateLength(value, Units.Kilometer);
    
    public static IMass Kilograms(this double value) => CreateMass(value, Units.Kilogram);
    public static IMass Grams(this double value) => CreateMass(value, Units.Gram);
    public static IMass Pounds(this double value) => CreateMass(value, Units.Pound);
    
    public static ITime Seconds(this double value) => CreateTime(value, Units.Second);
    public static ITime Minutes(this double value) => CreateTime(value, Units.Minute);
    public static ITime Hours(this double value) => CreateTime(value, Units.Hour);
    
    // Explicit generic versions for specific precision requirements
    public static Length<T> Meters<T>(this T value) where T : struct, INumber<T>
        => new Length<T>(ConvertToSI(value, Units.Meter));
    
    public static Mass<T> Kilograms<T>(this T value) where T : struct, INumber<T>
        => new Mass<T>(ConvertToSI(value, Units.Kilogram));
    
    public static Time<T> Seconds<T>(this T value) where T : struct, INumber<T>
        => new Time<T>(ConvertToSI(value, Units.Second));
    
    // Type conversions for seamless API
    public static ILength Meters(this float value) => ((double)value).Meters();
    public static ILength Meters(this decimal value) => ((double)value).Meters();
    public static ILength Meters(this int value) => ((double)value).Meters();
    
    // Helper methods use DI when available, fallback to double
    private static ILength CreateLength(double value, IUnit unit) =>
        GetFactory()?.CreateLength(value, unit) ?? new Length<double>(ConvertToSI(value, unit));
        
    private static IPhysicalQuantityFactory? GetFactory() =>
        // Resolve from DI container or return null for fallback
        ServiceLocator.Current?.GetService<IPhysicalQuantityFactory>();
}

// Clean usage with dependency injection
public class PhysicsCalculator
{
    private readonly IPhysicalQuantityFactory _factory;
    
    public PhysicsCalculator(IPhysicalQuantityFactory factory)
    {
        _factory = factory;
    }
    
    public IEnergy CalculateKineticEnergy(IMass mass, IVelocity velocity)
    {
        // Clean API: E = ½mv²
        var half = 0.5.Scalar();  // Uses scalar extension
        return half * mass * velocity * velocity;  // Type-safe operations
    }
    
    public IPressure CalculatePressure(IForce force, IArea area)
    {
        // Clean API: P = F/A  
        return force / area;  // Compile-time validated: Force ÷ Area → Pressure
    }
}
```

## Configuration System

An application using the library should be able to configure which data type to use as the storage type for the quantities, eliminating the need to specify the storage type at every call site.

### Global Storage Type Configuration

#### Primary Approach: Dependency Injection with Default Storage Type
```csharp
// Configuration options for physical quantities
public class PhysicalQuantityOptions
{
    public Type DefaultStorageType { get; set; } = typeof(double); // double is default
    public bool ValidateRanges { get; set; } = true;
    public bool EnableDebugMode { get; set; } = false;
    public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;
    public Dictionary<string, object> UnitSystemPreferences { get; set; } = new();
}

// DI-aware context for creating quantities with configured storage type
public interface IPhysicalQuantityContext
{
    ILength CreateLength(double value);
    IMass CreateMass(double value);
    ITime CreateTime(double value);
    IForce CreateForce(double value);
    IArea CreateArea(double value);
    IVelocity CreateVelocity(double value);
    // ... all quantity types
}

// Generic implementation that can be configured for any storage type T
public class PhysicalQuantityContext<T> : IPhysicalQuantityContext
    where T : struct, INumber<T>
{
    public ILength CreateLength(double value) => new Length<T>(T.CreateChecked(value));
    public IMass CreateMass(double value) => new Mass<T>(T.CreateChecked(value));
    public ITime CreateTime(double value) => new Time<T>(T.CreateChecked(value));
    public IForce CreateForce(double value) => new Force<T>(T.CreateChecked(value));
    public IArea CreateArea(double value) => new Area<T>(T.CreateChecked(value));
    public IVelocity CreateVelocity(double value) => new Velocity<T>(T.CreateChecked(value));
    // ... implement all quantity types
}

// Extension methods that use DI to get the configured context
public static class NumericExtensions
{
    private static IServiceProvider? _serviceProvider;
    
    // Called by the DI container during startup
    internal static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    // Clean extension methods that use configured storage type
    public static ILength Meters(this double value)
    {
        var context = GetContext();
        return context.CreateLength(value);
    }
    
    public static ILength Meters(this float value) => ((double)value).Meters();
    public static ILength Meters(this decimal value) => ((double)value).Meters();
    public static ILength Meters(this int value) => ((double)value).Meters();
    
    public static IMass Kilograms(this double value)
    {
        var context = GetContext();
        return context.CreateMass(value);
    }
    
    public static ITime Seconds(this double value)
    {
        var context = GetContext();
        return context.CreateTime(value);
    }
    
    // Still allow explicit generic usage for mixed scenarios
    public static Length<T> Meters<T>(this T value) where T : struct, INumber<T>
        => new Length<T>(value);
    
    public static Mass<T> Kilograms<T>(this T value) where T : struct, INumber<T>
        => new Mass<T>(value);
    
    private static IPhysicalQuantityContext GetContext()
    {
        if (_serviceProvider == null)
        {
            // Fallback to double if DI not configured
            return new PhysicalQuantityContext<double>();
        }
        
        return _serviceProvider.GetRequiredService<IPhysicalQuantityContext>();
    }
}

// Non-generic interfaces for clean API
public interface ILength
{
    string Dimension { get; }
    double ValueAsDouble { get; }
    T GetValue<T>() where T : struct, INumber<T>;
    IArea Multiply(ILength other);
    IVelocity Divide(ITime time);
    T In<T>() where T : IUnit<ILength>, new();
}

public interface IMass
{
    string Dimension { get; }
    double ValueAsDouble { get; }
    T GetValue<T>() where T : struct, INumber<T>;
    IForce Multiply(IAcceleration acceleration);
}

public interface ITime
{
    string Dimension { get; }
    double ValueAsDouble { get; }
    T GetValue<T>() where T : struct, INumber<T>;
}

// ... other interfaces for all quantity types
```



### Usage Examples

#### Application Startup Configuration
```csharp
// Program.cs or Startup.cs

// Option 1: Default registration (uses double storage)
services.AddPhysicalQuantities(); // Defaults to double

// Option 2: Explicit storage type configuration
services.AddPhysicalQuantities<decimal>(); // High precision for financial apps
services.AddPhysicalQuantities<float>();   // Memory efficient for game engines
services.AddPhysicalQuantities<double>();  // Explicit double (same as default)

// The DI container will inject IPhysicalQuantityContext with the configured storage type
```

#### Clean Usage Without Generic Specification
```csharp
// After DI configuration, clean API without generics
var length1 = 10.Meters();        // Uses DI-configured storage type (ILength)
var length2 = 5.5.Feet();         // Uses DI-configured storage type (ILength)
var mass = 2.Kilograms();         // Uses DI-configured storage type (IMass)

var area = length1 * length2;     // Returns IArea with configured storage type
var velocity = length1 / 3.Seconds(); // Returns IVelocity with configured storage type

// Convert to specific units
var distanceInKm = length1.In<Kilometers>(); // Gets value in km using configured storage type
var speedInMph = velocity.In<MilesPerHour>(); // Gets value in mph

// Still allows explicit generic usage when needed
var preciseLength = 10.123456789m.Meters<decimal>(); // Explicit decimal - Length<decimal>
var fastLength = 10.5f.Meters<float>();              // Explicit float - Length<float>
```

#### Default Behavior Without DI Configuration
```csharp
// If AddPhysicalQuantities() is never called, extension methods fall back to double
var length = 10.Meters();     // Falls back to Length<double> internally
var mass = 2.Kilograms();     // Falls back to Mass<double> internally
var time = 5.Seconds();       // Falls back to Time<double> internally

// All operations work normally with double as the storage type
var area = length * 8.Meters();      // Returns IArea backed by Area<double>
var velocity = length / time;        // Returns IVelocity backed by Velocity<double>
```

#### Mixed Storage Type Scenarios
```csharp
// Configure DI to use double as default
services.AddPhysicalQuantities(); // or services.AddPhysicalQuantities<double>();

// Most calculations use default (double via DI)
var distance = 100.Meters();      // ILength backed by Length<double>
var time = 10.Seconds();          // ITime backed by Time<double>
var speed = distance / time;      // IVelocity backed by Velocity<double>

// Specific high-precision calculations can still use explicit generics
var preciseDistance = 100.123456789m.Meters<decimal>(); // Length<decimal>
var preciseTime = 10.987654321m.Seconds<decimal>();     // Time<decimal>
var preciseSpeed = preciseDistance / preciseTime;      // Velocity<decimal>

// Can access underlying storage type when needed
var doubleValue = distance.GetValue<double>();   // Gets the double value
var decimalValue = preciseDistance.Value;        // Gets the decimal value directly
```

## Migration Path from Legacy Design

### Phase 1: Core Infrastructure
1. Implement simplified interfaces (`IPhysicalQuantity<T>`)
2. Create basic quantity structs (Length, Mass, Time)
3. Implement unit system with validation
4. Add basic arithmetic operations

### Phase 2: Advanced Features  
1. Add compound units and conversions
2. Implement dimensional validation
3. Add uncertainty propagation
4. Create bulk operations support

### Phase 3: Integration & Polish
1. Finalize DI integration patterns
2. Add comprehensive error handling
3. Performance optimization and benchmarking
4. Documentation and samples

## Key Benefits of This Design

1. **Simplified Architecture**: Removed complex generic self-referencing
2. **Better Error Handling**: Comprehensive exception hierarchy and safe operations
3. **Robust Unit System**: Support for compound units and proper validation
4. **Cleaner DI Pattern**: Removed static service provider anti-pattern
5. **Enhanced Type Safety**: Compile-time prevention of invalid operations
6. **Better Performance**: Optimized struct layout and bulk operations
7. **Improved Testability**: Dependency injection enables better unit testing
8. **Future-Proof**: Extensible architecture for advanced features

## Future Enhancements

1. **Complex Numbers Support** - For AC electrical calculations
2. **Vector Quantities** - Support for directional quantities
3. **Matrix Operations** - For advanced physics calculations  
4. **Symbolic Math Integration** - Work with computer algebra systems
5. **Serialization Support** - JSON, XML, Binary serialization
6. **Localization** - Multi-language unit names and formatting
7. **GraphQL/OpenAPI Integration** - Schema generation for web APIs
8. **Database Integration** - Entity Framework value converters

---

This design maintains the original vision while addressing architectural concerns and providing a more robust, maintainable foundation for the physical quantities library.
