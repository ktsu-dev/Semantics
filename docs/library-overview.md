# Semantics Library - Complete Overview

This document provides a comprehensive overview of all features and components in the ktsu.Semantics library.

## Table of Contents

- [Core Architecture](#core-architecture)
- [Semantic Strings](#semantic-strings)
- [Semantic Quantities](#semantic-quantities)
- [Path System](#path-system)
- [Validation System](#validation-system)
- [Factory Pattern](#factory-pattern)
- [Performance Utilities](#performance-utilities)
- [Integration Features](#integration-features)

## Core Architecture

The Semantics library is built around the principle of **semantic typing** - replacing primitive types with domain-specific types that carry meaning and validation. The library consists of five main components:

1. **Semantic Strings** - Type-safe string wrappers
2. **Semantic Quantities** - Type-safe numeric values with units
3. **Path System** - Comprehensive file system path handling
4. **Validation System** - Extensive validation framework
5. **Utilities** - Performance optimization tools

## Semantic Strings

### Core Concept

Semantic strings eliminate "primitive obsession" by wrapping raw strings in strongly-typed, validated containers. Each semantic string type carries its own validation rules and semantic meaning.

### Base Classes and Interfaces

```csharp
// Core interface
public interface ISemanticString

// Abstract base class for all semantic strings
public abstract record SemanticString<TDerived> : ISemanticString
    where TDerived : SemanticString<TDerived>

// Factory interface for dependency injection
public interface ISemanticStringFactory<T> where T : ISemanticString

// Concrete factory implementation
public class SemanticStringFactory<T> : ISemanticStringFactory<T>
```

### Creating Custom Types

```csharp
// Basic semantic string with validation
[IsEmail]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

// Multiple validation rules
[IsNotEmpty, HasLength(3, 50), RegexMatch(@"^[A-Z0-9_]+$")]
public sealed record ProductCode : SemanticString<ProductCode> { }

// Custom validation strategies
[ValidateAny]
[IsEmail, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

### String Compatibility

Semantic strings maintain full compatibility with `System.String`:

```csharp
var email = EmailAddress.FromString<EmailAddress>("user@example.com");

// Implicit conversion to string
string stringValue = email;

// All string methods available
bool startsWithUser = email.StartsWith("user");
string upperEmail = email.ToUpper();
int length = email.Length;

// LINQ operations
var emails = new[] { email1, email2, email3 };
var domains = emails.Select(e => e.Split('@')[1]).ToArray();
```

### Type Conversions

The library provides safe cross-type conversions:

```csharp
// Direct conversion with validation
var email = EmailAddress.FromString<EmailAddress>("user@example.com");
var genericString = email.As<GenericString>();

// Factory-based conversion
var factory = new SemanticStringFactory<UserId>();
var userId = factory.Create(email.ToString());
```

## Semantic Quantities

### Purpose

Semantic quantities provide type-safe numeric values with units, preventing unit confusion and enabling domain-specific arithmetic operations.

### Base Classes

```csharp
// Simple quantity storage
public record SemanticQuantity<TStorage> where TStorage : INumber<TStorage>

// Full quantity with arithmetic operations
public record SemanticQuantity<TSelf, TStorage> 
    : SemanticQuantity<TStorage>
    where TSelf : SemanticQuantity<TSelf, TStorage>, new()
    where TStorage : INumber<TStorage>
```

### Creating Quantity Types

```csharp
// Temperature with decimal precision
public sealed record Temperature : SemanticQuantity<Temperature, decimal> { }

// Distance with double precision  
public sealed record Distance : SemanticQuantity<Distance, double> { }

// Integer-based quantities
public sealed record Count : SemanticQuantity<Count, int> { }
```

### Arithmetic Operations

```csharp
var temp1 = Temperature.Create(25.5m);
var temp2 = Temperature.Create(18.2m);

// Basic arithmetic - results maintain type safety
var avgTemp = (temp1 + temp2) / 2m;        // Temperature
var tempDiff = temp1 - temp2;              // Temperature
var scaledTemp = temp1 * 1.5m;             // Temperature

// Division returns storage type when dividing quantities
decimal ratio = temp1 / temp2;             // decimal

// Prevents mixing incompatible units
// var invalid = temp1 + distance1;        // Compiler error!
```

### Advanced Quantity Operations

```csharp
// Cross-quantity calculations with explicit result types
public sealed record Area : SemanticQuantity<Area, double> { }
public sealed record Volume : SemanticQuantity<Volume, double> { }

public static Area CalculateArea(Distance length, Distance width)
{
    return SemanticQuantity<Distance, double>.Multiply<Area>(length, width);
}

public static Volume CalculateVolume(Area area, Distance height)
{
    return SemanticQuantity<Area, double>.Multiply<Volume>(area, height);
}
```

## Path System

### Overview

The path system provides comprehensive file system path handling with a sophisticated polymorphic interface hierarchy.

### Interface Hierarchy

```
IPath (base interface)
├── IAbsolutePath : IPath
├── IRelativePath : IPath
├── IFilePath : IPath
├── IDirectoryPath : IPath
├── IAbsoluteFilePath : IFilePath, IAbsolutePath
├── IRelativeFilePath : IFilePath, IRelativePath
├── IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
└── IRelativeDirectoryPath : IDirectoryPath, IRelativePath

Separate hierarchies:
├── IFileName
└── IFileExtension
```

### Concrete Path Types

```csharp
// Generic path types
public sealed record AbsolutePath : SemanticAbsolutePath<AbsolutePath>, IAbsolutePath
public sealed record RelativePath : SemanticRelativePath<RelativePath>, IRelativePath
public sealed record FilePath : SemanticFilePath<FilePath>, IFilePath
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath

// Specific path combinations
public sealed record AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
public sealed record RelativeFilePath : SemanticFilePath<RelativeFilePath>, IRelativeFilePath
public sealed record AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath

// Path components
public sealed record FileName : SemanticString<FileName>, IFileName
public sealed record FileExtension : SemanticString<FileExtension>, IFileExtension
```

### Path Operations

```csharp
var filePath = AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\Projects\App\config.json");

// Rich path information
Console.WriteLine(filePath.FileName);           // config.json
Console.WriteLine(filePath.FileExtension);      // .json
Console.WriteLine(filePath.DirectoryPath);      // C:\Projects\App
Console.WriteLine(filePath.FileNameWithoutExtension); // config

// File system operations
Console.WriteLine(filePath.Exists);             // True/False
Console.WriteLine(filePath.IsDirectory);        // False
Console.WriteLine(filePath.IsFile);             // True
Console.WriteLine(filePath.IsReadOnly);         // True/False

// Path manipulation
var newPath = filePath.ChangeExtension(".xml");
var backupPath = filePath.AddSuffix("_backup");
```

### Polymorphic Path Usage

```csharp
// Method accepting any path type
public void LogPath(IPath path)
{
    Console.WriteLine($"Processing path: {path}");
}

// Method accepting only file paths
public void ProcessFile(IFilePath filePath)
{
    if (filePath.Exists)
    {
        // Process file
    }
}

// Method accepting absolute paths only
public void ProcessAbsolute(IAbsolutePath absolutePath)
{
    // Work with absolute paths
}

// Collections of mixed path types
List<IPath> allPaths = [
    AbsoluteFilePath.FromString<AbsoluteFilePath>(@"C:\data.txt"),
    RelativeDirectoryPath.FromString<RelativeDirectoryPath>(@"temp\logs"),
    FilePath.FromString<FilePath>(@"config.json")
];

// Filter by interface
var files = allPaths.OfType<IFilePath>().ToList();
var directories = allPaths.OfType<IDirectoryPath>().ToList();
var absolutePaths = allPaths.OfType<IAbsolutePath>().ToList();
```

## Validation System

### Architecture

The validation system uses a layered approach:

1. **Validation Attributes** - Declarative validation rules
2. **Validation Strategies** - How multiple rules are combined
3. **Validation Rules** - Individual validation logic implementations

### Validation Categories

#### Text Validation
- `IsEmailAddress` - RFC-compliant email validation
- `RegexMatch(pattern)` - Custom regular expression matching
- `StartsWith(prefix)` / `EndsWith(suffix)` - String prefix/suffix validation
- `Contains(substring)` - Substring presence validation
- `IsBase64` - Base64 encoding validation
- `PrefixAndSuffix(prefix, suffix)` - Combined prefix and suffix validation

#### Format Validation
- `IsEmptyOrWhitespace` - Empty or whitespace-only strings
- `HasNonWhitespaceContent` - Must contain non-whitespace characters
- `IsSingleLine` / `IsMultiLine` - Line count validation
- `HasExactLines(count)` - Specific line count requirement
- `HasMinimumLines(min)` / `HasMaximumLines(max)` - Line count ranges

#### First-Class Type Validation
- `IsBoolean` - Valid boolean representation (true/false, 1/0, yes/no)
- `IsDateTime` - Valid date/time parsing
- `IsDecimal` / `IsDouble` / `IsInt32` - Numeric type validation
- `IsGuid` - Valid GUID format validation
- `IsIpAddress` - IPv4/IPv6 address validation
- `IsTimeSpan` - Valid time span format
- `IsUri` - Valid URI format validation
- `IsVersion` - Version string validation (e.g., "1.2.3.4")

#### Quantity Validation
- `IsPositive` / `IsNegative` - Sign validation for numeric values
- `IsInRange(min, max)` - Value range validation

#### Path Validation
- `IsPath` - Valid file system path format
- `IsAbsolutePath` / `IsRelativePath` - Path type validation
- `IsFilePath` / `IsDirectoryPath` - Path category validation
- `DoesExist` - File system existence validation

#### Casing Validation
- Various casing format validations (implementation-specific)

### Validation Strategies

```csharp
// All attributes must pass (default)
[ValidateAll]
[IsNotEmpty, IsEmail, HasLength(5, 100)]
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }

// Any attribute can pass
[ValidateAny]
[IsEmail, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

### Custom Validation Rules

```csharp
public class BusinessDomainRule : ValidationRuleBase
{
    public override string RuleName => "BusinessDomain";

    protected override bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        if (attribute is not BusinessDomainAttribute businessAttr)
            return false;

        var email = value.ToString();
        var domain = email.Split('@').LastOrDefault();
        
        return businessAttr.AllowedDomains.Contains(domain);
    }
}

[BusinessDomain(AllowedDomains: ["company.com", "partner.com"])]
public sealed record CorporateEmail : SemanticString<CorporateEmail> { }
```

## Factory Pattern

### Purpose

The factory pattern provides clean object creation, validation handling, and dependency injection support.

### Factory Interface

```csharp
public interface ISemanticStringFactory<T> where T : ISemanticString
{
    T Create(string value);
    bool TryCreate(string value, [NotNullWhen(true)] out T? result);
    T CreateOrThrow(string value);
}
```

### Usage Patterns

```csharp
// Basic factory usage
var factory = new SemanticStringFactory<EmailAddress>();
var email = factory.Create("user@example.com");

// Safe creation with error handling
if (factory.TryCreate("invalid-email", out var result))
{
    // Use result
}
else
{
    // Handle validation failure
}

// Dependency injection
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;
    
    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }
    
    public User CreateUser(string emailString)
    {
        var email = _emailFactory.Create(emailString);
        return new User(email);
    }
}
```

### DI Container Registration

```csharp
// ASP.NET Core
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();
services.AddTransient<ISemanticStringFactory<UserId>, SemanticStringFactory<UserId>>();

// Generic registration helper (if needed)
public static void RegisterSemanticFactory<T>(this IServiceCollection services) 
    where T : class, ISemanticString
{
    services.AddTransient<ISemanticStringFactory<T>, SemanticStringFactory<T>>();
}
```

## Performance Utilities

### Pooled String Builder

Reusable StringBuilder instances for high-performance string operations:

```csharp
// Internal utility - used automatically by the library
public class PooledStringBuilder : IDisposable
{
    public StringBuilder StringBuilder { get; }
    
    // Returns StringBuilder to pool when disposed
    public void Dispose() { /* return to pool */ }
}
```

### Interned Path Strings

Common path strings are interned to reduce memory usage:

```csharp
// Internal utility - optimizes memory for common paths
public static class InternedPathStrings
{
    public static string GetInterned(string path) { /* implementation */ }
}
```

### Span-based Path Utilities

High-performance path operations using Span<char>:

```csharp
// Internal utility - minimal allocation path processing
public static class SpanPathUtilities
{
    public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path) { /* implementation */ }
    public static ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path) { /* implementation */ }
    // ... other span-based operations
}
```

## Integration Features

### ASP.NET Core Integration

```csharp
// Model binding
public class CreateUserRequest
{
    public string Email { get; set; }
    public string UserId { get; set; }
}

[ApiController]
public class UserController : ControllerBase
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;
    
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (!_emailFactory.TryCreate(request.Email, out var email))
        {
            return BadRequest("Invalid email format");
        }
        
        // email is guaranteed valid
        var user = new User(email);
        return Ok(user);
    }
}
```

### Entity Framework Integration

```csharp
public class User
{
    public int Id { get; set; }
    public EmailAddress Email { get; set; }
    public UserId UserId { get; set; }
}

// DbContext configuration
public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Semantic strings are stored as their string values
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.ToString(),
                value => EmailAddress.FromString<EmailAddress>(value));
                
        modelBuilder.Entity<User>()
            .Property(u => u.UserId)
            .HasConversion(
                userId => userId.ToString(),
                value => UserId.FromString<UserId>(value));
    }
}
```

### JSON Serialization

```csharp
// Semantic strings serialize as their string values by default
var user = new User
{
    Email = EmailAddress.FromString<EmailAddress>("user@example.com"),
    UserId = UserId.FromString<UserId>("USER_123")
};

var json = JsonSerializer.Serialize(user);
// {"Email":"user@example.com","UserId":"USER_123"}

var deserializedUser = JsonSerializer.Deserialize<User>(json);
// Automatic conversion back to semantic types
```

### FluentValidation Integration

The library provides integration with FluentValidation for complex validation scenarios:

```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .Must(BeValidSemanticEmail)
            .WithMessage("Invalid email format");
    }
    
    private bool BeValidSemanticEmail(string email)
    {
        var factory = new SemanticStringFactory<EmailAddress>();
        return factory.TryCreate(email, out _);
    }
}
```

This comprehensive overview covers all major components of the Semantics library. Each component is designed to work together seamlessly while maintaining strong separation of concerns and following SOLID principles. 
