// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents illuminance with a specific unit of measurement.
/// Illuminance is the luminous flux incident on a surface per unit area.
/// It is measured in lux (lx).
/// </summary>
/// <typeparam name="T">The numeric type for the illuminance value.</typeparam>
public record Illuminance<T> : PhysicalQuantity<Illuminance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of illuminance [J L⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Illuminance;

	/// <summary>Initializes a new instance of the <see cref="Illuminance{T}"/> class.</summary>
	public Illuminance() : base() { }

	/// <summary>Creates a new illuminance from a value in lux.</summary>
	/// <param name="lux">The illuminance in lux.</param>
	/// <returns>A new Illuminance instance.</returns>
	public static Illuminance<T> FromLux(T lux) => Create(lux);

	/// <summary>Creates a new illuminance from a value in foot-candles.</summary>
	/// <param name="footCandles">The illuminance in foot-candles.</param>
	/// <returns>A new Illuminance instance.</returns>
	public static Illuminance<T> FromFootCandles(T footCandles)
	{
		T lux = footCandles * PhysicalConstants.Generic.FootCandlesToLux<T>();
		return Create(lux);
	}

	/// <summary>Creates a new illuminance from a value in millilux.</summary>
	/// <param name="millilux">The illuminance in millilux.</param>
	/// <returns>A new Illuminance instance.</returns>
	public static Illuminance<T> FromMillilux(T millilux)
	{
		T lux = millilux / T.CreateChecked(1000);
		return Create(lux);
	}

	/// <summary>Creates a new illuminance from a value in kilolux.</summary>
	/// <param name="kilolux">The illuminance in kilolux.</param>
	/// <returns>A new Illuminance instance.</returns>
	public static Illuminance<T> FromKilolux(T kilolux)
	{
		T lux = kilolux * T.CreateChecked(1000);
		return Create(lux);
	}

	/// <summary>Calculates luminous flux from illuminance and area.</summary>
	/// <param name="area">The illuminated area.</param>
	/// <returns>The luminous flux.</returns>
	/// <remarks>
	/// Uses the relationship: Φ = E × A
	/// where Φ is luminous flux, E is illuminance, and A is area.
	/// </remarks>
	public LuminousFlux<T> CalculateLuminousFlux(Area<T> area)
	{
		ArgumentNullException.ThrowIfNull(area);

		T illuminance = In(Units.Lux);
		T areaSquareMeters = area.In(Units.SquareMeter);
		T flux = illuminance * areaSquareMeters;
		return LuminousFlux<T>.Create(flux);
	}

	/// <summary>Calculates the distance from a point source given luminous intensity.</summary>
	/// <param name="luminousIntensity">The luminous intensity of the source.</param>
	/// <returns>The distance from the source.</returns>
	/// <remarks>
	/// Uses the inverse square law: r = √(I / E)
	/// where r is distance, I is luminous intensity, and E is illuminance.
	/// </remarks>
	public Length<T> CalculateDistanceFromSource(LuminousIntensity<T> luminousIntensity)
	{
		ArgumentNullException.ThrowIfNull(luminousIntensity);

		T illuminance = In(Units.Lux);
		T intensity = luminousIntensity.In(Units.Candela);
		T distance = T.CreateChecked(Math.Sqrt(double.CreateChecked(intensity / illuminance)));
		return Length<T>.Create(distance);
	}
}
