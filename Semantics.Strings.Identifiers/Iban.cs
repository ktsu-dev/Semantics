// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// An International Bank Account Number, e.g. <c>GB82WEST12345698765432</c>. Whitespace is stripped and
/// the value uppercased on creation. Validation covers the generic structure and the ISO 7064
/// mod-97-10 checksum; per-country BBAN length/format tables are not enforced.
/// </summary>
[IsIban]
public sealed record Iban : SemanticString<Iban>
{
	/// <summary>Normalizes the input by stripping spaces and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical IBAN string.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		return input.Replace(" ", string.Empty).ToUpperInvariant();
	}
}
