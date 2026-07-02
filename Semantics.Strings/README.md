# ktsu.Semantics.Strings

> Strongly-typed, self-validating string wrappers that replace primitive obsession with compile-time-safe domain types.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Strings?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Strings?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Strings?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Strings` is one package in the [ktsu.Semantics](../README.md) family. For the family overview and the other pillars (paths, quantities, music, color) start at the [root README](../README.md).

## Introduction

`ktsu.Semantics.Strings` gives you a base type, `SemanticString<TDerived>`, for defining string-shaped domain types such as `EmailAddress`, `UserId`, or `BlogSlug`. A semantic string validates itself on construction, normalizes its value, carries the whole `System.String` surface, and is distinct from every other semantic type at compile time. An `EmailAddress` will not silently substitute for a `UserId`, so a whole class of "passed the arguments in the wrong order" bugs stops compiling.

Validation is declarative. You attach attributes such as `[IsEmailAddress]` or `[StartsWith("USER_")]` to the type, and the framework runs them whenever an instance is created. Ready-made identifier types (`Uuid`, `Iban`, `Isbn`, and more) live in the companion [`ktsu.Semantics.Strings.Identifiers`](../Semantics.Strings.Identifiers/README.md) package.

## Features

- **`SemanticString<TDerived>` base type**: an abstract record using the curiously-recurring template pattern, so derived types get value equality, ordering, and the full string API for free.
- **Validating factories**: `Create` throws on invalid input, `TryCreate` returns a bool and never throws. Both accept `string`, `char[]`, and `ReadOnlySpan<char>`.
- **Declarative validation attributes**: casing, format, text, and first-class .NET type checks, combined with `[ValidateAll]` (default, logical AND) or `[ValidateAny]` (logical OR).
- **Normalization hook**: override `MakeCanonical` to trim, case-fold, or otherwise canonicalize a value before validation runs.
- **Fluent conversions**: `"user@example.com".As<EmailAddress>()` and cross-type reinterpretation via `source.As<TSource, TTarget>()`.
- **Factory abstraction for dependency injection**: `ISemanticStringFactory<T>` / `SemanticStringFactory<T>` for constructor injection, with a `SemanticStringFactory<T>.Default` singleton for non-DI use.
- **Span-friendly and allocation-conscious**: span-based overloads and a `ref struct` split enumerator on the target frameworks that support them.
- **JSON round-trip serialization**: values serialize as their underlying string via `ktsu.RoundTripStringJsonConverter`.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Strings
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Strings
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Strings" Version="x.y.z" />
```

## Usage Examples

### Basic Example

```csharp
using ktsu.Semantics.Strings;

[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

[StartsWith("USER_"), HasNonWhitespaceContent]
public sealed record UserId : SemanticString<UserId> { }

// Direct construction, no generic argument needed
EmailAddress email = EmailAddress.Create("user@example.com");
UserId userId = UserId.Create("USER_12345");

// Safe creation, no exception on failure
if (EmailAddress.TryCreate("maybe@invalid", out EmailAddress? safe))
{
    // use safe
}

// Compile-time safety
public void SendWelcomeEmail(EmailAddress to, UserId who) { /* ... */ }
// SendWelcomeEmail(userId, email);   // does not compile
```

A semantic string converts implicitly to `string`, so it drops into any API that expects one. Construction is always explicit (`Create` / `As`), which guarantees validation runs.

### Combining attributes

```csharp
// All attributes must pass (default behavior)
[IsEmailAddress, EndsWith(".com")]
public sealed record DotComEmail : SemanticString<DotComEmail> { }

// Any one attribute passing is sufficient
[ValidateAny]
[IsEmailAddress, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

The parameterized text attributes (`Contains`, `StartsWith`, `EndsWith`, `PrefixAndSuffix`, `RegexMatch`) allow multiples, so you can stack several and combine them with `[ValidateAny]`.

### Normalization before validation

```csharp
using ktsu.Semantics.Strings;

[HasNonWhitespaceContent]
public sealed record Slug : SemanticString<Slug>
{
    protected override string MakeCanonical(string input) =>
        input.Trim().ToLowerInvariant().Replace(' ', '-');
}

Slug slug = Slug.Create("  Hello World  ");   // stored as "hello-world"
```

### Dependency injection

The package ships no `AddSemanticStrings()` helper. Register each closed factory type you need:

```csharp
services.AddScoped<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();

public class UserService(ISemanticStringFactory<EmailAddress> emails)
{
    public User CreateUser(string raw) =>
        emails.TryFromString(raw, out EmailAddress? email)
            ? new User(email!)
            : throw new ArgumentException("invalid email");
}
```

For code that is not using a container, `SemanticStringFactory<EmailAddress>.Default` is a ready singleton.

## API Reference

### `SemanticString<TDerived>`

Abstract base record for all semantic string types. `TDerived` is the concrete type itself.

#### Key members

| Name | Signature | Description |
|------|-----------|-------------|
| `WeakString` | `string { get; init; }` | The underlying raw value. |
| `Length` | `int { get; }` | Length of the underlying string. |
| `Create` | `static TDerived Create(string?)` (also `char[]`, `ReadOnlySpan<char>`) | Validates and constructs. Throws `ArgumentException` on invalid input, `ArgumentNullException` on null. |
| `TryCreate` | `static bool TryCreate(string?, out TDerived?)` (also `char[]`, `ReadOnlySpan<char>`) | Returns `false` instead of throwing. |
| `As<TDest>()` | `TDest As<TDest>()` | Reinterprets the value as another semantic type, re-validating against its rules. |
| `MakeCanonical` | `protected virtual string MakeCanonical(string)` | Normalization hook run before validation. |
| `IsValid` | `virtual bool IsValid()` | True when the value is non-null and passes attribute validation. |
| `WithPrefix` / `WithSuffix` | `TDerived WithPrefix(string)` / `TDerived WithSuffix(string)` | Type-safe prefix/suffix transforms. |
| implicit `string` | `implicit operator string(SemanticString<TDerived>?)` | Converts to `string` (null becomes `string.Empty`). |
| `<`, `<=`, `>`, `>=`, `CompareTo` | ordering members | Ordinal comparison on the underlying string. |

The base also forwards the common `System.String` surface (`Contains`, `IndexOf`, `Substring`, `Split`, `Trim`, `StartsWith`, `EndsWith`, casing helpers, and more) plus span-based helpers on the target frameworks that support them.

### `ISemanticStringFactory<T>` / `SemanticStringFactory<T>`

| Name | Return Type | Description |
|------|-------------|-------------|
| `FromString(string?)` | `T` | Creates an instance, throwing on invalid input. |
| `FromCharArray(char[]?)` | `T` | As above from a char array. |
| `TryFromString(string?, out T?)` | `bool` | Non-throwing creation. |
| `SemanticStringFactory<T>.Default` | `SemanticStringFactory<T>` | Shared singleton for non-DI use. |

### Validation attributes

All attributes apply to a class, derive from `SemanticStringValidationAttribute`, and live in the `ktsu.Semantics.Strings` namespace.

| Category | Representative attributes |
|----------|---------------------------|
| Casing | `IsCamelCase`, `IsPascalCase`, `IsSnakeCase`, `IsKebabCase`, `IsMacroCase`, `IsTitleCase`, `IsSentenceCase`, `IsUpperCase`, `IsLowerCase` |
| Format | `HasNonWhitespaceContent`, `IsSingleLine`, `IsMultiLine`, `HasMinimumLines(n)`, `HasMaximumLines(n)`, `HasExactLines(n)`, `IsEmptyOrWhitespace` |
| Text | `Contains(substring)`, `StartsWith(prefix)`, `EndsWith(suffix)`, `PrefixAndSuffix(prefix, suffix)`, `RegexMatch(pattern)`, `IsBase64`, `IsEmailAddress` |
| First-class .NET types | `IsBoolean`, `IsDateTime`, `IsDecimal`, `IsDouble`, `IsInt32`, `IsGuid`, `IsUri`, `IsVersion`, `IsTimeSpan`, `IsIpAddress` |
| Combination markers | `[ValidateAll]` (default), `[ValidateAny]` |

The full catalogue lives in the [validation reference](../docs/validation-reference.md).

## Architecture

Validation is a small strategy/adapter/rule pipeline. A combination **strategy** (`ValidateAllStrategy` / `ValidateAnyStrategy`, chosen by `ValidationStrategyFactory`) decides whether a type's attributes are combined with AND or OR. Each attribute delegates to a **`ValidationAdapter`** that returns a `ValidationResult`. A separate **rule** abstraction (`IValidationRule`, `ValidationRuleBase`) provides an open extension point for adding named, prioritized rules without touching existing code. See the [architecture guide](../docs/architecture.md) for the full picture.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
