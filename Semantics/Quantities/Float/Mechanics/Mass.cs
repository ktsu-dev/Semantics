// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a mass quantity with float precision.
/// </summary>
public sealed record Mass : Generic.Mass<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Mass"/> class.
	/// </summary>
	public Mass() : base() { }

	/// <summary>
	/// Creates a new Mass from a value in kilograms.
	/// </summary>
	/// <param name="kilograms">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static new Mass FromKilograms(float kilograms) => new() { Value = kilograms };

}
