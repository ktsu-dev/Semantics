// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a velocity quantity with double precision.
/// </summary>
public sealed record Velocity
{
	/// <summary>Gets the underlying generic velocity instance.</summary>
	public Generic.Velocity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity"/> class.
	/// </summary>
	public Velocity() { }

	/// <summary>
	/// Creates a new Velocity from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity FromMetersPerSecond(double metersPerSecond) => new() { Value = Generic.Velocity<double>.FromMetersPerSecond(metersPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
