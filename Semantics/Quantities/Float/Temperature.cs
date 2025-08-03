// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a temperature quantity with float precision.
/// </summary>
public sealed record Temperature : Generic.Temperature<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Temperature"/> class.
	/// </summary>
	public Temperature() : base() { }

	/// <summary>
	/// Creates a new Temperature from a value in Kelvin.
	/// </summary>
	/// <param name="kelvin">The value in Kelvin.</param>
	/// <returns>A new Temperature instance.</returns>
	public static new Temperature FromKelvin(float kelvin) => new() { Value = kelvin };


}
