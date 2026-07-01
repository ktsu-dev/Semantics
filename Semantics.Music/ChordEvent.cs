// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

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
}
