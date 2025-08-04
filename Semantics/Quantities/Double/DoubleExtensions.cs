// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Double;

using System.Numerics;

/// <summary>
/// Extension methods for creating double-precision physical quantities from numeric values.
/// Provides convenient factory methods with unit conversion support for commonly used units.
/// For comprehensive unit conversions, use the Generic.ScalarExtensions with type parameters.
/// </summary>
public static class DoubleExtensions
{
	#region Helper Methods
	/// <summary>
	/// Converts a value from a specific unit to the base unit of that dimension.
	/// </summary>
	/// <param name="value">The value in the source unit.</param>
	/// <param name="sourceUnit">The source unit to convert from.</param>
	/// <returns>The value converted to the base unit.</returns>
	private static double ConvertToBaseUnit(double value, IUnit sourceUnit)
	{
		ArgumentNullException.ThrowIfNull(sourceUnit);

		// Handle offset units (like temperature conversions)
		if (Math.Abs(sourceUnit.ToBaseOffset) > 1e-10)
		{
			double factor = sourceUnit.ToBaseFactor;
			double offset = sourceUnit.ToBaseOffset;
			return (value * factor) + offset;
		}

		// Handle linear units (most common case)
		double conversionFactor = sourceUnit.ToBaseFactor;
		return value * conversionFactor;
	}
	#endregion

	#region Length Extensions

	/// <summary>Creates a Length from a value in meters (base unit).</summary>
	public static Length Meters(this double value) => Length.FromMeters(value);

	/// <summary>Creates a Length from a value in kilometers.</summary>
	public static Length Kilometers(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Kilometer));

	/// <summary>Creates a Length from a value in centimeters.</summary>
	public static Length Centimeters(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Centimeter));

	/// <summary>Creates a Length from a value in millimeters.</summary>
	public static Length Millimeters(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Millimeter));

	/// <summary>Creates a Length from a value in inches.</summary>
	public static Length Inches(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Inch));

	/// <summary>Creates a Length from a value in feet.</summary>
	public static Length Feet(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Foot));

	/// <summary>Creates a Length from a value in yards.</summary>
	public static Length Yards(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Yard));

	/// <summary>Creates a Length from a value in miles.</summary>
	public static Length Miles(this double value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Mile));

	#endregion

	#region Mass Extensions

	/// <summary>Creates a Mass from a value in kilograms (base unit).</summary>
	public static Mass Kilograms(this double value) => Mass.FromKilograms(value);

	/// <summary>Creates a Mass from a value in grams.</summary>
	public static Mass Grams(this double value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Gram));

	/// <summary>Creates a Mass from a value in pounds.</summary>
	public static Mass Pounds(this double value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Pound));

	/// <summary>Creates a Mass from a value in ounces.</summary>
	public static Mass Ounces(this double value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Ounce));

	/// <summary>Creates a Mass from a value in stones.</summary>
	public static Mass Stones(this double value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Stone));

	#endregion

	#region Time Extensions

	/// <summary>Creates a Time from a value in seconds (base unit).</summary>
	public static Time Seconds(this double value) => Time.FromSeconds(value);

	/// <summary>Creates a Time from a value in milliseconds.</summary>
	public static Time Milliseconds(this double value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Millisecond));

	/// <summary>Creates a Time from a value in minutes.</summary>
	public static Time Minutes(this double value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Minute));

	/// <summary>Creates a Time from a value in hours.</summary>
	public static Time Hours(this double value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Hour));

	/// <summary>Creates a Time from a value in days.</summary>
	public static Time Days(this double value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Day));

	#endregion

	#region Area Extensions

	/// <summary>Creates an Area from a value in square meters (base unit).</summary>
	public static Area SquareMeters(this double value) => Area.FromSquareMeters(value);

	/// <summary>Creates an Area from a value in square kilometers.</summary>
	public static Area SquareKilometers(this double value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareKilometer));

	/// <summary>Creates an Area from a value in square centimeters.</summary>
	public static Area SquareCentimeters(this double value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareCentimeter));

	/// <summary>Creates an Area from a value in square feet.</summary>
	public static Area SquareFeet(this double value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareFoot));

	/// <summary>Creates an Area from a value in acres.</summary>
	public static Area Acres(this double value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.Acre));

	/// <summary>Creates an Area from a value in hectares.</summary>
	public static Area Hectares(this double value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.Hectare));

	#endregion

	#region Volume Extensions

	/// <summary>Creates a Volume from a value in cubic meters (base unit).</summary>
	public static Volume CubicMeters(this double value) => Volume.FromCubicMeters(value);

	/// <summary>Creates a Volume from a value in liters.</summary>
	public static Volume Liters(this double value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.Liter));

	/// <summary>Creates a Volume from a value in milliliters.</summary>
	public static Volume Milliliters(this double value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.Milliliter));

	/// <summary>Creates a Volume from a value in cubic feet.</summary>
	public static Volume CubicFeet(this double value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.CubicFoot));

	/// <summary>Creates a Volume from a value in US gallons.</summary>
	public static Volume USGallons(this double value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.USGallon));

	#endregion

	#region Basic Quantity Extensions

	/// <summary>Creates an Energy from a value in joules (base unit).</summary>
	public static Energy Joules(this double value) => Energy.FromJoules(value);

	/// <summary>Creates a Power from a value in watts (base unit).</summary>
	public static Power Watts(this double value) => Power.FromWatts(value);

	/// <summary>Creates a Force from a value in newtons (base unit).</summary>
	public static Force Newtons(this double value) => Force.FromNewtons(value);

	/// <summary>Creates a Pressure from a value in pascals (base unit).</summary>
	public static Pressure Pascals(this double value) => Pressure.FromPascals(value);

	/// <summary>Creates a Pressure from a value in atmospheres.</summary>
	public static Pressure Atmospheres(this double value) => Pressure.FromPascals(ConvertToBaseUnit(value, Units.Atmosphere));

	/// <summary>Creates a Pressure from a value in PSI.</summary>
	public static Pressure PSI(this double value) => Pressure.FromPascals(ConvertToBaseUnit(value, Units.PSI));

	/// <summary>Creates a Velocity from a value in meters per second (base unit).</summary>
	public static Velocity MetersPerSecond(this double value) => Velocity.FromMetersPerSecond(value);

	/// <summary>Creates an Acceleration from a value in meters per second squared (base unit).</summary>
	public static Acceleration MetersPerSecondSquared(this double value) => Acceleration.FromMetersPerSecondSquared(value);

	/// <summary>Creates a Temperature from a value in Kelvin (base unit).</summary>
	public static Temperature Kelvin(this double value) => Temperature.FromKelvin(value);

	/// <summary>Creates a Frequency from a value in hertz (base unit).</summary>
	public static Frequency Hertz(this double value) => Frequency.FromHertz(value);

	#endregion

	#region Vector Quantities - 2D

	/// <summary>Creates a Position2D from X and Y coordinates in meters.</summary>
	public static Position2D MetersPosition2D(this (double x, double y) coordinates)
		=> Position2D.FromMeters(coordinates.x, coordinates.y);

	/// <summary>Creates a Displacement2D from X and Y components in meters.</summary>
	public static Displacement2D MetersDisplacement2D(this (double x, double y) components)
		=> Displacement2D.FromMeters(components.x, components.y);

	/// <summary>Creates a Velocity2D from X and Y components in meters per second.</summary>
	public static Velocity2D MetersPerSecondVelocity2D(this (double x, double y) components)
		=> Velocity2D.FromMetersPerSecond(components.x, components.y);

	/// <summary>Creates an Acceleration2D from X and Y components in meters per second squared.</summary>
	public static Acceleration2D MetersPerSecondSquaredAcceleration2D(this (double x, double y) components)
		=> Acceleration2D.FromMetersPerSecondSquared(components.x, components.y);

	/// <summary>Creates a Force2D from X and Y components in newtons.</summary>
	public static Force2D NewtonsForce2D(this (double x, double y) components)
		=> Force2D.FromNewtons(components.x, components.y);

	#endregion

	#region Vector Quantities - 3D

	/// <summary>Creates a Position3D from X, Y, and Z coordinates in meters.</summary>
	public static Position3D MetersPosition3D(this (double x, double y, double z) coordinates)
		=> Position3D.FromMeters(coordinates.x, coordinates.y, coordinates.z);

	/// <summary>Creates a Displacement3D from X, Y, and Z components in meters.</summary>
	public static Displacement3D MetersDisplacement3D(this (double x, double y, double z) components)
		=> Displacement3D.FromMeters(components.x, components.y, components.z);

	/// <summary>Creates a Velocity3D from X, Y, and Z components in meters per second.</summary>
	public static Velocity3D MetersPerSecondVelocity3D(this (double x, double y, double z) components)
		=> Velocity3D.FromMetersPerSecond(components.x, components.y, components.z);

	/// <summary>Creates an Acceleration3D from X, Y, and Z components in meters per second squared.</summary>
	public static Acceleration3D MetersPerSecondSquaredAcceleration3D(this (double x, double y, double z) components)
		=> Acceleration3D.FromMetersPerSecondSquared(components.x, components.y, components.z);

	/// <summary>Creates a Force3D from X, Y, and Z components in newtons.</summary>
	public static Force3D NewtonsForce3D(this (double x, double y, double z) components)
		=> Force3D.FromNewtons(components.x, components.y, components.z);

	#endregion

	#region System.Numerics Integration

	/// <summary>Creates a Position2D from a System.Numerics.Vector2 in meters.</summary>
	public static Position2D MetersPosition2D(this Vector2 vector)
		=> Position2D.FromMeters(vector);

	/// <summary>Creates a Velocity2D from a System.Numerics.Vector2 in meters per second.</summary>
	public static Velocity2D MetersPerSecondVelocity2D(this Vector2 vector)
		=> Velocity2D.FromMetersPerSecond(vector);

	/// <summary>Creates a Position3D from a System.Numerics.Vector3 in meters.</summary>
	public static Position3D MetersPosition3D(this Vector3 vector)
		=> Position3D.FromMeters(vector);

	/// <summary>Creates a Velocity3D from a System.Numerics.Vector3 in meters per second.</summary>
	public static Velocity3D MetersPerSecondVelocity3D(this Vector3 vector)
		=> Velocity3D.FromMetersPerSecond(vector);

	/// <summary>Creates a Force3D from a System.Numerics.Vector3 in newtons.</summary>
	public static Force3D NewtonsForce3D(this Vector3 vector)
		=> Force3D.FromNewtons(vector);

	/// <summary>Creates an Acceleration2D from a System.Numerics.Vector2 in meters per second squared.</summary>
	public static Acceleration2D MetersPerSecondSquaredAcceleration2D(this Vector2 vector)
		=> Acceleration2D.FromMetersPerSecondSquared(vector);

	/// <summary>Creates an Acceleration3D from a System.Numerics.Vector3 in meters per second squared.</summary>
	public static Acceleration3D MetersPerSecondSquaredAcceleration3D(this Vector3 vector)
		=> Acceleration3D.FromMetersPerSecondSquared(vector);

	/// <summary>Creates a Displacement2D from a System.Numerics.Vector2 in meters.</summary>
	public static Displacement2D MetersDisplacement2D(this Vector2 vector)
		=> Displacement2D.FromMeters(vector);

	/// <summary>Creates a Displacement3D from a System.Numerics.Vector3 in meters.</summary>
	public static Displacement3D MetersDisplacement3D(this Vector3 vector)
		=> Displacement3D.FromMeters(vector);

	#endregion
}
