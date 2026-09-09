// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// Common surface for every physical quantity. Exposes the underlying
/// <see cref="Value"/>, a structural validity check, the <see cref="Dimension"/>
/// the quantity belongs to, and same-dimension comparison.
/// </summary>
/// <remarks>
/// Concrete generated quantity types also expose a dimensionally-typed
/// <c>In(I&lt;Dim&gt;Unit)</c> method — kept off this interface so cross-dimension
/// comparison via the slim contract stays compile-time clean.
/// </remarks>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public interface IPhysicalQuantity<T>
	: ISemanticQuantity<T>
	, IComparable<IPhysicalQuantity<T>>
	, IEquatable<IPhysicalQuantity<T>>
	where T : struct, INumber<T>
{
	/// <summary>Gets the value stored in this quantity (in the dimension's SI base unit).</summary>
	public T Value { get; }

	/// <summary>Gets whether this quantity satisfies structural physical constraints (finite, non-NaN).</summary>
	public bool IsPhysicallyValid { get; }

	/// <summary>Gets the physical dimension this quantity belongs to.</summary>
	public DimensionInfo Dimension { get; }
}

/// <summary>
/// A physical quantity that knows its own type, so generic code can construct one.
/// </summary>
/// <remarks>
/// This is the seam that replaced the <c>TSelf</c>-constrained record base. Generated quantities
/// are structs and cannot inherit a shared <c>Create</c>, so each declares its own and this
/// interface is what lets a caller reach it without knowing which quantity it holds.
/// </remarks>
/// <typeparam name="TSelf">The implementing quantity type.</typeparam>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public interface IPhysicalQuantity<TSelf, T>
	: IPhysicalQuantity<T>
	where TSelf : struct, IPhysicalQuantity<TSelf, T>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Creates a quantity holding <paramref name="value"/>, interpreted in the dimension's SI
	/// base unit.
	/// </summary>
	/// <param name="value">The value in the SI base unit.</param>
	/// <returns>A quantity of this type holding <paramref name="value"/>.</returns>
	public static abstract TSelf Create(T value);
}
