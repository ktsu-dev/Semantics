// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

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
