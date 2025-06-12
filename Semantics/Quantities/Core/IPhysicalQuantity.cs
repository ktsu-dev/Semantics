// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Base interface for all physical quantities with compile-time type safety and dimensional validation.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value (e.g., double, float, decimal).</typeparam>
public interface IPhysicalQuantity<T> : ISemanticQuantity<T>, IEquatable<IPhysicalQuantity<T>>, IComparable<IPhysicalQuantity<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of this quantity.</summary>
	public PhysicalDimension Dimension { get; }

	/// <summary>Gets the SI base unit for this quantity type.</summary>
	public IUnit BaseUnit { get; }

	/// <summary>Converts the quantity value to the specified unit.</summary>
	/// <param name="targetUnit">The target unit for conversion.</param>
	/// <returns>The value in the target unit.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
	public T In(IUnit targetUnit);

	/// <summary>Gets whether this quantity satisfies physical constraints (e.g., positive temperature above absolute zero).</summary>
	public bool IsPhysicallyValid { get; }
}
