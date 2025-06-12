// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an electric potential (voltage) quantity with dimensional analysis support.
/// </summary>
public sealed record ElectricPotential : PhysicalQuantity<ElectricPotential>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricPotential;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Volt;

	/// <summary>
	/// Initializes a new instance of the ElectricPotential class.
	/// </summary>
	public ElectricPotential() : base() { }

	/// <summary>
	/// Divides electric potential by electric current to get electric resistance.
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric resistance (R = V/I).</returns>
	public static ElectricResistance operator /(ElectricPotential voltage, ElectricCurrent current)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(current);
		// R = V/I: Resistance = Voltage / Current (Ohm's Law)
		double resultValue = voltage.Value / current.Value;
		return ElectricResistance.Create(resultValue);
	}

	/// <summary>
	/// Divides electric potential by electric resistance to get electric current.
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="resistance">The electric resistance.</param>
	/// <returns>The resulting electric current (I = V/R).</returns>
	public static ElectricCurrent operator /(ElectricPotential voltage, ElectricResistance resistance)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(resistance);
		// I = V/R: Current = Voltage / Resistance (Ohm's Law)
		double resultValue = voltage.Value / resistance.Value;
		return ElectricCurrent.Create(resultValue);
	}

	/// <summary>
	/// Multiplies electric potential by electric charge to get energy.
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="charge">The electric charge.</param>
	/// <returns>The resulting energy (E = V·Q).</returns>
	public static Energy operator *(ElectricPotential voltage, ElectricCharge charge)
	{
		ArgumentNullException.ThrowIfNull(voltage);
		ArgumentNullException.ThrowIfNull(charge);
		// E = V·Q: Energy = Voltage × Charge
		double resultValue = voltage.Value * charge.Value;
		return Energy.Create(resultValue);
	}

	/// <summary>
	/// Multiplies electric charge by electric potential to get energy.
	/// </summary>
	/// <param name="charge">The electric charge.</param>
	/// <param name="voltage">The electric potential.</param>
	/// <returns>The resulting energy (E = V·Q).</returns>
	public static Energy operator *(ElectricCharge charge, ElectricPotential voltage) => voltage * charge;

	/// <inheritdoc/>
	public static ElectricResistance Divide(ElectricPotential left, ElectricPotential right) => throw new NotImplementedException();

	/// <inheritdoc/>
	public static Energy Multiply(ElectricPotential left, ElectricPotential right) => throw new NotImplementedException();
}
