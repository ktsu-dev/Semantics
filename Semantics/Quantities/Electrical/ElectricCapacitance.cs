// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric capacitance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCapacitance<T> : PhysicalQuantity<ElectricCapacitance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electriccapacitance [M⁻¹ L⁻² T⁴ I²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCapacitance;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCapacitance{T}"/> class.
	/// </summary>
	public ElectricCapacitance() : base() { }

	/// <summary>
	/// Creates a new ElectricCapacitance from a value in farads.
	/// </summary>
	/// <param name="farads">The value in farads.</param>
	/// <returns>A new ElectricCapacitance instance.</returns>
	public static ElectricCapacitance<T> FromFarads(T farads) => Create(farads);

	/// <summary>
	/// Calculates charge from capacitance and voltage (Q = C×V).
	/// </summary>
	/// <param name="capacitance">The capacitance.</param>
	/// <param name="voltage">The voltage.</param>
	/// <returns>The resulting charge.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricCharge<T> operator *(ElectricCapacitance<T> capacitance, ElectricPotential<T> voltage)
	{
		ArgumentNullException.ThrowIfNull(capacitance);
		ArgumentNullException.ThrowIfNull(voltage);

		T chargeValue = capacitance.Value * voltage.Value;

		return ElectricCharge<T>.Create(chargeValue);
	}

	/// <summary>
	/// Calculates voltage from capacitance and charge (V = Q/C).
	/// </summary>
	/// <param name="charge">The charge.</param>
	/// <param name="capacitance">The capacitance.</param>
	/// <returns>The resulting voltage.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricPotential<T> operator /(ElectricCharge<T> charge, ElectricCapacitance<T> capacitance)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(capacitance);

		T voltageValue = charge.Value / capacitance.Value;

		return ElectricPotential<T>.Create(voltageValue);
	}
}
