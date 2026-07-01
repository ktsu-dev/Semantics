// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A ULID (Universally Unique Lexicographically Sortable Identifier): 26 Crockford base32 characters,
/// e.g. <c>01ARZ3NDEKTSV4RRFFQ69G5FAV</c>. Input is uppercased on creation. Validation covers the
/// character set, length, and the first-character timestamp bound only; it does not decode the
/// embedded timestamp beyond that high bit.
/// </summary>
[IsUlid]
public sealed record Ulid : SemanticString<Ulid>
{
	/// <summary>Normalizes the input by trimming and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical ULID string.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		return input.Trim().ToUpperInvariant();
	}
}
