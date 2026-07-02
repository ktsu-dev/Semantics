// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A harmonic event: a chord sounding for a rhythmic duration (the harmonic rhythm).
/// </summary>
public sealed record ChordEvent : IMusicalEvent
{
	/// <summary>Gets the chord that sounds.</summary>
	public Chord Chord { get; private init; } = new();

	/// <summary>Gets the rhythmic duration for which the chord sounds.</summary>
	public Duration Duration { get; private init; } = Duration.Quarter;

	/// <summary>Creates a chord event from a chord and a duration.</summary>
	/// <param name="chord">The chord.</param>
	/// <param name="duration">The rhythmic duration.</param>
	/// <returns>A new chord event.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static ChordEvent Create(Chord chord, Duration duration)
	{
		Ensure.NotNull(chord);
		Ensure.NotNull(duration);
		return new() { Chord = chord, Duration = duration };
	}

	/// <summary>Parses a chord event "{chord}:{duration}" (e.g. "Cmaj7:1/4").</summary>
	/// <param name="text">The chord-event text.</param>
	/// <returns>The parsed chord event.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static ChordEvent Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out ChordEvent? result)
			? result
			: throw new FormatException($"Invalid chord event '{text}'.");
	}

	/// <summary>Tries to parse a chord event "{chord}:{duration}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed chord event, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out ChordEvent? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int colon = text.IndexOf(':');
		if (colon <= 0 || colon == text.Length - 1)
		{
			return false;
		}

		if (!Chord.TryParse(text[..colon], out Chord? chord)
			|| !Duration.TryParse(text[(colon + 1)..], out Duration? duration))
		{
			return false;
		}

		result = Create(chord, duration);
		return true;
	}

	/// <summary>Returns "{chord}:{duration}" (e.g. "Cmaj7:1/4").</summary>
	/// <returns>The canonical chord-event text.</returns>
	public override string ToString() => $"{Chord}:{Duration}";
}
