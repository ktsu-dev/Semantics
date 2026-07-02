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

	/// <summary>Parses a key "{tonic} {mode}" (e.g. "C major", "A aeolian").</summary>
	/// <param name="text">The key text.</param>
	/// <returns>The parsed key.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Key Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Key? result)
			? result
			: throw new FormatException($"Invalid key '{text}'.");
	}

	/// <summary>Tries to parse a key "{tonic} {mode}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed key, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Key? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int space = text.IndexOf(' ');
		if (space <= 0 || space == text.Length - 1)
		{
			return false;
		}

		if (!PitchClass.TryParse(text[..space], out PitchClass? tonic)
			|| !Mode.TryParse(text[(space + 1)..], out Mode? mode))
		{
			return false;
		}

		result = Create(tonic, mode);
		return true;
	}

	/// <summary>Returns "{tonic} {mode}" (e.g. "C major").</summary>
	/// <returns>The canonical key text.</returns>
	public override string ToString() => $"{Tonic} {Mode}";

	/// <summary>Returns the key transposed by a number of semitones (the mode is unchanged).</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed key.</returns>
	public Key Transpose(int semitones) => Create(PitchClass.Create(Tonic.Value + semitones), Mode);

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

	/// <summary>Parses a roman-numeral function relative to this key back into a concrete chord.</summary>
	/// <param name="numeral">
	/// A roman numeral such as "Imaj7", "ii7", "V7", "bII", or "vii°7": an optional accidental prefix
	/// (b/♭ or #/♯), the degree numeral (upper-case major, lower-case minor), then a quality suffix.
	/// </param>
	/// <returns>The chord rooted at the resolved scale degree.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="numeral"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the numeral cannot be parsed.</exception>
	public Chord ChordFromRomanNumeral(string numeral)
	{
		Ensure.NotNull(numeral);
		string text = numeral.Trim();
		if (text.Length == 0)
		{
			throw new FormatException("Roman numeral is empty.");
		}

		int index = 0;
		int alteration = Notation.ReadAccidentalOffset(text, ref index);

		(int degree, int length, bool upper) = ParseNumeral(text, index);
		if (length == 0)
		{
			throw new FormatException($"Invalid roman numeral in '{numeral}'.");
		}

		index += length;

		System.Collections.Generic.IReadOnlyList<PitchClass> pitchClasses = Scale.PitchClasses;
		if (degree > pitchClasses.Count)
		{
			throw new FormatException($"Degree {degree} is out of range for this key.");
		}

		PitchClass root = PitchClass.Create(pitchClasses[degree - 1].Value + alteration);

		// Normalise the suffix into a chord-symbol body, mapping °/+ onto dim/aug and
		// supplying a leading "m" for lower-case (minor) numerals.
		string suffix = text[index..];
		string quality;
		if (suffix.StartsWith('°'))
		{
			quality = "dim";
			suffix = suffix[1..];
		}
		else if (suffix.StartsWith("dim", StringComparison.Ordinal))
		{
			quality = "dim";
			suffix = suffix[3..];
		}
		else if (suffix.StartsWith('+'))
		{
			quality = "aug";
			suffix = suffix[1..];
		}
		else if (suffix.StartsWith("aug", StringComparison.Ordinal))
		{
			quality = "aug";
			suffix = suffix[3..];
		}
		else
		{
			quality = upper ? string.Empty : "m";
		}

		return Chord.Parse(root.Name + quality + suffix);
	}

	private static (int Degree, int Length, bool Upper) ParseNumeral(string text, int start)
	{
		string[] numerals = ["vii", "vi", "iv", "iii", "ii", "i", "v"];
		int[] degrees = [7, 6, 4, 3, 2, 1, 5];
		string lower = text.ToLowerInvariant();

		for (int i = 0; i < numerals.Length; i++)
		{
			string token = numerals[i];
			if (start + token.Length <= lower.Length && lower.Substring(start, token.Length) == token)
			{
				return (degrees[i], token.Length, char.IsUpper(text[start]));
			}
		}

		return (0, 0, false);
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
