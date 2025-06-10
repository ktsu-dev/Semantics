# Architecture Guide

This document provides a detailed overview of the Semantics library architecture, focusing on the SOLID principles and DRY practices implemented throughout the codebase.

## Table of Contents

-   [Overview](#overview)
-   [SOLID Principles Implementation](#solid-principles-implementation)
-   [DRY Implementation](#dry-implementation)
-   [Design Patterns](#design-patterns)
-   [Class Hierarchy](#class-hierarchy)
-   [Validation System](#validation-system)
-   [Testing Strategy](#testing-strategy)

## Overview

The Semantics library is designed around clean architecture principles, with a focus on maintainability, extensibility, and testability. The core philosophy is to provide type-safe string wrappers while maintaining excellent separation of concerns and avoiding code duplication.

## SOLID Principles Implementation

### Single Responsibility Principle (SRP)

Each class has a single, well-defined responsibility:

#### Factory Pattern Implementation

-   **`ISemanticStringFactory<T>`**: Object creation only
-   **`SemanticStringFactory<T>`**: Concrete creation logic
-   **Purpose**: Separates construction from business logic

#### Validation Strategy Separation

-   **`IValidationStrategy`**: Defines validation processing
-   **`ValidateAllStrategy`**: "All must pass" logic
-   **`ValidateAnyStrategy`**: "Any can pass" logic
-   **`ValidationStrategyFactory`**: Strategy creation

### Open/Closed Principle (OCP)

Open for extension, closed for modification:

```csharp
// Add new validation rules without modifying existing code
public class CustomBusinessRule : ValidationRuleBase
{
    public override string RuleName => "CustomBusiness";

    protected override bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        // Custom validation logic
        return true;
    }
}
```

### Liskov Substitution Principle (LSP)

All implementations are fully substitutable through behavioral contracts:

```csharp
// Contract validation ensures LSP compliance
public static class SemanticStringContracts
{
    public static bool ValidateContracts<T>(T instance) where T : SemanticString<T>
    {
        // Validates basic contracts (reflexivity, etc.)
    }

    public static bool ValidateEqualityContracts<T>(T? first, T? second) where T : SemanticString<T>
    {
        // Validates equality behavior
    }
}
```

### Interface Segregation Principle (ISP)

Focused, client-specific interfaces:

-   **`ISemanticStringFactory<T>`**: Factory operations only
-   **`IValidationStrategy`**: Validation strategy operations only
-   **`IValidationRule`**: Individual rule operations only

### Dependency Inversion Principle (DIP)

High-level modules depend on abstractions:

```csharp
// Service depends on abstraction, not concrete implementation
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserService(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }
}
```

## DRY Implementation

### Strategy Pattern for Validation Logic

**Problem Eliminated**: Duplicated validation logic across multiple methods.

**Solution**: Centralized validation strategies with pluggable implementations.

```csharp
public interface IValidationStrategy
{
    bool Validate(IEnumerable<SemanticStringValidationAttribute> attributes, ISemanticString value);
}

// Reusable strategies eliminate duplication
public class ValidateAllStrategy : IValidationStrategy { /* implementation */ }
public class ValidateAnyStrategy : IValidationStrategy { /* implementation */ }
```

### Template Method Pattern for Validation Rules

**Problem Eliminated**: Common validation patterns repeated across attribute types.

**Solution**: Base class with template method for common functionality.

```csharp
public abstract class ValidationRuleBase : IValidationRule
{
    // Template method - common structure, specific implementation in derived classes
    public bool Validate(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        if (!IsApplicable(attribute)) return false;
        if (value?.ToString() is not string stringValue) return false;
        return ValidateCore(attribute, value);  // Specific implementation
    }

    protected abstract bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value);
}
```

## Design Patterns

### Factory Pattern

-   **Purpose**: Encapsulate object creation logic
-   **Implementation**: `ISemanticStringFactory<T>` and `SemanticStringFactory<T>`
-   **Benefits**: Consistent creation, testability, DI support

### Strategy Pattern

-   **Purpose**: Interchangeable validation algorithms
-   **Implementation**: `IValidationStrategy` with concrete strategies
-   **Benefits**: Extensibility, configurable behavior

### Template Method Pattern

-   **Purpose**: Define algorithm structure with customizable steps
-   **Implementation**: `ValidationRuleBase` with abstract methods
-   **Benefits**: Code reuse, consistent structure

## Class Hierarchy

```
ISemanticString
├── SemanticString<TDerived> (abstract base)
    ├── SemanticPath<TDerived> (path-specific base)
    │   ├── SemanticAbsolutePath<TDerived> (absolute paths base)
    │   │   └── AbsolutePath
    │   ├── SemanticRelativePath<TDerived> (relative paths base)
    │   │   └── RelativePath
    │   ├── SemanticFilePath<TDerived> (file paths base)
    │   │   ├── FilePath
    │   │   ├── AbsoluteFilePath
    │   │   └── RelativeFilePath
    │   ├── SemanticDirectoryPath<TDerived> (directory paths base)
    │   │   ├── DirectoryPath
    │   │   ├── AbsoluteDirectoryPath
    │   │   └── RelativeDirectoryPath
    │   ├── FileName
    │   └── FileExtension
    └── [Custom semantic string types]

ISemanticStringFactory<T>
└── SemanticStringFactory<T>

IValidationStrategy
├── ValidateAllStrategy
├── ValidateAnyStrategy
└── [Custom validation strategies]

IValidationRule
├── ValidationRuleBase (abstract)
│   ├── LengthValidationRule
│   ├── PatternValidationRule
│   └── [Custom validation rules]
└── [Custom validation rules]
```

### Path Interface Hierarchy

The library provides a comprehensive interface hierarchy for path types that enables polymorphism and type-safe operations:

```
IPath (base interface for all path types)
├── IAbsolutePath : IPath
├── IRelativePath : IPath
├── IFilePath : IPath
├── IDirectoryPath : IPath
├── IAbsoluteFilePath : IFilePath, IAbsolutePath
├── IRelativeFilePath : IFilePath, IRelativePath
├── IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
└── IRelativeDirectoryPath : IDirectoryPath, IRelativePath

IFileName (separate hierarchy for non-path file components)
IFileExtension (separate hierarchy for file extensions)
```

**Interface Implementation Mapping:**

```csharp
// Path types implement their corresponding interfaces
AbsolutePath : SemanticAbsolutePath<AbsolutePath>, IAbsolutePath
RelativePath : SemanticRelativePath<RelativePath>, IRelativePath
FilePath : SemanticFilePath<FilePath>, IFilePath
DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
RelativeFilePath : SemanticFilePath<RelativeFilePath>, IRelativeFilePath
AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath

// Non-path types have separate interfaces
FileName : SemanticString<FileName>, IFileName
FileExtension : SemanticString<FileExtension>, IFileExtension
```

**Polymorphic Benefits:**

1. **Type-safe Collections**: Store different path types in the same collection using common interfaces
2. **Polymorphic Methods**: Write methods that accept any path type or specific categories
3. **Interface Segregation**: Use the most specific interface needed for your use case
4. **Extensibility**: Easy to add new path types that integrate with existing polymorphic code

## Validation System

### Architecture Overview

The validation system follows a layered approach:

1. **Attribute Layer**: `SemanticStringValidationAttribute` classes
2. **Strategy Layer**: `IValidationStrategy` implementations
3. **Rule Layer**: `IValidationRule` implementations
4. **Factory Layer**: `ValidationStrategyFactory` creates strategies

### Validation Flow

```
User Creates Semantic String
           ↓
    SemanticStringFactory
           ↓
   Get Validation Attributes
           ↓
   ValidationStrategyFactory.GetStrategy()
           ↓
   IValidationStrategy.Validate()
           ↓
   For Each Attribute: IValidationRule.Validate()
           ↓
   Combine Results (All/Any/Custom)
           ↓
   Return Valid Object or Throw Exception
```

## Testing Strategy

### Contract Testing

-   All implementations must pass contract validation
-   `SemanticStringContracts` provides standardized tests
-   Ensures LSP compliance

### Example Test Structure

```csharp
[Test]
public void EmailAddress_ShouldSatisfyContracts()
{
    var email1 = _factory.Create("user1@example.com");
    var email2 = _factory.Create("user2@example.com");
    var email3 = _factory.Create("user3@example.com");

    Assert.IsTrue(SemanticStringContracts.ValidateContracts(email1));
    Assert.IsTrue(SemanticStringContracts.ValidateEqualityContracts(email1, email2));
    Assert.IsTrue(SemanticStringContracts.ValidateComparisonContracts(email1, email2, email3));
}
```

This architecture ensures the library remains maintainable, extensible, and testable while providing excellent performance and type safety.
