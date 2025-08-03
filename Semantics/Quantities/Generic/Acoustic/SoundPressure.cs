// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a sound pressure quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record SoundPressure<T> : PhysicalQuantity<SoundPressure<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of soundpressure [M L⁻¹ T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundPressure;

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPressure{T}"/> class.
	/// </summary>
	public SoundPressure() : base() { }

	/// <summary>
	/// Creates a new SoundPressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static SoundPressure<T> FromPascals(T pascals) => Create(pascals);

	/// <summary>
	/// Creates a new SoundPressure from a value in micropascals.
	/// </summary>
	/// <param name="micropascals">The value in micropascals.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static SoundPressure<T> FromMicropascals(T micropascals) => Create(micropascals / T.CreateChecked(1_000_000));

	/// <summary>
	/// Creates a new SoundPressure from a value in bars.
	/// </summary>
	/// <param name="bars">The value in bars.</param>
	/// <returns>A new SoundPressure instance.</returns>
	public static SoundPressure<T> FromBars(T bars) => Create(bars * T.CreateChecked(100_000));

	/// <summary>
	/// Squares sound pressure for intensity calculations.
	/// </summary>
	/// <returns>The squared sound pressure.</returns>
	public T Squared() => Value * Value;
}
