// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a luminous intensity quantity with double precision.
/// </summary>
public sealed record LuminousIntensity
{
	/// <summary>Gets the underlying generic luminous intensity instance.</summary>
	public Generic.LuminousIntensity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="LuminousIntensity"/> class.
	/// </summary>
	public LuminousIntensity() { }

	/// <summary>
	/// Creates a new LuminousIntensity from a value in candelas.
	/// </summary>
	/// <param name="candelas">The value in candelas.</param>
	/// <returns>A new LuminousIntensity instance.</returns>
	public static LuminousIntensity FromCandelas(double candelas) => new() { Value = Generic.LuminousIntensity<double>.FromCandelas(candelas) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
