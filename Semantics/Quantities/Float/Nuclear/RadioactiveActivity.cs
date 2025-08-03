// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a radioactive activity quantity with float precision.
/// </summary>
public sealed record RadioactiveActivity : Generic.RadioactiveActivity<float>
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
	public static new RadioactiveActivity FromBecquerels(float becquerels) => new() { Value = becquerels };
}
