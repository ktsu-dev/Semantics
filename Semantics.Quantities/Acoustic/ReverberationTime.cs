// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a reverberation time quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ReverberationTime<T> : PhysicalQuantity<ReverberationTime<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of reverberationtime [T].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ReverberationTime;

	/// <summary>
	/// Initializes a new instance of the <see cref="ReverberationTime{T}"/> class.
	/// </summary>
	public ReverberationTime() : base() { }

	/// <summary>
	/// Creates a new ReverberationTime from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime<T> FromSeconds(T seconds) => Create(seconds);

	/// <summary>
	/// Creates a new ReverberationTime from T60 measurement.
	/// </summary>
	/// <param name="t60">The T60 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime<T> FromT60(T t60) => Create(t60);

	/// <summary>
	/// Creates a new ReverberationTime from T30 measurement (extrapolated to T60).
	/// </summary>
	/// <param name="t30">The T30 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime<T> FromT30(T t30) => Create(t30 * T.CreateChecked(2));

	/// <summary>
	/// Calculates reverberation time using Sabine formula: RT = 0.161 * V / A.
	/// </summary>
	/// <param name="volume">The room volume.</param>
	/// <param name="totalAbsorption">The total absorption.</param>
	/// <returns>The calculated reverberation time.</returns>
	public static ReverberationTime<T> CalculateSabine(Volume<T> volume, T totalAbsorption)
	{
		Ensure.NotNull(volume);
		return Create(T.CreateChecked(0.161) * volume.Value / totalAbsorption);
	}

	/// <summary>
	/// Converts ReverberationTime to Time.
	/// </summary>
	/// <returns>The equivalent time.</returns>
	public Time<T> ToTime() => Time<T>.Create(Value);
}
