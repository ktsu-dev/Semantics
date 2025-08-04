// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a luminance quantity with double precision.
/// </summary>
public sealed record Luminance : Generic.Luminance<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Luminance"/> class.
	/// </summary>
	public Luminance() : base() { }

	/// <summary>
	/// Creates a new Luminance from a value in candela per square meter.
	/// </summary>
	/// <param name="candelaPerSquareMeter">The value in candela per square meter.</param>
	/// <returns>A new Luminance instance.</returns>
	public static new Luminance FromCandelaPerSquareMeter(double candelaPerSquareMeter) => new() { Quantity = candelaPerSquareMeter };
}
