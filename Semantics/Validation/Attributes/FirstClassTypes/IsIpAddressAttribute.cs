// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using System.Net;
using FluentValidation;

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
public sealed class IsIpAddressAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for IP address validation.
	/// </summary>
	/// <returns>A FluentValidation validator for IP address strings</returns>
	protected override FluentValidationAdapter CreateValidator() => new IpAddressValidator();

	/// <summary>
	/// FluentValidation validator for IP address strings.
	/// </summary>
	private sealed class IpAddressValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the IpAddressValidator class.
		/// </summary>
		public IpAddressValidator()
		{
			RuleFor(value => value)
				.Must(BeValidIpAddress)
				.WithMessage("The value must be a valid IP address (IPv4 or IPv6).")
				.When(value => !string.IsNullOrEmpty(value));
		}

		/// <summary>
		/// Validates that a string is a valid IP address.
		/// </summary>
		/// <param name="value">The string to validate</param>
		/// <returns>True if the string is a valid IP address, false otherwise</returns>
		private static bool BeValidIpAddress(string value) => IPAddress.TryParse(value, out _);
	}
}
