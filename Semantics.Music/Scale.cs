// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

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
	/// The 1-based degree with alteration 0 for exact members; otherwise the nearest lower
	/// diatonic degree with a positive (sharp) alteration.
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

		// Not diatonic: attach to the nearest lower diatonic offset, raised.
		int bestDegree = 1;
		int bestAlteration = semitone - offsets[0];
		for (int i = 0; i < offsets.Count; i++)
		{
			if (offsets[i] < semitone && (semitone - offsets[i]) <= bestAlteration)
			{
				bestDegree = i + 1;
				bestAlteration = semitone - offsets[i];
			}
		}

		return new ScaleDegree(bestDegree, bestAlteration);
	}
}
