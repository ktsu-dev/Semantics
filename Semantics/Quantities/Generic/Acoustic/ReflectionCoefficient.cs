// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a reflection coefficient quantity with compile-time dimensional safety.
/// Reflection coefficient is the ratio of reflected to incident sound energy at an interface.
/// </summary>
public record ReflectionCoefficient<T> : PhysicalQuantity<ReflectionCoefficient<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of reflection coefficient [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ReflectionCoefficient;

	/// <summary>
	/// Initializes a new instance of the ReflectionCoefficient class.
	/// </summary>
	public ReflectionCoefficient() : base() { }

	/// <summary>
	/// Creates a new ReflectionCoefficient from a value (0 to 1).
	/// </summary>
	/// <param name="coefficient">The reflection coefficient (0 = perfect absorption, 1 = perfect reflection).</param>
	/// <returns>A new ReflectionCoefficient instance.</returns>
	public static ReflectionCoefficient<T> FromCoefficient(T coefficient) => Create(coefficient);

	/// <summary>
	/// Creates a ReflectionCoefficient from an absorption coefficient.
	/// R = 1 - α (where α is the absorption coefficient)
	/// </summary>
	/// <param name="absorptionCoefficient">The absorption coefficient.</param>
	/// <returns>The corresponding reflection coefficient.</returns>
	public static ReflectionCoefficient<T> FromAbsorptionCoefficient(SoundAbsorption<T> absorptionCoefficient)
	{
		ArgumentNullException.ThrowIfNull(absorptionCoefficient);
		return FromCoefficient(T.One - absorptionCoefficient.Value);
	}

	/// <summary>
	/// Gets the corresponding absorption coefficient.
	/// α = 1 - R
	/// </summary>
	/// <returns>The absorption coefficient.</returns>
	public SoundAbsorption<T> ToAbsorptionCoefficient() => SoundAbsorption<T>.Create(T.One - Value);

	/// <summary>
	/// Calculates the reflected sound pressure amplitude ratio.
	/// For normal incidence: r = (Z₂ - Z₁) / (Z₂ + Z₁)
	/// </summary>
	/// <param name="impedance1">Acoustic impedance of first medium.</param>
	/// <param name="impedance2">Acoustic impedance of second medium.</param>
	/// <returns>The pressure reflection coefficient.</returns>
	public static ReflectionCoefficient<T> FromImpedances(AcousticImpedance<T> impedance1, AcousticImpedance<T> impedance2)
	{
		ArgumentNullException.ThrowIfNull(impedance1);
		ArgumentNullException.ThrowIfNull(impedance2);

		T numerator = impedance2.Value - impedance1.Value;
		T denominator = impedance2.Value + impedance1.Value;
		T reflectionCoeff = numerator / denominator;
		return FromCoefficient(reflectionCoeff);
	}

	/// <summary>
	/// Calculates the transmission coefficient.
	/// T = 1 - R (for energy)
	/// </summary>
	/// <returns>The transmission coefficient.</returns>
	public T TransmissionCoefficient() => T.One - Value;

	/// <summary>
	/// Calculates reflection at oblique incidence (simplified Fresnel equation).
	/// </summary>
	/// <param name="incidenceAngle">Angle of incidence in radians.</param>
	/// <param name="impedanceRatio">Ratio of acoustic impedances Z₂/Z₁.</param>
	/// <returns>The oblique reflection coefficient.</returns>
	public static ReflectionCoefficient<T> AtObliqueIncidence(T incidenceAngle, T impedanceRatio)
	{
		T sinTheta = T.CreateChecked(Math.Sin(double.CreateChecked(incidenceAngle)));

		// Simplified calculation for oblique incidence
		T normalReflection = (impedanceRatio - T.One) / (impedanceRatio + T.One);
		T obliqueCorrection = T.One - (sinTheta * sinTheta / (impedanceRatio * impedanceRatio));
		T obliqueReflection = normalReflection * obliqueCorrection;

		return FromCoefficient(obliqueReflection);
	}
}
