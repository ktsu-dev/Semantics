// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// Base record for all physical quantity types. Inherits arithmetic operators from
/// <see cref="SemanticQuantity{TSelf, TStorage}"/> and adds <see cref="Value"/>,
/// <see cref="Dimension"/>, and same-dimension comparison.
/// </summary>
/// <typeparam name="TSelf">The derived physical quantity type.</typeparam>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public abstract record PhysicalQuantity<TSelf, T>
	: SemanticQuantity<TSelf, T>
	, IPhysicalQuantity<T>
	where TSelf : PhysicalQuantity<TSelf, T>, new()
	where T : struct, INumber<T>
{
	/// <summary>Gets the value stored in this quantity (in the dimension's SI base unit).</summary>
	public T Value => Quantity;

	/// <summary>Gets whether this quantity satisfies structural physical constraints.</summary>
	public virtual bool IsPhysicallyValid => !T.IsNaN(Value) && T.IsFinite(Value);

	/// <summary>Gets the physical dimension this quantity belongs to. Implemented by generated types.</summary>
	public abstract DimensionInfo Dimension { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="PhysicalQuantity{TSelf, T}"/> class.
	/// </summary>
	protected PhysicalQuantity() : base() { }

	/// <summary>
	/// Compares this quantity to another of the same physical dimension. Throws
	/// <see cref="ArgumentException"/> if dimensions differ — quantities of different
	/// dimensions are not ordered.
	/// </summary>
	public int CompareTo(IPhysicalQuantity<T>? other)
	{
		if (other is null)
		{
			return 1;
		}

		if (!Equals(Dimension, other.Dimension))
		{
			throw new ArgumentException(
				$"Cannot compare quantity of dimension '{Dimension.Name}' to quantity of dimension '{other.Dimension.Name}'.",
				nameof(other));
		}

		return Value.CompareTo(other.Value);
	}

	/// <summary>
	/// Equality across the <see cref="IPhysicalQuantity{T}"/> surface — two quantities
	/// are equal iff they share a dimension and a value. Cross-dimension comparisons
	/// return <c>false</c> (they don't throw — equality is total).
	/// </summary>
	public virtual bool Equals(IPhysicalQuantity<T>? other)
	{
		if (other is null)
		{
			return false;
		}

		return Equals(Dimension, other.Dimension) && Value.Equals(other.Value);
	}
}
