// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a force quantity with dimensional analysis support.
/// </summary>
public sealed record Force : PhysicalQuantity<Force>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Force;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Newton;

	/// <summary>
	/// Initializes a new instance of the Force class.
	/// </summary>
	public Force() : base() { }

	/// <summary>
	/// Initializes a new instance of the Force class.
	/// </summary>
	/// <param name="value">The magnitude of the force.</param>
	/// <param name="unit">The unit of the force.</param>
	private Force(double value, IUnit unit) : base(value, unit) { }

	/// <summary>
	/// Divides a force by a mass to get acceleration.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting acceleration (a = F/m).</returns>
	public static Acceleration operator /(Force force, Mass mass)
	{
		// a = F/m: Acceleration = Force / Mass
		double resultValue = force.Value / mass.Value;
		return Acceleration.Create(resultValue);
	}

	/// <summary>
	/// Divides a force by an acceleration to get mass.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting mass (m = F/a).</returns>
	public static Mass operator /(Force force, Acceleration acceleration)
	{
		// m = F/a: Mass = Force / Acceleration
		double resultValue = force.Value / acceleration.Value;
		IUnit massUnit = Units.Kilogram; // Default to kg for the result
		return Mass.Create(resultValue, massUnit);
	}

	/// <summary>
	/// Multiplies a force by a length to get energy (work).
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="length">The displacement length.</param>
	/// <returns>The resulting energy (W = F·d).</returns>
	public static Energy operator *(Force force, Length length)
	{
		// W = F·d: Work = Force × Displacement
		double resultValue = force.Value * length.Value;
		IUnit energyUnit = Units.Joule; // Default to Joule for the result
		return Energy.Create(resultValue, energyUnit);
	}

	/// <summary>
	/// Multiplies a length by a force to get energy (work).
	/// </summary>
	/// <param name="length">The displacement length.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting energy (W = F·d).</returns>
	public static Energy operator *(Length length, Force force) => force * length;

	/// <summary>
	/// Divides a force by an area to get pressure.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting pressure (P = F/A).</returns>
	public static Pressure operator /(Force force, Area area)
	{
		// P = F/A: Pressure = Force / Area
		double resultValue = force.Value / area.Value;
		IUnit pressureUnit = Units.Pascal; // Default to Pascal for the result
		return Pressure.Create(resultValue, pressureUnit);
	}

	/// <inheritdoc/>
	public static Acceleration Divide(Force left, Force right) => throw new NotImplementedException();

	public static Energy Multiply(Force left, Force right) => throw new NotImplementedException();
}
