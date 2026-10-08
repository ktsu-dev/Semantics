// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class IsbnTests
{
	[TestMethod]
	public void Create_ValidIsbn10_Succeeds()
	{
		Isbn isbn = Isbn.Create("0-306-40615-2");
		Assert.AreEqual("0306406152", isbn.WeakString);
	}

	[TestMethod]
	public void Create_ValidIsbn10WithXCheckDigit_Succeeds()
	{
		Isbn isbn = Isbn.Create("0-8044-2957-X");
		Assert.AreEqual("080442957X", isbn.WeakString);
	}

	[TestMethod]
	public void Create_ValidIsbn13_Succeeds()
	{
		Isbn isbn = Isbn.Create("978-0-306-40615-7");
		Assert.AreEqual("9780306406157", isbn.WeakString);
	}

	[TestMethod]
	public void Create_BadIsbn10CheckDigit_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Isbn.Create("0-306-40615-3"));
	}

	[TestMethod]
	public void Create_BadIsbn13CheckDigit_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Isbn.Create("978-0-306-40615-8"));
	}

	[TestMethod]
	public void Create_ValidIsbn13With979Prefix_Succeeds()
	{
		Isbn isbn = Isbn.Create("979-10-90636-07-1");
		Assert.AreEqual("9791090636071", isbn.WeakString);
	}

	[TestMethod]
	[DataRow("4006381333931")]
	[DataRow("5901234123457")]
	[DataRow("9770317847001")]
	public void Create_Ean13OutsideBooklandRange_Throws(string ean13)
	{
		Assert.ThrowsExactly<ArgumentException>(() => Isbn.Create(ean13));
	}

	[TestMethod]
	public void TryCreate_Ean13OutsideBooklandRange_ReturnsFalse()
	{
		bool created = Isbn.TryCreate("4006381333931", out Isbn? result);
		Assert.IsFalse(created);
		Assert.IsNull(result);
	}

	[TestMethod]
	public void Create_WrongLength_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Isbn.Create("123456789012"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Isbn.Create(string.Empty));
	}
}
