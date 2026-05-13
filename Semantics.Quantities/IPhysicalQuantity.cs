// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
	T Value { get; }

	/// <summary>Gets whether this quantity satisfies structural physical constraints (finite, non-NaN).</summary>
	bool IsPhysicallyValid { get; }

	/// <summary>Gets the physical dimension this quantity belongs to.</summary>
	DimensionInfo Dimension { get; }
}
