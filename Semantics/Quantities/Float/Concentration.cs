// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a concentration quantity with float precision.
/// </summary>
public sealed record Concentration : Generic.Concentration<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Concentration"/> class.
	/// </summary>
	public Concentration() : base() { }

	/// <summary>
	/// Creates a new Concentration from a value in molar (mol/L).
	/// </summary>
	/// <param name="molar">The value in molar.</param>
	/// <returns>A new Concentration instance.</returns>
	public static new Concentration FromMolar(float molar) => new() { Value = molar };
}
