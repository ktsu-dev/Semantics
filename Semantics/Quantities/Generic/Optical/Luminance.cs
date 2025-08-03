// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents luminance with a specific unit of measurement.
/// Luminance is the luminous intensity per unit projected area in a given direction.
/// It is measured in candela per square meter (cd/m²).
/// </summary>
/// <typeparam name="T">The numeric type for the luminance value.</typeparam>
public record Luminance<T> : PhysicalQuantity<Luminance<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of luminance [J L⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Luminance;

	/// <summary>Initializes a new instance of the <see cref="Luminance{T}"/> class.</summary>
	public Luminance() : base() { }

	/// <summary>Creates a new luminance from a value in candela per square meter.</summary>
	/// <param name="candelaPerSquareMeter">The luminance in cd/m².</param>
	/// <returns>A new Luminance instance.</returns>
	public static Luminance<T> FromCandelaPerSquareMeter(T candelaPerSquareMeter) => Create(candelaPerSquareMeter);

	/// <summary>Creates a new luminance from a value in nits (same as cd/m²).</summary>
	/// <param name="nits">The luminance in nits.</param>
	/// <returns>A new Luminance instance.</returns>
	public static Luminance<T> FromNits(T nits) => Create(nits);

	/// <summary>Creates a new luminance from a value in foot-lamberts.</summary>
	/// <param name="footLamberts">The luminance in foot-lamberts.</param>
	/// <returns>A new Luminance instance.</returns>
	public static Luminance<T> FromFootLamberts(T footLamberts)
	{
		T candelaPerSquareMeter = footLamberts * PhysicalConstants.Generic.FootLambertsToCandelasPerSquareMeter<T>();
		return Create(candelaPerSquareMeter);
	}

	/// <summary>Creates a new luminance from a value in stilbs.</summary>
	/// <param name="stilbs">The luminance in stilbs.</param>
	/// <returns>A new Luminance instance.</returns>
	public static Luminance<T> FromStilbs(T stilbs)
	{
		T candelaPerSquareMeter = stilbs * T.CreateChecked(10000);
		return Create(candelaPerSquareMeter);
	}

	/// <summary>Calculates luminous intensity from luminance and projected area.</summary>
	/// <param name="projectedArea">The projected area in the viewing direction.</param>
	/// <returns>The luminous intensity.</returns>
	/// <remarks>
	/// Uses the relationship: I = L × A_proj
	/// where I is luminous intensity, L is luminance, and A_proj is projected area.
	/// </remarks>
	public LuminousIntensity<T> CalculateLuminousIntensity(Area<T> projectedArea)
	{
		ArgumentNullException.ThrowIfNull(projectedArea);

		T luminance = In(Units.CandelaPerSquareMeter);
		T areaSquareMeters = projectedArea.In(Units.SquareMeter);
		T intensity = luminance * areaSquareMeters;
		return LuminousIntensity<T>.Create(intensity);
	}

	/// <summary>Calculates illuminance from luminance assuming a Lambertian surface.</summary>
	/// <returns>The illuminance for a Lambertian surface.</returns>
	/// <remarks>
	/// For a perfect Lambertian surface: E = π × L
	/// where E is illuminance and L is luminance.
	/// </remarks>
	public Illuminance<T> CalculateIlluminanceForLambertianSurface()
	{
		T luminance = In(Units.CandelaPerSquareMeter);
		T illuminance = luminance * T.CreateChecked(Math.PI);
		return Illuminance<T>.Create(illuminance);
	}
}
