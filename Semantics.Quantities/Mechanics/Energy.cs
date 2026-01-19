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
	/// <summary>Gets the physical dimension of energy [M L² T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Energy;

	/// <summary>
	/// Initializes a new instance of the <see cref="Energy{T}"/> class.
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
		Ensure.NotNull(mass);
		Ensure.NotNull(velocity);

		T halfValue = PhysicalConstants.Generic.OneHalf<T>();
		T kineticEnergyValue = halfValue * mass.Value * velocity.Value * velocity.Value;

		return Create(kineticEnergyValue);
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
		Ensure.NotNull(mass);
		Ensure.NotNull(height);

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
		Ensure.NotNull(force);
		Ensure.NotNull(distance);

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
		Ensure.NotNull(mass);

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

	/// <summary>
	/// Calculates power from energy and time (P = E/t).
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting power.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Power<T> operator /(Energy<T> energy, Time<T> time)
	{
		Ensure.NotNull(energy);
		Ensure.NotNull(time);

		T powerValue = energy.Value / time.Value;

		return Power<T>.Create(powerValue);
	}

	/// <summary>
	/// Calculates time from energy and power (t = E/P).
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="power">The power.</param>
	/// <returns>The resulting time duration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(Energy<T> energy, Power<T> power)
	{
		Ensure.NotNull(energy);
		Ensure.NotNull(power);

		T timeValue = energy.Value / power.Value;

		return Time<T>.Create(timeValue);
	}

	/// <summary>
	/// Calculates force from energy and distance (F = E/d).
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="distance">The distance.</param>
	/// <returns>The resulting force.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Force<T> operator /(Energy<T> energy, Length<T> distance)
	{
		Ensure.NotNull(energy);
		Ensure.NotNull(distance);

		T forceValue = energy.Value / distance.Value;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Calculates distance from energy and force (d = E/F).
	/// </summary>
	/// <param name="energy">The energy.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting distance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Length<T> operator /(Energy<T> energy, Force<T> force)
	{
		Ensure.NotNull(energy);
		Ensure.NotNull(force);

		T distanceValue = energy.Value / force.Value;

		return Length<T>.Create(distanceValue);
	}

	/// <summary>
	/// Calculates gravitational potential energy (PE = mgh).
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="height">The height.</param>
	/// <returns>The resulting gravitational potential energy.</returns>
	public static Energy<T> FromGravitationalPotential(Mass<T> mass, Length<T> height)
	{
		Ensure.NotNull(mass);
		Ensure.NotNull(height);

		T gravityValue = PhysicalConstants.Generic.StandardGravity<T>();
		T potentialEnergyValue = mass.Value * gravityValue * height.Value;

		return Create(potentialEnergyValue);
	}

	/// <summary>
	/// Calculates velocity from kinetic energy and mass (v = √(2KE/m)).
	/// </summary>
	/// <param name="kineticEnergy">The kinetic energy.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting velocity.</returns>
	public static Velocity<T> GetVelocityFromKineticEnergy(Energy<T> kineticEnergy, Mass<T> mass)
	{
		Ensure.NotNull(kineticEnergy);
		Ensure.NotNull(mass);

		T twoValue = T.CreateChecked(2.0);
		T velocitySquared = twoValue * kineticEnergy.Value / mass.Value;
		T velocityValue = T.CreateChecked(Math.Sqrt(double.CreateChecked(velocitySquared)));

		return Velocity<T>.Create(velocityValue);
	}
}
