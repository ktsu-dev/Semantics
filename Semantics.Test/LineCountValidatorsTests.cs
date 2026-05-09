// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class LineCountValidatorsTests
{
	[HasExactLines(0)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Exact0 : SemanticString<Exact0> { }

	[HasExactLines(2)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Exact2 : SemanticString<Exact2> { }

	[HasMinimumLines(2)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Min2 : SemanticString<Min2> { }

	[HasMaximumLines(2)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Max2 : SemanticString<Max2> { }

	[TestMethod]
	public void HasExactLines_ZeroAndTwo_WithLfAndCrlf()
	{
		Exact0 empty = SemanticString<Exact0>.Create<Exact0>("");
		Assert.AreEqual("", empty.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Exact0>.Create<Exact0>("one line"));

		Exact2 lf = SemanticString<Exact2>.Create<Exact2>("line1\nline2");
		Assert.AreEqual("line1\nline2", lf.WeakString);
		Exact2 crlf = SemanticString<Exact2>.Create<Exact2>("line1\r\nline2");
		Assert.AreEqual("line1\r\nline2", crlf.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Exact2>.Create<Exact2>("only one"));
	}

	[TestMethod]
	public void HasMinimumLines_Two_WithLfAndCrlf()
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Min2>.Create<Min2>("one"));
		Min2 twoLf = SemanticString<Min2>.Create<Min2>("1\n2");
		Assert.AreEqual("1\n2", twoLf.WeakString);
		Min2 twoCrlf = SemanticString<Min2>.Create<Min2>("1\r\n2");
		Assert.AreEqual("1\r\n2", twoCrlf.WeakString);
		Min2 three = SemanticString<Min2>.Create<Min2>("1\n2\n3");
		Assert.AreEqual("1\n2\n3", three.WeakString);
	}

	[TestMethod]
	public void HasMaximumLines_Two_WithLfAndCrlf()
	{
		Max2 one = SemanticString<Max2>.Create<Max2>("one");
		Assert.AreEqual("one", one.WeakString);
		Max2 two = SemanticString<Max2>.Create<Max2>("1\n2");
		Assert.AreEqual("1\n2", two.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Max2>.Create<Max2>("1\n2\n3"));
		Max2 twoCrlf = SemanticString<Max2>.Create<Max2>("1\r\n2");
		Assert.AreEqual("1\r\n2", twoCrlf.WeakString);
	}
}
