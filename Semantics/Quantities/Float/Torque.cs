// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a torque quantity with float precision.
/// </summary>
public sealed record Torque
{
	/// <summary>Gets the underlying generic torque instance.</summary>
	public Generic.Torque<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Torque"/> class.
	/// </summary>
	public Torque() { }

	/// <summary>
	/// Creates a new Torque from a value in newton-meters.
	/// </summary>
	/// <param name="newtonMeters">The value in newton-meters.</param>
	/// <returns>A new Torque instance.</returns>
	public static new Torque FromNewtonMeters(float newtonMeters) => new() { Value = Generic.Torque<float>.FromNewtonMeters(newtonMeters) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
