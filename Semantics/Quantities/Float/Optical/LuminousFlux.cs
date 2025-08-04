// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a luminous flux quantity with float precision.
/// </summary>
public sealed record LuminousFlux : Generic.LuminousFlux<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LuminousFlux"/> class.
	/// </summary>
	public LuminousFlux() : base() { }

	/// <summary>
	/// Creates a new LuminousFlux from a value in lumens.
	/// </summary>
	/// <param name="lumens">The value in lumens.</param>
	/// <returns>A new LuminousFlux instance.</returns>
	public static new LuminousFlux FromLumens(float lumens) => new() { Quantity = lumens };
}
