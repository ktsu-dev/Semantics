// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an electric resistance quantity with dimensional analysis support.
/// </summary>
public sealed record ElectricResistance : PhysicalQuantity<ElectricResistance>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricResistance;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Ohm;

	/// <summary>
	/// Initializes a new instance of the ElectricResistance class.
	/// </summary>
	public ElectricResistance() : base() { }

	/// <summary>
	/// Calculates the equivalent resistance of resistors in series.
	/// </summary>
	/// <param name="resistances">The resistances to combine in series.</param>
	/// <returns>The equivalent resistance (R_total = R1 + R2 + ...).</returns>
	public static ElectricResistance Series(params ElectricResistance[] resistances)
	{
		ArgumentNullException.ThrowIfNull(resistances);
		if (resistances.Length == 0)
		{
			throw new ArgumentException("At least one resistance is required", nameof(resistances));
		}

		double totalValue = resistances.Sum(r => r.Value);
		return Create(totalValue);
	}

	/// <summary>
	/// Calculates the equivalent resistance of resistors in parallel.
	/// </summary>
	/// <param name="resistances">The resistances to combine in parallel.</param>
	/// <returns>The equivalent resistance (1/R_total = 1/R1 + 1/R2 + ...).</returns>
	public static ElectricResistance Parallel(params ElectricResistance[] resistances)
	{
		ArgumentNullException.ThrowIfNull(resistances);
		if (resistances.Length == 0)
		{
			throw new ArgumentException("At least one resistance is required", nameof(resistances));
		}

		double reciprocalSum = resistances.Sum(r => 1.0 / r.Value);
		double equivalentValue = 1.0 / reciprocalSum;
		return Create(equivalentValue);
	}
}
