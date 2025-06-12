// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an energy quantity with dimensional analysis support.
/// </summary>
public sealed record Energy : PhysicalQuantity<Energy>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Energy;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Joule;

	/// <summary>
	/// Initializes a new instance of the Energy class.
	/// </summary>
	public Energy() : base() { }

	/// <summary>
	/// Divides energy by time to get power.
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting power (P = E/t).</returns>
	public static Power operator /(Energy energy, Time time)
	{
		ArgumentNullException.ThrowIfNull(energy);
		ArgumentNullException.ThrowIfNull(time);
		// P = E/t: Power = Energy / Time
		double resultValue = energy.Value / time.Value;
		return Power.Create(resultValue);
	}

	/// <summary>
	/// Divides energy by power to get time.
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="power">The power.</param>
	/// <returns>The resulting time (t = E/P).</returns>
	public static Time operator /(Energy energy, Power power)
	{
		ArgumentNullException.ThrowIfNull(energy);
		ArgumentNullException.ThrowIfNull(power);
		// t = E/P: Time = Energy / Power
		double resultValue = energy.Value / power.Value;
		return Time.Create(resultValue);
	}

	/// <inheritdoc/>
	public static Power Divide(Energy left, Energy right) => throw new NotImplementedException();

	/// <inheritdoc/>
	public static Energy Multiply(Energy left, Energy right) => throw new NotImplementedException();
}
