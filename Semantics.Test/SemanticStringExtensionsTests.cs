// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for SemanticStringExtensions that provide fluent conversion methods.
/// </summary>
[TestClass]
public class SemanticStringExtensionsTests
{
	/// <summary>
	/// Test semantic string implementation for testing purposes.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Test class is instantiated by test framework")]
	private sealed record TestSemanticString : SemanticString<TestSemanticString>
	{
		public TestSemanticString() : this(string.Empty) { }
		public TestSemanticString(string value) : base() => WeakString = value;
	}

	[TestMethod]
	public void StringAs_ValidValue_CreatesSemanticString()
	{
		// Arrange
		string value = "test";

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("test", result.WeakString);
	}

	[TestMethod]
	public void StringAs_EmptyValue_CreatesSemanticString()
	{
		// Arrange
		string value = string.Empty;

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(string.Empty, result.WeakString);
	}

	[TestMethod]
	public void StringAs_NullValue_ThrowsArgumentNullException()
	{
		// Arrange
		string value = null!;

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(value.As<TestSemanticString>);
	}

	[TestMethod]
	public void CharArrayAs_ValidValue_CreatesSemanticString()
	{
		// Arrange
		char[] value = ['t', 'e', 's', 't'];

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("test", result.WeakString);
	}

	[TestMethod]
	public void CharArrayAs_EmptyArray_CreatesSemanticString()
	{
		// Arrange
		char[] value = [];

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(string.Empty, result.WeakString);
	}

	[TestMethod]
	public void CharArrayAs_NullValue_ThrowsArgumentNullException()
	{
		// Arrange
		char[] value = null!;

		// Act & Assert
		Assert.ThrowsExactly<ArgumentNullException>(value.As<TestSemanticString>);
	}

	[TestMethod]
	public void ReadOnlySpanAs_ValidValue_CreatesSemanticString()
	{
		// Arrange
		ReadOnlySpan<char> value = "test".AsSpan();

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("test", result.WeakString);
	}

	[TestMethod]
	public void ReadOnlySpanAs_EmptySpan_CreatesSemanticString()
	{
		// Arrange
		ReadOnlySpan<char> value = [];

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(string.Empty, result.WeakString);
	}

	[TestMethod]
	public void ReadOnlySpanAs_SubstringSpan_CreatesSemanticString()
	{
		// Arrange
		ReadOnlySpan<char> value = "hello world".AsSpan(6, 5); // "world"

		// Act
		TestSemanticString result = value.As<TestSemanticString>();

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("world", result.WeakString);
	}

	[TestMethod]
	public void ExtensionMethods_FluentUsage_WorksCorrectly()
	{
		// Arrange
		string stringValue = "string";
		char[] charArrayValue = ['c', 'h', 'a', 'r'];
		ReadOnlySpan<char> spanValue = "span".AsSpan();

		// Act
		TestSemanticString fromString = stringValue.As<TestSemanticString>();
		TestSemanticString fromCharArray = charArrayValue.As<TestSemanticString>();
		TestSemanticString fromSpan = spanValue.As<TestSemanticString>();

		// Assert
		Assert.AreEqual("string", fromString.WeakString);
		Assert.AreEqual("char", fromCharArray.WeakString);
		Assert.AreEqual("span", fromSpan.WeakString);
	}

	[TestMethod]
	public void ExtensionMethods_ChainedOperations_WorksCorrectly()
	{
		// Arrange & Act
		TestSemanticString result = "test"
			.As<TestSemanticString>()
			.WithPrefix("prefix_")
			.WithSuffix("_suffix");

		// Assert
		Assert.AreEqual("prefix_test_suffix", result.WeakString);
	}
}
