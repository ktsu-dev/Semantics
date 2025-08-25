// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;
using System.Linq;

/// <summary>
/// Validates that a string has at least the specified minimum number of lines
/// </summary>
/// <remarks>
/// Line count is determined by counting line break characters plus one for the final line.
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

			// Count line breaks and add 1
			int lineCount = value.Count(c => c == '\n') + 1;

			// Handle Windows-style line endings (\r\n) - don't double count
			if (value.Contains("\r\n"))
			{
				int crlfCount = value.Split(["\r\n"], StringSplitOptions.None).Length - 1;
				int lfOnlyCount = value.Count(c => c == '\n') - crlfCount;
				lineCount = crlfCount + lfOnlyCount + 1;
			}

			bool hasValidLineCount = lineCount >= minimumLines;
			return hasValidLineCount
				? ValidationResult.Success()
				: ValidationResult.Failure($"The text must have at least {minimumLines} line(s).");
		}
	}
}
