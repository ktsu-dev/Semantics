// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound speed quantity with double precision.
/// </summary>
public sealed record SoundSpeed : Generic.SoundSpeed<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundSpeed"/> class.
	/// </summary>
	public SoundSpeed() : base() { }

	/// <summary>
	/// Creates a new SoundSpeed from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static new SoundSpeed FromMetersPerSecond(double metersPerSecond) => new() { Value = metersPerSecond };

	/// <summary>
	/// Creates a new SoundSpeed from a value in feet per second.
	/// </summary>
	/// <param name="feetPerSecond">The value in feet per second.</param>
	/// <returns>A new SoundSpeed instance.</returns>
	public static new SoundSpeed FromFeetPerSecond(double feetPerSecond) => new() { Value = feetPerSecond };
}
