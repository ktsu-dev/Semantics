// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test;

using System;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class FirstClassAndFormatValidatorsTests
{
	[HasNonWhitespaceContent]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record NonWhitespace : SemanticString<NonWhitespace> { }

	[IsEmptyOrWhitespace]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record EmptyOrWs : SemanticString<EmptyOrWs> { }

	[IsSingleLine]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record SingleLine : SemanticString<SingleLine> { }

	[IsMultiLine]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record MultiLine : SemanticString<MultiLine> { }

	[TestMethod]
	public void NonWhitespace_And_EmptyOrWhitespace()
	{
		NonWhitespace n1 = SemanticString<NonWhitespace>.Create<NonWhitespace>(" a ");
		Assert.AreEqual(" a ", n1.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<NonWhitespace>.Create<NonWhitespace>("   "));

		EmptyOrWs e1 = SemanticString<EmptyOrWs>.Create<EmptyOrWs>("   ");
		Assert.AreEqual("   ", e1.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<EmptyOrWs>.Create<EmptyOrWs>("x"));
	}

	[TestMethod]
	public void SingleLine_And_MultiLine()
	{
		SingleLine s1 = SemanticString<SingleLine>.Create<SingleLine>("one line");
		Assert.AreEqual("one line", s1.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SingleLine>.Create<SingleLine>("line1\nline2"));

		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<MultiLine>.Create<MultiLine>(""));
		MultiLine m1 = SemanticString<MultiLine>.Create<MultiLine>("line1\nline2");
		Assert.AreEqual("line1\nline2", m1.WeakString);
	}
}
