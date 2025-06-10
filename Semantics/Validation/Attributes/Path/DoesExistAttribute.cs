// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.IO;

/// <summary>
/// Validates that a path exists on the filesystem
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DoesExistAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string represents a path that exists on the filesystem.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>
	/// <see langword="true"/> if the path exists as either a file or directory; otherwise, <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This validation requires the path to actually exist on the filesystem as either a file or directory.
	/// Empty or null strings are considered invalid and will fail validation.
	/// The validation uses both <see cref="File.Exists(string)"/> and <see cref="Directory.Exists(string)"/>
	/// to check for existence.
	/// </remarks>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return !string.IsNullOrEmpty(value) && (File.Exists(value) || Directory.Exists(value));
	}
}
