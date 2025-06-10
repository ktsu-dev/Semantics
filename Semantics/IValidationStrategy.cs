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
