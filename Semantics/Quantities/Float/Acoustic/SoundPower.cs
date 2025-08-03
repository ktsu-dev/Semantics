// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound power quantity with float precision.
/// </summary>
public sealed record SoundPower : Generic.SoundPower<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundPower"/> class.
	/// </summary>
	public SoundPower() : base() { }

	/// <summary>
	/// Creates a new SoundPower from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static new SoundPower FromWatts(float watts) => new() { Value = watts };

	/// <summary>
	/// Creates a new SoundPower from a value in milliwatts.
	/// </summary>
	/// <param name="milliwatts">The value in milliwatts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static new SoundPower FromMilliwatts(float milliwatts) => new() { Value = milliwatts };

	/// <summary>
	/// Creates a new SoundPower from a value in acoustic watts.
	/// </summary>
	/// <param name="acousticWatts">The value in acoustic watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static new SoundPower FromAcousticWatts(float acousticWatts) => new() { Value = acousticWatts };
}
