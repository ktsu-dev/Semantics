# Type Conversions

This guide demonstrates the various ways to convert between semantic string types and perform type-safe operations.

## The .As<T>() Method

The primary method for creating and converting semantic strings:

```csharp
using ktsu.Semantics;

// Define semantic types
public sealed record UserId : SemanticString<UserId> { }
public sealed record ProductId : SemanticString<ProductId> { }
public sealed record CustomerEmail : SemanticString<CustomerEmail> { }

// Create instances using .As<T>()
var userId = "user_123".As<UserId>();
var productId = "PROD-456".As<ProductId>();
var email = "customer@example.com".As<CustomerEmail>();

Console.WriteLine($"User: {userId}");
Console.WriteLine($"Product: {productId}");
Console.WriteLine($"Email: {email}");
```

## Cross-Type Conversions

Convert from one semantic type to another:

```csharp
public sealed record OrderNumber : SemanticString<OrderNumber> { }
public sealed record InvoiceNumber : SemanticString<InvoiceNumber> { }

// Original order number
var orderNumber = "ORD-12345".As<OrderNumber>();

// Convert to invoice number (shares same format)
var invoiceNumber = orderNumber.As<InvoiceNumber>();

Console.WriteLine($"Order: {orderNumber}");     // ORD-12345
Console.WriteLine($"Invoice: {invoiceNumber}"); // ORD-12345
```

## Validation During Conversion

Type conversions respect validation rules:

```csharp
[RegexMatch(@"^EMP-\d{6}$")]
public sealed record EmployeeId : SemanticString<EmployeeId> { }

[RegexMatch(@"^DEPT-\d{3}$")]
public sealed record DepartmentId : SemanticString<DepartmentId> { }

var validEmployeeId = "EMP-123456".As<EmployeeId>();

try
{
    // This will fail validation - wrong format for DepartmentId
    var invalidDept = validEmployeeId.As<DepartmentId>();
}
catch (FormatException ex)
{
    Console.WriteLine($"Conversion failed: {ex.Message}");
}

// Valid conversion with proper format
var validDeptString = "DEPT-123";
var deptId = validDeptString.As<DepartmentId>();
```

## Implicit String Conversions

Semantic strings automatically convert to regular strings:

```csharp
public sealed record DocumentTitle : SemanticString<DocumentTitle> { }

var title = "Annual Report 2024".As<DocumentTitle>();

// Implicit conversion to string
string titleString = title;
string fileName = $"{title}.pdf";

// Use in string operations
bool containsYear = title.Contains("2024");
string upperTitle = title.ToUpper();
int length = title.Length;
```

## Explicit Type Conversions

Use explicit casting when needed:

```csharp
public sealed record ProductCode : SemanticString<ProductCode> { }

// Explicit conversion from string
var productCode = (ProductCode)"PROD-789";

// Explicit conversion to string
string codeString = (string)productCode;

Console.WriteLine($"Product Code: {productCode}");
Console.WriteLine($"As String: {codeString}");
```

## Safe Conversion Patterns

### Try-Convert Pattern

```csharp
public sealed record SafeIdentifier : SemanticString<SafeIdentifier>
{
    public override bool IsValid()
    {
        return base.IsValid() && WeakString.Length >= 5;
    }
}

// Safe conversion method
public static bool TryConvertToSafeId(string input, out SafeIdentifier? safeId)
{
    safeId = null;
    try
    {
        safeId = input.As<SafeIdentifier>();
        return true;
    }
    catch (FormatException)
    {
        return false;
    }
}

// Usage
if (TryConvertToSafeId("VALID_ID", out var safeId))
{
    Console.WriteLine($"Valid ID: {safeId}");
}
else
{
    Console.WriteLine("Invalid ID format");
}
```

## Converting Collections

Work with collections of different semantic types:

```csharp
public sealed record TagName : SemanticString<TagName> { }
public sealed record CategoryName : SemanticString<CategoryName> { }

// Convert collection of strings to semantic types
var tagStrings = new[] { "technology", "programming", "dotnet" };
var tags = tagStrings.Select(s => s.As<TagName>()).ToList();

// Convert between semantic types
var categories = tags.Select(tag => tag.As<CategoryName>()).ToList();

// Display results
Console.WriteLine("Tags:");
tags.ForEach(tag => Console.WriteLine($"  - {tag}"));

Console.WriteLine("Categories:");
categories.ForEach(cat => Console.WriteLine($"  - {cat}"));
```

## Format Conversions with Canonicalization

Automatic format normalization during conversion:

```csharp
public sealed record PhoneNumber : SemanticString<PhoneNumber>
{
    protected override string MakeCanonical(string input)
    {
        // Remove all non-digits and format as (XXX) XXX-XXXX
        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.Length == 10)
        {
            return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
        }

        return input; // Return original if not 10 digits
    }
}

// Various input formats become standardized
var phone1 = "5551234567".As<PhoneNumber>();           // Raw digits
var phone2 = "555-123-4567".As<PhoneNumber>();         // Dashed format
var phone3 = "(555) 123-4567".As<PhoneNumber>();       // Already formatted
var phone4 = "555.123.4567".As<PhoneNumber>();         // Dotted format

// All output the same format: (555) 123-4567
Console.WriteLine(phone1); // (555) 123-4567
Console.WriteLine(phone2); // (555) 123-4567
Console.WriteLine(phone3); // (555) 123-4567
Console.WriteLine(phone4); // (555) 123-4567
```

## Type Safety Benefits

Demonstrate how conversions maintain type safety:

```csharp
public sealed record OrderId : SemanticString<OrderId> { }
public sealed record CustomerId : SemanticString<CustomerId> { }

public class OrderService
{
    public void ProcessOrder(OrderId orderId, CustomerId customerId)
    {
        Console.WriteLine($"Processing order {orderId} for customer {customerId}");
    }
}

var orderService = new OrderService();

// Type-safe conversions prevent parameter confusion
var orderId = "ORD-123".As<OrderId>();
var customerId = "CUST-456".As<CustomerId>();

orderService.ProcessOrder(orderId, customerId);           // ✅ Correct
// orderService.ProcessOrder(customerId, orderId);        // ❌ Compile-time error!

// Even with same underlying values, types prevent confusion
var orderId2 = "ABC-999".As<OrderId>();
var customerId2 = "ABC-999".As<CustomerId>();

orderService.ProcessOrder(orderId2, customerId2);        // ✅ Correct types
// orderService.ProcessOrder(customerId2, orderId2);     // ❌ Still prevents confusion!
```

This type conversion system provides flexibility while maintaining type safety and validation guarantees throughout your application.
