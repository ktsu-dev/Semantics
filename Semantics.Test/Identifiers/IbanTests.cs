// Copyright (c) 2023-2026 ktsu-dev contributors

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
	[DataRow("GB98WEST12340000000012")]
	[DataRow("GB97WEST12340000000030")]
	[DataRow("GB02WEST00000000000029")]
	public void Create_CheckDigitsAtEdgeOfRange_Succeeds(string input)
	{
		Iban iban = Iban.Create(input);
		Assert.AreEqual(input, iban.WeakString);
	}

	[TestMethod]
	[DataRow("GB01WEST12340000000012")]
	[DataRow("GB00WEST12340000000030")]
	[DataRow("GB99WEST00000000000029")]
	public void Create_CheckDigitsNeverIssued_Throws(string input)
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(() => Iban.Create(input));
		Assert.Contains("check digits", ex.Message);
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

	[TestMethod]
	[DataRow("GB67WEST12345698765432\n")]
	[DataRow("GB67WEST12345698765432\r\n")]
	public void Create_InvalidChecksumWithTrailingNewline_Throws(string input)
	{
		Assert.ThrowsExactly<ArgumentException>(() => Iban.Create(input));
	}

	[TestMethod]
	[DataRow("GB82WEST12345698765432\n")]
	[DataRow("GB82WEST12345698765432\t")]
	[DataRow("\tGB82WEST12345698765432\r\n")]
	[DataRow("GB82 WEST 1234 5698 7654 32\n")]
	[DataRow("GB82\u00A0WEST\u00A012345698765432")]
	public void Create_ValidIbanWithWhitespace_StripsItAndValidates(string input)
	{
		Iban iban = Iban.Create(input);
		Assert.AreEqual("GB82WEST12345698765432", iban.WeakString);
	}
}
