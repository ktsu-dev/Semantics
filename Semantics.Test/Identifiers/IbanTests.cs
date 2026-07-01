// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class IbanTests
{
	[TestMethod]
	public void Create_ValidIban_Succeeds()
	{
		Iban iban = Iban.Create("GB82WEST12345698765432");
		Assert.AreEqual("GB82WEST12345698765432", iban.WeakString);
	}

	[TestMethod]
	public void Create_WithSpacesAndLowercase_IsCanonicalized()
	{
		Iban iban = Iban.Create("gb82 west 1234 5698 7654 32");
		Assert.AreEqual("GB82WEST12345698765432", iban.WeakString);
	}

	[TestMethod]
	public void Create_BadChecksum_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Iban.Create("GB82WEST12345698765431"));
	}

	[TestMethod]
	public void Create_BadStructure_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Iban.Create("1234WEST12345698765432"));
	}

	[TestMethod]
	public void Create_TooShort_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Iban.Create("GB82WEST1234"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Iban.Create(string.Empty));
	}
}
