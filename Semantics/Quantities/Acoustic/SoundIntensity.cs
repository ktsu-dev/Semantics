// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound intensity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record SoundIntensity<T> : PhysicalQuantity<SoundIntensity<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundIntensity;

	/// <summary>
	/// Initializes a new instance of the SoundIntensity class.
	/// </summary>
	public SoundIntensity() : base() { }

	/// <summary>
	/// Creates a new SoundIntensity from a value in watts per square meter.
	/// </summary>
	/// <param name="wattsPerSquareMeter">The value in watts per square meter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static SoundIntensity<T> FromWattsPerSquareMeter(T wattsPerSquareMeter) => Create(wattsPerSquareMeter);

	/// <summary>
	/// Creates a new SoundIntensity from a value in microwatts per square centimeter.
	/// </summary>
	/// <param name="microwattsPerSquareCentimeter">The value in microwatts per square centimeter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static SoundIntensity<T> FromMicrowattsPerSquareCentimeter(T microwattsPerSquareCentimeter) =>
		Create(microwattsPerSquareCentimeter / T.CreateChecked(100));

	/// <summary>
	/// Divides sound power by area to create sound intensity.
	/// </summary>
	/// <param name="soundPower">The sound power.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting sound intensity.</returns>
	public static SoundIntensity<T> Divide(SoundPower<T> soundPower, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(soundPower);
		ArgumentNullException.ThrowIfNull(area);
		return Create(soundPower.Value / area.Value);
	}
}
