// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

/// <summary>
/// Validation rule that checks for required patterns using regular expressions.
/// </summary>
/// <param name="pattern">The regular expression pattern to match</param>
/// <param name="options">Regular expression options</param>
public sealed class PatternValidationRule(string pattern, System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.None) : ValidationRuleBase
{
	/// <summary>
	/// Bounds how long a single match may run. Both the pattern and the value being validated come
	/// from the caller, so a pathological combination could otherwise backtrack unboundedly
	/// (catastrophic backtracking). A match exceeding this throws
	/// <see cref="System.Text.RegularExpressions.RegexMatchTimeoutException"/> rather than hanging.
	/// </summary>
	private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);

	private readonly string _pattern = pattern;
	private readonly System.Text.RegularExpressions.Regex _regex = new(pattern, options, MatchTimeout);

	/// <inheritdoc/>
	public override string Name => "Pattern";

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
		=> _regex.IsMatch(semanticString.ToString());

	/// <inheritdoc/>
	public override string GetErrorMessage(ISemanticString semanticString)
		=> $"String '{semanticString}' does not match required pattern: {_pattern}";
}
