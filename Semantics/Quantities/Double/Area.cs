// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an area quantity with double precision.
/// </summary>
public sealed record Area : Generic.Area<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Area"/> class.
	/// </summary>
	public Area() : base() { }

	/// <summary>
	/// Creates a new Area from a value in square meters.
	/// </summary>
	/// <param name="squareMeters">The value in square meters.</param>
	/// <returns>A new Area instance.</returns>
	public static new Area FromSquareMeters(double squareMeters) => new() { Value = squareMeters };
}
