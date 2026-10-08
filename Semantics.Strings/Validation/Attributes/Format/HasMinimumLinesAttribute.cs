// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that a string has at least the specified minimum number of lines
/// </summary>
/// <remarks>
/// Line count is the number of line breaks plus one for the final line. <c>\r\n</c> is one break, and so
/// is a lone <c>\r</c>, <c>\n</c>, U+2028 or U+2029, matching <see cref="IsMultiLineAttribute"/>.
/// Empty strings are considered to have 0 lines.
/// A string with no line breaks has 1 line.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="HasMinimumLinesAttribute"/> class.
/// </remarks>
/// <param name="minimumLines">The minimum number of lines required.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasMinimumLinesAttribute(int minimumLines) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the minimum number of lines required.
	/// </summary>
	public int MinimumLines { get; } = minimumLines;

	/// <summary>
	/// Creates the validation adapter for minimum lines validation.
	/// </summary>
	/// <returns>A validation adapter for minimum lines</returns>
	protected override ValidationAdapter CreateValidator() => new MinimumLinesValidator(MinimumLines);

	/// <summary>
	/// validation adapter for minimum lines.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the MinimumLinesValidator class.
	/// </remarks>
	/// <param name="minimumLines">The minimum number of lines required</param>
	private sealed class MinimumLinesValidator(int minimumLines) : ValidationAdapter
	{

		/// <summary>
		/// Validates that a string has at least the minimum number of lines.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				bool isValid = minimumLines <= 0;
				return isValid
					? ValidationResult.Success()
					: ValidationResult.Failure($"The text must have at least {minimumLines} line(s).");
			}

			int lineCount = LineBreaks.CountLines(value);

			bool hasValidLineCount = lineCount >= minimumLines;
			return hasValidLineCount
				? ValidationResult.Success()
				: ValidationResult.Failure($"The text must have at least {minimumLines} line(s).");
		}
	}
}
