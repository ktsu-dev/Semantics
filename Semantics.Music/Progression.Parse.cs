// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

public sealed partial record Progression
{
	/// <summary>Returns the chart-style progression line, e.g. "4/4  Dm7 / G7 / | Cmaj7 / / /".</summary>
	/// <returns>The canonical progression text.</returns>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append(TimeSignature).Append("  ");

		int beatUnit = TimeSignature.BeatUnit;
		int beatsPerBar = TimeSignature.Beats;
		int beatCursor = 0;
		bool first = true;

		foreach (ChordEvent ev in Chords)
		{
			if (!first && beatCursor % beatsPerBar == 0)
			{
				_ = sb.Append("| ");
			}

			first = false;

			double beatsExact = ev.Duration.AsWholeNotes * beatUnit;
			int beats = (int)System.Math.Round(beatsExact);
			bool wholeBeats = beats >= 1 && System.Math.Abs(beatsExact - beats) < 1e-9;

			if (wholeBeats)
			{
				_ = sb.Append(ev.Chord);
				for (int i = 1; i < beats; i++)
				{
					_ = sb.Append(" /");
				}

				beatCursor += beats;
			}
			else
			{
				_ = sb.Append(ev.Chord).Append('@').Append(ev.Duration);
				beatCursor = 0; // sub-beat durations reset bar tracking (cosmetic only)
			}

			_ = sb.Append(' ');
		}

		return sb.ToString().TrimEnd();
	}

	/// <summary>Parses a chart-style progression line.</summary>
	/// <param name="text">The progression text.</param>
	/// <returns>The parsed progression.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the text cannot be parsed.</exception>
	public static Progression Parse(string text)
	{
		Ensure.NotNull(text);
		return TryParse(text, out Progression? result)
			? result
			: throw new FormatException($"Invalid progression '{text}'.");
	}

	/// <summary>Tries to parse a chart-style progression line.</summary>
	/// <param name="text">The text to parse.</param>
	/// <param name="result">The parsed progression, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? text, [NotNullWhen(true)] out Progression? result)
	{
		result = null;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		string[] tokens = text!.Split((char[]?)null, System.StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length < 2 || !TimeSignature.TryParse(tokens[0], out TimeSignature? ts))
		{
			return false;
		}

		if (!TryReadChordEvents(tokens, ts.BeatUnit, out List<ChordEvent> events))
		{
			return false;
		}

		result = Create(events, ts);
		return true;
	}

	// Reads the chord/slash/bar tokens (everything after the leading time signature) into timed events.
	private static bool TryReadChordEvents(string[] tokens, int beatUnit, out List<ChordEvent> events)
	{
		events = [];
		Chord? current = null;
		int currentBeats = 0;
		Duration? explicitDuration = null;

		for (int i = 1; i < tokens.Length; i++)
		{
			string token = tokens[i];
			if (token == "|")
			{
				continue;
			}

			if (token == "/")
			{
				if (current is null)
				{
					return false;
				}

				currentBeats++;
				continue;
			}

			FlushChord(events, ref current, ref currentBeats, ref explicitDuration, beatUnit);
			if (!TryReadChordCell(token, out current, out explicitDuration, out currentBeats))
			{
				return false;
			}
		}

		FlushChord(events, ref current, ref currentBeats, ref explicitDuration, beatUnit);
		return events.Count > 0;
	}

	// Reads one chord token, which is either a bare chord (one beat) or "chord@n/d" (an explicit duration).
	private static bool TryReadChordCell(string token, out Chord? chord, out Duration? explicitDuration, out int beats)
	{
		chord = null;
		explicitDuration = null;
		beats = 0;

		int at = token.IndexOf('@');
		if (at >= 0)
		{
			return Chord.TryParse(token[..at], out chord) && Duration.TryParse(token[(at + 1)..], out explicitDuration);
		}

		if (!Chord.TryParse(token, out chord))
		{
			return false;
		}

		beats = 1;
		return true;
	}

	// Appends the pending chord (if any) as an event, then clears the running state.
	private static void FlushChord(List<ChordEvent> events, ref Chord? current, ref int currentBeats, ref Duration? explicitDuration, int beatUnit)
	{
		if (current is null)
		{
			return;
		}

		Duration duration = explicitDuration ?? Duration.Create(currentBeats, beatUnit);
		events.Add(ChordEvent.Create(current, duration));
		current = null;
		currentBeats = 0;
		explicitDuration = null;
	}
}
