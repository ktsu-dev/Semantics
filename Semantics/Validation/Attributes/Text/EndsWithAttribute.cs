// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string ends with the specified suffix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class EndsWithAttribute(string suffix, StringComparison comparison = StringComparison.Ordinal) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the suffix that the string must end with.
	/// </summary>
	public string Suffix => suffix;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Creates the validation adapter for suffix validation.
	/// </summary>
	/// <returns>A validation adapter for suffix validation</returns>
	protected override ValidationAdapter CreateValidator() => new EndsWithValidator(suffix, comparison);

	/// <summary>
	/// validation adapter for suffix validation.
	/// </summary>
	private sealed class EndsWithValidator : ValidationAdapter
	{
		private readonly string _suffix;
		private readonly StringComparison _comparison;

		/// <summary>
		/// Initializes a new instance of the EndsWithValidator class.
		/// </summary>
		/// <param name="suffix">The suffix that the string must end with</param>
		/// <param name="comparison">The comparison type</param>
		public EndsWithValidator(string suffix, StringComparison comparison)
		{
			_suffix = suffix;
			_comparison = comparison;
		}

		/// <summary>
		/// Validates that a string ends with the specified suffix.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool isValid = value.EndsWith(_suffix, _comparison);
			return isValid
				? ValidationResult.Success()
				: ValidationResult.Failure($"The value must end with '{_suffix}'.");
		}
	}
}
