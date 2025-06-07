// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Strategy interface for different validation approaches.
/// Implements the Strategy pattern to allow different validation behaviors.
/// </summary>
public interface IValidationStrategy
{
	/// <summary>
	/// Validates a semantic string using the strategy's logic.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <param name="type">The type of the semantic string for attribute retrieval</param>
	/// <returns>True if validation passes, false otherwise</returns>
	public bool Validate(ISemanticString semanticString, Type type);
}

/// <summary>
/// Validation strategy that requires all validation attributes to pass (AND logic).
/// </summary>
public sealed class ValidateAllStrategy : IValidationStrategy
{
	/// <inheritdoc/>
	public bool Validate(ISemanticString semanticString, Type type)
	{
		ArgumentNullException.ThrowIfNull(type);
		SemanticStringValidationAttribute[] validationAttributes = [.. type.GetCustomAttributes(typeof(SemanticStringValidationAttribute), true)
			.Cast<SemanticStringValidationAttribute>()];
		return validationAttributes.Length == 0 || validationAttributes.All(attr => attr.Validate(semanticString));
	}
}

/// <summary>
/// Validation strategy that requires any validation attribute to pass (OR logic).
/// </summary>
public sealed class ValidateAnyStrategy : IValidationStrategy
{
	/// <inheritdoc/>
	public bool Validate(ISemanticString semanticString, Type type)
	{
		ArgumentNullException.ThrowIfNull(type);
		SemanticStringValidationAttribute[] validationAttributes = [.. type.GetCustomAttributes(typeof(SemanticStringValidationAttribute), true)
			.Cast<SemanticStringValidationAttribute>()];
		return validationAttributes.Length == 0 || validationAttributes.Any(attr => attr.Validate(semanticString));
	}
}

/// <summary>
/// Factory for creating appropriate validation strategies based on type attributes.
/// </summary>
public static class ValidationStrategyFactory
{
	private static readonly ValidateAllStrategy AllStrategy = new();
	private static readonly ValidateAnyStrategy AnyStrategy = new();

	/// <summary>
	/// Creates the appropriate validation strategy based on the type's validation attributes.
	/// </summary>
	/// <param name="type">The type to examine for validation strategy attributes</param>
	/// <returns>The appropriate validation strategy instance</returns>
	public static IValidationStrategy CreateStrategy(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);
		// Check for ValidateAny attribute
		object[] validateAnyAttributes = type.GetCustomAttributes(typeof(ValidateAnyAttribute), true);
		return validateAnyAttributes.Length > 0 ? AnyStrategy : AllStrategy;
	}
}
