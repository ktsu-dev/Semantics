// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Represents acidity on the pH scale: <c>pH = −log10([H⁺])</c> with the
/// hydrogen-ion activity in mol/L.
/// </summary>
/// <remarks>
/// pH is a logarithmic scale, so — like the decibel quantities — it is a
/// hand-written companion that converts to and from the linear
/// <see cref="Concentration{T}"/> quantity (stored in the SI base mol/m³).
/// </remarks>
/// <typeparam name="T">The floating-point storage type.</typeparam>
/// <param name="Value">The pH value.</param>
public readonly record struct PH<T>(T Value) : IComparable<PH<T>>
	where T : struct, INumber<T>
{
	private const double MolesPerCubicMeterPerMolar = 1000.0;

	/// <summary>Gets the pH of pure water at 25 °C (7.0).</summary>
	public static PH<T> Neutral => new(PhysicalConstants.Generic.NeutralPH<T>());

	/// <summary>
	/// Creates a pH from a raw scale value.
	/// </summary>
	/// <param name="value">The pH value.</param>
	/// <returns>A new <see cref="PH{T}"/>.</returns>
	public static PH<T> Create(T value) => new(value);

	/// <summary>
	/// Creates a pH from a hydrogen-ion concentration using <c>pH = −log10([H⁺] in mol/L)</c>.
	/// </summary>
	/// <param name="hydrogenConcentration">The hydrogen-ion concentration.</param>
	/// <returns>A new <see cref="PH{T}"/>.</returns>
	public static PH<T> FromHydrogenConcentration(Concentration<T> hydrogenConcentration)
	{
		ArgumentNullException.ThrowIfNull(hydrogenConcentration);
		double molesPerLiter = double.CreateChecked(hydrogenConcentration.Value) / MolesPerCubicMeterPerMolar;
		return new(T.CreateChecked(-Math.Log10(molesPerLiter)));
	}

	/// <summary>
	/// Converts this pH to the equivalent hydrogen-ion concentration using <c>[H⁺] = 10^(−pH) mol/L</c>.
	/// </summary>
	/// <returns>The hydrogen-ion <see cref="Concentration{T}"/>.</returns>
	public Concentration<T> ToHydrogenConcentration()
	{
		double ph = double.CreateChecked(Value);
		double molesPerCubicMeter = Math.Pow(10.0, -ph) * MolesPerCubicMeterPerMolar;
		return Concentration<T>.Create(T.CreateChecked(molesPerCubicMeter));
	}

	/// <summary>
	/// Gets the complementary pOH at 25 °C using <c>pOH = 14 − pH</c>.
	/// </summary>
	/// <returns>The pOH as a <see cref="PH{T}"/> scale value.</returns>
	public PH<T> ToPOH() => new(T.CreateChecked(14) - Value);

	/// <summary>Gets whether this value describes an acid (pH below 7).</summary>
	public bool IsAcidic => Value < PhysicalConstants.Generic.NeutralPH<T>();

	/// <summary>Gets whether this value describes a base (pH above 7).</summary>
	public bool IsBasic => Value > PhysicalConstants.Generic.NeutralPH<T>();

	/// <inheritdoc/>
	public int CompareTo(PH<T> other) => Value.CompareTo(other.Value);

	/// <summary>Determines whether one pH is less than another.</summary>
	/// <param name="left">The left pH.</param>
	/// <param name="right">The right pH.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
	public static bool operator <(PH<T> left, PH<T> right) => left.CompareTo(right) < 0;

	/// <summary>Determines whether one pH is greater than another.</summary>
	/// <param name="left">The left pH.</param>
	/// <param name="right">The right pH.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
	public static bool operator >(PH<T> left, PH<T> right) => left.CompareTo(right) > 0;

	/// <summary>Determines whether one pH is less than or equal to another.</summary>
	/// <param name="left">The left pH.</param>
	/// <param name="right">The right pH.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
	public static bool operator <=(PH<T> left, PH<T> right) => left.CompareTo(right) <= 0;

	/// <summary>Determines whether one pH is greater than or equal to another.</summary>
	/// <param name="left">The left pH.</param>
	/// <param name="right">The right pH.</param>
	/// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
	public static bool operator >=(PH<T> left, PH<T> right) => left.CompareTo(right) >= 0;

	/// <summary>Returns a culture-invariant string representation of this pH.</summary>
	/// <returns>The value formatted with a <c>pH </c> prefix.</returns>
	public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"pH {Value}");
}
