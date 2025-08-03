// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric current quantity with double precision.
/// </summary>
public sealed record ElectricCurrent
{
	/// <summary>Gets the underlying generic electric current instance.</summary>
	public Generic.ElectricCurrent<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCurrent"/> class.
	/// </summary>
	public ElectricCurrent() { }

	/// <summary>
	/// Creates a new ElectricCurrent from a value in amperes.
	/// </summary>
	/// <param name="amperes">The value in amperes.</param>
	/// <returns>A new ElectricCurrent instance.</returns>
	public static ElectricCurrent FromAmperes(double amperes) => new() { Value = Generic.ElectricCurrent<double>.FromAmperes(amperes) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
