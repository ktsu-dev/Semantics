// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using System.Linq;

/// <summary>
/// Validates that a string represents a valid path with no invalid path characters and a reasonable length.
/// </summary>
/// <remarks>
/// This attribute enforces the following rules:
/// <list type="bullet">
/// <item><description>Path length must not exceed 256 characters</description></item>
/// <item><description>Path must not contain any characters returned by <see cref="Path.GetInvalidPathChars()"/></description></item>
/// <item><description>Empty or null strings are considered valid</description></item>
/// </list>
/// The 256-character limit provides a reasonable balance between compatibility and practical usage,
/// while being more restrictive than the maximum path lengths supported by most file systems.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsPathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a valid path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a valid path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		if (string.IsNullOrEmpty(value))
		{
			return true;
		}

		if (value.Length > 256)
		{
			return false;
		}

		// Check for characters from GetInvalidPathChars() and additional problematic characters
		// In .NET Core+, GetInvalidPathChars() doesn't include all characters that can cause issues in paths
		char[] invalidChars = [.. Path.GetInvalidPathChars(), '<', '>', '|'];
		return !value.Intersect(invalidChars).Any();
	}
}
