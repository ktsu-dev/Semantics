// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a volume quantity with float precision.
/// </summary>
public sealed record Volume : Generic.Volume<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Volume"/> class.
	/// </summary>
	public Volume() : base() { }

	/// <summary>
	/// Creates a new Volume from a value in cubic meters.
	/// </summary>
	/// <param name="cubicMeters">The value in cubic meters.</param>
	/// <returns>A new Volume instance.</returns>
	public static new Volume FromCubicMeters(float cubicMeters) => new() { Quantity = cubicMeters };

}
