# Factory Pattern

This guide demonstrates using the factory pattern for creating semantic strings, including dependency injection and validation strategies.

## Basic Factory Usage

The factory pattern provides a clean way to create semantic strings with validation:

```csharp
using ktsu.Semantics;

// Define your semantic string type
[RegexMatch(@"^[A-Z]{2}\d{6}$")]
public sealed record CustomerCode : SemanticString<CustomerCode> { }

// Create using factory pattern
var factory = new SemanticStringFactory<CustomerCode>();

// Safe creation with automatic validation
var customerCode = factory.Create("AB123456");
Console.WriteLine($"Customer: {customerCode}");

// Try-create pattern for error handling
if (factory.TryCreate("INVALID", out var invalidCode))
{
    Console.WriteLine($"Valid code: {invalidCode}");
}
else
{
    Console.WriteLine("Invalid customer code format detected");
}
```

## Dependency Injection with ASP.NET Core

Integrate factories into dependency injection:

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Register semantic string factories
    services.AddScoped<ISemanticStringFactory<CustomerCode>, SemanticStringFactory<CustomerCode>>();
    services.AddScoped<ISemanticStringFactory<ProductId>, SemanticStringFactory<ProductId>>();

    // Register your services
    services.AddScoped<CustomerService>();
}

public sealed record ProductId : SemanticString<ProductId> { }

// Service using injected factories
public class CustomerService
{
    private readonly ISemanticStringFactory<CustomerCode> _customerCodeFactory;

    public CustomerService(ISemanticStringFactory<CustomerCode> customerCodeFactory)
    {
        _customerCodeFactory = customerCodeFactory;
    }

    public Customer CreateCustomer(string codeInput)
    {
        var customerCode = _customerCodeFactory.Create(codeInput);
        return new Customer { Code = customerCode };
    }
}
```

## Validation Strategies

Use different validation strategies with factories:

```csharp
public interface IValidationStrategy
{
    bool IsValid(ISemanticString semanticString);
}

public class StrictValidationStrategy : IValidationStrategy
{
    public bool IsValid(ISemanticString semanticString)
    {
        return semanticString.IsValid();
    }
}

public class LenientValidationStrategy : IValidationStrategy
{
    public bool IsValid(ISemanticString semanticString)
    {
        return !string.IsNullOrEmpty(semanticString.ToString());
    }
}
```

This factory pattern approach provides clean separation of concerns and flexible validation strategies.
