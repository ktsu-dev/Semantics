// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

public sealed partial record Progression
{
	/// <summary>Returns the chart-style progression line, e.g. "4/4  Dm7 / G7 / | Cmaj7 / / /".</summary>
	/// <returns>The canonical progression text.</returns>
	/// <remarks>
	/// A chord lasting a whole number of beats is written as the chord and one <c>/</c> per further beat; any
	/// other length is written <c>chord@n/d</c>. A <c>|</c> is written wherever a bar ends between two tokens,
	/// including between the slashes of a chord held across the bar line (<c>G | /</c>). A bar that ends inside a
	/// <c>chord@n/d</c> cell, or inside a beat because an earlier cell left the chart off the beat, has no token
	/// boundary to carry it and gets no bar line.
	/// </remarks>
	public override string ToString()
	{
		StringBuilder sb = new();
		_ = sb.Append(TimeSignature).Append("  ");

		Duration bar = TimeSignature.BarDuration;
		Duration beat = TimeSignature.BeatDuration;
		Duration position = Duration.Create(0, 1);
		bool first = true;

		foreach (ChordEvent ev in Chords)
		{
			if (!first && IsMultipleOf(position, bar))
			{
				_ = sb.Append("| ");
			}

			first = false;
			_ = sb.Append(ev.Chord);

			if (TryCountWholeBeats(ev.Duration, TimeSignature.BeatUnit, out int beats))
			{
				for (int i = 1; i < beats; i++)
				{
					position = position.Add(beat);
					if (IsMultipleOf(position, bar))
					{
						_ = sb.Append(" |");
					}

					_ = sb.Append(" /");
				}

				position = position.Add(beat);
			}
			else
			{
				_ = sb.Append('@').Append(ev.Duration);
				position = position.Add(ev.Duration);
			}

			_ = sb.Append(' ');
		}

		return sb.ToString().TrimEnd();
	}

	// Whether a duration is a whole number of beats, at least one, and so can be written as a chord and slashes.
	private static bool TryCountWholeBeats(Duration duration, int beatUnit, out int beats)
	{
		long scaled = (long)duration.Numerator * beatUnit;
		beats = 0;
		if (scaled <= 0 || scaled % duration.Denominator != 0 || scaled / duration.Denominator > int.MaxValue)
		{
			return false;
		}

		beats = (int)(scaled / duration.Denominator);
		return true;
	}

	// Whether a position is a whole number of bars from the start: (n/d) / (b/u) is an integer.
	private static bool IsMultipleOf(Duration position, Duration bar) =>
		(long)position.Numerator * bar.Denominator % ((long)position.Denominator * bar.Numerator) == 0;

	/// <summary>Parses a chart-style progression line.</summary>
	/// <remarks>
	/// Each bare chord lasts one beat and each <c>/</c> extends it by one more. A <c>chord@n/d</c> cell has its length
	/// stated outright, so a <c>/</c> after one is rejected rather than silently dropped. A <c>|</c> is read as a
	/// visual aid and ignored: the shorthand <c>"4/4  C | G"</c> is two one-beat chords, and lengths come from the
	/// chord and slash tokens alone.
	/// </remarks>
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
		if (text is null)
		{
			return false;
		}

		// Splitting on whitespace with RemoveEmptyEntries already yields no tokens for an empty or
		// whitespace-only input, so the token-count check below subsumes an IsNullOrWhiteSpace guard.
		string[] tokens = text.Split((char[]?)null, System.StringSplitOptions.RemoveEmptyEntries);
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
				// A chord@n/d cell states its whole length, so a slash after it has nothing to extend.
				if (current is null || explicitDuration is not null)
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
			return Chord.TryParse(token[..at], out chord)
				&& Duration.TryParse(token[(at + 1)..], out explicitDuration)
				&& explicitDuration.IsPositive;
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
