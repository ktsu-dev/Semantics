// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

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
