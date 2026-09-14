// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Globalization;

/// <summary>
/// The value of one conversion factor in <c>conversions.json</c>: a decimal literal, or an exact
/// fraction of two decimal literals.
/// </summary>
/// <remarks>
/// <para>
/// A repeating ratio such as 5/9 has no finite decimal literal, so writing it as one rounds it before
/// any storage type sees it. Keeping the fraction lets each storage type divide at its own precision:
/// <see cref="double"/> gets the correctly rounded quotient, and <see cref="decimal"/> gets 28 digits
/// rather than the 17 a <see cref="double"/>-length literal carried.
/// </para>
/// <para>
/// Writing a factor as its exact definition can move its <see cref="double"/> constant to the adjacent
/// representable value, because a rounded 17-digit literal is not always the <see cref="double"/>
/// nearest the true value. 5.2.0 moved two constants this way: <c>PsiToPascals</c> from
/// 6894.757293168361 to 6894.757293168362, and <c>RevolutionPerMinuteToRadianPerSecond</c> from
/// 0.10471975511965977 to 0.10471975511965978. The public <c>IUnit.ToBaseFactor</c> of <c>Psi</c> and
/// <c>RevolutionPerMinute</c> moved with them.
/// </para>
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
	/// Every operand carries a <c>d</c> suffix. Without it a fraction such as <c>5 / 9</c> is integer
	/// division and evaluates to zero, and a literal with no point or exponent that is too large for
	/// <see cref="ulong"/>, such as <c>100000000000000000000</c>, is an integer literal the compiler
	/// rejects (CS1021). The suffix changes no value that compiled without it.
	/// </remarks>
	public string DoubleExpression => Denominator is null
		? $"{Numerator}d"
		: $"{Numerator}d / {Denominator}d";

	/// <summary>
	/// Gets the expression that parses the value into the storage type <c>T</c>.
	/// </summary>
	/// <remarks>
	/// The call answers <see langword="null"/> when <c>T</c> keeps converting the <see cref="double"/>
	/// constant, and the generated holder does that conversion at each read.
	/// </remarks>
	public string StorageExpression => Denominator is null
		? $"StorageLiteral.Parse<T>(\"{Numerator}\")"
		: $"StorageLiteral.Divide<T>(\"{Numerator}\", \"{Denominator}\")";

	/// <summary>
	/// Reads a factor value, accepting a decimal literal such as <c>"0.3048"</c> or <c>"1e-10"</c>, or a
	/// fraction of two such as <c>"5/9"</c>, when a <see cref="double"/> can hold it.
	/// </summary>
	/// <param name="text">The value as written in the metadata.</param>
	/// <returns>
	/// The value, or <see langword="null"/> when <paramref name="text"/> is neither form, the denominator
	/// is zero, or a literal or the quotient is beyond the range of <see cref="double"/> or is non-zero
	/// and rounds to zero in it.
	/// </returns>
	public static ConversionValue? Parse(string? text)
	{
		if (text is null)
		{
			return null;
		}

		int slash = text.IndexOf('/');
		if (slash < 0)
		{
			return IsDecimalLiteral(text) && TryReadDouble(text, out _)
				? new ConversionValue(text, null)
				: null;
		}

		string top = text.Substring(0, slash);
		string bottom = text.Substring(slash + 1);

		return IsDecimalLiteral(top)
			&& IsDecimalLiteral(bottom)
			&& !IsZero(bottom)
			&& TryReadDouble(top, out double dividend)
			&& TryReadDouble(bottom, out double divisor)
			&& IsHeldByDouble(dividend / divisor, IsZero(top))
				? new ConversionValue(top, bottom)
				: null;
	}

	/// <summary>
	/// Reads a decimal literal as a <see cref="double"/>, failing when the <see cref="double"/> cannot hold it.
	/// </summary>
	/// <param name="literal">A literal that <see cref="IsDecimalLiteral"/> accepted.</param>
	/// <param name="value">The value as a <see cref="double"/>.</param>
	/// <returns><see langword="true"/> when the literal is within the finite range of <see cref="double"/> and does not round a non-zero value to zero.</returns>
	/// <remarks>
	/// Parsing a literal beyond the range fails on .NET Framework and answers infinity on .NET, and the
	/// generator can run on either, so both outcomes are rejected.
	/// </remarks>
	private static bool TryReadDouble(string literal, out double value)
		=> double.TryParse(literal, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
			&& IsHeldByDouble(value, IsZero(literal));

	/// <summary>
	/// Reports whether a <see cref="double"/> faithfully holds a value: finite, and zero only when the value is zero.
	/// </summary>
	/// <param name="value">The value as a <see cref="double"/>.</param>
	/// <param name="isZero">Whether the exact value is zero.</param>
	/// <returns><see langword="true"/> when <paramref name="value"/> is usable as the constant.</returns>
	private static bool IsHeldByDouble(double value, bool isZero)
		=> !double.IsInfinity(value) && !double.IsNaN(value) && (isZero || value != 0d);

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
