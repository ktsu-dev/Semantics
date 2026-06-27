// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

using System;

/// <summary>
/// Base attribute for semantic string validation
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class SemanticStringValidationAttribute : Attribute
{
	/// <summary>
	/// Validates a SemanticString against the criteria defined by this attribute.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string passes validation, false otherwise</returns>
	public abstract bool Validate(ISemanticString semanticString);
}
