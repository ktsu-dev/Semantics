# Validation Reference

A complete reference of the built-in validation attributes shipped with the library, plus how validation strategies and custom rules fit together.

> Validation runs at construction time. A failed validation throws `ArgumentException` (not `FormatException`) — see CLAUDE.md.

For the architecture (attribute → strategy → rule → factory pipeline), see `architecture.md`. For practical patterns including custom rules, see `advanced-usage.md`.

## At a glance

| Category | Where | Count |
|---|---|---|
| [Text](#text) | `Semantics.Strings/Validation/Attributes/Text/` | 7 |
| [Format](#format) | `Semantics.Strings/Validation/Attributes/Format/` | 7 |
| [Casing](#casing) | `Semantics.Strings/Validation/Attributes/Casing/` | 9 |
| [First-class .NET types](#first-class-net-types) | `Semantics.Strings/Validation/Attributes/FirstClassTypes/` | 10 |
| [Path](#path) | `Semantics.Paths/Validation/Attributes/Path/` | 10 |
| [Strategies](#strategies) | `Semantics.Strings/Validation/Strategies/` | 2 |

There is **no** quantity validation in this list — physics quantities enforce their own invariants at the type level (see `strategy-unified-vector-quantities.md`).

## Text

### `[IsEmailAddress]`
Validates that the value parses as an email address.

```csharp
[IsEmailAddress]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

EmailAddress.Create("user@example.com");   // ✅
EmailAddress.Create("not-an-email");       // ❌ ArgumentException
```

### `[IsBase64]`
Validates that the value is well-formed Base64.

```csharp
[IsBase64]
public sealed record ApiToken : SemanticString<ApiToken> { }
```

### `[StartsWith(prefix)]`, `[EndsWith(suffix)]`, `[Contains(substring)]`
Self-explanatory substring constraints.

```csharp
[StartsWith("https://"), Contains(".example.com")]
public sealed record SecureApiUrl : SemanticString<SecureApiUrl> { }
```

### `[PrefixAndSuffix(prefix, suffix)]`
Convenience for "must start with X and end with Y".

```csharp
[PrefixAndSuffix("Bearer ", "==")]
public sealed record BearerToken : SemanticString<BearerToken> { }
```

### `[RegexMatch(pattern[, options])]`
Arbitrary regex constraint.

```csharp
[RegexMatch(@"^[a-z0-9]+(-[a-z0-9]+)*$")]
public sealed record BlogSlug : SemanticString<BlogSlug> { }
```

## Format

### `[IsEmptyOrWhitespace]` / `[HasNonWhitespaceContent]`
Mutually exclusive — pick one.

### `[IsSingleLine]` / `[IsMultiLine]`
Constrain whether the string contains line breaks.

### `[HasExactLines(n)]`, `[HasMinimumLines(n)]`, `[HasMaximumLines(n)]`
Constrain the line count.

```csharp
[HasMaximumLines(10), HasNonWhitespaceContent]
public sealed record CommitMessageHeader : SemanticString<CommitMessageHeader> { }
```

## Casing

| Attribute | Style | Example |
|---|---|---|
| `[IsCamelCase]` | `myVariable` | `httpRequest` |
| `[IsPascalCase]` | `MyClass` | `HttpRequest` |
| `[IsKebabCase]` | `lower-with-dashes` | `http-request` |
| `[IsSnakeCase]` | `lower_with_underscores` | `http_request` |
| `[IsMacroCase]` | `UPPER_WITH_UNDERSCORES` | `HTTP_REQUEST` |
| `[IsLowerCase]` | all lowercase | `httprequest` |
| `[IsUpperCase]` | all uppercase | `HTTPREQUEST` |
| `[IsSentenceCase]` | first letter upper, rest lower | `Http request` |
| `[IsTitleCase]` | first letter of each word upper | `Http Request` |

## First-class .NET types

These attributes assert that the string parses to a particular .NET type.

| Attribute | Parses as |
|---|---|
| `[IsBoolean]` | `bool` |
| `[IsDateTime]` | `DateTime` |
| `[IsDecimal]` | `decimal` |
| `[IsDouble]` | `double` |
| `[IsGuid]` | `Guid` |
| `[IsInt32]` | `int` |
| `[IsIpAddress]` | `IPAddress` |
| `[IsTimeSpan]` | `TimeSpan` |
| `[IsUri]` | `Uri` |
| `[IsVersion]` | `Version` |

```csharp
[IsGuid]
public sealed record TransactionId : SemanticString<TransactionId> { }

[IsUri]
public sealed record WebsiteUrl : SemanticString<WebsiteUrl> { }
```

> When the value will be used as the parsed type rather than as a string, prefer wrapping the .NET type directly (e.g. `record TransactionId(Guid Value)`). Use these attributes when the value lives inside a wider string-validation pipeline.

## Path

These live in `Semantics.Paths` and require `using ktsu.Semantics.Paths;`.

| Attribute | Validates |
|---|---|
| `[IsPath]` | Legal path characters and length. |
| `[IsValidPath]` | Stricter: also rejects reserved names. |
| `[IsAbsolutePath]` | Fully qualified path. |
| `[IsRelativePath]` | Not absolute. |
| `[IsFilePath]` | Refers to a file (not a directory). |
| `[IsDirectoryPath]` | Refers to a directory. |
| `[IsFileName]` | Filename without separators. |
| `[IsValidFileName]` | Stricter filename validation. |
| `[IsExtension]` | File extension including the leading dot. |
| `[DoesExist]` | The path exists at validation time. Use sparingly — couples the type to the file system. |

```csharp
[IsAbsolutePath, DoesExist]
public sealed record ConfigFilePath : SemanticString<ConfigFilePath> { }
```

For most use cases, prefer the dedicated path types (`AbsoluteFilePath`, `RelativeDirectoryPath`, etc.) from `Semantics.Paths` — they bundle these attributes and provide rich path operations.

## Strategies

By default, all attributes on a type must pass (`ValidateAll` semantics). Strategies override that behaviour.

### `[ValidateAll]` (default)
Every attribute must pass. Equivalent to leaving the strategy attribute off.

### `[ValidateAny]`
At least one attribute must pass.

```csharp
[ValidateAny]
[IsEmailAddress, IsUri]
public sealed record ContactMethod : SemanticString<ContactMethod> { }
```

Need richer logic (e.g. "all critical attributes must pass and at least one secondary attribute must pass")? Implement `IValidationStrategy` and register it via `ValidationStrategyFactory`. See `advanced-usage.md` for the worked example.

## Custom validation attributes

Subclass `SemanticStringValidationAttribute`:

```csharp
public sealed class IsProductCodeAttribute : SemanticStringValidationAttribute
{
    private static readonly Regex Pattern = new(@"^[A-Z][0-9]{5}$", RegexOptions.Compiled);

    public override bool Validate(ISemanticString semanticString) =>
        Pattern.IsMatch(semanticString.ToString());
}

[IsProductCode]
public sealed record ProductCode : SemanticString<ProductCode> { }
```

Validation runs through the attribute → strategy → rule pipeline regardless of whether the attribute is built-in or custom.

## Practical patterns

### Domain-specific types

```csharp
[IsEmailAddress]
public sealed record UserEmail : SemanticString<UserEmail> { }

[RegexMatch(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
public sealed record ThemeColor : SemanticString<ThemeColor> { }

[RegexMatch(@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$")]
public sealed record JwtToken : SemanticString<JwtToken> { }
```

### Combined constraints

```csharp
// Default ValidateAll
[StartsWith("https://"), Contains(".example.com"), HasNonWhitespaceContent]
public sealed record SecureApiUrl : SemanticString<SecureApiUrl> { }

// Either-or
[ValidateAny]
[EndsWith(".com"), EndsWith(".org")]
public sealed record TrustedDomain : SemanticString<TrustedDomain> { }
```

### When to prefer first-class .NET types

For values whose consumer cares about the parsed object (Guid, IPAddress, Uri, …), wrap the .NET type directly instead of validating the string:

```csharp
public sealed record TransactionId(Guid Value)
{
    public static TransactionId New() => new(Guid.NewGuid());
}
```

Use the `[Is*]` attributes when the value belongs in a string-shaped pipeline (logs, configs, serialised payloads) and the parsed object is incidental.
