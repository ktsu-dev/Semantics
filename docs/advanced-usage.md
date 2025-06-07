# Advanced Usage Guide

This guide covers advanced features and patterns for using the Semantics library effectively in complex scenarios.

## Table of Contents

-   [Factory Pattern Usage](#factory-pattern-usage)
-   [Custom Validation Strategies](#custom-validation-strategies)
-   [Contract Validation and LSP Compliance](#contract-validation-and-lsp-compliance)
-   [Type Conversions](#type-conversions)
-   [Validation Modes](#validation-modes)
-   [Path Operations](#path-operations)
-   [Best Practices](#best-practices)
-   [Dependency Injection](#dependency-injection)

## Factory Pattern Usage

The factory pattern provides a clean, extensible way to create semantic string instances with proper dependency injection support.

```csharp
// Dependency injection with factories
public class UserService
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;
    private readonly ISemanticStringFactory<UserId> _userIdFactory;

    public UserService(
        ISemanticStringFactory<EmailAddress> emailFactory,
        ISemanticStringFactory<UserId> userIdFactory)
    {
        _emailFactory = emailFactory;
        _userIdFactory = userIdFactory;
    }

    public User CreateUser(string email, string id)
    {
        // Safe creation with automatic validation
        var emailAddress = _emailFactory.Create(email);
        var userId = _userIdFactory.Create(id);
        return new User(userId, emailAddress);
    }

    public bool TryCreateUser(string email, string id, out User? user)
    {
        user = null;
        if (!_emailFactory.TryCreate(email, out var emailAddress) ||
            !_userIdFactory.TryCreate(id, out var userId))
        {
            return false;
        }

        user = new User(userId, emailAddress);
        return true;
    }
}
```

## Custom Validation Strategies

Create domain-specific validation strategies that implement complex business rules.

```csharp
// Implement domain-specific validation strategies
public class BusinessRuleValidationStrategy : IValidationStrategy
{
    public bool Validate(IEnumerable<SemanticStringValidationAttribute> attributes, ISemanticString value)
    {
        // Custom business logic for validation
        var criticalAttributes = attributes.Where(attr => attr is ICriticalValidation);
        var nonCriticalAttributes = attributes.Except(criticalAttributes);

        // All critical validations must pass
        bool criticalPass = criticalAttributes.All(attr => attr.Validate(value));

        // At least one non-critical validation must pass
        bool nonCriticalPass = !nonCriticalAttributes.Any() ||
                              nonCriticalAttributes.Any(attr => attr.Validate(value));

        return criticalPass && nonCriticalPass;
    }
}

// Use custom strategies with validation attributes
[ValidateWith(typeof(BusinessRuleValidationStrategy))]
[IsNotEmpty, IsEmail] // Critical validations
[IsCompanyEmail, IsInternalDomain] // Non-critical validations
public sealed record BusinessEmail : SemanticString<BusinessEmail> { }
```

### Creating Custom Validation Rules

```csharp
// Create reusable validation rules
public class ProductCodeValidationRule : ValidationRuleBase
{
    public override string RuleName => "ProductCode";

    protected override bool ValidateCore(SemanticStringValidationAttribute attribute, ISemanticString value)
    {
        string str = value.ToString();
        // Product codes: letter + 5 digits
        return Regex.IsMatch(str, @"^[A-Z][0-9]{5}$");
    }

    public override bool IsApplicable(SemanticStringValidationAttribute attribute)
        => attribute is IsProductCodeAttribute;
}

// Custom validation attribute
public class IsProductCodeAttribute : SemanticStringValidationAttribute
{
    public override bool Validate(ISemanticString semanticString)
    {
        var rule = new ProductCodeValidationRule();
        return rule.Validate(this, semanticString);
    }
}

// Apply custom validation
[IsProductCode]
public sealed record ProductCode : SemanticString<ProductCode> { }

// Usage with automatic validation
var factory = new SemanticStringFactory<ProductCode>();
var validCode = factory.Create("A12345");   // ✅ Valid
// factory.Create("123ABC");                 // ❌ Throws FormatException
```

## Contract Validation and LSP Compliance

Ensure your implementations satisfy behavioral contracts for proper Liskov Substitution Principle compliance.

```csharp
// Ensure implementations satisfy behavioral contracts
public class SemanticStringValidator<T> where T : SemanticString<T>
{
    public bool ValidateImplementation(T instance1, T instance2, T instance3)
    {
        // Validate basic contracts (reflexivity, symmetry, transitivity)
        bool basicContracts = SemanticStringContracts.ValidateContracts(instance1);

        // Validate equality contracts
        bool equalityContracts = SemanticStringContracts.ValidateEqualityContracts(instance1, instance2);

        // Validate comparison contracts (for IComparable implementations)
        bool comparisonContracts = SemanticStringContracts.ValidateComparisonContracts(instance1, instance2, instance3);

        return basicContracts && equalityContracts && comparisonContracts;
    }
}

// Use in unit tests to ensure LSP compliance
[Test]
public void EmailAddress_ShouldSatisfySemanticStringContracts()
{
    var email1 = EmailAddress.FromString<EmailAddress>("user@example.com");
    var email2 = EmailAddress.FromString<EmailAddress>("admin@example.com");
    var email3 = EmailAddress.FromString<EmailAddress>("test@example.com");

    var validator = new SemanticStringValidator<EmailAddress>();
    Assert.IsTrue(validator.ValidateImplementation(email1, email2, email3));
}
```

## Type Conversions

The library provides safe conversions between compatible types with zero-allocation optimizations.

```csharp
// Safe conversions between compatible types
var factory = new SemanticStringFactory<Path>();
var genericPath = factory.Create(@"C:\temp\file.txt");
var specificPath = genericPath.As<FilePath>(); // Convert to more specific type

// Implicit conversions to primitive types
string pathString = specificPath;              // Implicit to string
char[] pathChars = specificPath;               // Implicit to char[]
ReadOnlySpan<char> pathSpan = specificPath;    // Implicit to span
```

## Validation Modes

Configure how multiple validation attributes are processed:

```csharp
// Require ALL validation attributes to pass (default)
[ValidateAll]
[IsPath, IsAbsolutePath, DoesExist]
public sealed record ExistingAbsolutePath : SemanticPath<ExistingAbsolutePath> { }

// Require ANY validation attribute to pass
[ValidateAny]
[IsEmail, IsUrl]
public sealed record ContactInfo : SemanticString<ContactInfo> { }

// Custom validation strategy (shown earlier)
[ValidateWith(typeof(BusinessRuleValidationStrategy))]
[IsNotEmpty, IsEmail]
public sealed record StrictBusinessEmail : SemanticString<StrictBusinessEmail> { }
```

## Path Operations

Specialized operations for working with file system paths:

```csharp
var from = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\App");
var to = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\Lib\Utils.cs");

// Create relative path between two absolute paths
var relativePath = RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(from, to);
Console.WriteLine(relativePath); // ..\Lib\Utils.cs

// Use built-in path types with factory pattern
var pathFactory = new SemanticStringFactory<FilePath>();
var filePath = pathFactory.Create(@"C:\temp\data.json");

// Access path properties
Console.WriteLine(filePath.FileName);        // data.json
Console.WriteLine(filePath.FileExtension);   // .json
Console.WriteLine(filePath.DirectoryPath);   // C:\temp

// Check file system properties
var absolutePath = AbsolutePath.FromString<AbsolutePath>(@"C:\Projects\MyApp");
Console.WriteLine(absolutePath.Exists);      // True/False
Console.WriteLine(absolutePath.IsDirectory); // True/False
```

## Best Practices

### SOLID and DRY Principles

1. **Use Factory Pattern**: Prefer `ISemanticStringFactory<T>` for object creation to separate construction logic
2. **Implement Custom Strategies**: Create domain-specific validation strategies rather than duplicating validation logic
3. **Leverage Contract Validation**: Use `SemanticStringContracts` in unit tests to ensure LSP compliance
4. **Extend Through Interfaces**: Add new functionality through new interfaces rather than modifying existing ones
5. **Compose Validation Rules**: Build complex validation by composing simple, reusable `IValidationRule` implementations

### Domain Design

6. **Create Domain-Specific Types**: Use semantic strings for domain concepts like `UserId`, `OrderNumber`, `ProductSku`
7. **Validate at Boundaries**: Create semantic strings at system boundaries (APIs, user input, file I/O)
8. **Use Type Safety**: Let the compiler prevent string misuse with strong typing
9. **Combine Validations**: Use multiple validation attributes for comprehensive checking
10. **Document Intent**: Semantic types make code self-documenting

## Dependency Injection

Integrate semantic string factories with your dependency injection container:

```csharp
// Register factories in your DI container
services.AddTransient<ISemanticStringFactory<EmailAddress>, SemanticStringFactory<EmailAddress>>();
services.AddTransient<ISemanticStringFactory<UserId>, SemanticStringFactory<UserId>>();

// Use in controllers/services
public class UserController : ControllerBase
{
    private readonly ISemanticStringFactory<EmailAddress> _emailFactory;

    public UserController(ISemanticStringFactory<EmailAddress> emailFactory)
    {
        _emailFactory = emailFactory;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (!_emailFactory.TryCreate(request.Email, out var emailAddress))
        {
            return BadRequest("Invalid email format");
        }

        // emailAddress is guaranteed to be valid
        var user = new User(emailAddress);
        return Ok(user);
    }
}
```

This advanced usage guide demonstrates how to leverage the full power of the Semantics library while maintaining clean, maintainable, and testable code.
