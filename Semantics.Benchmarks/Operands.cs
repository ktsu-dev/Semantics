// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Globalization;
using System.Numerics;

/// <summary>
/// Builds the storage-type values the benchmarks run against.
/// </summary>
/// <remarks>
/// Every value is parsed from text rather than converted from a <see cref="double"/>, so that a
/// storage type wide enough to hold more than a double's 15 digits actually receives them. A
/// <see cref="decimal"/> or a <c>PreciseNumber</c> seeded through a double would be measured
/// carrying a double's worth of digits, which is the opposite of why those types are here.
/// </remarks>
internal static class Operands
{
	/// <summary>
	/// Parses a value into the storage type.
	/// </summary>
	/// <typeparam name="T">The storage type.</typeparam>
	/// <param name="text">The decimal text to parse.</param>
	/// <returns>The parsed value.</returns>
	internal static T Of<T>(string text)
		where T : struct, INumber<T> =>
		T.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
}
