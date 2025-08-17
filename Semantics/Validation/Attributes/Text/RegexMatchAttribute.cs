// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Validates that the string matches the specified regex pattern
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class RegexMatchAttribute(string pattern, RegexOptions options = RegexOptions.None) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the regex pattern that the string must match.
	/// </summary>
	public string Pattern => pattern;

	/// <summary>
	/// Gets the regex options used for matching.
	/// </summary>
	public RegexOptions Options => options;

	/// <summary>
	/// Creates the validation adapter for regex pattern matching.
	/// </summary>
	/// <returns>A validation adapter for regex pattern matching</returns>
	protected override ValidationAdapter CreateValidator() => new RegexValidator(pattern, options);

	/// <summary>
	/// validation adapter for regex pattern matching.
	/// </summary>
	private sealed class RegexValidator : ValidationAdapter
	{
		private readonly string _pattern;
		private readonly RegexOptions _options;

		/// <summary>
		/// Initializes a new instance of the RegexValidator class.
		/// </summary>
		/// <param name="pattern">The regex pattern to match</param>
		/// <param name="options">The regex options</param>
		public RegexValidator(string pattern, RegexOptions options)
		{
			_pattern = pattern;
			_options = options;

			RuleFor(value => value)
				.Matches(_pattern, _options)
				.WithMessage($"The value must match the pattern: {_pattern}");
		}
	}
}
