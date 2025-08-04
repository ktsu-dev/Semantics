// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a reverberation time quantity with float precision.
/// </summary>
public sealed record ReverberationTime : Generic.ReverberationTime<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReverberationTime"/> class.
	/// </summary>
	public ReverberationTime() : base() { }

	/// <summary>
	/// Creates a new ReverberationTime from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static new ReverberationTime FromSeconds(float seconds) => new() { Quantity = seconds };

	/// <summary>
	/// Creates a new ReverberationTime from T60 measurement.
	/// </summary>
	/// <param name="t60">The T60 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static new ReverberationTime FromT60(float t60) => new() { Quantity = t60 };

	/// <summary>
	/// Creates a new ReverberationTime from T30 measurement (extrapolated to T60).
	/// </summary>
	/// <param name="t30">The T30 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static new ReverberationTime FromT30(float t30) => new() { Quantity = t30 };
}
