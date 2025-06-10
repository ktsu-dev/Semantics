# Validation Reference

This document provides a comprehensive reference for all built-in validation attributes and validation strategies available in the Semantics library.

## Table of Contents

-   [Overview](#overview)
-   [Built-in Validation Attributes](#built-in-validation-attributes)
-   [Built-in Types](#built-in-types)
-   [Validation Strategies](#validation-strategies)
-   [Custom Validation](#custom-validation)

## Overview

The Semantics library provides a robust validation system with multiple layers:

-   **Validation Attributes**: Decorative attributes that define validation rules
-   **Validation Strategies**: Control how multiple validation rules are processed
-   **Validation Rules**: The actual implementation of validation logic
-   **Built-in Types**: Pre-configured semantic string types with common validations

## Built-in Validation Attributes

### String Validation

#### `IsEmailAttribute`

Validates email address format using standard email regex patterns.

```csharp
[IsEmail]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

var factory = new SemanticStringFactory<EmailAddress>();
var email = factory.Create("user@example.com"); // ✅ Valid
// factory.Create("invalid-email");              // ❌ Throws FormatException
```

#### `IsUrlAttribute`

Validates URL format for both HTTP and HTTPS URLs.

```csharp
[IsUrl]
public sealed record WebUrl : SemanticString<WebUrl> { }

var factory = new SemanticStringFactory<WebUrl>();
var url = factory.Create("https://example.com"); // ✅ Valid
// factory.Create("not-a-url");                   // ❌ Throws FormatException
```

#### `IsNotEmptyAttribute`

Prevents empty, null, or whitespace-only strings.

```csharp
[IsNotEmpty]
public sealed record NonEmptyString : SemanticString<NonEmptyString> { }

var factory = new SemanticStringFactory<NonEmptyString>();
var text = factory.Create("Hello World"); // ✅ Valid
// factory.Create("");                     // ❌ Throws FormatException
// factory.Create("   ");                  // ❌ Throws FormatException
```

#### `HasLengthAttribute`

Constrains string length to specified minimum and maximum values.

```csharp
[HasLength(5, 20)] // Min 5, Max 20 characters
public sealed record Username : SemanticString<Username> { }

var factory = new SemanticStringFactory<Username>();
var username = factory.Create("johndoe"); // ✅ Valid (7 characters)
// factory.Create("abc");                  // ❌ Throws FormatException (too short)
// factory.Create("verylongusernamethatexceedslimit"); // ❌ Throws FormatException (too long)
```

### Path Validation

#### `IsPathAttribute`

Validates that the string represents a valid path with legal characters and appropriate length.

```csharp
[IsPath]
public sealed record GenericPath : SemanticString<GenericPath> { }

var factory = new SemanticStringFactory<GenericPath>();
var path = factory.Create(@"C:\temp\file.txt"); // ✅ Valid
// factory.Create("C:\\invalid<>path");          // ❌ Throws FormatException
```

#### `IsAbsolutePathAttribute`

Validates fully qualified, absolute paths.

```csharp
[IsAbsolutePath]
public sealed record AbsolutePath : SemanticString<AbsolutePath> { }

var factory = new SemanticStringFactory<AbsolutePath>();
var absPath = factory.Create(@"C:\Projects\App"); // ✅ Valid
// factory.Create("relative\path");               // ❌ Throws FormatException
```

#### `IsRelativePathAttribute`

Validates relative paths (not starting from root).

```csharp
[IsRelativePath]
public sealed record RelativePath : SemanticString<RelativePath> { }

var factory = new SemanticStringFactory<RelativePath>();
var relPath = factory.Create(@"subfolder\file.txt"); // ✅ Valid
// factory.Create(@"C:\absolute\path");               // ❌ Throws FormatException
```

#### `IsFilePathAttribute`

Validates paths that point to files (not directories).

```csharp
[IsFilePath]
public sealed record FilePath : SemanticString<FilePath> { }

var factory = new SemanticStringFactory<FilePath>();
var filePath = factory.Create(@"C:\temp\document.pdf"); // ✅ Valid
// factory.Create(@"C:\temp\");                         // ❌ Throws FormatException (directory)
```

#### `IsDirectoryPathAttribute`

Validates paths that point to directories.

```csharp
[IsDirectoryPath]
public sealed record DirectoryPath : SemanticString<DirectoryPath> { }

var factory = new SemanticStringFactory<DirectoryPath>();
var dirPath = factory.Create(@"C:\Projects\"); // ✅ Valid
// factory.Create(@"C:\file.txt");              // ❌ Throws FormatException (file)
```

#### `IsFileNameAttribute`

Validates filenames without path separators.

```csharp
[IsFileName]
public sealed record FileName : SemanticString<FileName> { }

var factory = new SemanticStringFactory<FileName>();
var fileName = factory.Create("document.pdf"); // ✅ Valid
// factory.Create("folder\\file.txt");          // ❌ Throws FormatException (contains path)
```

#### `IsExtensionAttribute`

Validates file extensions (with period).

```csharp
[IsExtension]
public sealed record FileExtension : SemanticString<FileExtension> { }

var factory = new SemanticStringFactory<FileExtension>();
var extension = factory.Create(".pdf"); // ✅ Valid
// factory.Create("pdf");                // ❌ Throws FormatException (no period)
```

#### `DoesExistAttribute`

Validates that the path exists in the file system.

```csharp
[IsPath, DoesExist]
public sealed record ExistingPath : SemanticString<ExistingPath> { }

var factory = new SemanticStringFactory<ExistingPath>();
var existingPath = factory.Create(@"C:\Windows"); // ✅ Valid (if exists)
// factory.Create(@"C:\NonExistent");             // ❌ Throws FormatException
```

### Quantity Validation

#### `IsPositiveAttribute`

Validates that numeric values are positive (> 0).

```csharp
[IsPositive]
public sealed record PositiveNumber : SemanticString<PositiveNumber> { }

var factory = new SemanticStringFactory<PositiveNumber>();
var positive = factory.Create("42"); // ✅ Valid
// factory.Create("-5");              // ❌ Throws FormatException
// factory.Create("0");               // ❌ Throws FormatException
```

#### `IsNegativeAttribute`

Validates that numeric values are negative (< 0).

```csharp
[IsNegative]
public sealed record NegativeNumber : SemanticString<NegativeNumber> { }

var factory = new SemanticStringFactory<NegativeNumber>();
var negative = factory.Create("-42"); // ✅ Valid
// factory.Create("5");               // ❌ Throws FormatException
// factory.Create("0");               // ❌ Throws FormatException
```

#### `IsInRangeAttribute`

Validates that numeric values fall within a specified range.

```csharp
[IsInRange(1, 100)] // Between 1 and 100 inclusive
public sealed record Percentage : SemanticString<Percentage> { }

var factory = new SemanticStringFactory<Percentage>();
var percentage = factory.Create("75"); // ✅ Valid
// factory.Create("150");              // ❌ Throws FormatException
// factory.Create("0");                // ❌ Throws FormatException
```

## Built-in Types

The library provides pre-configured semantic string types with appropriate validations:

### Path Types

```csharp
// Pre-configured path types - no additional attributes needed
var pathFactory = new SemanticStringFactory<ktsu.Semantics.Path>();
var absoluteFactory = new SemanticStringFactory<ktsu.Semantics.AbsolutePath>();
var relativeFactory = new SemanticStringFactory<ktsu.Semantics.RelativePath>();
var fileFactory = new SemanticStringFactory<ktsu.Semantics.FilePath>();
var directoryFactory = new SemanticStringFactory<ktsu.Semantics.DirectoryPath>();
var nameFactory = new SemanticStringFactory<ktsu.Semantics.FileName>();
var extensionFactory = new SemanticStringFactory<ktsu.Semantics.FileExtension>();

// Each type has built-in validation and specialized properties
var filePath = fileFactory.Create(@"C:\temp\data.json");
Console.WriteLine(filePath.FileName);        // data.json
Console.WriteLine(filePath.FileExtension);   // .json
Console.WriteLine(filePath.DirectoryPath);   // C:\temp
```

## Validation Strategies

Control how multiple validation attributes are processed:

### `ValidateAllAttribute` (Default)

All validation attributes must pass for the value to be considered valid.

```csharp
[ValidateAll] // Explicit, but this is the default behavior
[IsNotEmpty, IsEmail, HasLength(5, 50)]
public sealed record StrictEmail : SemanticString<StrictEmail> { }

// All three validations must pass:
// 1. Must not be empty
// 2. Must be valid email format
// 3. Must be between 5-50 characters
```

### `ValidateAnyAttribute`

At least one validation attribute must pass for the value to be considered valid.

```csharp
[ValidateAny]
[IsEmail, IsUrl]
public sealed record ContactInfo : SemanticString<ContactInfo> { }

// Either validation can pass:
// 1. Valid email address, OR
// 2. Valid URL
var factory = new SemanticStringFactory<ContactInfo>();
var email = factory.Create("user@example.com");     // ✅ Valid (email)
var url = factory.Create("https://example.com");    // ✅ Valid (URL)
```

### `ValidateWithAttribute`

Use a custom validation strategy for complex business rules.

```csharp
[ValidateWith(typeof(BusinessRuleValidationStrategy))]
[IsNotEmpty, IsEmail] // Critical validations
[IsCompanyEmail, IsInternalDomain] // Non-critical validations
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }
```

## Custom Validation

### Creating Custom Validation Attributes

```csharp
// Custom validation attribute
public class IsProductCodeAttribute : SemanticStringValidationAttribute
{
    public override bool Validate(ISemanticString semanticString)
    {
        string value = semanticString.ToString();
        // Product codes: letter + 5 digits
        return Regex.IsMatch(value, @"^[A-Z][0-9]{5}$");
    }
}

// Apply to semantic string type
[IsProductCode]
public sealed record ProductCode : SemanticString<ProductCode> { }
```

### Combining Multiple Validations

```csharp
// Complex validation combining multiple attributes
[IsNotEmpty, IsEmail, HasLength(5, 100)]
public sealed record ProfessionalEmail : SemanticString<ProfessionalEmail> { }

// Path with existence checking
[IsAbsolutePath, DoesExist]
public sealed record ExistingAbsolutePath : SemanticPath<ExistingAbsolutePath> { }

// Flexible contact information
[ValidateAny]
[IsEmail, IsUrl, HasLength(10, 15)] // Email, URL, or phone number length
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

This validation reference provides the foundation for creating robust, type-safe string types with comprehensive validation rules.

## Practical Examples

### Domain-Specific Types

```csharp
// API token that must be Base64-encoded
[IsBase64]
public sealed record ApiToken : SemanticString<ApiToken>;
// Usage: var token = "SGVsbG9Xb3JsZA==".As<ApiToken>();

// User's email address for account management
[IsEmailAddress]
public sealed record UserEmail : SemanticString<UserEmail>;
// Usage: var email = "user@company.com".As<UserEmail>();

// Blog post URL slug that's SEO-friendly
[RegexMatch(@"^[a-z0-9]+(-[a-z0-9]+)*$")]
public sealed record BlogSlug : SemanticString<BlogSlug>;
// Usage: var slug = "my-awesome-blog-post".As<BlogSlug>();

// Hexadecimal color code for UI theming
[RegexMatch(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
public sealed record ThemeColor : SemanticString<ThemeColor>;
// Usage: var color = "#FF5733".As<ThemeColor>();

// JWT authentication token
[RegexMatch(@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$")]
public sealed record AuthToken : SemanticString<AuthToken>;
// Usage: var jwt = "header.payload.signature".As<AuthToken>();
```

### Combining Multiple Validations

```csharp
// Domain name that must end with .com or .org (ValidateAny strategy)
[ValidateAny]
[EndsWith(".com")]
[EndsWith(".org")]
public sealed record TrustedDomain : SemanticString<TrustedDomain>;
// Usage: var domain = "example.com".As<TrustedDomain>();

// Secure URL with multiple requirements (ValidateAll strategy - default)
[ValidateAll]
[StartsWith("https://")]
[Contains(".example.com")]
public sealed record SecureApiUrl : SemanticString<SecureApiUrl>;
// Usage: var url = "https://secure.example.com/api".As<SecureApiUrl>();
```

## Migration from Obsolete Attributes

Some validation attributes validate types that have first-class .NET representations. While these attributes still provide accurate validation, you should prefer the first-class types for better performance, type safety, and API richness.

### ❌ Discouraged: String-based validation
```csharp
// These work but are discouraged
[IsVersion]
public sealed record SoftwareVersion : SemanticString<SoftwareVersion>;

[IsGuid] 
public sealed record UniqueId : SemanticString<UniqueId>;

[IsIpAddress]
public sealed record ServerIpAddress : SemanticString<ServerIpAddress>;

[IsDateTime]
public sealed record EventTimestamp : SemanticString<EventTimestamp>;

[IsTimeSpan]
public sealed record Duration : SemanticString<Duration>;

[IsUri]
public sealed record WebsiteUrl : SemanticString<WebsiteUrl>;

[IsDecimal]
public sealed record Price : SemanticString<Price>;

[IsDouble]
public sealed record Measurement : SemanticString<Measurement>;

[IsInt32]
public sealed record Count : SemanticString<Count>;

[IsBoolean]
public sealed record Flag : SemanticString<Flag>;
```

### ✅ Recommended: First-class .NET types
```csharp
// Use these instead for better type safety and performance
public class SoftwareVersion 
{
    public Version Value { get; }
    public SoftwareVersion(string version) => Value = Version.Parse(version);
    // Rich API: Major, Minor, Build, Revision, comparison operators
}

public class UniqueId 
{
    public Guid Value { get; }  
    public UniqueId() => Value = Guid.NewGuid();
    public UniqueId(string guid) => Value = Guid.Parse(guid);
    // Built-in: NewGuid(), equality, efficient 16-byte storage
}

public class ServerIpAddress 
{
    public IPAddress Value { get; }
    public ServerIpAddress(string ip) => Value = IPAddress.Parse(ip);
    // Rich API: IPv4/IPv6 support, network operations, parsing
}

// For other types, use System.DateTime, System.TimeSpan, System.Uri, 
// System.Decimal, System.Double, System.Int32, System.Boolean directly
```

### When to Still Use String-based Validation

Use the obsolete validation attributes only when you specifically need:
- String-based semantic validation in a larger semantic string system
- Validation as part of a string processing pipeline
- Compatibility with existing semantic string infrastructure

For new code, prefer the first-class .NET types directly.
