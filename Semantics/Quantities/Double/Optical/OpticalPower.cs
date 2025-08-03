// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an optical power quantity with double precision.
/// </summary>
public sealed record OpticalPower : Generic.OpticalPower<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OpticalPower"/> class.
	/// </summary>
	public OpticalPower() : base() { }

	/// <summary>
	/// Creates a new OpticalPower from a value in diopters.
	/// </summary>
	/// <param name="diopters">The value in diopters.</param>
	/// <returns>A new OpticalPower instance.</returns>
	public static new OpticalPower FromDiopters(double diopters) => new() { Value = diopters };
}
