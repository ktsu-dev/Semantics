// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string is a properly formatted GUID/UUID.
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Guid directly:
/// - Guid.Parse() / Guid.TryParse() for parsing
/// - Guid.ToString() for string representation
/// - Guid.NewGuid() for generating new GUIDs
/// - Built-in comparison and equality operations
/// - More efficient memory usage (16 bytes vs string overhead)
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Guid directly instead of semantic string types. System.Guid provides better type safety, performance, efficient memory usage, and built-in comparison and equality operations.")]
public sealed class IsGuidAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid GUID.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid GUID, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => Guid.TryParse(semanticString.WeakString, out _);
}
