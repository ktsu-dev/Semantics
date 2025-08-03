// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a pH quantity with float precision.
/// </summary>
public sealed record PH : Generic.PH<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PH"/> class.
	/// </summary>
	public PH() : base() { }

	/// <summary>
	/// Creates a new pH from a dimensionless value.
	/// </summary>
	/// <param name="value">The pH value.</param>
	/// <returns>A new PH instance.</returns>
	public static PH FromValue(float value) => new() { Value = value };
}
