// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="POH{T}"/>; the logarithmic core (including the
/// <see cref="Concentration{T}"/> conversions) is generated from <c>logarithmic.json</c>.
/// </summary>
public readonly partial record struct POH<T>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Gets the complementary pH at 25 °C using <c>pH = 14 − pOH</c>.
	/// </summary>
	/// <returns>The pH as a <see cref="PH{T}"/> scale value.</returns>
	public PH<T> ToPH() => PH<T>.Create(T.CreateChecked(14) - Value);
}
