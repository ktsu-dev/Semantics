// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric charge quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricCharge<T> : PhysicalQuantity<ElectricCharge<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electriccharge [I T].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricCharge;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricCharge{T}"/> class.
	/// </summary>
	public ElectricCharge() : base() { }

	/// <summary>
	/// Creates a new ElectricCharge from a value in coulombs.
	/// </summary>
	/// <param name="coulombs">The value in coulombs.</param>
	/// <returns>A new ElectricCharge instance.</returns>
	public static ElectricCharge<T> FromCoulombs(T coulombs) => Create(coulombs);

	/// <summary>
	/// Calculates current from charge and time (I = Q/t).
	/// </summary>
	/// <param name="charge">The electric charge.</param>
	/// <param name="time">The time duration.</param>
	/// <returns>The resulting electric current.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static ElectricCurrent<T> operator /(ElectricCharge<T> charge, Time<T> time)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(time);

		T currentValue = charge.Value / time.Value;

		return ElectricCurrent<T>.Create(currentValue);
	}

	/// <summary>
	/// Calculates time from charge and current (t = Q/I).
	/// </summary>
	/// <param name="charge">The electric charge.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting time duration.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Time<T> operator /(ElectricCharge<T> charge, ElectricCurrent<T> current)
	{
		ArgumentNullException.ThrowIfNull(charge);
		ArgumentNullException.ThrowIfNull(current);

		T timeValue = charge.Value / current.Value;

		return Time<T>.Create(timeValue);
	}
}
