// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Provides conversion factors for various physical quantities and metric prefixes.
/// </summary>
public static class PhysicalConstants
{
	// Metric prefixes
	/// <inheritdoc/>
	public static double Deca { get; } = 1e1;
	/// <inheritdoc/>
	public static double Hecto { get; } = 1e2;
	/// <inheritdoc/>
	public static double Kilo { get; } = 1e3;
	/// <inheritdoc/>
	public static double Mega { get; } = 1e6;
	/// <inheritdoc/>
	public static double Giga { get; } = 1e9;
	/// <inheritdoc/>
	public static double Tera { get; } = 1e12;
	/// <inheritdoc/>
	public static double Peta { get; } = 1e15;
	/// <inheritdoc/>
	public static double Exa { get; } = 1e18;
	/// <inheritdoc/>
	public static double Zetta { get; } = 1e21;
	/// <inheritdoc/>
	public static double Yotta { get; } = 1e24;
	/// <inheritdoc/>
	public static double Deci { get; } = 1e-1;
	/// <inheritdoc/>
	public static double Centi { get; } = 1e-2;
	/// <inheritdoc/>
	public static double Milli { get; } = 1e-3;
	/// <inheritdoc/>
	public static double Micro { get; } = 1e-6;
	/// <inheritdoc/>
	public static double Nano { get; } = 1e-9;
	/// <inheritdoc/>
	public static double Pico { get; } = 1e-12;
	/// <inheritdoc/>
	public static double Femto { get; } = 1e-15;
	/// <inheritdoc/>
	public static double Atto { get; } = 1e-18;
	/// <inheritdoc/>
	public static double Zepto { get; } = 1e-21;
	/// <inheritdoc/>
	public static double Yocto { get; } = 1e-24;

	// Length conversion factors
	/// <inheritdoc/>
	public static double FeetToMetersFactor { get; } = 0.3048;
	/// <inheritdoc/>
	public static double InchesToMetersFactor { get; } = 0.0254;
	/// <inheritdoc/>
	public static double YardsToMetersFactor { get; } = 0.9144;
	/// <inheritdoc/>
	public static double MilesToMetersFactor { get; } = 1609.344;
	/// <inheritdoc/>
	public static double NauticalMilesToMetersFactor { get; } = 1852;
	/// <inheritdoc/>
	public static double FathomsToMetersFactor { get; } = 1.8288;
	/// <inheritdoc/>
	public static double AstronomicalUnitsToMetersFactor { get; } = 1.495978707e11;
	/// <inheritdoc/>
	public static double LightYearsToMetersFactor { get; } = 9.4607304725808e15;
	/// <inheritdoc/>
	public static double ParsecsToMetersFactor { get; } = 3.08567758149137e16;

	// Angular conversion factors
	/// <inheritdoc/>
	public static double DegreesToRadiansFactor { get; } = 0.01745329251994329576923690768489;
	/// <inheritdoc/>
	public static double GradiansToRadiansFactor { get; } = 0.0157079632679489661923132169164;
	/// <inheritdoc/>
	public static double MinutesToRadiansFactor { get; } = 0.00029088820866572159615394846141459;
	/// <inheritdoc/>
	public static double SecondsToRadiansFactor { get; } = 4.8481368110953599358991410235795e-6;
	/// <inheritdoc/>
	public static double RevolutionsToRadiansFactor { get; } = 6.283185307179586476925286766559;
	/// <inheritdoc/>
	public static double CyclesToRadiansFactor { get; } = 6.283185307179586476925286766559;
	/// <inheritdoc/>
	public static double TurnsToRadiansFactor { get; } = 6.283185307179586476925286766559;

	// Mass conversion factors
	/// <inheritdoc/>
	public static double PoundsToKilogramsFactor { get; } = 0.45359237;
	/// <inheritdoc/>
	public static double OuncesToKilogramsFactor { get; } = 0.028349523125;
	/// <inheritdoc/>
	public static double StonesToKilogramsFactor { get; } = 6.35029318;
	/// <inheritdoc/>
	public static double ImperialTonsToKilogramsFactor { get; } = 1016.0469088;
	/// <inheritdoc/>
	public static double USTonsToKilogramsFactor { get; } = 907.18474;
	/// <inheritdoc/>
	public static double MetricTonsToKilogramsFactor { get; } = 1000;

	// Force conversion factors
	/// <inheritdoc/>
	public static double PoundsForceToNewtonsFactor { get; } = 4.4482216152605;

	// Energy conversion factors
	/// <inheritdoc/>
	public static double CaloriesToJoulesFactor { get; } = 4.184;
	/// <inheritdoc/>
	public static double BTUsToJoulesFactor { get; } = 1055.05585262;

	// Power conversion factors
	/// <inheritdoc/>
	public static double HorsepowerToWattsFactor { get; } = 745.69987158227022;
	/// <inheritdoc/>
	public static double MetricHorsePowerToWattsFactor { get; } = 735.49875;

	// Pressure conversion factors
	/// <inheritdoc/>
	public static double BarToPascalsFactor { get; } = 1e5;
	/// <inheritdoc/>
	public static double PsiToPascalsFactor { get; } = 6894.757293168361;
	/// <inheritdoc/>
	public static double AtmToPascalsFactor { get; } = 101325;
	/// <inheritdoc/>
	public static double TorrToPascalsFactor { get; } = 133.32236842105263;

	// Temperature conversion factors
	/// <inheritdoc/>
	public static double CelsiusToKelvinFactor { get; } = 1;
	/// <inheritdoc/>
	public static double CelsiusToKelvinOffset { get; } = 273.15;
	/// <inheritdoc/>
	public static double FahrenheitToCelsiusFactor { get; } = 9.0 / 5.0;
	/// <inheritdoc/>
	public static double FahrenheitToCelsiusOffset { get; } = 32;

	// Time conversion factors
	/// <inheritdoc/>
	public static double MinutesToSecondsFactor { get; } = 60;
	/// <inheritdoc/>
	public static double HoursToSecondsFactor { get; } = 3600;
	/// <inheritdoc/>
	public static double DaysToSecondsFactor { get; } = 86400;
	/// <inheritdoc/>
	public static double HoursToMinutesFactor { get; } = 60;
	/// <inheritdoc/>
	public static double DaysToMinutesFactor { get; } = 1440;
	/// <inheritdoc/>
	public static double DaysToHoursFactor { get; } = 24;
	/// <inheritdoc/>
	public static double YearsToSecondsFactor { get; } = 31556952;

	// Torque conversion factors
	/// <inheritdoc/>
	public static double FootPoundsToNewtonMetersFactor { get; } = 1.3558179483314004;
	/// <inheritdoc/>
	public static double PoundInchesToNewtonMetersFactor { get; } = 0.1130057188312;

	// Illuminance conversion factors
	/// <inheritdoc/>
	public static double FootCandleToLuxFactor { get; } = 10.763910416709722;

	// Area conversion factors
	/// <inheritdoc/>
	public static double AcresToSquareMetersFactor { get; } = 4046.8564224;

	// Solid Angle conversion factors
	/// <inheritdoc/>
	public static double SquareDegreesToSteradiansFactor { get; } = 3282.80635001;

	// Charge conversion factors
	/// <inheritdoc/>
	public static double MilliampereHoursToCoulombsFactor { get; } = 3.6;
	/// <inheritdoc/>
	public static double AmpereHoursToCoulombsFactor { get; } = 3600;
}
