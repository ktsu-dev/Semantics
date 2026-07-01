// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

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
}
