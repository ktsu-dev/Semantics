// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an electric current quantity with float precision.
/// </summary>
public sealed record ElectricCurrent : Generic.ElectricCurrent<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCurrent"/> class.
	/// </summary>
	public ElectricCurrent() : base() { }

	/// <summary>
	/// Creates a new ElectricCurrent from a value in amperes.
	/// </summary>
	/// <param name="amperes">The value in amperes.</param>
	/// <returns>A new ElectricCurrent instance.</returns>
	public static new ElectricCurrent FromAmperes(float amperes) => new() { Value = amperes };

}
