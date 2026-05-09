// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System;
using ktsu.Semantics.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class FirstClassAndFormatValidatorsTests
{
#pragma warning disable CS0618 // Obsolete attributes used intentionally for coverage
	[IsDateTime]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record DateTimeString : SemanticString<DateTimeString> { }

	[IsDecimal]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record DecimalString : SemanticString<DecimalString> { }

	[IsDouble]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record DoubleString : SemanticString<DoubleString> { }

	[IsGuid]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record GuidString : SemanticString<GuidString> { }

	[IsInt32]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record Int32String : SemanticString<Int32String> { }

	[IsIpAddress]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record IpString : SemanticString<IpString> { }

	[IsTimeSpan]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record TimeSpanString : SemanticString<TimeSpanString> { }

	[IsUri]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record UriString : SemanticString<UriString> { }

	[IsVersion]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record VersionString : SemanticString<VersionString> { }
#pragma warning restore CS0618

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
	public void DateTime_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		DateTimeString ok = SemanticString<DateTimeString>.Create<DateTimeString>("2025-01-02T03:04:05Z");
		Assert.AreEqual("2025-01-02T03:04:05Z", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<DateTimeString>.Create<DateTimeString>("not-a-date"));
		DateTimeString empty = SemanticString<DateTimeString>.Create<DateTimeString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Decimal_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		DecimalString ok = SemanticString<DecimalString>.Create<DecimalString>("123.45");
		Assert.AreEqual("123.45", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<DecimalString>.Create<DecimalString>("123,45x"));
		DecimalString empty = SemanticString<DecimalString>.Create<DecimalString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Double_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		DoubleString ok = SemanticString<DoubleString>.Create<DoubleString>("1.23");
		Assert.AreEqual("1.23", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<DoubleString>.Create<DoubleString>("nanx"));
		DoubleString empty = SemanticString<DoubleString>.Create<DoubleString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Guid_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		string guid = System.Guid.NewGuid().ToString();
		GuidString ok = SemanticString<GuidString>.Create<GuidString>(guid);
		Assert.AreEqual(guid, ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<GuidString>.Create<GuidString>("not-a-guid"));
		GuidString empty = SemanticString<GuidString>.Create<GuidString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Int32_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		Int32String ok = SemanticString<Int32String>.Create<Int32String>("123");
		Assert.AreEqual("123", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<Int32String>.Create<Int32String>("12.3"));
		Int32String empty = SemanticString<Int32String>.Create<Int32String>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void IpAddress_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		IpString ok4 = SemanticString<IpString>.Create<IpString>("192.168.0.1");
		Assert.AreEqual("192.168.0.1", ok4.WeakString);
		IpString ok6 = SemanticString<IpString>.Create<IpString>("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
		Assert.AreEqual("2001:0db8:85a3:0000:0000:8a2e:0370:7334", ok6.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<IpString>.Create<IpString>("999.999.999.999"));
		IpString empty = SemanticString<IpString>.Create<IpString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void TimeSpan_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		TimeSpanString ok = SemanticString<TimeSpanString>.Create<TimeSpanString>("01:02:03");
		Assert.AreEqual("01:02:03", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<TimeSpanString>.Create<TimeSpanString>("1 day"));
		TimeSpanString empty = SemanticString<TimeSpanString>.Create<TimeSpanString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Uri_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		UriString ok = SemanticString<UriString>.Create<UriString>("https://example.com/path");
		Assert.AreEqual("https://example.com/path", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<UriString>.Create<UriString>("/relative"));
		UriString empty = SemanticString<UriString>.Create<UriString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

	[TestMethod]
	public void Version_Valid_Invalid_Empty()
	{
#pragma warning disable CS0618
		VersionString ok = SemanticString<VersionString>.Create<VersionString>("1.2.3.4");
		Assert.AreEqual("1.2.3.4", ok.WeakString);
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<VersionString>.Create<VersionString>("v1.2.3"));
		VersionString empty = SemanticString<VersionString>.Create<VersionString>("");
		Assert.AreEqual("", empty.WeakString);
#pragma warning restore CS0618
	}

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
