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

	/// <summary>Converts the quantity value to the specified unit.</summary>
	/// <param name="targetUnit">The target unit for conversion.</param>
	/// <returns>The value in the target unit.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
	public T In(IUnit targetUnit);

	/// <summary>Gets whether this quantity satisfies physical constraints (e.g., positive temperature above absolute zero).</summary>
	public bool IsPhysicallyValid { get; }

	/// <summary>
	/// Converts a value from a specific unit to the base unit of that dimension.
	/// </summary>
	/// <param name="value">The value in the source unit.</param>
	/// <param name="sourceUnit">The source unit to convert from.</param>
	/// <returns>The value converted to the base unit.</returns>
	public static T ConvertToBaseUnit(T value, IUnit sourceUnit)
	{
		ArgumentNullException.ThrowIfNull(sourceUnit);

		// Handle offset units (like temperature conversions)
		if (Math.Abs(sourceUnit.ToBaseOffset) > 1e-10)
		{
			T factor = T.CreateChecked(sourceUnit.ToBaseFactor);
			T offset = T.CreateChecked(sourceUnit.ToBaseOffset);
			return (value * factor) + offset;
		}

		// Handle linear units (most common case)
		T conversionFactor = T.CreateChecked(sourceUnit.ToBaseFactor);
		return value * conversionFactor;
	}
}
