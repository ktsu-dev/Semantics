// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a luminous intensity quantity with float precision.
/// </summary>
public sealed record LuminousIntensity
{
	/// <summary>Gets the underlying generic luminous intensity instance.</summary>
	public Generic.LuminousIntensity<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="LuminousIntensity"/> class.
	/// </summary>
	public LuminousIntensity() { }

	/// <summary>
	/// Creates a new LuminousIntensity from a value in candelas.
	/// </summary>
	/// <param name="candelas">The value in candelas.</param>
	/// <returns>A new LuminousIntensity instance.</returns>
	public static new LuminousIntensity FromCandelas(float candelas) => new() { Value = Generic.LuminousIntensity<float>.FromCandelas(candelas) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
