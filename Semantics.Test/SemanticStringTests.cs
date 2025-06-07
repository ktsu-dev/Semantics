// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

public record MySemanticString : SemanticString<MySemanticString> { }

[TestClass]
public class StringTests
{

	[TestMethod]
	public void ImplicitCastToString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		string systemString = semanticString;
		Assert.AreEqual("test", systemString);
	}

	[TestMethod]
	public void ExplicitCastFromString()
	{
		string systemString = "test";
		MySemanticString semanticString = (MySemanticString)systemString;
		Assert.AreEqual("test", semanticString.WeakString);
	}

	[TestMethod]
	public void ToStringMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		Assert.AreEqual("test", semanticString.ToString());
	}

	private static readonly char[] TestCharArray = ['t', 'e', 's', 't'];

	[TestMethod]
	public void ToCharArrayMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		char[] chars = semanticString.ToCharArray();
		CollectionAssert.AreEqual(TestCharArray, chars);
	}

	[TestMethod]
	public void IsEmptyMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>(string.Empty);
		Assert.IsTrue(semanticString.IsEmpty());
	}

	[TestMethod]
	public void IsValidMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		Assert.IsTrue(semanticString.IsValid());
	}

	[TestMethod]
	public void WithPrefixMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		MySemanticString result = semanticString.WithPrefix("pre-");
		Assert.AreEqual("pre-test", result.ToString());
	}

	[TestMethod]
	public void WithSuffixMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.FromString<MySemanticString>("test");
		MySemanticString result = semanticString.WithSuffix("-post");
		Assert.AreEqual("test-post", result.ToString());
	}
	[TestMethod]
	public void AsStringExtensionMethod()
	{
		string systemString = "test";
		MySemanticString semanticString = systemString.As<MySemanticString>();
		Assert.AreEqual("test", semanticString.WeakString);
	}

	[TestMethod]
	public void AsCharArrayExtensionMethod()
	{
		char[] charArray = ['t', 'e', 's', 't'];
		MySemanticString semanticString = charArray.As<MySemanticString>();
		CollectionAssert.AreEqual(charArray, semanticString.ToCharArray());
	}

	[TestMethod]
	public void AsReadOnlySpanExtensionMethod()
	{
		ReadOnlySpan<char> span = "test".AsSpan();
		MySemanticString semanticString = span.As<MySemanticString>();
		Assert.AreEqual("test", semanticString.WeakString);
	}
}
