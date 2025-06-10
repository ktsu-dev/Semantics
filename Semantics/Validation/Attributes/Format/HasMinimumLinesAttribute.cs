// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

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
public sealed class HasMinimumLinesAttribute(int minimumLines) : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the minimum number of lines required.
	/// </summary>
	public int MinimumLines { get; } = minimumLines;

	/// <summary>
	/// Creates the FluentValidation validator for minimum lines validation.
	/// </summary>
	/// <returns>A FluentValidation validator for minimum lines</returns>
	protected override FluentValidationAdapter CreateValidator() => new MinimumLinesValidator(MinimumLines);

	/// <summary>
	/// FluentValidation validator for minimum lines.
	/// </summary>
	private sealed class MinimumLinesValidator : FluentValidationAdapter
	{
		private readonly int minimumLines;

		/// <summary>
		/// Initializes a new instance of the MinimumLinesValidator class.
		/// </summary>
		/// <param name="minimumLines">The minimum number of lines required</param>
		public MinimumLinesValidator(int minimumLines)
		{
			this.minimumLines = minimumLines;

			RuleFor(value => value)
				.Must(HaveMinimumLines)
				.WithMessage($"The text must have at least {minimumLines} line(s).");
		}

		/// <summary>
		/// Validates that a string has at least the minimum number of lines.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string has at least the minimum lines, false otherwise</returns>
		private bool HaveMinimumLines(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return minimumLines <= 0;
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

			return lineCount >= minimumLines;
		}
	}
}
