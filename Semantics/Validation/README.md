# Validation Directory Organization

This directory contains all validation-related components for the Semantics library, organized by functionality and purpose.

## Directory Structure

### `/Attributes/`
Contains validation attributes organized by domain:

#### `/Attributes/Text/`
Text content and format validation:
- `IsBase64Attribute` - Base64 format validation
- `IsEmailAddressAttribute` - Email address format validation  
- `StartsWithAttribute` - Prefix validation
- `EndsWithAttribute` - Suffix validation
- `ContainsAttribute` - Substring validation
- `RegexMatchAttribute` - Regular expression pattern matching
- `PrefixAndSuffixAttribute` - Combined prefix/suffix validation

#### `/Attributes/FirstClassTypes/` ⚠️ OBSOLETE
Validation attributes for types with first-class .NET representations:
- `IsVersionAttribute` - Use `System.Version` instead
- `IsGuidAttribute` - Use `System.Guid` instead  
- `IsIpAddressAttribute` - Use `System.Net.IPAddress` instead
- `IsDateTimeAttribute` - Use `System.DateTime` instead
- `IsTimeSpanAttribute` - Use `System.TimeSpan` instead
- `IsUriAttribute` - Use `System.Uri` instead
- `IsDecimalAttribute` - Use `System.Decimal` instead
- `IsDoubleAttribute` - Use `System.Double` instead
- `IsInt32Attribute` - Use `System.Int32` instead
- `IsBooleanAttribute` - Use `System.Boolean` instead

> **Note**: These attributes are marked as obsolete and should be avoided in new code. Use the corresponding first-class .NET types for better type safety, performance, and API richness.

#### `/Attributes/Path/`
File system path validation:
- `IsPathAttribute` - General path validation
- `IsAbsolutePathAttribute` - Absolute path validation
- `IsRelativePathAttribute` - Relative path validation
- `IsFilePathAttribute` - File path validation
- `IsDirectoryPathAttribute` - Directory path validation
- `IsFileNameAttribute` - File name validation
- `IsExtensionAttribute` - File extension validation
- `IsValidPathAttribute` - Valid path character validation
- `IsValidFileNameAttribute` - Valid filename character validation
- `DoesExistAttribute` - File system existence validation

#### `/Attributes/Format/`
Text formatting and structure validation:
- `IsSingleLineAttribute` - Single line validation
- `IsMultiLineAttribute` - Multi-line validation
- `IsEmptyOrWhitespaceAttribute` - Empty/whitespace validation
- `HasNonWhitespaceContentAttribute` - Non-whitespace content validation
- `HasMinimumLinesAttribute` - Minimum line count validation
- `HasMaximumLinesAttribute` - Maximum line count validation
- `HasExactLinesAttribute` - Exact line count validation

#### `/Attributes/Casing/`
Text casing validation:
- `IsLowerCaseAttribute` - Lowercase validation
- `IsUpperCaseAttribute` - Uppercase validation
- `IsCamelCaseAttribute` - camelCase validation
- `IsPascalCaseAttribute` - PascalCase validation
- `IsSnakeCaseAttribute` - snake_case validation
- `IsKebabCaseAttribute` - kebab-case validation
- `IsMacroCaseAttribute` - MACRO_CASE validation
- `IsSentenceCaseAttribute` - Sentence case validation
- `IsTitleCaseAttribute` - Title Case validation

#### `/Attributes/Quantity/` 
Numeric and quantity validation (placeholder for future attributes):
- Reserved for attributes like `IsPositiveAttribute`, `IsNegativeAttribute`, `IsInRangeAttribute`, etc.

### `/Strategies/`
Validation strategy implementations and attributes:
- `IValidationStrategy` - Strategy interface
- `ValidateAllStrategy` - Requires all validations to pass (default)
- `ValidateAnyStrategy` - Requires any validation to pass
- `ValidationStrategyFactory` - Creates appropriate strategies
- `ValidateAllAttribute` - Marks types to use ValidateAll strategy
- `ValidateAnyAttribute` - Marks types to use ValidateAny strategy

### `/Rules/`
Validation rule implementations:
- `IValidationRule` - Rule interface
- `ValidationRuleBase` - Base rule implementation
- `PatternValidationRule` - Pattern-based validation
- `LengthValidationRule` - Length-based validation

## Core Files

- `SemanticStringValidationAttribute.cs` - Base validation attribute class
- `SemanticStringValidationAttributes.cs` - Validation helper utilities
- `SemanticPathValidationAttributes.cs` - Path-specific validation utilities

## Usage Guidelines

1. **Prefer first-class .NET types** over obsolete validation attributes
2. **Use appropriate directories** when adding new validation attributes
3. **Follow naming conventions**: `Is{Condition}Attribute` for validation attributes
4. **Document validation rules** clearly in XML comments
5. **Consider validation strategies** for complex multi-attribute scenarios

## Migration Guide

When migrating from obsolete attributes in `/FirstClassTypes/`:

```csharp
// ❌ Obsolete approach
[IsVersion]
public sealed record SoftwareVersion : SemanticString<SoftwareVersion>;

// ✅ Recommended approach  
public class SoftwareVersion 
{
    public Version Value { get; }
    public SoftwareVersion(string version) => Value = Version.Parse(version);
}
```

See the [validation reference documentation](../../docs/validation-reference.md) for complete examples and migration guidance. 
