// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that a string represents a valid file extension (starts with a period)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsExtensionAttribute : SemanticStringValidationAttribute
{
	/// <inheritdoc/>
	public override bool Validate(ISemanticString semanticString)
	{
		string value = semanticString.WeakString;
		return string.IsNullOrEmpty(value) || value.StartsWith('.');
	}
}
