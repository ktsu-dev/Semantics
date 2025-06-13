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
	/// Calculates work/energy from force and distance (W = F·d).
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="distance">The distance.</param>
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
	/// Calculates work/energy from distance and force (W = F·d).
	/// </summary>
	/// <param name="distance">The distance.</param>
	/// <param name="force">The force.</param>
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
}
