// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric charge quantity with float precision.
/// </summary>
public sealed record ElectricCharge : Generic.ElectricCharge<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCharge"/> class.
	/// </summary>
	public ElectricCharge() : base() { }

	/// <summary>
	/// Creates a new ElectricCharge from a value in coulombs.
	/// </summary>
	/// <param name="coulombs">The value in coulombs.</param>
	/// <returns>A new ElectricCharge instance.</returns>
	public static new ElectricCharge FromCoulombs(float coulombs) => new() { Value = coulombs };
}
