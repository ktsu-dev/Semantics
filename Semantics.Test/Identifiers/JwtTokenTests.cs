// Copyright (c) 2023-2026 ktsu-dev contributors

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

	[TestMethod]
	public void Create_HeaderNotUtf8_Throws()
	{
		// "_v8A" is base64url for the bytes FE FF 00; 0xFE is never valid in UTF-8.
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create("_v8A.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderIsJsonString_Throws()
	{
		// "ImEi" is base64url for the JSON string "a" — valid JSON, but not an object.
		Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create("ImEi.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig"));
	}

	[TestMethod]
	public void Create_HeaderWithMalformedObjectBody_Succeeds()
	{
		// "eyJhIjp9" is base64url for {"a":} — brace-delimited but not well-formed JSON.
		// Validation is deliberately structural and does not parse the body, so this is
		// accepted. See the remarks on IsJwtTokenAttribute.
		string malformed = "eyJhIjp9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig";
		JwtToken token = JwtToken.Create(malformed);
		Assert.AreEqual(malformed, token.WeakString);
	}
}
