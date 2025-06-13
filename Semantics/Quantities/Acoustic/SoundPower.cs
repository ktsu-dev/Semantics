// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound power quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record SoundPower<T> : PhysicalQuantity<SoundPower<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundPower;

	/// <summary>
	/// Initializes a new instance of the SoundPower class.
	/// </summary>
	public SoundPower() : base() { }

	/// <summary>
	/// Creates a new SoundPower from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower<T> FromWatts(T watts) => Create(watts);

	/// <summary>
	/// Creates a new SoundPower from a value in milliwatts.
	/// </summary>
	/// <param name="milliwatts">The value in milliwatts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower<T> FromMilliwatts(T milliwatts) => Create(milliwatts / T.CreateChecked(1000));

	/// <summary>
	/// Creates a new SoundPower from a value in acoustic watts.
	/// </summary>
	/// <param name="acousticWatts">The value in acoustic watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static SoundPower<T> FromAcousticWatts(T acousticWatts) => Create(acousticWatts);

	/// <summary>
	/// Multiplies sound intensity by area to create sound power.
	/// </summary>
	/// <param name="soundIntensity">The sound intensity.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting sound power.</returns>
	public static SoundPower<T> Multiply(SoundIntensity<T> soundIntensity, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(soundIntensity);
		ArgumentNullException.ThrowIfNull(area);
		return Create(soundIntensity.Value * area.Value);
	}

	/// <summary>
	/// Calculates the sound power from sound intensity and area.
	/// </summary>
	/// <param name="soundIntensity">The sound intensity in watts per square meter.</param>
	/// <param name="area">The area in square meters.</param>
	/// <returns>The resulting sound power.</returns>
	public static SoundPower<T> FromSoundIntensityAndArea(SoundIntensity<T> soundIntensity, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(soundIntensity);
		ArgumentNullException.ThrowIfNull(area);

		return Create(soundIntensity.Value * area.Value);
	}

	/// <summary>
	/// Calculates sound intensity from sound power and area (I = P/A).
	/// </summary>
	/// <param name="power">The sound power.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting sound intensity.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static SoundIntensity<T> operator /(SoundPower<T> power, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(power);
		ArgumentNullException.ThrowIfNull(area);

		T intensityValue = power.Value / area.Value;

		return SoundIntensity<T>.Create(intensityValue);
	}
}
