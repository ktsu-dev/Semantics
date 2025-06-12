// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a time quantity with compile-time dimensional safety.
/// </summary>
public sealed record Time : PhysicalQuantity<Time>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimension.TimeDimension;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Second;

	/// <summary>
	/// Initializes a new instance of the Time class.
	/// </summary>
	public Time() : base() { }

	/// <summary>
	/// Creates a new Time from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new Time instance.</returns>
	public static Time FromSeconds(double seconds) => Create(seconds);

	/// <summary>
	/// Gets the time value in seconds.
	/// </summary>
	/// <returns>The value in seconds.</returns>
	public double Seconds() => Value;

	/// <summary>
	/// Gets the time value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in seconds.</returns>
	public T Seconds<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the time value in minutes.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in minutes.</returns>
	public T Minutes<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Minute));

	/// <summary>
	/// Gets the time value in hours.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in hours.</returns>
	public T Hours<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Hour));

	/// <summary>
	/// Gets the time value in days.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in days.</returns>
	public T Days<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Day));

	/// <summary>
	/// Gets the time value in milliseconds.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in milliseconds.</returns>
	public T Milliseconds<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Millisecond));
}

/// <summary>
/// Generic time quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Time<T> : PhysicalQuantity<Time<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimension.TimeDimension;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Second;

	/// <summary>
	/// Initializes a new instance of the Time class.
	/// </summary>
	public Time() : base() { }

	/// <summary>
	/// Creates a new Time from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> FromSeconds(T seconds) => Create(seconds);
}
