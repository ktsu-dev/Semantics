// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A rest: a span of silence with a rhythmic duration.
/// </summary>
public sealed record Rest : IMusicalEvent
{
	/// <summary>Gets the rhythmic duration of the rest.</summary>
	public Duration Duration { get; private init; } = Duration.Quarter;

	/// <summary>Creates a rest of the given duration.</summary>
	/// <param name="duration">The rhythmic duration.</param>
	/// <returns>A new rest.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="duration"/> is null.</exception>
	public static Rest Create(Duration duration)
	{
		Ensure.NotNull(duration);
		return new() { Duration = duration };
	}

	/// <summary>Returns the real-time length of this rest in seconds at the given tempo.</summary>
	/// <param name="tempo">The tempo.</param>
	/// <returns>The rest's length in seconds.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="tempo"/> is null.</exception>
	public double Seconds(Tempo tempo)
	{
		Ensure.NotNull(tempo);
		return tempo.Seconds(Duration);
	}

	/// <summary>Parses a rest "R:{duration}" (e.g. "R:1/4").</summary>
	/// <param name="text">The rest text.</param>
	/// <returns>The parsed rest.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Rest Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Rest? result)
			? result
			: throw new FormatException($"Invalid rest '{text}'.");
	}

	/// <summary>Tries to parse a rest "R:{duration}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed rest, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Rest? result)
	{
		result = null;
		if (text is null || !text.StartsWith("R:", StringComparison.Ordinal))
		{
			return false;
		}

		if (!Duration.TryParse(text[2..], out Duration? duration))
		{
			return false;
		}

		result = Create(duration);
		return true;
	}

	/// <summary>Returns "R:{duration}" (e.g. "R:1/4").</summary>
	/// <returns>The canonical rest text.</returns>
	public override string ToString() => $"R:{Duration}";
}
