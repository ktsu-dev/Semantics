# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

This is a .NET C# library project that uses custom MSBuild SDKs from ktsu. Common development commands:

- **Build**: `dotnet build`
- **Test**: `dotnet test` (runs all unit tests with coverage)
- **Test (no parallel)**: `dotnet test --logger "console;verbosity=detailed"`
- **Clean**: `dotnet clean`
- **Restore**: `dotnet restore`
- **Format**: `dotnet format` (fixes formatting issues like IDE0055)

The project uses MSTest for unit testing with comprehensive coverage across all physics domains.

## Project Architecture

### Core Components

**ktsu.Semantics** is a comprehensive .NET library for creating type-safe, validated types with semantic meaning, encompassing three major areas:

1. **Semantic Strings** - Strongly-typed string wrappers with validation
2. **Semantic Paths** - Specialized file system path handling with polymorphic interfaces  
3. **Physics Quantities System** - Complete physics quantities across 8 scientific domains

### Key Architectural Patterns

- **Bootstrap Architecture**: Resolves circular dependencies between units, dimensions, and constants using `BootstrapUnits` class
- **Generic Type Safety**: All physics quantities use generic constraints `where T : struct, INumber<T>`
- **Factory Pattern**: `SemanticStringFactory<T>` for dependency injection scenarios
- **Polymorphic Path Interfaces**: Rich interface hierarchy (`IPath`, `IFilePath`, `IDirectoryPath`, etc.)

### Physics System Structure

**8 Complete Domains** (80+ quantities total):
- Mechanics (15): Force, Energy, Power, Pressure, Velocity, etc.
- Electrical (11): Voltage, Current, Resistance, Capacitance, etc.
- Thermal (10): Temperature, Heat, Entropy, Thermal Conductivity, etc.
- Chemical (10): AmountOfSubstance, Concentration, pH, Reaction rates, etc.
- Acoustic (20): Frequency, Sound pressure/power, Wavelength, etc.
- Nuclear (5): Radioactive activity, Absorbed dose, Exposure, etc.
- Optical (6): Luminous flux, Illuminance, Refractive index, etc.
- Fluid Dynamics (5): Viscosity, Flow rates, Reynolds number, etc.

### Physical Constants System

Centralized in `PhysicalConstants` class with type-safe generic access:
- `PhysicalConstants.Generic.SpeedOfLight<T>()`
- `PhysicalConstants.Generic.PlanckConstant<T>()`  
- `PhysicalConstants.Conversion.FeetToMeters<T>()`

All derived constants are validated against fundamental relationships in comprehensive unit tests.

## Code Standards and Guidelines

### File Headers
Always include this header on new files:
```csharp
// Copyright (c) KTSU. All rights reserved.
```

### Physics Quantities Standards
- Use `PhysicalConstants.Generic` methods instead of hardcoded values
- Implement physics relationships as operators with dimensional analysis
- Suppress CA2225 warnings for physics operators: 
```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
```

### Validation and Error Handling
- Throw `ArgumentException` for validation failures (not `FormatException`)
- Use specific exception types instead of general exceptions
- Temperature values cannot be below absolute zero (0 K)
- Frequency values cannot be negative
- Throw `DivideByZeroException` when dividing by zero in `DivideToStorage`

### Testing Standards
- Use explicit types instead of `var`
- Pre-create objects outside measurement loops in performance tests
- Mark OS-specific tests with `[TestCategory("OS-Specific")]`
- Use path length limit of 259 characters for cross-platform compatibility
- Force GC before memory measurements: `GC.Collect(); GC.WaitForPendingFinalizers()`

### XML Documentation Standards
- Use explicit dimension documentation: `/// <summary>Gets the physical dimension of [quantity] [SYMBOL].</summary>`
- Constructor documentation: `/// <summary>Initializes a new instance of the <see cref="[ClassName]{T}"/> class.</summary>`
- Include `<param>`, `<returns>`, `<exception>`, and `<see cref="">` tags appropriately

## Important Implementation Notes

### Semantic String Creation
```csharp
// Preferred creation methods
var email = EmailAddress.Create("user@example.com");
var userId = UserId.Create("USER_123");

// Extension method conversion
var email = "user@example.com".As<EmailAddress>();

// Cross-type conversion
var converted = sourceString.As<SourceType, TargetType>();
```

### Path Interface Usage
Complete conversion API:
- `AsAbsolute()` - Convert to absolute using current working directory
- `AsAbsolute(baseDirectory)` - Convert to absolute using specific base  
- `AsRelative(baseDirectory)` - Convert to relative using specific base

### Physics Relationships Implementation
Use `.Value` property directly for calculations since quantities are already in SI base units:
```csharp
// Force * Length = Energy (Work)
public static Energy<T> operator *(Force<T> force, Length<T> length) =>
    Energy<T>.FromJoules(force.Value * length.Value);
```

### Bootstrap vs Regular Units
- Use `BootstrapUnits` in `PhysicalDimensions.cs` to avoid circular dependencies
- Replace with full `Units` class after system initialization
- Keep bootstrap units in dedicated `BootstrapUnits` class

This architecture enables a sophisticated type-safe physics system while maintaining clean separation of concerns and avoiding circular dependencies through the bootstrap pattern.