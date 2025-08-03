// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents an electric power density quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record ElectricPowerDensity<T> : PhysicalQuantity<ElectricPowerDensity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricpowerdensity [M T⁻³].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricPowerDensity;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricPowerDensity{T}"/> class.
	/// </summary>
	public ElectricPowerDensity() : base() { }

	/// <summary>
	/// Creates a new ElectricPowerDensity from a value in watts per cubic meter.
	/// </summary>
	/// <param name="wattsPerCubicMeter">The value in watts per cubic meter.</param>
	/// <returns>A new ElectricPowerDensity instance.</returns>
	public static ElectricPowerDensity<T> FromWattsPerCubicMeter(T wattsPerCubicMeter) => Create(wattsPerCubicMeter);

	/// <summary>
	/// Divides power by volume to create power density.
	/// </summary>
	/// <param name="power">The power.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting power density.</returns>
	public static ElectricPowerDensity<T> Divide(Power<T> power, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(power);
		ArgumentNullException.ThrowIfNull(volume);
		return Create(power.Value / volume.Value);
	}

	/// <summary>
	/// Multiplies power density by volume to get power.
	/// </summary>
	/// <param name="powerDensity">The power density.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting power.</returns>
	public static Power<T> Multiply(ElectricPowerDensity<T> powerDensity, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(powerDensity);
		ArgumentNullException.ThrowIfNull(volume);
		return Power<T>.Create(powerDensity.Value * volume.Value);
	}
}
