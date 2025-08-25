// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Reflection;
using System.Text.RegularExpressions;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class RegexMatchAttributeTests
{
	[RegexMatch("[", RegexOptions.None)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record InvalidPatternString : SemanticString<InvalidPatternString> { }

	[RegexMatch("^[a-z]+$", RegexOptions.None)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record AlphaOnlyString : SemanticString<AlphaOnlyString> { }

	[RegexMatch("^abc$", RegexOptions.IgnoreCase)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record CaseInsensitiveString : SemanticString<CaseInsensitiveString> { }

	[RegexMatch("^line1$\n^line2$", RegexOptions.Multiline)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used via generic type references")]
	private sealed partial record MultilineAnchorsString : SemanticString<MultilineAnchorsString> { }

	[TestMethod]
	public void RegexMatchAttribute_InvalidPattern_ShouldThrow()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<InvalidPatternString>.Create<InvalidPatternString>("anything"));
	}

	[TestMethod]
	public void RegexMatchAttribute_EmptyString_ShouldPass()
	{
		AlphaOnlyString empty = SemanticString<AlphaOnlyString>.Create<AlphaOnlyString>(string.Empty);
		Assert.IsNotNull(empty);
		Assert.AreEqual(string.Empty, empty.WeakString);
	}

	[TestMethod]
	public void RegexMatchAttribute_IgnoreCase_ShouldPass()
	{
		CaseInsensitiveString value = SemanticString<CaseInsensitiveString>.Create<CaseInsensitiveString>("AbC");
		Assert.IsTrue(value.IsValid());
	}

	[TestMethod]
	public void RegexMatchAttribute_MultilineAnchors_ShouldPass()
	{
		string input = "line1\nline2";
		MultilineAnchorsString value = SemanticString<MultilineAnchorsString>.Create<MultilineAnchorsString>(input);
		Assert.AreEqual(input, value.WeakString);
	}

	[TestMethod]
	public void RegexMatchAttribute_Metadata_ShouldExposePatternAndOptions()
	{
		Type? alphaType = typeof(RegexMatchAttributeTests).GetNestedType("AlphaOnlyString", BindingFlags.NonPublic);
		Type? caseType = typeof(RegexMatchAttributeTests).GetNestedType("CaseInsensitiveString", BindingFlags.NonPublic);
		Assert.IsNotNull(alphaType);
		Assert.IsNotNull(caseType);

		RegexMatchAttribute alphaAttr = (RegexMatchAttribute)alphaType!.GetCustomAttributes(typeof(RegexMatchAttribute), true).Single();
		RegexMatchAttribute caseAttr = (RegexMatchAttribute)caseType!.GetCustomAttributes(typeof(RegexMatchAttribute), true).Single();

		Assert.AreEqual("^[a-z]+$", alphaAttr.Pattern);
		Assert.AreEqual(RegexOptions.None, alphaAttr.Options);
		Assert.AreEqual("^abc$", caseAttr.Pattern);
		Assert.AreEqual(RegexOptions.IgnoreCase, caseAttr.Options);
	}
}
