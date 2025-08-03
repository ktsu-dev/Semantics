// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a mass flow rate quantity with double precision.
/// </summary>
public sealed record MassFlowRate : Generic.MassFlowRate<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MassFlowRate"/> class.
	/// </summary>
	public MassFlowRate() : base() { }

	/// <summary>
	/// Creates a new MassFlowRate from a value in kilograms per second.
	/// </summary>
	/// <param name="kilogramsPerSecond">The value in kilograms per second.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static new MassFlowRate FromKilogramsPerSecond(double kilogramsPerSecond) => new() { Value = kilogramsPerSecond };
}
