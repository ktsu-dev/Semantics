# Semantics

[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![Build Status](https://github.com/ktsu-dev/Semantics/workflows/CI/badge.svg)](https://github.com/ktsu-dev/Semantics/actions)

A powerful .NET library for creating type-safe, validated string types using semantic meaning. Transform primitive string obsession into strongly-typed, self-validating domain models.

Built with **SOLID principles** and **DRY (Don't Repeat Yourself)** practices at its core, the library provides a clean, extensible architecture that promotes maintainable and testable code.

## Overview

The Semantics library enables you to create strongly-typed string wrappers that carry semantic meaning and built-in validation. Instead of passing raw strings around your application, you can create specific types like `EmailAddress`, `FilePath`, or `UserId` that are impossible to misuse and automatically validate their content.

## Key Features

-   **Type Safety**: Eliminate primitive obsession with strongly-typed string wrappers
-   **SOLID Architecture**: Built following Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion principles
-   **Extensible Validation**: Pluggable validation system with custom strategies and rules
-   **Factory Pattern**: Clean object creation with `ISemanticStringFactory<T>`
-   **Contract Programming**: Behavioral contracts ensuring Liskov Substitution Principle compliance
-   **Automatic Validation**: Built-in attribute-based validation system
-   **Path Handling**: Specialized semantic path types with file system operations
-   **Quantity Types**: Support for numeric values with units and validation
-   **Performance**: Zero-allocation conversions and optimized operations
-   **Comprehensive**: Full XML documentation and IntelliSense support

## Quick Start

### Installation

```bash
dotnet add package ktsu.Semantics
```

### Basic Usage

```csharp
using ktsu.Semantics;

// Define a custom semantic string type
[IsEmail]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

// Create using factory pattern (recommended)
var factory = new SemanticStringFactory<EmailAddress>();
var email = factory.Create("user@example.com");

// Or use the traditional approach
var email2 = EmailAddress.FromString<EmailAddress>("user@example.com");

// Type safety prevents mixing incompatible string types
public void SendEmail(EmailAddress to, string subject) { /* ... */ }

// This won't compile - type safety in action!
// SendEmail("not-an-email", subject); // Compiler error!
```

### Path Handling

```csharp
using ktsu.Semantics;

// Use built-in path types with factory pattern
var pathFactory = new SemanticStringFactory<FilePath>();
var filePath = pathFactory.Create(@"C:\temp\data.json");

// Access path properties
Console.WriteLine(filePath.FileName);        // data.json
Console.WriteLine(filePath.FileExtension);   // .json
Console.WriteLine(filePath.DirectoryPath);   // C:\temp

// Traditional approach still works
var absolutePath = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\MyApp");
Console.WriteLine(absolutePath.Exists);      // True/False
Console.WriteLine(absolutePath.IsDirectory); // True/False
```

### Multiple Validations

```csharp
// Combine multiple validation attributes
[IsNotEmpty, IsEmail, HasLength(5, 100)]
public sealed record ProfessionalEmail : SemanticString<ProfessionalEmail> { }

// Use validation strategies for complex scenarios
[ValidateAny]
[IsEmail, IsUrl]
public sealed record ContactInfo : SemanticString<ContactInfo> { }

var factory = new SemanticStringFactory<ContactInfo>();
var email = factory.Create("user@example.com");     // ✅ Valid (email)
var url = factory.Create("https://example.com");    // ✅ Valid (URL)
```

## Documentation

Comprehensive documentation is available in the [`docs/`](docs/) directory:

-   **[Architecture Guide](docs/architecture.md)** - Detailed overview of SOLID principles, design patterns, and system architecture
-   **[Advanced Usage Guide](docs/advanced-usage.md)** - Advanced features including custom validation strategies, dependency injection, and best practices
-   **[Validation Reference](docs/validation-reference.md)** - Complete reference of all built-in validation attributes and strategies

## Examples

Extensive examples demonstrating all library features are available in the [`examples/`](examples/) directory. Start with the [Examples Index](examples/examples-index.md) for organized learning paths:

-   **[Getting Started](examples/getting-started.md)** - Basic usage patterns and your first semantic strings
-   **[Validation Attributes](examples/validation-attributes.md)** - Built-in validation and custom rules
-   **[Path Handling](examples/path-handling.md)** - File system paths and validation
-   **[Factory Pattern](examples/factory-pattern.md)** - Object creation and dependency injection
-   **[Real-World Scenarios](examples/real-world-scenarios.md)** - Complete domain examples for e-commerce, finance, and more

Each example includes complete, runnable code that you can copy and adapt to your specific needs.

## Common Use Cases

### Domain-Specific Types

```csharp
// Create strongly-typed identifiers
[HasLength(8, 12), IsNotEmpty]
public sealed record UserId : SemanticString<UserId> { }

[HasLength(3, 10), IsNotEmpty]
public sealed record ProductSku : SemanticString<ProductSku> { }

// Use them in your domain models
public class Order
{
    public UserId CustomerId { get; set; }
    public ProductSku[] Items { get; set; }
}
```

### Input Validation

```csharp
public class UserController : ControllerBase
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserController(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (!_emailFactory.TryCreate(request.Email, out var emailAddress))
        {
            return BadRequest("Invalid email format");
        }

        // emailAddress is guaranteed to be valid
        var user = new User(emailAddress);
        return Ok(user);
    }
}
```

### File System Operations

```csharp
// Work with type-safe paths
var sourceFactory = new SemanticStringFactory<FilePath>();
var destinationFactory = new SemanticStringFactory<DirectoryPath>();

var sourceFile = sourceFactory.Create(@"C:\temp\data.csv");
var destinationDir = destinationFactory.Create(@"C:\backup\");

if (sourceFile.Exists)
{
    // Type-safe file operations
    File.Copy(sourceFile, Path.Combine(destinationDir, sourceFile.FileName));
}
```

## Built-in Validation Attributes

The library includes comprehensive validation for common scenarios:

-   **String**: `IsEmail`, `IsUrl`, `IsNotEmpty`, `HasLength`
-   **Path**: `IsPath`, `IsAbsolutePath`, `IsRelativePath`, `IsFilePath`, `IsDirectoryPath`, `DoesExist`
-   **Numeric**: `IsPositive`, `IsNegative`, `IsInRange`

See the [Validation Reference](docs/validation-reference.md) for complete details.

## Dependency Injection

```csharp
// Register in your DI container
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();
services.AddTransient<ISemanticStringFactory<UserId>, SemanticStringFactory<UserId>>();

// Inject into services
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Support

-   📖 [Documentation](https://github.com/ktsu-dev/Semantics/wiki)
-   🐛 [Issues](https://github.com/ktsu-dev/Semantics/issues)
-   💬 [Discussions](https://github.com/ktsu-dev/Semantics/discussions)
-   📦 [NuGet Package](https://www.nuget.org/packages/ktsu.Semantics/)
