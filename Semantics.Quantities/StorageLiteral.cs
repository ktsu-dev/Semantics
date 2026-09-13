// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

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
/// An integer storage type keeps the old route. Parsing <c>"0.3048"</c> into an <see cref="int"/>
/// fails, and dividing 5 by 9 in one answers zero for a different reason than truncating 0.5555…
/// does, so integers convert the <see cref="double"/> exactly as they did before. A type that
/// cannot parse the literal falls back the same way.
/// </para>
/// </remarks>
internal static class StorageLiteral
{
	/// <summary>
	/// Converts a decimal literal to <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="literal">The literal as written in the metadata, for example <c>"0.3048"</c> or <c>"1e-10"</c>.</param>
	/// <param name="fallback">The same value as a <see cref="double"/>, used when <typeparamref name="T"/> is an integer or cannot parse the literal.</param>
	/// <returns>The nearest value <typeparamref name="T"/> represents.</returns>
	internal static T Parse<T>(string literal, double fallback)
		where T : struct, INumber<T>
		=> !IsIntegral<T>() && T.TryParse(literal, NumberStyles.Float, CultureInfo.InvariantCulture, out T value)
			? value
			: T.CreateChecked(fallback);

	/// <summary>
	/// Converts an exact fraction of two decimal literals to <typeparamref name="T"/>, dividing in <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <param name="numerator">The numerator literal, for example <c>"5"</c>.</param>
	/// <param name="denominator">The denominator literal, for example <c>"9"</c>.</param>
	/// <param name="fallback">The quotient as a <see cref="double"/>, used when <typeparamref name="T"/> is an integer or cannot parse either literal.</param>
	/// <returns>The quotient at the precision <typeparamref name="T"/> divides to.</returns>
	internal static T Divide<T>(string numerator, string denominator, double fallback)
		where T : struct, INumber<T>
		=> !IsIntegral<T>()
			&& T.TryParse(numerator, NumberStyles.Float, CultureInfo.InvariantCulture, out T dividend)
			&& T.TryParse(denominator, NumberStyles.Float, CultureInfo.InvariantCulture, out T divisor)
			&& !T.IsZero(divisor)
				? dividend / divisor
				: T.CreateChecked(fallback);

	/// <summary>
	/// Reports whether <typeparamref name="T"/> discards fractions, which is to say whether a half is zero in it.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns><see langword="true"/> when one divided by two is zero in <typeparamref name="T"/>.</returns>
	private static bool IsIntegral<T>()
		where T : struct, INumber<T>
		=> T.IsZero(T.One / (T.One + T.One));
}
