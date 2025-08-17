// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;

/// <summary>
/// Validates that the string starts with the specified prefix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class StartsWithAttribute(string prefix, StringComparison comparison = StringComparison.Ordinal) : NativeSemanticStringValidationAttribute
{
	/// <summary>
	/// Gets the prefix that the string must start with.
	/// </summary>
	public string Prefix => prefix;

	/// <summary>
	/// Gets the comparison type used for matching.
	/// </summary>
	public StringComparison Comparison => comparison;

	/// <summary>
	/// Creates the validation adapter for prefix validation.
	/// </summary>
	/// <returns>A validation adapter for prefix validation</returns>
	protected override ValidationAdapter CreateValidator() => new StartsWithValidator(prefix, comparison);

	/// <summary>
	/// validation adapter for prefix validation.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the StartsWithValidator class.
	/// </remarks>
	/// <param name="prefix">The prefix that the string must start with</param>
	/// <param name="comparison">The comparison type</param>
	private sealed class StartsWithValidator(string prefix, StringComparison comparison) : ValidationAdapter
	{
		private readonly string _prefix = prefix;
		private readonly StringComparison _comparison = comparison;

		/// <summary>
		/// Validates that a string starts with the specified prefix.
		/// </summary>
		/// <param name="value">The string value to validate</param>
		/// <returns>A validation result indicating success or failure</returns>
		protected override ValidationResult ValidateValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return ValidationResult.Success();
			}

			bool startsWithPrefix = value.StartsWith(_prefix, _comparison);
			return startsWithPrefix
				? ValidationResult.Success()
				: ValidationResult.Failure($"The value must start with '{_prefix}'.");
		}
	}
}
