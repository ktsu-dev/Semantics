// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a pitch quantity with compile-time dimensional safety.
/// Pitch is the perceptual correlate of frequency, measured in Hz, mels, or barks.
/// </summary>
public sealed record Pitch<T> : PhysicalQuantity<Pitch<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of pitch [T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Pitch;

	/// <summary>
	/// Initializes a new instance of the Pitch class.
	/// </summary>
	public Pitch() : base() { }

	/// <summary>
	/// Creates a new Pitch from a frequency value in Hz.
	/// </summary>
	/// <param name="hertz">The frequency in Hz.</param>
	/// <returns>A new Pitch instance.</returns>
	public static Pitch<T> FromHertz(T hertz) => Create(hertz);

	/// <summary>
	/// Creates a new Pitch from a value in mels (perceptual pitch scale).
	/// Mel scale: f_mel = 1127 * ln(1 + f_hz / 700)
	/// </summary>
	/// <param name="mels">The pitch in mels.</param>
	/// <returns>A new Pitch instance.</returns>
	public static Pitch<T> FromMels(T mels)
	{
		double melValue = double.CreateChecked(mels);
		double hertz = 700.0 * (Math.Exp(melValue / 1127.0) - 1.0);
		return FromHertz(T.CreateChecked(hertz));
	}

	/// <summary>
	/// Creates a new Pitch from a value in barks (critical band scale).
	/// Bark scale: f_bark = 7 * ln(f_hz / 650 + sqrt((f_hz / 650)² + 1))
	/// </summary>
	/// <param name="barks">The pitch in barks.</param>
	/// <returns>A new Pitch instance.</returns>
	public static Pitch<T> FromBarks(T barks)
	{
		// Inverse bark formula: f_hz = 650 * sinh(f_bark / 7)
		double barkValue = double.CreateChecked(barks);
		double hertz = 650.0 * Math.Sinh(barkValue / 7.0);
		return FromHertz(T.CreateChecked(hertz));
	}

	/// <summary>
	/// Converts frequency to mels (perceptual pitch scale).
	/// </summary>
	/// <returns>The pitch in mels.</returns>
	public T ToMels()
	{
		double hertz = double.CreateChecked(Value);
		double mels = 1127.0 * Math.Log(1.0 + (hertz / 700.0));
		return T.CreateChecked(mels);
	}

	/// <summary>
	/// Converts frequency to barks (critical band scale).
	/// </summary>
	/// <returns>The pitch in barks.</returns>
	public T ToBarks()
	{
		double hertz = double.CreateChecked(Value);
		double ratio = hertz / 650.0;
		double barks = 7.0 * Math.Log(ratio + Math.Sqrt((ratio * ratio) + 1.0));
		return T.CreateChecked(barks);
	}

	/// <summary>
	/// Calculates the musical interval between two pitches in cents.
	/// </summary>
	/// <param name="other">The other pitch.</param>
	/// <returns>The interval in cents (1200 cents = 1 octave).</returns>
	public T IntervalInCents(Pitch<T> other)
	{
		ArgumentNullException.ThrowIfNull(other);

		double ratio = double.CreateChecked(other.Value) / double.CreateChecked(Value);
		double cents = 1200.0 * Math.Log2(ratio);
		return T.CreateChecked(cents);
	}

	/// <summary>
	/// Gets the musical note name closest to this pitch (assuming equal temperament).
	/// </summary>
	/// <returns>The note name and octave.</returns>
	public string GetNoteName()
	{
		double frequency = double.CreateChecked(Value);

		// A4 = 440 Hz as reference
		double a4 = 440.0;
		double semitonesFromA4 = 12.0 * Math.Log2(frequency / a4);
		int semitones = (int)Math.Round(semitonesFromA4);

		string[] noteNames = ["A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"];
		int noteIndex = ((semitones % 12) + 12) % 12;
		int octave = 4 + ((semitones + 9) / 12);

		return $"{noteNames[noteIndex]}{octave}";
	}

	/// <summary>
	/// Calculates the frequency of a musical note given its offset in semitones from this pitch.
	/// </summary>
	/// <param name="semitones">The number of semitones (positive = higher, negative = lower).</param>
	/// <returns>The resulting pitch.</returns>
	public Pitch<T> TransposeBySemitones(int semitones)
	{
		double ratio = Math.Pow(2.0, semitones / 12.0);
		T newFrequency = T.CreateChecked(double.CreateChecked(Value) * ratio);
		return FromHertz(newFrequency);
	}

	/// <summary>
	/// Gets the pitch category description.
	/// </summary>
	/// <returns>A string describing the pitch range.</returns>
	public string GetPitchCategory() => double.CreateChecked(Value) switch
	{
		< 20 => "Infrasonic",
		< 200 => "Very Low",
		< 500 => "Low",
		< 2000 => "Mid",
		< 5000 => "High",
		< 20000 => "Very High",
		_ => "Ultrasonic"
	};
}
