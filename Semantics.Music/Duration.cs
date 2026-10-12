// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Globalization;

/// <summary>
/// A note duration expressed as an exact rational fraction of a whole note.
/// </summary>
public sealed record Duration
{
	/// <summary>Gets the reduced numerator.</summary>
	public int Numerator { get; private init; } = 1;

	/// <summary>Gets the reduced, positive denominator.</summary>
	public int Denominator { get; private init; } = 1;

	/// <summary>Gets the duration as a fraction of a whole note.</summary>
	public double AsWholeNotes => (double)Numerator / Denominator;

	/// <summary>A whole note (1/1).</summary>
	public static Duration Whole => Create(1, 1);

	/// <summary>A half note (1/2).</summary>
	public static Duration Half => Create(1, 2);

	/// <summary>A quarter note (1/4).</summary>
	public static Duration Quarter => Create(1, 4);

	/// <summary>An eighth note (1/8).</summary>
	public static Duration Eighth => Create(1, 8);

	/// <summary>A sixteenth note (1/16).</summary>
	public static Duration Sixteenth => Create(1, 16);

	/// <summary>Creates a reduced duration from a numerator and denominator.</summary>
	/// <param name="numerator">The numerator.</param>
	/// <param name="denominator">The denominator; must be non-zero.</param>
	/// <returns>A new, reduced duration with a positive denominator.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="denominator"/> is 0.</exception>
	/// <exception cref="OverflowException">Thrown when the reduced fraction does not fit in <see cref="int"/>, as for <c>int.MinValue/-1</c>.</exception>
	public static Duration Create(int numerator, int denominator)
	{
		if (denominator == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(denominator), "Denominator must be non-zero.");
		}

		return Reduce(numerator, denominator);
	}

	/// <summary>Returns the sum of this duration and another.</summary>
	/// <param name="other">The duration to add.</param>
	/// <returns>The summed duration.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
	/// <exception cref="OverflowException">Thrown when the reduced sum does not fit in <see cref="int"/>.</exception>
	public Duration Add(Duration other)
	{
		Ensure.NotNull(other);

		// Over the least common denominator, so a run of sums grows only as fast as the lcm of the
		// denominators seen, not their product. Every intermediate fits in long because each factor
		// is an int.
		long g = Gcd(Denominator, other.Denominator);
		long numerator = (Numerator * (other.Denominator / g)) + (other.Numerator * (Denominator / g));
		long denominator = Denominator / g * other.Denominator;
		return Reduce(numerator, denominator);
	}

	/// <summary>Returns this duration multiplied by an integer factor.</summary>
	/// <param name="factor">The integer factor.</param>
	/// <returns>The scaled duration.</returns>
	/// <exception cref="OverflowException">Thrown when the reduced product does not fit in <see cref="int"/>.</exception>
	public Duration Multiply(int factor) => Reduce((long)Numerator * factor, Denominator);

	/// <summary>Returns this duration with a single augmentation dot (×3/2).</summary>
	/// <returns>The dotted duration.</returns>
	/// <exception cref="OverflowException">Thrown when the reduced result does not fit in <see cref="int"/>.</exception>
	public Duration Dotted() => Reduce((long)Numerator * 3, (long)Denominator * 2);

	/// <summary>Parses a fraction "n/d".</summary>
	/// <param name="text">The fraction text.</param>
	/// <returns>The reduced duration.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is not a valid non-zero-denominator fraction.</exception>
	public static Duration Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Duration? result)
			? result
			: throw new FormatException($"Invalid duration '{text}'.");
	}

	/// <summary>Tries to parse a fraction "n/d".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The reduced duration, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Duration? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int slash = text.IndexOf('/');
		if (slash <= 0 || slash == text.Length - 1)
		{
			return false;
		}

		if (!int.TryParse(text[..slash], NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int numerator)
			|| !int.TryParse(text[(slash + 1)..], NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int denominator)
			|| denominator == 0)
		{
			return false;
		}

		result = Create(numerator, denominator);
		return true;
	}

	/// <summary>Returns the reduced fraction (e.g. "1/4").</summary>
	/// <returns>The canonical duration text.</returns>
	public override string ToString() =>
		$"{Numerator.ToString(CultureInfo.InvariantCulture)}/{Denominator.ToString(CultureInfo.InvariantCulture)}";

	/// <summary>
	/// Reduces a fraction computed in <see cref="long"/> and narrows it to <see cref="int"/>, throwing
	/// rather than wrapping when it does not fit.
	/// </summary>
	private static Duration Reduce(long numerator, long denominator)
	{
		long sign = denominator < 0 ? -1 : 1;
		long n = numerator * sign;
		long d = denominator * sign;
		long g = Gcd(Math.Abs(n), d);
		g = g == 0 ? 1 : g;
		return new() { Numerator = checked((int)(n / g)), Denominator = checked((int)(d / g)) };
	}

	private static long Gcd(long a, long b)
	{
		while (b != 0)
		{
			(a, b) = (b, a % b);
		}

		return a;
	}
}
