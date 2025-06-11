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

The Semantics library consists of five main areas:

1. **Semantic Strings** - Type-safe string wrappers with validation
2. **Semantic Quantities** - Type-safe numeric values with units  
3. **Path System** - Comprehensive file system path handling
4. **Validation System** - 50+ validation attributes across multiple categories
5. **Performance Utilities** - Optimizations for high-performance scenarios

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

## Semantic Quantities

Type-safe numeric values with units and arithmetic operations:

```csharp
public sealed record Temperature : SemanticQuantity<Temperature, decimal> { }
public sealed record Distance : SemanticQuantity<Distance, double> { }

var temp1 = Temperature.Create(25.5m);
var temp2 = Temperature.Create(18.2m);

// Type-safe arithmetic
var avgTemp = (temp1 + temp2) / 2m;     // Returns Temperature
var tempDiff = temp1 - temp2;           // Returns Temperature

// Prevents unit confusion
// var invalid = temp1 + distance1;     // ❌ Compiler error!
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

This library transforms primitive-obsessed code into strongly-typed, self-validating domain models with comprehensive validation and excellent performance characteristics. 
