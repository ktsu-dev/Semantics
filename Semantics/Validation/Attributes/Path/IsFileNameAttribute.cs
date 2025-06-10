// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;
using System.Linq;

/// <summary>
/// Validates that a string represents a valid filename (no invalid filename characters, not a directory)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsFileNameAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a valid filename.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the string is a valid filename; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// A valid filename must meet the following criteria:
	/// <list type="bullet">
	/// <item><description>Must not contain any characters from <see cref="Path.GetInvalidFileNameChars()"/></description></item>
	/// <item><description>Must not be an existing directory path</description></item>
	/// <item><description>Empty or null strings are considered valid</description></item>
	/// </list>
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || (!Directory.Exists(value) && !value.Intersect(Path.GetInvalidFileNameChars()).Any());
	}
}
