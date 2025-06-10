# Real-World Scenarios

Complete examples demonstrating the Semantics library in realistic domain contexts.

## E-Commerce System

A complete e-commerce domain using semantic strings:

```csharp
using ktsu.Semantics;

// Product domain
[RegexMatch(@"^[A-Z]{2,4}-\d{4,6}$")]
public sealed record ProductSku : SemanticString<ProductSku> { }

[RegexMatch(@"^CAT-\d{3}$")]
public sealed record CategoryId : SemanticString<CategoryId> { }

[Contains("$")]
public sealed record Price : SemanticString<Price>
{
    protected override string MakeCanonical(string input)
    {
        // Normalize price format
        if (decimal.TryParse(input.Replace("$", ""), out decimal amount))
        {
            return $"${amount:F2}";
        }
        return input;
    }
}

// Customer domain
[RegexMatch(@"^CUST-\d{8}$")]
public sealed record CustomerId : SemanticString<CustomerId> { }

[RegexMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
public sealed record EmailAddress : SemanticString<EmailAddress> { }

// Order domain
[StartsWith("ORD-")]
public sealed record OrderId : SemanticString<OrderId> { }

public sealed record OrderStatus : SemanticString<OrderStatus>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        var validStatuses = new[] { "PENDING", "CONFIRMED", "SHIPPED", "DELIVERED", "CANCELLED" };
        return validStatuses.Contains(WeakString.ToUpperInvariant());
    }

    protected override string MakeCanonical(string input)
    {
        return input.ToUpperInvariant();
    }
}

// Domain entities
public class Product
{
    public ProductSku Sku { get; init; }
    public string Name { get; init; } = "";
    public CategoryId CategoryId { get; init; }
    public Price Price { get; init; }
}

public class Customer
{
    public CustomerId Id { get; init; }
    public string Name { get; init; } = "";
    public EmailAddress Email { get; init; }
}

public class Order
{
    public OrderId Id { get; init; }
    public CustomerId CustomerId { get; init; }
    public List<ProductSku> ProductSkus { get; init; } = new();
    public OrderStatus Status { get; init; }
    public DateTime OrderDate { get; init; }
}

// Service layer
public class ECommerceService
{
    public Order CreateOrder(string customerId, List<string> productSkus)
    {
        var orderNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

        return new Order
        {
            Id = $"ORD-{orderNumber}".As<OrderId>(),
            CustomerId = customerId.As<CustomerId>(),
            ProductSkus = productSkus.Select(sku => sku.As<ProductSku>()).ToList(),
            Status = "PENDING".As<OrderStatus>(),
            OrderDate = DateTime.Now
        };
    }

    public void UpdateOrderStatus(OrderId orderId, string newStatus)
    {
        var status = newStatus.As<OrderStatus>(); // Validates status
        Console.WriteLine($"Order {orderId} status updated to {status}");
    }
}

// Usage example
var service = new ECommerceService();

// Create products
var laptop = new Product
{
    Sku = "ELEC-1234".As<ProductSku>(),
    Name = "Gaming Laptop",
    CategoryId = "CAT-001".As<CategoryId>(),
    Price = "1299.99".As<Price>()
};

// Create customer
var customer = new Customer
{
    Id = "CUST-12345678".As<CustomerId>(),
    Name = "John Smith",
    Email = "john@example.com".As<EmailAddress>()
};

// Create order
var order = service.CreateOrder("CUST-12345678", new[] { "ELEC-1234", "BOOK-5678" });
Console.WriteLine($"Created order: {order.Id} for customer: {order.CustomerId}");

// Update order status
service.UpdateOrderStatus(order.Id, "confirmed");
```

## Financial Services

Banking and financial domain example:

```csharp
// Account identifiers
[RegexMatch(@"^\d{10}$")]
public sealed record AccountNumber : SemanticString<AccountNumber> { }

[RegexMatch(@"^\d{9}$")]
public sealed record RoutingNumber : SemanticString<RoutingNumber> { }

[RegexMatch(@"^\d{4}-\d{4}-\d{4}-\d{4}$")]
public sealed record CreditCardNumber : SemanticString<CreditCardNumber> { }

// Transaction types
public sealed record TransactionId : SemanticString<TransactionId>
{
    protected override string MakeCanonical(string input)
    {
        // Ensure transaction IDs are always uppercase
        return input.ToUpperInvariant();
    }
}

public sealed record Amount : SemanticString<Amount>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        // Remove currency symbol for validation
        string numberPart = WeakString.Replace("$", "").Replace(",", "");

        if (!decimal.TryParse(numberPart, out decimal value))
            return false;

        // Business rule: No negative amounts in this context
        return value >= 0;
    }

    protected override string MakeCanonical(string input)
    {
        // Standardize currency format
        string cleaned = input.Replace("$", "").Replace(",", "");
        if (decimal.TryParse(cleaned, out decimal amount))
        {
            return $"${amount:N2}";
        }
        return input;
    }
}

// Financial entities
public class BankAccount
{
    public AccountNumber AccountNumber { get; init; }
    public RoutingNumber RoutingNumber { get; init; }
    public Amount Balance { get; init; }
    public string AccountHolderName { get; init; } = "";
}

public class Transaction
{
    public TransactionId Id { get; init; }
    public AccountNumber FromAccount { get; init; }
    public AccountNumber ToAccount { get; init; }
    public Amount Amount { get; init; }
    public DateTime Timestamp { get; init; }
    public string Description { get; init; } = "";
}

// Banking service
public class BankingService
{
    public Transaction ProcessTransfer(
        string fromAccount,
        string toAccount,
        string amount,
        string description)
    {
        // All conversions are validated
        var transaction = new Transaction
        {
            Id = Guid.NewGuid().ToString().As<TransactionId>(),
            FromAccount = fromAccount.As<AccountNumber>(),
            ToAccount = toAccount.As<AccountNumber>(),
            Amount = amount.As<Amount>(),
            Timestamp = DateTime.Now,
            Description = description
        };

        Console.WriteLine($"Processing transfer: {transaction.Amount} from {transaction.FromAccount} to {transaction.ToAccount}");
        return transaction;
    }

    public bool ValidateAccount(string accountNumber, string routingNumber)
    {
        try
        {
            var account = accountNumber.As<AccountNumber>();
            var routing = routingNumber.As<RoutingNumber>();

            // Additional business validation could go here
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

// Usage
var bankingService = new BankingService();

// Validate account information
bool isValid = bankingService.ValidateAccount("1234567890", "123456789");
Console.WriteLine($"Account valid: {isValid}");

// Process transfer with automatic validation and formatting
var transfer = bankingService.ProcessTransfer(
    "1234567890",
    "0987654321",
    "1500.50",
    "Monthly payment");

Console.WriteLine($"Transfer ID: {transfer.Id}");
Console.WriteLine($"Amount: {transfer.Amount}"); // Formatted as $1,500.50
```

## Configuration Management

Application configuration with semantic strings:

```csharp
// Configuration keys and values
[IsFilePath]
public sealed record ConfigFilePath : SemanticString<ConfigFilePath> { }

[RegexMatch(@"^[a-zA-Z][a-zA-Z0-9_]*(\.[a-zA-Z][a-zA-Z0-9_]*)*$")]
public sealed record ConfigKey : SemanticString<ConfigKey> { }

public sealed record ConnectionString : SemanticString<ConnectionString>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        // Basic connection string validation
        return WeakString.Contains("=") && WeakString.Length > 10;
    }
}

[ValidateAny]
[StartsWith("http://", StringComparison.OrdinalIgnoreCase)]
[StartsWith("https://", StringComparison.OrdinalIgnoreCase)]
public sealed record ApiEndpoint : SemanticString<ApiEndpoint> { }

// Configuration management
public class ConfigurationManager
{
    private readonly Dictionary<ConfigKey, string> _config = new();

    public void LoadFromFile(ConfigFilePath configPath)
    {
        if (File.Exists(configPath))
        {
            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
            {
                if (line.Contains('='))
                {
                    var parts = line.Split('=', 2);
                    var key = parts[0].Trim().As<ConfigKey>();
                    _config[key] = parts[1].Trim();
                }
            }
            Console.WriteLine($"Loaded configuration from: {configPath}");
        }
    }

    public T GetValue<T>(string key) where T : class, ISemanticString<T>
    {
        var configKey = key.As<ConfigKey>();
        if (_config.TryGetValue(configKey, out var value))
        {
            return value.As<T>();
        }
        throw new KeyNotFoundException($"Configuration key not found: {key}");
    }

    public void SetValue<T>(string key, T value) where T : ISemanticString
    {
        var configKey = key.As<ConfigKey>();
        _config[configKey] = value.ToString();
    }
}

// Application startup
public class Application
{
    private readonly ConfigurationManager _config;

    public Application()
    {
        _config = new ConfigurationManager();
        Initialize();
    }

    private void Initialize()
    {
        // Load configuration with validation
        var configPath = "app.config".As<ConfigFilePath>();
        _config.LoadFromFile(configPath);

        // Access typed configuration values
        try
        {
            var dbConnection = _config.GetValue<ConnectionString>("database.connection");
            var apiEndpoint = _config.GetValue<ApiEndpoint>("api.baseUrl");

            Console.WriteLine($"Database: {dbConnection}");
            Console.WriteLine($"API: {apiEndpoint}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration error: {ex.Message}");
        }
    }

    public void UpdateApiEndpoint(string newEndpoint)
    {
        // Type-safe configuration updates
        var endpoint = newEndpoint.As<ApiEndpoint>();
        _config.SetValue("api.baseUrl", endpoint);
        Console.WriteLine($"Updated API endpoint: {endpoint}");
    }
}

// Usage
var app = new Application();
app.UpdateApiEndpoint("https://api.newdomain.com/v2");
```

## Document Management System

File and document handling:

```csharp
// Document identifiers
[RegexMatch(@"^DOC-\d{8}-[A-Z]{3}$")]
public sealed record DocumentId : SemanticString<DocumentId> { }

[IsFileName]
public sealed record DocumentName : SemanticString<DocumentName> { }

[ValidateAny]
[EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".doc", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".docx", StringComparison.OrdinalIgnoreCase)]
[EndsWith(".txt", StringComparison.OrdinalIgnoreCase)]
public sealed record SupportedDocument : SemanticString<SupportedDocument> { }

public sealed record DocumentStatus : SemanticString<DocumentStatus>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        var validStatuses = new[] { "DRAFT", "REVIEW", "APPROVED", "PUBLISHED", "ARCHIVED" };
        return validStatuses.Contains(WeakString.ToUpperInvariant());
    }
}

// Document entity
public class Document
{
    public DocumentId Id { get; init; }
    public DocumentName Name { get; init; }
    public SupportedDocument FilePath { get; init; }
    public DocumentStatus Status { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime LastModified { get; init; }
}

// Document service
public class DocumentService
{
    private readonly List<Document> _documents = new();

    public DocumentId CreateDocument(string fileName, string filePath)
    {
        var docId = $"DOC-{DateTime.Now:yyyyMMdd}-{GenerateCode()}".As<DocumentId>();

        var document = new Document
        {
            Id = docId,
            Name = fileName.As<DocumentName>(),
            FilePath = filePath.As<SupportedDocument>(), // Validates file type
            Status = "DRAFT".As<DocumentStatus>(),
            CreatedDate = DateTime.Now,
            LastModified = DateTime.Now
        };

        _documents.Add(document);
        Console.WriteLine($"Created document: {docId}");
        return docId;
    }

    public void UpdateStatus(DocumentId documentId, string newStatus)
    {
        var status = newStatus.As<DocumentStatus>(); // Validates status
        var document = _documents.FirstOrDefault(d => d.Id == documentId);

        if (document != null)
        {
            // In real implementation, would update the document
            Console.WriteLine($"Document {documentId} status updated to {status}");
        }
    }

    private static string GenerateCode()
    {
        return new Random().Next(100, 999).ToString() +
               (char)('A' + new Random().Next(0, 26)) +
               (char)('A' + new Random().Next(0, 26)) +
               (char)('A' + new Random().Next(0, 26));
    }
}

// Usage
var docService = new DocumentService();

// Create documents with automatic validation
var docId1 = docService.CreateDocument("annual_report", @"C:\Documents\annual_report.pdf");
var docId2 = docService.CreateDocument("meeting_notes", @"C:\Documents\notes.docx");

// Update document status
docService.UpdateStatus(docId1, "REVIEW");
```

These real-world scenarios demonstrate how semantic strings provide type safety, validation, and clear domain modeling in practical applications.
