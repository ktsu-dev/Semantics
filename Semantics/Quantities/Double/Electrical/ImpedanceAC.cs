// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an AC impedance quantity with double precision.
/// </summary>
public sealed record ImpedanceAC : Generic.ImpedanceAC<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ImpedanceAC"/> class.
	/// </summary>
	public ImpedanceAC() : base() { }

	/// <summary>
	/// Creates a new ImpedanceAC from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ImpedanceAC instance.</returns>
	public static new ImpedanceAC FromOhms(double ohms) => new() { Quantity = ohms };
}
