// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class CreditCardNumberTests
{
	[TestMethod]
	public void Create_ValidLuhn_Succeeds()
	{
		CreditCardNumber card = CreditCardNumber.Create("4111111111111111");
		Assert.AreEqual("4111111111111111", card.WeakString);
	}

	[TestMethod]
	public void Create_WithSpacesAndHyphens_IsCanonicalized()
	{
		CreditCardNumber card = CreditCardNumber.Create("4111-1111 1111-1111");
		Assert.AreEqual("4111111111111111", card.WeakString);
	}

	[TestMethod]
	public void Create_FailingLuhn_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => CreditCardNumber.Create("4111111111111112"));
	}

	[TestMethod]
	public void Create_TooShort_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => CreditCardNumber.Create("411111111111"));
	}

	[TestMethod]
	public void Create_NonDigit_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => CreditCardNumber.Create("4111a11111111111"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => CreditCardNumber.Create(string.Empty));
	}
}
