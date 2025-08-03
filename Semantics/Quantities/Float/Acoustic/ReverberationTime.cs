// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a reverberation time quantity with float precision.
/// </summary>
public sealed record ReverberationTime
{
	/// <summary>Gets the underlying generic reverberation time instance.</summary>
	public Generic.ReverberationTime<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReverberationTime"/> class.
	/// </summary>
	public ReverberationTime() { }

	/// <summary>
	/// Creates a new ReverberationTime from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime FromSeconds(float seconds) => new() { Value = Generic.ReverberationTime<float>.FromSeconds(seconds) };

	/// <summary>
	/// Creates a new ReverberationTime from T60 measurement.
	/// </summary>
	/// <param name="t60">The T60 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime FromT60(float t60) => new() { Value = Generic.ReverberationTime<float>.FromT60(t60) };

	/// <summary>
	/// Creates a new ReverberationTime from T30 measurement (extrapolated to T60).
	/// </summary>
	/// <param name="t30">The T30 time in seconds.</param>
	/// <returns>A new ReverberationTime instance.</returns>
	public static ReverberationTime FromT30(float t30) => new() { Value = Generic.ReverberationTime<float>.FromT30(t30) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
