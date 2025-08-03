// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a mass flow rate quantity with float precision.
/// </summary>
public sealed record MassFlowRate
{
	/// <summary>Gets the underlying generic mass flow rate instance.</summary>
	public Generic.MassFlowRate<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="MassFlowRate"/> class.
	/// </summary>
	public MassFlowRate() { }

	/// <summary>
	/// Creates a new MassFlowRate from a value in kilograms per second.
	/// </summary>
	/// <param name="kilogramsPerSecond">The value in kilograms per second.</param>
	/// <returns>A new MassFlowRate instance.</returns>
	public static MassFlowRate FromKilogramsPerSecond(float kilogramsPerSecond) => new() { Value = Generic.MassFlowRate<float>.FromKilogramsPerSecond(kilogramsPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
