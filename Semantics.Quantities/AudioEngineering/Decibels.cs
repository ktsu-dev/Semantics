// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="Decibels{T}"/>; the logarithmic core (including the
/// <see cref="Gain{T}"/> and power-<see cref="Ratio{T}"/> conversions) is generated
/// from <c>logarithmic.json</c>.
/// </summary>
public readonly partial record struct Decibels<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets a level of zero decibels (unity).</summary>
	public static Decibels<T> Unity => new(T.Zero);

	/// <summary>
	/// Creates a level from a raw linear amplitude ratio using <c>dB = 20·log10(ratio)</c>.
	/// </summary>
	/// <param name="amplitude">The linear amplitude ratio.</param>
	/// <returns>A new <see cref="Decibels{T}"/>. An amplitude of zero maps to negative infinity where <typeparamref name="T"/> can hold it.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><typeparamref name="T"/> cannot hold the level, as for an amplitude of zero with <see langword="decimal"/> or integer storage.</exception>
	public static Decibels<T> FromAmplitude(T amplitude)
	{
		double linear = double.CreateChecked(amplitude);
		return new(LogarithmicStorage.FromLinear<T>(20.0 * Math.Log10(linear), nameof(amplitude)));
	}

	/// <summary>
	/// Creates a level from a raw linear power ratio using <c>dB = 10·log10(ratio)</c>.
	/// </summary>
	/// <param name="power">The linear power ratio.</param>
	/// <returns>A new <see cref="Decibels{T}"/>. A power of zero maps to negative infinity where <typeparamref name="T"/> can hold it.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><typeparamref name="T"/> cannot hold the level, as for a power of zero with <see langword="decimal"/> or integer storage.</exception>
	public static Decibels<T> FromPower(T power)
	{
		double linear = double.CreateChecked(power);
		return new(LogarithmicStorage.FromLinear<T>(10.0 * Math.Log10(linear), nameof(power)));
	}
}
