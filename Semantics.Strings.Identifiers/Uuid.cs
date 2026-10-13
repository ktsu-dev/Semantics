// Copyright (c) 2023-2026 ktsu-dev contributors

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
	/// <summary>
	/// Normalizes the input by removing one matched pair of wrapping braces or parentheses and
	/// lowercasing. Unbalanced or repeated wrappers are left in place, so validation rejects them as
	/// <see cref="Guid.TryParse(string, out Guid)"/> does.
	/// </summary>
	/// <param name="input">The raw input string.</param>
	/// <returns>The canonical UUID string.</returns>
	protected override string MakeCanonical(string input)
	{
		Ensure.NotNull(input);
		string trimmed = input.Trim();
		if (trimmed.Length >= 2)
		{
			char first = trimmed[0];
			char last = trimmed[^1];
			if ((first == '{' && last == '}') || (first == '(' && last == ')'))
			{
				trimmed = trimmed[1..^1];
			}
		}

		return trimmed.ToLowerInvariant();
	}
}
