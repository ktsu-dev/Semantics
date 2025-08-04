// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a radioactive activity quantity with double precision.
/// </summary>
public sealed record RadioactiveActivity : Generic.RadioactiveActivity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RadioactiveActivity"/> class.
	/// </summary>
	public RadioactiveActivity() : base() { }

	/// <summary>
	/// Creates a new RadioactiveActivity from a value in becquerels.
	/// </summary>
	/// <param name="becquerels">The value in becquerels.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static new RadioactiveActivity FromBecquerels(double becquerels) => new() { Quantity = becquerels };
}
