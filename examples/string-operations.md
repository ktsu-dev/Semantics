# String Operations

This guide demonstrates how semantic strings provide full compatibility with System.String operations while maintaining type safety.

## String Property Access

Semantic strings expose all standard string properties:

```csharp
using ktsu.Semantics;

public sealed record MessageText : SemanticString<MessageText> { }

var message = "Hello, World! Welcome to Semantic Strings.".As<MessageText>();

// String properties work naturally
Console.WriteLine($"Length: {message.Length}");              // 42
Console.WriteLine($"Is Empty: {message.IsEmpty()}");         // false
Console.WriteLine($"First char: {message[0]}");              // 'H'
Console.WriteLine($"Last char: {message[message.Length - 1]}"); // '.'
```

## String Methods

All standard string methods are available:

```csharp
public sealed record DocumentContent : SemanticString<DocumentContent> { }

var content = "  The Quick Brown Fox Jumps Over The Lazy Dog  ".As<DocumentContent>();

// Case manipulation
string upper = content.ToUpper();           // "  THE QUICK BROWN FOX..."
string lower = content.ToLower();           // "  the quick brown fox..."
string titleCase = content.ToTitleCase();   // Custom extension if available

// Trimming
string trimmed = content.Trim();            // "The Quick Brown Fox..."
string trimStart = content.TrimStart();     // "The Quick Brown Fox...  "
string trimEnd = content.TrimEnd();         // "  The Quick Brown Fox..."

// Substring operations
string substring = content.Substring(2, 10); // "The Quick "
int foxIndex = content.IndexOf("Fox");       // 19
bool containsDog = content.Contains("Dog");  // true
bool startsWithThe = content.StartsWith("  The"); // true
bool endsWithDog = content.EndsWith("Dog  ");     // true

Console.WriteLine($"Original: '{content}'");
Console.WriteLine($"Trimmed: '{trimmed}'");
Console.WriteLine($"Fox at index: {foxIndex}");
```

## String Comparison

Semantic strings support all comparison operations:

```csharp
public sealed record ProductName : SemanticString<ProductName> { }

var product1 = "Widget".As<ProductName>();
var product2 = "Gadget".As<ProductName>();
var product3 = "Widget".As<ProductName>();

// Equality comparisons
Console.WriteLine(product1 == product3);                    // true
Console.WriteLine(product1 != product2);                    // true
Console.WriteLine(product1.Equals(product3));               // true
Console.WriteLine(ReferenceEquals(product1, product3));     // false (different instances)

// String comparisons
Console.WriteLine(product1.CompareTo(product2));            // > 0 (W > G)
Console.WriteLine(string.Compare(product1, product2));      // > 0

// Case-insensitive comparisons
Console.WriteLine(product1.Equals("WIDGET", StringComparison.OrdinalIgnoreCase)); // true
Console.WriteLine(product1.StartsWith("wid", StringComparison.OrdinalIgnoreCase)); // true
```

## String Formatting and Interpolation

Use semantic strings in all formatting scenarios:

```csharp
public sealed record CustomerName : SemanticString<CustomerName> { }
public sealed record OrderId : SemanticString<OrderId> { }

var customerName = "John Smith".As<CustomerName>();
var orderId = "ORD-12345".As<OrderId>();

// String interpolation
string message = $"Order {orderId} for {customerName} is ready";
Console.WriteLine(message); // "Order ORD-12345 for John Smith is ready"

// String.Format
string formatted = string.Format("Customer: {0}, Order: {1}", customerName, orderId);
Console.WriteLine(formatted);

// Composite formatting
string composite = $"Processing order {orderId:UPPER} for customer {customerName:LOWER}";
```

## Character Enumeration

Iterate through characters in semantic strings:

```csharp
public sealed record CodeSnippet : SemanticString<CodeSnippet> { }

var code = "Hello123!".As<CodeSnippet>();

// Character enumeration
Console.WriteLine("Characters:");
foreach (char c in code)
{
    Console.WriteLine($"  '{c}' - {(char.IsLetter(c) ? "Letter" : char.IsDigit(c) ? "Digit" : "Other")}");
}

// Character analysis with LINQ
int letterCount = code.Count(char.IsLetter);           // 5
int digitCount = code.Count(char.IsDigit);             // 3
int specialCount = code.Count(c => !char.IsLetterOrDigit(c)); // 1

var upperCaseLetters = code.Where(char.IsUpper).ToArray();    // ['H']
var lowerCaseLetters = code.Where(char.IsLower).ToArray();    // ['e', 'l', 'l', 'o']

Console.WriteLine($"Letters: {letterCount}, Digits: {digitCount}, Special: {specialCount}");
```

## String Split and Join

Work with string arrays and collections:

```csharp
public sealed record TagList : SemanticString<TagList> { }
public sealed record Tag : SemanticString<Tag> { }

var tagList = "programming,csharp,dotnet,semantics".As<TagList>();

// Split into array
string[] tagArray = tagList.Split(',');
Console.WriteLine($"Found {tagArray.Length} tags");

// Convert to semantic types
var semanticTags = tagArray.Select(tag => tag.Trim().As<Tag>()).ToList();

// Display tags
Console.WriteLine("Tags:");
foreach (var tag in semanticTags)
{
    Console.WriteLine($"  - {tag}");
}

// Join back together
string rejoinedTags = string.Join(" | ", semanticTags);
Console.WriteLine($"Rejoined: {rejoinedTags}");

// Create new TagList from processed tags
var newTagList = string.Join(",", semanticTags).As<TagList>();
```

## String Replacement

Text replacement operations:

```csharp
public sealed record TemplateText : SemanticString<TemplateText> { }

var template = "Hello {name}, your order {orderId} is {status}.".As<TemplateText>();

// Simple replacements
string personalized = template.Replace("{name}", "Alice");
personalized = personalized.Replace("{orderId}", "ORD-789");
personalized = personalized.Replace("{status}", "shipped");

Console.WriteLine(personalized); // "Hello Alice, your order ORD-789 is shipped."

// Multiple replacements with dictionary
var replacements = new Dictionary<string, string>
{
    ["{name}"] = "Bob",
    ["{orderId}"] = "ORD-456",
    ["{status}"] = "processing"
};

string result = template.WeakString;
foreach (var replacement in replacements)
{
    result = result.Replace(replacement.Key, replacement.Value);
}

var finalTemplate = result.As<TemplateText>();
Console.WriteLine(finalTemplate);
```

## LINQ Operations

Use semantic strings naturally in LINQ queries:

```csharp
public sealed record EmployeeName : SemanticString<EmployeeName> { }
public sealed record DepartmentName : SemanticString<DepartmentName> { }

var employees = new List<(EmployeeName Name, DepartmentName Department)>
{
    ("John Smith".As<EmployeeName>(), "Engineering".As<DepartmentName>()),
    ("Jane Doe".As<EmployeeName>(), "Marketing".As<DepartmentName>()),
    ("Bob Johnson".As<EmployeeName>(), "Engineering".As<DepartmentName>()),
    ("Alice Brown".As<EmployeeName>(), "Sales".As<DepartmentName>())
};

// LINQ queries work naturally
var engineeringEmployees = employees
    .Where(emp => emp.Department.Contains("Engineering"))
    .Select(emp => emp.Name)
    .ToList();

var sortedEmployees = employees
    .OrderBy(emp => emp.Department)
    .ThenBy(emp => emp.Name)
    .ToList();

var departmentGroups = employees
    .GroupBy(emp => emp.Department)
    .ToDictionary(g => g.Key, g => g.Select(emp => emp.Name).ToList());

Console.WriteLine("Engineering Employees:");
engineeringEmployees.ForEach(name => Console.WriteLine($"  - {name}"));

Console.WriteLine("\nDepartment Groups:");
foreach (var group in departmentGroups)
{
    Console.WriteLine($"  {group.Key}: {string.Join(", ", group.Value)}");
}
```

## Regular Expression Operations

Use regex with semantic strings:

```csharp
using System.Text.RegularExpressions;

public sealed record EmailContent : SemanticString<EmailContent> { }

var emailText = @"Contact us at support@example.com or sales@example.com.
For urgent matters, reach admin@example.com.".As<EmailContent>();

// Find all email addresses
var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
var matches = Regex.Matches(emailText, emailPattern);

Console.WriteLine($"Found {matches.Count} email addresses:");
foreach (Match match in matches)
{
    Console.WriteLine($"  - {match.Value}");
}

// Replace emails with masked versions
string maskedContent = Regex.Replace(emailText, emailPattern, "***@example.com");
var maskedEmailContent = maskedContent.As<EmailContent>();
Console.WriteLine($"\nMasked content: {maskedEmailContent}");

// Validate format
bool isValidEmail = Regex.IsMatch("test@example.com", emailPattern);
Console.WriteLine($"Is valid email: {isValidEmail}");
```

## StringBuilder Integration

Work with StringBuilder for dynamic content:

```csharp
using System.Text;

public sealed record LogMessage : SemanticString<LogMessage> { }

var logBuilder = new StringBuilder();
logBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Application started");
logBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Loading configuration");
logBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Application ready");

// Convert StringBuilder to semantic string
var logMessage = logBuilder.ToString().As<LogMessage>();

Console.WriteLine("Log Output:");
Console.WriteLine(logMessage);

// Append more content
var moreContent = "\n[2024-01-01 12:00:00] Processing request";
var updatedLog = (logMessage + moreContent).As<LogMessage>();
```

## Hash Code and Dictionary Usage

Semantic strings work as dictionary keys and in hash-based collections:

```csharp
public sealed record ProductCode : SemanticString<ProductCode> { }
public sealed record ProductInfo : SemanticString<ProductInfo> { }

// Dictionary with semantic string keys
var productCatalog = new Dictionary<ProductCode, ProductInfo>
{
    ["WIDGET-001".As<ProductCode>()] = "Standard Widget - $19.99".As<ProductInfo>(),
    ["GADGET-002".As<ProductCode>()] = "Premium Gadget - $49.99".As<ProductInfo>(),
    ["TOOL-003".As<ProductCode>()] = "Professional Tool - $99.99".As<ProductInfo>()
};

// HashSet for unique values
var availableProducts = new HashSet<ProductCode>(productCatalog.Keys);

// Lookup operations
var searchCode = "WIDGET-001".As<ProductCode>();
if (productCatalog.TryGetValue(searchCode, out var info))
{
    Console.WriteLine($"Found: {info}");
}

// Hash code consistency
var code1 = "TEST-123".As<ProductCode>();
var code2 = "TEST-123".As<ProductCode>();
Console.WriteLine($"Hash codes equal: {code1.GetHashCode() == code2.GetHashCode()}"); // true
```

## Performance Considerations

Leverage efficient string operations:

```csharp
public sealed record LargeText : SemanticString<LargeText> { }

var largeContent = string.Join("\n", Enumerable.Range(1, 1000).Select(i => $"Line {i}"));
var largeText = largeContent.As<LargeText>();

// Use Span operations for better performance
ReadOnlySpan<char> span = largeText.AsSpan();
int lineCount = 0;
foreach (var c in span)
{
    if (c == '\n') lineCount++;
}

Console.WriteLine($"Line count: {lineCount + 1}");

// Memory-efficient substring operations
ReadOnlyMemory<char> memory = largeText.AsMemory();
var firstHundredChars = memory.Slice(0, Math.Min(100, largeText.Length));
```

This comprehensive string compatibility ensures semantic strings work seamlessly with existing .NET string operations while providing additional type safety.
