// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents optical power with a specific unit of measurement.
/// Optical power is the degree to which a lens, mirror, or other optical system converges or diverges light.
/// It is measured in diopters (D), which is the reciprocal of the focal length in meters.
/// </summary>
/// <typeparam name="T">The numeric type for the optical power value.</typeparam>
public sealed record OpticalPower<T> : PhysicalQuantity<OpticalPower<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of optical power [L⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.OpticalPower;

	/// <summary>Initializes a new instance of the <see cref="OpticalPower{T}"/> class.</summary>
	public OpticalPower() : base() { }

	/// <summary>Creates a new optical power from a value in diopters.</summary>
	/// <param name="diopters">The optical power in diopters.</param>
	/// <returns>A new OpticalPower instance.</returns>
	public static OpticalPower<T> FromDiopters(T diopters) => Create(diopters);

	/// <summary>Creates a new optical power from a focal length.</summary>
	/// <param name="focalLength">The focal length.</param>
	/// <returns>A new OpticalPower instance.</returns>
	/// <remarks>
	/// Uses the relationship: P = 1 / f
	/// where P is optical power and f is focal length in meters.
	/// </remarks>
	public static OpticalPower<T> FromFocalLength(Length<T> focalLength)
	{
		Ensure.NotNull(focalLength);

		T focalLengthMeters = focalLength.In(Units.Meter);
		if (focalLengthMeters == T.Zero)
		{
			throw new ArgumentException("Focal length cannot be zero.", nameof(focalLength));
		}

		T diopters = T.One / focalLengthMeters;
		return Create(diopters);
	}

	/// <summary>Creates a new optical power from a focal length in millimeters.</summary>
	/// <param name="focalLengthMm">The focal length in millimeters.</param>
	/// <returns>A new OpticalPower instance.</returns>
	public static OpticalPower<T> FromFocalLengthMillimeters(T focalLengthMm)
	{
		if (focalLengthMm == T.Zero)
		{
			throw new ArgumentException("Focal length cannot be zero.", nameof(focalLengthMm));
		}

		T focalLengthMeters = focalLengthMm / T.CreateChecked(1000);
		T diopters = T.One / focalLengthMeters;
		return Create(diopters);
	}

	/// <summary>Calculates the focal length from optical power.</summary>
	/// <returns>The focal length.</returns>
	/// <remarks>
	/// Uses the relationship: f = 1 / P
	/// where f is focal length and P is optical power.
	/// </remarks>
	public Length<T> CalculateFocalLength()
	{
		T diopters = In(Units.Diopter);
		if (diopters == T.Zero)
		{
			throw new InvalidOperationException("Cannot calculate focal length for zero optical power.");
		}

		T focalLengthMeters = T.One / diopters;
		return Length<T>.Create(focalLengthMeters);
	}

	/// <summary>Calculates the combined optical power of two lenses in contact.</summary>
	/// <param name="other">The optical power of the second lens.</param>
	/// <returns>The combined optical power.</returns>
	/// <remarks>
	/// For thin lenses in contact: P_total = P₁ + P₂
	/// where P_total is the combined power, P₁ and P₂ are individual powers.
	/// </remarks>
	public OpticalPower<T> CombineWith(OpticalPower<T> other)
	{
		Ensure.NotNull(other);

		T power1 = In(Units.Diopter);
		T power2 = other.In(Units.Diopter);
		T combinedPower = power1 + power2;
		return Create(combinedPower);
	}

	/// <summary>Calculates the combined optical power of two lenses separated by a distance.</summary>
	/// <param name="other">The optical power of the second lens.</param>
	/// <param name="separation">The distance between the lenses.</param>
	/// <returns>The combined optical power.</returns>
	/// <remarks>
	/// For thin lenses separated by distance d: P_total = P₁ + P₂ - d×P₁×P₂
	/// where d is the separation distance in meters.
	/// </remarks>
	public OpticalPower<T> CombineWithSeparation(OpticalPower<T> other, Length<T> separation)
	{
		Ensure.NotNull(other);
		Ensure.NotNull(separation);

		T power1 = In(Units.Diopter);
		T power2 = other.In(Units.Diopter);
		T separationMeters = separation.In(Units.Meter);

		T combinedPower = power1 + power2 - (separationMeters * power1 * power2);
		return Create(combinedPower);
	}
}
