// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

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
public sealed class IsIpAddressAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the validation adapter for IP address validation.
	/// </summary>
	/// <returns>A validation adapter for IP address strings</returns>
	protected override ValidationAdapter CreateValidator() => new IpAddressValidator();

	/// <summary>
	/// validation adapter for IP address strings.
	/// </summary>
	private sealed class IpAddressValidator : ValidationAdapter
	{
		/// <summary>
		/// Validates that a string is a valid IP address.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = IPAddress.TryParse(value, out _);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid IP address (IPv4 or IPv6).");
		}
	}
}
