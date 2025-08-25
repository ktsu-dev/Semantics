// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric resistance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricResistance<T> : PhysicalQuantity<ElectricResistance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricresistance [M L² T⁻³ I⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricResistance;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricResistance{T}"/> class.
	/// </summary>
	public ElectricResistance() : base() { }

	/// <summary>
	/// Creates a new ElectricResistance from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ElectricResistance instance.</returns>
	public static ElectricResistance<T> FromOhms(T ohms) => Create(ohms);

	/// <summary>
	/// Calculates electric potential from resistance and current using Ohm's law (V = I×R).
	/// </summary>
	/// <param name="resistance">The electric resistance.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric potential.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricPotential<T> operator *(ElectricResistance<T> resistance, ElectricCurrent<T> current)
	{
		ArgumentNullException.ThrowIfNull(resistance);
		ArgumentNullException.ThrowIfNull(current);

		T voltageValue = resistance.Value * current.Value;

		return ElectricPotential<T>.Create(voltageValue);
	}

	/// <summary>
	/// Calculates current from resistance and voltage (I = V/R).
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
}
