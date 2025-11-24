// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a power quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Power<T> : PhysicalQuantity<Power<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of power [M L² T⁻³].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Power;

	/// <summary>
	/// Initializes a new instance of the <see cref="Power{T}"/> class.
	/// </summary>
	public Power() : base() { }

	/// <summary>
	/// Creates a new Power from a value in watts.
	/// </summary>
	/// <param name="watts">The value in watts.</param>
	/// <returns>A new Power instance.</returns>
	public static Power<T> FromWatts(T watts) => Create(watts);

	/// <summary>
	/// Calculates energy from power and time (E = P·t).
	/// </summary>
	/// <param name="power">The power.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting energy.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Energy<T> operator *(Power<T> power, Time<T> time)
	{
		Guard.NotNull(power);
		Guard.NotNull(time);

		T energyValue = power.Value * time.Value;

		return Energy<T>.Create(energyValue);
	}

	/// <summary>
	/// Calculates energy from time and power (E = P·t).
	/// </summary>
	/// <param name="time">The time duration.</param>
	/// <param name="power">The power.</param>
	/// <returns>The resulting energy.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Energy<T> operator *(Time<T> time, Power<T> power) => power * time;
}
