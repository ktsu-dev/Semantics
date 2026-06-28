// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Text;

/// <summary>
/// A musical key: a tonic pitch class in a mode. Resolves chords to roman-numeral functions.
/// </summary>
public sealed record Key
{
	private static readonly string[] RomanNumerals =
		["I", "II", "III", "IV", "V", "VI", "VII"];

	/// <summary>Gets the tonic pitch class.</summary>
	public PitchClass Tonic { get; private init; } = PitchClass.Create(0);

	/// <summary>Gets the mode.</summary>
	public Mode Mode { get; private init; } = Mode.Major;

	/// <summary>Gets the scale of this key.</summary>
	public Scale Scale => Scale.Create(Tonic, Mode);

	/// <summary>Creates a key from a tonic and mode.</summary>
	/// <param name="tonic">The tonic pitch class.</param>
	/// <param name="mode">The mode.</param>
	/// <returns>A new key.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static Key Create(PitchClass tonic, Mode mode)
	{
		Ensure.NotNull(tonic);
		Ensure.NotNull(mode);
		return new() { Tonic = tonic, Mode = mode };
	}

	/// <summary>Resolves a pitch class to its scale-degree function in this key.</summary>
	/// <param name="root">The pitch class to resolve.</param>
	/// <returns>The scale degree with any chromatic alteration.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> is null.</exception>
	public ScaleDegree FunctionOf(PitchClass root)
	{
		Ensure.NotNull(root);
		return Scale.DegreeOf(root);
	}

	/// <summary>Returns the roman-numeral function of a chord relative to this key.</summary>
	/// <param name="chord">The chord to label.</param>
	/// <returns>
	/// A roman numeral: an accidental prefix (b/#) for chromatic roots, the degree numeral
	/// cased upper for major/augmented and lower for minor/diminished, then a quality suffix.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="chord"/> is null.</exception>
	public string RomanNumeralOf(Chord chord)
	{
		Ensure.NotNull(chord);

		ScaleDegree degree = Scale.DegreeOf(chord.Root);
		StringBuilder sb = new();

		if (degree.Alteration < 0)
		{
			_ = sb.Append('b', -degree.Alteration);
		}
		else if (degree.Alteration > 0)
		{
			_ = sb.Append('#', degree.Alteration);
		}

		string numeral = RomanNumerals[(degree.Degree - 1) % RomanNumerals.Length];
		bool lowerCase = chord.Quality is ChordQuality.Minor or ChordQuality.Diminished;
		_ = sb.Append(lowerCase ? numeral.ToLowerInvariant() : numeral);

		_ = sb.Append(QualitySuffix(chord));
		return sb.ToString();
	}

	private static string QualitySuffix(Chord chord)
	{
		string seventh = chord.Seventh switch
		{
			SeventhType.Major => "maj7",
			SeventhType.Dominant => "7",
			SeventhType.Diminished => chord.Quality == ChordQuality.Diminished ? "" : "7",
			_ => "",
		};

		string quality = chord.Quality switch
		{
			ChordQuality.Diminished => "°",
			ChordQuality.Augmented => "+",
			ChordQuality.Sus2 => "sus2",
			ChordQuality.Sus4 => "sus4",
			ChordQuality.Power => "5",
			_ => "",
		};

		return quality + seventh;
	}
}
