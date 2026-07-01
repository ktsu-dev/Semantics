// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System.Collections.Generic;

public sealed partial record Progression
{
	/// <summary>Detects cadences from the scale-degree motion of each adjacent chord pair in a key.</summary>
	/// <param name="key">The key to analyze against.</param>
	/// <returns>The cadences found, in order; empty when none are present.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public IReadOnlyList<CadenceInstance> Cadences(Key key)
	{
		Ensure.NotNull(key);
		List<CadenceInstance> result = [];
		for (int i = 0; i + 1 < Chords.Count; i++)
		{
			ScaleDegree from = key.FunctionOf(Chords[i].Chord.Root);
			ScaleDegree to = key.FunctionOf(Chords[i + 1].Chord.Root);
			if (from.Alteration != 0 || to.Alteration != 0)
			{
				continue;
			}

			Cadence? cadence = Classify(from.Degree, to.Degree);
			if (cadence is Cadence value)
			{
				result.Add(CadenceInstance.Create(i + 1, value));
			}
		}

		return result;
	}

	private static Cadence? Classify(int from, int to) => (from, to) switch
	{
		(5, 1) => Cadence.Authentic,
		(5, 6) => Cadence.Deceptive,
		(4, 1) => Cadence.Plagal,
		(_, 5) => Cadence.Half,
		_ => null,
	};
}
