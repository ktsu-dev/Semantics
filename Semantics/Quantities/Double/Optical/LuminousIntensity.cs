// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a luminous intensity quantity with double precision.
/// </summary>
public sealed record LuminousIntensity : Generic.LuminousIntensity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="LuminousIntensity"/> class.
	/// </summary>
	public LuminousIntensity() : base() { }

	/// <summary>
	/// Creates a new LuminousIntensity from a value in candelas.
	/// </summary>
	/// <param name="candelas">The value in candelas.</param>
	/// <returns>A new LuminousIntensity instance.</returns>
	public static new LuminousIntensity FromCandelas(double candelas) => new() { Quantity = candelas };
}
