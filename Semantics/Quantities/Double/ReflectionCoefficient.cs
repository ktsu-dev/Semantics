// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a reflection coefficient quantity with double precision.
/// </summary>
public sealed record ReflectionCoefficient
{
	/// <summary>Gets the underlying generic reflection coefficient instance.</summary>
	public Generic.ReflectionCoefficient<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReflectionCoefficient"/> class.
	/// </summary>
	public ReflectionCoefficient() { }

	/// <summary>
	/// Creates a new ReflectionCoefficient from a value (0 to 1).
	/// </summary>
	/// <param name="coefficient">The reflection coefficient (0 = perfect absorption, 1 = perfect reflection).</param>
	/// <returns>A new ReflectionCoefficient instance.</returns>
	public static ReflectionCoefficient FromCoefficient(double coefficient) => new() { Value = Generic.ReflectionCoefficient<double>.FromCoefficient(coefficient) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
