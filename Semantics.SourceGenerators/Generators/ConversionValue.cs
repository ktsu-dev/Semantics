// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

/// <summary>
/// The value of one conversion factor in <c>conversions.json</c>: a decimal literal, or an exact
/// fraction of two decimal literals.
/// </summary>
/// <remarks>
/// A repeating ratio such as 5/9 has no finite decimal literal, so writing it as one rounds it before
/// any storage type sees it. Keeping the fraction lets each storage type divide at its own precision:
/// <see cref="double"/> gets the correctly rounded quotient, and <see cref="decimal"/> gets 28 digits
/// rather than the 17 a <see cref="double"/>-length literal carried.
/// </remarks>
/// <param name="numerator">The literal, or the numerator of the fraction.</param>
/// <param name="denominator">The denominator of the fraction, or <see langword="null"/> for a plain literal.</param>
internal sealed class ConversionValue(string numerator, string? denominator)
{
	/// <summary>Gets the literal, or the numerator of the fraction.</summary>
	public string Numerator { get; } = numerator;

	/// <summary>Gets the denominator of the fraction, or <see langword="null"/> for a plain literal.</summary>
	public string? Denominator { get; } = denominator;

	/// <summary>
	/// Gets a C# constant expression of type <see cref="double"/> for the value.
	/// </summary>
	/// <remarks>
	/// A plain literal is written exactly as the metadata spells it, so the committed constants for
	/// existing factors do not change. Each operand of a fraction carries a <c>d</c> suffix, because
	/// <c>5 / 9</c> would be integer division and evaluate to zero.
	/// </remarks>
	public string DoubleExpression => Denominator is null
		? Numerator
		: $"{Numerator}d / {Denominator}d";

	/// <summary>
	/// Builds the expression that materialises the value into the storage type <c>T</c>.
	/// </summary>
	/// <param name="fallback">An expression for the same value as a <see cref="double"/>, for storage types that cannot parse it.</param>
	/// <returns>A call to <c>StorageLiteral.Parse</c> or <c>StorageLiteral.Divide</c>.</returns>
	public string StorageExpression(string fallback) => Denominator is null
		? $"StorageLiteral.Parse<T>(\"{Numerator}\", {fallback})"
		: $"StorageLiteral.Divide<T>(\"{Numerator}\", \"{Denominator}\", {fallback})";

	/// <summary>
	/// Reads a factor value, accepting a decimal literal such as <c>"0.3048"</c> or <c>"1e-10"</c>, or a
	/// fraction of two such as <c>"5/9"</c>.
	/// </summary>
	/// <param name="text">The value as written in the metadata.</param>
	/// <returns>The value, or <see langword="null"/> when <paramref name="text"/> is neither form or the denominator is zero.</returns>
	public static ConversionValue? Parse(string? text)
	{
		if (text is null)
		{
			return null;
		}

		int slash = text.IndexOf('/');
		if (slash < 0)
		{
			return IsDecimalLiteral(text) ? new ConversionValue(text, null) : null;
		}

		string top = text.Substring(0, slash);
		string bottom = text.Substring(slash + 1);

		return IsDecimalLiteral(top) && IsDecimalLiteral(bottom) && !IsZero(bottom)
			? new ConversionValue(top, bottom)
			: null;
	}

	/// <summary>
	/// Reports whether <paramref name="text"/> is a decimal literal that is valid both in C# and in
	/// <c>Parse</c> with <c>NumberStyles.Float</c>: an optional sign, digits with an optional
	/// fractional part, and an optional exponent.
	/// </summary>
	/// <param name="text">The candidate literal.</param>
	/// <returns><see langword="true"/> when <paramref name="text"/> has that shape.</returns>
	private static bool IsDecimalLiteral(string text)
	{
		int index = 0;
		if (index < text.Length && (text[index] == '+' || text[index] == '-'))
		{
			index++;
		}

		int integerDigits = CountDigits(text, ref index);
		int fractionDigits = 0;

		if (index < text.Length && text[index] == '.')
		{
			index++;
			fractionDigits = CountDigits(text, ref index);
			if (fractionDigits == 0)
			{
				return false;
			}
		}

		if (integerDigits + fractionDigits == 0)
		{
			return false;
		}

		if (index < text.Length && (text[index] == 'e' || text[index] == 'E'))
		{
			index++;
			if (index < text.Length && (text[index] == '+' || text[index] == '-'))
			{
				index++;
			}

			if (CountDigits(text, ref index) == 0)
			{
				return false;
			}
		}

		return index == text.Length;
	}

	/// <summary>
	/// Advances past a run of ASCII digits.
	/// </summary>
	/// <param name="text">The text being scanned.</param>
	/// <param name="index">The position to start at, moved to the first character that is not a digit.</param>
	/// <returns>The number of digits passed.</returns>
	private static int CountDigits(string text, ref int index)
	{
		int start = index;
		while (index < text.Length && text[index] >= '0' && text[index] <= '9')
		{
			index++;
		}

		return index - start;
	}

	/// <summary>
	/// Reports whether a decimal literal is zero, which is to say whether every digit of its significand is zero.
	/// </summary>
	/// <param name="literal">A literal that <see cref="IsDecimalLiteral"/> accepted.</param>
	/// <returns><see langword="true"/> when the literal denotes zero.</returns>
	private static bool IsZero(string literal)
	{
		foreach (char character in literal)
		{
			if (character is 'e' or 'E')
			{
				break;
			}

			if (character is >= '1' and <= '9')
			{
				return false;
			}
		}

		return true;
	}
}
