// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a reflection coefficient quantity with float precision.
/// </summary>
public sealed record ReflectionCoefficient : Generic.ReflectionCoefficient<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReflectionCoefficient"/> class.
	/// </summary>
	public ReflectionCoefficient() : base() { }

	/// <summary>
	/// Creates a new ReflectionCoefficient from a value (0 to 1).
	/// </summary>
	/// <param name="coefficient">The reflection coefficient (0 = perfect absorption, 1 = perfect reflection).</param>
	/// <returns>A new ReflectionCoefficient instance.</returns>
	public static new ReflectionCoefficient FromCoefficient(float coefficient) => new() { Quantity = coefficient };
}
