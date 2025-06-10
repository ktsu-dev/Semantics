// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Net;

/// <summary>
/// Validates that the string is a properly formatted IP address (IPv4 or IPv6).
///
/// RECOMMENDATION: Instead of using this attribute, consider using System.Net.IPAddress directly:
/// - IPAddress.Parse() / IPAddress.TryParse() for parsing
/// - IPAddress.ToString() for string representation
/// - Built-in support for both IPv4 and IPv6
/// - Rich API for network operations
///
/// Use this attribute only when you specifically need string-based semantic validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
[Obsolete("Consider using System.Net.IPAddress directly instead of semantic string types. IPAddress provides better type safety, performance, built-in IPv4/IPv6 support, and rich API for network operations.")]
public sealed class IsIpAddressAttribute : SemanticStringValidationAttribute
{
	/// <summary>
	/// Validates that the semantic string is a valid IP address.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate.</param>
	/// <returns>True if the string is a valid IP address, false otherwise.</returns>
	public override bool Validate(ISemanticString semanticString) => IPAddress.TryParse(semanticString.WeakString, out _);
}
