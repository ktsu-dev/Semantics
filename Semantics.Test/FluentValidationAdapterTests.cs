// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using FluentValidation;
using ktsu.Semantics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for FluentValidationAdapter classes that provide integration with FluentValidation.
/// </summary>
[TestClass]
public class FluentValidationAdapterTests
{
	/// <summary>
	/// Test implementation of FluentValidationAdapter for testing purposes.
	/// </summary>
	private sealed class TestFluentValidationAdapter : FluentValidationAdapter
	{
		public TestFluentValidationAdapter()
		{
			RuleFor(x => x)
				.NotEmpty()
				.WithMessage("Value cannot be empty")
				.Length(3, 10)
				.WithMessage("Value must be between 3 and 10 characters");
		}
	}

	/// <summary>
	/// Test implementation of FluentSemanticStringValidationAttribute for testing purposes.
	/// </summary>
	private sealed class TestFluentSemanticStringValidationAttribute : FluentSemanticStringValidationAttribute
	{
		protected override FluentValidationAdapter CreateValidator() => new TestFluentValidationAdapter();
	}

	/// <summary>
	/// Test semantic string with validation attribute applied.
	/// </summary>
	[TestFluentSemanticStringValidation]
	private sealed record TestSemanticString : SemanticString<TestSemanticString>
	{
		public TestSemanticString() : this(string.Empty) { }
		public TestSemanticString(string value) : base() => WeakString = value;
	}

	[TestMethod]
	public void ValidateSemanticString_ValidValue_ReturnsTrue()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("valid");

		// Act
		bool result = adapter.ValidateSemanticString(semanticString);

		// Assert
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void ValidateSemanticString_InvalidValue_ReturnsFalse()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("x"); // Too short

		// Act
		bool result = adapter.ValidateSemanticString(semanticString);

		// Assert
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void ValidateSemanticString_EmptyValue_ReturnsFalse()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("");

		// Act
		bool result = adapter.ValidateSemanticString(semanticString);

		// Assert
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void ValidateSemanticString_NullSemanticString_ReturnsFalse()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();

		// Act
		bool result = adapter.ValidateSemanticString(null!);

		// Assert
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void GetValidationErrors_ValidValue_ReturnsEmptyCollection()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("valid");

		// Act
		List<string> errors = [.. adapter.GetValidationErrors(semanticString)];

		// Assert
		Assert.AreEqual(0, errors.Count);
	}

	[TestMethod]
	public void GetValidationErrors_InvalidValue_ReturnsErrorMessages()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("x"); // Too short

		// Act
		List<string> errors = [.. adapter.GetValidationErrors(semanticString)];

		// Assert
		Assert.IsTrue(errors.Count > 0);
		Assert.IsTrue(errors.Any(e => e.Contains("must be between 3 and 10 characters")));
	}

	[TestMethod]
	public void GetValidationErrors_EmptyValue_ReturnsEmptyErrorMessage()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("");

		// Act
		List<string> errors = [.. adapter.GetValidationErrors(semanticString)];

		// Assert
		Assert.IsTrue(errors.Count > 0);
		Assert.IsTrue(errors.Any(e => e.Contains("cannot be empty")));
	}

	[TestMethod]
	public void GetValidationErrors_NullSemanticString_HandlesGracefully()
	{
		// Arrange
		TestFluentValidationAdapter adapter = new();

		// Act
		List<string> errors = [.. adapter.GetValidationErrors(null!)];

		// Assert
		Assert.IsTrue(errors.Count > 0); // Should have validation errors for empty value
	}

	[TestMethod]
	public void FluentSemanticStringValidationAttribute_Validate_ValidValue_ReturnsTrue()
	{
		// Arrange
		TestFluentSemanticStringValidationAttribute attribute = new();
		TestSemanticString semanticString = new("valid");

		// Act
		bool result = attribute.Validate(semanticString);

		// Assert
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void FluentSemanticStringValidationAttribute_Validate_InvalidValue_ReturnsFalse()
	{
		// Arrange
		TestFluentSemanticStringValidationAttribute attribute = new();
		TestSemanticString semanticString = new("x"); // Too short

		// Act
		bool result = attribute.Validate(semanticString);

		// Assert
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void FluentSemanticStringValidationAttribute_GetValidationErrors_InvalidValue_ReturnsErrors()
	{
		// Arrange
		TestFluentSemanticStringValidationAttribute attribute = new();
		TestSemanticString semanticString = new("x"); // Too short

		// Act
		List<string> errors = [.. attribute.GetValidationErrors(semanticString)];

		// Assert
		Assert.IsTrue(errors.Count > 0);
		Assert.IsTrue(errors.Any(e => e.Contains("must be between 3 and 10 characters")));
	}

	[TestMethod]
	public void FluentSemanticStringValidationAttribute_ValidatorIsLazilyCreated()
	{
		// Arrange
		TestFluentSemanticStringValidationAttribute attribute = new();
		TestSemanticString semanticString = new("valid");

		// Act - Call multiple times to ensure the same validator instance is reused
		bool result1 = attribute.Validate(semanticString);
		bool result2 = attribute.Validate(semanticString);

		// Assert
		Assert.IsTrue(result1);
		Assert.IsTrue(result2);
	}

	/// <summary>
	/// Tests that the validator works correctly when applied as an attribute to a semantic string class.
	/// </summary>
	[TestMethod]
	public void SemanticStringWithFluentValidation_CreatesAndValidatesCorrectly()
	{
		// Arrange & Act
		TestSemanticString validString = new("valid");
		TestSemanticString invalidString = new("x");

		// Assert - Just ensuring no exceptions are thrown during creation
		Assert.IsNotNull(validString);
		Assert.IsNotNull(invalidString);
		Assert.AreEqual("valid", validString.WeakString);
		Assert.AreEqual("x", invalidString.WeakString);
	}

	/// <summary>
	/// Test validator with multiple rules to ensure comprehensive error collection.
	/// </summary>
	private sealed class MultiRuleFluentValidationAdapter : FluentValidationAdapter
	{
		public MultiRuleFluentValidationAdapter()
		{
			RuleFor(x => x)
				.NotEmpty()
				.WithMessage("Cannot be empty")
				.Must(x => !x.Contains('@'))
				.WithMessage("Cannot contain @ symbol")
				.Must(x => x.Length >= 5)
				.WithMessage("Must be at least 5 characters");
		}
	}

	[TestMethod]
	public void GetValidationErrors_MultipleRuleViolations_ReturnsAllErrors()
	{
		// Arrange
		MultiRuleFluentValidationAdapter adapter = new();
		TestSemanticString semanticString = new("a@"); // Violates length and @ symbol rules

		// Act
		List<string> errors = [.. adapter.GetValidationErrors(semanticString)];

		// Assert
		Assert.IsTrue(errors.Count >= 2);
		Assert.IsTrue(errors.Any(e => e.Contains("Cannot contain @ symbol")));
		Assert.IsTrue(errors.Any(e => e.Contains("Must be at least 5 characters")));
	}

	[TestMethod]
	public void ValidateSemanticString_WithComplexRules_WorksCorrectly()
	{
		// Arrange
		MultiRuleFluentValidationAdapter adapter = new();
		TestSemanticString validString = new("valid");
		TestSemanticString invalidString = new("bad@");

		// Act
		bool validResult = adapter.ValidateSemanticString(validString);
		bool invalidResult = adapter.ValidateSemanticString(invalidString);

		// Assert
		Assert.IsTrue(validResult);
		Assert.IsFalse(invalidResult);
	}
}
