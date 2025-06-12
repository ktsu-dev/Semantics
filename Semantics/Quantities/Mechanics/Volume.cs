// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a volume quantity with compile-time dimensional safety.
/// </summary>
public sealed record Volume : PhysicalQuantity<Volume>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Volume;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.CubicMeter;

	/// <summary>
	/// Initializes a new instance of the Volume class.
	/// </summary>
	public Volume() : base() { }

	/// <summary>
	/// Creates a new Volume from a value in cubic meters.
	/// </summary>
	/// <param name="cubicMeters">The value in cubic meters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume FromCubicMeters(double cubicMeters) => Create(cubicMeters);

	/// <summary>
	/// Gets the volume value in cubic meters.
	/// </summary>
	/// <returns>The value in cubic meters.</returns>
	public double CubicMeters() => Value;

	/// <summary>
	/// Gets the volume value in the specified unit.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in cubic meters.</returns>
	public T CubicMeters<T>() where T : struct, INumber<T> => T.CreateChecked(Value);

	/// <summary>
	/// Gets the volume value in liters.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in liters.</returns>
	public T Liters<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Liter));

	/// <summary>
	/// Gets the volume value in milliliters.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in milliliters.</returns>
	public T Milliliters<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.Milliliter));

	/// <summary>
	/// Gets the volume value in cubic feet.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in cubic feet.</returns>
	public T CubicFeet<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.CubicFoot));

	/// <summary>
	/// Gets the volume value in US gallons.
	/// </summary>
	/// <typeparam name="T">The numeric type for the result.</typeparam>
	/// <returns>The value in US gallons.</returns>
	public T USGallons<T>() where T : struct, INumber<T> => T.CreateChecked(In(Units.USGallon));
}

/// <summary>
/// Generic volume quantity with configurable storage type.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Volume<T> : PhysicalQuantity<Volume<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Volume;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.CubicMeter;

	/// <summary>
	/// Initializes a new instance of the Volume class.
	/// </summary>
	public Volume() : base() { }

	/// <summary>
	/// Creates a new Volume from a value in cubic meters.
	/// </summary>
	/// <param name="cubicMeters">The value in cubic meters.</param>
	/// <returns>A new Volume instance.</returns>
	public static Volume<T> FromCubicMeters(T cubicMeters) => Create(cubicMeters);
}
