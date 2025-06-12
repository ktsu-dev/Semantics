// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;
/// <summary>
/// Represents a physical dimension with powers of fundamental SI base quantities.
/// Provides compile-time dimensional safety and runtime dimensional analysis.
/// </summary>
/// <remarks>
/// Initializes a new instance of the PhysicalDimension struct.
/// </remarks>
/// <param name="length">Power of length dimension.</param>
/// <param name="mass">Power of mass dimension.</param>
/// <param name="time">Power of time dimension.</param>
/// <param name="electricCurrent">Power of electric current dimension.</param>
/// <param name="temperature">Power of temperature dimension.</param>
/// <param name="amountOfSubstance">Power of amount of substance dimension.</param>
/// <param name="luminousIntensity">Power of luminous intensity dimension.</param>
/// <param name="baseUnit">The SI base unit for this dimension.</param>
public readonly struct PhysicalDimension(
	int length = 0,
	int mass = 0,
	int time = 0,
	int electricCurrent = 0,
	int temperature = 0,
	int amountOfSubstance = 0,
	int luminousIntensity = 0,
	IUnit? baseUnit = null) : IEquatable<PhysicalDimension>
{
	/// <summary>The power of Length [L] in this dimension.</summary>
	public int Length { get; } = length;

	/// <summary>The power of Mass [M] in this dimension.</summary>
	public int Mass { get; } = mass;

	/// <summary>The power of Time [T] in this dimension.</summary>
	public int Time { get; } = time;

	/// <summary>The power of Electric Current [I] in this dimension.</summary>
	public int ElectricCurrent { get; } = electricCurrent;

	/// <summary>The power of Thermodynamic Temperature [Θ] in this dimension.</summary>
	public int Temperature { get; } = temperature;

	/// <summary>The power of Amount of Substance [N] in this dimension.</summary>
	public int AmountOfSubstance { get; } = amountOfSubstance;

	/// <summary>The power of Luminous Intensity [J] in this dimension.</summary>
	public int LuminousIntensity { get; } = luminousIntensity;

	/// <summary>Gets whether this dimension is dimensionless (all powers are zero).</summary>
	public bool IsDimensionless => Length == 0 && Mass == 0 && Time == 0 && ElectricCurrent == 0 && Temperature == 0 && AmountOfSubstance == 0 && LuminousIntensity == 0;

	/// <summary>Gets the SI base unit for this dimension.</summary>
	public IUnit BaseUnit { get; } = baseUnit ?? throw new InvalidOperationException("Base unit must be specified for physical dimensions.");

	/// <summary>
	/// Determines whether this dimension is equal to another dimension.
	/// </summary>
	/// <param name="other">The other dimension to compare.</param>
	/// <returns>True if the dimensions are equal, false otherwise.</returns>
	public bool Equals(PhysicalDimension other) =>
		Length == other.Length &&
		Mass == other.Mass &&
		Time == other.Time &&
		ElectricCurrent == other.ElectricCurrent &&
		Temperature == other.Temperature &&
		AmountOfSubstance == other.AmountOfSubstance &&
		LuminousIntensity == other.LuminousIntensity;

	/// <summary>
	/// Determines whether this dimension is equal to another object.
	/// </summary>
	/// <param name="obj">The object to compare.</param>
	/// <returns>True if the objects are equal, false otherwise.</returns>
	public override bool Equals(object? obj) => obj is PhysicalDimension other && Equals(other);

	/// <summary>
	/// Gets the hash code for this dimension.
	/// </summary>
	/// <returns>A hash code for this dimension.</returns>
	public override int GetHashCode() => HashCode.Combine(Length, Mass, Time, ElectricCurrent, Temperature, AmountOfSubstance, LuminousIntensity);

	/// <summary>
	/// Determines whether two dimensions are equal.
	/// </summary>
	/// <param name="left">The first dimension.</param>
	/// <param name="right">The second dimension.</param>
	/// <returns>True if the dimensions are equal, false otherwise.</returns>
	public static bool operator ==(PhysicalDimension left, PhysicalDimension right) => left.Equals(right);

	/// <summary>
	/// Determines whether two dimensions are not equal.
	/// </summary>
	/// <param name="left">The first dimension.</param>
	/// <param name="right">The second dimension.</param>
	/// <returns>True if the dimensions are not equal, false otherwise.</returns>
	public static bool operator !=(PhysicalDimension left, PhysicalDimension right) => !left.Equals(right);

	/// <summary>
	/// Multiplies two dimensions (adds their powers).
	/// </summary>
	/// <param name="left">The first dimension.</param>
	/// <param name="right">The second dimension.</param>
	/// <returns>The product dimension.</returns>
	public static PhysicalDimension operator *(PhysicalDimension left, PhysicalDimension right) =>
		new(left.Length + right.Length,
			left.Mass + right.Mass,
			left.Time + right.Time,
			left.ElectricCurrent + right.ElectricCurrent,
			left.Temperature + right.Temperature,
			left.AmountOfSubstance + right.AmountOfSubstance,
			left.LuminousIntensity + right.LuminousIntensity);

	/// <summary>
	/// Divides two dimensions (subtracts their powers).
	/// </summary>
	/// <param name="left">The numerator dimension.</param>
	/// <param name="right">The denominator dimension.</param>
	/// <returns>The quotient dimension.</returns>
	public static PhysicalDimension operator /(PhysicalDimension left, PhysicalDimension right) =>
		new(left.Length - right.Length,
			left.Mass - right.Mass,
			left.Time - right.Time,
			left.ElectricCurrent - right.ElectricCurrent,
			left.Temperature - right.Temperature,
			left.AmountOfSubstance - right.AmountOfSubstance,
			left.LuminousIntensity - right.LuminousIntensity);

	/// <summary>
	/// Raises a dimension to a power.
	/// </summary>
	/// <param name="dimension">The dimension to raise.</param>
	/// <param name="power">The power to raise to.</param>
	/// <returns>The dimension raised to the power.</returns>
	public static PhysicalDimension Pow(PhysicalDimension dimension, int power) =>
		new(dimension.Length * power,
			dimension.Mass * power,
			dimension.Time * power,
			dimension.ElectricCurrent * power,
			dimension.Temperature * power,
			dimension.AmountOfSubstance * power,
			dimension.LuminousIntensity * power);

	/// <summary>
	/// Returns a string representation of this dimension using standard notation.
	/// </summary>
	/// <returns>A string like "L² M T⁻¹" or "1" for dimensionless.</returns>
	public override string ToString()
	{
		if (IsDimensionless)
		{
			return "1";
		}

		List<string> parts = [];

		if (Length != 0)
		{
			parts.Add(FormatDimension("L", Length));
		}

		if (Mass != 0)
		{
			parts.Add(FormatDimension("M", Mass));
		}

		if (Time != 0)
		{
			parts.Add(FormatDimension("T", Time));
		}

		if (ElectricCurrent != 0)
		{
			parts.Add(FormatDimension("I", ElectricCurrent));
		}

		if (Temperature != 0)
		{
			parts.Add(FormatDimension("Θ", Temperature));
		}

		if (AmountOfSubstance != 0)
		{
			parts.Add(FormatDimension("N", AmountOfSubstance));
		}

		if (LuminousIntensity != 0)
		{
			parts.Add(FormatDimension("J", LuminousIntensity));
		}

		return string.Join(" ", parts);
	}

	/// <summary>
	/// Formats a single dimension with its power using Unicode superscript.
	/// </summary>
	/// <param name="symbol">The dimension symbol.</param>
	/// <param name="power">The power of the dimension.</param>
	/// <returns>Formatted dimension string.</returns>
	private static string FormatDimension(string symbol, int power)
	{
		return power switch
		{
			1 => symbol,
			-1 => symbol + "⁻¹",
			var p when p > 0 => symbol + ToSuperscript(p),
			var p => symbol + "⁻" + ToSuperscript(-p),
		};
	}

	/// <summary>
	/// Converts a positive integer to Unicode superscript characters.
	/// </summary>
	/// <param name="number">The number to convert.</param>
	/// <returns>The superscript representation.</returns>
	private static string ToSuperscript(int number)
	{
		Dictionary<char, char> superscriptMap = new()
		{
			['0'] = '⁰',
			['1'] = '¹',
			['2'] = '²',
			['3'] = '³',
			['4'] = '⁴',
			['5'] = '⁵',
			['6'] = '⁶',
			['7'] = '⁷',
			['8'] = '⁸',
			['9'] = '⁹'
		};

		return new string([.. number.ToString().Select(c => superscriptMap[c])]);
	}

	/// <inheritdoc/>
	public static PhysicalDimension Multiply(PhysicalDimension left, PhysicalDimension right) => throw new NotImplementedException();
	/// <inheritdoc/>

	public static PhysicalDimension Divide(PhysicalDimension left, PhysicalDimension right) => throw new NotImplementedException();
}
