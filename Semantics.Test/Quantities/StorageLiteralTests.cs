// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// A storage type whose parse throws for <c>NumberStyles.Float</c> rather than returning
/// <see langword="false"/> falls back to converting the <see cref="double"/> constant.
/// </summary>
[TestClass]
public sealed class StorageLiteralTests
{
	[TestMethod]
	public void AParseThatThrowsNotSupportedFallsBackToTheDoubleFactor()
	{
		ParseThrowingNumber<NotSupportedException> foot = Length<ParseThrowingNumber<NotSupportedException>>.FromFoot(ParseThrowingNumber<NotSupportedException>.One).Value;
		ParseThrowingNumber<NotSupportedException> kilometer = Length<ParseThrowingNumber<NotSupportedException>>.FromKilometer(ParseThrowingNumber<NotSupportedException>.One).Value;

		Assert.AreEqual(new ParseThrowingNumber<NotSupportedException>(0.3048), foot);
		Assert.AreEqual(new ParseThrowingNumber<NotSupportedException>(1000d), kilometer);
	}

	[TestMethod]
	public void AParseThatThrowsArgumentExceptionFallsBackToTheDoubleFactor()
	{
		ParseThrowingNumber<ArgumentException> knot = Speed<ParseThrowingNumber<ArgumentException>>.FromKnot(ParseThrowingNumber<ArgumentException>.One).Value;

		Assert.AreEqual(new ParseThrowingNumber<ArgumentException>(1852d / 3600d), knot);
	}
}
