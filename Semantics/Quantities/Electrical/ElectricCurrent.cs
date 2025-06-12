// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an electric current quantity with dimensional analysis support.
/// </summary>
public sealed record ElectricCurrent : PhysicalQuantity<ElectricCurrent>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCurrent;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Ampere;

	/// <summary>
	/// Initializes a new instance of the ElectricCurrent class.
	/// </summary>
	public ElectricCurrent() : base() { }

	/// <summary>
	/// Multiplies electric current by time to get electric charge.
	/// </summary>
	/// <param name="current">The electric current.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting electric charge (Q = I·t).</returns>
	public static ElectricCharge operator *(ElectricCurrent current, Time time)
	{
		ArgumentNullException.ThrowIfNull(current);
		ArgumentNullException.ThrowIfNull(time);
		// Q = I·t: Charge = Current × Time
		double resultValue = current.Value * time.Value;
		return ElectricCharge.Create(resultValue);
	}

	/// <summary>
	/// Multiplies time by electric current to get electric charge.
	/// </summary>
	/// <param name="time">The time.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric charge (Q = I·t).</returns>
	public static ElectricCharge operator *(Time time, ElectricCurrent current) => current * time;

	/// <summary>
	/// Multiplies electric current by electric potential to get power.
	/// </summary>
	/// <param name="current">The electric current.</param>
	/// <param name="voltage">The electric potential (voltage).</param>
	/// <returns>The resulting power (P = I·V).</returns>
	public static Power operator *(ElectricCurrent current, ElectricPotential voltage)
	{
		ArgumentNullException.ThrowIfNull(current);
		ArgumentNullException.ThrowIfNull(voltage);
		// P = I·V: Power = Current × Voltage
		double resultValue = current.Value * voltage.Value;
		return Power.Create(resultValue);
	}

	/// <summary>
	/// Multiplies electric potential by electric current to get power.
	/// </summary>
	/// <param name="voltage">The electric potential (voltage).</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting power (P = I·V).</returns>
	public static Power operator *(ElectricPotential voltage, ElectricCurrent current) => current * voltage;

	/// <summary>
	/// Multiplies electric current by electric resistance to get electric potential.
	/// </summary>
	/// <param name="current">The electric current.</param>
	/// <param name="resistance">The electric resistance.</param>
	/// <returns>The resulting electric potential (V = I·R).</returns>
	public static ElectricPotential operator *(ElectricCurrent current, ElectricResistance resistance)
	{
		ArgumentNullException.ThrowIfNull(current);
		ArgumentNullException.ThrowIfNull(resistance);
		// V = I·R: Voltage = Current × Resistance (Ohm's Law)
		double resultValue = current.Value * resistance.Value;
		return ElectricPotential.Create(resultValue);
	}

	/// <summary>
	/// Multiplies electric resistance by electric current to get electric potential.
	/// </summary>
	/// <param name="resistance">The electric resistance.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric potential (V = I·R).</returns>
	public static ElectricPotential operator *(ElectricResistance resistance, ElectricCurrent current) => current * resistance;

	/// <inheritdoc/>
	public static ElectricCharge Multiply(ElectricCurrent left, ElectricCurrent right) => throw new NotImplementedException();
}
