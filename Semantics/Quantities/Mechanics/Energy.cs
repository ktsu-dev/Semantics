// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an energy quantity with compile-time dimensional safety.
/// Energy can be kinetic (½mv²), potential (mgh), or work done (F·d).
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Energy<T> : PhysicalQuantity<Energy<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Energy;

	/// <summary>
	/// Initializes a new instance of the Energy class.
	/// </summary>
	public Energy() : base() { }

	/// <summary>
	/// Creates a new Energy from a value in joules.
	/// </summary>
	/// <param name="joules">The value in joules.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> FromJoules(T joules) => Create(joules);

	/// <summary>
	/// Calculates kinetic energy from mass and velocity (KE = ½mv²).
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <param name="velocity">The velocity of the object.</param>
	/// <returns>The kinetic energy.</returns>
	public static Energy<T> FromKineticEnergy(Mass<T> mass, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(velocity);

		T massValue = mass.In(Units.Kilogram);
		T velocityValue = velocity.In(Units.MetersPerSecond);
		T half = T.CreateChecked(0.5);
		T energyValue = half * massValue * velocityValue * velocityValue;

		return Create(energyValue);
	}

	/// <summary>
	/// Calculates gravitational potential energy from mass, height, and gravity (PE = mgh).
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <param name="height">The height above reference level.</param>
	/// <param name="gravity">The gravitational acceleration (optional, defaults to standard gravity).</param>
	/// <returns>The gravitational potential energy.</returns>
	public static Energy<T> FromPotentialEnergy(Mass<T> mass, Length<T> height, Acceleration<T>? gravity = null)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(height);

		T massValue = mass.In(Units.Kilogram);
		T heightValue = height.In(Units.Meter);
		T gravityValue = gravity?.In(Units.MetersPerSecondSquared) ?? PhysicalConstants.Generic.StandardGravity<T>();
		T energyValue = massValue * gravityValue * heightValue;

		return Create(energyValue);
	}

	/// <summary>
	/// Calculates work done by a force over a distance (W = F·d).
	/// </summary>
	/// <param name="force">The applied force.</param>
	/// <param name="distance">The distance over which the force is applied.</param>
	/// <returns>The work done (energy transferred).</returns>
	public static Energy<T> FromWork(Force<T> force, Length<T> distance)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(distance);

		T forceValue = force.In(Units.Newton);
		T distanceValue = distance.In(Units.Meter);
		T energyValue = forceValue * distanceValue;

		return Create(energyValue);
	}

	/// <summary>
	/// Calculates the velocity an object would have if all this energy were kinetic energy.
	/// Note: This method requires floating-point arithmetic and works best with double or float types.
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <returns>The velocity (v = √(2E/m)).</returns>
	public Velocity<T> GetVelocityFromKineticEnergy(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T energyValue = In(Units.Joule);
		T massValue = mass.In(Units.Kilogram);
		T two = T.CreateChecked(2.0);
		T velocitySquared = two * energyValue / massValue;

		// Convert to double for square root calculation, then back to T
		double velocitySquaredDouble = double.CreateChecked(velocitySquared);
		double velocityDouble = Math.Sqrt(velocitySquaredDouble);
		T velocityValue = T.CreateChecked(velocityDouble);

		return Velocity<T>.Create(velocityValue);
	}
}
