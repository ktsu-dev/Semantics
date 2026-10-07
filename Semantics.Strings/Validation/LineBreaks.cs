// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Strings;

using System.Globalization;

/// <summary>
/// The one definition of a line break that every line-related validation attribute shares, so that
/// <see cref="IsSingleLineAttribute"/>, <see cref="IsMultiLineAttribute"/> and the <c>Has*Lines</c>
/// attributes cannot disagree about how many lines a string has.
/// </summary>
internal static class LineBreaks
{
	/// <summary>
	/// Reports whether a character ends a line: a line feed, a carriage return, the Unicode line
	/// separator (U+2028) or the Unicode paragraph separator (U+2029).
	/// </summary>
	/// <param name="c">The character to check.</param>
	/// <returns><see langword="true"/> if <paramref name="c"/> is a line break.</returns>
	internal static bool IsLineBreak(char c) =>
		c is '\n' or '\r'
		|| char.GetUnicodeCategory(c) is UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator;

	/// <summary>
	/// Counts the lines in a non-empty string: one more than the number of line breaks, where
	/// <c>\r\n</c> is a single break and a lone <c>\r</c>, <c>\n</c>, U+2028 or U+2029 is one each.
	/// </summary>
	/// <param name="value">The string to count, which must not be empty.</param>
	/// <returns>The number of lines in <paramref name="value"/>.</returns>
	internal static int CountLines(string value)
	{
		int lines = 1;
		for (int i = 0; i < value.Length; i++)
		{
			if (!IsLineBreak(value[i]))
			{
				continue;
			}

			if (value[i] == '\r' && i + 1 < value.Length && value[i + 1] == '\n')
			{
				i++;
			}

			lines++;
		}

		return lines;
	}
}
