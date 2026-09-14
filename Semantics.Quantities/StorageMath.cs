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
	/// The most Newton steps taken before the root is reported as not converging.
	/// </summary>
	/// <remarks>
	/// The seed is always within a factor of two of the root, and from there each step roughly doubles
	/// the number of correct digits, so a type would need far more digits than any real one holds to come
	/// near the cap. Reaching it means the estimate is cycling rather than settling, and the estimate at
	/// that point is not the root, so it is an error rather than an answer.
	/// </remarks>
	private const int MaximumIterations = 256;

	/// <summary>
	/// Computes the square root of a storage value at the precision of its type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of. Generated callers pass a sum of squares, which is never negative.</param>
	/// <returns>The square root of <paramref name="value"/>, or its floor for an integer type.</returns>
	/// <exception cref="OverflowException">
	/// <paramref name="value"/> is negative and <typeparamref name="T"/> cannot represent the <see cref="double.NaN"/> that results.
	/// </exception>
	/// <exception cref="ArithmeticException">
	/// The Newton steps do not settle on a root within <see cref="MaximumIterations"/>.
	/// </exception>
	/// <remarks>
	/// <para>
	/// The binary floating point and integer primitives take the route every generated
	/// <c>Length()</c> and <c>Distance()</c> took before this helper existed, a round trip through
	/// <see cref="Math.Sqrt(double)"/>, so their results are unchanged. For them that route is
	/// already as precise as the type.
	/// </para>
	/// <para>
	/// Any other type is refined in its own arithmetic: seeded near the root, then Newton steps
	/// <c>x = (x + value / x) / 2</c> until the estimate stops changing. A <see cref="decimal"/> root
	/// therefore has 28 significant digits rather than the 15 its conversion from <see cref="double"/>
	/// keeps, and no constraint beyond <see cref="INumber{TSelf}"/> is needed, which
	/// <see cref="decimal"/> could not meet had this required <see cref="IRootFunctions{TSelf}"/>.
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
		T estimate = Seed(value, two);
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

		throw new ArithmeticException($"The square root did not settle within {MaximumIterations} Newton steps.");
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
	/// <param name="two">Two, in <typeparamref name="T"/>.</param>
	/// <returns>An estimate within a factor of two of the root.</returns>
	/// <remarks>
	/// <para>
	/// The <see cref="double"/> root is used directly when <paramref name="value"/> converts to a normal
	/// <see cref="double"/>. Otherwise the value is outside the range of <see cref="double"/>, or too
	/// small for a <see cref="double"/> to hold with any precision, or the type does not convert at all.
	/// It is then scaled by powers of four into [1, 4), which every type holds, the root is taken there,
	/// and that root is scaled back by the matching power of two.
	/// </para>
	/// <para>
	/// This used to start from the value itself. Newton's method still converges from there, but each
	/// early step only halves the estimate, so a <see cref="System.Numerics.BigInteger"/> of 2^2048 used
	/// every step on halving and returned about 2^1792 as its root.
	/// </para>
	/// </remarks>
	private static T Seed<T>(T value, T two)
		where T : struct, INumber<T>
	{
		if (TryRootThroughDouble(value, out T direct))
		{
			return direct;
		}

		T four = two * two;
		T scaled = value;
		int powerOfTwo = 0;

		while (scaled >= four)
		{
			scaled /= four;
			powerOfTwo++;
		}

		while (scaled < T.One)
		{
			scaled *= four;
			powerOfTwo--;
		}

		T root = TryRootThroughDouble(scaled, out T scaledRoot) ? scaledRoot : T.One;

		for (; powerOfTwo > 0; powerOfTwo--)
		{
			root *= two;
		}

		for (; powerOfTwo < 0; powerOfTwo++)
		{
			root /= two;
		}

		return root;
	}

	/// <summary>
	/// Takes the root of a value through <see cref="double"/> when the value converts to a normal
	/// <see cref="double"/> and the root converts back to something other than zero.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The positive value.</param>
	/// <param name="root">The root in <typeparamref name="T"/>, or the default when this fails.</param>
	/// <returns><see langword="true"/> when <paramref name="root"/> holds a usable estimate.</returns>
	private static bool TryRootThroughDouble<T>(T value, out T root)
		where T : struct, INumber<T>
	{
		try
		{
			double asDouble = double.CreateChecked(value);
			if (double.IsNormal(asDouble))
			{
				root = T.CreateChecked(Math.Sqrt(asDouble));
				if (!T.IsZero(root))
				{
					return true;
				}
			}
		}
		catch (NotSupportedException)
		{
			// The type does not convert through double. The caller scales into a range that needs no conversion.
		}
		catch (OverflowException)
		{
			// The value or its root is outside the range of double or of the type. The caller scales it.
		}

		root = default;
		return false;
	}
}
