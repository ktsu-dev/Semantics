# Semantics.Strings.Identifiers Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship a `Semantics.Strings.Identifiers` NuGet package with six validated identifier string types (`Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`).

**Architecture:** Each type is a thin CRTP record (`sealed record X : SemanticString<X>`) decorated with a single package-local validation attribute. Normalization happens in a `MakeCanonical` override; validation (regex + checksum math) lives in a private `ValidationAdapter` inside each attribute. The framework package (`Semantics.Strings`) is untouched — the new attributes extend its public `NativeSemanticStringValidationAttribute` / `ValidationAdapter` base classes.

**Tech Stack:** C# / .NET (multi-target), `ktsu.Sdk`, `System.Text.Json` (JWT payload parsing), MSTest.

Reference spec: `docs/superpowers/specs/2026-07-01-concrete-semantic-strings-design.md`.

## Global Constraints

Every task's requirements implicitly include this section.

- **TFMs (package):** `net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1`. **TFM (test project):** `net10.0` only.
- **Namespace:** all new production files use `namespace ktsu.Semantics.Strings.Identifiers;` (file-scoped) and add `using ktsu.Semantics.Strings;` to reach the base types. Test files use `namespace ktsu.Semantics.Test.Identifiers;` (repo convention: `ktsu.Semantics.Test.<Area>`), omit the MSTest `using` (it is a global using from `MSTest.Sdk`), and add `using System;` where a `System` type (e.g. `ArgumentException`) is used.
- **File header (verbatim, every `.cs` file):**
  ```csharp
  // Copyright (c) ktsu.dev
  // All rights reserved.
  // Licensed under the MIT license.
  ```
- **Style:** tabs for indentation; file-scoped namespaces; usings inside namespace *is not* used here (this repo places framework usings after the namespace line — follow the exact layout shown in each task); braces on every control-flow statement; explicit accessibility modifiers; no `this.`; nullable enabled; no `var` in test bodies; warnings are errors.
- **Line endings:** CRLF + UTF-8 **no BOM** + trailing newline. After creating/editing any `.cs` file and BEFORE building, run this normalization command (Git Bash):
  ```bash
  find Semantics.Strings.Identifiers Semantics.Test/Identifiers -name '*.cs' -print0 2>/dev/null | while IFS= read -r -d '' f; do
    perl -i -pe 's/\r\n|\n/\r\n/g' "$f"
  done
  ```
  Then verify no BOM (first two bytes must be `2f 2f`, i.e. `//`):
  ```bash
  head -c 2 Semantics.Strings.Identifiers/Uuid.cs | xxd
  ```
- **Regex rules:** anchored patterns, always pass `RegexOptions.None, TimeSpan.FromSeconds(1)` (matches `RegexMatchAttribute`).
- **Empty-string rule:** identifier attributes reject `""` — do **not** add an `if (string.IsNullOrEmpty(value)) return Success();` guard. Each format/structural/checksum check already fails on empty, so `X.Create("")` throws `ArgumentException`.
- **Factory surface (inherited, do not redefine):** `X.Create(string)` throws `ArgumentException` on invalid; `X.TryCreate(string, out X?)` returns `bool`; `X.Create(string)` applies `MakeCanonical` then validates. Records get value equality for free.
- **Build/test commands:**
  - Build package: `dotnet build Semantics.Strings.Identifiers/Semantics.Strings.Identifiers.csproj`
  - Run one test class: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~<ClassName>"`

---

### Task 1: Scaffold the `Semantics.Strings.Identifiers` package

**Files:**
- Create: `Semantics.Strings.Identifiers/Semantics.Strings.Identifiers.csproj`
- Modify: `Semantics.sln` (via `dotnet sln add`)
- Modify: `Semantics.Test/Semantics.Test.csproj`

**Interfaces:**
- Consumes: `Semantics.Strings` public types (`SemanticString<T>`, `NativeSemanticStringValidationAttribute`, `ValidationAdapter`, `ValidationResult`).
- Produces: a buildable, solution-wired, test-referenced empty package that later tasks add types to.

- [ ] **Step 1: Create the csproj**

`Semantics.Strings.Identifiers/Semantics.Strings.Identifiers.csproj`:
```xml
<Project>
  <Sdk Name="Microsoft.NET.Sdk" />
  <Sdk Name="ktsu.Sdk" />
  <PropertyGroup>
    <TargetFrameworks>net10.0;net9.0;net8.0;netstandard2.0;netstandard2.1</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Polyfill" PrivateAssets="all" />
    <PackageReference Include="System.Text.Json" />
    <PackageReference Include="System.Memory" Condition="'$(TargetFramework)' == 'netstandard2.0'" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Semantics.Strings\Semantics.Strings.csproj" />
  </ItemGroup>
  <ItemGroup>
    <InternalsVisibleTo Include="ktsu.Semantics.Test" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Add a placeholder type so the project has content**

`Semantics.Strings.Identifiers/AssemblyMarker.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

/// <summary>Internal marker type; anchors the assembly's root namespace.</summary>
internal static class AssemblyMarker
{
}
```

- [ ] **Step 3: Add the project to the solution**

Run: `dotnet sln Semantics.sln add Semantics.Strings.Identifiers/Semantics.Strings.Identifiers.csproj`
Expected: `Project ... added to the solution.`

- [ ] **Step 4: Reference the package from the test project**

In `Semantics.Test/Semantics.Test.csproj`, add to the existing `<ItemGroup>` of `ProjectReference`s:
```xml
    <ProjectReference Include="..\Semantics.Strings.Identifiers\Semantics.Strings.Identifiers.csproj" />
```

- [ ] **Step 5: Normalize line endings, then build**

Run the normalization command from Global Constraints, then:
Run: `dotnet build Semantics.Strings.Identifiers/Semantics.Strings.Identifiers.csproj`
Expected: `Build succeeded` across all five TFMs, 0 warnings.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers Semantics.sln Semantics.Test/Semantics.Test.csproj
git commit -m "chore(strings): scaffold Semantics.Strings.Identifiers package"
```

---

### Task 2: `Uuid`

**Files:**
- Create: `Semantics.Strings.Identifiers/IsUuidAttribute.cs`
- Create: `Semantics.Strings.Identifiers/Uuid.cs`
- Test: `Semantics.Test/Identifiers/UuidTests.cs`

**Interfaces:**
- Consumes: `SemanticString<T>`, `NativeSemanticStringValidationAttribute`, `ValidationAdapter`, `ValidationResult`.
- Produces: `public sealed record Uuid : SemanticString<Uuid>`; `public sealed class IsUuidAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/UuidTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class UuidTests
{
	[TestMethod]
	public void Create_CanonicalUuid_Succeeds()
	{
		Uuid uuid = Uuid.Create("123e4567-e89b-12d3-a456-426614174000");
		Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_NilUuid_Succeeds()
	{
		Uuid uuid = Uuid.Create("00000000-0000-0000-0000-000000000000");
		Assert.AreEqual("00000000-0000-0000-0000-000000000000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_UppercaseWithBraces_IsCanonicalized()
	{
		Uuid uuid = Uuid.Create("{123E4567-E89B-12D3-A456-426614174000}");
		Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_MissingSegment_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Uuid.Create("123e4567-e89b-12d3-a456"));
	}

	[TestMethod]
	public void Create_NonHexCharacter_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Uuid.Create("123e4567-e89b-12d3-a456-42661417400g"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Uuid.Create(string.Empty));
	}

	[TestMethod]
	public void TryCreate_Invalid_ReturnsFalse()
	{
		bool created = Uuid.TryCreate("not-a-uuid", out Uuid? result);
		Assert.IsFalse(created);
		Assert.IsNull(result);
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~UuidTests"`
Expected: FAIL — compile error, `Uuid` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsUuidAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a canonical RFC 4122 UUID: lowercase 8-4-4-4-12 hexadecimal.
/// Any variant/version is accepted; the value is not version-checked.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUuidAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for UUID validation.</summary>
	/// <returns>A validation adapter for UUIDs.</returns>
	protected override ValidationAdapter CreateValidator() => new UuidValidator();

	private sealed class UuidValidator : ValidationAdapter
	{
		private const string Pattern = "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";

		protected override ValidationResult ValidateValue(string value) =>
			Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1))
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a canonical RFC 4122 UUID (8-4-4-4-12 hexadecimal).");
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/Uuid.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A universally unique identifier (UUID / GUID) in canonical RFC 4122 form: lowercase 8-4-4-4-12
/// hexadecimal, e.g. <c>123e4567-e89b-12d3-a456-426614174000</c>. Wrapping braces or parentheses and
/// uppercase hexadecimal are normalized away on creation. Any variant/version is accepted (including
/// the nil UUID); the value is not version-checked.
/// </summary>
[IsUuid]
public sealed record Uuid : SemanticString<Uuid>
{
	/// <summary>Normalizes the input by trimming wrapping braces/parentheses and lowercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical UUID string.</returns>
	protected override string MakeCanonical(string input) =>
		input.Trim().Trim('{', '}', '(', ')').ToLowerInvariant();
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~UuidTests"`
Expected: PASS — 7 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsUuidAttribute.cs Semantics.Strings.Identifiers/Uuid.cs Semantics.Test/Identifiers/UuidTests.cs
git commit -m "feat(strings): add Uuid identifier type"
```

---

### Task 3: `Ulid`

**Files:**
- Create: `Semantics.Strings.Identifiers/IsUlidAttribute.cs`
- Create: `Semantics.Strings.Identifiers/Ulid.cs`
- Test: `Semantics.Test/Identifiers/UlidTests.cs`

**Interfaces:**
- Produces: `public sealed record Ulid : SemanticString<Ulid>`; `public sealed class IsUlidAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/UlidTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class UlidTests
{
	[TestMethod]
	public void Create_CanonicalUlid_Succeeds()
	{
		Ulid ulid = Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAV");
		Assert.AreEqual("01ARZ3NDEKTSV4RRFFQ69G5FAV", ulid.WeakString);
	}

	[TestMethod]
	public void Create_Lowercase_IsUppercased()
	{
		Ulid ulid = Ulid.Create("01arz3ndektsv4rrffq69g5fav");
		Assert.AreEqual("01ARZ3NDEKTSV4RRFFQ69G5FAV", ulid.WeakString);
	}

	[TestMethod]
	public void Create_WrongLength_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FA"));
	}

	[TestMethod]
	public void Create_ExcludedLetterI_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAI"));
	}

	[TestMethod]
	public void Create_TimestampOverflow_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Ulid.Create("81ARZ3NDEKTSV4RRFFQ69G5FAV"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Ulid.Create(string.Empty));
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~UlidTests"`
Expected: FAIL — `Ulid` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsUlidAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a ULID: 26 uppercase Crockford base32 characters (excluding I, L, O, U)
/// whose first character is in the range 0-7 (the 48-bit timestamp cannot overflow past that).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUlidAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for ULID validation.</summary>
	/// <returns>A validation adapter for ULIDs.</returns>
	protected override ValidationAdapter CreateValidator() => new UlidValidator();

	private sealed class UlidValidator : ValidationAdapter
	{
		private const string Pattern = "^[0-7][0-9A-HJKMNP-TV-Z]{25}$";

		protected override ValidationResult ValidateValue(string value) =>
			Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1))
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a 26-character Crockford base32 ULID.");
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/Ulid.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A ULID (Universally Unique Lexicographically Sortable Identifier): 26 Crockford base32 characters,
/// e.g. <c>01ARZ3NDEKTSV4RRFFQ69G5FAV</c>. Input is uppercased on creation. Validation covers the
/// character set, length, and the first-character timestamp bound only; it does not decode the
/// embedded timestamp beyond that high bit.
/// </summary>
[IsUlid]
public sealed record Ulid : SemanticString<Ulid>
{
	/// <summary>Normalizes the input by trimming and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical ULID string.</returns>
	protected override string MakeCanonical(string input) => input.Trim().ToUpperInvariant();
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~UlidTests"`
Expected: PASS — 6 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsUlidAttribute.cs Semantics.Strings.Identifiers/Ulid.cs Semantics.Test/Identifiers/UlidTests.cs
git commit -m "feat(strings): add Ulid identifier type"
```

---

### Task 4: `CreditCardNumber` (Luhn)

**Files:**
- Create: `Semantics.Strings.Identifiers/IsCreditCardNumberAttribute.cs`
- Create: `Semantics.Strings.Identifiers/CreditCardNumber.cs`
- Test: `Semantics.Test/Identifiers/CreditCardNumberTests.cs`

**Interfaces:**
- Produces: `public sealed record CreditCardNumber : SemanticString<CreditCardNumber>`; `public sealed class IsCreditCardNumberAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/CreditCardNumberTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class CreditCardNumberTests
{
	[TestMethod]
	public void Create_ValidLuhn_Succeeds()
	{
		CreditCardNumber card = CreditCardNumber.Create("4111111111111111");
		Assert.AreEqual("4111111111111111", card.WeakString);
	}

	[TestMethod]
	public void Create_WithSpacesAndHyphens_IsCanonicalized()
	{
		CreditCardNumber card = CreditCardNumber.Create("4111-1111 1111-1111");
		Assert.AreEqual("4111111111111111", card.WeakString);
	}

	[TestMethod]
	public void Create_FailingLuhn_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => CreditCardNumber.Create("4111111111111112"));
	}

	[TestMethod]
	public void Create_TooShort_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => CreditCardNumber.Create("411111111111"));
	}

	[TestMethod]
	public void Create_NonDigit_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => CreditCardNumber.Create("4111a11111111111"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => CreditCardNumber.Create(string.Empty));
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~CreditCardNumberTests"`
Expected: FAIL — `CreditCardNumber` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsCreditCardNumberAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a payment card number: 13-19 digits passing the Luhn (mod-10) check.
/// This is a format/checksum check only — it performs no BIN/issuer/network detection and offers no
/// PCI guarantees. Do not log the underlying value.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsCreditCardNumberAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for credit card number validation.</summary>
	/// <returns>A validation adapter for credit card numbers.</returns>
	protected override ValidationAdapter CreateValidator() => new CreditCardNumberValidator();

	private sealed class CreditCardNumberValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length < 13 || value.Length > 19)
			{
				return ValidationResult.Failure("A credit card number must have between 13 and 19 digits.");
			}

			foreach (char c in value)
			{
				if (c < '0' || c > '9')
				{
					return ValidationResult.Failure("A credit card number must contain digits only.");
				}
			}

			return PassesLuhn(value)
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid credit card number (Luhn check failed).");
		}

		private static bool PassesLuhn(string digits)
		{
			int sum = 0;
			bool alternate = false;
			for (int i = digits.Length - 1; i >= 0; i--)
			{
				int n = digits[i] - '0';
				if (alternate)
				{
					n *= 2;
					if (n > 9)
					{
						n -= 9;
					}
				}

				sum += n;
				alternate = !alternate;
			}

			return (sum % 10) == 0;
		}
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/CreditCardNumber.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A payment card number (PAN): 13-19 digits passing the Luhn checksum. Spaces and hyphens are
/// stripped on creation. Validation is Luhn-only — no BIN/issuer/network detection and no PCI
/// guarantees; treat the value as sensitive and do not log it.
/// </summary>
[IsCreditCardNumber]
public sealed record CreditCardNumber : SemanticString<CreditCardNumber>
{
	/// <summary>Normalizes the input by stripping spaces and hyphens.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The digit-only canonical form.</returns>
	protected override string MakeCanonical(string input) =>
		input.Replace(" ", string.Empty).Replace("-", string.Empty);
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~CreditCardNumberTests"`
Expected: PASS — 6 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsCreditCardNumberAttribute.cs Semantics.Strings.Identifiers/CreditCardNumber.cs Semantics.Test/Identifiers/CreditCardNumberTests.cs
git commit -m "feat(strings): add CreditCardNumber identifier type"
```

---

### Task 5: `Isbn` (ISBN-10 / ISBN-13)

**Files:**
- Create: `Semantics.Strings.Identifiers/IsIsbnAttribute.cs`
- Create: `Semantics.Strings.Identifiers/Isbn.cs`
- Test: `Semantics.Test/Identifiers/IsbnTests.cs`

**Interfaces:**
- Produces: `public sealed record Isbn : SemanticString<Isbn>`; `public sealed class IsIsbnAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/IsbnTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class IsbnTests
{
	[TestMethod]
	public void Create_ValidIsbn10_Succeeds()
	{
		Isbn isbn = Isbn.Create("0-306-40615-2");
		Assert.AreEqual("0306406152", isbn.WeakString);
	}

	[TestMethod]
	public void Create_ValidIsbn10WithXCheckDigit_Succeeds()
	{
		Isbn isbn = Isbn.Create("0-8044-2957-X");
		Assert.AreEqual("080442957X", isbn.WeakString);
	}

	[TestMethod]
	public void Create_ValidIsbn13_Succeeds()
	{
		Isbn isbn = Isbn.Create("978-0-306-40615-7");
		Assert.AreEqual("9780306406157", isbn.WeakString);
	}

	[TestMethod]
	public void Create_BadIsbn10CheckDigit_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Isbn.Create("0-306-40615-3"));
	}

	[TestMethod]
	public void Create_BadIsbn13CheckDigit_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Isbn.Create("978-0-306-40615-8"));
	}

	[TestMethod]
	public void Create_WrongLength_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Isbn.Create("123456789012"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Isbn.Create(string.Empty));
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~IsbnTests"`
Expected: FAIL — `Isbn` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsIsbnAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is an ISBN-10 (weighted mod-11, final digit may be X) or an ISBN-13
/// (mod-10), evaluated on the separator-stripped value. Registration-group and publisher ranges are
/// not validated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsIsbnAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for ISBN validation.</summary>
	/// <returns>A validation adapter for ISBNs.</returns>
	protected override ValidationAdapter CreateValidator() => new IsbnValidator();

	private sealed class IsbnValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length == 10 && IsValidIsbn10(value))
			{
				return ValidationResult.Success();
			}

			if (value.Length == 13 && IsValidIsbn13(value))
			{
				return ValidationResult.Success();
			}

			return ValidationResult.Failure("The value must be a valid ISBN-10 or ISBN-13.");
		}

		private static bool IsValidIsbn10(string s)
		{
			int sum = 0;
			for (int i = 0; i < 10; i++)
			{
				char c = s[i];
				int d;
				if (c >= '0' && c <= '9')
				{
					d = c - '0';
				}
				else if (c == 'X' && i == 9)
				{
					d = 10;
				}
				else
				{
					return false;
				}

				sum += (10 - i) * d;
			}

			return (sum % 11) == 0;
		}

		private static bool IsValidIsbn13(string s)
		{
			int sum = 0;
			for (int i = 0; i < 13; i++)
			{
				char c = s[i];
				if (c < '0' || c > '9')
				{
					return false;
				}

				int d = c - '0';
				sum += ((i % 2) == 0) ? d : (d * 3);
			}

			return (sum % 10) == 0;
		}
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/Isbn.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// An International Standard Book Number in ISBN-10 or ISBN-13 form, e.g. <c>0306406152</c> or
/// <c>9780306406157</c>. Hyphens and spaces are stripped and the value is uppercased (for the ISBN-10
/// <c>X</c> check digit) on creation. The check digit is validated; registration-group and publisher
/// ranges are not.
/// </summary>
[IsIsbn]
public sealed record Isbn : SemanticString<Isbn>
{
	/// <summary>Normalizes the input by stripping hyphens/spaces and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical ISBN string.</returns>
	protected override string MakeCanonical(string input) =>
		input.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~IsbnTests"`
Expected: PASS — 7 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsIsbnAttribute.cs Semantics.Strings.Identifiers/Isbn.cs Semantics.Test/Identifiers/IsbnTests.cs
git commit -m "feat(strings): add Isbn identifier type"
```

---

### Task 6: `Iban` (ISO 7064 mod-97-10)

**Files:**
- Create: `Semantics.Strings.Identifiers/IsIbanAttribute.cs`
- Create: `Semantics.Strings.Identifiers/Iban.cs`
- Test: `Semantics.Test/Identifiers/IbanTests.cs`

**Interfaces:**
- Produces: `public sealed record Iban : SemanticString<Iban>`; `public sealed class IsIbanAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/IbanTests.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class IbanTests
{
	[TestMethod]
	public void Create_ValidIban_Succeeds()
	{
		Iban iban = Iban.Create("GB82WEST12345698765432");
		Assert.AreEqual("GB82WEST12345698765432", iban.WeakString);
	}

	[TestMethod]
	public void Create_WithSpacesAndLowercase_IsCanonicalized()
	{
		Iban iban = Iban.Create("gb82 west 1234 5698 7654 32");
		Assert.AreEqual("GB82WEST12345698765432", iban.WeakString);
	}

	[TestMethod]
	public void Create_BadChecksum_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Iban.Create("GB82WEST12345698765431"));
	}

	[TestMethod]
	public void Create_BadStructure_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Iban.Create("1234WEST12345698765432"));
	}

	[TestMethod]
	public void Create_TooShort_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Iban.Create("GB82WEST1234"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => Iban.Create(string.Empty));
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~IbanTests"`
Expected: FAIL — `Iban` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsIbanAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is an IBAN: two letters, two check digits, then alphanumerics, total
/// length 15-34, satisfying the ISO 7064 mod-97-10 checksum. Country-specific BBAN length/format
/// tables are not enforced.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsIbanAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for IBAN validation.</summary>
	/// <returns>A validation adapter for IBANs.</returns>
	protected override ValidationAdapter CreateValidator() => new IbanValidator();

	private sealed class IbanValidator : ValidationAdapter
	{
		private const string Pattern = "^[A-Z]{2}[0-9]{2}[A-Z0-9]+$";

		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length < 15 || value.Length > 34)
			{
				return ValidationResult.Failure("An IBAN must have between 15 and 34 characters.");
			}

			if (!Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				return ValidationResult.Failure("The value must be a valid IBAN (two letters, two check digits, then alphanumerics).");
			}

			return PassesMod97(value)
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid IBAN (mod-97 checksum failed).");
		}

		private static bool PassesMod97(string iban)
		{
			string rearranged = iban.Substring(4) + iban.Substring(0, 4);
			int remainder = 0;
			foreach (char c in rearranged)
			{
				if (c >= '0' && c <= '9')
				{
					remainder = ((remainder * 10) + (c - '0')) % 97;
				}
				else
				{
					remainder = ((remainder * 100) + (c - 'A' + 10)) % 97;
				}
			}

			return remainder == 1;
		}
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/Iban.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// An International Bank Account Number, e.g. <c>GB82WEST12345698765432</c>. Whitespace is stripped and
/// the value uppercased on creation. Validation covers the generic structure and the ISO 7064
/// mod-97-10 checksum; per-country BBAN length/format tables are not enforced.
/// </summary>
[IsIban]
public sealed record Iban : SemanticString<Iban>
{
	/// <summary>Normalizes the input by stripping spaces and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical IBAN string.</returns>
	protected override string MakeCanonical(string input) =>
		input.Replace(" ", string.Empty).ToUpperInvariant();
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~IbanTests"`
Expected: PASS — 6 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsIbanAttribute.cs Semantics.Strings.Identifiers/Iban.cs Semantics.Test/Identifiers/IbanTests.cs
git commit -m "feat(strings): add Iban identifier type"
```

---

### Task 7: `JwtToken`

**Files:**
- Create: `Semantics.Strings.Identifiers/IsJwtTokenAttribute.cs`
- Create: `Semantics.Strings.Identifiers/JwtToken.cs`
- Test: `Semantics.Test/Identifiers/JwtTokenTests.cs`

**Interfaces:**
- Produces: `public sealed record JwtToken : SemanticString<JwtToken>`; `public sealed class IsJwtTokenAttribute`.

- [ ] **Step 1: Write the failing tests**

`Semantics.Test/Identifiers/JwtTokenTests.cs`. The valid sample is the standard JWT from jwt.io (HS256 header `{"alg":"HS256","typ":"JWT"}`, payload `{"sub":"1234567890","name":"John Doe","iat":1516239022}`):
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using System;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class JwtTokenTests
{
	private const string ValidJwt =
		"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
		"eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ." +
		"SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

	[TestMethod]
	public void Create_ValidJwt_Succeeds()
	{
		JwtToken token = JwtToken.Create(ValidJwt);
		Assert.AreEqual(ValidJwt, token.WeakString);
	}

	[TestMethod]
	public void Create_AlgNoneWithEmptySignature_Succeeds()
	{
		string algNone =
			"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
			"eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.";
		JwtToken token = JwtToken.Create(algNone);
		Assert.AreEqual(algNone, token.WeakString);
	}

	[TestMethod]
	public void Create_TwoSegments_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => JwtToken.Create("header.payload"));
	}

	[TestMethod]
	public void Create_EmptyHeader_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => JwtToken.Create(".eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderNotJsonObject_Throws()
	{
		// "WyJhIl0" is base64url for the JSON array ["a"], which is valid JSON but not an object.
		Assert.ThrowsException<ArgumentException>(() => JwtToken.Create("WyJhIl0.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderNotBase64Url_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => JwtToken.Create("not*base64.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsException<ArgumentException>(() => JwtToken.Create(string.Empty));
	}
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~JwtTokenTests"`
Expected: FAIL — `JwtToken` does not exist.

- [ ] **Step 3: Create the attribute**

`Semantics.Strings.Identifiers/IsJwtTokenAttribute.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.Json;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a structurally well-formed JWT: exactly three '.'-separated segments,
/// with non-empty header and payload segments that base64url-decode to JSON objects. The signature
/// segment may be empty (e.g. <c>alg=none</c>) and is neither decoded nor verified.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsJwtTokenAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for JWT validation.</summary>
	/// <returns>A validation adapter for JWTs.</returns>
	protected override ValidationAdapter CreateValidator() => new JwtTokenValidator();

	private sealed class JwtTokenValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			string[] parts = value.Split('.');
			if (parts.Length != 3)
			{
				return ValidationResult.Failure("A JWT must have exactly three '.'-separated segments.");
			}

			if (parts[0].Length == 0 || parts[1].Length == 0)
			{
				return ValidationResult.Failure("A JWT must have non-empty header and payload segments.");
			}

			if (!DecodesToJsonObject(parts[0]))
			{
				return ValidationResult.Failure("The JWT header must be base64url-encoded JSON object.");
			}

			return DecodesToJsonObject(parts[1])
				? ValidationResult.Success()
				: ValidationResult.Failure("The JWT payload must be base64url-encoded JSON object.");
		}

		private static bool DecodesToJsonObject(string segment)
		{
			string base64 = segment.Replace('-', '+').Replace('_', '/');
			switch (base64.Length % 4)
			{
				case 2:
					base64 += "==";
					break;
				case 3:
					base64 += "=";
					break;
				case 1:
					return false;
				default:
					break;
			}

			byte[] bytes;
			try
			{
				bytes = Convert.FromBase64String(base64);
			}
			catch (FormatException)
			{
				return false;
			}

			try
			{
				using JsonDocument document = JsonDocument.Parse(bytes);
				return document.RootElement.ValueKind == JsonValueKind.Object;
			}
			catch (JsonException)
			{
				return false;
			}
		}
	}
}
```

- [ ] **Step 4: Create the type**

`Semantics.Strings.Identifiers/JwtToken.cs`:
```csharp
// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A JSON Web Token in compact serialization: three '.'-separated base64url segments. The value is
/// stored verbatim (no normalization) because it is opaque and case-sensitive. Validation confirms the
/// header and payload decode to JSON objects; the <b>signature is not verified</b>, and <c>alg</c>,
/// claims, and expiry are not inspected.
/// </summary>
[IsJwtToken]
public sealed record JwtToken : SemanticString<JwtToken>
{
}
```

- [ ] **Step 5: Normalize line endings, then run the tests to verify they pass**

Run the normalization command, then:
Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~JwtTokenTests"`
Expected: PASS — 7 tests.

- [ ] **Step 6: Commit**

```bash
git add Semantics.Strings.Identifiers/IsJwtTokenAttribute.cs Semantics.Strings.Identifiers/JwtToken.cs Semantics.Test/Identifiers/JwtTokenTests.cs
git commit -m "feat(strings): add JwtToken identifier type"
```

---

### Task 8: Full build, remove the scaffold marker, and document

**Files:**
- Delete: `Semantics.Strings.Identifiers/AssemblyMarker.cs`
- Modify: `CLAUDE.md` (project-layout table)

**Interfaces:**
- Consumes: all six types from Tasks 2-7.
- Produces: a clean full-solution build + updated docs.

- [ ] **Step 1: Delete the scaffold marker**

The package now has real content, so the placeholder is no longer needed.
```bash
git rm Semantics.Strings.Identifiers/AssemblyMarker.cs
```

- [ ] **Step 2: Full solution build**

Run: `dotnet build Semantics.sln`
Expected: `Build succeeded`, 0 warnings, 0 errors.

- [ ] **Step 3: Full identifiers test run**

Run: `dotnet test Semantics.Test/Semantics.Test.csproj --filter "FullyQualifiedName~Identifiers"`
Expected: PASS — 39 tests (7 + 6 + 6 + 7 + 6 + 7).

- [ ] **Step 4: Add the package to the CLAUDE.md project-layout table**

In `CLAUDE.md`, in the "Project layout" table, add this row immediately after the `Semantics.Strings` row:
```markdown
| `Semantics.Strings.Identifiers` | Concrete identifier string types (`Uuid`, `Ulid`, `Iban`, `Isbn`, `CreditCardNumber`, `JwtToken`) built on the `Semantics.Strings` framework. |
```

- [ ] **Step 5: Normalize, then commit**

Run the normalization command, then:
```bash
git add -A
git commit -m "chore(strings): finalize Identifiers package and document it"
```

- [ ] **Step 6: Update user-facing docs (skill-driven)**

After the code is committed, invoke the `update-docs` skill to refresh `README.md` / `docs/complete-library-guide.md` with the new package. Do **not** hand-edit `VERSION.md` / `CHANGELOG.md`. The PR/merge commit should carry a `[minor]` tag (new package, additive surface).

---

## Notes for the executor

- **Do not** modify anything under `Semantics.Strings/` — the framework stays generic; all new validators live in the Identifiers package.
- If `dotnet build` reports a per-TFM public-surface ApiCompat error (CP0006/CP0014/CP0016) during pack, add `<EnablePackageValidation>false</EnablePackageValidation>` to the csproj `<PropertyGroup>` with the same justification comment `Semantics.Strings.csproj` carries. The surface here is expected to be uniform, so only do this if the build actually flags it.
- Builds multi-target five frameworks and can be slow; use generous timeouts.
```
