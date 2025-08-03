// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sound absorption coefficient quantity with double precision.
/// </summary>
public sealed record SoundAbsorption
{
	/// <summary>Gets the underlying generic sound absorption instance.</summary>
	public Generic.SoundAbsorption<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundAbsorption"/> class.
	/// </summary>
	public SoundAbsorption() { }

	/// <summary>
	/// Creates a new SoundAbsorption from a dimensionless coefficient (0-1).
	/// </summary>
	/// <param name="coefficient">The absorption coefficient (0-1).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static SoundAbsorption FromCoefficient(double coefficient) => new() { Value = Generic.SoundAbsorption<double>.FromCoefficient(coefficient) };

	/// <summary>
	/// Creates a new SoundAbsorption from a percentage value.
	/// </summary>
	/// <param name="percentage">The absorption percentage (0-100).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static SoundAbsorption FromPercentage(double percentage) => new() { Value = Generic.SoundAbsorption<double>.FromPercentage(percentage) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
