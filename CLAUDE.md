# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ktsu.Semantics is a comprehensive .NET library for creating type-safe, validated types with semantic meaning. The library consists of three main packages:

- **Semantics.Strings**: Core semantic string types with validation framework
- **Semantics.Paths**: Specialized path handling with polymorphic interfaces
- **Semantics.Quantities**: Complete physics quantities system with 80+ quantities across 8 scientific domains

## Build Commands

### Building the Solution
```powershell
dotnet build Semantics.sln
```

### Running Tests
```powershell
# Run all tests
dotnet test Semantics.Test/Semantics.Test.csproj

# Run specific test with filter
dotnet test --filter "FullyQualifiedName~SemanticStringTests"
```

### Restore Packages
```powershell
dotnet restore
```

### Clean Build Artifacts
```powershell
dotnet clean
```

## Architecture

### Core Design Patterns

1. **CRTP (Curiously Recurring Template Pattern)**: All semantic types use CRTP for type safety:
   ```csharp
   public abstract record SemanticString<TDerived> : ISemanticString
       where TDerived : SemanticString<TDerived>
   ```

2. **Bootstrap Architecture**: The physics system uses a sophisticated bootstrap pattern to resolve circular dependencies:
   - `BootstrapUnits` provides initial unit definitions during system initialization
   - `PhysicalDimensions` uses bootstrap units to define dimensions without circular dependencies
   - `Units` class replaces bootstrap units with full unit definitions after initialization

3. **Factory Pattern**: All semantic types support factory-based creation for dependency injection:
   - `ISemanticStringFactory<T>` interface
   - `SemanticStringFactory<T>` implementation

### Project Structure

#### Semantics.Strings
- Core base class: `SemanticString<TDerived>`
- Factory infrastructure: `ISemanticStringFactory<T>`, `SemanticStringFactory<T>`
- Validation framework in `Validation/`:
  - `Attributes/`: 50+ built-in validation attributes
  - `Rules/`: Validation rule implementations
  - `Strategies/`: `ValidateAllStrategy`, `ValidateAnyStrategy`

#### Semantics.Paths
- Base class: `SemanticPath<TDerived>` (extends `SemanticString<TDerived>`)
- `Implementations/`: Concrete path types (AbsoluteFilePath, RelativeDirectoryPath, etc.)
- `Interfaces/`: Polymorphic path interfaces (IPath, IFilePath, IAbsolutePath, etc.)
- `Primitives/`: Core path primitives
- Path canonicalization normalizes directory separators and removes trailing separators

#### Semantics.Quantities
- Core base classes:
  - `SemanticQuantity<TStorage>`: Base quantity with storage type
  - `PhysicalQuantity<TSelf, T>`: Physics quantities with dimensional analysis
- `Core/`: Physical dimension system, units, and constants
  - `PhysicalDimension`: Dimensional analysis with SI base quantities
  - `PhysicalConstants`: Centralized physical constants with type-safe generic access
  - `BootstrapUnits` and `Units`: Bootstrap architecture for circular dependency resolution
- Domain-specific quantities in subdirectories:
  - `Mechanics/`: Length, Mass, Force, Energy, Velocity, etc. (15 quantities)
  - `Electrical/`: Voltage, Current, Resistance, Capacitance, etc. (11 quantities)
  - `Thermal/`: Temperature, Heat, Entropy, Thermal Conductivity, etc. (10 quantities)
  - `Chemical/`: Concentration, pH, Molar Mass, Reaction Rate, etc. (10 quantities)
  - `Acoustic/`: Frequency, Sound Intensity, Wavelength, etc. (20 quantities)
  - `Nuclear/`: Radioactive Activity, Absorbed Dose, etc. (5 quantities)
  - `Optical/`: Luminous Flux, Illuminance, Refractive Index, etc. (6 quantities)
  - `FluidDynamics/`: Viscosity, Flow Rate, Reynolds Number, etc. (5 quantities)

### Key Architectural Concepts

1. **Type Safety Through Generics**: The library extensively uses generic type parameters and constraints to enforce compile-time type safety.

2. **Validation Architecture**:
   - Attribute-based validation using reflection
   - Strategy pattern for combining validation rules (`ValidateAll` vs `ValidateAny`)
   - Validation occurs in `MakeCanonical()` before object construction

3. **Canonicalization**: `MakeCanonical(string input)` method provides a hook for derived types to normalize input before validation (e.g., path separator normalization).

4. **WeakString Property**: The underlying string value is exposed via `WeakString` property to enable interoperability with regular string APIs while maintaining type safety.

5. **Physical Constants System**: All constants are centralized in `PhysicalConstants` with derived constants validated against fundamental relationships in tests.

## Code Style Requirements

### File Headers
All C# files must include this copyright header:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.
```

### Code Style
- **Indentation**: Use tabs (tab_width = 4) for C# files
- **Namespaces**: File-scoped namespaces (`csharp_style_namespace_declarations = file_scoped:error`)
- **Usings**: Inside namespace (`csharp_using_directive_placement = inside_namespace:error`)
- **Braces**: Always use braces (`csharp_prefer_braces = true:error`)
- **Expression bodies**: When on single line (`when_on_single_line:error`)
- **Primary constructors**: Preferred (`csharp_style_prefer_primary_constructors = true:error`)
- **No top-level statements**: (`csharp_style_prefer_top_level_statements = false:error`)

### Warning Suppressions
- **Never add global suppressions** for warnings in project properties
- Use explicit `[SuppressMessage]` attributes with justifications when needed
- Fallback to `#pragma warning disable` preprocessor directives with comment justifications only if attributes are unavailable
- Make suppressions as targeted as possible (smallest scope)

### Common Suppressions in Project
Some projects suppress specific warnings in `.csproj`:
- CA1866: Use 'string.Method(char)' instead of 'string.Method(string)'
- CA2249: Consider using String.Contains instead of String.IndexOf
- IDE0057: Substring can be simplified

## Target Frameworks

The library supports multiple target frameworks for maximum compatibility:
- .NET 9.0, 8.0, 7.0, 6.0, 5.0
- .NET Standard 2.0, 2.1

Use conditional compilation for framework-specific features:
```csharp
#if NET5_0_OR_GREATER
    // .NET 5+ specific code
#else
    // Fallback for older frameworks
#endif
```

## Testing

- Test project: `Semantics.Test/`
- Uses MSTest framework with MSTest.Sdk
- Target framework: net9.0 only
- Tests validate:
  - All validation attributes and rules
  - Physics quantity relationships and dimensional analysis
  - Derived physical constants match calculated values
  - Path operations and file system interactions
  - Factory pattern and dependency injection
  - Performance characteristics

## SDK and Build System

- Uses custom `ktsu.Sdk` MSBuild SDK (version 1.75.0)
- Central package management enabled via `Directory.Packages.props`
- CI/CD pipeline managed by PowerShell module `scripts/PSBuild.psm1`
- GitHub Actions workflow in `.github/workflows/dotnet.yml` handles:
  - Building and testing
  - SonarQube analysis
  - NuGet package publishing
  - Release creation with changelog management
  - Winget manifest updates

## Important Implementation Notes

1. **Physics Relationships**: When implementing new physics quantities, ensure dimensional analysis is correct and operators return proper types based on dimensional multiplication/division.

2. **Unit Conversions**: Use `In(IUnit targetUnit)` method for unit conversions. Handle both linear units (most common) and offset units (like temperature conversions).

3. **Path Canonicalization**: Paths automatically normalize directory separators to platform-specific separators and remove trailing separators (except for root paths).

4. **Factory Methods**: Semantic types support multiple creation patterns:
   - Direct: `EmailAddress.Create("user@example.com")`
   - Factory: `factory.Create("user@example.com")`
   - Casting: `(EmailAddress)"user@example.com"`
   - Safe: `EmailAddress.TryCreate("maybe@invalid", out var result)`

5. **Validation Strategies**: Use `[ValidateAll]` (default) for AND logic or `[ValidateAny]` for OR logic when combining multiple validation attributes.

6. **Physical Constants**: Access constants via `PhysicalConstants.Generic` with type parameter for storage type, e.g., `PhysicalConstants.Generic.SpeedOfLight<double>()`.
