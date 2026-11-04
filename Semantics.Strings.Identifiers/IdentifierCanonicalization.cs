// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings.Identifiers;

using System.Linq;
using System.Text;

/// <summary>
/// Normalization shared by the identifier types, so that what counts as a separator cannot drift
/// between them.
/// </summary>
internal static class IdentifierCanonicalization
{
	/// <summary>
	/// Removes every whitespace character (spaces, tabs, line breaks, non-breaking spaces) and every
	/// hyphen, which is what a copied card number or ISBN typically carries around its digits.
	/// </summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The input with whitespace and hyphens removed.</returns>
	internal static string StripWhitespaceAndHyphens(string input)
	{
		StringBuilder canonical = new(input.Length);
		foreach (char c in input.Where(static ch => !char.IsWhiteSpace(ch) && ch != '-'))
		{
			canonical.Append(c);
		}

		return canonical.ToString();
	}
}
