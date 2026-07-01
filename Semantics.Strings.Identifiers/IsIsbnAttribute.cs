// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using System;

using ktsu.Semantics.Strings;

/// <summary>
/// Validates that the string is an ISBN-10 (weighted mod-11, final digit may be X) or an ISBN-13
/// (mod-10), evaluated on the separator-stripped value. Registration-group and publisher ranges are
/// not validated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsIsbnAttribute : NativeSemanticStringValidationAttribute
{
	/// <summary>Creates the validation adapter for ISBN validation.</summary>
	/// <returns>A validation adapter for ISBNs.</returns>
	protected override ValidationAdapter CreateValidator() => new IsbnValidator();

	private sealed class IsbnValidator : ValidationAdapter
	{
		protected override ValidationResult ValidateValue(string value)
		{
			if (value.Length == 10 && IsValidIsbn10(value))
			{
				return ValidationResult.Success();
			}

			if (value.Length == 13 && IsValidIsbn13(value))
			{
				return ValidationResult.Success();
			}

			return ValidationResult.Failure("The value must be a valid ISBN-10 or ISBN-13.");
		}

		private static bool IsValidIsbn10(string s)
		{
			int sum = 0;
			for (int i = 0; i < 10; i++)
			{
				char c = s[i];
				int d;
				if (c is >= '0' and <= '9')
				{
					d = c - '0';
				}
				else if (c == 'X' && i == 9)
				{
					d = 10;
				}
				else
				{
					return false;
				}

				sum += (10 - i) * d;
			}

			return (sum % 11) == 0;
		}

		private static bool IsValidIsbn13(string s)
		{
			int sum = 0;
			for (int i = 0; i < 13; i++)
			{
				char c = s[i];
				if (c is < '0' or > '9')
				{
					return false;
				}

				int d = c - '0';
				sum += ((i % 2) == 0) ? d : (d * 3);
			}

			return (sum % 10) == 0;
		}
	}
}
