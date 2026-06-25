// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="PH{T}"/>; the logarithmic core (including the
/// <see cref="Concentration{T}"/> conversions) is generated from <c>logarithmic.json</c>.
/// </summary>
public readonly partial record struct PH<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the pH of pure water at 25 °C (7.0).</summary>
	public static PH<T> Neutral => new(PhysicalConstants.Generic.NeutralPH<T>());

	/// <summary>
	/// Gets the complementary pOH at 25 °C using <c>pOH = 14 − pH</c>.
	/// </summary>
	/// <returns>The pOH as a <see cref="PH{T}"/> scale value.</returns>
	public PH<T> ToPOH() => new(T.CreateChecked(14) - Value);

	/// <summary>Gets whether this value describes an acid (pH below 7).</summary>
	public bool IsAcidic => Value < PhysicalConstants.Generic.NeutralPH<T>();

	/// <summary>Gets whether this value describes a base (pH above 7).</summary>
	public bool IsBasic => Value > PhysicalConstants.Generic.NeutralPH<T>();
}
