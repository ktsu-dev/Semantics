// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a Reynolds number quantity with float precision.
/// </summary>
public sealed record ReynoldsNumber
{
	/// <summary>Gets the underlying generic Reynolds number instance.</summary>
	public Generic.ReynoldsNumber<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReynoldsNumber"/> class.
	/// </summary>
	public ReynoldsNumber() { }

	/// <summary>
	/// Creates a new ReynoldsNumber from a dimensionless value.
	/// </summary>
	/// <param name="value">The dimensionless Reynolds number value.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	public static new ReynoldsNumber FromValue(float value) => new() { Value = Generic.ReynoldsNumber<float>.FromValue(value) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
