// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string contains the specified substring
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ContainsAttribute(string substring, StringComparison comparison = StringComparison.Ordinal) : FluentSemanticStringValidationAttribute
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
	/// Creates the FluentValidation validator for substring validation.
	/// </summary>
	/// <returns>A FluentValidation validator for substring validation</returns>
	protected override FluentValidationAdapter CreateValidator() => new ContainsValidator(substring, comparison);

	/// <summary>
	/// FluentValidation validator for substring validation.
	/// </summary>
	private sealed class ContainsValidator : FluentValidationAdapter
	{
		private readonly string _substring;
		private readonly StringComparison _comparison;

		/// <summary>
		/// Initializes a new instance of the ContainsValidator class.
		/// </summary>
		/// <param name="substring">The substring that the string must contain</param>
		/// <param name="comparison">The comparison type</param>
		public ContainsValidator(string substring, StringComparison comparison)
		{
			_substring = substring;
			_comparison = comparison;

			RuleFor(value => value)
				.Must(value => value?.Contains(_substring, _comparison) == true)
				.WithMessage($"The value must contain '{_substring}'.");
		}
	}
}
