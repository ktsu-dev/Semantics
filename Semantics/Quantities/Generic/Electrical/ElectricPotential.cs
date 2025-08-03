// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents an electric potential quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record ElectricPotential<T> : PhysicalQuantity<ElectricPotential<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricpotential [M L² T⁻³ I⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricPotential;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricPotential{T}"/> class.
	/// </summary>
	public ElectricPotential() : base() { }

	/// <summary>
	/// Creates a new ElectricPotential from a value in volts.
	/// </summary>
	/// <param name="volts">The value in volts.</param>
	/// <returns>A new ElectricPotential instance.</returns>
	public static ElectricPotential<T> FromVolts(T volts) => Create(volts);

	/// <summary>
	/// Calculates power from voltage and current (P = V×I).
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electrical power.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Power<T> operator *(ElectricPotential<T> voltage, ElectricCurrent<T> current)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(current);

		T powerValue = voltage.Value * current.Value;

		return Power<T>.Create(powerValue);
	}

	/// <summary>
	/// Calculates current from voltage and resistance (I = V/R).
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="resistance">The electric resistance.</param>
	/// <returns>The resulting electric current.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricCurrent<T> operator /(ElectricPotential<T> voltage, ElectricResistance<T> resistance)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(resistance);

		T currentValue = voltage.Value / resistance.Value;

		return ElectricCurrent<T>.Create(currentValue);
	}

	/// <summary>
	/// Calculates electric field from voltage and distance (E = V/d).
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="distance">The distance.</param>
	/// <returns>The resulting electric field.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricField<T> operator /(ElectricPotential<T> voltage, Length<T> distance)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(distance);

		T fieldValue = voltage.Value / distance.Value;

		return ElectricField<T>.Create(fieldValue);
	}
}
