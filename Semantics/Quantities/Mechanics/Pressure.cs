// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a pressure quantity with dimensional analysis support.
/// </summary>
public sealed record Pressure : PhysicalQuantity<Pressure>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Pressure;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Pascal;

	/// <summary>
	/// Initializes a new instance of the Pressure class.
	/// </summary>
	public Pressure() : base() { }

	/// <summary>
	/// Multiplies a pressure by an area to get force.
	/// </summary>
	/// <param name="pressure">The pressure.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting force (F = P·A).</returns>
	public static Force operator *(Pressure pressure, Area area)
	{
		ArgumentNullException.ThrowIfNull(pressure);
		ArgumentNullException.ThrowIfNull(area);
		// F = P·A: Force = Pressure × Area
		double resultValue = pressure.Value * area.Value;
		return Force.Create(resultValue);
	}

	/// <summary>
	/// Multiplies an area by a pressure to get force.
	/// </summary>
	/// <param name="area">The area.</param>
	/// <param name="pressure">The pressure.</param>
	/// <returns>The resulting force (F = P·A).</returns>
	public static Force operator *(Area area, Pressure pressure) => pressure * area;

	/// <summary>
	/// Multiplies a pressure by a volume to get energy.
	/// </summary>
	/// <param name="pressure">The pressure.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting energy (E = P·V).</returns>
	public static Energy operator *(Pressure pressure, Volume volume)
	{
		ArgumentNullException.ThrowIfNull(pressure);
		ArgumentNullException.ThrowIfNull(volume);
		// E = P·V: Energy = Pressure × Volume (thermodynamic work)
		double resultValue = pressure.Value * volume.Value;
		return Energy.Create(resultValue);
	}

	/// <summary>
	/// Multiplies a volume by a pressure to get energy.
	/// </summary>
	/// <param name="volume">The volume.</param>
	/// <param name="pressure">The pressure.</param>
	/// <returns>The resulting energy (E = P·V).</returns>
	public static Energy operator *(Volume volume, Pressure pressure) => pressure * volume;

	/// <inheritdoc/>
	public static Force Multiply(Pressure left, Pressure right) => throw new NotImplementedException();
}
