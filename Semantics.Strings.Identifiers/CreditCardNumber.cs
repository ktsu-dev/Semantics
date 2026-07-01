// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A payment card number (PAN): 13-19 digits passing the Luhn checksum. Spaces and hyphens are
/// stripped on creation. Validation is Luhn-only — no BIN/issuer/network detection and no PCI
/// guarantees; treat the value as sensitive and do not log it.
/// </summary>
[IsCreditCardNumber]
public sealed record CreditCardNumber : SemanticString<CreditCardNumber>
{
	/// <summary>Normalizes the input by stripping spaces and hyphens.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The digit-only canonical form.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		return input.Replace(" ", string.Empty).Replace("-", string.Empty);
	}
}
