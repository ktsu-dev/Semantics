// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a Reynolds number quantity with float precision.
/// </summary>
public sealed record ReynoldsNumber : Generic.ReynoldsNumber<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReynoldsNumber"/> class.
	/// </summary>
	public ReynoldsNumber() : base() { }

	/// <summary>
	/// Creates a new ReynoldsNumber from a dimensionless value.
	/// </summary>
	/// <param name="value">The dimensionless Reynolds number value.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	public static new ReynoldsNumber FromValue(float value) => new() { Value = value };
}
