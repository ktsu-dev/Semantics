// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A scale: a mode rooted at a specific pitch class.
/// </summary>
public sealed record Scale
{
	/// <summary>Gets the root pitch class.</summary>
	public PitchClass Root { get; private init; } = PitchClass.Create(0);

	/// <summary>Gets the mode shape.</summary>
	public Mode Mode { get; private init; } = Mode.Major;

	/// <summary>Gets the pitch classes of the scale, ascending from the root.</summary>
	public IReadOnlyList<PitchClass> PitchClasses =>
		[.. Mode.Intervals.Select(offset => PitchClass.Create(Root.Value + offset))];

	/// <summary>Creates a scale from a root pitch class and a mode.</summary>
	/// <param name="root">The root pitch class.</param>
	/// <param name="mode">The mode shape.</param>
	/// <returns>A new scale.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static Scale Create(PitchClass root, Mode mode)
	{
		Ensure.NotNull(root);
		Ensure.NotNull(mode);
		return new() { Root = root, Mode = mode };
	}

	/// <summary>Returns the scale transposed by a number of semitones (the mode is unchanged).</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed scale.</returns>
	public Scale Transpose(int semitones) => Create(PitchClass.Create(Root.Value + semitones), Mode);

	/// <summary>Returns whether the given pitch class is a member of the scale.</summary>
	/// <param name="pitchClass">The pitch class to test.</param>
	/// <returns><see langword="true"/> if it is a scale member.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="pitchClass"/> is null.</exception>
	public bool Contains(PitchClass pitchClass)
	{
		Ensure.NotNull(pitchClass);
		int semitone = (((pitchClass.Value - Root.Value) % 12) + 12) % 12;
		return Mode.Intervals.Contains(semitone);
	}

	/// <summary>Resolves a pitch class to a diatonic degree plus chromatic alteration.</summary>
	/// <param name="pitchClass">The pitch class to resolve.</param>
	/// <returns>
	/// The 1-based degree with alteration 0 for exact members; otherwise the nearest diatonic
	/// degree with a chromatic alteration, preferring the flat (upper degree lowered) on a tie
	/// so chromatic roots spell conventionally (e.g. ♭II, ♭III, ♭VI, ♭VII).
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="pitchClass"/> is null.</exception>
	public ScaleDegree DegreeOf(PitchClass pitchClass)
	{
		Ensure.NotNull(pitchClass);
		int semitone = (((pitchClass.Value - Root.Value) % 12) + 12) % 12;
		IReadOnlyList<int> offsets = Mode.Intervals;

		for (int i = 0; i < offsets.Count; i++)
		{
			if (offsets[i] == semitone)
			{
				return new ScaleDegree(i + 1, 0);
			}
		}

		// Chromatic: spell against the lower neighbour (raised/sharp) or the upper
		// neighbour (lowered/flat), whichever is closer; tie prefers the flat.
		int lowerIndex = -1;
		for (int i = 0; i < offsets.Count; i++)
		{
			if (offsets[i] < semitone)
			{
				lowerIndex = i;
			}
		}

		int upperIndex = -1;
		for (int i = offsets.Count - 1; i >= 0; i--)
		{
			if (offsets[i] > semitone)
			{
				upperIndex = i;
			}
		}

		// Wrap around the octave when there is no neighbour on one side.
		int sharpDegree = lowerIndex >= 0 ? lowerIndex + 1 : offsets.Count;
		int sharpAlteration = semitone - (lowerIndex >= 0 ? offsets[lowerIndex] : offsets[^1] - 12);

		int flatDegree = upperIndex >= 0 ? upperIndex + 1 : 1;
		int flatAlteration = semitone - (upperIndex >= 0 ? offsets[upperIndex] : offsets[0] + 12);

		return Math.Abs(flatAlteration) <= Math.Abs(sharpAlteration)
			? new ScaleDegree(flatDegree, flatAlteration)
			: new ScaleDegree(sharpDegree, sharpAlteration);
	}
}
