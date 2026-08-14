// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System.Text.RegularExpressions;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers <see cref="PatternValidationRule"/>, which had no tests despite carrying the regex used
/// to validate caller-supplied values against caller-supplied patterns.
/// </summary>
[TestClass]
public class PatternValidationRuleTests
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record PlainString : SemanticString<PlainString> { }

	private static PlainString Value(string value) => SemanticString<PlainString>.Create<PlainString>(value);

	[TestMethod]
	public void Validate_MatchingValue_ReturnsTrue()
	{
		PatternValidationRule rule = new(@"^\d{3}-\d{4}$");

		Assert.IsTrue(rule.Validate(Value("555-1234")));
	}

	[TestMethod]
	public void Validate_NonMatchingValue_ReturnsFalse()
	{
		PatternValidationRule rule = new(@"^\d{3}-\d{4}$");

		Assert.IsFalse(rule.Validate(Value("not-a-number")));
	}

	[TestMethod]
	public void Validate_HonoursRegexOptions()
	{
		PatternValidationRule caseSensitive = new("^abc$");
		PatternValidationRule caseInsensitive = new("^abc$", RegexOptions.IgnoreCase);

		Assert.IsFalse(caseSensitive.Validate(Value("ABC")));
		Assert.IsTrue(caseInsensitive.Validate(Value("ABC")));
	}

	[TestMethod]
	public void Name_IsPattern() => Assert.AreEqual("Pattern", new PatternValidationRule("x").Name);

	[TestMethod]
	public void GetErrorMessage_NamesTheValueAndThePattern()
	{
		const string pattern = @"^\d+$";
		PatternValidationRule rule = new(pattern);

		string message = rule.GetErrorMessage(Value("abc"));

		Assert.Contains("abc", message);
		Assert.Contains(pattern, message);
	}

	/// <summary>
	/// The rule matches caller-supplied values against caller-supplied patterns, so a pathological
	/// combination must terminate rather than backtrack unboundedly. This pattern against a
	/// non-matching run of 'a' is the classic catastrophic-backtracking case; without the match
	/// timeout it does not finish in any practical time.
	/// </summary>
	[TestMethod]
	public void Validate_CatastrophicBacktracking_TimesOutRatherThanHanging()
	{
		PatternValidationRule rule = new("^(a+)+$");
		PlainString value = Value(new string('a', 40) + "!");

		Assert.ThrowsExactly<RegexMatchTimeoutException>(() => rule.Validate(value));
	}
}
