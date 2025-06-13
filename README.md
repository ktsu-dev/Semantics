# ktsu.Semantics

[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![Build Status](https://github.com/ktsu-dev/Semantics/workflows/CI/badge.svg)](https://github.com/ktsu-dev/Semantics/actions)

A comprehensive .NET library for creating type-safe, validated types with semantic meaning. Transform primitive string obsession into strongly-typed, self-validating domain models with comprehensive validation, specialized path handling, and a complete physics quantities system covering all major scientific domains.

## Overview

The Semantics library enables you to create strongly-typed wrappers that carry semantic meaning and built-in validation. Instead of passing raw primitives around your application, you can create specific types like `EmailAddress`, `FilePath`, `Temperature`, or `UserId` that are impossible to misuse and automatically validate their content.

## 🌟 Key Features

- **Type Safety**: Eliminate primitive obsession with strongly-typed wrappers
- **Comprehensive Validation**: 50+ built-in validation attributes for all common scenarios
- **Path Handling**: Specialized path types with polymorphic interfaces and file system operations
- **Complete Physics System**: 80+ physics quantities across 8 scientific domains with dimensional analysis
- **Physical Constants**: Centralized, type-safe access to fundamental and derived constants
- **Unit Conversions**: Automatic unit handling with compile-time dimensional safety
- **Factory Pattern**: Clean object creation with dependency injection support
- **Performance Optimized**: Span-based operations, pooled builders, and minimal allocations
- **Enterprise Ready**: Full .NET ecosystem integration (ASP.NET Core, Entity Framework, etc.)

## 🚀 Quick Start

### Installation

```bash
dotnet add package ktsu.Semantics
```

### Basic Usage

```csharp
using ktsu.Semantics;

// Define strongly-typed domain models
[IsEmail]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[HasLength(8, 50), IsNotEmpty]
public sealed record UserId : SemanticString<UserId> { }

// Simple direct usage - Clean API with type inference:

// 1. Create methods (recommended) - no generic parameters needed!
var email1 = EmailAddress.Create("user@example.com");
var userId1 = UserId.Create("USER_12345");

// 2. From character arrays
char[] emailChars = ['u', 's', 'e', 'r', '@', 'e', 'x', 'a', 'm', 'p', 'l', 'e', '.', 'c', 'o', 'm'];
var email2 = EmailAddress.Create(emailChars);

// 3. From ReadOnlySpan<char> (performance optimized)
var userId2 = UserId.Create("USER_12345".AsSpan());

// 4. Explicit string casting
var email3 = (EmailAddress)"user@example.com";
var userId3 = (UserId)"USER_12345";

// 5. Safe creation with TryCreate (no exceptions)
if (EmailAddress.TryCreate("maybe@invalid", out EmailAddress? safeEmail))
{
    // Use safeEmail - validation succeeded
}

// Compile-time safety prevents mistakes
public void SendWelcomeEmail(EmailAddress to, UserId userId) { /* ... */ }

// This won't compile - type safety in action!
// SendWelcomeEmail(userId, email); // ❌ Compiler error!
```

### Factory Pattern Usage

```csharp
// Use factory pattern (recommended for dependency injection)
var emailFactory = new SemanticStringFactory<EmailAddress>();
var userFactory = new SemanticStringFactory<UserId>();

// Clean overloaded API - Create methods
var email = emailFactory.Create("user@example.com");
var userId = userFactory.Create("USER_12345");

// All input types supported via overloading
var email2 = emailFactory.Create(['u', 's', 'e', 'r', '@', 'e', 'x', 'a', 'm', 'p', 'l', 'e', '.', 'c', 'o', 'm']);
var userId2 = userFactory.Create("USER_12345".AsSpan());

// Safe creation with TryCreate
if (emailFactory.TryCreate("maybe@invalid", out EmailAddress? safeEmail))
{
    // Success!
}

// Legacy FromString methods still available  
var email3 = emailFactory.FromString("user@example.com");
```

### Physics Quantities System

```csharp
// Complete physics system with 80+ quantities across 8 domains
public sealed record Temperature<T> : PhysicalQuantity<Temperature<T>, T> where T : struct, INumber<T> { }
public sealed record Force<T> : PhysicalQuantity<Force<T>, T> where T : struct, INumber<T> { }
public sealed record Energy<T> : PhysicalQuantity<Energy<T>, T> where T : struct, INumber<T> { }

// Create quantities with dimensional safety
var temp = Temperature<double>.FromCelsius(25.0);      // 298.15 K
var force = Force<double>.FromNewtons(100.0);         // 100 N
var distance = Length<double>.FromMeters(5.0);        // 5 m

// Physics relationships with compile-time safety
var work = force * distance;                          // Results in Energy<double>
var power = work / Time<double>.FromSeconds(10.0);    // Results in Power<double>

// Type-safe unit conversions
Console.WriteLine(temp.ToFahrenheit());               // 77°F
Console.WriteLine(force.ToPounds());                  // 22.48 lbf

// Dimensional analysis prevents errors
// var invalid = force + temp;                        // ❌ Compiler error!
```

### Path Handling

```csharp
// Use specialized path types
var fileFactory = new SemanticStringFactory<AbsoluteFilePath>();
var configFile = fileFactory.Create(@"C:\app\config.json");

// Rich path operations
Console.WriteLine(configFile.FileName);        // config.json
Console.WriteLine(configFile.FileExtension);   // .json  
Console.WriteLine(configFile.DirectoryPath);   // C:\app
Console.WriteLine(configFile.Exists);          // True/False

// Polymorphic path collections
List<IPath> allPaths = [
    AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\data.txt"),
    RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"logs\app"),
    FilePath.FromString<FilePath>(@"document.pdf")
];

// Filter by interface type
var filePaths = allPaths.OfType<IFilePath>().ToList();
var absolutePaths = allPaths.OfType<IAbsolutePath>().ToList();
```

### Complex Validation

```csharp
// Combine multiple validation rules
[IsNotEmpty, IsEmail, HasLength(5, 100)]
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }

// Use validation strategies for flexible requirements
[ValidateAny] // Either email OR phone is acceptable
[IsEmail, RegexMatch(@"^\+?\d{10,15}$")]
public sealed record ContactInfo : SemanticString<ContactInfo> { }

// First-class type validation
[IsDateTime]
public sealed record ScheduledDate : SemanticString<ScheduledDate> { }

[IsDecimal, IsPositive]
public sealed record Price : SemanticString<Price> { }

[IsGuid]
public sealed record TransactionId : SemanticString<TransactionId> { }
```

## 🔧 Common Use Cases

### E-commerce Domain

```csharp
[HasLength(3, 20), IsNotEmpty]
public sealed record ProductSku : SemanticString<ProductSku> { }

[IsPositive, IsDecimal]  
public sealed record Price : SemanticString<Price> { }

[IsEmail]
public sealed record CustomerEmail : SemanticString<CustomerEmail> { }

public class Order
{
    public CustomerEmail CustomerEmail { get; set; }
    public ProductSku[] Items { get; set; }
    public Price TotalAmount { get; set; }
}
```

### Configuration Management

```csharp
[IsAbsolutePath, DoesExist]
public sealed record ConfigFilePath : SemanticString<ConfigFilePath> { }

[IsIpAddress]
public sealed record ServerAddress : SemanticString<ServerAddress> { }

[IsInRange(1, 65535)]
public sealed record Port : SemanticQuantity<Port, int> { }
```

### Complete Physics Domains

The library includes **80+ physics quantities** across **8 scientific domains**:

#### 🔧 Mechanics (15 quantities)
```csharp
// Kinematics and dynamics
var velocity = Velocity<double>.FromMetersPerSecond(15.0);
var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
var force = Mass<double>.FromKilograms(10.0) * acceleration;    // F = ma

// Work and energy
var work = force * Length<double>.FromMeters(5.0);             // W = F⋅d
var power = work / Time<double>.FromSeconds(2.0);              // P = W/t
```

#### ⚡ Electrical (11 quantities)
```csharp
// Ohm's law relationships
var voltage = Voltage<double>.FromVolts(12.0);
var current = Current<double>.FromAmperes(2.0);
var resistance = voltage / current;                            // R = V/I
var power = voltage * current;                                 // P = VI
```

#### 🌡️ Thermal (10 quantities)
```csharp
// Thermodynamics
var temp = Temperature<double>.FromCelsius(25.0);
var heat = Heat<double>.FromJoules(1000.0);
var capacity = HeatCapacity<double>.FromJoulesPerKelvin(100.0);
var entropy = heat / temp;                                     // S = Q/T
```

#### 🧪 Chemical (10 quantities)
```csharp
// Chemical calculations
var moles = AmountOfSubstance<double>.FromMoles(0.5);
var molarity = moles / Volume<double>.FromLiters(2.0);         // M = n/V
var rate = ReactionRate<double>.FromMolarPerSecond(0.01);
```

#### 🔊 Acoustic (20 quantities)
```csharp
// Sound and vibration
var frequency = Frequency<double>.FromHertz(440.0);            // A4 note
var wavelength = SoundSpeed<double>.Default / frequency;       // λ = v/f  
var intensity = SoundIntensity<double>.FromWattsPerSquareMeter(1e-6);
```

#### ☢️ Nuclear (5 quantities)
```csharp
// Nuclear physics
var activity = RadioactiveActivity<double>.FromBecquerels(1000.0);
var dose = AbsorbedDose<double>.FromGrays(0.001);
var exposure = Exposure<double>.FromCoulombsPerKilogram(1e-6);
```

#### 💡 Optical (6 quantities)
```csharp
// Photometry and optics
var flux = LuminousFlux<double>.FromLumens(800.0);
var illuminance = flux / Area<double>.FromSquareMeters(4.0);   // E = Φ/A
var luminance = Luminance<double>.FromCandelasPerSquareMeter(100.0);
```

#### 🌊 Fluid Dynamics (5 quantities)
```csharp
// Fluid mechanics
var viscosity = DynamicViscosity<double>.FromPascalSeconds(0.001);
var flowRate = VolumetricFlowRate<double>.FromCubicMetersPerSecond(0.1);
var reynolds = ReynoldsNumber<double>.Calculate(velocity, Length<double>.FromMeters(0.1), viscosity);
```

## 🏗️ Dependency Injection

```csharp
// Register factories in your DI container
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();

// Use in services
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }

    public async Task<User> CreateUserAsync(string email)
    {
        // Factory handles validation and throws meaningful exceptions
        var validatedEmail = _emailFactory.Create(email);
        return new User(validatedEmail);
    }
}
```

## 📖 Documentation

Comprehensive documentation is available in the [`docs/`](docs/) directory:

-   **[Complete Library Guide](docs/complete-library-guide.md)** - 🌟 **START HERE** - Complete overview of all library features and components
-   **[Architecture Guide](docs/architecture.md)** - SOLID principles, design patterns, and system architecture
-   **[Advanced Usage Guide](docs/advanced-usage.md)** - Advanced features, custom validation, and best practices  
-   **[Validation Reference](docs/validation-reference.md)** - Complete reference of all validation attributes
-   **[FluentValidation Integration](docs/fluent-validation-integration.md)** - Integration with FluentValidation library

## 💡 Examples

Extensive examples are available in [`docs/examples/`](docs/examples/):

-   **[Getting Started](docs/examples/getting-started.md)** - Basic usage patterns
-   **[Physics Relationships](docs/examples/PhysicsRelationshipExamples.md)** - Physics calculations and relationships
-   **[Validation Attributes](docs/examples/validation-attributes.md)** - Built-in and custom validation
-   **[Path Handling](docs/examples/path-handling.md)** - File system operations
-   **[Factory Pattern](docs/examples/factory-pattern.md)** - Object creation and DI
-   **[String Operations](docs/examples/string-operations.md)** - String compatibility and LINQ
-   **[Type Conversions](docs/examples/type-conversions.md)** - Cross-type conversions
-   **[Real-World Scenarios](docs/examples/real-world-scenarios.md)** - Complete domain examples

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🆘 Support

-   📖 [Documentation](https://github.com/ktsu-dev/Semantics/wiki)
-   🐛 [Issues](https://github.com/ktsu-dev/Semantics/issues)
-   💬 [Discussions](https://github.com/ktsu-dev/Semantics/discussions)
-   📦 [NuGet Package](https://www.nuget.org/packages/ktsu.Semantics/)

---

*Transform your primitive-obsessed code into a strongly-typed, self-validating domain model with ktsu.Semantics.*
