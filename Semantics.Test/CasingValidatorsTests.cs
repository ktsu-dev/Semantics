// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class CasingValidatorsTests
{
	[IsUpperCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record UpperCaseString : SemanticString<UpperCaseString> { }

	[IsLowerCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record LowerCaseString : SemanticString<LowerCaseString> { }

	[IsTitleCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record TitleCaseString : SemanticString<TitleCaseString> { }

	[IsSentenceCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record SentenceCaseString : SemanticString<SentenceCaseString> { }

	[IsSnakeCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record SnakeCaseString : SemanticString<SnakeCaseString> { }

	[IsKebabCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record KebabCaseString : SemanticString<KebabCaseString> { }

	[IsMacroCase]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record MacroCaseString : SemanticString<MacroCaseString> { }

	[TestMethod]
	public void UpperCase_Valid_Invalid_Empty()
	{
		UpperCaseString valid = SemanticString<UpperCaseString>.Create<UpperCaseString>("HELLO WORLD 123!");
		Assert.AreEqual("HELLO WORLD 123!", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<UpperCaseString>.Create<UpperCaseString>("Hello"));
		UpperCaseString empty = SemanticString<UpperCaseString>.Create<UpperCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void LowerCase_Valid_Invalid_Empty()
	{
		LowerCaseString valid = SemanticString<LowerCaseString>.Create<LowerCaseString>("hello world 123!");
		Assert.AreEqual("hello world 123!", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<LowerCaseString>.Create<LowerCaseString>("helloWorld"));
		LowerCaseString empty = SemanticString<LowerCaseString>.Create<LowerCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void TitleCase_Valid_Invalid_Empty()
	{
		TitleCaseString valid = SemanticString<TitleCaseString>.Create<TitleCaseString>("Hello World");
		Assert.AreEqual("Hello World", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TitleCaseString>.Create<TitleCaseString>("hello world"));
		TitleCaseString empty = SemanticString<TitleCaseString>.Create<TitleCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void SentenceCase_Valid_Invalid_Empty()
	{
		SentenceCaseString valid = SemanticString<SentenceCaseString>.Create<SentenceCaseString>("Hello world");
		Assert.AreEqual("Hello world", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SentenceCaseString>.Create<SentenceCaseString>("Hello World"));
		SentenceCaseString empty = SemanticString<SentenceCaseString>.Create<SentenceCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void SnakeCase_Valid_Invalid_Empty()
	{
		SnakeCaseString valid = SemanticString<SnakeCaseString>.Create<SnakeCaseString>("hello_world_123");
		Assert.AreEqual("hello_world_123", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SnakeCaseString>.Create<SnakeCaseString>("_leading"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SnakeCaseString>.Create<SnakeCaseString>("trailing_"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SnakeCaseString>.Create<SnakeCaseString>("double__underscore"));
		SnakeCaseString empty = SemanticString<SnakeCaseString>.Create<SnakeCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void KebabCase_Valid_Invalid_Empty()
	{
		KebabCaseString valid = SemanticString<KebabCaseString>.Create<KebabCaseString>("hello-world-123");
		Assert.AreEqual("hello-world-123", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<KebabCaseString>.Create<KebabCaseString>("-leading"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<KebabCaseString>.Create<KebabCaseString>("trailing-"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<KebabCaseString>.Create<KebabCaseString>("double--hyphen"));
		KebabCaseString empty = SemanticString<KebabCaseString>.Create<KebabCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}

	[TestMethod]
	public void MacroCase_Valid_Invalid_Empty()
	{
		MacroCaseString valid = SemanticString<MacroCaseString>.Create<MacroCaseString>("HELLO_WORLD_123");
		Assert.AreEqual("HELLO_WORLD_123", valid.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<MacroCaseString>.Create<MacroCaseString>("_LEADING"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<MacroCaseString>.Create<MacroCaseString>("TRAILING_"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<MacroCaseString>.Create<MacroCaseString>("DOUBLE__UNDERSCORE"));
		MacroCaseString empty = SemanticString<MacroCaseString>.Create<MacroCaseString>("");
		Assert.AreEqual("", empty.WeakString);
	}
}
