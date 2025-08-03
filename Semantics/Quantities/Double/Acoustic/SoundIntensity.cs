// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound intensity quantity with double precision.
/// </summary>
public sealed record SoundIntensity : Generic.SoundIntensity<double>
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
	public static new SoundIntensity FromWattsPerSquareMeter(double wattsPerSquareMeter) => new() { Value = wattsPerSquareMeter };

	/// <summary>
	/// Creates a new SoundIntensity from a value in microwatts per square centimeter.
	/// </summary>
	/// <param name="microwattsPerSquareCentimeter">The value in microwatts per square centimeter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static new SoundIntensity FromMicrowattsPerSquareCentimeter(double microwattsPerSquareCentimeter) => new() { Value = microwattsPerSquareCentimeter };
}
