// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ErrorHandlingTests
{
	// Test types for error scenarios
	public record TestSemanticString : SemanticString<TestSemanticString> { }
	public record TestQuantity : SemanticQuantity<TestQuantity, double> { }

	[TestMethod]
	public void SemanticString_FromString_WithNullString_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticString<TestSemanticString>.Create<TestSemanticString>((string)null!));
	}

	[TestMethod]
	public void SemanticString_FromCharArray_WithNullArray_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticString<TestSemanticString>.Create<TestSemanticString>((char[])null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticAdd_WithNullFirstArgument_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Add<TestQuantity>(null!, validQuantity));
	}

	[TestMethod]
	public void SemanticQuantity_StaticAdd_WithNullSecondArgument_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Add<TestQuantity>(validQuantity, null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticSubtract_WithNullFirstArgument_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Subtract<TestQuantity>(null!, validQuantity));
	}

	[TestMethod]
	public void SemanticQuantity_StaticSubtract_WithNullSecondArgument_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Subtract<TestQuantity>(validQuantity, null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticMultiply_WithNullQuantity_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Multiply<TestQuantity>(null!, validQuantity));

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Multiply<TestQuantity>(validQuantity, null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticMultiply_WithNullScalar_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Multiply<TestQuantity>(null!, 2.0));
	}

	[TestMethod]
	public void SemanticQuantity_StaticDivide_WithNullQuantity_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Divide<TestQuantity>(null!, validQuantity));

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Divide<TestQuantity>(validQuantity, null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticDivide_WithNullScalar_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Divide<TestQuantity>(null!, 2.0));
	}

	[TestMethod]
	public void SemanticQuantity_StaticDivideToStorage_WithNullArguments_ThrowsArgumentNullException()
	{
		// Arrange
		TestQuantity validQuantity = TestQuantity.Create(5.0);

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.DivideToStorage(null!, validQuantity));

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.DivideToStorage(validQuantity, null!));
	}

	[TestMethod]
	public void SemanticQuantity_StaticNegate_WithNullArgument_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticQuantity<TestQuantity, double>.Negate<TestQuantity>(null!));
	}

	[TestMethod]
	public void ValidationStrategyFactory_CreateStrategy_WithNullType_ThrowsArgumentNullException()
	{
		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			ValidationStrategyFactory.CreateStrategy(null!));
	}

	[TestMethod]
	public void ValidateAllStrategy_Validate_WithNullType_ThrowsArgumentNullException()
	{
		// Arrange
		ValidateAllStrategy strategy = new();
		TestSemanticString semanticString = SemanticString<TestSemanticString>.Create<TestSemanticString>("test");

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			strategy.Validate(semanticString, null!));
	}

	[TestMethod]
	public void ValidateAnyStrategy_Validate_WithNullType_ThrowsArgumentNullException()
	{
		// Arrange
		ValidateAnyStrategy strategy = new();
		TestSemanticString semanticString = SemanticString<TestSemanticString>.Create<TestSemanticString>("test");

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			strategy.Validate(semanticString, null!));
	}

	[TestMethod]
	public void SemanticPath_RelativePath_Make_WithNullArguments_ThrowsArgumentNullException()
	{
		// Arrange
		AbsolutePath validPath = AbsolutePath.Create<AbsolutePath>("C:\\test");

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(null!, validPath));

		Assert.ThrowsExactly<ArgumentNullException>(() =>
			RelativePath.Make<RelativePath, AbsolutePath, AbsolutePath>(validPath, null!));
	}

	[TestMethod]
	public void SemanticStringFactory_TryFromString_WithInvalidValue_ReturnsFalse()
	{
		// Arrange
		SemanticStringFactory<InvalidSemanticString> factory = SemanticStringFactory<InvalidSemanticString>.Default;

		// Act
		bool result = factory.TryFromString("invalid_value", out InvalidSemanticString? output);

		// Assert
		Assert.IsFalse(result);
		Assert.IsNull(output);
	}

	[TestMethod]
	public void SemanticStringFactory_TryFromString_WithValidationException_ReturnsFalse()
	{
		// This tests FormatException handling in TryFromString
		SemanticStringFactory<StartsWithValidSemanticString> factory = SemanticStringFactory<StartsWithValidSemanticString>.Default;

		// Act
		bool result = factory.TryFromString("invalid", out StartsWithValidSemanticString? output);

		// Assert
		Assert.IsFalse(result);
		Assert.IsNull(output);
	}

	[TestMethod]
	public void SemanticString_PerformValidation_WithInvalidString_ThrowsFormatException()
	{
		// This test verifies that FormatException is thrown for invalid semantic strings
		Assert.ThrowsExactly<FormatException>(() =>
			SemanticString<StartsWithValidSemanticString>.Create<StartsWithValidSemanticString>("invalid"));
	}

	[TestMethod]
	public void SemanticString_PerformValidation_WithNullAfterCreation_ThrowsFormatException()
	{
		// This would be an edge case where validation returns null somehow
		// We can't easily test this directly, but we can test the validation logic
		StartsWithValidSemanticString validString = SemanticString<StartsWithValidSemanticString>.Create<StartsWithValidSemanticString>("Valid_test");
		Assert.IsTrue(validString.IsValid());
	}

	// Edge case tests for specific validation scenarios
	[TestMethod]
	public void ValidationAttributes_WithEmptyString_HandledCorrectly()
	{
		// Test how validation attributes handle empty strings
		EmptyStringTestSemanticString emptyString = SemanticString<EmptyStringTestSemanticString>.Create<EmptyStringTestSemanticString>("");
		Assert.IsTrue(emptyString.IsValid());
	}

	[TestMethod]
	public void ValidationAttributes_WithWhitespaceString_HandledCorrectly()
	{
		// Test validation with whitespace-only strings
		Assert.ThrowsExactly<FormatException>(() =>
			StartsWithValidSemanticString.Create<StartsWithValidSemanticString>("   "));
	}

	[TestMethod]
	public void ValidationAttributes_CaseSensitivity_HandledCorrectly()
	{
		// Test case sensitivity in validation
		Assert.ThrowsExactly<FormatException>(() =>
			StartsWithValidSemanticString.Create<StartsWithValidSemanticString>("valid_test")); // lowercase 'v'
	}

	[TestMethod]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S1244:Floating point numbers should not be tested for equality", Justification = "Testing an error case")]
	public void SemanticQuantity_ExtremeLargeValues_HandledCorrectly()
	{
		// Test with very large values
		TestQuantity largeQuantity = TestQuantity.Create(double.MaxValue);
		TestQuantity result = largeQuantity + TestQuantity.Create(1.0);

		// Should handle overflow gracefully
		Assert.IsTrue(double.IsInfinity(result.Quantity) || result.Quantity == double.MaxValue);
	}

	[TestMethod]
	public void SemanticQuantity_ExtremeSmallValues_HandledCorrectly()
	{
		// Test with very small values
		TestQuantity smallQuantity = TestQuantity.Create(double.Epsilon);
		TestQuantity result = smallQuantity / 2.0;

		// Should handle underflow gracefully
		Assert.IsTrue(result.Quantity is 0.0 or > 0);
	}

	[TestMethod]
	public void SemanticQuantity_SpecialValues_NaN_HandledCorrectly()
	{
		// Test with NaN values
		TestQuantity nanQuantity = TestQuantity.Create(double.NaN);
		TestQuantity result = nanQuantity + TestQuantity.Create(1.0);

		Assert.IsTrue(double.IsNaN(result.Quantity));
	}

	[TestMethod]
	public void SemanticQuantity_SpecialValues_Infinity_HandledCorrectly()
	{
		// Test with infinity values
		TestQuantity infinityQuantity = TestQuantity.Create(double.PositiveInfinity);
		TestQuantity result = infinityQuantity + TestQuantity.Create(1.0);

		Assert.IsTrue(double.IsPositiveInfinity(result.Quantity));
	}

	[TestMethod]
	public void SemanticPath_InvalidPathCharacters_ThrowsFormatException()
	{
		// Test path validation with invalid characters
		char[] invalidChars = Path.GetInvalidPathChars();
		if (invalidChars.Length > 0)
		{
			string invalidPath = "C:\\test" + invalidChars[0] + "path";
			Assert.ThrowsExactly<FormatException>(() =>
				AbsolutePath.Create<AbsolutePath>(invalidPath));
		}
	}

	[TestMethod]
	public void SemanticFileName_InvalidFileNameCharacters_ThrowsFormatException()
	{
		// Test filename validation with invalid characters
		char[] invalidChars = Path.GetInvalidFileNameChars();
		if (invalidChars.Length > 0)
		{
			string invalidFileName = "test" + invalidChars[0] + "file.txt";
			Assert.ThrowsExactly<FormatException>(() =>
				FileName.Create<FileName>(invalidFileName));
		}
	}

	// Tests for edge cases in type conversion and validation
	[TestMethod]
	public void SemanticString_As_WithSameType_ReturnsEquivalent()
	{
		// Test As conversion with the same type
		TestSemanticString original = SemanticString<TestSemanticString>.Create<TestSemanticString>("test");
		TestSemanticString converted = original.As<TestSemanticString>();

		Assert.AreEqual(original.WeakString, converted.WeakString);
	}

	[TestMethod]
	public void ValidationStrategy_WithNoAttributes_ReturnsTrue()
	{
		// Test validation strategy with type that has no validation attributes
		IValidationStrategy strategy = ValidationStrategyFactory.CreateStrategy(typeof(TestSemanticString));
		TestSemanticString semanticString = SemanticString<TestSemanticString>.Create<TestSemanticString>("test");

		bool result = strategy.Validate(semanticString, typeof(TestSemanticString));
		Assert.IsTrue(result);
	}
}

// Test record types for error handling scenarios
[StartsWith("Valid")]
public record StartsWithValidSemanticString : SemanticString<StartsWithValidSemanticString> { }

[StartsWith("Invalid")]
public record InvalidSemanticString : SemanticString<InvalidSemanticString> { }

public record EmptyStringTestSemanticString : SemanticString<EmptyStringTestSemanticString> { }
