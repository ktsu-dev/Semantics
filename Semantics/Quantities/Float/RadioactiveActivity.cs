// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a radioactive activity quantity with float precision.
/// </summary>
public sealed record RadioactiveActivity
{
	/// <summary>Gets the underlying generic radioactive activity instance.</summary>
	public Generic.RadioactiveActivity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="RadioactiveActivity"/> class.
	/// </summary>
	public RadioactiveActivity() { }

	/// <summary>
	/// Creates a new RadioactiveActivity from a value in becquerels.
	/// </summary>
	/// <param name="becquerels">The value in becquerels.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static new RadioactiveActivity FromBecquerels(float becquerels) => new() { Value = Generic.RadioactiveActivity<float>.FromBecquerels(becquerels) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
