// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a luminous flux quantity with double precision.
/// </summary>
public sealed record LuminousFlux
{
	/// <summary>Gets the underlying generic luminous flux instance.</summary>
	public Generic.LuminousFlux<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="LuminousFlux"/> class.
	/// </summary>
	public LuminousFlux() { }

	/// <summary>
	/// Creates a new LuminousFlux from a value in lumens.
	/// </summary>
	/// <param name="lumens">The value in lumens.</param>
	/// <returns>A new LuminousFlux instance.</returns>
	public static LuminousFlux FromLumens(double lumens) => new() { Value = Generic.LuminousFlux<double>.FromLumens(lumens) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
