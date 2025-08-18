// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

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
}
