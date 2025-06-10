// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Validation rule that checks string length constraints.
/// </summary>
/// <param name="minLength">Minimum allowed length (inclusive)</param>
/// <param name="maxLength">Maximum allowed length (inclusive)</param>
public sealed class LengthValidationRule(int minLength, int maxLength) : ValidationRuleBase
{
	private readonly int _minLength = minLength;
	private readonly int _maxLength = maxLength;

	/// <inheritdoc/>
	public override string Name => "Length";

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
		=> semanticString.Length >= _minLength && semanticString.Length <= _maxLength;

	/// <inheritdoc/>
	public override string GetErrorMessage(ISemanticString semanticString)
		=> $"String length {semanticString.Length} is not between {_minLength} and {_maxLength}";
}
