// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a force quantity with compile-time dimensional safety.
/// Force is defined by Newton's second law: F = ma
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Force<T> : PhysicalQuantity<Force<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Force;

	/// <summary>
	/// Initializes a new instance of the Force class.
	/// </summary>
	public Force() : base() { }

	/// <summary>
	/// Creates a new Force from a value in newtons.
	/// </summary>
	/// <param name="newtons">The value in newtons.</param>
	/// <returns>A new Force instance.</returns>
	public static Force<T> FromNewtons(T newtons) => Create(newtons);

	/// <summary>
	/// Calculates force from mass and acceleration using Newton's second law (F = ma).
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <param name="acceleration">The acceleration of the object.</param>
	/// <returns>The resulting force.</returns>
	public static Force<T> FromMassAndAcceleration(Mass<T> mass, Acceleration<T> acceleration)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(acceleration);

		T massValue = mass.In(Units.Kilogram);
		T accelerationValue = acceleration.In(Units.MetersPerSecondSquared);
		T forceValue = massValue * accelerationValue;

		return Create(forceValue);
	}

	/// <summary>
	/// Calculates the weight force of an object under standard gravity.
	/// </summary>
	/// <param name="mass">The mass of the object.</param>
	/// <returns>The weight force under standard gravity (9.80665 m/s²).</returns>
	public static Force<T> FromWeight(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T massValue = mass.In(Units.Kilogram);
		T gravity = PhysicalConstants.Generic.StandardGravity<T>();
		T forceValue = massValue * gravity;

		return Create(forceValue);
	}

	/// <summary>
	/// Calculates the acceleration that this force would produce on a given mass.
	/// </summary>
	/// <param name="mass">The mass to accelerate.</param>
	/// <returns>The resulting acceleration (a = F/m).</returns>
	public Acceleration<T> GetAcceleration(Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(mass);

		T forceValue = In(Units.Newton);
		T massValue = mass.In(Units.Kilogram);
		T accelerationValue = forceValue / massValue;

		return Acceleration<T>.Create(accelerationValue);
	}

	/// <summary>
	/// Calculates power from force and velocity (P = F·v).
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>The resulting power.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Power<T> operator *(Force<T> force, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(velocity);

		T powerValue = force.Value * velocity.Value;

		return Power<T>.Create(powerValue);
	}

	/// <summary>
	/// Calculates power from velocity and force (P = F·v).
	/// </summary>
	/// <param name="velocity">The velocity.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting power.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Power<T> operator *(Velocity<T> velocity, Force<T> force) => force * velocity;

	/// <summary>
	/// Calculates work/energy from force and displacement distance (W = F·d).
	/// Use this when force is applied in the direction of motion.
	/// For torque calculations, use CalculateTorque() method instead.
	/// </summary>
	/// <param name="force">The applied force.</param>
	/// <param name="distance">The displacement distance in the direction of force.</param>
	/// <returns>The resulting work/energy.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Energy<T> operator *(Force<T> force, Length<T> distance)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(distance);

		T energyValue = force.Value * distance.Value;

		return Energy<T>.Create(energyValue);
	}

	/// <summary>
	/// Calculates work/energy from displacement distance and force (W = F·d).
	/// Use this when force is applied in the direction of motion.
	/// For torque calculations, use CalculateTorque() method instead.
	/// </summary>
	/// <param name="distance">The displacement distance in the direction of force.</param>
	/// <param name="force">The applied force.</param>
	/// <returns>The resulting work/energy.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Energy<T> operator *(Length<T> distance, Force<T> force) => force * distance;

	/// <summary>
	/// Calculates acceleration from force and mass (a = F/m).
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting acceleration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Acceleration<T> operator /(Force<T> force, Mass<T> mass)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(mass);

		T accelerationValue = force.Value / mass.Value;

		return Acceleration<T>.Create(accelerationValue);
	}

	/// <summary>
	/// Calculates mass from force and acceleration (m = F/a).
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting mass.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Mass<T> operator /(Force<T> force, Acceleration<T> acceleration)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(acceleration);

		T massValue = force.Value / acceleration.Value;

		return Mass<T>.Create(massValue);
	}

	/// <summary>
	/// Calculates torque from force and moment arm distance (τ = F×r).
	/// Use this method when the force is applied perpendicular to the moment arm.
	/// For work/energy calculations, use the * operator instead.
	/// </summary>
	/// <param name="momentArm">The perpendicular distance from the axis of rotation.</param>
	/// <returns>The resulting torque.</returns>
	public Torque<T> CalculateTorque(Length<T> momentArm)
	{
		ArgumentNullException.ThrowIfNull(momentArm);

		T torqueValue = Value * momentArm.Value;

		return Torque<T>.Create(torqueValue);
	}

	/// <summary>
	/// Calculates torque from force and moment arm distance (τ = F×r).
	/// Use this method when the force is applied perpendicular to the moment arm.
	/// For work/energy calculations, use the * operator instead.
	/// </summary>
	/// <param name="force">The applied force.</param>
	/// <param name="momentArm">The perpendicular distance from the axis of rotation.</param>
	/// <returns>The resulting torque.</returns>
	public static Torque<T> CalculateTorque(Force<T> force, Length<T> momentArm)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(momentArm);

		T torqueValue = force.Value * momentArm.Value;

		return Torque<T>.Create(torqueValue);
	}

	/// <summary>
	/// Calculates pressure from force and area (P = F/A).
	/// </summary>
	/// <param name="force">The applied force.</param>
	/// <param name="area">The area over which force is applied.</param>
	/// <returns>The resulting pressure.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Pressure<T> operator /(Force<T> force, Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(force);
		ArgumentNullException.ThrowIfNull(area);

		T pressureValue = force.Value / area.Value;

		return Pressure<T>.Create(pressureValue);
	}
}
