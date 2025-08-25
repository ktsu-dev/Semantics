// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a directionality index quantity with compile-time dimensional safety.
/// Directionality index (DI) measures how directional a sound source is, in dB.
/// </summary>
public sealed record DirectionalityIndex<T> : PhysicalQuantity<DirectionalityIndex<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of directionality index [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.DirectionalityIndex;

	/// <summary>
	/// Initializes a new instance of the DirectionalityIndex class.
	/// </summary>
	public DirectionalityIndex() : base() { }

	/// <summary>
	/// Creates a new DirectionalityIndex from a value in decibels.
	/// </summary>
	/// <param name="decibels">The directionality index in dB.</param>
	/// <returns>A new DirectionalityIndex instance.</returns>
	public static DirectionalityIndex<T> FromDecibels(T decibels) => Create(decibels);

	/// <summary>
	/// Creates a DirectionalityIndex from directivity factor Q.
	/// DI = 10 * log₁₀(Q)
	/// </summary>
	/// <param name="directivityFactor">The directivity factor Q.</param>
	/// <returns>The corresponding directionality index.</returns>
	public static DirectionalityIndex<T> FromDirectivityFactor(T directivityFactor)
	{
		T decibels = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(directivityFactor)));
		return FromDecibels(decibels);
	}

	/// <summary>
	/// Converts to directivity factor Q.
	/// Q = 10^(DI/10)
	/// </summary>
	/// <returns>The directivity factor.</returns>
	public T ToDirectivityFactor()
	{
		double factor = Math.Pow(10.0, double.CreateChecked(Value) / 10.0);
		return T.CreateChecked(factor);
	}

	/// <summary>
	/// Gets the directivity pattern description.
	/// </summary>
	/// <returns>A string describing the directivity pattern.</returns>
	public string GetDirectivityPattern() => double.CreateChecked(Value) switch
	{
		< 1.0 => "Omnidirectional (no directivity)",
		< 3.0 => "Slightly directional",
		< 6.0 => "Moderately directional",
		< 9.0 => "Highly directional",
		< 12.0 => "Very directional",
		_ => "Extremely directional (beam-like)"
	};

	/// <summary>
	/// Estimates the beamwidth (approximate half-power angle) from DI.
	/// This is a rough approximation based on circular patterns.
	/// </summary>
	/// <returns>Estimated beamwidth in degrees.</returns>
	public T EstimateBeamwidth()
	{
		// Rough approximation: θ ≈ 58.3° / sqrt(Q)
		T directivityFactor = ToDirectivityFactor();
		double beamwidth = 58.3 / Math.Sqrt(double.CreateChecked(directivityFactor));
		return T.CreateChecked(beamwidth);
	}

	/// <summary>
	/// Calculates the front-to-back ratio for typical loudspeaker patterns.
	/// </summary>
	/// <returns>Front-to-back ratio in dB (estimate).</returns>
	public T EstimateFrontToBackRatio()
	{
		// Rough correlation between DI and front-to-back ratio
		double ratio = double.CreateChecked(Value) * 1.5; // Empirical approximation
		return T.CreateChecked(Math.Min(ratio, 30.0)); // Cap at 30 dB
	}

	/// <summary>
	/// Gets the typical application based on directionality index.
	/// </summary>
	/// <returns>A string describing typical applications.</returns>
	public string GetTypicalApplication() => double.CreateChecked(Value) switch
	{
		< 2.0 => "Ambient sound sources, subwoofers",
		< 4.0 => "Monitor speakers, near-field applications",
		< 6.0 => "Home audio, bookshelf speakers",
		< 8.0 => "Studio monitors, PA speakers",
		< 10.0 => "Horn-loaded speakers, line arrays",
		_ => "Highly directional arrays, sound reinforcement"
	};

	/// <summary>
	/// Calculates coverage angle for symmetrical patterns.
	/// </summary>
	/// <param name="level">Coverage level in dB below peak (typically -3, -6, or -10 dB).</param>
	/// <returns>Coverage angle in degrees.</returns>
	public T CoverageAngle(T level)
	{
		// Simplified calculation based on circular symmetry
		// Real calculations require detailed polar patterns
		T adjustedDi = Value + level; // Adjust for coverage level
		T adjustedQ = T.CreateChecked(Math.Pow(10.0, double.CreateChecked(adjustedDi) / 10.0));
		double angle = 2.0 * Math.Acos(1.0 / Math.Sqrt(double.CreateChecked(adjustedQ))) * 180.0 / Math.PI;
		return T.CreateChecked(angle);
	}

	/// <summary>
	/// Estimates sound pressure level gain compared to omnidirectional source.
	/// </summary>
	/// <returns>SPL gain in dB on-axis.</returns>
	public T OnAxisGain() => Value; // DI directly represents on-axis gain vs omnidirectional
}
