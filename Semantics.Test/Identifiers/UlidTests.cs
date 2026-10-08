// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings;
using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class UlidTests
{
	[TestMethod]
	public void Create_CanonicalUlid_Succeeds()
	{
		Ulid ulid = Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAV");
		Assert.AreEqual("01ARZ3NDEKTSV4RRFFQ69G5FAV", ulid.WeakString);
	}

	[TestMethod]
	public void Create_Lowercase_IsUppercased()
	{
		Ulid ulid = Ulid.Create("01arz3ndektsv4rrffq69g5fav");
		Assert.AreEqual("01ARZ3NDEKTSV4RRFFQ69G5FAV", ulid.WeakString);
	}

	[TestMethod]
	public void Create_WrongLength_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FA"));
	}

	[TestMethod]
	public void Create_ExcludedLetterI_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Ulid.Create("01ARZ3NDEKTSV4RRFFQ69G5FAI"));
	}

	[TestMethod]
	public void Create_TimestampOverflow_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Ulid.Create("81ARZ3NDEKTSV4RRFFQ69G5FAV"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Ulid.Create(string.Empty));
	}

	// Ulid trims its input, which hides the attribute's own anchoring; a type that applies [IsUlid]
	// without canonicalizing sees the raw value.
	[IsUlid]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used via generic type references")]
	private sealed partial record UntrimmedUlid : SemanticString<UntrimmedUlid> { }

	[TestMethod]
	[DataRow("01ARZ3NDEKTSV4RRFFQ69G5FAV\n")]
	[DataRow("01ARZ3NDEKTSV4RRFFQ69G5FAV\r\n")]
	public void IsUlid_OnUntrimmedType_RejectsTrailingNewline(string input)
	{
		Assert.ThrowsExactly<ArgumentException>(() => SemanticString<UntrimmedUlid>.Create<UntrimmedUlid>(input));
		Assert.AreEqual("01ARZ3NDEKTSV4RRFFQ69G5FAV", SemanticString<UntrimmedUlid>.Create<UntrimmedUlid>("01ARZ3NDEKTSV4RRFFQ69G5FAV").WeakString);
	}
}
