// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// The validity and cross-dimension comparison rules every generated physical quantity shares.
/// </summary>
/// <remarks>
/// These used to be inherited from an abstract <c>PhysicalQuantity</c> record. Generated
/// quantities are structs now, and a struct cannot inherit, so each one declares the three
/// members and delegates here — three one-line bodies per type instead of a copy of the rule.
/// The receiver is taken by <see langword="in"/> and constrained to a struct so that delegating
/// does not box it; only the <see cref="IPhysicalQuantity{T}"/> argument, which is already an
/// interface at the call site, can be boxed.
/// </remarks>
public static class PhysicalQuantityCore
{
	/// <summary>
	/// Reports whether <paramref name="value"/> is a value a physical quantity can hold: finite
	/// and not NaN.
	/// </summary>
	/// <typeparam name="T">The storage type for the quantity value.</typeparam>
	/// <param name="value">The stored value, in the dimension's SI base unit.</param>
	/// <returns><see langword="true"/> when the value is finite and not NaN.</returns>
	public static bool IsPhysicallyValid<T>(T value)
		where T : struct, INumber<T>
		=> !T.IsNaN(value) && T.IsFinite(value);

	/// <summary>
	/// Compares <paramref name="self"/> to another quantity of the same physical dimension.
	/// </summary>
	/// <remarks>
	/// Quantities of different dimensions are not ordered, so this throws rather than inventing
	/// an answer. Equality, below, is total instead and returns <see langword="false"/>.
	/// </remarks>
	/// <typeparam name="TSelf">The receiver's quantity type.</typeparam>
	/// <typeparam name="T">The storage type for the quantity value.</typeparam>
	/// <param name="self">The receiver.</param>
	/// <param name="other">The quantity to compare against, or <see langword="null"/>.</param>
	/// <returns>
	/// A negative number, zero or a positive number as <paramref name="self"/> sorts before,
	/// with, or after <paramref name="other"/>. A <see langword="null"/> other sorts first.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// When the two quantities do not share a physical dimension.
	/// </exception>
	public static int Compare<TSelf, T>(in TSelf self, IPhysicalQuantity<T>? other)
		where TSelf : struct, IPhysicalQuantity<T>
		where T : struct, INumber<T>
	{
		if (other is null)
		{
			return 1;
		}

		if (!Equals(self.Dimension, other.Dimension))
		{
			throw new ArgumentException(
				$"Cannot compare quantity of dimension '{self.Dimension.Name}' to quantity of dimension '{other.Dimension.Name}'.",
				nameof(other));
		}

		return self.Value.CompareTo(other.Value);
	}

	/// <summary>
	/// Reports whether two quantities share a dimension and a value.
	/// </summary>
	/// <remarks>
	/// Equality is total: comparing across dimensions returns <see langword="false"/> rather than
	/// throwing, because a collection asking whether it already holds a value must get an answer.
	/// </remarks>
	/// <typeparam name="TSelf">The receiver's quantity type.</typeparam>
	/// <typeparam name="T">The storage type for the quantity value.</typeparam>
	/// <param name="self">The receiver.</param>
	/// <param name="other">The quantity to compare against, or <see langword="null"/>.</param>
	/// <returns><see langword="true"/> when both dimension and value match.</returns>
	public static bool AreEqual<TSelf, T>(in TSelf self, IPhysicalQuantity<T>? other)
		where TSelf : struct, IPhysicalQuantity<T>
		where T : struct, INumber<T>
	{
		if (other is null)
		{
			return false;
		}

		return Equals(self.Dimension, other.Dimension) && self.Value.Equals(other.Value);
	}
}
