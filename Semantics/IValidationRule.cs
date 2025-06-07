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

/// <summary>
/// Base class for validation rules that provides common functionality.
/// </summary>
public abstract class ValidationRuleBase : IValidationRule
{
	/// <inheritdoc/>
	public abstract string Name { get; }

	/// <inheritdoc/>
	public virtual int Priority => 0;

	/// <inheritdoc/>
	public abstract bool Validate(ISemanticString semanticString);

	/// <inheritdoc/>
	public virtual string GetErrorMessage(ISemanticString semanticString)
		=> $"Validation rule '{Name}' failed for value: {semanticString}";
}

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

/// <summary>
/// Validation rule that checks for required patterns using regular expressions.
/// </summary>
/// <param name="pattern">The regular expression pattern to match</param>
/// <param name="options">Regular expression options</param>
public sealed class PatternValidationRule(string pattern, System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None) : ValidationRuleBase
{
	private readonly string _pattern = pattern;
	private readonly System.Text.RegularExpressions.Regex _regex = new(pattern, options);

	/// <inheritdoc/>
	public override string Name => "Pattern";

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
		=> _regex.IsMatch(semanticString.ToString());

	/// <inheritdoc/>
	public override string GetErrorMessage(ISemanticString semanticString)
		=> $"String '{semanticString}' does not match required pattern: {_pattern}";
}
