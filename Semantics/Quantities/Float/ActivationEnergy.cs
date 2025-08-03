// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an activation energy quantity with float precision.
/// </summary>
public sealed record ActivationEnergy : Generic.ActivationEnergy<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ActivationEnergy"/> class.
	/// </summary>
	public ActivationEnergy() : base() { }

	/// <summary>
	/// Creates a new ActivationEnergy from a value in joules per mole.
	/// </summary>
	/// <param name="joulesPerMole">The value in joules per mole.</param>
	/// <returns>A new ActivationEnergy instance.</returns>
	public static new ActivationEnergy FromJoulesPerMole(float joulesPerMole) => new() { Value = joulesPerMole };
}
