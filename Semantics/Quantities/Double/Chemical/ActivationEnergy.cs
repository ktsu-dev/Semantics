// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an activation energy quantity with double precision.
/// </summary>
public sealed record ActivationEnergy : Generic.ActivationEnergy<double>
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
	public static ActivationEnergy FromJoulesPerMole(double joulesPerMole) => new() { Value = joulesPerMole };
}
