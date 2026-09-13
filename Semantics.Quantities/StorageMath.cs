// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// Numeric operations the generated quantities need that <see cref="INumber{TSelf}"/> does not declare.
/// </summary>
internal static class StorageMath
{
	/// <summary>
	/// The most Newton steps taken before the current estimate is returned as it stands.
	/// </summary>
	/// <remarks>
	/// From a <see cref="double"/> seed each step roughly doubles the number of correct digits, so a
	/// handful suffice. The cap matters only when no seed could be taken and the estimate starts at
	/// the value itself, where the early steps halve it rather than refine it.
	/// </remarks>
	private const int MaximumIterations = 256;

	/// <summary>
	/// Computes the square root of a storage value at the precision of its type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of. Generated callers pass a sum of squares, which is never negative.</param>
	/// <returns>The square root of <paramref name="value"/>.</returns>
	/// <exception cref="OverflowException">
	/// <paramref name="value"/> is negative and <typeparamref name="T"/> cannot represent the <see cref="double.NaN"/> that results.
	/// </exception>
	/// <remarks>
	/// <para>
	/// The binary floating point and integer primitives take the route every generated
	/// <c>Length()</c> and <c>Distance()</c> took before this helper existed, a round trip through
	/// <see cref="Math.Sqrt(double)"/>, so their results are unchanged. For them that route is
	/// already as precise as the type.
	/// </para>
	/// <para>
	/// Any other type is refined in its own arithmetic: seeded from that same <see cref="double"/>
	/// root, then Newton steps <c>x = (x + value / x) / 2</c> until the estimate stops changing. A
	/// <see cref="decimal"/> root therefore has 28 significant digits rather than the 15 its
	/// conversion from <see cref="double"/> keeps, and no constraint beyond <see cref="INumber{TSelf}"/>
	/// is needed, which <see cref="decimal"/> could not meet had this required
	/// <see cref="IRootFunctions{TSelf}"/>.
	/// </para>
	/// <para>
	/// A negative input keeps its old behaviour too, <see cref="double.NaN"/> where the type has
	/// one and an exception where it does not.
	/// </para>
	/// </remarks>
	internal static T Sqrt<T>(T value)
		where T : struct, INumber<T>
	{
		if (IsRoundedThroughDouble<T>() || T.IsNegative(value))
		{
			return T.CreateChecked(Math.Sqrt(double.CreateChecked(value)));
		}

		if (T.IsZero(value))
		{
			return T.Zero;
		}

		T two = T.One + T.One;
		T estimate = Seed(value);
		T previous = estimate;

		for (int iteration = 0; iteration < MaximumIterations; iteration++)
		{
			T next = (estimate + (value / estimate)) / two;

			if (next == estimate)
			{
				return next;
			}

			// Rounding can leave the estimate alternating between two neighbours instead of settling.
			if (next == previous)
			{
				return T.Min(estimate, next);
			}

			previous = estimate;
			estimate = next;
		}

		return estimate;
	}

	/// <summary>
	/// Reports whether <typeparamref name="T"/> is one of the primitives whose square root has always
	/// been taken through <see cref="double"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns><see langword="true"/> for the binary floating point and integer primitives.</returns>
	/// <remarks>
	/// Written as <see langword="typeof"/> comparisons because the JIT folds them for a value type,
	/// so each specialisation of <see cref="Sqrt{T}"/> compiles down to one of its two branches.
	/// </remarks>
	private static bool IsRoundedThroughDouble<T>()
		where T : struct, INumber<T>
		=> typeof(T) == typeof(double)
			|| typeof(T) == typeof(float)
			|| typeof(T) == typeof(Half)
			|| typeof(T) == typeof(byte)
			|| typeof(T) == typeof(sbyte)
			|| typeof(T) == typeof(short)
			|| typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int)
			|| typeof(T) == typeof(uint)
			|| typeof(T) == typeof(long)
			|| typeof(T) == typeof(ulong)
			|| typeof(T) == typeof(nint)
			|| typeof(T) == typeof(nuint)
			|| typeof(T) == typeof(Int128)
			|| typeof(T) == typeof(UInt128);

	/// <summary>
	/// Chooses the first Newton estimate for a positive value.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The positive value whose root is wanted.</param>
	/// <returns>The <see cref="double"/> root converted to <typeparamref name="T"/> when both conversions succeed, otherwise the larger of <paramref name="value"/> and one.</returns>
	/// <remarks>
	/// Any positive start converges, because Newton's method for a square root approaches from above
	/// after its first step. The fallback exists for a type that will not convert to or from
	/// <see cref="double"/>, or whose value is outside its range.
	/// </remarks>
	private static T Seed<T>(T value)
		where T : struct, INumber<T>
	{
		try
		{
			double root = Math.Sqrt(double.CreateChecked(value));
			if (double.IsFinite(root) && root > 0d)
			{
				T seed = T.CreateChecked(root);
				if (!T.IsZero(seed))
				{
					return seed;
				}
			}
		}
		catch (NotSupportedException)
		{
			// The type does not convert through double. The fallback below still converges.
		}
		catch (OverflowException)
		{
			// The value or its root is outside the range of double or of the type. The fallback below still converges.
		}

		return value > T.One ? value : T.One;
	}
}
