// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a length quantity with compile-time dimensional safety.
/// </summary>
public sealed record Length : PhysicalQuantity<Length>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Length;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Meter;

	/// <summary>
	/// Initializes a new instance of the Length class.
	/// </summary>
	public Length() : base() { }

	/// <summary>
	/// Creates a new Length from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length FromMeters(double meters) => Create(meters);

	/// <summary>
	/// Gets the length value in meters.
	/// </summary>
	/// <returns>The value in meters.</returns>
	public double Meters() => Value;

	/// <summary>
	/// Gets the length value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in meters.</returns>
	public T Meters<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the length value in kilometers.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in kilometers.</returns>
	public T Kilometers<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Kilometer));

	/// <summary>
	/// Gets the length value in centimeters.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in centimeters.</returns>
	public T Centimeters<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Centimeter));

	/// <summary>
	/// Gets the length value in millimeters.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in millimeters.</returns>
	public T Millimeters<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Millimeter));

	/// <summary>
	/// Gets the length value in feet.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in feet.</returns>
	public T Feet<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Foot));

	/// <summary>
	/// Gets the length value in inches.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in inches.</returns>
	public T Inches<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Inch));

	/// <summary>
	/// Gets the length value in yards.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in yards.</returns>
	public T Yards<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Yard));

	/// <summary>
	/// Gets the length value in miles.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in miles.</returns>
	public T Miles<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Mile));

	/// <summary>
	/// Multiplies this length by another length to create an area.
	/// </summary>
	/// <param name="left">The first length.</param>
	/// <param name="right">The second length.</param>
	/// <returns>The resulting area.</returns>
	public static Area operator *(Length left, Length right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Area.Create(left.Value * right.Value);
	}

	/// <summary>
	/// Divides this length by time to create a velocity.
	/// </summary>
	/// <param name="left">The length.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting velocity.</returns>
	public static Velocity operator /(Length left, Time right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);
		return Velocity.Create(left.Value / right.Value);
	}

	/// <summary>
	/// Gets the absolute value of this length.
	/// </summary>
	/// <returns>The absolute length.</returns>
	public Length Abs() => Create(Math.Abs(Value));

	/// <summary>
	/// Clamps this length between the specified minimum and maximum values.
	/// </summary>
	/// <param name="min">The minimum value.</param>
	/// <param name="max">The maximum value.</param>
	/// <returns>The clamped length.</returns>
	public Length Clamp(double min, double max) => Create(Math.Clamp(Value, min, max));

	/// <summary>
	/// Raises this length to the specified power.
	/// </summary>
	/// <param name="exponent">The exponent.</param>
	/// <returns>The length raised to the power.</returns>
	public Length Pow(double exponent) => Create(Math.Pow(Value, exponent));

	/// <inheritdoc/>
	public static Area Multiply(Length left, Length right) => throw new NotImplementedException();
	/// <inheritdoc/>
	public static Velocity Divide(Length left, Length right) => throw new NotImplementedException();
}

/// <summary>
/// Generic length quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Length<T> : PhysicalQuantity<Length<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Length;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Meter;

	/// <summary>
	/// Initializes a new instance of the Length class.
	/// </summary>
	public Length() : base() { }

	/// <summary>
	/// Creates a new Length from a value in meters.
	/// </summary>
	/// <param name="meters">The value in meters.</param>
	/// <returns>A new Length instance.</returns>
	public static Length<T> FromMeters(T meters) => Create(meters);

	// Note: Operators are inherited from base class for standard arithmetic operations.
	// Dimensional operators are defined in the non-generic Length class.
}

// Note: Operator interfaces removed due to C# constraints on binary operators.
// Dimensional operations are defined directly on each quantity type.
