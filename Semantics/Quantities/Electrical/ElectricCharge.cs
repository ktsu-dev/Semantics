// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an electric charge quantity with dimensional analysis support.
/// </summary>
public sealed record ElectricCharge : PhysicalQuantity<ElectricCharge>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCharge;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Coulomb;

	/// <summary>
	/// Initializes a new instance of the ElectricCharge class.
	/// </summary>
	public ElectricCharge() : base() { }

	/// <summary>
	/// Divides electric charge by time to get electric current.
	/// </summary>
	/// <param name="charge">The electric charge.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting electric current (I = Q/t).</returns>
	public static ElectricCurrent operator /(ElectricCharge charge, Time time)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(time);
		// I = Q/t: Current = Charge / Time
		double resultValue = charge.Value / time.Value;
		return ElectricCurrent.Create(resultValue);
	}

	/// <summary>
	/// Divides electric charge by electric capacitance to get electric potential.
	/// </summary>
	/// <param name="charge">The electric charge.</param>
	/// <param name="capacitance">The electric capacitance.</param>
	/// <returns>The resulting electric potential (V = Q/C).</returns>
	public static ElectricPotential operator /(ElectricCharge charge, ElectricCapacitance capacitance)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(capacitance);
		// V = Q/C: Voltage = Charge / Capacitance
		double resultValue = charge.Value / capacitance.Value;
		return ElectricPotential.Create(resultValue);
	}

	/// <inheritdoc/>
	public static ElectricCurrent Divide(ElectricCharge left, ElectricCharge right) => throw new NotImplementedException();

	/// <inheritdoc/>
	public static ElectricCharge Multiply(ElectricCharge left, ElectricCharge right) => throw new NotImplementedException();
}
