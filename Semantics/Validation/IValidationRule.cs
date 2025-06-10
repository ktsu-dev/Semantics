// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for validation rules that can be applied to semantic strings.
/// Enables the Open/Closed Principle by allowing new validation rules without modifying existing code.
/// </summary>
public interface IValidationRule
{
	/// <summary>
	/// Gets the name of this validation rule for identification purposes.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets the priority of this validation rule. Higher values are executed first.
	/// </summary>
	public int Priority { get; }

	/// <summary>
	/// Validates the given semantic string according to this rule's logic.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>True if the validation passes, false otherwise</returns>
	public bool Validate(ISemanticString semanticString);

	/// <summary>
	/// Gets a descriptive error message when validation fails.
	/// </summary>
	/// <param name="semanticString">The semantic string that failed validation</param>
	/// <returns>A user-friendly error message</returns>
	public string GetErrorMessage(ISemanticString semanticString);
}
