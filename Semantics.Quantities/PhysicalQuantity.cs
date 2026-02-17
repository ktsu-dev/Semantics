// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Base record for all physical quantity types. Inherits arithmetic operators from
/// <see cref="SemanticQuantity{TSelf, TStorage}"/> and adds a <see cref="Value"/> property
/// as an alias for the underlying storage.
/// </summary>
/// <typeparam name="TSelf">The derived physical quantity type.</typeparam>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public abstract record PhysicalQuantity<TSelf, T>
	: SemanticQuantity<TSelf, T>
	, IPhysicalQuantity<T>
	where TSelf : PhysicalQuantity<TSelf, T>, new()
	where T : struct, INumber<T>
{
	/// <summary>Gets the value stored in this quantity.</summary>
	public T Value => Quantity;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public virtual bool IsPhysicallyValid => !T.IsNaN(Value) && T.IsFinite(Value);

	/// <summary>
	/// Initializes a new instance of the <see cref="PhysicalQuantity{TSelf, T}"/> class.
	/// </summary>
	protected PhysicalQuantity() : base() { }
}
