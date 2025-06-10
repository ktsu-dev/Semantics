# FluentValidation Integration

This document describes how the Semantics library has been enhanced to use FluentValidation internally for implementing validation attributes while maintaining the existing public API.

## Overview

The validation attributes in the Semantics library now use FluentValidation internally to provide more robust, flexible, and maintainable validation logic. This integration provides several benefits:

- **Rich validation rules**: Access to FluentValidation's extensive set of built-in validators
- **Better error messages**: More descriptive and customizable error messages
- **Composable validation**: Easy combination of multiple validation rules
- **Maintainability**: Cleaner, more readable validation code
- **Extensibility**: Easier to create complex custom validation logic

## Architecture

### FluentValidationAdapter

The `FluentValidationAdapter` class serves as a bridge between FluentValidation and the semantic string validation system:

```csharp
public abstract class FluentValidationAdapter : AbstractValidator<string>
{
    public bool ValidateSemanticString(ISemanticString semanticString);
    public IEnumerable<string> GetValidationErrors(ISemanticString semanticString);
}
```

### FluentSemanticStringValidationAttribute

The `FluentSemanticStringValidationAttribute` is a new base class for validation attributes that use FluentValidation internally:

```csharp
public abstract class FluentSemanticStringValidationAttribute : SemanticStringValidationAttribute
{
    protected abstract FluentValidationAdapter CreateValidator();
    public override bool Validate(ISemanticString semanticString);
    public virtual IEnumerable<string> GetValidationErrors(ISemanticString semanticString);
}
```

## Refactored Attributes

The following validation attributes have been refactored to use FluentValidation:

### Text Validation
- `IsEmailAddressAttribute` - Email format validation with length limits
- `IsBase64Attribute` - Base64 format validation with proper padding checks
- `RegexMatchAttribute` - Pattern matching with customizable regex options
- `StartsWithAttribute` - Prefix validation with string comparison options
- `EndsWithAttribute` - Suffix validation with string comparison options
- `ContainsAttribute` - Substring validation with string comparison options
- `PrefixAndSuffixAttribute` - Combined prefix and suffix validation

### Format Validation
- `IsEmptyOrWhitespaceAttribute` - Validates empty or whitespace-only content
- `IsSingleLineAttribute` - Validates single-line strings (no line breaks)
- `IsMultiLineAttribute` - Validates multi-line strings (contains line breaks)

### Path Validation
- `IsPathAttribute` - Valid path format with character and length limits
- `DoesExistAttribute` - Path existence validation on filesystem
- `IsExtensionAttribute` - File extension format validation
- `IsAbsolutePathAttribute` - Absolute path validation

### Casing Validation
- `IsLowerCaseAttribute` - Lowercase string validation
- `IsUpperCaseAttribute` - Uppercase string validation

### First-Class Type Validation
- `IsGuidAttribute` - GUID format validation

## Benefits of FluentValidation Integration

### 1. Rich Built-in Validators

FluentValidation provides many built-in validators that can be easily composed:

```csharp
RuleFor(value => value)
    .NotEmpty()
    .EmailAddress()
    .MaximumLength(254);
```

### 2. Custom Validation Logic

Complex custom validation can be implemented using the `Must()` method:

```csharp
RuleFor(value => value)
    .Must(BeValidBase64)
    .WithMessage("The value must be a valid Base64 string.")
    .When(value => !string.IsNullOrEmpty(value));
```

### 3. Conditional Validation

Validation rules can be applied conditionally:

```csharp
RuleFor(value => value)
    .Length(6, 10)
    .WithMessage("Product code must be between 6 and 10 characters long.")
    .When(value => !string.IsNullOrEmpty(value));
```

### 4. Better Error Messages

FluentValidation provides more descriptive error messages and allows for easy customization:

```csharp
RuleFor(value => value)
    .NotEmpty()
    .WithMessage("Product code cannot be empty.");

RuleFor(value => value)
    .Must(value => char.IsLetter(value[0]))
    .WithMessage("Product code must start with a letter.");
```

## Creating Custom Validation Attributes

### Example 1: Product Code Validation

This example demonstrates creating a validation attribute for product codes with multiple validation rules:

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsProductCodeAttribute : FluentSemanticStringValidationAttribute
{
    protected override FluentValidationAdapter CreateValidator() => new ProductCodeValidator();

    private sealed class ProductCodeValidator : FluentValidationAdapter
    {
        public ProductCodeValidator()
        {
            // Product codes must not be empty
            RuleFor(value => value)
                .NotEmpty()
                .WithMessage("Product code cannot be empty.");

            // Product codes must be 6-10 characters long
            RuleFor(value => value)
                .Length(6, 10)
                .WithMessage("Product code must be between 6 and 10 characters long.")
                .When(value => !string.IsNullOrEmpty(value));

            // Product codes must start with a letter
            RuleFor(value => value)
                .Must(value => char.IsLetter(value[0]))
                .WithMessage("Product code must start with a letter.")
                .When(value => !string.IsNullOrEmpty(value));

            // Product codes must contain only alphanumeric characters
            RuleFor(value => value)
                .Matches(@"^[A-Za-z][A-Za-z0-9]*$")
                .WithMessage("Product code must contain only alphanumeric characters and start with a letter.")
                .When(value => !string.IsNullOrEmpty(value));
        }
    }
}

// Usage
[IsProductCode]
public sealed record ProductCode : SemanticString<ProductCode> { }
```

### Example 2: Business Email Validation

This example shows how to implement complex business logic with domain restrictions:

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsBusinessEmailAttribute : FluentSemanticStringValidationAttribute
{
    private readonly string[] _allowedDomains;

    public IsBusinessEmailAttribute(params string[] allowedDomains)
    {
        _allowedDomains = allowedDomains ?? [];
    }

    public string[] AllowedDomains => _allowedDomains;

    protected override FluentValidationAdapter CreateValidator() => new BusinessEmailValidator(_allowedDomains);

    private sealed class BusinessEmailValidator : FluentValidationAdapter
    {
        private readonly string[] _allowedDomains;

        public BusinessEmailValidator(string[] allowedDomains)
        {
            _allowedDomains = allowedDomains;

            // Must be a valid email address
            RuleFor(value => value)
                .EmailAddress()
                .WithMessage("Must be a valid email address.")
                .When(value => !string.IsNullOrEmpty(value));

            // Must be from an allowed domain (if domains are specified)
            if (_allowedDomains.Length > 0)
            {
                RuleFor(value => value)
                    .Must(BeFromAllowedDomain)
                    .WithMessage($"Email must be from one of the allowed domains: {string.Join(", ", _allowedDomains)}")
                    .When(value => !string.IsNullOrEmpty(value));
            }

            // Must not be a personal email domain
            RuleFor(value => value)
                .Must(NotBePersonalEmail)
                .WithMessage("Personal email domains are not allowed for business emails.")
                .When(value => !string.IsNullOrEmpty(value));
        }

        private bool BeFromAllowedDomain(string email)
        {
            if (string.IsNullOrEmpty(email) || _allowedDomains.Length == 0)
            {
                return true;
            }

            string? domain = email.Split('@').LastOrDefault()?.ToLowerInvariant();
            return domain != null && _allowedDomains.Any(d => d.Equals(domain, StringComparison.OrdinalIgnoreCase));
        }

        private static bool NotBePersonalEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return true;
            }

            string[] personalDomains = ["gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "aol.com"];
            string? domain = email.Split('@').LastOrDefault()?.ToLowerInvariant();
            return domain == null || !personalDomains.Contains(domain);
        }
    }
}

// Usage
[IsBusinessEmail("company.com", "business.org")]
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }
```

### Example 3: Using Built-in Validation Attributes

You can also use the existing FluentValidation-based attributes:

```csharp
// Email address validation
[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

// File path validation
[IsPath]
public sealed record FilePath : SemanticString<FilePath> { }

// Multiple validation attributes
[IsLowerCase]
[Contains("_")]
[RegexMatch(@"^[a-z]+_[a-z]+$")]
public sealed record SnakeCase : SemanticString<SnakeCase> { }
```

## Migration Guide

### For Existing Code

All existing validation attributes continue to work exactly as before. The changes are internal implementation details that don't affect the public API.

### For New Custom Validation Attributes

When creating new validation attributes:

1. **Inherit from FluentSemanticStringValidationAttribute** instead of `SemanticStringValidationAttribute`
2. **Implement CreateValidator()** to return your custom `FluentValidationAdapter`
3. **Create a validator class** that inherits from `FluentValidationAdapter`
4. **Define validation rules** in the validator's constructor using FluentValidation syntax

## Future Enhancements

The FluentValidation integration opens up possibilities for future enhancements:

- **Async validation**: Support for asynchronous validation rules
- **Dependency injection**: Integration with DI containers for validator dependencies
- **Localization**: Built-in support for localized error messages
- **Conditional validation**: More complex conditional validation scenarios
- **Cross-field validation**: Validation that depends on multiple properties 
