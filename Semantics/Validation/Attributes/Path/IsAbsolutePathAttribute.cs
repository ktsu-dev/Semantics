// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path is absolute (fully qualified), meaning it specifies a complete path from the root of the file system.
/// </summary>
/// <remarks>
/// An absolute path provides the complete location of a file or directory from the root directory.
/// Examples of absolute paths:
/// <list type="bullet">
/// <item><description><c>C:\Windows\System32</c> - Windows absolute path</description></item>
/// <item><description><c>/usr/local/bin</c> - Unix/Linux absolute path</description></item>
/// <item><description><c>\\server\share\file.txt</c> - UNC path</description></item>
/// </list>
/// This validation uses <see cref="Path.IsPathFullyQualified(string)"/> with a directory separator appended
/// to handle edge cases where the path might be interpreted differently.
/// Empty or null strings are considered valid for flexibility in initialization scenarios.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsAbsolutePathAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents an absolute path.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is an absolute path; otherwise, <see langword="false"/>.
	/// </returns>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || Path.IsPathFullyQualified(value + Path.DirectorySeparatorChar);
	}
}
