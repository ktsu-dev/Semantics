// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

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
}
