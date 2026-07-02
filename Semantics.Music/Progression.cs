// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An ordered chord sequence with a harmonic rhythm, plus functional-harmony analysis.
/// </summary>
public sealed partial record Progression
{
	/// <summary>Gets the ordered chord events (the harmonic content).</summary>
	public IReadOnlyList<ChordEvent> Chords { get; private init; } = [];

	/// <summary>Gets the time signature used to interpret durations as bars and beats.</summary>
	public TimeSignature TimeSignature { get; private init; } = TimeSignature.Create(4, 4);

	/// <summary>Determines structural equality: the time signature and ordered chord events all match.</summary>
	/// <param name="other">The progression to compare.</param>
	/// <returns><see langword="true"/> when the progressions are value-equal.</returns>
	public bool Equals(Progression? other) =>
		other is not null
		&& TimeSignature == other.TimeSignature
		&& Chords.SequenceEqual(other.Chords);

	/// <summary>Returns a hash code consistent with <see cref="Equals(Progression)"/>.</summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode() => (TimeSignature, Chords.Count).GetHashCode();

	/// <summary>Gets the total rhythmic length as a fraction of a whole note.</summary>
	public Duration TotalDuration =>
		Chords.Aggregate(Duration.Create(0, 1), (sum, chordEvent) => sum.Add(chordEvent.Duration));

	/// <summary>Gets the total length measured in bars of the time signature.</summary>
	public double TotalBars => TotalDuration.AsWholeNotes / TimeSignature.BarDuration.AsWholeNotes;

	/// <summary>Creates a progression in 4/4 from chord events.</summary>
	/// <param name="chords">The ordered chord events; must be non-empty.</param>
	/// <returns>A new progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="chords"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<ChordEvent> chords) => Create(chords, TimeSignature.Create(4, 4));

	/// <summary>Creates a progression from chord events with an explicit time signature.</summary>
	/// <param name="chords">The ordered chord events; must be non-empty.</param>
	/// <param name="timeSignature">The time signature.</param>
	/// <returns>A new progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<ChordEvent> chords, TimeSignature timeSignature)
	{
		Ensure.NotNull(chords);
		Ensure.NotNull(timeSignature);
		List<ChordEvent> list = [.. chords];
		if (list.Count == 0)
		{
			throw new ArgumentException("A progression must contain at least one chord.", nameof(chords));
		}

		return new() { Chords = list, TimeSignature = timeSignature };
	}

	/// <summary>Creates a progression from chords that all share one rhythmic duration.</summary>
	/// <param name="chords">The ordered chords; must be non-empty.</param>
	/// <param name="each">The duration applied to every chord.</param>
	/// <returns>A new progression in 4/4.</returns>
	/// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="chords"/> is empty.</exception>
	public static Progression Create(IEnumerable<Chord> chords, Duration each)
	{
		Ensure.NotNull(chords);
		Ensure.NotNull(each);
		return Create([.. chords.Select(chord => ChordEvent.Create(chord, each))]);
	}
}
