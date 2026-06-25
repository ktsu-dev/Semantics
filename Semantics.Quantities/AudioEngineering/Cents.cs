// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="Cents{T}"/>; the logarithmic core (including the
/// frequency-<see cref="Ratio{T}"/> conversions) is generated from <c>logarithmic.json</c>.
/// </summary>
public readonly partial record struct Cents<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the interval of zero cents (unison).</summary>
	public static Cents<T> Unison => new(T.Zero);

	/// <summary>
	/// Creates an interval from semitones (one semitone is 100 cents).
	/// </summary>
	/// <param name="semitones">The interval in semitones.</param>
	/// <returns>A new <see cref="Cents{T}"/>.</returns>
	public static Cents<T> FromSemitones(Semitones<T> semitones) => semitones.ToCents();

	/// <summary>
	/// Converts this interval to semitones (100 cents is one semitone).
	/// </summary>
	/// <returns>The equivalent <see cref="Semitones{T}"/>.</returns>
	public Semitones<T> ToSemitones() => new(Value / T.CreateChecked(100));
}
