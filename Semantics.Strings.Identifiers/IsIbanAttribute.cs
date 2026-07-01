// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;
using System.Text.RegularExpressions;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is an IBAN: two letters, two check digits, then alphanumerics, total
/// length 15-34, satisfying the ISO 7064 mod-97-10 checksum. Country-specific BBAN length/format
/// tables are not enforced.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsIbanAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for IBAN validation.</summary>
	/// <returns>A validation adapter for IBANs.</returns>
	protected override ValidationAdapter CreateValidator() => new IbanValidator();

	private sealed class IbanValidator : ValidationAdapter
	{
		private const string Pattern = "^[A-Z]{2}[0-9]{2}[A-Z0-9]+$";

		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length is < 15 or > 34)
			{
				return ValidationResult.Failure("An IBAN must have between 15 and 34 characters.");
			}

			if (!Regex.IsMatch(value, Pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
			{
				return ValidationResult.Failure("The value must be a valid IBAN (two letters, two check digits, then alphanumerics).");
			}

			return PassesMod97(value)
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid IBAN (mod-97 checksum failed).");
		}

		private static int ApplyChar(char c, int remainder)
		{
			if (c is >= '0' and <= '9')
			{
				return ((remainder * 10) + (c - '0')) % 97;
			}

			return ((remainder * 100) + c - 'A' + 10) % 97;
		}

		private static bool PassesMod97(string iban)
		{
			int remainder = 0;
			for (int i = 4; i < iban.Length; i++)
			{
				remainder = ApplyChar(iban[i], remainder);
			}

			for (int i = 0; i < 4; i++)
			{
				remainder = ApplyChar(iban[i], remainder);
			}

			return remainder == 1;
		}
	}
}
