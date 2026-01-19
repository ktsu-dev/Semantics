// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a velocity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Velocity<T> : PhysicalQuantity<Velocity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of velocity [L T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Velocity;

	/// <summary>
	/// Initializes a new instance of the <see cref="Velocity{T}"/> class.
	/// </summary>
	public Velocity() : base() { }

	/// <summary>
	/// Creates a new Velocity from a value in meters per second.
	/// </summary>
	/// <param name="metersPerSecond">The value in meters per second.</param>
	/// <returns>A new Velocity instance.</returns>
	public static Velocity<T> FromMetersPerSecond(T metersPerSecond) => Create(metersPerSecond);

	/// <summary>
	/// Divides this velocity by time to create an acceleration.
	/// </summary>
	/// <param name="left">The velocity.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting acceleration.</returns>
	public static Acceleration<T> operator /(Velocity<T> left, Time<T> right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		T velocityValue = left.In(Units.MetersPerSecond);
		T timeValue = right.In(Units.Second);
		T accelerationValue = velocityValue / timeValue;

		return Acceleration<T>.Create(accelerationValue);
	}

	/// <summary>
	/// Divides this velocity by time to create an acceleration.
	/// </summary>
	/// <param name="left">The velocity.</param>
	/// <param name="right">The time.</param>
	/// <returns>The resulting acceleration.</returns>
	public static Acceleration<T> Divide(Velocity<T> left, Time<T> right) => left / right;

	/// <summary>
	/// Calculates distance from velocity and time (d = v·t).
	/// </summary>
	/// <param name="velocity">The velocity.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting distance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Length<T> operator *(Velocity<T> velocity, Time<T> time)
	{
		Ensure.NotNull(velocity);
		Ensure.NotNull(time);

		T distanceValue = velocity.Value * time.Value;

		return Length<T>.Create(distanceValue);
	}

	/// <summary>
	/// Calculates distance from time and velocity (d = v·t).
	/// </summary>
	/// <param name="time">The time duration.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>The resulting distance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Length<T> operator *(Time<T> time, Velocity<T> velocity) => velocity * time;

	/// <summary>
	/// Calculates time from velocity and distance (t = d/v).
	/// </summary>
	/// <param name="distance">The distance.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>The resulting time duration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(Length<T> distance, Velocity<T> velocity)
	{
		Ensure.NotNull(distance);
		Ensure.NotNull(velocity);

		T timeValue = distance.Value / velocity.Value;

		return Time<T>.Create(timeValue);
	}

	/// <summary>
	/// Calculates time from velocity and acceleration (t = v/a).
	/// </summary>
	/// <param name="velocity">The velocity.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting time duration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(Velocity<T> velocity, Acceleration<T> acceleration)
	{
		Ensure.NotNull(velocity);
		Ensure.NotNull(acceleration);

		T timeValue = velocity.Value / acceleration.Value;

		return Time<T>.Create(timeValue);
	}
}
