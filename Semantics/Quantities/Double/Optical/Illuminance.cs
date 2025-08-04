// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an illuminance quantity with double precision.
/// </summary>
public sealed record Illuminance : Generic.Illuminance<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Illuminance"/> class.
	/// </summary>
	public Illuminance() : base() { }

	/// <summary>
	/// Creates a new Illuminance from a value in lux.
	/// </summary>
	/// <param name="lux">The value in lux.</param>
	/// <returns>A new Illuminance instance.</returns>
	public static new Illuminance FromLux(double lux) => new() { Quantity = lux };
}
