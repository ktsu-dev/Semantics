// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a refractive index quantity with double precision.
/// </summary>
public sealed record RefractiveIndex
{
	/// <summary>Gets the underlying generic refractive index instance.</summary>
	public Generic.RefractiveIndex<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="RefractiveIndex"/> class.
	/// </summary>
	public RefractiveIndex() { }

	/// <summary>
	/// Creates a new RefractiveIndex from a dimensionless value.
	/// </summary>
	/// <param name="value">The refractive index value.</param>
	/// <returns>A new RefractiveIndex instance.</returns>
	public static RefractiveIndex FromValue(double value) => new() { Value = Generic.RefractiveIndex<double>.FromValue(value) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
