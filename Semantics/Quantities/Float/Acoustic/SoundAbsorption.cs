// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound absorption coefficient quantity with float precision.
/// </summary>
public sealed record SoundAbsorption : Generic.SoundAbsorption<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundAbsorption"/> class.
	/// </summary>
	public SoundAbsorption() : base() { }

	/// <summary>
	/// Creates a new SoundAbsorption from a dimensionless coefficient (0-1).
	/// </summary>
	/// <param name="coefficient">The absorption coefficient (0-1).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static new SoundAbsorption FromCoefficient(float coefficient) => new() { Quantity = coefficient };

	/// <summary>
	/// Creates a new SoundAbsorption from a percentage value.
	/// </summary>
	/// <param name="percentage">The absorption percentage (0-100).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static new SoundAbsorption FromPercentage(float percentage) => new() { Quantity = percentage };
}
