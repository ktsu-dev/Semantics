// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a directionality index quantity with double precision.
/// </summary>
public sealed record DirectionalityIndex : Generic.DirectionalityIndex<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DirectionalityIndex"/> class.
	/// </summary>
	public DirectionalityIndex() : base() { }

	/// <summary>
	/// Creates a new DirectionalityIndex from a value in decibels.
	/// </summary>
	/// <param name="decibels">The directionality index in dB.</param>
	/// <returns>A new DirectionalityIndex instance.</returns>
	public static new DirectionalityIndex FromDecibels(double decibels) => new() { Value = decibels };

	/// <summary>
	/// Creates a DirectionalityIndex from directivity factor Q.
	/// </summary>
	/// <param name="directivityFactor">The directivity factor Q.</param>
	/// <returns>The corresponding directionality index.</returns>
	public static new DirectionalityIndex FromDirectivityFactor(double directivityFactor) => new() { Value = directivityFactor };
}
