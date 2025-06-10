# Validation Attributes

This guide covers all the built-in validation attributes and how to control validation logic in the Semantics library.

## Built-in String Validation Attributes

### Text Pattern Validation

```csharp
using ktsu.Semantics;

// Email validation using regex
[RegexMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

// Phone number validation
[RegexMatch(@"^\(\d{3}\) \d{3}-\d{4}$")]
public sealed record USPhoneNumber : SemanticString<USPhoneNumber> { }

// Social Security Number
[RegexMatch(@"^\d{3}-\d{2}-\d{4}$")]
public sealed record SSN : SemanticString<SSN> { }

// Usage examples
var email = "user@example.com".As<EmailAddress>();
var phone = "(555) 123-4567".As<USPhoneNumber>();
var ssn = "123-45-6789".As<SSN>();
```

### Prefix and Suffix Validation

```csharp
// URL validation with multiple prefixes
[StartsWith("http://", StringComparison.OrdinalIgnoreCase)]
[StartsWith("https://", StringComparison.OrdinalIgnoreCase)]
public sealed record WebUrl : SemanticString<WebUrl> { }

// File extension validation
[EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".doc", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".docx", StringComparison.OrdinalIgnoreCase)]
public sealed record DocumentFile : SemanticString<DocumentFile> { }

// Combined prefix and suffix
[PrefixAndSuffix("API_", "_v1", StringComparison.OrdinalIgnoreCase)]
public sealed record ApiKey : SemanticString<ApiKey> { }

// Usage examples
var url = "https://example.com".As<WebUrl>();
var document = "report.PDF".As<DocumentFile>(); // Case insensitive
var apiKey = "API_secret_key_v1".As<ApiKey>();
```

### Content Validation

```csharp
// Must contain specific substring
[Contains("@")]
public sealed record EmailString : SemanticString<EmailString> { }

// Must contain hashtag
[Contains("#")]
public sealed record HashtagString : SemanticString<HashtagString> { }

// Usage examples
var email = "user@domain.com".As<EmailString>();
var hashtag = "#programming".As<HashtagString>();
```

## Path Validation Attributes

### Basic Path Types

```csharp
// General path validation
[IsPath]
public sealed record FilePath : SemanticString<FilePath> { }

// Relative path only
[IsRelativePath]
public sealed record RelativePath : SemanticString<RelativePath> { }

// Absolute path only
[IsAbsolutePath]
public sealed record AbsolutePath : SemanticString<AbsolutePath> { }

// Directory paths only
[IsDirectoryPath]
public sealed record DirectoryPath : SemanticString<DirectoryPath> { }

// File paths only
[IsFilePath]
public sealed record FileOnlyPath : SemanticString<FileOnlyPath> { }

// Filename only (no path separators)
[IsFileName]
public sealed record FileName : SemanticString<FileName> { }

// File extension
[IsExtension]
public sealed record FileExtension : SemanticString<FileExtension> { }

// Usage examples
var absolutePath = @"C:\Users\John\Documents\file.txt".As<AbsolutePath>();
var relativePath = @"docs\readme.txt".As<RelativePath>();
var fileName = "document.pdf".As<FileName>();
var extension = ".txt".As<FileExtension>();
```

### Path Existence Validation

```csharp
// Must exist on filesystem
[DoesExist]
public sealed record ExistingPath : SemanticString<ExistingPath> { }

// Example with temporary file
string tempFile = Path.GetTempFileName();
File.WriteAllText(tempFile, "test content");

try
{
    var existingPath = tempFile.As<ExistingPath>(); // ✅ Works
    Console.WriteLine($"File exists: {existingPath}");
}
finally
{
    File.Delete(tempFile);
}
```

## Validation Logic Control

### Default Behavior (ValidateAll)

By default, ALL validation attributes must pass:

```csharp
// All attributes must pass (default behavior)
[StartsWith("ID-")]
[RegexMatch(@"^ID-\d{6}$")]
public sealed record StrictIdentifier : SemanticString<StrictIdentifier> { }

// Explicit ValidateAll (same as default)
[ValidateAll]
[StartsWith("PRD-")]
[RegexMatch(@"^PRD-\d{4}$")]
public sealed record ProductCode : SemanticString<ProductCode> { }

// Usage examples
var strictId = "ID-123456".As<StrictIdentifier>(); // Must satisfy ALL rules
var productCode = "PRD-1234".As<ProductCode>();
```

### ValidateAny (OR Logic)

Any single attribute can pass:

```csharp
// Any of these extensions is acceptable
[ValidateAny]
[EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".png", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".gif", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)]
public sealed record ImageFileName : SemanticString<ImageFileName> { }

// Any of these domains is acceptable
[ValidateAny]
[EndsWith("@company.com", StringComparison.OrdinalIgnoreCase)]
[EndsWith("@contractor.com", StringComparison.OrdinalIgnoreCase)]
[EndsWith("@partner.org", StringComparison.OrdinalIgnoreCase)]
public sealed record AuthorizedEmail : SemanticString<AuthorizedEmail> { }

// Usage examples
var image1 = "photo.jpg".As<ImageFileName>();    // ✅ Valid (.jpg)
var image2 = "icon.PNG".As<ImageFileName>();     // ✅ Valid (.png, case insensitive)
var email1 = "john@company.com".As<AuthorizedEmail>(); // ✅ Valid
```

## Custom Validation Attributes

### Simple Custom Validation

```csharp
// Custom validation for color names
public sealed class IsValidColorAttribute : SemanticStringValidationAttribute
{
    private static readonly string[] ValidColors =
    {
        "red", "green", "blue", "yellow", "orange", "purple",
        "black", "white", "gray", "pink", "brown", "cyan"
    };

    public override bool Validate(ISemanticString semanticString)
    {
        string color = semanticString.ToString().ToLowerInvariant();
        return ValidColors.Contains(color);
    }
}

[IsValidColor]
public sealed record ColorName : SemanticString<ColorName> { }

// Usage examples
var red = "Red".As<ColorName>();        // ✅ Valid (case insensitive)
var blue = "BLUE".As<ColorName>();      // ✅ Valid
// var invalid = "magenta".As<ColorName>(); // ❌ Not in allowed list
```

### Parameterized Custom Validation

```csharp
// Custom validation with parameters
public sealed class IsInRangeAttribute : SemanticStringValidationAttribute
{
    private readonly int _min;
    private readonly int _max;

    public IsInRangeAttribute(int min, int max)
    {
        _min = min;
        _max = max;
    }

    public override bool Validate(ISemanticString semanticString)
    {
        if (int.TryParse(semanticString.ToString(), out int value))
        {
            return value >= _min && value <= _max;
        }
        return false;
    }
}

[IsInRange(1, 100)]
public sealed record Percentage : SemanticString<Percentage> { }

[IsInRange(1900, 2100)]
public sealed record Year : SemanticString<Year> { }

// Usage examples
var percentage = "85".As<Percentage>();  // ✅ Valid (85 is between 1-100)
var year = "2024".As<Year>();           // ✅ Valid
```

## Validation with Canonicalization

Combine validation with automatic input normalization:

```csharp
[RegexMatch(@"^[A-Z]{3}-\d{4}$")] // Must match after canonicalization
public sealed record NormalizedCode : SemanticString<NormalizedCode>
{
    protected override string MakeCanonical(string input)
    {
        // Remove spaces and convert to uppercase
        var cleaned = input.Replace(" ", "").Replace("-", "").ToUpperInvariant();
        if (cleaned.Length == 7)
        {
            return cleaned.Insert(3, "-"); // Add hyphen at position 3
        }
        return input.ToUpperInvariant();
    }
}

// Usage examples - all become "ABC-1234"
var code1 = "abc1234".As<NormalizedCode>();     // Lowercase input
var code2 = "ABC 1234".As<NormalizedCode>();    // With space
var code3 = "abc-1234".As<NormalizedCode>();    // Mixed case with hyphen

Console.WriteLine(code1); // Output: ABC-1234
Console.WriteLine(code2); // Output: ABC-1234
Console.WriteLine(code3); // Output: ABC-1234
```

## Best Practices

### 1. Use Appropriate Validation Attributes

```csharp
// Good: Specific validation for domain
[RegexMatch(@"^[A-Z]{2}\d{6}$")]
public sealed record CustomerCode : SemanticString<CustomerCode> { }

// Better: More specific with multiple constraints
[RegexMatch(@"^CU\d{6}$")]
public sealed record CustomerCodeSpecific : SemanticString<CustomerCodeSpecific> { }
```

### 2. Combine Built-in Attributes

```csharp
// Comprehensive email validation
[Contains("@")]
[Contains(".")]
[RegexMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
public sealed record ComprehensiveEmail : SemanticString<ComprehensiveEmail> { }
```

### 3. Use ValidateAny for Flexibility

```csharp
// Support multiple ID formats
[ValidateAny]
[RegexMatch(@"^EMP-\d{6}$")]  // Employee ID format
[RegexMatch(@"^CONT-\d{4}$")] // Contractor ID format
[RegexMatch(@"^TEMP-\d{3}$")] // Temporary ID format
public sealed record WorkerId : SemanticString<WorkerId> { }
```

This validation system ensures your semantic strings meet exact business requirements while providing clear error messages when validation fails.
