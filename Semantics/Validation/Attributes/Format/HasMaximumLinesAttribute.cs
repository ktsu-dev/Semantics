// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Linq;

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
public sealed class HasMaximumLinesAttribute(int maximumLines) : SemanticStringValidationAttribute
{

	/// <summary>
	/// Gets the maximum number of lines allowed.
	/// </summary>
	public int MaximumLines { get; } = maximumLines;

	/// <summary>
	/// Validates that the semantic string has at most the maximum number of lines.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string has at most the maximum lines; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
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

		return lineCount <= MaximumLines;
	}
}
