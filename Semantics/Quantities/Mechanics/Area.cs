// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an area quantity with compile-time dimensional safety.
/// </summary>
public sealed record Area : PhysicalQuantity<Area>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Area;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.SquareMeter;

	/// <summary>
	/// Initializes a new instance of the Area class.
	/// </summary>
	public Area() : base() { }

	/// <summary>
	/// Creates a new Area from a value in square meters.
	/// </summary>
	/// <param name="squareMeters">The value in square meters.</param>
	/// <returns>A new Area instance.</returns>
	public static Area FromSquareMeters(double squareMeters) => Create(squareMeters);

	/// <summary>
	/// Gets the area value in square meters.
	/// </summary>
	/// <returns>The value in square meters.</returns>
	public double SquareMeters() => Value;

	/// <summary>
	/// Gets the area value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in square meters.</returns>
	public T SquareMeters<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the area value in square kilometers.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in square kilometers.</returns>
	public T SquareKilometers<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.SquareKilometer));

	/// <summary>
	/// Gets the area value in square feet.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in square feet.</returns>
	public T SquareFeet<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.SquareFoot));

	/// <summary>
	/// Gets the area value in acres.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in acres.</returns>
	public T Acres<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Acre));

	/// <summary>
	/// Multiplies this area by a length to create a volume.
	/// </summary>
	/// <param name="left">The area.</param>
	/// <param name="right">The length.</param>
	/// <returns>The resulting volume.</returns>
	public static Volume operator *(Area left, Length right) => Volume.Create(left.Value * right.Value);

	/// <inheritdoc/>
	public static Volume Multiply(Area left, Area right) => throw new NotImplementedException();
}

/// <summary>
/// Generic area quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Area<T> : PhysicalQuantity<Area<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Area;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.SquareMeter;

	/// <summary>
	/// Initializes a new instance of the Area class.
	/// </summary>
	public Area() : base() { }

	/// <summary>
	/// Creates a new Area from a value in square meters.
	/// </summary>
	/// <param name="squareMeters">The value in square meters.</param>
	/// <returns>A new Area instance.</returns>
	public static Area<T> FromSquareMeters(T squareMeters) => Create(squareMeters);

	// Note: Operators are inherited from base class for standard arithmetic operations.
	// Dimensional operators are defined in the non-generic Area class.
}
