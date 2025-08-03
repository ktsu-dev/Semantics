// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a momentum quantity with double precision.
/// </summary>
public sealed record Momentum : Generic.Momentum<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Momentum"/> class.
	/// </summary>
	public Momentum() : base() { }

	/// <summary>
	/// Creates a new Momentum from a value in kilogram-meters per second.
	/// </summary>
	/// <param name="kilogramMetersPerSecond">The value in kilogram-meters per second.</param>
	/// <returns>A new Momentum instance.</returns>
	public static new Momentum FromKilogramMetersPerSecond(double kilogramMetersPerSecond) => new() { Value = kilogramMetersPerSecond };
}
