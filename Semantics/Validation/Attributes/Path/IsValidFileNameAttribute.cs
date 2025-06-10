// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path string contains valid filename characters using span semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsValidFileNameAttribute : SemanticStringValidationAttribute
{
	private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
	{
		ReadOnlySpan<char> value = semanticString.WeakString.AsSpan();

		// Use span-based search for invalid characters
		return value.IndexOfAny(InvalidFileNameChars) == -1;
	}
}
