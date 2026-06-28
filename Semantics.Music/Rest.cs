// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

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
}
