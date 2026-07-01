// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;

public sealed partial record Progression
{
	/// <summary>Parses a bar-delimited chord progression in 4/4.</summary>
	/// <param name="text">Chord symbols with <c>|</c> as a barline and spaces separating chords in a bar.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is empty or a bar is empty.</exception>
	public static Progression Parse(string text) => Parse(text, TimeSignature.Create(4, 4));

	/// <summary>Parses a bar-delimited chord progression with an explicit time signature.</summary>
	/// <param name="text">Chord symbols with <c>|</c> as a barline and spaces separating chords in a bar.</param>
	/// <param name="timeSignature">The time signature; a bar's chords split its bar duration evenly.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="FormatException">Thrown when the text is empty or a bar is empty.</exception>
	public static Progression Parse(string text, TimeSignature timeSignature)
	{
		Ensure.NotNull(text);
		Ensure.NotNull(timeSignature);

		string trimmed = text.Trim();
		if (trimmed.Length == 0)
		{
			throw new FormatException("Progression is empty.");
		}

		// Strip one leading and one trailing barline so "| C | G |" parses cleanly.
		if (trimmed[0] == '|')
		{
			trimmed = trimmed[1..];
		}

		if (trimmed.Length > 0 && trimmed[^1] == '|')
		{
			trimmed = trimmed[..^1];
		}

		Duration barDuration = timeSignature.BarDuration;
		List<ChordEvent> events = [];
		foreach (string bar in trimmed.Split('|'))
		{
			string[] tokens = bar.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0)
			{
				throw new FormatException("Progression has an empty bar.");
			}

			Duration each = Duration.Create(barDuration.Numerator, barDuration.Denominator * tokens.Length);
			foreach (string token in tokens)
			{
				events.Add(ChordEvent.Create(Chord.Parse(token), each));
			}
		}

		return Create(events, timeSignature);
	}
}
