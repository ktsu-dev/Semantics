// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Generic PhysicalQuantity with configurable storage type.
/// </summary>
/// <typeparam name="TSelf">The derived physical quantity type.</typeparam>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public abstract record PhysicalQuantity<TSelf, T>
	: SemanticQuantity<TSelf, T>
	, IPhysicalQuantity<T>
	where TSelf : PhysicalQuantity<TSelf, T>, new()
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of this quantity.</summary>
	public abstract PhysicalDimension Dimension { get; }

	/// <summary>Gets the value stored in this quantity.</summary>
	public T Value => Quantity;

	/// <summary>Gets whether this quantity satisfies physical constraints.</summary>
	public virtual bool IsPhysicallyValid => !T.IsNaN(Value) && T.IsFinite(Value);

	/// <summary>
	/// Initializes a new instance of the PhysicalQuantity class.
	/// </summary>
	protected PhysicalQuantity() : base() { }

	/// <summary>
	/// Creates a new instance with the specified value.
	/// </summary>
	/// <param name="value">The value for the quantity.</param>
	/// <returns>A new instance of the quantity.</returns>
	public static new TSelf Create(T value) => new TSelf() with { Quantity = value };

	/// <summary>
	/// Converts the quantity value to the specified unit.
	/// </summary>
	/// <param name="targetUnit">The target unit for conversion.</param>
	/// <returns>The value in the target unit.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
	public virtual T In(IUnit targetUnit)
	{
		ArgumentNullException.ThrowIfNull(targetUnit);

		if (!targetUnit.Dimension.Equals(Dimension))
		{
			throw new UnitConversionException(Dimension.BaseUnit, targetUnit, $"Cannot convert {Dimension} to {targetUnit.Dimension}");
		}

		// Handle offset units (like temperature conversions)
		if (Math.Abs(targetUnit.ToBaseOffset) > 1e-10)
		{
			T factor = T.CreateChecked(targetUnit.ToBaseFactor);
			T offset = T.CreateChecked(targetUnit.ToBaseOffset);
			return (Value - offset) / factor;
		}

		// Handle linear units (most common case)
		T conversionFactor = T.CreateChecked(targetUnit.ToBaseFactor);
		return Value / conversionFactor;
	}

	/// <summary>
	/// Compares this quantity to another quantity.
	/// </summary>
	/// <param name="other">The other quantity to compare to.</param>
	/// <returns>A value indicating the relative order of the quantities.</returns>
	public int CompareTo(TSelf? other) => other is null ? 1 : Value.CompareTo(other.Value);

	/// <summary>
	/// Compares this quantity to another physical quantity.
	/// </summary>
	/// <param name="other">The other quantity to compare to.</param>
	/// <returns>A value indicating the relative order of the quantities.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "<Pending>")]
	public int CompareTo(IPhysicalQuantity<T>? other)
	{
		return other is null
			? 1
			: other is not TSelf typedOther
			? throw new ArgumentException($"Cannot compare {GetType().Name} to {other.GetType().Name}")
			: CompareTo(typedOther);
	}

	/// <summary>
	/// Determines whether this quantity is equal to another physical quantity.
	/// </summary>
	/// <param name="other">The other quantity to compare to.</param>
	/// <returns>True if the quantities are equal; otherwise, false.</returns>
	public virtual bool Equals(IPhysicalQuantity<T>? other) => other is TSelf typedOther && Equals(typedOther);

	/// <summary>
	/// Returns a string representation of this quantity.
	/// </summary>
	/// <returns>A string representation including the value and unit symbol.</returns>
	public override string ToString() => $"{Value} {Dimension.BaseUnit.Symbol}";
	/// <inheritdoc/>
	public static bool operator <(PhysicalQuantity<TSelf, T> left, PhysicalQuantity<TSelf, T> right) => left is null ? right is not null : left.CompareTo(right) < 0;

	/// <inheritdoc/>
	public static bool operator <=(PhysicalQuantity<TSelf, T> left, PhysicalQuantity<TSelf, T> right) => left is null || left.CompareTo(right) <= 0;

	/// <inheritdoc/>
	public static bool operator >(PhysicalQuantity<TSelf, T> left, PhysicalQuantity<TSelf, T> right) => left is not null && left.CompareTo(right) > 0;

	/// <inheritdoc/>
	public static bool operator >=(PhysicalQuantity<TSelf, T> left, PhysicalQuantity<TSelf, T> right) => left is null ? right is null : left.CompareTo(right) >= 0;
}
