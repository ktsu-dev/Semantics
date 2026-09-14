// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System;
using System.Globalization;
using System.Numerics;

/// <summary>
/// Materialises a metadata literal into a storage type at that type's own precision, for the
/// conversion factors and metric magnitudes the generated code caches per closed generic type.
/// </summary>
/// <remarks>
/// <para>
/// A factor used to reach a storage type through <c>T.CreateChecked(double)</c>, so every type
/// inherited the rounding of <see cref="double"/>, and <see cref="decimal"/> kept only the 15
/// significant digits that conversion preserves. Parsing the literal gives each type the nearest
/// value it can represent, and a fraction is divided in the storage type itself.
/// </para>
/// <para>
/// Both methods answer <see langword="null"/> when a storage type keeps the old route, and the
/// generated holder then converts the <see cref="double"/> constant with <c>T.CreateChecked</c> each
/// time the value is read. An integer type keeps it, because parsing <c>"0.3048"</c> into an
/// <see cref="int"/> fails, and dividing 5 by 9 in one answers zero for a different reason than
/// truncating 0.5555… does. A type that cannot parse the literal keeps it too.
/// </para>
/// <para>
/// Nothing here converts the <see cref="double"/>, and that is deliberate. The generated holder
/// evaluates every value for a storage type in one static initializer, so a conversion that overflowed
/// there, such as 10^12 into <see cref="int"/>, would make every value for that type throw
/// <see cref="TypeInitializationException"/>, which is what 5.2.0 shipped. Converting at the read lets
/// each value succeed or throw <see cref="OverflowException"/> on its own, as the generated factories
/// did before 5.2.0.
/// </para>
/// </remarks>
internal static class StorageLiteral
{
	/// <summary>
	/// Parses a decimal literal into <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="literal">The literal as written in the metadata, for example <c>"0.3048"</c> or <c>"1e-10"</c>.</param>
	/// <returns>
	/// The nearest value <typeparamref name="T"/> represents, or <see langword="null"/> when
	/// <typeparamref name="T"/> is an integer or cannot parse the literal.
	/// </returns>
	internal static T? Parse<T>(string literal)
		where T : struct, INumber<T>
		=> !IsIntegral<T>() && TryParseLiteral(literal, out T value) ? value : null;

	/// <summary>
	/// Divides one decimal literal by another in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="numerator">The numerator literal, for example <c>"5"</c>.</param>
	/// <param name="denominator">The denominator literal, for example <c>"9"</c>.</param>
	/// <returns>
	/// The quotient at the precision <typeparamref name="T"/> divides to, or <see langword="null"/> when
	/// <typeparamref name="T"/> is an integer, cannot parse either literal, or overflows dividing them.
	/// </returns>
	internal static T? Divide<T>(string numerator, string denominator)
		where T : struct, INumber<T>
	{
		if (IsIntegral<T>()
			|| !TryParseLiteral(numerator, out T dividend)
			|| !TryParseLiteral(denominator, out T divisor)
			|| T.IsZero(divisor))
		{
			return null;
		}

		try
		{
			return dividend / divisor;
		}
		catch (OverflowException)
		{
			// Converting the double quotient reports the same overflow, at the read that needs the value.
			return null;
		}
	}

	/// <summary>
	/// Parses a literal with <see cref="NumberStyles.Float"/>, treating a parse that throws for that style
	/// the same as one that returns <see langword="false"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="literal">The literal.</param>
	/// <param name="value">The parsed value, or the default when parsing failed.</param>
	/// <returns><see langword="true"/> when <typeparamref name="T"/> parsed the literal.</returns>
	/// <remarks>
	/// A numeric type outside the base library may throw <see cref="NotSupportedException"/> or
	/// <see cref="ArgumentException"/> for a style it does not handle, rather than return
	/// <see langword="false"/>. Only those two are caught, so any other failure still surfaces.
	/// </remarks>
	private static bool TryParseLiteral<T>(string literal, out T value)
		where T : struct, INumber<T>
	{
		try
		{
			return T.TryParse(literal, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
		}
		catch (NotSupportedException)
		{
			// The type does not parse this style. The double constant still converts.
		}
		catch (ArgumentException)
		{
			// The type rejects this style as an argument. The double constant still converts.
		}

		value = default;
		return false;
	}

	/// <summary>
	/// Reports whether <typeparamref name="T"/> discards fractions, which is to say whether a half is zero in it.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns><see langword="true"/> when one divided by two is zero in <typeparamref name="T"/>.</returns>
	private static bool IsIntegral<T>()
		where T : struct, INumber<T>
		=> T.IsZero(T.One / (T.One + T.One));
}
