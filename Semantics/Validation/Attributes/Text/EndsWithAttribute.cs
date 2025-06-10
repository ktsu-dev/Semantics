// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string ends with the specified suffix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class EndsWithAttribute(string suffix, StringComparison comparison = StringComparison.Ordinal) : FluentSemanticStringValidationAttribute
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
	/// Creates the FluentValidation validator for suffix validation.
	/// </summary>
	/// <returns>A FluentValidation validator for suffix validation</returns>
	protected override FluentValidationAdapter CreateValidator() => new EndsWithValidator(suffix, comparison);

	/// <summary>
	/// FluentValidation validator for suffix validation.
	/// </summary>
	private sealed class EndsWithValidator : FluentValidationAdapter
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

			RuleFor(value => value)
				.Must(value => value?.EndsWith(_suffix, _comparison) == true)
				.WithMessage($"The value must end with '{_suffix}'.");
		}
	}
}
