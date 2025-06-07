# Semantics

[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.svg)](https://www.nuget.org/packages/ktsu.Semantics/)
[![Build Status](https://github.com/ktsu-dev/Semantics/workflows/CI/badge.svg)](https://github.com/ktsu-dev/Semantics/actions)

A powerful .NET library for creating type-safe, validated string types using semantic meaning. Transform primitive string obsession into strongly-typed, self-validating domain models.

## Overview

The Semantics library enables you to create strongly-typed string wrappers that carry semantic meaning and built-in validation. Instead of passing raw strings around your application, you can create specific types like `EmailAddress`, `FilePath`, or `UserId` that are impossible to misuse and automatically validate their content.

## Key Features

-   **Type Safety**: Eliminate primitive obsession with strongly-typed string wrappers
-   **Automatic Validation**: Built-in attribute-based validation system
-   **Path Handling**: Specialized semantic path types with file system operations
-   **Quantity Types**: Support for numeric values with units and validation
-   **Extensible**: Easy to create custom semantic string types
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

// Create and use semantic strings
var email = EmailAddress.FromString<EmailAddress>("user@example.com");
Console.WriteLine(email); // user@example.com

// Type safety prevents mixing incompatible string types
public void SendEmail(EmailAddress to, string subject) { /* ... */ }

// This won't compile - type safety in action!
// SendEmail("not-an-email", subject); // Compiler error!
```

### Path Handling

```csharp
using ktsu.Semantics;

// Use built-in path types
var absolutePath = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\MyApp");
var relativePath = RelativePath.FromString<RelativePath>(@"src\Program.cs");
var filePath = FilePath.FromString<FilePath>(@"C:\temp\data.json");

// Access path properties
Console.WriteLine(filePath.FileName);        // data.json
Console.WriteLine(filePath.FileExtension);   // .json
Console.WriteLine(filePath.DirectoryPath);   // C:\temp
Console.WriteLine(absolutePath.Exists);      // True/False
Console.WriteLine(absolutePath.IsDirectory); // True/False
```

### Custom Validation

```csharp
// Create custom validation attributes
public class IsProductCodeAttribute : SemanticStringValidationAttribute
{
    public override bool Validate(ISemanticString semanticString)
    {
        string value = semanticString.ToString();
        // Product codes must be 6 characters, start with letter, followed by 5 digits
        return Regex.IsMatch(value, @"^[A-Z][0-9]{5}$");
    }
}

// Apply custom validation
[IsProductCode]
public sealed record ProductCode : SemanticString<ProductCode> { }

// Usage with automatic validation
var validCode = ProductCode.FromString<ProductCode>("A12345");   // ✅ Valid
var invalidCode = ProductCode.FromString<ProductCode>("123ABC"); // ❌ Throws FormatException
```

## Built-in Validation Attributes

The library provides comprehensive validation attributes for common scenarios:

### String Validation

-   `IsEmailAttribute` - Email address format validation
-   `IsUrlAttribute` - URL format validation
-   `IsNotEmptyAttribute` - Prevents empty/null strings
-   `HasLengthAttribute` - String length constraints

### Path Validation

-   `IsPathAttribute` - Valid path characters and length
-   `IsAbsolutePathAttribute` - Fully qualified paths
-   `IsRelativePathAttribute` - Relative paths
-   `IsFilePathAttribute` - File-specific paths
-   `IsDirectoryPathAttribute` - Directory-specific paths
-   `IsFileNameAttribute` - Valid filename validation
-   `IsExtensionAttribute` - File extension validation
-   `DoesExistAttribute` - File system existence checking

### Quantity Validation

-   `IsPositiveAttribute` - Positive numeric values
-   `IsNegativeAttribute` - Negative numeric values
-   `IsInRangeAttribute` - Value range constraints

## Built-in Types

### Path Types

-   `Path` - Any valid path
-   `AbsolutePath` - Fully qualified paths
-   `RelativePath` - Relative paths
-   `FilePath` - File paths with file operations
-   `DirectoryPath` - Directory paths
-   `FileName` - Just the filename component
-   `FileExtension` - File extensions (with period)

### String Types

-   Base `SemanticString<T>` for custom types
-   Extensible validation system
-   Type-safe conversions and operations

## Advanced Features

### Type Conversions

```csharp
// Safe conversions between compatible types
var genericPath = Path.FromString<Path>(@"C:\temp\file.txt");
var specificPath = genericPath.As<FilePath>(); // Convert to more specific type

// Implicit conversions to primitive types
string pathString = specificPath;              // Implicit to string
char[] pathChars = specificPath;               // Implicit to char[]
ReadOnlySpan<char> pathSpan = specificPath;    // Implicit to span
```

### Validation Modes

```csharp
// Require ALL validation attributes to pass (default)
[ValidateAll]
[IsPath, IsAbsolutePath, DoesExist]
public sealed record ExistingAbsolutePath : SemanticPath<ExistingAbsolutePath> { }

// Require ANY validation attribute to pass
[ValidateAny]
[IsEmail, IsUrl]
public sealed record ContactInfo : SemanticString<ContactInfo> { }
```

### Path Operations

```csharp
var from = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\App");
var to = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\Lib\Utils.cs");

// Create relative path between two absolute paths
var relativePath = RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(from, to);
Console.WriteLine(relativePath); // ..\Lib\Utils.cs
```

## Best Practices

1. **Create Domain-Specific Types**: Use semantic strings for domain concepts like `UserId`, `OrderNumber`, `ProductSku`
2. **Validate at Boundaries**: Create semantic strings at system boundaries (APIs, user input, file I/O)
3. **Use Type Safety**: Let the compiler prevent string misuse with strong typing
4. **Combine Validations**: Use multiple validation attributes for comprehensive checking
5. **Document Intent**: Semantic types make code self-documenting

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Support

-   📖 [Documentation](https://github.com/ktsu-dev/Semantics/wiki)
-   🐛 [Issues](https://github.com/ktsu-dev/Semantics/issues)
-   💬 [Discussions](https://github.com/ktsu-dev/Semantics/discussions)
-   📦 [NuGet Package](https://www.nuget.org/packages/ktsu.Semantics/)
