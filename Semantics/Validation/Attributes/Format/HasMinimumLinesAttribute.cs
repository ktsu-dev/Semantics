// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

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
public sealed class HasMinimumLinesAttribute(int minimumLines) : SemanticStringValidationAttribute
{

	/// <summary>
	/// Gets the minimum number of lines required.
	/// </summary>
	public int MinimumLines { get; } = minimumLines;

	/// <summary>
	/// Validates that the semantic string has at least the minimum number of lines.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string has at least the minimum lines; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return MinimumLines <= 0;
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

		return lineCount >= MinimumLines;
	}
}
