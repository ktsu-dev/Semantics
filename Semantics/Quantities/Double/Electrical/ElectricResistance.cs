// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an electric resistance quantity with double precision.
/// </summary>
public sealed record ElectricResistance
{
	/// <summary>Gets the underlying generic electric resistance instance.</summary>
	public Generic.ElectricResistance<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricResistance"/> class.
	/// </summary>
	public ElectricResistance() { }

	/// <summary>
	/// Creates a new ElectricResistance from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ElectricResistance instance.</returns>
	public static ElectricResistance FromOhms(double ohms) => new() { Value = Generic.ElectricResistance<double>.FromOhms(ohms) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
