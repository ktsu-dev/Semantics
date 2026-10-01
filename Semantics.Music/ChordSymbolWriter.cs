// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Text;

internal static class ChordSymbolWriter
{
	private const ChordTensions NaturalTensions = ChordTensions.Nine | ChordTensions.Eleven | ChordTensions.Thirteen;
	private const ChordTensions AlteredTensions = ChordTensions.FlatNine | ChordTensions.SharpNine
		| ChordTensions.SharpEleven | ChordTensions.FlatThirteen;
	private const ChordTensions AllTensions = NaturalTensions | AlteredTensions;
	private const ChordOmissions AllOmissions = ChordOmissions.Third | ChordOmissions.Fifth;

	internal static string Format(Chord chord)
	{
		Ensure.NotNull(chord);
		StringBuilder symbol = new(chord.Root.Name);
		AppendQuality(symbol, chord);
		ChordTensions impliedTensions = AppendExtension(symbol, chord);
		bool hasSeventh = chord.Seventh != SeventhType.None;
		ChordTensions remaining = chord.Tensions & ~impliedTensions;
		if (!hasSeventh && chord.Sixth == SixthType.Natural)
		{
			_ = symbol.Append(remaining.HasFlag(ChordTensions.Nine) ? "6/9" : "6");
			if (remaining.HasFlag(ChordTensions.Nine))
			{
				remaining &= ~ChordTensions.Nine;
			}
		}
		else if (!hasSeventh && chord.Sixth == SixthType.Flat)
		{
			if (symbol.Length == chord.Root.Name.Length)
			{
				_ = symbol.Append("(b6)");
			}
			else
			{
				_ = symbol.Append("b6");
			}
		}

		AppendSuspension(symbol, chord);

		StringBuilder alterations = new();
		AppendFlag(alterations, chord.Quality == ChordQuality.MajorFlatFive || (chord.Quality == ChordQuality.Diminished && chord.Seventh == SeventhType.Dominant), "b5");
		AppendFlag(alterations, chord.Quality == ChordQuality.MinorSharpFive, "#5");
		AppendFlag(alterations, chord.Tensions.HasFlag(ChordTensions.FlatNine), "b9");
		AppendFlag(alterations, chord.Tensions.HasFlag(ChordTensions.SharpNine), "#9");
		AppendFlag(alterations, chord.Tensions.HasFlag(ChordTensions.SharpEleven), "#11");
		AppendFlag(alterations, chord.Tensions.HasFlag(ChordTensions.FlatThirteen), "b13");

		if (alterations.Length > 0)
		{
			bool startsWithAccidental = symbol.Length == chord.Root.Name.Length;
			if (startsWithAccidental)
			{
				_ = symbol.Append('(').Append(alterations).Append(')');
			}
			else
			{
				_ = symbol.Append(alterations);
			}
		}

		if (chord.Sixth == SixthType.Natural)
		{
			if (hasSeventh)
			{
				_ = symbol.Append("add6");
			}
		}
		else if (chord.Sixth == SixthType.Flat && hasSeventh)
		{
			_ = symbol.Append("addb6");
		}

		AppendFlag(symbol, remaining.HasFlag(ChordTensions.Nine), "add9");
		AppendFlag(symbol, remaining.HasFlag(ChordTensions.Eleven), "add11");
		AppendFlag(symbol, remaining.HasFlag(ChordTensions.Thirteen), "add13");
		AppendFlag(symbol, chord.Omissions.HasFlag(ChordOmissions.Third), "no3");
		AppendFlag(symbol, chord.Omissions.HasFlag(ChordOmissions.Fifth), "no5");
		if (chord.Bass is not null)
		{
			_ = symbol.Append('/').Append(chord.Bass.Name);
		}

		return symbol.ToString();
	}

	internal static bool IsExpressible(Chord chord)
	{
		Ensure.NotNull(chord);
		if (!Enum.IsDefined(chord.Quality)
			|| !Enum.IsDefined(chord.Seventh)
			|| !Enum.IsDefined(chord.Sixth)
			|| (chord.Tensions & ~AllTensions) != 0
			|| (chord.Omissions & ~AllOmissions) != 0)
		{
			return false;
		}

		if (chord.Seventh == SeventhType.Diminished && chord.Quality != ChordQuality.Diminished)
		{
			return false;
		}

		if (chord.Quality == ChordQuality.Power
			&& (chord.Seventh != SeventhType.None
				|| chord.Sixth != SixthType.None
				|| chord.Tensions != ChordTensions.None
				|| chord.Omissions != ChordOmissions.None))
		{
			return false;
		}

		if ((chord.Tensions.HasFlag(ChordTensions.Nine)
				&& (chord.Tensions.HasFlag(ChordTensions.FlatNine) || chord.Tensions.HasFlag(ChordTensions.SharpNine)))
			|| (chord.Tensions.HasFlag(ChordTensions.Eleven) && chord.Tensions.HasFlag(ChordTensions.SharpEleven))
			|| (chord.Tensions.HasFlag(ChordTensions.Thirteen) && chord.Tensions.HasFlag(ChordTensions.FlatThirteen)))
		{
			return false;
		}

		return true;
	}

	private static void AppendQuality(StringBuilder symbol, Chord chord)
	{
		switch (chord.Quality)
		{
			case ChordQuality.Minor:
			case ChordQuality.MinorSharpFive:
				_ = symbol.Append('m');
				break;
			case ChordQuality.Augmented:
				_ = symbol.Append("aug");
				break;
			case ChordQuality.Diminished when chord.Seventh == SeventhType.Dominant:
				_ = symbol.Append('m');
				break;
			case ChordQuality.Diminished when chord.Seventh == SeventhType.Major:
				_ = symbol.Append("dimmaj");
				break;
			case ChordQuality.Diminished:
				_ = symbol.Append("dim");
				break;
			case ChordQuality.Major:
			case ChordQuality.MajorFlatFive:
			case ChordQuality.Sus2:
			case ChordQuality.Sus4:
			case ChordQuality.Power:
				break;
		}

		if (chord.Quality == ChordQuality.Power)
		{
			_ = symbol.Append('5');
		}
	}

	private static ChordTensions AppendExtension(StringBuilder symbol, Chord chord)
	{
		if (chord.Seventh == SeventhType.Diminished)
		{
			_ = symbol.Append('7');
			return ChordTensions.None;
		}

		if (chord.Seventh == SeventhType.None)
		{
			return ChordTensions.None;
		}

		int extension = GetExtension(chord.Tensions);
		if (chord.Quality == ChordQuality.Diminished && chord.Seventh == SeventhType.Major)
		{
			_ = symbol.Append('7');
		}
		else if (extension == 0)
		{
			_ = symbol.Append(chord.Seventh == SeventhType.Major ? "maj7" : "7");
		}
		else
		{
			if (chord.Seventh == SeventhType.Major)
			{
				_ = symbol.Append("maj");
			}

			_ = symbol.Append(extension);
		}

		return extension switch
		{
			13 => ChordTensions.Nine | ChordTensions.Eleven | ChordTensions.Thirteen,
			11 => ChordTensions.Nine | ChordTensions.Eleven,
			9 => ChordTensions.Nine,
			_ => ChordTensions.None,
		};
	}

	private static int GetExtension(ChordTensions tensions)
	{
		bool hasNine = (tensions & (ChordTensions.Nine | ChordTensions.FlatNine | ChordTensions.SharpNine)) != 0;
		bool hasEleven = (tensions & (ChordTensions.Eleven | ChordTensions.SharpEleven)) != 0;
		if (tensions.HasFlag(ChordTensions.Thirteen) && hasEleven && hasNine)
		{
			return 13;
		}

		if (tensions.HasFlag(ChordTensions.Eleven) && hasNine)
		{
			return 11;
		}

		return tensions.HasFlag(ChordTensions.Nine) ? 9 : 0;
	}

	private static void AppendSuspension(StringBuilder symbol, Chord chord)
	{
		if (chord.Quality == ChordQuality.Sus2)
		{
			_ = symbol.Append("sus2");
		}
		else if (chord.Quality == ChordQuality.Sus4)
		{
			_ = symbol.Append("sus4");
		}
	}

	private static void AppendFlag(StringBuilder target, bool include, string text)
	{
		if (include && text.Length > 0)
		{
			_ = target.Append(text);
		}
	}
}
