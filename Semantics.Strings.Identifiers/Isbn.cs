// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Strings.Identifiers;

using ktsu.Semantics.Strings;

/// <summary>
/// An International Standard Book Number in ISBN-10 or ISBN-13 form, e.g. <c>0306406152</c> or
/// <c>9780306406157</c>. Hyphens and spaces are stripped and the value is uppercased (for the ISBN-10
/// <c>X</c> check digit) on creation. The check digit is validated; registration-group and publisher
/// ranges are not.
/// </summary>
[IsIsbn]
public sealed record Isbn : SemanticString<Isbn>
{
	/// <summary>Normalizes the input by stripping hyphens/spaces and uppercasing.</summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical ISBN string.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		return input.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
	}
}
