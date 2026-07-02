// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Globalization;

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

	/// <summary>Parses a tempo "{bpm}bpm@{beat}" (e.g. "120bpm@1/4").</summary>
	/// <param name="text">The tempo text.</param>
	/// <returns>The parsed tempo.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Tempo Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Tempo? result)
			? result
			: throw new FormatException($"Invalid tempo '{text}'.");
	}

	/// <summary>Tries to parse a tempo "{bpm}bpm@{beat}".</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed tempo, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Tempo? result)
	{
		result = null;
		if (text is null)
		{
			return false;
		}

		int marker = text.IndexOf("bpm@", StringComparison.Ordinal);
		if (marker <= 0)
		{
			return false;
		}

		if (!double.TryParse(text[..marker], NumberStyles.Float, CultureInfo.InvariantCulture, out double bpm)
			|| bpm <= 0.0
			|| !Duration.TryParse(text[(marker + 4)..], out Duration? beat))
		{
			return false;
		}

		result = Create(bpm, beat);
		return true;
	}

	/// <summary>Returns "{bpm}bpm@{beat}" (e.g. "120bpm@1/4").</summary>
	/// <returns>The canonical tempo text.</returns>
	public override string ToString() =>
		$"{BeatsPerMinute.ToString("R", CultureInfo.InvariantCulture)}bpm@{Beat}";
}
