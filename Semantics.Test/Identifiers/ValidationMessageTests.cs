// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Identifiers;

using ktsu.Semantics.Strings.Identifiers;

/// <summary>
/// Issue #333: a rejected sensitive value stays out of the exception message, and the message says
/// which rule the value broke.
/// </summary>
[TestClass]
public sealed class ValidationMessageTests
{
	[TestMethod]
	public void CreditCardNumber_Rejected_MessageOmitsValueAndGivesReason()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(() => CreditCardNumber.Create("4111 1111 1111 1112"));

		Assert.DoesNotContain("4111111111111112", ex.Message);
		Assert.Contains(nameof(CreditCardNumber), ex.Message);
		Assert.Contains("Luhn", ex.Message);
	}

	[TestMethod]
	public void JwtToken_Rejected_MessageOmitsValueAndGivesReason()
	{
		const string token = "WyJhIl0.eyJzdWIiOiIxMjM0NTY3ODkwIn0.sig";
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(() => JwtToken.Create(token));

		Assert.DoesNotContain("eyJzdWIiOiIxMjM0NTY3ODkwIn0", ex.Message);
		Assert.Contains("JWT header", ex.Message);
	}

	[TestMethod]
	public void Iban_Rejected_MessageOmitsValueAndGivesReason()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(() => Iban.Create("GB82 WEST 1234 5698 7654 33"));

		Assert.DoesNotContain("GB82WEST12345698765433", ex.Message);
		Assert.Contains("mod-97", ex.Message);
	}

	[TestMethod]
	public void Uuid_Rejected_MessageKeepsValueAndGivesReason()
	{
		ArgumentException ex = Assert.ThrowsExactly<ArgumentException>(() => Uuid.Create("not-a-uuid"));

		Assert.Contains("not-a-uuid", ex.Message);
		Assert.Contains(nameof(Uuid), ex.Message);
		Assert.Contains("RFC 4122", ex.Message);
	}
}
