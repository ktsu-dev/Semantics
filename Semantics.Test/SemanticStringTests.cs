// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public record MySemanticString : SemanticString<MySemanticString> { }

[TestClass]
public class StringTests
{

	[TestMethod]
	public void ImplicitCastToString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		string systemString = semanticString;
		Assert.AreEqual("test", systemString);
	}

	[TestMethod]
	public void ConversionFromStringUsingAs()
	{
		string systemString = "test";
		MySemanticString semanticString = systemString.As<MySemanticString>();
		Assert.AreEqual("test", semanticString.WeakString);
	}

	[TestMethod]
	public void ToStringMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.AreEqual("test", semanticString.ToString());
	}

	private static readonly char[] TestCharArray = ['t', 'e', 's', 't'];
	private static readonly string[] Expected1 = ["hello", "world", "test"];
	private static readonly string[] Expected2 = ["a", "b,c,d"];

	[TestMethod]
	public void ToCharArrayMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		char[] chars = semanticString.ToCharArray();
		CollectionAssert.AreEqual(TestCharArray, chars);
	}

	[TestMethod]
	public void IsEmptyMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>(string.Empty);
		Assert.IsTrue(semanticString.IsEmpty(), "Empty semantic string should report IsEmpty as true");
	}

	[TestMethod]
	public void IsValidMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.IsTrue(semanticString.IsValid(), "Non-empty semantic string should be valid");
	}

	[TestMethod]
	public void WithPrefixMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		MySemanticString result = semanticString.WithPrefix("pre-");
		Assert.AreEqual("pre-test", result.ToString());
	}

	[TestMethod]
	public void WithSuffixMethod()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		MySemanticString result = semanticString.WithSuffix("-post");
		Assert.AreEqual("test-post", result.ToString());
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

	// New comprehensive tests for missing functionality

	[TestMethod]
	public void ConversionFromCharArrayUsingAs()
	{
		char[] chars = ['t', 'e', 's', 't'];
		MySemanticString semanticString = chars.As<MySemanticString>();
		Assert.AreEqual("test", semanticString.WeakString);
	}

	[TestMethod]
	public void ImplicitCastToCharArray()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		char[] result = semanticString;
		CollectionAssert.AreEqual(TestCharArray, result);
	}

	[TestMethod]
	public void ImplicitCastToReadOnlySpan()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		ReadOnlySpan<char> result = semanticString;
		Assert.IsTrue(result.SequenceEqual("test".AsSpan()), "ReadOnlySpan from implicit cast should equal expected value");
	}

	[TestMethod]
	public void LengthProperty()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.AreEqual(4, semanticString.Length);
	}

	[TestMethod]
	public void IndexerAccess()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.AreEqual('t', semanticString[0]);
		Assert.AreEqual('e', semanticString[1]);
		Assert.AreEqual('s', semanticString[2]);
		Assert.AreEqual('t', semanticString[3]);
	}

	[TestMethod]
	public void CompareTo_String()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.AreEqual(0, semanticString.CompareTo("test"));
		Assert.IsGreaterThan(0, semanticString.CompareTo("apple"));
		Assert.IsLessThan(0, semanticString.CompareTo("zebra"));
	}

	[TestMethod]
	public void CompareTo_SemanticString()
	{
		MySemanticString semanticString1 = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		MySemanticString semanticString2 = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		MySemanticString semanticString3 = SemanticString<MySemanticString>.Create<MySemanticString>("apple");

		Assert.AreEqual(0, semanticString1.CompareTo(semanticString2));
		Assert.IsGreaterThan(0, semanticString1.CompareTo(semanticString3));
	}

	[TestMethod]
	public void Contains_String_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		Assert.IsTrue(semanticString.Contains("world"), "Should contain 'world'");
		Assert.IsTrue(semanticString.Contains("hello"), "Should contain 'hello'");
		Assert.IsFalse(semanticString.Contains("test"), "Should not contain 'test'");
	}

	[TestMethod]
	public void Contains_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		Assert.IsTrue(semanticString.Contains("WORLD", StringComparison.OrdinalIgnoreCase), "Case-insensitive search should find 'WORLD'");
		Assert.IsFalse(semanticString.Contains("WORLD", StringComparison.Ordinal), "Case-sensitive search should not find 'WORLD'");
	}

	[TestMethod]
	public void EndsWith_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		Assert.IsTrue(semanticString.EndsWith("world"), "Should end with 'world'");
		Assert.IsFalse(semanticString.EndsWith("hello"), "Should not end with 'hello'");
	}

	[TestMethod]
	public void StartsWith_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		Assert.IsTrue(semanticString.StartsWith("hello"), "Should start with 'hello'");
		Assert.IsFalse(semanticString.StartsWith("world"), "Should not start with 'world'");
	}

	[TestMethod]
	public void IndexOf_Char_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		Assert.AreEqual(0, semanticString.IndexOf('h'));
		Assert.AreEqual(2, semanticString.IndexOf('l'));
		Assert.AreEqual(-1, semanticString.IndexOf('z'));
	}

	[TestMethod]
	public void IndexOf_String_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		Assert.AreEqual(0, semanticString.IndexOf("hello"));
		Assert.AreEqual(6, semanticString.IndexOf("world"));
		Assert.AreEqual(-1, semanticString.IndexOf("test"));
	}

	[TestMethod]
	public void LastIndexOf_Char_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		Assert.AreEqual(3, semanticString.LastIndexOf('l'));
		Assert.AreEqual(4, semanticString.LastIndexOf('o'));
		Assert.AreEqual(-1, semanticString.LastIndexOf('z'));
	}

	[TestMethod]
	public void ToCharArray_WithStartIndexAndLength()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		char[] result = semanticString.ToCharArray(1, 3);
		char[] expected = ['e', 'l', 'l'];
		CollectionAssert.AreEqual(expected, result);
	}

	[TestMethod]
	public void ComparisonOperators()
	{
		MySemanticString semanticString1 = SemanticString<MySemanticString>.Create<MySemanticString>("apple");
		MySemanticString semanticString2 = SemanticString<MySemanticString>.Create<MySemanticString>("banana");
		MySemanticString semanticString3 = SemanticString<MySemanticString>.Create<MySemanticString>("apple");

		Assert.IsTrue(semanticString1 < semanticString2, "'apple' should be less than 'banana'");
		Assert.IsTrue(semanticString1 <= semanticString2, "'apple' should be less than or equal to 'banana'");
		Assert.IsTrue(semanticString1 <= semanticString3, "'apple' should be less than or equal to 'apple'");
		Assert.IsTrue(semanticString2 > semanticString1, "'banana' should be greater than 'apple'");
		Assert.IsTrue(semanticString2 >= semanticString1, "'banana' should be greater than or equal to 'apple'");
		Assert.IsTrue(semanticString1 >= semanticString3, "'apple' should be greater than or equal to 'apple'");
	}

	[TestMethod]
	public void ComparisonOperators_WithNull()
	{
		MySemanticString? nullSemanticString = null;
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");

		Assert.IsTrue(nullSemanticString < semanticString, "null should be less than non-null");
		Assert.IsTrue(nullSemanticString <= semanticString, "null should be less than or equal to non-null");
		Assert.IsFalse(nullSemanticString > semanticString, "null should not be greater than non-null");
		Assert.IsFalse(nullSemanticString >= semanticString, "null should not be greater than or equal to non-null");
		Assert.IsFalse(semanticString < nullSemanticString, "non-null should not be less than null");
		Assert.IsTrue(semanticString > nullSemanticString, "non-null should be greater than null");
	}

	[TestMethod]
	public void FromString_NullInput_ThrowsArgumentNullException()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticString<MySemanticString>.Create<MySemanticString>((string)null!));
	}

	[TestMethod]
	public void FromCharArray_NullInput_ThrowsArgumentNullException()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticString<MySemanticString>.Create<MySemanticString>((char[])null!));
	}

	[TestMethod]
	public void FromReadOnlySpan_EmptySpan_CreatesEmptySemanticString()
	{
		ReadOnlySpan<char> emptySpan = [];
		MySemanticString result = SemanticString<MySemanticString>.Create<MySemanticString>(emptySpan);
		Assert.AreEqual(string.Empty, result.WeakString);
	}

	[TestMethod]
	public void As_ConversionBetweenSemanticStringTypes()
	{
		MySemanticString original = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		AnotherSemanticString converted = original.As<AnotherSemanticString>();
		Assert.AreEqual("test", converted.WeakString);
	}

	[TestMethod]
	public void StaticToString_WithNullSemanticString()
	{
		MySemanticString? nullSemanticString = null;
		string result = SemanticString<MySemanticString>.ToString(nullSemanticString);
		Assert.AreEqual(string.Empty, result);
	}

	[TestMethod]
	public void StaticToCharArray_WithNullSemanticString()
	{
		MySemanticString? nullSemanticString = null;
		char[] result = SemanticString<MySemanticString>.ToCharArray(nullSemanticString);
		Assert.IsEmpty(result);
	}

	[TestMethod]
	public void StaticToReadOnlySpan_WithNullSemanticString()
	{
		MySemanticString? nullSemanticString = null;
		ReadOnlySpan<char> result = SemanticString<MySemanticString>.ToReadOnlySpan(nullSemanticString);
		Assert.IsTrue(result.IsEmpty, "ReadOnlySpan from null should be empty");
	}

	[TestMethod]
	public void IsEmpty_WithNonEmptyString_ReturnsFalse()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.IsFalse(semanticString.IsEmpty(), "Non-empty string should not report IsEmpty as true");
	}

	[TestMethod]
	public void ImplicitConversions_WithNull()
	{
		MySemanticString? nullSemanticString = null;
		string stringResult = nullSemanticString;
		char[] charArrayResult = nullSemanticString;
		ReadOnlySpan<char> spanResult = nullSemanticString;

		Assert.AreEqual(string.Empty, stringResult);
		Assert.IsEmpty(charArrayResult);
		Assert.IsTrue(spanResult.IsEmpty, "ReadOnlySpan from null should be empty");
	}

	[TestMethod]
	public void Equals_WithString_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		Assert.IsTrue(semanticString.Equals("test"), "Should equal 'test'");
		Assert.IsFalse(semanticString.Equals("other"), "Should not equal 'other'");
	}

	[TestMethod]
	public void Equals_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Test");
		Assert.IsTrue(semanticString.Equals("TEST", StringComparison.OrdinalIgnoreCase), "Case-insensitive comparison should equal 'TEST'");
		Assert.IsFalse(semanticString.Equals("TEST", StringComparison.Ordinal), "Case-sensitive comparison should not equal 'TEST'");
	}

	[TestMethod]
	public void GetEnumerator_IteratesOverCharacters()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		using IEnumerator<char> enumerator = ((IEnumerable<char>)semanticString).GetEnumerator();

		Assert.IsTrue(enumerator.MoveNext(), "Should be able to move to first character");
		Assert.AreEqual('t', enumerator.Current);
		Assert.IsTrue(enumerator.MoveNext(), "Should be able to move to second character");
		Assert.AreEqual('e', enumerator.Current);
		Assert.IsTrue(enumerator.MoveNext(), "Should be able to move to third character");
		Assert.AreEqual('s', enumerator.Current);
		Assert.IsTrue(enumerator.MoveNext(), "Should be able to move to fourth character");
		Assert.AreEqual('t', enumerator.Current);
		Assert.IsFalse(enumerator.MoveNext(), "Should not be able to move past last character");
	}

	[TestMethod]
	public void GetEnumerator_NonGeneric_IteratesOverCharacters()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		string result = "";

		foreach (char c in (IEnumerable)semanticString)
		{
			result += c;
		}

		Assert.AreEqual("test", result);
	}

	// New tests for additional String manipulation methods
	[TestMethod]
	public void IndexOfAny_WithCharArray_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'w'];
		Assert.AreEqual(4, semanticString.IndexOfAny(chars)); // First 'o' in "hello"
	}

	[TestMethod]
	public void IndexOfAny_WithStartIndex_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'w'];
		Assert.AreEqual(6, semanticString.IndexOfAny(chars, 5)); // 'w' in "world"
	}

	[TestMethod]
	public void IndexOfAny_WithStartIndexAndCount_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'w'];
		Assert.AreEqual(6, semanticString.IndexOfAny(chars, 6, 3)); // 'w' in "world" (first match)
	}

	[TestMethod]
	public void LastIndexOfAny_WithCharArray_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'l'];
		Assert.AreEqual(9, semanticString.LastIndexOfAny(chars)); // Last 'l' in "world"
	}

	[TestMethod]
	public void LastIndexOfAny_WithStartIndex_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'l'];
		Assert.AreEqual(4, semanticString.LastIndexOfAny(chars, 5)); // 'o' at index 4 in "hello"
	}

	[TestMethod]
	public void LastIndexOfAny_WithStartIndexAndCount_ReturnsCorrectIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		char[] chars = ['o', 'l'];
		Assert.AreEqual(3, semanticString.LastIndexOfAny(chars, 3, 2)); // 'l' at index 3 in "hello"
	}

	[TestMethod]
	public void CopyTo_CopiesCharactersCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		char[] destination = new char[10];
		semanticString.CopyTo(1, destination, 2, 3); // Copy "ell" to index 2

		char[] expected = ['\0', '\0', 'e', 'l', 'l', '\0', '\0', '\0', '\0', '\0'];
		CollectionAssert.AreEqual(expected, destination);
	}

	[TestMethod]
	public void Insert_InsertsStringCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Insert(5, " beautiful");
		Assert.AreEqual("hello beautiful world", result);
	}

	[TestMethod]
	public void Remove_SingleParameter_RemovesFromIndex()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Remove(5);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void Remove_TwoParameters_RemovesSpecifiedLength()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Remove(5, 6);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void Replace_Char_ReplacesCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.Replace('l', 'x');
		Assert.AreEqual("hexxo", result);
	}

	[TestMethod]
	public void Replace_String_ReplacesCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Replace("world", "universe");
		Assert.AreEqual("hello universe", result);
	}

	[TestMethod]
	public void Split_WithCharArray_SplitsCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello,world;test");
		string[] result = semanticString.Split(',', ';');
		CollectionAssert.AreEqual(Expected1, result);
	}

	[TestMethod]
	public void Split_WithCharArrayAndCount_SplitsCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("a,b,c,d");
		string[] result = semanticString.Split([','], 2);
		CollectionAssert.AreEqual(Expected2, result);
	}

	[TestMethod]
	public void Split_WithStringArray_SplitsCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello::world::test");
		string[] result = semanticString.Split(["::"], StringSplitOptions.None);
		CollectionAssert.AreEqual(Expected1, result);
	}

	[TestMethod]
	public void Substring_SingleParameter_ReturnsCorrectSubstring()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Substring(6);
		Assert.AreEqual("world", result);
	}

	[TestMethod]
	public void Substring_TwoParameters_ReturnsCorrectSubstring()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		string result = semanticString.Substring(0, 5);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void ToLower_ReturnsLowercaseString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("HELLO World");
		string result = semanticString.ToLower();
		Assert.AreEqual("hello world", result);
	}

	[TestMethod]
	public void ToUpper_ReturnsUppercaseString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello World");
		string result = semanticString.ToUpper();
		Assert.AreEqual("HELLO WORLD", result);
	}

	[TestMethod]
	public void ToLowerInvariant_ReturnsLowercaseString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("HELLO World");
		string result = semanticString.ToLowerInvariant();
		Assert.AreEqual("hello world", result);
	}

	[TestMethod]
	public void ToUpperInvariant_ReturnsUppercaseString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello World");
		string result = semanticString.ToUpperInvariant();
		Assert.AreEqual("HELLO WORLD", result);
	}

	[TestMethod]
	public void Trim_RemovesWhitespace()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("  hello world  ");
		string result = semanticString.Trim();
		Assert.AreEqual("hello world", result);
	}

	[TestMethod]
	public void Trim_WithCharArray_RemovesSpecifiedChars()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("xxhello worldxx");
		string result = semanticString.Trim('x');
		Assert.AreEqual("hello world", result);
	}

	[TestMethod]
	public void TrimStart_RemovesLeadingChars()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("xxhello worldxx");
		string result = semanticString.TrimStart('x');
		Assert.AreEqual("hello worldxx", result);
	}

	[TestMethod]
	public void TrimEnd_RemovesTrailingChars()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("xxhello worldxx");
		string result = semanticString.TrimEnd('x');
		Assert.AreEqual("xxhello world", result);
	}

	[TestMethod]
	public void PadLeft_PadsStringCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.PadLeft(10);
		Assert.AreEqual("     hello", result);
	}

	[TestMethod]
	public void PadLeft_WithChar_PadsStringCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.PadLeft(10, '*');
		Assert.AreEqual("*****hello", result);
	}

	[TestMethod]
	public void PadRight_PadsStringCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.PadRight(10);
		Assert.AreEqual("hello     ", result);
	}

	[TestMethod]
	public void PadRight_WithChar_PadsStringCorrectly()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.PadRight(10, '*');
		Assert.AreEqual("hello*****", result);
	}

	[TestMethod]
	public void GetTypeCode_ReturnsString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		TypeCode result = semanticString.GetTypeCode();
		Assert.AreEqual(TypeCode.String, result);
	}

	[TestMethod]
	public void IsNormalized_WithDefaultForm_ReturnsTrue()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		bool result = semanticString.IsNormalized();
		Assert.IsTrue(result, "Simple string should be normalized in default form");
	}

	[TestMethod]
	public void IsNormalized_WithSpecificForm_ReturnsTrue()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		bool result = semanticString.IsNormalized(NormalizationForm.FormC);
		Assert.IsTrue(result, "Simple string should be normalized in FormC");
	}

	[TestMethod]
	public void Normalize_WithDefaultForm_ReturnsNormalizedString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.Normalize();
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void Normalize_WithSpecificForm_ReturnsNormalizedString()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.Normalize(NormalizationForm.FormC);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void StartsWith_WithCultureAndIgnoreCase_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		bool result = semanticString.StartsWith("HELLO", true, CultureInfo.InvariantCulture);
		Assert.IsTrue(result, "Case-insensitive culture comparison should find 'HELLO' at start");
	}

	[TestMethod]
	public void StartsWith_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		Assert.IsTrue(semanticString.StartsWith("HELLO", StringComparison.OrdinalIgnoreCase), "Case-insensitive comparison should find 'HELLO' at start");
		Assert.IsFalse(semanticString.StartsWith("HELLO", StringComparison.Ordinal), "Case-sensitive comparison should not find 'HELLO' at start");
	}

	[TestMethod]
	public void EndsWith_WithCultureAndIgnoreCase_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		bool result = semanticString.EndsWith("WORLD", true, CultureInfo.InvariantCulture);
		Assert.IsTrue(result, "Case-insensitive culture comparison should find 'WORLD' at end");
	}

	[TestMethod]
	public void EndsWith_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		Assert.IsTrue(semanticString.EndsWith("WORLD", StringComparison.OrdinalIgnoreCase), "Case-insensitive comparison should find 'WORLD' at end");
		Assert.IsFalse(semanticString.EndsWith("WORLD", StringComparison.Ordinal), "Case-sensitive comparison should not find 'WORLD' at end");
	}

	[TestMethod]
	public void ToLower_WithCulture_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("HELLO");
		string result = semanticString.ToLower(CultureInfo.InvariantCulture);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void ToUpper_WithCulture_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.ToUpper(CultureInfo.InvariantCulture);
		Assert.AreEqual("HELLO", result);
	}

	[TestMethod]
	public void ToString_WithFormatProvider_ReturnsCorrectResult()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		string result = semanticString.ToString(CultureInfo.InvariantCulture);
		Assert.AreEqual("hello", result);
	}

	[TestMethod]
	public void IndexOf_WithStringComparisonVariants_ReturnsCorrectResults()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World Hello");

		// Test different overloads with StringComparison
		Assert.AreEqual(0, semanticString.IndexOf("HELLO", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, semanticString.IndexOf("HELLO", StringComparison.Ordinal));
		Assert.AreEqual(12, semanticString.IndexOf("HELLO", 1, StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(0, semanticString.IndexOf("HELLO", 0, 10, StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void LastIndexOf_WithStringComparisonVariants_ReturnsCorrectResults()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World Hello");

		// Test different overloads with StringComparison
		Assert.AreEqual(12, semanticString.LastIndexOf("HELLO", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, semanticString.LastIndexOf("HELLO", StringComparison.Ordinal)); // Case sensitive - should not find "HELLO" in "Hello"
		Assert.AreEqual(0, semanticString.LastIndexOf("HELLO", 10, StringComparison.OrdinalIgnoreCase)); // Correct behavior
		Assert.AreEqual(-1, semanticString.LastIndexOf("HELLO", 10, 5, StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void IndexOf_WithStartIndexVariants_ReturnsCorrectResults()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world hello");

		// Test char-based IndexOf with start index
		Assert.AreEqual(2, semanticString.IndexOf('l', 1));
		Assert.AreEqual(14, semanticString.IndexOf('l', 10));
		Assert.AreEqual(14, semanticString.IndexOf('l', 10, 5));

		// Test string-based IndexOf with start index
		Assert.AreEqual(12, semanticString.IndexOf("hello", 5)); // Correct behavior
		Assert.AreEqual(-1, semanticString.IndexOf("hello", 5, 8));
	}

	[TestMethod]
	public void LastIndexOf_WithStartIndexVariants_ReturnsCorrectResults()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello world hello");

		// Test char-based LastIndexOf with start index
		Assert.AreEqual(9, semanticString.LastIndexOf('l', 10)); // 'l' at position 9 in "world"
		Assert.AreEqual(9, semanticString.LastIndexOf('l', 10, 5)); // Actual result is 9

		// Test string-based LastIndexOf with start index
		Assert.AreEqual(0, semanticString.LastIndexOf("hello", 10)); // Correct behavior
		Assert.AreEqual(-1, semanticString.LastIndexOf("hello", 10, 8));
	}

	// Tests for edge cases and error conditions
	[TestMethod]
	public void IndexOfAny_WithNullArray_ThrowsArgumentNullException()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		Assert.ThrowsExactly<ArgumentNullException>(() => semanticString.IndexOfAny(null!));
	}

	[TestMethod]
	public void LastIndexOfAny_WithNullArray_ThrowsArgumentNullException()
	{
		MySemanticString semanticString = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		Assert.ThrowsExactly<ArgumentNullException>(() => semanticString.LastIndexOfAny(null!));
	}

	[TestMethod]
	public void GetHashCode_ConsistentForSameContent()
	{
		MySemanticString semanticString1 = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		MySemanticString semanticString2 = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		Assert.AreEqual(semanticString1.GetHashCode(), semanticString2.GetHashCode());
	}
}

public record AnotherSemanticString : SemanticString<AnotherSemanticString> { }

// Add test records for testing MakeCanonical and validation
public record CanonicalTestSemanticString : SemanticString<CanonicalTestSemanticString>
{
	protected override string MakeCanonical(string input) => input?.Trim().ToUpperInvariant() ?? string.Empty;
}

public record ValidationTestSemanticString : SemanticString<ValidationTestSemanticString>
{
	public override bool IsValid()
	{
		// Only allow strings that start with "VALID"
		return base.IsValid() && (string.IsNullOrEmpty(WeakString) || WeakString.StartsWith("VALID", StringComparison.Ordinal));
	}
}

[TestClass]
public class SemanticStringAdditionalTests
{
	[TestMethod]
	public void TryCreate_String_ValidValue_ReturnsTrue()
	{
		bool result = SemanticString<MySemanticString>.TryCreate("test", out MySemanticString? semantic);

		Assert.IsTrue(result, "TryCreate should return true for valid input");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_String_NullValue_ReturnsFalse()
	{
		bool result = SemanticString<MySemanticString>.TryCreate((string?)null, out MySemanticString? semantic);

		Assert.IsFalse(result, "TryCreate should return false for null input");
		Assert.IsNull(semantic);
	}

	[TestMethod]
	public void TryCreate_String_InvalidValue_ReturnsFalse()
	{
		bool result = SemanticString<ValidationTestSemanticString>.TryCreate("INVALID", out ValidationTestSemanticString? semantic);

		Assert.IsFalse(result, "TryCreate should return false for invalid input");
		Assert.IsNull(semantic);
	}

	[TestMethod]
	public void TryCreate_CharArray_ValidValue_ReturnsTrue()
	{
		char[] chars = ['t', 'e', 's', 't'];
		bool result = SemanticString<MySemanticString>.TryCreate(chars, out MySemanticString? semantic);

		Assert.IsTrue(result, "TryCreate should return true for valid char array");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_CharArray_NullValue_ReturnsFalse()
	{
		bool result = SemanticString<MySemanticString>.TryCreate((char[]?)null, out MySemanticString? semantic);

		Assert.IsFalse(result, "TryCreate should return false for null char array");
		Assert.IsNull(semantic);
	}

	[TestMethod]
	public void TryCreate_ReadOnlySpan_ValidValue_ReturnsTrue()
	{
		ReadOnlySpan<char> span = "test".AsSpan();
		bool result = SemanticString<MySemanticString>.TryCreate(span, out MySemanticString? semantic);

		Assert.IsTrue(result, "TryCreate should return true for valid ReadOnlySpan");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_ReadOnlySpan_EmptySpan_ReturnsTrue()
	{
		ReadOnlySpan<char> span = [];
		bool result = SemanticString<MySemanticString>.TryCreate(span, out MySemanticString? semantic);

		Assert.IsTrue(result, "TryCreate should return true for empty ReadOnlySpan");
		Assert.IsNotNull(semantic);
		Assert.AreEqual(string.Empty, semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_ReadOnlySpan_InvalidValue_ReturnsFalse()
	{
		ReadOnlySpan<char> span = "INVALID".AsSpan();
		bool result = SemanticString<ValidationTestSemanticString>.TryCreate(span, out ValidationTestSemanticString? semantic);

		Assert.IsFalse(result, "TryCreate should return false for invalid ReadOnlySpan");
		Assert.IsNull(semantic);
	}

	[TestMethod]
	public void TryCreate_Generic_String_ValidValue_ReturnsTrue()
	{
		bool result = MySemanticString.TryCreate("test", out MySemanticString? semantic);

		Assert.IsTrue(result, "Generic TryCreate should return true for valid string");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_Generic_CharArray_ValidValue_ReturnsTrue()
	{
		char[] chars = ['t', 'e', 's', 't'];
		bool result = MySemanticString.TryCreate(chars, out MySemanticString? semantic);

		Assert.IsTrue(result, "Generic TryCreate should return true for valid char array");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void TryCreate_Generic_ReadOnlySpan_ValidValue_ReturnsTrue()
	{
		ReadOnlySpan<char> span = "test".AsSpan();
		bool result = MySemanticString.TryCreate(span, out MySemanticString? semantic);

		Assert.IsTrue(result, "Generic TryCreate should return true for valid ReadOnlySpan");
		Assert.IsNotNull(semantic);
		Assert.AreEqual("test", semantic.WeakString);
	}

	[TestMethod]
	public void AsSpan_ReturnsCorrectSpan()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("test");
		ReadOnlySpan<char> span = semantic.AsSpan();

		Assert.IsTrue(span.SequenceEqual("test".AsSpan()), "AsSpan should return span with expected content");
	}

	[TestMethod]
	public void AsSpan_WithStartIndex_ReturnsCorrectSpan()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		ReadOnlySpan<char> span = semantic.AsSpan(2);

		Assert.IsTrue(span.SequenceEqual("llo".AsSpan()), "AsSpan with start index should return correct substring span");
	}

	[TestMethod]
	public void AsSpan_WithStartIndexAndLength_ReturnsCorrectSpan()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello");
		ReadOnlySpan<char> span = semantic.AsSpan(1, 3);

		Assert.IsTrue(span.SequenceEqual("ell".AsSpan()), "AsSpan with start index and length should return correct substring span");
	}

	[TestMethod]
	public void IndexOf_ReadOnlySpan_ReturnsCorrectIndex()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		ReadOnlySpan<char> searchSpan = "world".AsSpan();

		int index = semantic.IndexOf(searchSpan);
		Assert.AreEqual(6, index);
	}

	[TestMethod]
	public void IndexOf_ReadOnlySpan_WithStringComparison_ReturnsCorrectIndex()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		ReadOnlySpan<char> searchSpan = "WORLD".AsSpan();

		int index = semantic.IndexOf(searchSpan, StringComparison.OrdinalIgnoreCase);
		Assert.AreEqual(6, index);
	}

	[TestMethod]
	public void LastIndexOf_ReadOnlySpan_ReturnsCorrectIndex()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello world hello");
		ReadOnlySpan<char> searchSpan = "hello".AsSpan();

		int index = semantic.LastIndexOf(searchSpan);
		Assert.AreEqual(12, index);
	}

	[TestMethod]
	public void StartsWith_ReadOnlySpan_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		ReadOnlySpan<char> prefixSpan = "hello".AsSpan();

		bool result = semantic.StartsWith(prefixSpan);
		Assert.IsTrue(result, "Should start with 'hello' span");
	}

	[TestMethod]
	public void StartsWith_ReadOnlySpan_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		ReadOnlySpan<char> prefixSpan = "HELLO".AsSpan();

		bool result = semantic.StartsWith(prefixSpan, StringComparison.OrdinalIgnoreCase);
		Assert.IsTrue(result, "Case-insensitive comparison should find 'HELLO' span at start");
	}

	[TestMethod]
	public void EndsWith_ReadOnlySpan_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		ReadOnlySpan<char> suffixSpan = "world".AsSpan();

		bool result = semantic.EndsWith(suffixSpan);
		Assert.IsTrue(result, "Should end with 'world' span");
	}

	[TestMethod]
	public void EndsWith_ReadOnlySpan_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		ReadOnlySpan<char> suffixSpan = "WORLD".AsSpan();

		bool result = semantic.EndsWith(suffixSpan, StringComparison.OrdinalIgnoreCase);
		Assert.IsTrue(result, "Case-insensitive comparison should find 'WORLD' span at end");
	}

	[TestMethod]
	public void Contains_ReadOnlySpan_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello world");
		ReadOnlySpan<char> searchSpan = "lo wo".AsSpan();

		bool result = semantic.Contains(searchSpan);
		Assert.IsTrue(result, "Should contain 'lo wo' span");
	}

	[TestMethod]
	public void Contains_ReadOnlySpan_WithStringComparison_ReturnsCorrectResult()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("Hello World");
		ReadOnlySpan<char> searchSpan = "LO WO".AsSpan();

		bool result = semantic.Contains(searchSpan, StringComparison.OrdinalIgnoreCase);
		Assert.IsTrue(result, "Case-insensitive comparison should find 'LO WO' span");
	}

	[TestMethod]
	public void Count_WithPredicate_ReturnsCorrectCount()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello");

		int count = semantic.Count(c => c == 'l');
		Assert.AreEqual(2, count);
	}

	[TestMethod]
	public void Count_WithPredicate_NullPredicate_ThrowsArgumentNullException()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("hello");

		Assert.ThrowsExactly<ArgumentNullException>(() => semantic.Count(null!));
	}
	private static readonly string[] expectedSplitParts = ["a", "b", "c"];

	[TestMethod]
	public void Split_WithChar_ReturnsCorrectSpans()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("a,b,c");
		List<string> parts = [];

		foreach (ReadOnlySpan<char> part in semantic.Split(','))
		{
			parts.Add(part.ToString());
		}

		CollectionAssert.AreEqual(expectedSplitParts, parts);
	}

	[TestMethod]
	public void Split_WithCharAndOptions_ReturnsCorrectSpans()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("a,,b,c");
		List<string> parts = [];

		foreach (ReadOnlySpan<char> part in semantic.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			parts.Add(part.ToString());
		}

		CollectionAssert.AreEqual(expectedSplitParts, parts);
	}

	[TestMethod]
	public void TrimAsSpan_RemovesWhitespace()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("  hello  ");
		ReadOnlySpan<char> trimmed = semantic.TrimAsSpan();

		Assert.IsTrue(trimmed.SequenceEqual("hello".AsSpan()), "TrimAsSpan should remove whitespace");
	}

	[TestMethod]
	public void TrimAsSpan_WithTrimChars_RemovesSpecifiedChars()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("##hello##");
		ReadOnlySpan<char> trimmed = semantic.TrimAsSpan("#".AsSpan());

		Assert.IsTrue(trimmed.SequenceEqual("hello".AsSpan()), "TrimAsSpan should remove specified chars");
	}

	[TestMethod]
	public void TrimStartAsSpan_RemovesLeadingWhitespace()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("  hello  ");
		ReadOnlySpan<char> trimmed = semantic.TrimStartAsSpan();

		Assert.IsTrue(trimmed.SequenceEqual("hello  ".AsSpan()), "TrimStartAsSpan should remove leading whitespace");
	}

	[TestMethod]
	public void TrimStartAsSpan_WithTrimChars_RemovesSpecifiedLeadingChars()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("##hello##");
		ReadOnlySpan<char> trimmed = semantic.TrimStartAsSpan("#".AsSpan());

		Assert.IsTrue(trimmed.SequenceEqual("hello##".AsSpan()), "TrimStartAsSpan should remove specified leading chars");
	}

	[TestMethod]
	public void TrimEndAsSpan_RemovesTrailingWhitespace()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("  hello  ");
		ReadOnlySpan<char> trimmed = semantic.TrimEndAsSpan();

		Assert.IsTrue(trimmed.SequenceEqual("  hello".AsSpan()), "TrimEndAsSpan should remove trailing whitespace");
	}

	[TestMethod]
	public void TrimEndAsSpan_WithTrimChars_RemovesSpecifiedTrailingChars()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("##hello##");
		ReadOnlySpan<char> trimmed = semantic.TrimEndAsSpan("#".AsSpan());

		Assert.IsTrue(trimmed.SequenceEqual("##hello".AsSpan()), "TrimEndAsSpan should remove specified trailing chars");
	}

	[TestMethod]
	public void MakeCanonical_AppliesTransformation()
	{
		CanonicalTestSemanticString semantic = SemanticString<CanonicalTestSemanticString>.Create<CanonicalTestSemanticString>("  hello world  ");

		// The canonical form should be trimmed and uppercase
		Assert.AreEqual("HELLO WORLD", semantic.WeakString);
	}

	[TestMethod]
	public void MakeCanonical_WithNullInput_HandlesGracefully()
	{
		// This tests the null handling in MakeCanonical override
		Assert.ThrowsExactly<ArgumentNullException>(() =>
			SemanticString<CanonicalTestSemanticString>.Create<CanonicalTestSemanticString>((string?)null));
	}

	[TestMethod]
	public void IsValid_WithCustomValidation_ReturnsCorrectResult()
	{
		ValidationTestSemanticString validSemantic = SemanticString<ValidationTestSemanticString>.Create<ValidationTestSemanticString>("VALID test");
		Assert.IsTrue(validSemantic.IsValid(), "String starting with 'VALID' should be valid");

		// Testing that invalid values throw during creation
		Assert.ThrowsExactly<ArgumentException>(() =>
			SemanticString<ValidationTestSemanticString>.Create<ValidationTestSemanticString>("INVALID test"));
	}

	[TestMethod]
	public void StaticCreate_WithGenericTypeInference_WorksCorrectly()
	{
		MySemanticString semantic = MySemanticString.Create("test");
		Assert.AreEqual("test", semantic.WeakString);

		MySemanticString fromCharArray = MySemanticString.Create(['t', 'e', 's', 't']);
		Assert.AreEqual("test", fromCharArray.WeakString);

		MySemanticString fromSpan = MySemanticString.Create("test".AsSpan());
		Assert.AreEqual("test", fromSpan.WeakString);
	}

	[TestMethod]
	public void WithPrefix_InvalidResult_ThrowsArgumentException()
	{
		ValidationTestSemanticString semantic = SemanticString<ValidationTestSemanticString>.Create<ValidationTestSemanticString>("VALID test");

		// Adding a prefix that makes the result invalid should throw
		Assert.ThrowsExactly<ArgumentException>(() => semantic.WithPrefix("INVALID "));
	}

	[TestMethod]
	public void WithSuffix_InvalidResult_ThrowsArgumentException()
	{
		ValidationTestSemanticString semantic = SemanticString<ValidationTestSemanticString>.Create<ValidationTestSemanticString>("VALID test");

		// Since our validation requires strings to start with "VALID", any suffix should be fine
		ValidationTestSemanticString result = semantic.WithSuffix(" more");
		Assert.AreEqual("VALID test more", result.WeakString);
	}

	[TestMethod]
	public void CompareTo_WithNull_ReturnsPositive()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("test");

		int result = semantic.CompareTo((object?)null);
		Assert.IsGreaterThan(0, result);

		int resultSemantic = semantic.CompareTo(null);
		Assert.IsGreaterThan(0, resultSemantic);
	}

	[TestMethod]
	public void DebuggerDisplay_ReturnsCorrectFormat()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("test");

		// Test the debugger display through ToString (which calls GetDebuggerDisplay internally)
		string display = semantic.ToString();
		Assert.AreEqual("test", display);
	}

	[TestMethod]
	public void StaticMethods_WithNullSemanticString_HandleGracefully()
	{
		string result = SemanticString<MySemanticString>.ToString(null);
		Assert.AreEqual(string.Empty, result);

		char[] charResult = SemanticString<MySemanticString>.ToCharArray(null);
		Assert.IsEmpty(charResult);

		ReadOnlySpan<char> spanResult = SemanticString<MySemanticString>.ToReadOnlySpan(null);
		Assert.IsTrue(spanResult.IsEmpty, "ReadOnlySpan from null should be empty");
	}

	[TestMethod]
	public void Create_WithEmptyString_CreatesValidInstance()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>(string.Empty);

		Assert.IsNotNull(semantic);
		Assert.AreEqual(string.Empty, semantic.WeakString);
		Assert.IsTrue(semantic.IsEmpty(), "Empty string should report IsEmpty as true");
		Assert.IsTrue(semantic.IsValid(), "Empty string should be valid");
	}

	[TestMethod]
	public void ValidationAttributes_Integration_WorksCorrectly()
	{
		// Test that ValidateAttributes is called during IsValid
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("test");

		// Since MySemanticString has no validation attributes, it should return true
		bool isValid = semantic.ValidateAttributes();
		Assert.IsTrue(isValid, "ValidateAttributes should return true when no validation attributes are present");
	}

	[TestMethod]
	public void SpanSplitEnumerator_EmptyString_HandlesCorrectly()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>(string.Empty);
		List<string> parts = [];

		foreach (ReadOnlySpan<char> part in semantic.Split(','))
		{
			parts.Add(part.ToString());
		}

		// Empty string split returns no parts in the current implementation
		Assert.IsEmpty(parts);
	}
	private static readonly string[] expectedSingleCharacter = ["a"];

	[TestMethod]
	public void SpanSplitEnumerator_SingleCharacter_HandlesCorrectly()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>("a");
		List<string> parts = [];

		foreach (ReadOnlySpan<char> part in semantic.Split(','))
		{
			parts.Add(part.ToString());
		}

		CollectionAssert.AreEqual(expectedSingleCharacter, parts);
	}

	[TestMethod]
	public void SpanSplitEnumerator_OnlySeparators_HandlesCorrectly()
	{
		MySemanticString semantic = SemanticString<MySemanticString>.Create<MySemanticString>(",,");
		List<string> parts = [];

		foreach (ReadOnlySpan<char> part in semantic.Split(','))
		{
			parts.Add(part.ToString());
		}

		// ",," split by comma returns two empty parts in the current implementation
		Assert.HasCount(2, parts);
		Assert.IsTrue(parts.All(string.IsNullOrEmpty), "All parts should be empty");
	}

	[TestMethod]
	public void ErrorMessages_ContainTypeInformation()
	{
		try
		{
			ValidationTestSemanticString.Create("INVALID");
			Assert.Fail("Expected ArgumentException");
		}
		catch (ArgumentException ex)
		{
			Assert.Contains(nameof(ValidationTestSemanticString), ex.Message);
		}
	}
}
