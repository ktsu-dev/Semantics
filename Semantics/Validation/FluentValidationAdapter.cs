// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using FluentValidation;
using FluentValidation.Results;

/// <summary>
/// Adapter that uses FluentValidation internally to implement validation logic for semantic string attributes.
/// </summary>
public abstract class FluentValidationAdapter : AbstractValidator<string>
{
	/// <summary>
	/// Validates a semantic string using FluentValidation internally.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>True if validation passes, false otherwise</returns>
	public bool ValidateSemanticString(ISemanticString semanticString)
	{
		string value = semanticString?.WeakString ?? string.Empty;
		ValidationResult result = Validate(value);
		return result.IsValid;
	}

	/// <summary>
	/// Gets validation errors for a semantic string.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>Collection of validation error messages</returns>
	public IEnumerable<string> GetValidationErrors(ISemanticString semanticString)
	{
		string value = semanticString?.WeakString ?? string.Empty;
		ValidationResult result = Validate(value);
		return result.Errors.Select(e => e.ErrorMessage);
	}
}

/// <summary>
/// Base attribute for semantic string validation that uses FluentValidation internally.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class FluentSemanticStringValidationAttribute : SemanticStringValidationAttribute
{
	private readonly Lazy<FluentValidationAdapter> _validator;

	/// <summary>
	/// Initializes a new instance of the FluentSemanticStringValidationAttribute class.
	/// </summary>
	protected FluentSemanticStringValidationAttribute() => _validator = new Lazy<FluentValidationAdapter>(CreateValidator);

	/// <summary>
	/// Creates the FluentValidation validator for this attribute.
	/// </summary>
	/// <returns>A FluentValidation validator</returns>
	protected abstract FluentValidationAdapter CreateValidator();

	/// <summary>
	/// Validates a SemanticString using FluentValidation internally.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string passes validation, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => _validator.Value.ValidateSemanticString(semanticString);

	/// <summary>
	/// Gets validation errors for the semantic string.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>Collection of validation error messages</returns>
	public virtual IEnumerable<string> GetValidationErrors(ISemanticString semanticString) => _validator.Value.GetValidationErrors(semanticString);
}
