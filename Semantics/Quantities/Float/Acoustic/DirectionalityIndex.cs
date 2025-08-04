// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a directionality index quantity with float precision.
/// </summary>
public sealed record DirectionalityIndex : Generic.DirectionalityIndex<float>
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
	public static new DirectionalityIndex FromDecibels(float decibels) => Create(decibels);

	/// <summary>
	/// Creates a DirectionalityIndex from directivity factor Q.
	/// </summary>
	/// <param name="directivityFactor">The directivity factor Q.</param>
	/// <returns>The corresponding directionality index.</returns>
	public static new DirectionalityIndex FromDirectivityFactor(float directivityFactor) => Create(10.0f * (float)Math.Log10(directivityFactor));
}
