# Complete Semantics Library Guide

This document provides a comprehensive overview of all features and components in the ktsu.Semantics library.

## Table of Contents

- [Core Components](#core-components)
- [Semantic Strings](#semantic-strings)
- [Semantic Quantities](#semantic-quantities)
- [Path System](#path-system)
- [Validation System](#validation-system)
- [Performance Features](#performance-features)

## Core Components

The Semantics library consists of six main areas:

1. **Semantic Strings** - Type-safe string wrappers with validation
2. **Physics Quantities System** - Complete physics library with 80+ quantities across 8 domains
3. **Physical Constants** - Centralized, type-safe access to fundamental and derived constants
4. **Path System** - Comprehensive file system path handling
5. **Validation System** - 50+ validation attributes across multiple categories
6. **Performance Utilities** - Optimizations for high-performance scenarios

## Semantic Strings

Transform primitive string obsession into strongly-typed domain models:

```csharp
[IsEmail]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[HasLength(8, 50), IsNotEmpty]
public sealed record UserId : SemanticString<UserId> { }

// Usage with factory pattern
var emailFactory = new SemanticStringFactory<EmailAddress>();
var email = emailFactory.Create("user@example.com");

// Compile-time safety
public void SendEmail(EmailAddress to, UserId userId) { /* ... */ }
// SendEmail(userId, email); // ❌ Won't compile!
```

## Physics Quantities System

A comprehensive physics library with **80+ quantities** across **8 scientific domains** featuring:

- **Type-safe arithmetic** with dimensional analysis
- **Automatic unit conversions** with compile-time safety  
- **Physics relationships** as operators (F = ma, E = mc², etc.)
- **Physical constants** integrated throughout
- **Generic numeric types** (double, float, decimal) support

### Complete Domain Coverage

#### 🔧 Mechanics (15 quantities)
Position, velocity, acceleration, force, pressure, energy, power, momentum, torque, angular velocity, angular acceleration, moment of inertia, density, and more.

#### ⚡ Electrical (11 quantities)  
Voltage, current, resistance, power, charge, capacitance, inductance, electric field, magnetic field, and electrical properties.

#### 🌡️ Thermal (10 quantities)
Temperature, heat, entropy, thermal conductivity, heat capacity, thermal expansion, and thermodynamic properties.

#### 🧪 Chemical (10 quantities)
Amount of substance, molarity, reaction rates, pH, molar mass, activation energy, and chemical kinetics.

#### 🔊 Acoustic (20 quantities)
Sound pressure, intensity, frequency, wavelength, acoustic impedance, loudness, pitch, and audio metrics.

#### ☢️ Nuclear (5 quantities)
Radioactive activity, absorbed dose, equivalent dose, exposure, and nuclear cross-sections.

#### 💡 Optical (6 quantities)
Luminous intensity, flux, illuminance, luminance, refractive index, and optical power.

#### 🌊 Fluid Dynamics (5 quantities)
Viscosity, flow rates, Reynolds numbers, bulk modulus, and fluid properties.

### Usage Examples

```csharp
// Create quantities with dimensional safety
var force = Force<double>.FromNewtons(100.0);
var distance = Length<double>.FromMeters(5.0);
var time = Time<double>.FromSeconds(2.0);

// Physics relationships as operators
var work = force * distance;                    // W = F⋅d (Energy)
var power = work / time;                        // P = W/t (Power)
var velocity = distance / time;                 // v = d/t (Velocity)

// Automatic unit conversions
Console.WriteLine(work.ToKilowattHours());      // 1.389e-7 kWh
Console.WriteLine(power.ToHorsepower());        // 6.705e-5 hp

// Type safety prevents errors
// var invalid = force + time;                  // ❌ Compiler error!

// Complex calculations with multiple domains
var temp = Temperature<double>.FromCelsius(25.0);
var pressure = Pressure<double>.FromPascals(101325.0);
var volume = Volume<double>.FromLiters(22.4);
var gasConstant = PhysicalConstants.Generic.GasConstant<double>();

// Ideal gas law: PV = nRT
var moles = (pressure * volume) / (gasConstant * temp);
```

## Physical Constants

Centralized, type-safe access to **100+ physical constants** across all domains:

```csharp
// Fundamental constants (CODATA 2018)
var c = PhysicalConstants.Generic.SpeedOfLight<double>();        // 299,792,458 m/s
var h = PhysicalConstants.Generic.PlanckConstant<double>();      // 6.626070×10⁻³⁴ J⋅s
var Na = PhysicalConstants.Generic.AvogadroNumber<double>();     // 6.022140×10²³ mol⁻¹

// Derived constants with automatic type conversion
var g = PhysicalConstants.Generic.StandardGravity<float>();     // 9.80665 m/s²
var R = PhysicalConstants.Generic.GasConstant<decimal>();       // 8.314462618 J/(mol⋅K)

// Domain-specific constants
var rho = PhysicalConstants.Generic.StandardAirDensity<double>(); // 1.225 kg/m³
var c_sound = PhysicalConstants.Generic.SoundSpeedInAir<double>(); // 343 m/s
```

## Path System

Comprehensive polymorphic path handling with 11 different path types:

### Interface Hierarchy
```
IPath (base)
├── IAbsolutePath : IPath
├── IRelativePath : IPath
├── IFilePath : IPath
├── IDirectoryPath : IPath
├── IAbsoluteFilePath : IFilePath, IAbsolutePath
├── IRelativeFilePath : IFilePath, IRelativePath
├── IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
└── IRelativeDirectoryPath : IDirectoryPath, IRelativePath

IFileName / IFileExtension (separate hierarchies)
```

### Usage Example
```csharp
var filePath = AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\app\config.json");

// Rich path operations
Console.WriteLine(filePath.FileName);        // config.json
Console.WriteLine(filePath.FileExtension);   // .json
Console.WriteLine(filePath.DirectoryPath);   // C:\app
Console.WriteLine(filePath.Exists);          // True/False

// Polymorphic collections
List<IPath> paths = [
    AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\data.txt"),
    RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"temp\logs")
];

var files = paths.OfType<IFilePath>().ToList();
```

## Validation System

The library includes 50+ validation attributes across multiple categories:

### Text Validation
- `IsEmailAddress` - Email format validation
- `RegexMatch(pattern)` - Custom regex patterns
- `StartsWith` / `EndsWith` - Prefix/suffix validation
- `Contains` - Substring validation
- `IsBase64` - Base64 encoding validation

### Format Validation  
- `IsEmptyOrWhitespace` - Empty/whitespace validation
- `HasNonWhitespaceContent` - Non-whitespace requirement
- `IsSingleLine` / `IsMultiLine` - Line count validation
- `HasExactLines` / `HasMinimumLines` / `HasMaximumLines` - Line counts

### First-Class Type Validation
- `IsBoolean` - Boolean representation validation
- `IsDateTime` - Date/time format validation  
- `IsDecimal` / `IsDouble` / `IsInt32` - Numeric validation
- `IsGuid` - GUID format validation
- `IsIpAddress` - IP address validation
- `IsTimeSpan` - Time span validation
- `IsUri` - URI format validation
- `IsVersion` - Version string validation

### Quantity Validation
- `IsPositive` / `IsNegative` - Sign validation
- `IsInRange(min, max)` - Value range validation

### Path Validation
- `IsPath` - Path format validation
- `IsAbsolutePath` / `IsRelativePath` - Path type validation
- `IsFilePath` / `IsDirectoryPath` - Path category validation
- `DoesExist` - File system existence validation

### Validation Strategies
```csharp
// All must pass (default)
[ValidateAll]
[IsNotEmpty, IsEmail, HasLength(5, 100)]
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }

// Any can pass
[ValidateAny]
[IsEmail, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

## Performance Features

The library is optimized for high-performance scenarios:

- **Span-based Operations** - Minimal memory allocations
- **Pooled String Builders** - Reused StringBuilder instances
- **Interned Path Strings** - Memory optimization for common paths
- **Zero-cost Conversions** - Efficient implicit conversions
- **Lazy Validation** - Validation only when needed

### Performance Utilities
```csharp
// Pooled string builder for high-performance string operations
public class PooledStringBuilder : IDisposable

// Interned strings for common paths  
public static class InternedPathStrings

// Span-based path utilities for minimal allocations
public static class SpanPathUtilities
```

## Integration Examples

### Dependency Injection
```csharp
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();

public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;
    
    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }
}
```

### Entity Framework
```csharp
modelBuilder.Entity<User>()
    .Property(u => u.Email)
    .HasConversion(
        email => email.ToString(),
        value => EmailAddress.FromString<EmailAddress>(value));
```

### ASP.NET Core
```csharp
[HttpPost]
public IActionResult CreateUser([FromBody] CreateUserRequest request)
{
    if (!_emailFactory.TryCreate(request.Email, out var email))
    {
        return BadRequest("Invalid email format");
    }
    
    var user = new User(email);
    return Ok(user);
}
```

This library transforms primitive-obsessed code into strongly-typed, self-validating domain models with comprehensive validation, complete physics capabilities across all major scientific domains, and excellent performance characteristics. With **80+ physics quantities**, **100+ physical constants**, and **50+ validation attributes**, it provides enterprise-ready solutions for scientific computing, engineering applications, and domain modeling. 
