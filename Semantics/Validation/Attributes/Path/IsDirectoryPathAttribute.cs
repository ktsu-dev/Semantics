// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path represents a directory (not an existing file)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsDirectoryPathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a directory path by ensuring it's not an existing file.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is empty, null, or not an existing file; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation passes if the path doesn't exist as a file, allowing for non-existent directories
	/// and existing directories. It only fails if the path exists and is specifically a file.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !File.Exists(value);
	}
}
