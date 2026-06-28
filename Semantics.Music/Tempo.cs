// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>
/// A tempo in beats per minute, where one beat is a chosen note duration (quarter note by default).
/// Converts rhythmic <see cref="Duration"/> values into real time.
/// </summary>
public sealed record Tempo
{
	/// <summary>Gets the tempo in beats per minute.</summary>
	public double BeatsPerMinute { get; private init; } = 120.0;

	/// <summary>Gets the note duration that counts as one beat (quarter note by default).</summary>
	public Duration Beat { get; private init; } = Duration.Quarter;

	/// <summary>Gets the real-time length of one beat in seconds.</summary>
	public double SecondsPerBeat => 60.0 / BeatsPerMinute;

	/// <summary>Creates a tempo with a quarter-note beat.</summary>
	/// <param name="beatsPerMinute">The tempo in beats per minute; must be positive.</param>
	/// <returns>A new tempo.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="beatsPerMinute"/> is not positive.</exception>
	public static Tempo Create(double beatsPerMinute) => Create(beatsPerMinute, Duration.Quarter);

	/// <summary>Creates a tempo with an explicit beat unit.</summary>
	/// <param name="beatsPerMinute">The tempo in beats per minute; must be positive.</param>
	/// <param name="beat">The note duration that counts as one beat.</param>
	/// <returns>A new tempo.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="beatsPerMinute"/> is not positive.</exception>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="beat"/> is null.</exception>
	public static Tempo Create(double beatsPerMinute, Duration beat)
	{
		if (beatsPerMinute <= 0.0 || double.IsNaN(beatsPerMinute))
		{
			throw new ArgumentOutOfRangeException(nameof(beatsPerMinute), beatsPerMinute, "Tempo must be positive.");
		}

		Ensure.NotNull(beat);
		return new() { BeatsPerMinute = beatsPerMinute, Beat = beat };
	}

	/// <summary>Returns the real-time length, in seconds, of a rhythmic duration at this tempo.</summary>
	/// <param name="duration">The rhythmic duration to convert.</param>
	/// <returns>The duration's length in seconds.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="duration"/> is null.</exception>
	public double Seconds(Duration duration)
	{
		Ensure.NotNull(duration);
		return duration.AsWholeNotes / Beat.AsWholeNotes * SecondsPerBeat;
	}
}
