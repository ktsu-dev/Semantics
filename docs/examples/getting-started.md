# Getting Started with Semantics Library

This guide covers the fundamentals of using the Semantics library to create type-safe semantic strings.

## What are Semantic Strings?

Semantic strings provide compile-time type safety for string values, preventing common errors like parameter confusion while maintaining full compatibility with System.String operations.

## Your First Semantic String

```csharp
using ktsu.Semantics;

// Define a semantic string type
public sealed record UserName : SemanticString<UserName> { }

// Create instances
var userName = "john_doe".As<UserName>();
Console.WriteLine($"User: {userName}"); // Output: User: john_doe
```

## Creation Methods

There are several ways to create semantic string instances:

```csharp
public sealed record ProductId : SemanticString<ProductId> { }

// Extension method (recommended)
var productId1 = "PROD-123".As<ProductId>();

// Factory method
var productId2 = SemanticString<ProductId>.FromString("PROD-456");

// Explicit conversion
var productId3 = (ProductId)"PROD-789";

// From character array
var productId4 = SemanticString<ProductId>.FromCharArray(['P', 'R', 'O', 'D', '-', '1', '0', '1']);

// From span
var productId5 = SemanticString<ProductId>.FromReadOnlySpan("PROD-202".AsSpan());
```

## Type Safety Benefits

The primary benefit is preventing parameter confusion:

```csharp
public sealed record EmailAddress : SemanticString<EmailAddress> { }
public sealed record PhoneNumber : SemanticString<PhoneNumber> { }

public class UserService
{
    // Parameters cannot be accidentally swapped
    public void CreateUser(EmailAddress email, PhoneNumber phone)
    {
        Console.WriteLine($"Creating user: {email}, {phone}");
    }
}

var email = "user@example.com".As<EmailAddress>();
var phone = "555-1234".As<PhoneNumber>();

var service = new UserService();
service.CreateUser(email, phone);           // ✅ Correct
// service.CreateUser(phone, email);        // ❌ Compile-time error!
```

## String Compatibility

Semantic strings work seamlessly with existing string operations:

```csharp
public sealed record DocumentTitle : SemanticString<DocumentTitle> { }

var title = "Annual Report 2024".As<DocumentTitle>();

// Implicit conversion to string
string titleString = title;

// String properties and methods work naturally
int length = title.Length;                  // 18
char firstChar = title[0];                  // 'A'
bool isEmpty = title.IsEmpty();             // false
bool containsYear = title.Contains("2024"); // true
bool startsWithAnnual = title.StartsWith("Annual"); // true

// String manipulation
string upperTitle = title.ToUpper();        // "ANNUAL REPORT 2024"
string trimmed = title.Trim();
```

## Basic Validation

Semantic strings automatically validate their values:

```csharp
public sealed record PositiveNumber : SemanticString<PositiveNumber>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        if (int.TryParse(WeakString, out int value))
        {
            return value > 0;
        }
        return false;
    }
}

try
{
    var validNumber = "42".As<PositiveNumber>();      // ✅ Valid
    var invalidNumber = "-5".As<PositiveNumber>();    // ❌ Throws FormatException
}
catch (FormatException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
```

## Collections and LINQ

Semantic strings work naturally in collections:

```csharp
public sealed record CategoryName : SemanticString<CategoryName> { }

var categories = new List<CategoryName>
{
    "Electronics".As<CategoryName>(),
    "Books".As<CategoryName>(),
    "Clothing".As<CategoryName>(),
    "Sports".As<CategoryName>()
};

// LINQ operations work naturally
var sortedCategories = categories.OrderBy(c => c).ToList();
var longCategories = categories.Where(c => c.Length > 5).ToList();
var electronicsCategory = categories.FirstOrDefault(c => c.Contains("Electronics"));

// Dictionary usage
var categoryIds = new Dictionary<CategoryName, int>
{
    ["Electronics".As<CategoryName>()] = 1,
    ["Books".As<CategoryName>()] = 2,
    ["Clothing".As<CategoryName>()] = 3
};

// HashSet usage
var uniqueCategories = new HashSet<CategoryName>(categories);
```

## Comparison and Equality

Semantic strings support natural comparison operations:

```csharp
public sealed record Version : SemanticString<Version> { }

var version1 = "1.0.0".As<Version>();
var version2 = "1.0.1".As<Version>();
var version3 = "1.0.0".As<Version>();

// Equality
Console.WriteLine(version1 == version3);    // True
Console.WriteLine(version1 != version2);    // True
Console.WriteLine(version1.Equals(version3)); // True

// Comparison
Console.WriteLine(version1 < version2);     // True (lexicographic)
Console.WriteLine(version2 > version1);     // True
Console.WriteLine(version1.CompareTo(version2)); // -1

// Sorting
var versions = new[] { version2, version1, version3 }.OrderBy(v => v).ToArray();
```

## Error Handling

Handle validation errors gracefully:

```csharp
public sealed record EmailAddress : SemanticString<EmailAddress>
{
    public override bool IsValid()
    {
        return base.IsValid() && WeakString.Contains("@") && WeakString.Contains(".");
    }
}

// Try-catch approach
try
{
    var email = "invalid-email".As<EmailAddress>();
}
catch (FormatException ex)
{
    Console.WriteLine($"Invalid email: {ex.Message}");
}

// Validation check approach
string emailInput = "user@example.com";
var testEmail = SemanticString<EmailAddress>.FromStringInternal(emailInput);
if (testEmail.IsValid())
{
    var validEmail = emailInput.As<EmailAddress>();
    Console.WriteLine($"Valid email: {validEmail}");
}
else
{
    Console.WriteLine("Email validation failed");
}
```

## String Enumeration

You can enumerate characters directly:

```csharp
public sealed record CodeSnippet : SemanticString<CodeSnippet> { }

var code = "Hello123".As<CodeSnippet>();

// Character enumeration
foreach (char c in code)
{
    Console.WriteLine($"Character: {c}");
}

// LINQ on characters
int letterCount = code.Count(char.IsLetter);     // 5
int digitCount = code.Count(char.IsDigit);       // 3
bool hasSpecialChars = code.Any(c => !char.IsLetterOrDigit(c)); // false

// Find specific characters
var upperCaseLetters = code.Where(char.IsUpper).ToArray(); // ['H']
```

## Working with WeakString

When you need to interoperate with non-semantic APIs:

```csharp
public sealed record ApiKey : SemanticString<ApiKey> { }

var apiKey = "key_12345".As<ApiKey>();

// Access the underlying string value
string rawKey = apiKey.WeakString;

// Pass to external APIs that expect strings
await SomeExternalApi.AuthenticateAsync(apiKey.WeakString);

// Implicit conversion is often preferred
await SomeExternalApi.AuthenticateAsync(apiKey); // Implicit conversion to string
```

## Next Steps

Now that you understand the basics, explore:

-   **[Validation Attributes](validation-attributes.md)** - Add automatic validation rules
-   **[Type Conversions](type-conversions.md)** - Convert between semantic types safely
-   **[Factory Pattern](factory-pattern.md)** - Use factories for dependency injection
-   **[Path Handling](path-handling.md)** - Work with file system paths
-   **[Real-World Scenarios](real-world-scenarios.md)** - See complete domain examples

## Common Patterns

### Record Types (Recommended)

```csharp
// Use sealed records for semantic strings
public sealed record UserId : SemanticString<UserId> { }
public sealed record OrderNumber : SemanticString<OrderNumber> { }
```

### Value Objects

```csharp
public sealed record Money : SemanticString<Money>
{
    protected override string MakeCanonical(string input)
    {
        // Always format as currency
        if (decimal.TryParse(input.Replace("$", ""), out decimal amount))
        {
            return $"${amount:F2}";
        }
        return input;
    }
}

var price = "19.99".As<Money>(); // Automatically becomes "$19.99"
```

### Domain-Specific Types

```csharp
public sealed record CustomerId : SemanticString<CustomerId> { }
public sealed record OrderId : SemanticString<OrderId> { }
public sealed record ProductCode : SemanticString<ProductCode> { }

public class Order
{
    public OrderId Id { get; init; }
    public CustomerId CustomerId { get; init; }
    public List<ProductCode> ProductCodes { get; init; } = new();
}

// Type safety prevents mixing up IDs
var order = new Order
{
    Id = "ORD-001".As<OrderId>(),
    CustomerId = "CUST-123".As<CustomerId>(),
    ProductCodes = new() { "PROD-A".As<ProductCode>(), "PROD-B".As<ProductCode>() }
};
```
