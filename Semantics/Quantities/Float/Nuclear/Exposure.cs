// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an exposure quantity with float precision.
/// </summary>
public sealed record Exposure
{
	/// <summary>Gets the underlying generic exposure instance.</summary>
	public Generic.Exposure<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Exposure"/> class.
	/// </summary>
	public Exposure() { }

	/// <summary>
	/// Creates a new Exposure from a value in coulombs per kilogram.
	/// </summary>
	/// <param name="coulombsPerKilogram">The value in coulombs per kilogram.</param>
	/// <returns>A new Exposure instance.</returns>
	public static Exposure FromCoulombsPerKilogram(float coulombsPerKilogram) => new() { Value = Generic.Exposure<float>.FromCoulombsPerKilogram(coulombsPerKilogram) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
