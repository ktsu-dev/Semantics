// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a mass quantity with compile-time dimensional safety.
/// </summary>
public sealed record Mass : PhysicalQuantity<Mass>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Mass;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Kilogram;

	/// <summary>
	/// Initializes a new instance of the Mass class.
	/// </summary>
	public Mass() : base() { }

	/// <summary>
	/// Creates a new Mass from a value in kilograms.
	/// </summary>
	/// <param name="kilograms">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass FromKilograms(double kilograms) => Create(kilograms);

	/// <summary>
	/// Gets the mass value in kilograms.
	/// </summary>
	/// <returns>The value in kilograms.</returns>
	public double Kilograms() => Value;

	/// <summary>
	/// Gets the mass value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in kilograms.</returns>
	public T Kilograms<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the mass value in grams.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in grams.</returns>
	public T Grams<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Gram));

	/// <summary>
	/// Gets the mass value in pounds.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in pounds.</returns>
	public T Pounds<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Pound));

	/// <summary>
	/// Gets the mass value in ounces.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in ounces.</returns>
	public T Ounces<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Ounce));

	/// <summary>
	/// Gets the mass value in stones.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in stones.</returns>
	public T Stones<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Stone));
}

/// <summary>
/// Generic mass quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Mass<T> : PhysicalQuantity<Mass<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Mass;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Kilogram;

	/// <summary>
	/// Initializes a new instance of the Mass class.
	/// </summary>
	public Mass() : base() { }

	/// <summary>
	/// Creates a new Mass from a value in kilograms.
	/// </summary>
	/// <param name="kilograms">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> FromKilograms(T kilograms) => Create(kilograms);
}
