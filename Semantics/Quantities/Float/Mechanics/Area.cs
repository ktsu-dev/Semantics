// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an area quantity with float precision.
/// </summary>
public sealed record Area : Generic.Area<float>
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
	public static new Area FromSquareMeters(float squareMeters) => new() { Quantity = squareMeters };

}
