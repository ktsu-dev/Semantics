// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a power quantity with dimensional analysis support.
/// </summary>
public sealed record Power : PhysicalQuantity<Power>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Power;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Watt;

	/// <summary>
	/// Initializes a new instance of the Power class.
	/// </summary>
	public Power() : base() { }

	/// <summary>
	/// Multiplies power by time to get energy.
	/// </summary>
	/// <param name="power">The power.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting energy (E = P·t).</returns>
	public static Energy operator *(Power power, Time time)
	{
		ArgumentNullException.ThrowIfNull(power);
		ArgumentNullException.ThrowIfNull(time);
		// E = P·t: Energy = Power × Time
		double resultValue = power.Value * time.Value;
		return Energy.Create(resultValue);
	}

	/// <summary>
	/// Multiplies time by power to get energy.
	/// </summary>
	/// <param name="time">The time.</param>
	/// <param name="power">The power.</param>
	/// <returns>The resulting energy (E = P·t).</returns>
	public static Energy operator *(Time time, Power power) => power * time;

	/// <summary>
	/// Divides power by voltage to get current (electrical power).
	/// </summary>
	/// <param name="power">The power.</param>
	/// <param name="voltage">The voltage.</param>
	/// <returns>The resulting current (I = P/V).</returns>
	public static ElectricCurrent operator /(Power power, ElectricPotential voltage)
	{
		ArgumentNullException.ThrowIfNull(power);
		ArgumentNullException.ThrowIfNull(voltage);
		// I = P/V: Current = Power / Voltage
		double resultValue = power.Value / voltage.Value;
		return ElectricCurrent.Create(resultValue);
	}

	/// <summary>
	/// Divides power by current to get voltage (electrical power).
	/// </summary>
	/// <param name="power">The power.</param>
	/// <param name="current">The current.</param>
	/// <returns>The resulting voltage (V = P/I).</returns>
	public static ElectricPotential operator /(Power power, ElectricCurrent current)
	{
		ArgumentNullException.ThrowIfNull(power);
		ArgumentNullException.ThrowIfNull(current);
		// V = P/I: Voltage = Power / Current
		double resultValue = power.Value / current.Value;
		return ElectricPotential.Create(resultValue);
	}

	/// <inheritdoc/>
	public static Energy Multiply(Power left, Power right) => throw new NotImplementedException();

	/// <inheritdoc/>
	public static ElectricCurrent Divide(Power left, Power right) => throw new NotImplementedException();
}
