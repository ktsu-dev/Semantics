// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a ULID: 26 uppercase Crockford base32 characters (excluding I, L, O, U)
/// whose first character is in the range 0-7 (the 48-bit timestamp cannot overflow past that).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUlidAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for ULID validation.</summary>
	/// <returns>A validation adapter for ULIDs.</returns>
	protected override ValidationAdapter CreateValidator() => new UlidValidator();

	private sealed class UlidValidator : ValidationAdapter
	{
		private const string Pattern = "^[0-7][0-9A-HJKMNP-TV-Z]{25}$";

		protected override ValidationResult ValidateValue(string value) =>
			Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1))
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a 26-character Crockford base32 ULID.");
	}
}
