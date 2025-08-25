// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings;

/// <summary>
/// Validation result containing the validation status and any error messages.
/// </summary>
public class ValidationResult
{
	/// <summary>
	/// Gets a value indicating whether the validation passed.
	/// </summary>
	public bool IsValid { get; set; }

	/// <summary>
	/// Gets the collection of validation error messages.
	/// </summary>
	public IReadOnlyList<string> Errors { get; set; } = [];

	/// <summary>
	/// Creates a successful validation result.
	/// </summary>
	/// <returns>A successful validation result</returns>
	public static ValidationResult Success() => new() { IsValid = true };

	/// <summary>
	/// Creates a failed validation result with the specified error message.
	/// </summary>
	/// <param name="errorMessage">The error message</param>
	/// <returns>A failed validation result</returns>
	public static ValidationResult Failure(string errorMessage) => new()
	{
		IsValid = false,
		Errors = [errorMessage]
	};

	/// <summary>
	/// Creates a failed validation result with the specified error messages.
	/// </summary>
	/// <param name="errorMessages">The error messages</param>
	/// <returns>A failed validation result</returns>
	public static ValidationResult Failure(IEnumerable<string> errorMessages) => new()
	{
		IsValid = false,
		Errors = [.. errorMessages]
	};
}

/// <summary>
/// Base class for native validation adapters that implement validation logic for semantic string attributes.
/// </summary>
public abstract class ValidationAdapter
{
	/// <summary>
	/// Validates a semantic string.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>A validation result indicating success or failure with error messages</returns>
	public ValidationResult ValidateSemanticString(ISemanticString semanticString)
	{
		string value = semanticString?.WeakString ?? string.Empty;
		return ValidateValue(value);
	}

	/// <summary>
	/// Validates a string value.
	/// </summary>
	/// <param name="value">The string value to validate</param>
	/// <returns>A validation result indicating success or failure with error messages</returns>
	protected abstract ValidationResult ValidateValue(string value);

	/// <summary>
	/// Gets validation errors for a semantic string.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>Collection of validation error messages</returns>
	public IEnumerable<string> GetValidationErrors(ISemanticString semanticString)
	{
		ValidationResult result = ValidateSemanticString(semanticString);
		return result.Errors;
	}
}

/// <summary>
/// Base attribute for semantic string validation that uses native validation internally.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class NativeSemanticStringValidationAttribute : SemanticStringValidationAttribute
{
	private readonly Lazy<ValidationAdapter> _validator;

	/// <summary>
	/// Initializes a new instance of the NativeSemanticStringValidationAttribute class.
	/// </summary>
	protected NativeSemanticStringValidationAttribute() => _validator = new Lazy<ValidationAdapter>(CreateValidator);

	/// <summary>
	/// Creates the validation adapter for this attribute.
	/// </summary>
	/// <returns>A validation adapter</returns>
	protected abstract ValidationAdapter CreateValidator();

	/// <summary>
	/// Validates a SemanticString using native validation internally.
	/// </summary>
	/// <param name="semanticString">The SemanticString to validate</param>
	/// <returns>True if the string passes validation, false otherwise</returns>
	public override bool Validate(ISemanticString semanticString) => _validator.Value.ValidateSemanticString(semanticString).IsValid;

	/// <summary>
	/// Gets validation errors for the semantic string.
	/// </summary>
	/// <param name="semanticString">The semantic string to validate</param>
	/// <returns>Collection of validation error messages</returns>
	public virtual IEnumerable<string> GetValidationErrors(ISemanticString semanticString) => _validator.Value.GetValidationErrors(semanticString);
}
