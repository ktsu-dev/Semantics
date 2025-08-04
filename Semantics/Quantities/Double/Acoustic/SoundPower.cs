// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound power quantity with double precision.
/// </summary>
public sealed record SoundPower : Generic.SoundPower<double>
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
	public static new SoundPower FromWatts(double watts) => new() { Quantity = watts };

	/// <summary>
	/// Creates a new SoundPower from a value in milliwatts.
	/// </summary>
	/// <param name="milliwatts">The value in milliwatts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static new SoundPower FromMilliwatts(double milliwatts) => new() { Quantity = milliwatts };

	/// <summary>
	/// Creates a new SoundPower from a value in acoustic watts.
	/// </summary>
	/// <param name="acousticWatts">The value in acoustic watts.</param>
	/// <returns>A new SoundPower instance.</returns>
	public static new SoundPower FromAcousticWatts(double acousticWatts) => new() { Quantity = acousticWatts };
}
