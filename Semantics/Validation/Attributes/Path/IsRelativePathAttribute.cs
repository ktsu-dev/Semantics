// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path is relative (not fully qualified), meaning it does not start from a root directory.
/// </summary>
/// <remarks>
/// A relative path is one that specifies a location relative to the current working directory or another specified directory.
/// Examples of relative paths:
/// <list type="bullet">
/// <item><description><c>file.txt</c> - file in current directory</description></item>
/// <item><description><c>folder/file.txt</c> - file in subdirectory</description></item>
/// <item><description><c>../file.txt</c> - file in parent directory</description></item>
/// <item><description><c>./folder/file.txt</c> - file in subdirectory (explicit current directory)</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> to determine if a path is absolute.
/// Empty or null strings are considered valid relative paths.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsRelativePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a relative path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a relative path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || !Path.IsPathFullyQualified(value);
	}
}
