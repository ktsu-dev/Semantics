// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

/// <summary>
/// Validation strategy that requires all validation attributes to pass (AND logic).
/// </summary>
public sealed class ValidateAllStrategy : IValidationStrategy
{
	/// <inheritdoc/>
	public bool Validate(ISemanticString semanticString, Type type)
	{
		Ensure.NotNull(type);

		SemanticStringValidationAttribute[] validationAttributes = [.. type.GetCustomAttributes(typeof(SemanticStringValidationAttribute), true)
			.Cast<SemanticStringValidationAttribute>()];
		return validationAttributes.Length == 0 || validationAttributes.All(attr => attr.Validate(semanticString));
	}
}
