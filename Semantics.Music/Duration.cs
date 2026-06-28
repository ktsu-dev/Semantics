// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

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
	public static Duration Create(int numerator, int denominator)
	{
		if (denominator == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(denominator), "Denominator must be non-zero.");
		}

		int sign = denominator < 0 ? -1 : 1;
		int n = numerator * sign;
		int d = denominator * sign;
		int g = Gcd(Math.Abs(n), d);
		g = g == 0 ? 1 : g;
		return new() { Numerator = n / g, Denominator = d / g };
	}

	/// <summary>Returns the sum of this duration and another.</summary>
	/// <param name="other">The duration to add.</param>
	/// <returns>The summed duration.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
	public Duration Add(Duration other)
	{
		Ensure.NotNull(other);
		return Create((Numerator * other.Denominator) + (other.Numerator * Denominator), Denominator * other.Denominator);
	}

	/// <summary>Returns this duration multiplied by an integer factor.</summary>
	/// <param name="factor">The integer factor.</param>
	/// <returns>The scaled duration.</returns>
	public Duration Multiply(int factor) => Create(Numerator * factor, Denominator);

	/// <summary>Returns this duration with a single augmentation dot (×3/2).</summary>
	/// <returns>The dotted duration.</returns>
	public Duration Dotted() => Create(Numerator * 3, Denominator * 2);

	private static int Gcd(int a, int b)
	{
		while (b != 0)
		{
			(a, b) = (b, a % b);
		}

		return a;
	}
}
