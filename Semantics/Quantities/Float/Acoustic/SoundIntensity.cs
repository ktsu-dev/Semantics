// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound intensity quantity with float precision.
/// </summary>
public sealed record SoundIntensity
{
	/// <summary>Gets the underlying generic sound intensity instance.</summary>
	public Generic.SoundIntensity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundIntensity"/> class.
	/// </summary>
	public SoundIntensity() { }

	/// <summary>
	/// Creates a new SoundIntensity from a value in watts per square meter.
	/// </summary>
	/// <param name="wattsPerSquareMeter">The value in watts per square meter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static SoundIntensity FromWattsPerSquareMeter(float wattsPerSquareMeter) => new() { Value = Generic.SoundIntensity<float>.FromWattsPerSquareMeter(wattsPerSquareMeter) };

	/// <summary>
	/// Creates a new SoundIntensity from a value in microwatts per square centimeter.
	/// </summary>
	/// <param name="microwattsPerSquareCentimeter">The value in microwatts per square centimeter.</param>
	/// <returns>A new SoundIntensity instance.</returns>
	public static SoundIntensity FromMicrowattsPerSquareCentimeter(float microwattsPerSquareCentimeter) => new() { Value = Generic.SoundIntensity<float>.FromMicrowattsPerSquareCentimeter(microwattsPerSquareCentimeter) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
