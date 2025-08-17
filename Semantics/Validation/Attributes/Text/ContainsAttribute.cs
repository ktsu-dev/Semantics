// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string contains the specified substring
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ContainsAttribute(string substring, StringComparison comparison = StringComparison.Ordinal) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the substring that the string must contain.
	/// </summary>
	public string Substring => substring;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Creates the validation adapter for substring validation.
	/// </summary>
	/// <returns>A validation adapter for substring validation</returns>
	protected override ValidationAdapter CreateValidator() => new ContainsValidator(substring, comparison);

	/// <summary>
	/// validation adapter for substring validation.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the ContainsValidator class.
	/// </remarks>
	/// <param name="substring">The substring that the string must contain</param>
	/// <param name="comparison">The comparison type</param>
	private sealed class ContainsValidator(string substring, StringComparison comparison) : ValidationAdapter
	{
		private readonly string _substring = substring;
		private readonly StringComparison _comparison = comparison;

		/// <summary>
		/// Validates that a string contains the specified substring.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = value.Contains(_substring, _comparison);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure($"The value must contain '{_substring}'.");
		}
	}
}
