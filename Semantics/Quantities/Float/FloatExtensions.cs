// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Float;

using System.Numerics;

/// <summary>
/// Extension methods for creating single-precision physical quantities from numeric values.
/// Provides convenient factory methods with unit conversion support for commonly used units.
/// For comprehensive unit conversions, use the Generic.ScalarExtensions with type parameters.
/// </summary>
public static class FloatExtensions
{
	#region Helper Methods
	/// <summary>
	/// Converts a value from a specific unit to the base unit of that dimension.
	/// </summary>
	/// <param name="value">The value in the source unit.</param>
	/// <param name="sourceUnit">The source unit to convert from.</param>
	/// <returns>The value converted to the base unit.</returns>
	private static float ConvertToBaseUnit(float value, IUnit sourceUnit)
	{
		ArgumentNullException.ThrowIfNull(sourceUnit);

		// Handle offset units (like temperature conversions)
		if (Math.Abs(sourceUnit.ToBaseOffset) > 1e-10)
		{
			float factor = (float)sourceUnit.ToBaseFactor;
			float offset = (float)sourceUnit.ToBaseOffset;
			return (value * factor) + offset;
		}

		// Handle linear units (most common case)
		float conversionFactor = (float)sourceUnit.ToBaseFactor;
		return value * conversionFactor;
	}
	#endregion

	#region Length Extensions

	/// <summary>Creates a Length from a value in meters (base unit).</summary>
	public static Length Meters(this float value) => Length.FromMeters(value);

	/// <summary>Creates a Length from a value in kilometers.</summary>
	public static Length Kilometers(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Kilometer));

	/// <summary>Creates a Length from a value in centimeters.</summary>
	public static Length Centimeters(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Centimeter));

	/// <summary>Creates a Length from a value in millimeters.</summary>
	public static Length Millimeters(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Millimeter));

	/// <summary>Creates a Length from a value in inches.</summary>
	public static Length Inches(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Inch));

	/// <summary>Creates a Length from a value in feet.</summary>
	public static Length Feet(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Foot));

	/// <summary>Creates a Length from a value in yards.</summary>
	public static Length Yards(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Yard));

	/// <summary>Creates a Length from a value in miles.</summary>
	public static Length Miles(this float value) => Length.FromMeters(ConvertToBaseUnit(value, Units.Mile));

	#endregion

	#region Mass Extensions

	/// <summary>Creates a Mass from a value in kilograms (base unit).</summary>
	public static Mass Kilograms(this float value) => Mass.FromKilograms(value);

	/// <summary>Creates a Mass from a value in grams.</summary>
	public static Mass Grams(this float value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Gram));

	/// <summary>Creates a Mass from a value in pounds.</summary>
	public static Mass Pounds(this float value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Pound));

	/// <summary>Creates a Mass from a value in ounces.</summary>
	public static Mass Ounces(this float value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Ounce));

	/// <summary>Creates a Mass from a value in stones.</summary>
	public static Mass Stones(this float value) => Mass.FromKilograms(ConvertToBaseUnit(value, Units.Stone));

	#endregion

	#region Time Extensions

	/// <summary>Creates a Time from a value in seconds (base unit).</summary>
	public static Time Seconds(this float value) => Time.FromSeconds(value);

	/// <summary>Creates a Time from a value in milliseconds.</summary>
	public static Time Milliseconds(this float value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Millisecond));

	/// <summary>Creates a Time from a value in minutes.</summary>
	public static Time Minutes(this float value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Minute));

	/// <summary>Creates a Time from a value in hours.</summary>
	public static Time Hours(this float value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Hour));

	/// <summary>Creates a Time from a value in days.</summary>
	public static Time Days(this float value) => Time.FromSeconds(ConvertToBaseUnit(value, Units.Day));

	#endregion

	#region Area Extensions

	/// <summary>Creates an Area from a value in square meters (base unit).</summary>
	public static Area SquareMeters(this float value) => Area.FromSquareMeters(value);

	/// <summary>Creates an Area from a value in square kilometers.</summary>
	public static Area SquareKilometers(this float value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareKilometer));

	/// <summary>Creates an Area from a value in square centimeters.</summary>
	public static Area SquareCentimeters(this float value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareCentimeter));

	/// <summary>Creates an Area from a value in square feet.</summary>
	public static Area SquareFeet(this float value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.SquareFoot));

	/// <summary>Creates an Area from a value in acres.</summary>
	public static Area Acres(this float value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.Acre));

	/// <summary>Creates an Area from a value in hectares.</summary>
	public static Area Hectares(this float value) => Area.FromSquareMeters(ConvertToBaseUnit(value, Units.Hectare));

	#endregion

	#region Volume Extensions

	/// <summary>Creates a Volume from a value in cubic meters (base unit).</summary>
	public static Volume CubicMeters(this float value) => Volume.FromCubicMeters(value);

	/// <summary>Creates a Volume from a value in liters.</summary>
	public static Volume Liters(this float value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.Liter));

	/// <summary>Creates a Volume from a value in milliliters.</summary>
	public static Volume Milliliters(this float value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.Milliliter));

	/// <summary>Creates a Volume from a value in cubic feet.</summary>
	public static Volume CubicFeet(this float value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.CubicFoot));

	/// <summary>Creates a Volume from a value in US gallons.</summary>
	public static Volume USGallons(this float value) => Volume.FromCubicMeters(ConvertToBaseUnit(value, Units.USGallon));

	#endregion

	#region Basic Quantity Extensions

	/// <summary>Creates an Energy from a value in joules (base unit).</summary>
	public static Energy Joules(this float value) => Energy.FromJoules(value);

	/// <summary>Creates a Power from a value in watts (base unit).</summary>
	public static Power Watts(this float value) => Power.FromWatts(value);

	/// <summary>Creates a Force from a value in newtons (base unit).</summary>
	public static Force Newtons(this float value) => Force.FromNewtons(value);

	/// <summary>Creates a Pressure from a value in pascals (base unit).</summary>
	public static Pressure Pascals(this float value) => Pressure.FromPascals(value);

	/// <summary>Creates a Pressure from a value in atmospheres.</summary>
	public static Pressure Atmospheres(this float value) => Pressure.FromPascals(ConvertToBaseUnit(value, Units.Atmosphere));

	/// <summary>Creates a Pressure from a value in PSI.</summary>
	public static Pressure PSI(this float value) => Pressure.FromPascals(ConvertToBaseUnit(value, Units.PSI));

	/// <summary>Creates a Velocity from a value in meters per second (base unit).</summary>
	public static Velocity MetersPerSecond(this float value) => Velocity.FromMetersPerSecond(value);

	/// <summary>Creates an Acceleration from a value in meters per second squared (base unit).</summary>
	public static Acceleration MetersPerSecondSquared(this float value) => Acceleration.FromMetersPerSecondSquared(value);

	/// <summary>Creates a Temperature from a value in Kelvin (base unit).</summary>
	public static Temperature Kelvin(this float value) => Temperature.FromKelvin(value);

	/// <summary>Creates a Frequency from a value in hertz (base unit).</summary>
	public static Frequency Hertz(this float value) => Frequency.FromHertz(value);

	#endregion

	#region Vector Quantities - 2D

	/// <summary>Creates a Position2D from X and Y coordinates in meters.</summary>
	public static Position2D MetersPosition2D(this (float x, float y) coordinates)
		=> Position2D.FromMeters(coordinates.x, coordinates.y);

	/// <summary>Creates a Displacement2D from X and Y components in meters.</summary>
	public static Displacement2D MetersDisplacement2D(this (float x, float y) components)
		=> Displacement2D.FromMeters(components.x, components.y);

	/// <summary>Creates a Velocity2D from X and Y components in meters per second.</summary>
	public static Velocity2D MetersPerSecondVelocity2D(this (float x, float y) components)
		=> Velocity2D.FromMetersPerSecond(components.x, components.y);

	/// <summary>Creates an Acceleration2D from X and Y components in meters per second squared.</summary>
	public static Acceleration2D MetersPerSecondSquaredAcceleration2D(this (float x, float y) components)
		=> Acceleration2D.FromMetersPerSecondSquared(components.x, components.y);

	/// <summary>Creates a Force2D from X and Y components in newtons.</summary>
	public static Force2D NewtonsForce2D(this (float x, float y) components)
		=> Force2D.FromNewtons(components.x, components.y);

	#endregion

	#region Vector Quantities - 3D

	/// <summary>Creates a Position3D from X, Y, and Z coordinates in meters.</summary>
	public static Position3D MetersPosition3D(this (float x, float y, float z) coordinates)
		=> Position3D.FromMeters(coordinates.x, coordinates.y, coordinates.z);

	/// <summary>Creates a Displacement3D from X, Y, and Z components in meters.</summary>
	public static Displacement3D MetersDisplacement3D(this (float x, float y, float z) components)
		=> Displacement3D.FromMeters(components.x, components.y, components.z);

	/// <summary>Creates a Velocity3D from X, Y, and Z components in meters per second.</summary>
	public static Velocity3D MetersPerSecondVelocity3D(this (float x, float y, float z) components)
		=> Velocity3D.FromMetersPerSecond(components.x, components.y, components.z);

	/// <summary>Creates an Acceleration3D from X, Y, and Z components in meters per second squared.</summary>
	public static Acceleration3D MetersPerSecondSquaredAcceleration3D(this (float x, float y, float z) components)
		=> Acceleration3D.FromMetersPerSecondSquared(components.x, components.y, components.z);

	/// <summary>Creates a Force3D from X, Y, and Z components in newtons.</summary>
	public static Force3D NewtonsForce3D(this (float x, float y, float z) components)
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
