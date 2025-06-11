# ktsu.Semantics

[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![Build Status](https://github.com/ktsu-dev/Semantics/workflows/CI/badge.svg)](https://github.com/ktsu-dev/Semantics/actions)

A powerful .NET library for creating type-safe, validated types with semantic meaning. Transform primitive string obsession into strongly-typed, self-validating domain models with comprehensive validation, specialized path handling, and quantity types.

## Overview

The Semantics library enables you to create strongly-typed wrappers that carry semantic meaning and built-in validation. Instead of passing raw primitives around your application, you can create specific types like `EmailAddress`, `FilePath`, `Temperature`, or `UserId` that are impossible to misuse and automatically validate their content.

## 🌟 Key Features

- **Type Safety**: Eliminate primitive obsession with strongly-typed wrappers
- **Comprehensive Validation**: 50+ built-in validation attributes for all common scenarios
- **Path Handling**: Specialized path types with polymorphic interfaces and file system operations
- **Quantity System**: Type-safe numeric values with units and arithmetic operations
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

### Semantic Quantities

```csharp
// Define quantity types with units
public sealed record Temperature : SemanticQuantity<Temperature, decimal> { }
public sealed record Distance : SemanticQuantity<Distance, double> { }

// Create instances
var temp1 = Temperature.Create(23.5m);  // 23.5°C
var temp2 = Temperature.Create(18.2m);  // 18.2°C

// Type-safe arithmetic operations
var avgTemp = (temp1 + temp2) / 2m;     // Average temperature
var tempDiff = temp1 - temp2;           // Temperature difference

// Quantities are strongly typed
public void SetThermostat(Temperature target) { /* ... */ }

// This won't compile - different quantity types
// SetThermostat(Distance.Create(100)); // ❌ Compiler error!
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

### Scientific Computing

```csharp
public sealed record Temperature : SemanticQuantity<Temperature, double> { }
public sealed record Pressure : SemanticQuantity<Pressure, decimal> { }

// Type-safe calculations
public Volume CalculateVolume(Pressure pressure, Temperature temperature)
{
    // Ideal gas law calculation with type safety
    var result = (pressure * Volume.Create(1.0)) / temperature;
    return result;
}
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
