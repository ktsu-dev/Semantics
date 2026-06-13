// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="Semitones{T}"/>; the logarithmic core (including the
/// frequency-<see cref="Ratio{T}"/> conversions) is generated from <c>logarithmic.json</c>.
/// </summary>
/// <typeparam name="T">The floating-point storage type.</typeparam>
public readonly partial record struct Semitones<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the interval of zero semitones (unison).</summary>
	public static Semitones<T> Unison => new(T.Zero);

	/// <summary>Gets the interval of one octave (12 semitones).</summary>
	public static Semitones<T> Octave => new(T.CreateChecked(12));

	/// <summary>
	/// Creates an interval from cents (100 cents is one semitone).
	/// </summary>
	/// <param name="cents">The interval in cents.</param>
	/// <returns>A new <see cref="Semitones{T}"/>.</returns>
	public static Semitones<T> FromCents(Cents<T> cents) => cents.ToSemitones();

	/// <summary>
	/// Converts this interval to cents (one semitone is 100 cents).
	/// </summary>
	/// <returns>The equivalent <see cref="Cents{T}"/>.</returns>
	public Cents<T> ToCents() => new(Value * T.CreateChecked(100));
}
