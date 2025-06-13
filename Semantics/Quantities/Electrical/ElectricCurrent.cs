// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric current quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCurrent<T> : PhysicalQuantity<ElectricCurrent<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCurrent;

	/// <summary>
	/// Initializes a new instance of the ElectricCurrent class.
	/// </summary>
	public ElectricCurrent() : base() { }

	/// <summary>
	/// Creates a new ElectricCurrent from a value in amperes.
	/// </summary>
	/// <param name="amperes">The value in amperes.</param>
	/// <returns>A new ElectricCurrent instance.</returns>
	public static ElectricCurrent<T> FromAmperes(T amperes) => Create(amperes);

	/// <summary>
	/// Calculates electric charge from current and time (Q = I×t).
	/// </summary>
	/// <param name="current">The electric current.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting electric charge.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricCharge<T> operator *(ElectricCurrent<T> current, Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(current);
		ArgumentNullException.ThrowIfNull(time);

		T chargeValue = current.Value * time.Value;

		return ElectricCharge<T>.Create(chargeValue);
	}

	/// <summary>
	/// Calculates electric charge from time and current (Q = I×t).
	/// </summary>
	/// <param name="time">The time duration.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric charge.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricCharge<T> operator *(Time<T> time, ElectricCurrent<T> current) => current * time;

	/// <summary>
	/// Calculates electric potential from current and resistance using Ohm's law (V = I×R).
	/// </summary>
	/// <param name="current">The electric current.</param>
	/// <param name="resistance">The electric resistance.</param>
	/// <returns>The resulting electric potential.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricPotential<T> operator *(ElectricCurrent<T> current, ElectricResistance<T> resistance)
	{
		ArgumentNullException.ThrowIfNull(current);
		ArgumentNullException.ThrowIfNull(resistance);

		T voltageValue = current.Value * resistance.Value;

		return ElectricPotential<T>.Create(voltageValue);
	}

	/// <summary>
	/// Calculates resistance from current and voltage (R = V/I).
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric resistance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricResistance<T> operator /(ElectricPotential<T> voltage, ElectricCurrent<T> current)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(current);

		T resistanceValue = voltage.Value / current.Value;

		return ElectricResistance<T>.Create(resistanceValue);
	}
}
