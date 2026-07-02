// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a scale shape (mode) by its ascending semitone offsets from the root.
/// </summary>
public sealed record Mode
{
	private static readonly Dictionary<string, int[]> Shapes = new(StringComparer.OrdinalIgnoreCase)
	{
		// Diatonic modes.
		["major"] = [0, 2, 4, 5, 7, 9, 11],
		["ionian"] = [0, 2, 4, 5, 7, 9, 11],
		["dorian"] = [0, 2, 3, 5, 7, 9, 10],
		["phrygian"] = [0, 1, 3, 5, 7, 8, 10],
		["lydian"] = [0, 2, 4, 6, 7, 9, 11],
		["mixolydian"] = [0, 2, 4, 5, 7, 9, 10],
		["aeolian"] = [0, 2, 3, 5, 7, 8, 10],
		["locrian"] = [0, 1, 3, 5, 6, 8, 10],

		// Minor scales.
		["harmonic_minor"] = [0, 2, 3, 5, 7, 8, 11],
		["melodic_minor"] = [0, 2, 3, 5, 7, 9, 11],

		// Modes of melodic minor.
		["dorian_b2"] = [0, 1, 3, 5, 7, 9, 10],
		["lydian_augmented"] = [0, 2, 4, 6, 8, 9, 11],
		["lydian_dominant"] = [0, 2, 4, 6, 7, 9, 10],
		["mixolydian_b6"] = [0, 2, 4, 5, 7, 8, 10],
		["locrian_natural2"] = [0, 2, 3, 5, 6, 8, 10],
		["altered"] = [0, 1, 3, 4, 6, 8, 10],

		// Modes of harmonic minor.
		["locrian_natural6"] = [0, 1, 3, 5, 6, 9, 10],
		["ionian_augmented"] = [0, 2, 4, 5, 8, 9, 11],
		["dorian_sharp4"] = [0, 2, 3, 6, 7, 9, 10],
		["phrygian_dominant"] = [0, 1, 4, 5, 7, 8, 10],
		["lydian_sharp2"] = [0, 3, 4, 6, 7, 9, 11],
		["ultralocrian"] = [0, 1, 3, 4, 6, 8, 9],

		// Symmetric scales.
		["whole_tone"] = [0, 2, 4, 6, 8, 10],
		["octatonic_hw"] = [0, 1, 3, 4, 6, 7, 9, 10],
		["octatonic_wh"] = [0, 2, 3, 5, 6, 8, 9, 11],
		["chromatic"] = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],

		// Pentatonic and blues scales.
		["major_pentatonic"] = [0, 2, 4, 7, 9],
		["minor_pentatonic"] = [0, 3, 5, 7, 10],
		["blues_minor"] = [0, 3, 5, 6, 7, 10],
		["blues_major"] = [0, 2, 3, 4, 7, 9],
	};

	/// <summary>Gets the canonical lower-case name of the mode.</summary>
	public string Name { get; private init; } = "major";

	/// <summary>Gets the ascending semitone offsets from the root.</summary>
	public IReadOnlyList<int> Intervals => Shapes[Name];

	/// <summary>Gets the number of scale degrees.</summary>
	public int DegreeCount => Shapes[Name].Length;

	/// <summary>The major (Ionian) mode.</summary>
	public static Mode Major => new() { Name = "major" };

	/// <summary>The Ionian mode (alias of <see cref="Major"/>).</summary>
	public static Mode Ionian => Major;

	/// <summary>The Dorian mode.</summary>
	public static Mode Dorian => new() { Name = "dorian" };

	/// <summary>The Phrygian mode.</summary>
	public static Mode Phrygian => new() { Name = "phrygian" };

	/// <summary>The Lydian mode.</summary>
	public static Mode Lydian => new() { Name = "lydian" };

	/// <summary>The Mixolydian mode.</summary>
	public static Mode Mixolydian => new() { Name = "mixolydian" };

	/// <summary>The Aeolian (natural minor) mode.</summary>
	public static Mode Aeolian => new() { Name = "aeolian" };

	/// <summary>The Locrian mode.</summary>
	public static Mode Locrian => new() { Name = "locrian" };

	/// <summary>The harmonic minor scale.</summary>
	public static Mode HarmonicMinor => new() { Name = "harmonic_minor" };

	/// <summary>The melodic minor (jazz minor) scale: natural sixth and seventh.</summary>
	public static Mode MelodicMinor => new() { Name = "melodic_minor" };

	/// <summary>Dorian ♭2 (Phrygian ♮6), the second mode of melodic minor.</summary>
	public static Mode DorianFlat2 => new() { Name = "dorian_b2" };

	/// <summary>Lydian augmented, the third mode of melodic minor.</summary>
	public static Mode LydianAugmented => new() { Name = "lydian_augmented" };

	/// <summary>Lydian dominant (acoustic/overtone), the fourth mode of melodic minor.</summary>
	public static Mode LydianDominant => new() { Name = "lydian_dominant" };

	/// <summary>Mixolydian ♭6 (melodic major), the fifth mode of melodic minor.</summary>
	public static Mode MixolydianFlat6 => new() { Name = "mixolydian_b6" };

	/// <summary>Locrian ♮2 (half-diminished), the sixth mode of melodic minor.</summary>
	public static Mode LocrianNatural2 => new() { Name = "locrian_natural2" };

	/// <summary>The altered (super-Locrian) scale, the seventh mode of melodic minor.</summary>
	public static Mode Altered => new() { Name = "altered" };

	/// <summary>Locrian ♮6, the second mode of harmonic minor.</summary>
	public static Mode LocrianNatural6 => new() { Name = "locrian_natural6" };

	/// <summary>Ionian ♯5 (augmented), the third mode of harmonic minor.</summary>
	public static Mode IonianAugmented => new() { Name = "ionian_augmented" };

	/// <summary>Dorian ♯4 (Ukrainian Dorian), the fourth mode of harmonic minor.</summary>
	public static Mode DorianSharp4 => new() { Name = "dorian_sharp4" };

	/// <summary>The Phrygian dominant scale (fifth mode of harmonic minor): the V7♭9 colour.</summary>
	public static Mode PhrygianDominant => new() { Name = "phrygian_dominant" };

	/// <summary>Lydian ♯2, the sixth mode of harmonic minor.</summary>
	public static Mode LydianSharp2 => new() { Name = "lydian_sharp2" };

	/// <summary>The ultralocrian (altered diminished) scale, the seventh mode of harmonic minor.</summary>
	public static Mode Ultralocrian => new() { Name = "ultralocrian" };

	/// <summary>The whole-tone scale.</summary>
	public static Mode WholeTone => new() { Name = "whole_tone" };

	/// <summary>The half-whole octatonic (diminished) scale, used over dominant/altered harmony.</summary>
	public static Mode OctatonicHalfWhole => new() { Name = "octatonic_hw" };

	/// <summary>The whole-half octatonic (diminished) scale, used over diminished harmony.</summary>
	public static Mode OctatonicWholeHalf => new() { Name = "octatonic_wh" };

	/// <summary>The chromatic scale (all twelve pitch classes).</summary>
	public static Mode Chromatic => new() { Name = "chromatic" };

	/// <summary>The major pentatonic scale.</summary>
	public static Mode MajorPentatonic => new() { Name = "major_pentatonic" };

	/// <summary>The minor pentatonic scale.</summary>
	public static Mode MinorPentatonic => new() { Name = "minor_pentatonic" };

	/// <summary>The minor blues scale (minor pentatonic plus the ♭5 blue note).</summary>
	public static Mode BluesMinor => new() { Name = "blues_minor" };

	/// <summary>The major blues scale (major pentatonic plus the ♭3 blue note).</summary>
	public static Mode BluesMajor => new() { Name = "blues_major" };

	/// <summary>Parses a mode by name, case-insensitively.</summary>
	/// <param name="name">The mode name (e.g. "aeolian", "harmonic_minor", "octatonic_hw").</param>
	/// <returns>The matching mode.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the name is not a known mode.</exception>
	public static Mode Parse(string name)
	{
		Ensure.NotNull(name);
		return TryParse(name, out Mode? result)
			? result
			: throw new FormatException($"Unknown mode '{name}'.");
	}

	/// <summary>Tries to parse a mode by name, case-insensitively.</summary>
	/// <param name="name">The mode name.</param>
	/// <param name="result">The matching mode, or null on failure.</param>
	/// <returns><see langword="true"/> when the name is a known mode.</returns>
	public static bool TryParse(string? name, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Mode? result)
	{
		if (name is not null && Shapes.ContainsKey(name))
		{
			result = new() { Name = name.ToLowerInvariant() };
			return true;
		}

		result = null;
		return false;
	}

	/// <summary>Returns the canonical lower-case mode name.</summary>
	/// <returns>The mode name.</returns>
	public override string ToString() => Name;
}
