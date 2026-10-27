// Copyright (c) 2023-2026 ktsu-dev contributors

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

		SemanticStringValidationAttribute[] validationAttributes = ValidationAttributeCache.Attributes(type);
		if (validationAttributes.Length == 0)
		{
			return true;
		}

		foreach (SemanticStringValidationAttribute attribute in validationAttributes)
		{
			if (!attribute.Validate(semanticString))
			{
				return false;
			}
		}

		return true;
	}
}
