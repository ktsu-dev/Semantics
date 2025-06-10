// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;
using FluentValidation;

/// <summary>
/// Validates that a string has at most the specified maximum number of lines
/// </summary>
/// <remarks>
/// Line count is determined by counting line break characters plus one for the final line.
/// Empty strings are considered to have 0 lines.
/// A string with no line breaks has 1 line.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="HasMaximumLinesAttribute"/> class.
/// </remarks>
/// <param name="maximumLines">The maximum number of lines allowed.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class HasMaximumLinesAttribute(int maximumLines) : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the maximum number of lines allowed.
	/// </summary>
	public int MaximumLines { get; } = maximumLines;

	/// <summary>
	/// Creates the FluentValidation validator for maximum lines validation.
	/// </summary>
	/// <returns>A FluentValidation validator for maximum lines</returns>
	protected override FluentValidationAdapter CreateValidator() => new MaximumLinesValidator(MaximumLines);

	/// <summary>
	/// FluentValidation validator for maximum lines.
	/// </summary>
	private sealed class MaximumLinesValidator : FluentValidationAdapter
	{
		private readonly int maximumLines;

		/// <summary>
		/// Initializes a new instance of the MaximumLinesValidator class.
		/// </summary>
		/// <param name="maximumLines">The maximum number of lines allowed</param>
		public MaximumLinesValidator(int maximumLines)
		{
			this.maximumLines = maximumLines;

			RuleFor(value => value)
				.Must(HaveMaximumLines)
				.WithMessage($"The text must have at most {maximumLines} line(s).");
		}

		/// <summary>
		/// Validates that a string has at most the maximum number of lines.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string has at most the maximum lines, false otherwise</returns>
		private bool HaveMaximumLines(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true; // Empty strings have 0 lines, which is <= any positive maximum
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

			return lineCount <= maximumLines;
		}
	}
}
