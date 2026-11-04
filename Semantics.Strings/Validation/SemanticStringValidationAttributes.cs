// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Helper class for validating using attributes
/// </summary>
internal static class AttributeValidation
{
	/// <summary>
	/// Validates a SemanticString against all validation attributes defined on its type.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <param name="type">The type of the SemanticString, which contains the validation attributes</param>
	/// <returns>True if the string passes all validation according to the rules, false otherwise</returns>
	/// <remarks>
	/// Uses the Strategy pattern to determine the appropriate validation approach based on type attributes.
	/// This eliminates code duplication and improves maintainability.
	/// </remarks>
	public static bool ValidateAttributes(ISemanticString semanticString, Type type)
	{
		IValidationStrategy strategy = ValidationStrategyFactory.CreateStrategy(type);
		return strategy.Validate(semanticString, type);
	}

	/// <summary>
	/// Gets the reasons the validation attributes defined on a type give for rejecting a SemanticString.
	/// </summary>
	/// <param name="semanticString">The SemanticString that failed validation</param>
	/// <param name="type">The type of the SemanticString, which contains the validation attributes</param>
	/// <returns>The error messages of every attribute that rejects the string, in declaration order</returns>
	/// <remarks>
	/// Under <see cref="ValidateAllAttribute"/> these are the attributes that failed; under
	/// <see cref="ValidateAnyAttribute"/> every attribute failed, so this is the union of their errors.
	/// An attribute that reports no reason of its own is named instead.
	/// </remarks>
	public static IEnumerable<string> GetValidationErrors(ISemanticString semanticString, Type type)
	{
		Ensure.NotNull(type);

		foreach (SemanticStringValidationAttribute attribute in ValidationAttributeCache.Attributes(type)
			.Where(attribute => !attribute.Validate(semanticString)))
		{
			string[] errors = attribute is NativeSemanticStringValidationAttribute native
				? [.. native.GetValidationErrors(semanticString)]
				: [];
			if (errors.Length == 0)
			{
				yield return $"{attribute.GetType().Name} validation failed.";
				continue;
			}

			foreach (string error in errors)
			{
				yield return error;
			}
		}
	}
}
