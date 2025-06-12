// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an acceleration quantity with dimensional analysis support.
/// </summary>
public sealed record Acceleration : PhysicalQuantity<Acceleration>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Acceleration;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.MetersPerSecondSquared;

	/// <summary>
	/// Initializes a new instance of the Acceleration class.
	/// </summary>
	public Acceleration() : base() { }

	/// <summary>
	/// Multiplies an acceleration by a mass to get a force.
	/// </summary>
	/// <param name="acceleration">The acceleration.</param>
	/// <param name="mass">The mass.</param>
	/// <returns>The resulting force (F = ma).</returns>
	public static Force operator *(Acceleration acceleration, Mass mass)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		ArgumentNullException.ThrowIfNull(mass);
		// F = ma: Force = Mass × Acceleration
		double resultValue = acceleration.Value * mass.Value;
		return Force.Create(resultValue);
	}

	/// <summary>
	/// Multiplies a mass by an acceleration to get a force.
	/// </summary>
	/// <param name="mass">The mass.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting force (F = ma).</returns>
	public static Force operator *(Mass mass, Acceleration acceleration) => acceleration * mass;

	/// <summary>
	/// Multiplies an acceleration by a time to get a velocity.
	/// </summary>
	/// <param name="acceleration">The acceleration.</param>
	/// <param name="time">The time.</param>
	/// <returns>The resulting velocity (v = at).</returns>
	public static Velocity operator *(Acceleration acceleration, Time time)
	{
		ArgumentNullException.ThrowIfNull(acceleration);
		ArgumentNullException.ThrowIfNull(time);
		// v = at: Velocity = Acceleration × Time
		double resultValue = acceleration.Value * time.Value;
		return Velocity.Create(resultValue);
	}

	/// <summary>
	/// Multiplies a time by an acceleration to get a velocity.
	/// </summary>
	/// <param name="time">The time.</param>
	/// <param name="acceleration">The acceleration.</param>
	/// <returns>The resulting velocity (v = at).</returns>
	public static Velocity operator *(Time time, Acceleration acceleration) => acceleration * time;

	/// <inheritdoc/>
	public static Force Multiply(Acceleration left, Acceleration right) => throw new NotImplementedException();
}
