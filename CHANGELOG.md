# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0-pre.0] - 2025-01-28

### Added

-   Initial release of the Semantics library
-   Core `ISemanticString` interface for type-safe string wrappers
-   Base `SemanticString<T>` class with comprehensive validation system
-   Extensive validation attribute library:
    -   String validation: `IsEmailAttribute`, `IsUrlAttribute`, `IsNotEmptyAttribute`, `HasLengthAttribute`
    -   Path validation: `IsPathAttribute`, `IsAbsolutePathAttribute`, `IsRelativePathAttribute`, etc.
    -   Quantity validation: `IsPositiveAttribute`, `IsNegativeAttribute`, `IsInRangeAttribute`
-   Specialized semantic path types:
    -   `Path`, `AbsolutePath`, `RelativePath`
    -   `FilePath`, `DirectoryPath`, `FileName`, `FileExtension`
-   Advanced path operations and file system integration
-   `SemanticQuantity<T>` for numeric types with validation
-   Type-safe conversion methods (`As<T>()`, `FromString<T>()`)
-   Comprehensive XML documentation for all public APIs
-   Extension methods for enhanced usability
-   Validation examples and best practices
-   Support for custom validation attributes
-   Zero-allocation implicit conversions to primitive types
-   Full .NET 9.0 compatibility

### Features

-   **Type Safety**: Eliminate primitive obsession with strongly-typed wrappers
-   **Automatic Validation**: Attribute-based validation system with custom extensibility
-   **Path Handling**: Specialized types for file system operations
-   **Performance**: Optimized for zero-allocation scenarios
-   **Extensibility**: Easy creation of custom semantic types
-   **Documentation**: Complete XML documentation and IntelliSense support

[Unreleased]: https://github.com/ktsu-dev/Semantics/compare/v1.0.0-pre.0...HEAD
[1.0.0-pre.0]: https://github.com/ktsu-dev/Semantics/releases/tag/v1.0.0-pre.0
