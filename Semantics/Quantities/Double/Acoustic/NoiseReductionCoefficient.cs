// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a noise reduction coefficient quantity with double precision.
/// </summary>
public sealed record NoiseReductionCoefficient
{
	/// <summary>Gets the underlying generic noise reduction coefficient instance.</summary>
	public Generic.NoiseReductionCoefficient<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="NoiseReductionCoefficient"/> class.
	/// </summary>
	public NoiseReductionCoefficient() { }

	/// <summary>
	/// Creates a new NoiseReductionCoefficient from a value (0 to 1.25).
	/// </summary>
	/// <param name="coefficient">The NRC value (0-1.25, typically 0-1).</param>
	/// <returns>A new NoiseReductionCoefficient instance.</returns>
	public static NoiseReductionCoefficient FromCoefficient(double coefficient) => new() { Value = Generic.NoiseReductionCoefficient<double>.FromCoefficient(coefficient) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
