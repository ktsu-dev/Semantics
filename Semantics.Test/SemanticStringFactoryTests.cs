// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SemanticStringFactoryTests
{
	public record TestSemanticString : SemanticString<TestSemanticString> { }

	[TestMethod]
	public void Default_ReturnsInstanceOfFactory()
	{
		// Act
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;

		// Assert
		Assert.IsNotNull(factory);
		Assert.IsInstanceOfType<SemanticStringFactory<TestSemanticString>>(factory);
	}

	[TestMethod]
	public void Default_ReturnsSameInstance()
	{
		// Act
		SemanticStringFactory<TestSemanticString> factory1 = SemanticStringFactory<TestSemanticString>.Default;
		SemanticStringFactory<TestSemanticString> factory2 = SemanticStringFactory<TestSemanticString>.Default;

		// Assert
		Assert.AreSame(factory1, factory2);
	}

	[TestMethod]
	public void FromString_ValidValue_ReturnsSemanticString()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;
		const string testValue = "test";

		// Act
		TestSemanticString result = factory.FromString(testValue);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(testValue, result.WeakString);
	}

	[TestMethod]
	public void FromString_NullValue_ThrowsArgumentNullException()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() => factory.FromString(null));
	}

	[TestMethod]
	public void FromCharArray_ValidValue_ReturnsSemanticString()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;
		char[] testValue = ['t', 'e', 's', 't'];

		// Act
		TestSemanticString result = factory.FromCharArray(testValue);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("test", result.WeakString);
	}

	[TestMethod]
	public void FromCharArray_NullValue_ThrowsArgumentNullException()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(() => factory.FromCharArray(null));
	}

	[TestMethod]
	public void FromReadOnlySpan_ValidValue_ReturnsSemanticString()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;
		ReadOnlySpan<char> testValue = "test".AsSpan();

		// Act
		TestSemanticString result = factory.FromReadOnlySpan(testValue);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("test", result.WeakString);
	}

	[TestMethod]
	public void TryFromString_ValidValue_ReturnsTrueAndSetsResult()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;
		const string testValue = "test";

		// Act
		bool success = factory.TryFromString(testValue, out TestSemanticString? result);

		// Assert
		Assert.IsTrue(success);
		Assert.IsNotNull(result);
		Assert.AreEqual(testValue, result.WeakString);
	}

	[TestMethod]
	public void TryFromString_NullValue_ReturnsFalseAndNullResult()
	{
		// Arrange
		SemanticStringFactory<TestSemanticString> factory = SemanticStringFactory<TestSemanticString>.Default;

		// Act
		bool success = factory.TryFromString(null, out TestSemanticString? result);

		// Assert
		Assert.IsFalse(success);
		Assert.IsNull(result);
	}

	[TestMethod]
	public void TryFromString_InvalidValue_ReturnsFalseAndNullResult()
	{
		// Arrange
		SemanticStringFactory<InvalidTestString> factory = SemanticStringFactory<InvalidTestString>.Default;
		const string invalidValue = "invalid"; // This won't pass validation

		// Act
		bool success = factory.TryFromString(invalidValue, out InvalidTestString? result);

		// Assert
		Assert.IsFalse(success);
		Assert.IsNull(result);
	}
}

[StartsWith("Valid")]
public record InvalidTestString : SemanticString<InvalidTestString> { }
