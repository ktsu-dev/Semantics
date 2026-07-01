// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// A universally unique identifier (UUID / GUID) in canonical RFC 4122 form: lowercase 8-4-4-4-12
/// hexadecimal, e.g. <c>123e4567-e89b-12d3-a456-426614174000</c>. Wrapping braces or parentheses and
/// uppercase hexadecimal are normalized away on creation. Any variant/version is accepted (including
/// the nil UUID); the value is not version-checked.
/// </summary>
[IsUuid]
public sealed record Uuid : SemanticString<Uuid>
{
	/// <summary>Normalizes the input by trimming wrapping braces/parentheses and lowercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical UUID string.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		return input.Trim().Trim('{', '}', '(', ')').ToLowerInvariant();
	}
}
