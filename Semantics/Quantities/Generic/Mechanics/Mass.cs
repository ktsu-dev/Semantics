// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a mass quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Mass<T> : PhysicalQuantity<Mass<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of mass [M].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Mass;

	/// <summary>
	/// Initializes a new instance of the <see cref="Mass{T}"/> class.
	/// </summary>
	public Mass() : base() { }

	/// <summary>
	/// Creates a new Mass from a value in kilograms.
	/// </summary>
	/// <param name="kilograms">The value in kilograms.</param>
	/// <returns>A new Mass instance.</returns>
	public static Mass<T> FromKilograms(T kilograms) => Create(kilograms);

	/// <summary>
	/// Calculates force from mass and acceleration using Newton's second law (F = ma).
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting force.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Force<T> operator *(Mass<T> mass, Acceleration<T> acceleration)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(acceleration);

		T forceValue = mass.Value * acceleration.Value;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Calculates force from acceleration and mass using Newton's second law (F = ma).
	/// </summary>
	/// <param name="acceleration">The acceleration.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting force.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Force<T> operator *(Acceleration<T> acceleration, Mass<T> mass) => mass * acceleration;

	/// <summary>
	/// Calculates the kinetic energy of this mass at a given velocity (KE = ½mv²).
	/// </summary>
	/// <param name="velocity">The velocity of the mass.</param>
	/// <returns>The kinetic energy.</returns>
	public Energy<T> CalculateKineticEnergy(Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(velocity);

		T massValue = In(Units.Kilogram);
		T velocityValue = velocity.In(Units.MetersPerSecond);
		T half = T.One / (T.One + T.One);
		T energyValue = half * massValue * velocityValue * velocityValue;

		return Energy<T>.Create(energyValue);
	}

	/// <summary>
	/// Calculates the momentum of this mass at a given velocity (p = mv).
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="velocity">The velocity of the mass.</param>
	/// <returns>The momentum.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Momentum<T> operator *(Mass<T> mass, Velocity<T> velocity)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(velocity);

		T momentumValue = mass.Value * velocity.Value;

		return Momentum<T>.Create(momentumValue);
	}

	/// <summary>
	/// Calculates the momentum of this mass at a given velocity (p = mv).
	/// </summary>
	/// <param name="velocity">The velocity of the mass.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The momentum.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Momentum<T> operator *(Velocity<T> velocity, Mass<T> mass) => mass * velocity;

	/// <summary>
	/// Calculates the weight (gravitational force) of this mass under standard gravity.
	/// </summary>
	/// <returns>The weight force under standard gravity (9.80665 m/s²).</returns>
	public Force<T> CalculateWeight()
	{
		T massValue = In(Units.Kilogram);
		T gravity = PhysicalConstants.Generic.StandardGravity<T>();
		T forceValue = massValue * gravity;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Calculates density from mass and volume (ρ = m/V).
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="volume">The volume.</param>
	/// <returns>The resulting density.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Density<T> operator /(Mass<T> mass, Volume<T> volume)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(volume);

		T densityValue = mass.Value / volume.Value;

		return Density<T>.Create(densityValue);
	}

	/// <summary>
	/// Calculates volume from mass and density (V = m/ρ).
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="density">The density.</param>
	/// <returns>The resulting volume.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Volume<T> operator /(Mass<T> mass, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(mass);
		ArgumentNullException.ThrowIfNull(density);

		T volumeValue = mass.Value / density.Value;

		return Volume<T>.Create(volumeValue);
	}
}
