// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a pressure quantity with compile-time dimensional safety.
/// Pressure is defined as force per unit area (P = F/A).
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Pressure<T> : PhysicalQuantity<Pressure<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Pressure;

	/// <summary>
	/// Initializes a new instance of the Pressure class.
	/// </summary>
	public Pressure() : base() { }

	/// <summary>
	/// Creates a new Pressure from a value in pascals.
	/// </summary>
	/// <param name="pascals">The value in pascals.</param>
	/// <returns>A new Pressure instance.</returns>
	public static Pressure<T> FromPascals(T pascals) => Create(pascals);

	/// <summary>
	/// Calculates force from pressure and area (F = P×A).
	/// </summary>
	/// <param name="pressure">The pressure.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting force.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Force<T> operator *(Pressure<T> pressure, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(pressure);
		ArgumentNullException.ThrowIfNull(area);

		T forceValue = pressure.Value * area.Value;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Creates a pressure instance representing standard atmospheric pressure.
	/// </summary>
	/// <returns>Standard atmospheric pressure (101,325 Pa).</returns>
	public static Pressure<T> StandardAtmospheric() =>
		Create(PhysicalConstants.Generic.StandardAtmosphericPressure<T>());

	/// <summary>
	/// Calculates the force that this pressure would exert on a given area.
	/// </summary>
	/// <param name="area">The area to calculate force for.</param>
	/// <returns>The resulting force (F = P·A).</returns>
	public Force<T> GetForce(Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(area);

		T pressureValue = In(Units.Pascal);
		T areaValue = area.In(Units.SquareMeter);
		T forceValue = pressureValue * areaValue;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Calculates the hydrostatic pressure at a given depth in a fluid.
	/// </summary>
	/// <param name="density">The density of the fluid.</param>
	/// <param name="depth">The depth below the surface.</param>
	/// <param name="gravity">The gravitational acceleration (optional, defaults to standard gravity).</param>
	/// <returns>The hydrostatic pressure (P = ρgh).</returns>
	public static Pressure<T> FromHydrostaticPressure(Density<T> density, Length<T> depth, Acceleration<T>? gravity = null)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(depth);

		T densityValue = density.In(Units.KilogramPerCubicMeter);
		T depthValue = depth.In(Units.Meter);
		T gravityValue = gravity?.In(Units.MetersPerSecondSquared) ?? PhysicalConstants.Generic.StandardGravity<T>();
		T pressureValue = densityValue * gravityValue * depthValue;

		return Create(pressureValue);
	}
}
