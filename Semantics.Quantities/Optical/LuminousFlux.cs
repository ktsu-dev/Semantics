// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents luminous flux with a specific unit of measurement.
/// Luminous flux is the measure of the perceived power of light.
/// It is measured in lumens (lm).
/// </summary>
/// <typeparam name="T">The numeric type for the luminous flux value.</typeparam>
public sealed record LuminousFlux<T> : PhysicalQuantity<LuminousFlux<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of luminous flux [J].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.LuminousFlux;

	/// <summary>Initializes a new instance of the <see cref="LuminousFlux{T}"/> class.</summary>
	public LuminousFlux() : base() { }

	/// <summary>Creates a new luminous flux from a value in lumens.</summary>
	/// <param name="lumens">The luminous flux in lumens.</param>
	/// <returns>A new LuminousFlux instance.</returns>
	public static LuminousFlux<T> FromLumens(T lumens) => Create(lumens);

	/// <summary>Creates a new luminous flux from a value in millilumens.</summary>
	/// <param name="millilumens">The luminous flux in millilumens.</param>
	/// <returns>A new LuminousFlux instance.</returns>
	public static LuminousFlux<T> FromMillilumens(T millilumens)
	{
		T lumens = millilumens / T.CreateChecked(1000);
		return Create(lumens);
	}

	/// <summary>Creates a new luminous flux from a value in kilolumens.</summary>
	/// <param name="kilolumens">The luminous flux in kilolumens.</param>
	/// <returns>A new LuminousFlux instance.</returns>
	public static LuminousFlux<T> FromKilolumens(T kilolumens)
	{
		T lumens = kilolumens * T.CreateChecked(1000);
		return Create(lumens);
	}

	/// <summary>Calculates illuminance from luminous flux and area.</summary>
	/// <param name="area">The area over which the flux is distributed.</param>
	/// <returns>The illuminance.</returns>
	/// <remarks>
	/// Uses the relationship: E = Φ / A
	/// where E is illuminance, Φ is luminous flux, and A is area.
	/// </remarks>
	public Illuminance<T> CalculateIlluminance(Area<T> area)
	{
		Guard.NotNull(area);

		T flux = In(Units.Lumen);
		T areaSquareMeters = area.In(Units.SquareMeter);
		T illuminance = flux / areaSquareMeters;
		return Illuminance<T>.Create(illuminance);
	}

	/// <summary>Calculates luminous efficacy from luminous flux and radiant flux.</summary>
	/// <param name="radiantFlux">The radiant flux (power) in watts.</param>
	/// <returns>The luminous efficacy in lumens per watt.</returns>
	/// <remarks>
	/// Uses the relationship: η = Φ / P
	/// where η is luminous efficacy, Φ is luminous flux, and P is radiant flux.
	/// </remarks>
	public T CalculateLuminousEfficacy(Power<T> radiantFlux)
	{
		Guard.NotNull(radiantFlux);

		T flux = In(Units.Lumen);
		T power = radiantFlux.In(Units.Watt);
		return flux / power;
	}

	/// <summary>
	/// Calculates illuminance from luminous flux and area (E = Φ/A).
	/// </summary>
	/// <param name="flux">The luminous flux.</param>
	/// <param name="area">The area.</param>
	/// <returns>The resulting illuminance.</returns>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "CA2225:Provide named alternates for operator overloads", Justification = "Physics relationship operators represent fundamental equations, not arithmetic")]
	public static Illuminance<T> operator /(LuminousFlux<T> flux, Area<T> area)
	{
		Guard.NotNull(flux);
		Guard.NotNull(area);

		T illuminanceValue = flux.Value / area.Value;

		return Illuminance<T>.Create(illuminanceValue);
	}
}
