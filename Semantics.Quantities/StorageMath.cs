// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Numerics;

/// <summary>
/// Roots at the precision of an arbitrary <see cref="INumber{TSelf}"/>, which
/// <see cref="System.Numerics"/> declares only for the types that can satisfy
/// <see cref="IRootFunctions{TSelf}"/>.
/// </summary>
/// <remarks>
/// <para>
/// The generated quantities call these for <c>Length()</c> and <c>Distance()</c>. They are public
/// because an application doing its own vector math over quantities — a physics integrator, an orbit
/// propagator, anything computing a norm the library does not already emit — otherwise has to
/// reimplement them, and no constraint beyond <see cref="INumber{TSelf}"/> is needed, which is the
/// whole trick: <see cref="decimal"/> could not meet <see cref="IRootFunctions{TSelf}"/> anyway.
/// </para>
/// <para>
/// What every method here guarantees, so that none of it has to be discovered:
/// </para>
/// <list type="bullet">
/// <item>
/// A root is refined by Newton steps in <c>T</c>'s own arithmetic, from a seed within
/// a factor of two of the answer. At most 256 steps are taken; an estimate still moving after that is
/// cycling rather than settling, and is reported as <see cref="ArithmeticException"/> rather than
/// returned as an answer.
/// </item>
/// <item>
/// The binary floating point primitives round trip through <see cref="double"/>. So
/// <c>Sqrt&lt;double&gt;</c> is exactly <see cref="Math.Sqrt(double)"/> and <c>Cbrt&lt;double&gt;</c>
/// exactly <see cref="Math.Cbrt(double)"/>, not a refinement of either: a caller expecting digits
/// beyond what <see cref="double"/> holds will not get them from a <see cref="double"/> quantity.
/// </item>
/// <item>
/// An integer type returns the floor of the root. <see cref="Sqrt{T}(T)"/> reaches it through
/// <see cref="double"/> for the integer primitives, which is the route the generated code always
/// inlined; <see cref="Cbrt{T}(T)"/> and <see cref="RootN{T}(T, int)"/> refine every integer type in
/// integer arithmetic, so their floor is exact rather than whatever
/// <see cref="Math.Pow(double, double)"/> happened to round to.
/// </item>
/// <item>
/// A root with no real answer — an even root of a negative value — behaves as the
/// <see cref="double"/> round trip always did: <see cref="double.NaN"/> where
/// <c>T</c> has one, and <see cref="OverflowException"/> where it does not, as
/// <see cref="decimal"/> does not.
/// </item>
/// </list>
/// </remarks>
public static class StorageMath
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
	public static T Sqrt<T>(T value)
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
	/// Computes the cube root of a storage value at the precision of its type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of. A negative value has a real cube root and gets it.</param>
	/// <returns>The cube root of <paramref name="value"/>, or its floor towards zero for an integer type.</returns>
	/// <exception cref="ArithmeticException">
	/// The Newton steps do not settle on a root within 256 iterations.
	/// </exception>
	/// <remarks>
	/// The binary floating point primitives take <see cref="Math.Cbrt(double)"/>. Every other type,
	/// integers included, is refined in its own arithmetic, so a <see cref="decimal"/> cube root has the
	/// digits the type holds and an integer one is the exact floor.
	/// </remarks>
	public static T Cbrt<T>(T value)
		where T : struct, INumber<T>
		=> RootN(value, 3);

	/// <summary>
	/// Computes the <paramref name="n"/>th root of a storage value at the precision of its type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of.</param>
	/// <param name="n">The degree of the root. An odd degree accepts a negative <paramref name="value"/>; an even one does not.</param>
	/// <returns>
	/// The <paramref name="n"/>th root of <paramref name="value"/>, or its floor towards zero for an
	/// integer type.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="n"/> is not positive.</exception>
	/// <exception cref="OverflowException">
	/// <paramref name="n"/> is even and <paramref name="value"/> is negative, and <typeparamref name="T"/>
	/// cannot represent the <see cref="double.NaN"/> that results.
	/// </exception>
	/// <exception cref="ArithmeticException">
	/// The Newton steps do not settle on a root within 256 iterations, which a type too narrow to hold
	/// the intermediate <c>estimate^(n-1)</c> can cause.
	/// </exception>
	/// <remarks>
	/// <para>
	/// A degree of one returns <paramref name="value"/> and a degree of two is
	/// <see cref="Sqrt{T}(T)"/>, contract and all. Any other degree is seeded through
	/// <see cref="double"/> and refined by the Newton step
	/// <c>x = (((n - 1) * x) + (value / x^(n - 1))) / n</c> in <typeparamref name="T"/>'s arithmetic.
	/// </para>
	/// <para>
	/// The seed for a value outside the range of <see cref="double"/> is taken by scaling by powers of
	/// <c>2^n</c> into <c>[1, 2^n)</c>, rooting there, and scaling the root back by the matching power of
	/// two — the same trick <see cref="Sqrt{T}(T)"/> plays with powers of four, and for the same reason:
	/// without it a <see cref="System.Numerics.BigInteger"/> of 2^2048 spends every step on halving.
	/// </para>
	/// </remarks>
	public static T RootN<T>(T value, int n)
		where T : struct, INumber<T>
	{
		if (n < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(n), n, "The degree of a root is a positive integer.");
		}

		if (n == 1)
		{
			return value;
		}

		if (n == 2)
		{
			return Sqrt(value);
		}

		if (T.IsNegative(value))
		{
			// An even root of a negative value has no real answer, so it keeps the behaviour of the
			// double round trip: NaN where the type has one, and an exception where it does not.
			return int.IsEvenInteger(n)
				? T.CreateChecked(Math.Pow(double.CreateChecked(value), 1d / n))
				: -RootN(-value, n);
		}

		if (T.IsZero(value) || value == T.One)
		{
			return value;
		}

		return IsBinaryFloatingPoint<T>()
			? T.CreateChecked(RootThroughDouble(double.CreateChecked(value), n))
			: RootByNewton(value, n);
	}

	/// <summary>
	/// Computes the length of the hypotenuse of a right triangle at the precision of the storage type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="x">One leg of the triangle.</param>
	/// <param name="y">The other leg of the triangle.</param>
	/// <returns>The square root of <c>x² + y²</c>, or its floor for an integer type.</returns>
	/// <exception cref="ArithmeticException">
	/// The Newton steps do not settle on a root within 256 iterations.
	/// </exception>
	/// <remarks>
	/// <para>
	/// A type with a fractional part is computed as <c>larger * Sqrt(1 + (smaller / larger)²)</c>, so a
	/// pair whose squares would leave the range of the type still has its hypotenuse. An integer type
	/// squares and sums directly, because the ratio of two integers is not a ratio, and the sum is
	/// therefore bounded by the type exactly as the caller's own <c>x * x + y * y</c> would be.
	/// </para>
	/// <para>
	/// The primitives take the <see cref="double"/> round trip, so <c>Hypot&lt;double&gt;</c> is exactly
	/// <see cref="double.Hypot(double, double)"/>.
	/// </para>
	/// </remarks>
	public static T Hypot<T>(T x, T y)
		where T : struct, INumber<T>
	{
		if (IsRoundedThroughDouble<T>())
		{
			return T.CreateChecked(double.Hypot(double.CreateChecked(x), double.CreateChecked(y)));
		}

		T larger = T.Max(T.Abs(x), T.Abs(y));
		T smaller = T.Min(T.Abs(x), T.Abs(y));

		if (T.IsZero(larger))
		{
			return T.Zero;
		}

		if (HasFloorDivision<T>())
		{
			return Sqrt((x * x) + (y * y));
		}

		T ratio = smaller / larger;
		return larger * Sqrt(T.One + (ratio * ratio));
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
	/// Takes the square root of a value through <see cref="double"/> when the value converts to a normal
	/// <see cref="double"/> and the root converts back to something other than zero.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The positive value.</param>
	/// <param name="root">The root in <typeparamref name="T"/>, or the default when this fails.</param>
	/// <returns><see langword="true"/> when <paramref name="root"/> holds a usable estimate.</returns>
	private static bool TryRootThroughDouble<T>(T value, out T root)
		where T : struct, INumber<T>
		=> TryRootThroughDouble(value, 2, out root);

	/// <summary>
	/// Takes the <paramref name="degree"/>th root of a value through <see cref="double"/> when the value
	/// converts to a normal <see cref="double"/> and the root converts back to something other than zero.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The positive value.</param>
	/// <param name="degree">The degree of the root.</param>
	/// <param name="root">The root in <typeparamref name="T"/>, or the default when this fails.</param>
	/// <returns><see langword="true"/> when <paramref name="root"/> holds a usable estimate.</returns>
	private static bool TryRootThroughDouble<T>(T value, int degree, out T root)
		where T : struct, INumber<T>
	{
		try
		{
			double asDouble = double.CreateChecked(value);
			if (double.IsNormal(asDouble))
			{
				root = T.CreateChecked(RootThroughDouble(asDouble, degree));
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

	/// <summary>
	/// Takes a root in <see cref="double"/>, by the most accurate route the framework offers for its degree.
	/// </summary>
	/// <param name="value">The value to take the root of.</param>
	/// <param name="degree">The degree of the root.</param>
	/// <returns>The root of <paramref name="value"/>.</returns>
	/// <remarks>
	/// <see cref="Math.Sqrt(double)"/> and <see cref="Math.Cbrt(double)"/> are correctly rounded where
	/// <see cref="Math.Pow(double, double)"/> is not, and <see cref="Math.Pow(double, double)"/> answers
	/// <see cref="double.NaN"/> for a negative value however odd the degree, so the sign is taken out
	/// first.
	/// </remarks>
	private static double RootThroughDouble(double value, int degree)
	{
		if (degree == 2)
		{
			return Math.Sqrt(value);
		}

		if (degree == 3)
		{
			return Math.Cbrt(value);
		}

		return double.IsNegative(value) && int.IsOddInteger(degree)
			? -Math.Pow(-value, 1d / degree)
			: Math.Pow(value, 1d / degree);
	}

	/// <summary>
	/// Reports whether <typeparamref name="T"/> is one of the binary floating point primitives, whose
	/// root is already as precise as the type once it has been taken in <see cref="double"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns><see langword="true"/> for <see cref="double"/>, <see cref="float"/> and <see cref="Half"/>.</returns>
	private static bool IsBinaryFloatingPoint<T>()
		where T : struct, INumber<T>
		=> typeof(T) == typeof(double)
			|| typeof(T) == typeof(float)
			|| typeof(T) == typeof(Half);

	/// <summary>
	/// Reports whether division in <typeparamref name="T"/> discards the fractional part, which is what
	/// makes a root in it the floor of the root rather than the root.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns><see langword="true"/> when one divided by two is zero.</returns>
	/// <remarks>
	/// Asked of the arithmetic rather than of a list of types, so a numeric type written outside this
	/// library is classified by how it behaves.
	/// </remarks>
	private static bool HasFloorDivision<T>()
		where T : struct, INumber<T>
		=> T.IsZero(T.One / (T.One + T.One));

	/// <summary>
	/// Refines the <paramref name="degree"/>th root of a positive value by Newton steps in
	/// <typeparamref name="T"/>'s own arithmetic, by whichever of the two disciplines the type's
	/// division calls for.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of, greater than zero and not one.</param>
	/// <param name="degree">The degree of the root, three or more.</param>
	/// <returns>The root, or its floor for a type whose division floors.</returns>
	/// <exception cref="ArithmeticException">The estimate does not settle within <see cref="MaximumIterations"/> steps.</exception>
	/// <remarks>
	/// Every power either loop takes is checked, because a narrow type can hold a value whose
	/// <c>estimate^(degree - 1)</c> it cannot, and an estimate that wraps sends the steps somewhere
	/// arbitrary. The step itself is <c>x = (((degree - 1) * x) + (value / x^(degree - 1))) / degree</c>
	/// in both.
	/// </remarks>
	private static T RootByNewton<T>(T value, int degree)
		where T : struct, INumber<T>
	{
		T two = T.One + T.One;

		return HasFloorDivision<T>()
			? RootByDescent(value, degree, two)
			: RootBySettling(value, degree, two);
	}

	/// <summary>
	/// Takes the root in a type whose division floors, by descending to it.
	/// </summary>
	/// <typeparam name="T">The numeric storage type, whose division floors.</typeparam>
	/// <param name="value">The value to take the root of, greater than one.</param>
	/// <param name="degree">The degree of the root, three or more.</param>
	/// <param name="two">Two, in <typeparamref name="T"/>.</param>
	/// <returns>The floor of the root.</returns>
	/// <exception cref="ArithmeticException">The descent does not reach the root within <see cref="MaximumIterations"/> steps.</exception>
	/// <remarks>
	/// From an estimate at or above the root every step lands no lower than the floor of the root, so
	/// the first step that does not descend has passed it and the estimate before it is the answer. The
	/// seed is lifted to at or above the root first, since a descent that starts below it would stop on
	/// the first step and answer with the seed. A degree so high that the type cannot hold
	/// <c>2^degree</c> is answered before any of that: no value the type holds has a root of two or
	/// more, and the values with a root below one were answered before this was called.
	/// </remarks>
	private static T RootByDescent<T>(T value, int degree, T two)
		where T : struct, INumber<T>
	{
		if (!TryPower(two, degree, out _))
		{
			return T.One;
		}

		T count = T.CreateChecked(degree);
		T countLessOne = count - T.One;
		T estimate = SeedForRoot(value, degree, two);

		// An estimate whose own power overflows is above the root, since the root's power divides a
		// value the type holds, so the lift stops there as well as on a power that exceeds the value.
		while (TryPower(estimate, degree, out T raised) && raised < value)
		{
			estimate *= two;
		}

		for (int iteration = 0; iteration < MaximumIterations; iteration++)
		{
			if (!TryPower(estimate, degree - 1, out T power) || T.IsZero(power))
			{
				// The estimate is too large for its own power to be taken in the type. Halving reaches a
				// range the type holds, and stays at or above the root.
				estimate = (estimate + T.One) / two;
				continue;
			}

			T next = ((countLessOne * estimate) + (value / power)) / count;

			if (next >= estimate)
			{
				return estimate;
			}

			estimate = next;
		}

		throw new ArithmeticException($"The root of degree {degree} did not reach its floor within {MaximumIterations} Newton steps.");
	}

	/// <summary>
	/// Takes the root in a type that keeps a fractional part, by settling on it.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to take the root of, greater than zero and not one.</param>
	/// <param name="degree">The degree of the root, three or more.</param>
	/// <param name="two">Two, in <typeparamref name="T"/>.</param>
	/// <returns>The root, to the precision the type holds.</returns>
	/// <exception cref="ArithmeticException">The estimate does not settle within <see cref="MaximumIterations"/> steps.</exception>
	/// <remarks>
	/// On the same terms as <see cref="Sqrt{T}(T)"/>: an estimate that stops changing is the root, and
	/// one alternating between two neighbours is rounding, so the smaller of the two is taken.
	/// </remarks>
	private static T RootBySettling<T>(T value, int degree, T two)
		where T : struct, INumber<T>
	{
		T count = T.CreateChecked(degree);
		T countLessOne = count - T.One;
		T estimate = SeedForRoot(value, degree, two);
		T previous = estimate;

		for (int iteration = 0; iteration < MaximumIterations; iteration++)
		{
			if (!TryPower(estimate, degree - 1, out T power) || T.IsZero(power))
			{
				// The estimate is too far from the root for its own power to be taken in the type.
				estimate = (estimate + T.One) / two;
				continue;
			}

			T next = ((countLessOne * estimate) + (value / power)) / count;

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

		throw new ArithmeticException($"The root of degree {degree} did not settle within {MaximumIterations} Newton steps.");
	}

	/// <summary>
	/// Chooses the first Newton estimate for the <paramref name="degree"/>th root of a positive value.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The positive value whose root is wanted.</param>
	/// <param name="degree">The degree of the root.</param>
	/// <param name="two">Two, in <typeparamref name="T"/>.</param>
	/// <returns>An estimate within a factor of two of the root.</returns>
	/// <remarks>
	/// The <see cref="double"/> root is used directly when <paramref name="value"/> converts to a normal
	/// <see cref="double"/>. Otherwise the value is scaled by powers of <c>2^degree</c> into
	/// <c>[1, 2^degree)</c>, the root is taken there, and that root is scaled back by the matching power
	/// of two — <see cref="Seed{T}(T, T)"/>'s trick for a degree other than two.
	/// </remarks>
	private static T SeedForRoot<T>(T value, int degree, T two)
		where T : struct, INumber<T>
	{
		if (TryRootThroughDouble(value, degree, out T direct))
		{
			return direct;
		}

		if (!TryPower(two, degree, out T scale))
		{
			// The type cannot hold 2^degree, so it cannot hold a value needing this scaling either.
			return T.One;
		}

		T scaled = value;
		int powerOfTwo = 0;

		while (scaled >= scale)
		{
			scaled /= scale;
			powerOfTwo++;
		}

		while (scaled < T.One)
		{
			scaled *= scale;
			powerOfTwo--;
		}

		T root = TryRootThroughDouble(scaled, degree, out T scaledRoot) ? scaledRoot : T.One;

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
	/// Raises a value to a non-negative integer power, reporting rather than throwing when the result
	/// leaves the range of the type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="value">The value to raise.</param>
	/// <param name="exponent">The power to raise it to.</param>
	/// <param name="power">The result, or the default when it does not fit.</param>
	/// <returns><see langword="true"/> when <paramref name="power"/> holds the result.</returns>
	/// <remarks>
	/// The multiplications are checked, so a narrow type reports the overflow instead of wrapping into a
	/// number that would send the Newton steps somewhere arbitrary.
	/// </remarks>
	private static bool TryPower<T>(T value, int exponent, out T power)
		where T : struct, INumber<T>
	{
		T result = T.One;
		T factor = value;

		try
		{
			checked
			{
				for (int remaining = exponent; remaining > 0; remaining >>= 1)
				{
					if (int.IsOddInteger(remaining))
					{
						result *= factor;
					}

					if (remaining > 1)
					{
						factor *= factor;
					}
				}
			}
		}
		catch (OverflowException)
		{
			power = default;
			return false;
		}

		power = result;
		return true;
	}
}
