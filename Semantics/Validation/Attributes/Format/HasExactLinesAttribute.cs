// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

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
public sealed class HasExactLinesAttribute(int exactLines) : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the exact number of lines required.
	/// </summary>
	public int ExactLines { get; } = exactLines;

	/// <summary>
	/// Creates the FluentValidation validator for exact lines validation.
	/// </summary>
	/// <returns>A FluentValidation validator for exact lines</returns>
	protected override FluentValidationAdapter CreateValidator() => new ExactLinesValidator(ExactLines);

	/// <summary>
	/// FluentValidation validator for exact lines.
	/// </summary>
	private sealed class ExactLinesValidator : FluentValidationAdapter
	{
		private readonly int exactLines;

		/// <summary>
		/// Initializes a new instance of the ExactLinesValidator class.
		/// </summary>
		/// <param name="exactLines">The exact number of lines required</param>
		public ExactLinesValidator(int exactLines)
		{
			this.exactLines = exactLines;

			RuleFor(value => value)
				.Must(HaveExactLines)
				.WithMessage($"The text must have exactly {exactLines} line(s).");
		}

		/// <summary>
		/// Validates that a string has exactly the specified number of lines.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string has exactly the specified lines, false otherwise</returns>
		private bool HaveExactLines(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return exactLines == 0;
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

			return lineCount == exactLines;
		}
	}
}
