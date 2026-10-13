// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// Runtime guards used by generated <see cref="IVector0{TSelf, T}"/> quantity types
/// to enforce the non-negativity invariant declared in the unified-vector model.
/// </summary>
/// <remarks>
/// Per the locked design decisions in <c>docs/strategy-unified-vector-quantities.md</c>:
/// <list type="bullet">
/// <item><description>A Vector0 quantity is always non-negative. Construction with a negative value throws <see cref="ArgumentException"/>.</description></item>
/// <item><description>The conversion from a non-base unit can flip the sign (e.g. -460&#176;F is below absolute zero in Kelvin); the guard runs after conversion to catch that.</description></item>
/// </list>
/// </remarks>
public static class Vector0Guards
{
	/// <summary>
	/// Returns <paramref name="value"/> unchanged when it is non-negative; throws
	/// <see cref="ArgumentException"/> otherwise. Used in generated <c>From{Unit}</c>
	/// factories to enforce the non-negativity invariant on Vector0 quantities.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value (already converted to the SI base unit) to validate.</param>
	/// <param name="paramName">Name of the originating parameter, used for the exception message.</param>
	/// <returns>The validated, non-negative value.</returns>
	/// <exception cref="ArgumentException">When <paramref name="value"/> is NaN or negative.</exception>
	public static T EnsureNonNegative<T>(T value, string paramName)
		where T : struct, INumber<T>
	{
		EnsureNumber(value, paramName);

		if (value < T.Zero)
		{
			throw new ArgumentException(
				$"Magnitude must be non-negative; received {value}.",
				paramName);
		}

		return value;
	}

	/// <summary>
	/// Returns <paramref name="value"/> unchanged when it is strictly positive; throws
	/// <see cref="ArgumentException"/> otherwise. Used in generated <c>From{Unit}</c>
	/// factories on V0 overloads that declare <c>physicalConstraints.minExclusive: "0"</c>
	/// in <c>dimensions.json</c> (per #51). Examples: <c>Wavelength</c>, <c>Period</c>,
	/// <c>HalfLife</c> — quantities for which zero is unphysical, distinct from the V0
	/// default that allows zero.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value (already converted to the SI base unit) to validate.</param>
	/// <param name="paramName">Name of the originating parameter, used for the exception message.</param>
	/// <returns>The validated, strictly-positive value.</returns>
	/// <exception cref="ArgumentException">When <paramref name="value"/> is NaN, zero or negative.</exception>
	public static T EnsurePositive<T>(T value, string paramName)
		where T : struct, INumber<T>
	{
		EnsureNumber(value, paramName);

		if (value <= T.Zero)
		{
			throw new ArgumentException(
				$"Value must be strictly positive; received {value}.",
				paramName);
		}

		return value;
	}

	/// <summary>
	/// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is NaN. Checked before the
	/// sign test because <c>T.Sign(NaN)</c> throws <see cref="ArithmeticException"/> for the IEEE types,
	/// and a NaN compares false against zero, so neither test rejects it on its own.
	/// </summary>
	private static void EnsureNumber<T>(T value, string paramName)
		where T : struct, INumber<T>
	{
		if (T.IsNaN(value))
		{
			throw new ArgumentException("Magnitude must be a number; received NaN.", paramName);
		}
	}
}
