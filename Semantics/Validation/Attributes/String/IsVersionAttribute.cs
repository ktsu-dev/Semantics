// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted .NET Version.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Version directly:
/// - Version.Parse() / Version.TryParse() for parsing
/// - Version.ToString() for string representation
/// - Built-in comparison operators (&gt;, &lt;, ==, etc.)
/// - Rich API for version operations (Major, Minor, Build, Revision)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Version directly instead of semantic string types. System.Version provides better type safety, performance, built-in comparison operations, and rich API for version operations.")]
public sealed class IsVersionAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid .NET Version.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid Version, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => Version.TryParse(semanticString.WeakString, out _);
}
