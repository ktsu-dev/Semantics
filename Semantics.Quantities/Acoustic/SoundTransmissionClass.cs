// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound transmission class quantity with compile-time dimensional safety.
/// STC is a rating of how well a building partition blocks airborne sound.
/// </summary>
public sealed record SoundTransmissionClass<T> : PhysicalQuantity<SoundTransmissionClass<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sound transmission class [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundTransmissionClass;

	/// <summary>
	/// Initializes a new instance of the SoundTransmissionClass class.
	/// </summary>
	public SoundTransmissionClass() : base() { }

	/// <summary>
	/// Creates a new SoundTransmissionClass from a rating value.
	/// </summary>
	/// <param name="rating">The STC rating (typically 15-65).</param>
	/// <returns>A new SoundTransmissionClass instance.</returns>
	public static SoundTransmissionClass<T> FromRating(T rating) => Create(rating);

	/// <summary>
	/// Gets the acoustic performance description based on STC rating.
	/// </summary>
	/// <returns>A string describing the acoustic performance.</returns>
	public string GetPerformanceDescription() => double.CreateChecked(Value) switch
	{
		< 25 => "Poor - Speech clearly audible",
		< 30 => "Fair - Normal speech audible",
		< 35 => "Good - Loud speech audible",
		< 40 => "Very Good - Shouting barely audible",
		< 45 => "Excellent - Very loud sounds faintly audible",
		< 50 => "Superior - Loud sounds barely audible",
		< 55 => "Outstanding - Very loud sounds faintly audible",
		_ => "Exceptional - Minimal sound transmission"
	};

	/// <summary>
	/// Gets the typical application based on STC rating.
	/// </summary>
	/// <returns>A string describing typical applications.</returns>
	public string GetTypicalApplication() => double.CreateChecked(Value) switch
	{
		< 30 => "Interior partitions in open offices",
		< 35 => "Standard interior walls between rooms",
		< 40 => "Walls between bedrooms and living areas",
		< 45 => "Walls between apartments or hotel rooms",
		< 50 => "Walls between noisy and quiet areas",
		< 55 => "Recording studios, conference rooms",
		_ => "Specialized acoustic environments"
	};

	/// <summary>
	/// Estimates the transmission loss at a specific frequency.
	/// This is a simplified approximation based on the STC contour.
	/// </summary>
	/// <param name="frequency">The frequency in Hz.</param>
	/// <returns>The estimated transmission loss in dB.</returns>
	public T EstimateTransmissionLoss(Frequency<T> frequency)
	{
		ArgumentNullException.ThrowIfNull(frequency);

		// Simplified STC contour approximation
		// Real STC calculation requires detailed frequency analysis
		double freq = double.CreateChecked(frequency.Value);
		double stc = double.CreateChecked(Value);

		double transmissionLoss = freq switch
		{
			< 200 => stc - 15,  // Low frequency penalty
			< 500 => stc - 5,   // Mid-low frequency
			< 1000 => stc,      // Reference frequency range
			< 2000 => stc + 3,  // Mid-high frequency
			_ => stc + 5        // High frequency bonus
		};

		return T.CreateChecked(Math.Max(0, transmissionLoss));
	}

	/// <summary>
	/// Converts STC to approximate noise reduction in dB.
	/// </summary>
	/// <returns>The approximate noise reduction.</returns>
	public T ToNoiseReduction() => Value;

	/// <summary>
	/// Determines if the STC rating meets building code requirements.
	/// </summary>
	/// <param name="buildingType">Type of building ("residential", "commercial", "industrial").</param>
	/// <returns>True if the rating meets typical requirements.</returns>
	public bool MeetsBuildingCode(string buildingType) => buildingType?.ToLower() switch
	{
		"residential" => double.CreateChecked(Value) >= 50,
		"commercial" => double.CreateChecked(Value) >= 45,
		"industrial" => double.CreateChecked(Value) >= 40,
		_ => false
	};
}
