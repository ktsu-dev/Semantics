# ktsu.Semantics.Strings.Identifiers

> Ready-made, self-validating identifier string types (`Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`) built on the Semantics.Strings framework.

[![License](https://img.shields.io/github/license/ktsu-dev/Semantics.svg?label=License&logo=nuget)](../LICENSE.md)
[![NuGet Version](https://img.shields.io/nuget/v/ktsu.Semantics.Strings.Identifiers?label=Stable&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings.Identifiers)
[![NuGet Version](https://img.shields.io/nuget/vpre/ktsu.Semantics.Strings.Identifiers?label=Latest&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings.Identifiers)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ktsu.Semantics.Strings.Identifiers?label=Downloads&logo=nuget)](https://nuget.org/packages/ktsu.Semantics.Strings.Identifiers)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/ktsu-dev/Semantics?label=Commits&logo=github)](https://github.com/ktsu-dev/Semantics/commits/main)
[![GitHub contributors](https://img.shields.io/github/contributors/ktsu-dev/Semantics?label=Contributors&logo=github)](https://github.com/ktsu-dev/Semantics/graphs/contributors)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ktsu-dev/Semantics/dotnet.yml?label=Build&logo=github)](https://github.com/ktsu-dev/Semantics/actions)

`ktsu.Semantics.Strings.Identifiers` is one package in the [ktsu.Semantics](../README.md) family. It builds on [`ktsu.Semantics.Strings`](../Semantics.Strings/README.md), which you should read first for the underlying `SemanticString<T>` model.

## Introduction

Defining your own semantic string types is the core use of the framework, but a handful of identifier formats show up in almost every codebase. `ktsu.Semantics.Strings.Identifiers` ships six of them ready to use, each with real normalization and real check-digit or structural validation. You get compile-time-distinct types with no boilerplate.

Validation is pragmatic rather than exhaustive, with documented limits. For example `JwtToken` verifies the three-segment structure and that the header and payload decode to JSON objects, but it does **not** verify the signature.

## Features

- **Six identifier types**: `Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`.
- **Normalization on creation**: whitespace and separators stripped, casing folded to the canonical form for each format.
- **Real validation**: check-digit maths for `Iban` (ISO 7064 mod-97), `Isbn` (mod-11 / mod-10), and `CreditCardNumber` (Luhn), structural and format rules for `Uuid`, `Ulid`, and `JwtToken`.
- **Same surface as any semantic string**: `Create`, `TryCreate`, `As<T>()`, value equality, ordering, implicit `string` conversion, and JSON round-tripping, all inherited from `SemanticString<T>`.

## Installation

### Package Manager Console

```powershell
Install-Package ktsu.Semantics.Strings.Identifiers
```

### .NET CLI

```bash
dotnet add package ktsu.Semantics.Strings.Identifiers
```

### Package Reference

```xml
<PackageReference Include="ktsu.Semantics.Strings.Identifiers" Version="x.y.z" />
```

## Usage Examples

### Basic Example

```csharp
using ktsu.Semantics.Strings.Identifiers;

Uuid id = Uuid.Create("{123E4567-E89B-12D3-A456-426614174000}"); // -> "123e4567-e89b-12d3-a456-426614174000"
Ulid ulid = Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAV");
Isbn isbn = Isbn.Create("978-0-306-40615-7");                     // ISBN-10 or ISBN-13, check-digit validated
Iban iban = Iban.Create("GB82 WEST 1234 5698 7654 32");           // whitespace stripped, mod-97 validated

Uuid.Create("not-a-uuid"); // throws ArgumentException
```

### Safe creation and normalization

```csharp
using ktsu.Semantics.Strings.Identifiers;

// TryCreate never throws
if (CreditCardNumber.TryCreate("4111 1111 1111 1111", out CreditCardNumber? card))
{
    // separators stripped, Luhn-checked; card.WeakString == "4111111111111111"
}

// The .As<T>() extension works too
CreditCardNumber same = "4111-1111-1111-1111".As<CreditCardNumber>();
```

Every type inherits the full `SemanticString<T>` factory surface (`Create`, `TryCreate`, the `char[]` and `ReadOnlySpan<char>` overloads, `As<T>()`), so anything shown in the [Semantics.Strings README](../Semantics.Strings/README.md) applies here as well.

## API Reference

Each type is a `sealed record` deriving from `SemanticString<T>`. None declare their own `Create`/`TryCreate`, they inherit them. The table describes what each type normalizes and validates.

| Type | Normalization | Validation | Documented limits |
|------|---------------|------------|-------------------|
| `Uuid` | Trim, strip wrapping `{}` / `()`, lowercase | Canonical 8-4-4-4-12 hex (RFC 4122 layout) | Any variant/version accepted, including the nil UUID. Not version-checked. |
| `Ulid` | Trim, uppercase | 26 Crockford base32 chars, first char `0`-`7` | Timestamp is not otherwise decoded. |
| `Iban` | Strip spaces, uppercase | Length 15-34, country/check prefix, ISO 7064 mod-97-10 checksum | Country-specific BBAN structure is not enforced. |
| `Isbn` | Strip `-` and spaces, uppercase | ISBN-10 (weighted mod-11, `X` allowed) or ISBN-13 (mod-10) | Registration-group/publisher ranges are not validated. |
| `CreditCardNumber` | Strip spaces and hyphens | 13-19 digits, Luhn (mod-10) checksum | Luhn only. No issuer/network detection, no PCI guarantee. The value is sensitive, do not log it. |
| `JwtToken` | None (stored verbatim, case-sensitive) | Three `.`-separated segments, non-empty header and payload that decode to JSON objects | Signature is not decoded or verified. `alg`, claims, and expiry are not inspected. |

Creation follows the base-type contract: `Create(...)` throws `ArgumentException` on invalid input and `ArgumentNullException` on null, while `TryCreate(...)` returns `false` instead.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE.md](../LICENSE.md) file for details.
