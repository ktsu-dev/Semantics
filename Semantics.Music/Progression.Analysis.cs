// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;
using System.Linq;

public sealed partial record Progression
{
	/// <summary>Returns the roman-numeral function of each chord relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One roman numeral per chord, in order.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<string> RomanNumerals(Key key)
	{
		Ensure.NotNull(key);
		return [.. Chords.Select(chordEvent => key.RomanNumeralOf(chordEvent.Chord))];
	}

	/// <summary>Returns the harmonic function of each chord relative to a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>One function per chord, in order.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<HarmonicFunction> Functions(Key key)
	{
		Ensure.NotNull(key);
		return [.. Chords.Select(chordEvent => FunctionOf(key, chordEvent.Chord))];
	}

	/// <summary>Classifies a single chord's function in a key by its root scale degree.</summary>
	private static HarmonicFunction FunctionOf(Key key, Chord chord)
	{
		ScaleDegree degree = key.FunctionOf(chord.Root);
		if (degree.Alteration != 0)
		{
			return HarmonicFunction.Chromatic;
		}

		return degree.Degree switch
		{
			1 or 3 or 6 => HarmonicFunction.Tonic,
			2 or 4 => HarmonicFunction.Predominant,
			5 or 7 => HarmonicFunction.Dominant,
			_ => HarmonicFunction.Chromatic,
		};
	}
}
