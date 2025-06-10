// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System;
using FluentValidation;

/// <summary>
/// Validates that the string has basic email address format (contains @ with valid characters).
/// For full RFC compliance, use MailAddress.TryCreate() in your application code.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IsEmailAddressAttribute : FluentSemanticStringValidationAttribute
{
	/// <summary>
	/// Creates the FluentValidation validator for email address validation.
	/// </summary>
	/// <returns>A FluentValidation validator for email addresses</returns>
	protected override FluentValidationAdapter CreateValidator() => new EmailValidator();

	/// <summary>
	/// FluentValidation validator for email addresses.
	/// </summary>
	private sealed class EmailValidator : FluentValidationAdapter
	{
		/// <summary>
		/// Initializes a new instance of the EmailValidator class.
		/// </summary>
		public EmailValidator()
		{
			RuleFor(value => value)
				.EmailAddress()
				.WithMessage("The value must be a valid email address.")
				.When(value => !string.IsNullOrEmpty(value));

			RuleFor(value => value)
				.MaximumLength(254)
				.WithMessage("Email address cannot exceed 254 characters.")
				.When(value => !string.IsNullOrEmpty(value));
		}
	}
}
