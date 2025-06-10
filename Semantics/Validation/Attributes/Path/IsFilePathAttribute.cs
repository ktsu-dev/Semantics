// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path represents a file (not an existing directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFilePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a file path by ensuring it's not an existing directory.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is empty, null, or not an existing directory; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation passes if the path doesn't exist as a directory, allowing for non-existent files
	/// and existing files. It only fails if the path exists and is specifically a directory.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !Directory.Exists(value);
	}
}
