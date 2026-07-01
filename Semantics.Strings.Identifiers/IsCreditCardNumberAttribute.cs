// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is a payment card number: 13-19 digits passing the Luhn (mod-10) check.
/// This is a format/checksum check only — it performs no BIN/issuer/network detection and offers no
/// PCI guarantees. Do not log the underlying value.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsCreditCardNumberAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for credit card number validation.</summary>
	/// <returns>A validation adapter for credit card numbers.</returns>
	protected override ValidationAdapter CreateValidator() => new CreditCardNumberValidator();

	private sealed class CreditCardNumberValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length is < 13 or > 19)
			{
				return ValidationResult.Failure("A credit card number must have between 13 and 19 digits.");
			}

			foreach (char c in value)
			{
				if (c is < '0' or > '9')
				{
					return ValidationResult.Failure("A credit card number must contain digits only.");
				}
			}

			return PassesLuhn(value)
				? ValidationResult.Success()
				: ValidationResult.Failure("The value must be a valid credit card number (Luhn check failed).");
		}

		private static bool PassesLuhn(string digits)
		{
			int sum = 0;
			bool alternate = false;
			for (int i = digits.Length - 1; i >= 0; i--)
			{
				int n = digits[i] - '0';
				if (alternate)
				{
					n *= 2;
					if (n is > 9)
					{
						n -= 9;
					}
				}

				sum += n;
				alternate = !alternate;
			}

			return (sum % 10) == 0;
		}
	}
}
