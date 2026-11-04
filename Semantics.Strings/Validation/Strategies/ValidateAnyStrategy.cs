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

		SemanticStringValidationAttribute[] validationAttributes = ValidationAttributeCache.Attributes(type);
		if (validationAttributes.Length == 0)
		{
			return true;
		}

		foreach (SemanticStringValidationAttribute attribute in validationAttributes)
		{
			if (attribute.Validate(semanticString))
			{
				return true;
			}
		}

		return false;
	}
}
