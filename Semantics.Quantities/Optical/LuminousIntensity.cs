// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents luminous intensity with a specific unit of measurement.
/// Luminous intensity is the luminous flux emitted per unit solid angle in a given direction.
/// It is one of the seven SI base units.
/// </summary>
/// <typeparam name="T">The numeric type for the luminous intensity value.</typeparam>
public sealed record LuminousIntensity<T> : PhysicalQuantity<LuminousIntensity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of luminous intensity [J].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.LuminousIntensity;

	/// <summary>Initializes a new instance of the <see cref="LuminousIntensity{T}"/> class.</summary>
	public LuminousIntensity() : base() { }

	/// <summary>Creates a new luminous intensity from a value in candelas.</summary>
	/// <param name="candelas">The luminous intensity in candelas.</param>
	/// <returns>A new LuminousIntensity instance.</returns>
	public static LuminousIntensity<T> FromCandelas(T candelas) => Create(candelas);

	/// <summary>Creates a new luminous intensity from a value in millicandelas.</summary>
	/// <param name="millicandelas">The luminous intensity in millicandelas.</param>
	/// <returns>A new LuminousIntensity instance.</returns>
	public static LuminousIntensity<T> FromMillicandelas(T millicandelas)
	{
		T candelas = millicandelas / T.CreateChecked(1000);
		return Create(candelas);
	}

	/// <summary>Calculates luminous flux from luminous intensity and solid angle.</summary>
	/// <param name="solidAngle">The solid angle in steradians.</param>
	/// <returns>The luminous flux.</returns>
	/// <remarks>
	/// Uses the relationship: Φ = I × Ω
	/// where Φ is luminous flux, I is luminous intensity, and Ω is solid angle.
	/// </remarks>
	public LuminousFlux<T> CalculateLuminousFlux(T solidAngle)
	{
		T intensity = In(Units.Candela);
		T flux = intensity * solidAngle;
		return LuminousFlux<T>.Create(flux);
	}

	/// <summary>Calculates illuminance at a distance from a point source.</summary>
	/// <param name="distance">The distance from the source.</param>
	/// <returns>The illuminance at the specified distance.</returns>
	/// <remarks>
	/// Uses the inverse square law: E = I / r²
	/// where E is illuminance, I is luminous intensity, and r is distance.
	/// </remarks>
	public Illuminance<T> CalculateIlluminanceAtDistance(Length<T> distance)
	{
		Guard.NotNull(distance);

		T intensity = In(Units.Candela);
		T distanceMeters = distance.In(Units.Meter);
		T illuminance = intensity / (distanceMeters * distanceMeters);
		return Illuminance<T>.Create(illuminance);
	}
}
