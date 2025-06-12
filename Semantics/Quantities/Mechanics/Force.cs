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
}
