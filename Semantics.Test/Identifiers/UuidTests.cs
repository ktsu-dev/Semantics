// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class UuidTests
{
	[TestMethod]
	public void Create_CanonicalUuid_Succeeds()
	{
		Uuid uuid = Uuid.Create("123e4567-e89b-12d3-a456-426614174000");
		Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_NilUuid_Succeeds()
	{
		Uuid uuid = Uuid.Create("00000000-0000-0000-0000-000000000000");
		Assert.AreEqual("00000000-0000-0000-0000-000000000000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_UppercaseWithBraces_IsCanonicalized()
	{
		Uuid uuid = Uuid.Create("{123E4567-E89B-12D3-A456-426614174000}");
		Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", uuid.WeakString);
	}

	[TestMethod]
	public void Create_MissingSegment_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Uuid.Create("123e4567-e89b-12d3-a456"));
	}

	[TestMethod]
	public void Create_NonHexCharacter_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Uuid.Create("123e4567-e89b-12d3-a456-42661417400g"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => Uuid.Create(string.Empty));
	}

	[TestMethod]
	public void TryCreate_Invalid_ReturnsFalse()
	{
		bool created = Uuid.TryCreate("not-a-uuid", out Uuid? result);
		Assert.IsFalse(created);
		Assert.IsNull(result);
	}

	[TestMethod]
	public void As_RoundTrip_PreservesValue()
	{
		Uuid uuid = Uuid.Create("123e4567-e89b-12d3-a456-426614174000");
		Uuid roundTripped = uuid.As<Uuid>();
		Assert.AreEqual(uuid, roundTripped);
		Assert.AreEqual(uuid.WeakString, roundTripped.WeakString);
	}
}
