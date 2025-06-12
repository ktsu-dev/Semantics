// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an electric capacitance quantity with dimensional analysis support.
/// </summary>
public sealed record ElectricCapacitance : PhysicalQuantity<ElectricCapacitance>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCapacitance;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Farad;

	/// <summary>
	/// Initializes a new instance of the ElectricCapacitance class.
	/// </summary>
	public ElectricCapacitance() : base() { }

	/// <summary>
	/// Multiplies electric capacitance by electric potential to get electric charge.
	/// </summary>
	/// <param name="capacitance">The electric capacitance.</param>
	/// <param name="voltage">The electric potential.</param>
	/// <returns>The resulting electric charge (Q = C·V).</returns>
	public static ElectricCharge operator *(ElectricCapacitance capacitance, ElectricPotential voltage)
	{
		ArgumentNullException.ThrowIfNull(capacitance);
		ArgumentNullException.ThrowIfNull(voltage);
		// Q = C·V: Charge = Capacitance × Voltage
		double resultValue = capacitance.Value * voltage.Value;
		return ElectricCharge.Create(resultValue);
	}

	/// <summary>
	/// Multiplies electric potential by electric capacitance to get electric charge.
	/// </summary>
	/// <param name="voltage">The electric potential.</param>
	/// <param name="capacitance">The electric capacitance.</param>
	/// <returns>The resulting electric charge (Q = C·V).</returns>
	public static ElectricCharge operator *(ElectricPotential voltage, ElectricCapacitance capacitance) => capacitance * voltage;

	/// <summary>
	/// Calculates the equivalent capacitance of capacitors in parallel.
	/// </summary>
	/// <param name="capacitances">The capacitances to combine in parallel.</param>
	/// <returns>The equivalent capacitance (C_total = C1 + C2 + ...).</returns>
	public static ElectricCapacitance Parallel(params ElectricCapacitance[] capacitances)
	{
		ArgumentNullException.ThrowIfNull(capacitances);
		if (capacitances.Length == 0)
		{
			throw new ArgumentException("At least one capacitance is required", nameof(capacitances));
		}

		double totalValue = capacitances.Sum(c => c.Value);
		return Create(totalValue);
	}

	/// <summary>
	/// Calculates the equivalent capacitance of capacitors in series.
	/// </summary>
	/// <param name="capacitances">The capacitances to combine in series.</param>
	/// <returns>The equivalent capacitance (1/C_total = 1/C1 + 1/C2 + ...).</returns>
	public static ElectricCapacitance Series(params ElectricCapacitance[] capacitances)
	{
		ArgumentNullException.ThrowIfNull(capacitances);
		if (capacitances.Length == 0)
		{
			throw new ArgumentException("At least one capacitance is required", nameof(capacitances));
		}

		double reciprocalSum = capacitances.Sum(c => 1.0 / c.Value);
		double equivalentValue = 1.0 / reciprocalSum;
		return Create(equivalentValue);
	}

	/// <inheritdoc/>
	public static ElectricCharge Multiply(ElectricCapacitance left, ElectricCapacitance right) => throw new NotImplementedException();
}
