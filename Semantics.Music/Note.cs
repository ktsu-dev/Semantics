// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A sounding note: a pitch with a rhythmic duration and a velocity.
/// </summary>
public sealed record Note : IMusicalEvent
{
	/// <summary>Gets the pitch of the note.</summary>
	public Pitch Pitch { get; private init; } = Pitch.Create(60);

	/// <summary>Gets the rhythmic duration of the note.</summary>
	public Duration Duration { get; private init; } = Duration.Quarter;

	/// <summary>Gets the velocity (dynamic intensity) of the note.</summary>
	public Velocity Velocity { get; private init; } = Velocity.MezzoForte;

	/// <summary>Creates a note from a pitch, duration, and velocity.</summary>
	/// <param name="pitch">The pitch.</param>
	/// <param name="duration">The rhythmic duration.</param>
	/// <param name="velocity">The velocity.</param>
	/// <returns>A new note.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	public static Note Create(Pitch pitch, Duration duration, Velocity velocity)
	{
		Ensure.NotNull(pitch);
		Ensure.NotNull(duration);
		Ensure.NotNull(velocity);
		return new() { Pitch = pitch, Duration = duration, Velocity = velocity };
	}

	/// <summary>Returns the real-time length of this note in seconds at the given tempo.</summary>
	/// <param name="tempo">The tempo.</param>
	/// <returns>The note's length in seconds.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="tempo"/> is null.</exception>
	public double Seconds(Tempo tempo)
	{
		Ensure.NotNull(tempo);
		return tempo.Seconds(Duration);
	}

	/// <summary>Parses a note "{pitch}:{duration}:v{velocity}" (e.g. "C4:1/4:v80").</summary>
	/// <param name="text">The note text.</param>
	/// <returns>The parsed note.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Note Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Note? result)
			? result
			: throw new FormatException($"Invalid note '{text}'.");
	}

	/// <summary>Tries to parse a note "{pitch}:{duration}:v{velocity}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed note, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Note? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		string[] parts = text.Split(':');
		if (parts.Length != 3 || parts[2].Length < 2 || parts[2][0] != 'v')
		{
			return false;
		}

		if (!Pitch.TryParse(parts[0], out Pitch? pitch)
			|| !Duration.TryParse(parts[1], out Duration? duration)
			|| !Velocity.TryParse(parts[2][1..], out Velocity? velocity))
		{
			return false;
		}

		result = Create(pitch, duration, velocity);
		return true;
	}

	/// <summary>Returns "{pitch}:{duration}:v{velocity}" (e.g. "C4:1/4:v80").</summary>
	/// <returns>The canonical note text.</returns>
	public override string ToString() => $"{Pitch}:{Duration}:v{Velocity}";
}
