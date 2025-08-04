// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound intensity quantity with float precision.
/// </summary>
public sealed record SoundIntensity : Generic.SoundIntensity<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundIntensity"/> class.
	/// </summary>
	public SoundIntensity() : base() { }

	/// <summary>
	/// Creates a new SoundIntensity from a value in watts per square meter.
	/// </summary>
	/// <param name="wattsPerSquareMeter">The value in watts per square meter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static new SoundIntensity FromWattsPerSquareMeter(float wattsPerSquareMeter) => new() { Quantity = wattsPerSquareMeter };

	/// <summary>
	/// Creates a new SoundIntensity from a value in microwatts per square centimeter.
	/// </summary>
	/// <param name="microwattsPerSquareCentimeter">The value in microwatts per square centimeter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static new SoundIntensity FromMicrowattsPerSquareCentimeter(float microwattsPerSquareCentimeter) => new() { Quantity = microwattsPerSquareCentimeter };
}
