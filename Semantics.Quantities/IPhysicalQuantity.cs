// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Base interface for all physical quantities with compile-time type safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value (e.g., double, float, decimal).</typeparam>
public interface IPhysicalQuantity<T> : ISemanticQuantity<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the value stored in this quantity.</summary>
	public T Value { get; }

	/// <summary>Gets whether this quantity satisfies physical constraints (e.g., finite, non-NaN).</summary>
	public bool IsPhysicallyValid { get; }
}
