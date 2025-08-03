// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents an acceleration quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Acceleration<T> : PhysicalQuantity<Acceleration<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of acceleration [L T⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Acceleration;

	/// <summary>
	/// Initializes a new instance of the <see cref="Acceleration{T}"/> class.
	/// </summary>
	public Acceleration() : base() { }

	/// <summary>
	/// Creates a new Acceleration from a value in meters per second squared.
	/// </summary>
	/// <param name="metersPerSecondSquared">The value in meters per second squared.</param>
	/// <returns>A new Acceleration instance.</returns>
	public static Acceleration<T> FromMetersPerSecondSquared(T metersPerSecondSquared) => Create(metersPerSecondSquared);

	/// <summary>
	/// Calculates velocity from acceleration and time (v = a·t).
	/// </summary>
	/// <param name="acceleration">The acceleration.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting velocity.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Velocity<T> operator *(Acceleration<T> acceleration, Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		ArgumentNullException.ThrowIfNull(time);

		T velocityValue = acceleration.Value * time.Value;

		return Velocity<T>.Create(velocityValue);
	}

	/// <summary>
	/// Calculates velocity from time and acceleration (v = a·t).
	/// </summary>
	/// <param name="time">The time duration.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting velocity.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Velocity<T> operator *(Time<T> time, Acceleration<T> acceleration) => acceleration * time;

	/// <summary>
	/// Calculates time from acceleration and velocity (t = v/a).
	/// </summary>
	/// <param name="velocity">The velocity.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting time duration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(Velocity<T> velocity, Acceleration<T> acceleration)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(acceleration);

		T timeValue = velocity.Value / acceleration.Value;

		return Time<T>.Create(timeValue);
	}
}
