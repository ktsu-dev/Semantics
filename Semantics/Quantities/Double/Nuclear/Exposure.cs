// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an exposure quantity with double precision.
/// </summary>
public sealed record Exposure : Generic.Exposure<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Exposure"/> class.
	/// </summary>
	public Exposure() : base() { }

	/// <summary>
	/// Creates a new Exposure from a value in coulombs per kilogram.
	/// </summary>
	/// <param name="coulombsPerKilogram">The value in coulombs per kilogram.</param>
	/// <returns>A new Exposure instance.</returns>
	public static new Exposure FromCoulombsPerKilogram(double coulombsPerKilogram) => new() { Quantity = coulombsPerKilogram };
}
