// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Provides fundamental physical constants used throughout the Semantics library.
/// All values are based on the 2019 redefinition of SI base units and CODATA 2018 values.
/// </summary>
public static class PhysicalConstants
{
	/// <summary>
	/// Fundamental constants
	/// </summary>
	public static class Fundamental
	{
		/// <summary>Avogadro's number: 6.02214076 × 10²³ entities/mol (exact, SI defining constant)</summary>
		public const double AvogadroNumber = 6.02214076e23;

		/// <summary>Gas constant: 8.31446261815324 J/(mol·K) (exact, derived from Avogadro and Boltzmann constants)</summary>
		public const double GasConstant = 8.31446261815324;

		/// <summary>Elementary charge: 1.602176634 × 10⁻¹⁹ C (exact, SI defining constant)</summary>
		public const double ElementaryCharge = 1.602176634e-19;

		/// <summary>Speed of light in vacuum: 299,792,458 m/s (exact, SI defining constant)</summary>
		public const double SpeedOfLight = 299792458.0;

		/// <summary>Planck constant: 6.62607015 × 10⁻³⁴ J·s (exact, SI defining constant)</summary>
		public const double PlanckConstant = 6.62607015e-34;

		/// <summary>Boltzmann constant: 1.380649 × 10⁻²³ J/K (exact, SI defining constant)</summary>
		public const double BoltzmannConstant = 1.380649e-23;
	}

	/// <summary>
	/// Temperature-related constants
	/// </summary>
	public static class Temperature
	{
		/// <summary>Absolute zero in Celsius: 273.15 K (exact by definition)</summary>
		public const double AbsoluteZeroInCelsius = 273.15;

		/// <summary>Water triple point: 273.16 K (exact by definition)</summary>
		public const double WaterTriplePoint = 273.16;

		/// <summary>Standard temperature (STP): 273.15 K (0°C)</summary>
		public const double StandardTemperature = 273.15;

		/// <summary>Water boiling point at 1 atm: 373.15 K (100°C)</summary>
		public const double WaterBoilingPoint = 373.15;
	}

	/// <summary>
	/// Chemical constants
	/// </summary>
	public static class Chemical
	{
		/// <summary>Water ion product (Kw) at 25°C: 1.0 × 10⁻¹⁴ (pKw = 14.0)</summary>
		public const double WaterIonProduct = 14.0; // pKw value

		/// <summary>Molar volume of ideal gas at STP: 22.413969545014137 L/mol (calculated from R*T/P at 273.15K, 101325Pa)</summary>
		public const double MolarVolumeSTP = 22.413969545014137;

		/// <summary>Natural logarithm of 2: 0.6931471805599453</summary>
		public const double Ln2 = 0.6931471805599453;

		/// <summary>Neutral pH value at 25°C: 7.0</summary>
		public const double NeutralPH = 7.0;
	}

	/// <summary>
	/// Mechanical constants
	/// </summary>
	public static class Mechanical
	{
		/// <summary>Standard gravitational acceleration: 9.80665 m/s² (exact by definition)</summary>
		public const double StandardGravity = 9.80665;

		/// <summary>Standard atmospheric pressure: 101,325 Pa (exact by definition)</summary>
		public const double StandardAtmosphericPressure = 101325.0;

		/// <summary>Pound mass to kilogram: 0.453592 kg/lb (exact)</summary>
		public const double PoundMassToKilogram = 0.453592;
	}

	/// <summary>
	/// Conversion factors for unit systems
	/// </summary>
	public static class Conversion
	{
		/// <summary>Celsius to Fahrenheit slope: 9/5 = 1.8</summary>
		public const double CelsiusToFahrenheitSlope = 1.8;

		/// <summary>Fahrenheit to Celsius slope: 5/9 ≈ 0.5555555555555556</summary>
		public const double FahrenheitToCelsiusSlope = 5.0 / 9.0;

		/// <summary>Fahrenheit offset: 32°F</summary>
		public const double FahrenheitOffset = 32.0;

		/// <summary>Calorie to Joule: 4.184 J/cal (thermochemical calorie)</summary>
		public const double CalorieToJoule = 4.184;

		/// <summary>BTU to Joule: 1055.05585262 J/BTU (International Table BTU)</summary>
		public const double BtuToJoule = 1055.05585262;

		/// <summary>Feet to meters: 0.3048 m/ft (exact)</summary>
		public const double FeetToMeters = 0.3048;
	}

	/// <summary>
	/// Acoustic constants
	/// </summary>
	public static class Acoustic
	{
		/// <summary>Reference sound pressure: 20 × 10⁻⁶ Pa (threshold of hearing)</summary>
		public const double ReferenceSoundPressure = 20e-6;

		/// <summary>Reference sound intensity: 1 × 10⁻¹² W/m² (threshold of hearing)</summary>
		public const double ReferenceSoundIntensity = 1e-12;

		/// <summary>Reference sound power: 1 × 10⁻¹² W</summary>
		public const double ReferenceSoundPower = 1e-12;

		/// <summary>Sabine reverberation constant: 0.161 m/s</summary>
		public const double SabineConstant = 0.161;
	}

	/// <summary>
	/// Mathematical constants commonly used in physics
	/// </summary>
	public static class Mathematical
	{
		/// <summary>Natural logarithm of 10: 2.302585092994046</summary>
		public const double Ln10 = 2.302585092994046;

		/// <summary>Base-10 logarithm of e: 0.4342944819032518</summary>
		public const double Log10E = 0.4342944819032518;
	}

	/// <summary>
	/// Helper methods to get constants as generic numeric types
	/// </summary>
	public static class Generic
	{
		/// <summary>Gets Avogadro's number as type T</summary>
		public static T AvogadroNumber<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.AvogadroNumber);

		/// <summary>Gets gas constant as type T</summary>
		public static T GasConstant<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.GasConstant);

		/// <summary>Gets absolute zero in Celsius as type T</summary>
		public static T AbsoluteZeroInCelsius<T>() where T : struct, INumber<T> => T.CreateChecked(Temperature.AbsoluteZeroInCelsius);

		/// <summary>Gets water ion product (pKw) as type T</summary>
		public static T WaterIonProduct<T>() where T : struct, INumber<T> => T.CreateChecked(Chemical.WaterIonProduct);

		/// <summary>Gets molar volume at STP as type T</summary>
		public static T MolarVolumeSTP<T>() where T : struct, INumber<T> => T.CreateChecked(Chemical.MolarVolumeSTP);

		/// <summary>Gets natural logarithm of 2 as type T</summary>
		public static T Ln2<T>() where T : struct, INumber<T> => T.CreateChecked(Chemical.Ln2);

		/// <summary>Gets neutral pH as type T</summary>
		public static T NeutralPH<T>() where T : struct, INumber<T> => T.CreateChecked(Chemical.NeutralPH);

		/// <summary>Gets standard gravitational acceleration as type T</summary>
		public static T StandardGravity<T>() where T : struct, INumber<T> => T.CreateChecked(Mechanical.StandardGravity);

		/// <summary>Gets standard atmospheric pressure as type T</summary>
		public static T StandardAtmosphericPressure<T>() where T : struct, INumber<T> => T.CreateChecked(Mechanical.StandardAtmosphericPressure);

		/// <summary>Gets pound mass to kilogram conversion as type T</summary>
		public static T PoundMassToKilogram<T>() where T : struct, INumber<T> => T.CreateChecked(Mechanical.PoundMassToKilogram);
	}
}
