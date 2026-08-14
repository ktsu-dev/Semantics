// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

/// <summary>
/// Validation strategy that requires any validation attribute to pass (OR logic).
/// </summary>
public sealed class ValidateAnyStrategy : IValidationStrategy
{
	/// <inheritdoc/>
	public bool Validate(ISemanticString semanticString, Type type)
	{
		Ensure.NotNull(type);

		SemanticStringValidationAttribute[] validationAttributes = [.. type.GetCustomAttributes(typeof(SemanticStringValidationAttribute), true)
			.Cast<SemanticStringValidationAttribute>()];
		return validationAttributes.Length == 0 || validationAttributes.Any(attr => attr.Validate(semanticString));
	}
}
