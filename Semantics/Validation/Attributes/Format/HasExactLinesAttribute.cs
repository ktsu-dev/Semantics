// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

/// <summary>
/// Validates that a string has exactly the specified number of lines
/// </summary>
/// <remarks>
/// Line count is determined by counting line break characters plus one for the final line.
/// Empty strings are considered to have 0 lines.
/// A string with no line breaks has 1 line.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="HasExactLinesAttribute"/> class.
/// </remarks>
/// <param name="exactLines">The exact number of lines required.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasExactLinesAttribute(int exactLines) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the exact number of lines required.
	/// </summary>
	public int ExactLines { get; } = exactLines;

	/// <summary>
	/// Creates the validation adapter for exact lines validation.
	/// </summary>
	/// <returns>A validation adapter for exact lines</returns>
	protected override ValidationAdapter CreateValidator() => new ExactLinesValidator(ExactLines);

	/// <summary>
	/// validation adapter for exact lines.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the ExactLinesValidator class.
	/// </remarks>
	/// <param name="exactLines">The exact number of lines required</param>
	private sealed class ExactLinesValidator(int exactLines) : ValidationAdapter
	{

		/// <summary>
		/// Validates that a string has exactly the specified number of lines.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				bool isValid = exactLines == 0;
				return isValid
					? ValidationResult.Success()
					: ValidationResult.Failure($"The text must have exactly {exactLines} line(s).");
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

			bool hasExactLines = lineCount == exactLines;
			return hasExactLines
				? ValidationResult.Success()
				: ValidationResult.Failure($"The text must have exactly {exactLines} line(s).");
		}
	}
}
