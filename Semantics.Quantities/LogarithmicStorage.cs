// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Globalization;
using System.Numerics;

/// <summary>
/// Brings the <see cref="double"/> result of a logarithmic-scale conversion into the storage type, with one
/// documented failure for a result the storage type cannot hold.
/// </summary>
/// <remarks>
/// A linear value of zero is a level of negative infinity. <see cref="double"/> and <see cref="float"/> hold
/// that; <see cref="decimal"/>, the integer types and most others do not, and <c>T.CreateChecked</c> reported it
/// as a bare <see cref="OverflowException"/> that contradicted the conversion's own documentation. The same
/// happens converting back from a level so high its linear value is past the type's range.
/// </remarks>
internal static class LogarithmicStorage
{
	/// <summary>Converts the level computed from a linear value.</summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="level">The level, which is negative infinity for a linear value of zero.</param>
	/// <param name="paramName">The name of the parameter holding the linear value.</param>
	/// <returns>The level in <typeparamref name="T"/>.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// <typeparamref name="T"/> cannot hold the level: the linear value is zero, or negative, or so near zero
	/// that its level is past the type's range.
	/// </exception>
	internal static T FromLinear<T>(double level, string paramName)
		where T : struct, INumber<T>
	{
		if (TryConvert(level, out T result))
		{
			return result;
		}

		throw new ArgumentOutOfRangeException(paramName, string.Format(
			CultureInfo.InvariantCulture,
			"The level of this value is {0}, which {1} cannot hold. A linear value of zero is negative infinity on every logarithmic scale.",
			level,
			typeof(T).Name));
	}

	/// <summary>Converts the linear value computed from a level.</summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="linear">The linear value.</param>
	/// <returns>The linear value in <typeparamref name="T"/>.</returns>
	/// <exception cref="OverflowException">The level is so high that <typeparamref name="T"/> cannot hold its linear value.</exception>
	internal static T ToLinear<T>(double linear)
		where T : struct, INumber<T>
	{
		if (TryConvert(linear, out T result))
		{
			return result;
		}

		throw new OverflowException(string.Format(
			CultureInfo.InvariantCulture,
			"The linear value of this level is {0}, which {1} cannot hold.",
			linear,
			typeof(T).Name));
	}

	private static bool TryConvert<T>(double value, out T result)
		where T : struct, INumber<T>
	{
		if (double.IsNaN(value))
		{
			result = default;
			return false;
		}

		try
		{
			result = T.CreateChecked(value);
			return true;
		}
		catch (OverflowException)
		{
			result = default;
			return false;
		}
	}
}
