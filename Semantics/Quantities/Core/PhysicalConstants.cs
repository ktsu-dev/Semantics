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
	/// Optical constants
	/// </summary>
	public static class Optical
	{
		/// <summary>Luminous efficacy of monochromatic radiation at 540 THz: 683 lm/W (exact by definition)</summary>
		public const double LuminousEfficacy = 683.0;
	}

	/// <summary>
	/// Nuclear constants
	/// </summary>
	public static class Nuclear
	{
		/// <summary>Atomic mass unit: 1.66053906660×10⁻²⁷ kg (2018 CODATA)</summary>
		public const double AtomicMassUnit = 1.66053906660e-27;

		/// <summary>Nuclear magneton: 5.0507837461×10⁻²⁷ J/T (2018 CODATA)</summary>
		public const double NuclearMagneton = 5.0507837461e-27;
	}

	/// <summary>
	/// Fluid dynamics constants
	/// </summary>
	public static class FluidDynamics
	{
		/// <summary>Standard air density at 15°C and 1 atm: 1.225 kg/m³ (ISO 2533)</summary>
		public const double StandardAirDensity = 1.225;

		/// <summary>Water surface tension at 20°C: 0.0728 N/m (NIST)</summary>
		public const double WaterSurfaceTension = 0.0728;
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

		/// <summary>Kilowatt-hour to Joule: 3,600,000 J/kWh (exact)</summary>
		public const double KilowattHourToJoule = 3600000.0;

		/// <summary>BTU per hour-foot-Fahrenheit to Watts per meter-Kelvin: 1.7307 W/(m·K) per BTU/(h·ft·°F)</summary>
		public const double BtuPerHourFootFahrenheitToWattsPerMeterKelvin = 1.7307;

		/// <summary>Thermal resistance: Fahrenheit-hour per BTU to Kelvin per Watt: 1.8956 K/W per °F·h/BTU</summary>
		public const double FahrenheitHourPerBtuToKelvinPerWatt = 1.8956;

		/// <summary>Specific heat: BTU per pound-Fahrenheit to Joules per kilogram-Kelvin: 4186.8 J/(kg·K) per BTU/(lb·°F)</summary>
		public const double BtuPerPoundFahrenheitToJoulesPerKilogramKelvin = 4186.8;

		/// <summary>Heat transfer coefficient: BTU per hour-square foot-Fahrenheit to Watts per square meter-Kelvin: 5.678 W/(m²·K) per BTU/(h·ft²·°F)</summary>
		public const double BtuPerHourSquareFootFahrenheitToWattsPerSquareMeterKelvin = 5.678;

		/// <summary>Heat capacity: BTU per Fahrenheit to Joules per Kelvin: 1899.1 J/K per BTU/°F</summary>
		public const double BtuPerFahrenheitToJoulesPerKelvin = 1899.1;

		/// <summary>Illuminance: foot-candles to lux: 10.764 lux per fc</summary>
		public const double FootCandlesToLux = 10.764;

		/// <summary>Luminance: foot-lamberts to candelas per square meter: 3.426 cd/m² per fL</summary>
		public const double FootLambertsToCandelasPerSquareMeter = 3.426;

		/// <summary>Absorbed dose: rads to grays: 0.01 Gy per rad</summary>
		public const double RadsToGrays = 0.01;

		/// <summary>Equivalent dose: rems to sieverts: 0.01 Sv per rem</summary>
		public const double RemsToSieverts = 0.01;

		/// <summary>Nuclear exposure: roentgens to absorbed dose in air: 0.00876 Gy per R</summary>
		public const double RoentgensToGraysInAir = 0.00876;
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

		/// <summary>One half: 0.5 (commonly used in kinetic energy equations)</summary>
		public const double OneHalf = 0.5;

		/// <summary>Two thirds: 2/3 ≈ 0.6666666666666666 (used in various physics equations)</summary>
		public const double TwoThirds = 2.0 / 3.0;

		/// <summary>Three halves: 3/2 = 1.5 (used in kinetic theory)</summary>
		public const double ThreeHalves = 1.5;

		/// <summary>Four thirds: 4/3 ≈ 1.3333333333333333 (used in sphere volume)</summary>
		public const double FourThirds = 4.0 / 3.0;
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

		/// <summary>Gets elementary charge as type T</summary>
		public static T ElementaryCharge<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.ElementaryCharge);

		/// <summary>Gets Planck constant as type T</summary>
		public static T PlanckConstant<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.PlanckConstant);

		/// <summary>Gets Boltzmann constant as type T</summary>
		public static T BoltzmannConstant<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.BoltzmannConstant);

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

		/// <summary>Gets Celsius to Fahrenheit slope as type T</summary>
		public static T CelsiusToFahrenheitSlope<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.CelsiusToFahrenheitSlope);

		/// <summary>Gets Fahrenheit to Celsius slope as type T</summary>
		public static T FahrenheitToCelsiusSlope<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FahrenheitToCelsiusSlope);

		/// <summary>Gets Fahrenheit offset as type T</summary>
		public static T FahrenheitOffset<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FahrenheitOffset);

		/// <summary>Gets calorie to joule conversion as type T</summary>
		public static T CalorieToJoule<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.CalorieToJoule);

		/// <summary>Gets BTU to joule conversion as type T</summary>
		public static T BtuToJoule<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.BtuToJoule);

		/// <summary>Gets feet to meters conversion as type T</summary>
		public static T FeetToMeters<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FeetToMeters);

		/// <summary>Gets kilowatt-hour to joule conversion as type T</summary>
		public static T KilowattHourToJoule<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.KilowattHourToJoule);

		/// <summary>Gets BTU per hour-foot-Fahrenheit to Watts per meter-Kelvin conversion as type T</summary>
		public static T BtuPerHourFootFahrenheitToWattsPerMeterKelvin<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.BtuPerHourFootFahrenheitToWattsPerMeterKelvin);

		/// <summary>Gets water triple point temperature as type T</summary>
		public static T WaterTriplePoint<T>() where T : struct, INumber<T> => T.CreateChecked(Temperature.WaterTriplePoint);

		/// <summary>Gets standard temperature as type T</summary>
		public static T StandardTemperature<T>() where T : struct, INumber<T> => T.CreateChecked(Temperature.StandardTemperature);

		/// <summary>Gets water boiling point as type T</summary>
		public static T WaterBoilingPoint<T>() where T : struct, INumber<T> => T.CreateChecked(Temperature.WaterBoilingPoint);

		/// <summary>Gets reference sound pressure as type T</summary>
		public static T ReferenceSoundPressure<T>() where T : struct, INumber<T> => T.CreateChecked(Acoustic.ReferenceSoundPressure);

		/// <summary>Gets reference sound intensity as type T</summary>
		public static T ReferenceSoundIntensity<T>() where T : struct, INumber<T> => T.CreateChecked(Acoustic.ReferenceSoundIntensity);

		/// <summary>Gets reference sound power as type T</summary>
		public static T ReferenceSoundPower<T>() where T : struct, INumber<T> => T.CreateChecked(Acoustic.ReferenceSoundPower);

		/// <summary>Gets Sabine reverberation constant as type T</summary>
		public static T SabineConstant<T>() where T : struct, INumber<T> => T.CreateChecked(Acoustic.SabineConstant);

		// === OPTICAL CONSTANTS ===

		/// <summary>Gets the luminous efficacy of monochromatic radiation at 540 THz.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The luminous efficacy (683 lm/W) as type T.</returns>
		public static T LuminousEfficacy<T>() where T : struct, INumber<T> => T.CreateChecked(Optical.LuminousEfficacy);

		/// <summary>Gets the speed of light in vacuum.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The speed of light (299,792,458 m/s) as type T.</returns>
		public static T SpeedOfLight<T>() where T : struct, INumber<T> => T.CreateChecked(Fundamental.SpeedOfLight);

		// === NUCLEAR CONSTANTS ===

		/// <summary>Gets the atomic mass unit.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The atomic mass unit (1.66053906660×10⁻²⁷ kg) as type T.</returns>
		public static T AtomicMassUnit<T>() where T : struct, INumber<T> => T.CreateChecked(Nuclear.AtomicMassUnit);

		/// <summary>Gets the nuclear magneton.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The nuclear magneton (5.0507837461×10⁻²⁷ J/T) as type T.</returns>
		public static T NuclearMagneton<T>() where T : struct, INumber<T> => T.CreateChecked(Nuclear.NuclearMagneton);

		// === FLUID DYNAMICS CONSTANTS ===

		/// <summary>Gets the standard air density.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The standard air density (1.225 kg/m³) as type T.</returns>
		public static T StandardAirDensity<T>() where T : struct, INumber<T> => T.CreateChecked(FluidDynamics.StandardAirDensity);

		/// <summary>Gets the water surface tension at 20°C.</summary>
		/// <typeparam name="T">The numeric type.</typeparam>
		/// <returns>The water surface tension (0.0728 N/m) as type T.</returns>
		public static T WaterSurfaceTension<T>() where T : struct, INumber<T> => T.CreateChecked(FluidDynamics.WaterSurfaceTension);

		/// <summary>Gets natural logarithm of 10 as type T</summary>
		public static T Ln10<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.Ln10);

		/// <summary>Gets base-10 logarithm of e as type T</summary>
		public static T Log10E<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.Log10E);

		/// <summary>Gets one half (0.5) as type T</summary>
		public static T OneHalf<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.OneHalf);

		/// <summary>Gets two thirds (2/3) as type T</summary>
		public static T TwoThirds<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.TwoThirds);

		/// <summary>Gets three halves (3/2) as type T</summary>
		public static T ThreeHalves<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.ThreeHalves);

		/// <summary>Gets four thirds (4/3) as type T</summary>
		public static T FourThirds<T>() where T : struct, INumber<T> => T.CreateChecked(Mathematical.FourThirds);

		// === ADDITIONAL CONVERSION CONSTANTS ===

		/// <summary>Gets thermal resistance conversion factor as type T</summary>
		public static T FahrenheitHourPerBtuToKelvinPerWatt<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FahrenheitHourPerBtuToKelvinPerWatt);

		/// <summary>Gets specific heat conversion factor as type T</summary>
		public static T BtuPerPoundFahrenheitToJoulesPerKilogramKelvin<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.BtuPerPoundFahrenheitToJoulesPerKilogramKelvin);

		/// <summary>Gets heat transfer coefficient conversion factor as type T</summary>
		public static T BtuPerHourSquareFootFahrenheitToWattsPerSquareMeterKelvin<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.BtuPerHourSquareFootFahrenheitToWattsPerSquareMeterKelvin);

		/// <summary>Gets heat capacity conversion factor as type T</summary>
		public static T BtuPerFahrenheitToJoulesPerKelvin<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.BtuPerFahrenheitToJoulesPerKelvin);

		/// <summary>Gets illuminance conversion factor as type T</summary>
		public static T FootCandlesToLux<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FootCandlesToLux);

		/// <summary>Gets luminance conversion factor as type T</summary>
		public static T FootLambertsToCandelasPerSquareMeter<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.FootLambertsToCandelasPerSquareMeter);

		/// <summary>Gets absorbed dose conversion factor as type T</summary>
		public static T RadsToGrays<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.RadsToGrays);

		/// <summary>Gets equivalent dose conversion factor as type T</summary>
		public static T RemsToSieverts<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.RemsToSieverts);

		/// <summary>Gets nuclear exposure conversion factor as type T</summary>
		public static T RoentgensToGraysInAir<T>() where T : struct, INumber<T> => T.CreateChecked(Conversion.RoentgensToGraysInAir);
	}
}
