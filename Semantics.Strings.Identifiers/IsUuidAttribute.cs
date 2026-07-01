// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a canonical RFC 4122 UUID: lowercase 8-4-4-4-12 hexadecimal.
/// Any variant/version is accepted; the value is not version-checked.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsUuidAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for UUID validation.</summary>
	/// <returns>A validation adapter for UUIDs.</returns>
	protected override ValidationAdapter CreateValidator() => new UuidValidator();

	private sealed class UuidValidator : ValidationAdapter
	{
		private const string Pattern = "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";

		protected override ValidationResult ValidateValue(string value) =>
			Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1))
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a canonical RFC 4122 UUID (8-4-4-4-12 hexadecimal).");
	}
}
