// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string starts with the specified prefix
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class StartsWithAttribute(string prefix, StringComparison comparison = StringComparison.Ordinal) : FluentSemanticStringValidationAttribute
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
	/// Creates the FluentValidation validator for prefix validation.
	/// </summary>
	/// <returns>A FluentValidation validator for prefix validation</returns>
	protected override FluentValidationAdapter CreateValidator() => new StartsWithValidator(prefix, comparison);

	/// <summary>
	/// FluentValidation validator for prefix validation.
	/// </summary>
	private sealed class StartsWithValidator : FluentValidationAdapter
	{
		private readonly string _prefix;
		private readonly StringComparison _comparison;

		/// <summary>
		/// Initializes a new instance of the StartsWithValidator class.
		/// </summary>
		/// <param name="prefix">The prefix that the string must start with</param>
		/// <param name="comparison">The comparison type</param>
		public StartsWithValidator(string prefix, StringComparison comparison)
		{
			_prefix = prefix;
			_comparison = comparison;

			RuleFor(value => value)
				.Must(value => value?.StartsWith(_prefix, _comparison) == true)
				.WithMessage($"The value must start with '{_prefix}'.");
		}
	}
}
