// Copyright (c) 2023-2026 ktsu-dev contributors

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

	[HasExactLines(3)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Exact3 : SemanticString<Exact3> { }

	[HasMaximumLines(1)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Max1 : SemanticString<Max1> { }

	[IsMultiLine]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record MultiLine : SemanticString<MultiLine> { }

	[IsSingleLine]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record SingleLine : SemanticString<SingleLine> { }

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

	[TestMethod]
	[DataRow("a\rb")]
	[DataRow("a\nb")]
	[DataRow("a\r\nb")]
	[DataRow("a\u2028b")]
	[DataRow("a\u2029b")]
	public void EveryLineEndingCountsAsOneBreak(string twoLines)
	{
		Assert.AreEqual(twoLines, SemanticString<Exact2>.Create<Exact2>(twoLines).WeakString);
		Assert.AreEqual(twoLines, SemanticString<Min2>.Create<Min2>(twoLines).WeakString);
		Assert.AreEqual(twoLines, SemanticString<Max2>.Create<Max2>(twoLines).WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Max1>.Create<Max1>(twoLines));
	}

	[TestMethod]
	public void ConsecutiveCrlfPairsAreSeparateBreaks()
	{
		Exact3 three = SemanticString<Exact3>.Create<Exact3>("a\r\n\r\nb");
		Assert.AreEqual("a\r\n\r\nb", three.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Max2>.Create<Max2>("a\r\n\r\nb"));
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Exact3>.Create<Exact3>("a\n\rb\r\nc"));
	}

	[TestMethod]
	[DataRow("a\rb")]
	[DataRow("a\nb")]
	[DataRow("a\r\nb")]
	[DataRow("a\u2028b")]
	[DataRow("a\u2029b")]
	public void LineCountsAgreeWithIsMultiLineAndIsSingleLine(string twoLines)
	{
		Assert.AreEqual(twoLines, SemanticString<MultiLine>.Create<MultiLine>(twoLines).WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<SingleLine>.Create<SingleLine>(twoLines));
	}
}
