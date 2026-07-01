// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

[TestClass]
public sealed class JwtTokenTests
{
	private const string ValidJwt =
		"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
		"eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ." +
		"SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

	[TestMethod]
	public void Create_ValidJwt_Succeeds()
	{
		JwtToken token = JwtToken.Create(ValidJwt);
		Assert.AreEqual(ValidJwt, token.WeakString);
	}

	[TestMethod]
	public void Create_AlgNoneWithEmptySignature_Succeeds()
	{
		string algNone =
			"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
			"eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.";
		JwtToken token = JwtToken.Create(algNone);
		Assert.AreEqual(algNone, token.WeakString);
	}

	[TestMethod]
	public void Create_TwoSegments_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create("header.payload"));
	}

	[TestMethod]
	public void Create_EmptyHeader_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create(".eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderNotJsonObject_Throws()
	{
		// "WyJhIl0" is base64url for the JSON array ["a"], which is valid JSON but not an object.
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create("WyJhIl0.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderNotBase64Url_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create("not*base64.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_Empty_Throws()
	{
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create(string.Empty));
	}
}
