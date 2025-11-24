// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a torque quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Torque<T> : PhysicalQuantity<Torque<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of torque [M L² T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Torque;

	/// <summary>
	/// Initializes a new instance of the <see cref="Torque{T}"/> class.
	/// </summary>
	public Torque() : base() { }

	/// <summary>
	/// Creates a new Torque from a value in newton-meters.
	/// </summary>
	/// <param name="newtonMeters">The value in newton-meters.</param>
	/// <returns>A new Torque instance.</returns>
	public static Torque<T> FromNewtonMeters(T newtonMeters) => Create(newtonMeters);

	/// <summary>
	/// Multiplies force by length to create torque.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="length">The length (lever arm).</param>
	/// <returns>The resulting torque.</returns>
	public static Torque<T> Multiply(Force<T> force, Length<T> length)
	{
		Guard.NotNull(force);
		Guard.NotNull(length);
		return Create(force.Value * length.Value);
	}

	/// <summary>
	/// Divides torque by force to get length.
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting length.</returns>
	public static Length<T> Divide(Torque<T> torque, Force<T> force)
	{
		Guard.NotNull(torque);
		Guard.NotNull(force);
		return Length<T>.Create(torque.Value / force.Value);
	}

	/// <summary>
	/// Divides torque by length to get force.
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="length">The length.</param>
	/// <returns>The resulting force.</returns>
	public static Force<T> Divide(Torque<T> torque, Length<T> length)
	{
		Guard.NotNull(torque);
		Guard.NotNull(length);
		return Force<T>.Create(torque.Value / length.Value);
	}

	/// <summary>
	/// Returns the tangential force from the torque and radius.
	/// </summary>
	/// <param name="length">The radius or moment arm.</param>
	/// <returns>The tangential force.</returns>
	public Force<T> GetTangentialForce(Length<T> length)
	{
		Guard.NotNull(length);

		return Force<T>.Create(Value / length.Value);
	}

	/// <summary>
	/// Calculates force from torque and distance (F = τ/r).
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="distance">The moment arm distance.</param>
	/// <returns>The resulting force.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Force<T> operator /(Torque<T> torque, Length<T> distance)
	{
		Guard.NotNull(torque);
		Guard.NotNull(distance);

		T forceValue = torque.Value / distance.Value;

		return Force<T>.Create(forceValue);
	}

	/// <summary>
	/// Calculates distance from torque and force (r = τ/F).
	/// </summary>
	/// <param name="torque">The torque.</param>
	/// <param name="force">The force.</param>
	/// <returns>The resulting moment arm distance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Length<T> operator /(Torque<T> torque, Force<T> force)
	{
		Guard.NotNull(torque);
		Guard.NotNull(force);

		T distanceValue = torque.Value / force.Value;

		return Length<T>.Create(distanceValue);
	}
}
