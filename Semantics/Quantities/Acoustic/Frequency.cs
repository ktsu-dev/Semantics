// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a frequency quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Frequency<T> : PhysicalQuantity<Frequency<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Frequency;

	/// <summary>
	/// Initializes a new instance of the Frequency class.
	/// </summary>
	public Frequency() : base() { }

	/// <summary>
	/// Creates a new Frequency from a value in hertz.
	/// </summary>
	/// <param name="hertz">The value in hertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromHertz(T hertz) => Create(hertz);

	/// <summary>
	/// Creates a new Frequency from a value in kilohertz.
	/// </summary>
	/// <param name="kilohertz">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromKilohertz(T kilohertz) => Create(kilohertz * T.CreateChecked(1000));

	/// <summary>
	/// Creates a new Frequency from a value in megahertz.
	/// </summary>
	/// <param name="megahertz">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency<T> FromMegahertz(T megahertz) => Create(megahertz * T.CreateChecked(1_000_000));

	/// <summary>
	/// Divides one by time to create frequency.
	/// </summary>
	/// <param name="one">The value one.</param>
	/// <param name="time">The time period.</param>
	/// <returns>The resulting frequency.</returns>
	public static Frequency<T> Divide(T one, Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(time);
		return Create(one / time.Value);
	}

	/// <summary>
	/// Multiplies frequency by wavelength to get speed.
	/// </summary>
	/// <param name="frequency">The frequency.</param>
	/// <param name="wavelength">The wavelength.</param>
	/// <returns>The resulting speed.</returns>
	public static Velocity<T> Multiply(Frequency<T> frequency, Wavelength<T> wavelength)
	{
		ArgumentNullException.ThrowIfNull(frequency);
		ArgumentNullException.ThrowIfNull(wavelength);
		return Velocity<T>.Create(frequency.Value * wavelength.Value);
	}
}
