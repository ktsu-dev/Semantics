// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Validates that a string has at most the specified maximum number of lines
/// </summary>
/// <remarks>
/// Line count is the number of line breaks plus one for the final line. <c>\r\n</c> is one break, and so
/// is a lone <c>\r</c>, <c>\n</c>, U+2028 or U+2029, matching <see cref="IsMultiLineAttribute"/>.
/// Empty strings are considered to have 0 lines.
/// A string with no line breaks has 1 line.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="HasMaximumLinesAttribute"/> class.
/// </remarks>
/// <param name="maximumLines">The maximum number of lines allowed.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasMaximumLinesAttribute(int maximumLines) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the maximum number of lines allowed.
	/// </summary>
	public int MaximumLines { get; } = maximumLines;

	/// <summary>
	/// Creates the validation adapter for maximum lines validation.
	/// </summary>
	/// <returns>A validation adapter for maximum lines</returns>
	protected override ValidationAdapter CreateValidator() => new MaximumLinesValidator(MaximumLines);

	/// <summary>
	/// validation adapter for maximum lines.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the MaximumLinesValidator class.
	/// </remarks>
	/// <param name="maximumLines">The maximum number of lines allowed</param>
	private sealed class MaximumLinesValidator(int maximumLines) : ValidationAdapter
	{

		/// <summary>
		/// Validates that a string has at most the maximum number of lines.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success(); // Empty strings have 0 lines, which is <= any positive maximum
			}

			int lineCount = LineBreaks.CountLines(value);

			bool hasValidLineCount = lineCount <= maximumLines;
			return hasValidLineCount
				? ValidationResult.Success()
				: ValidationResult.Failure($"The text must have at most {maximumLines} line(s).");
		}
	}
}
