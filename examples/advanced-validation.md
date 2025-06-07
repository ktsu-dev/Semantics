# Advanced Validation

This guide covers custom validation scenarios, business rules, and multi-level validation strategies.

## Custom Validation Rules

Create reusable validation logic:

```csharp
using ktsu.Semantics;

// Custom business rule validation
public sealed record ProductCode : SemanticString<ProductCode>
{
    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        // Business rule: Product codes must have valid department prefix
        var validPrefixes = new[] { "ELEC", "BOOK", "CLTH", "SPRT" };
        var parts = WeakString.Split('-');

        if (parts.Length != 2) return false;
        if (!validPrefixes.Contains(parts[0])) return false;
        if (!int.TryParse(parts[1], out int id) || id <= 0) return false;

        return true;
    }
}

// Usage
try
{
    var validProduct = "ELEC-12345".As<ProductCode>();    // ✅ Valid
    var invalidDept = "INVALID-123".As<ProductCode>();    // ❌ Invalid department
}
catch (FormatException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
}
```

## Multi-Level Validation

Implement hierarchical validation with different strictness levels:

```csharp
public enum ValidationLevel
{
    Basic,      // Format only
    Business,   // Format + business rules
    External    // Format + business + external validation
}

public sealed record OrderNumber : SemanticString<OrderNumber>
{
    public ValidationLevel RequiredLevel { get; init; } = ValidationLevel.Business;

    public override bool IsValid()
    {
        // Level 1: Basic format validation
        if (!base.IsValid()) return false;
        if (!WeakString.StartsWith("ORD-")) return false;

        if (RequiredLevel == ValidationLevel.Basic) return true;

        // Level 2: Business rules
        var parts = WeakString.Split('-');
        if (parts.Length != 3) return false;

        if (!DateTime.TryParseExact(parts[1], "yyyyMMdd", null,
            DateTimeStyles.None, out var orderDate)) return false;

        if (orderDate > DateTime.Today) return false; // No future orders

        return RequiredLevel != ValidationLevel.External || ValidateExternally();
    }

    private bool ValidateExternally()
    {
        // Placeholder for external validation
        return WeakString.Length <= 20;
    }
}
```

## Conditional Validation

Apply different rules based on context:

```csharp
public sealed record UserId : SemanticString<UserId>
{
    public bool IsEmployee { get; init; } = false;
    public bool IsContractor { get; init; } = false;

    public override bool IsValid()
    {
        if (!base.IsValid()) return false;

        if (IsEmployee)
        {
            // Employee IDs: EMP-XXXXXX (6 digits)
            return WeakString.StartsWith("EMP-") &&
                   WeakString.Length == 10 &&
                   WeakString.Substring(4).All(char.IsDigit);
        }

        if (IsContractor)
        {
            // Contractor IDs: CONT-XXXX (4 digits)
            return WeakString.StartsWith("CONT-") &&
                   WeakString.Length == 9 &&
                   WeakString.Substring(5).All(char.IsDigit);
        }

        // Default validation for general users
        return WeakString.Length >= 5 && WeakString.Length <= 20;
    }
}

// Usage with context
var employeeId = "EMP-123456".As<UserId>() with { IsEmployee = true };
var contractorId = "CONT-7890".As<UserId>() with { IsContractor = true };
```

This advanced validation system provides flexible, maintainable validation for complex business requirements.
