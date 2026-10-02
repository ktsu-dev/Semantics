// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

internal sealed class ChordSymbolReader(string text)
{
	private readonly string text = text;
	private int index;

	internal static bool TryRead(string text, out Chord? chord)
	{
		ChordSymbolReader reader = new(text);
		chord = null;
		if (!reader.TryReadRoot(out PitchClass? root)
			|| !reader.TryReadBody(out ChordBuilder builder))
		{
			return false;
		}

		PitchClass? bass = null;
		if (reader.TryTake("/") && (!reader.TryReadRoot(out bass) || reader.index != text.Length))
		{
			return false;
		}

		if (reader.index != text.Length || !builder.TryBuild(root!, bass, out chord))
		{
			chord = null;
			return false;
		}

		return true;
	}

	private bool TryReadRoot([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PitchClass? root)
	{
		root = null;
		if (index >= text.Length || !Notation.TryReadNoteLetter(text[index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(text, ref index);
		root = PitchClass.Create((int)letter + accidental);
		return true;
	}

	private bool TryReadBody(out ChordBuilder builder)
	{
		builder = new ChordBuilder();
		if (!TryReadQuality(builder))
		{
			return false;
		}

		bool susBeforeExtension = TryReadSus(builder);
		if (!TryReadExtension(builder))
		{
			return false;
		}

		if (!susBeforeExtension)
		{
			_ = TryReadSus(builder);
		}

		return TryReadModifiers(builder);
	}

	private bool TryReadQuality(ChordBuilder builder)
	{
		ReadMinorQuality(builder);
		if (!ReadDiminishedQuality(builder))
		{
			return false;
		}

		ReadAugmentedQuality(builder);
		ReadMajorMarker(builder);
		return HasValidQualityCombination(builder);
	}

	private void ReadMinorQuality(ChordBuilder builder)
	{
		if (TryTake("min") || (Peek('m') && !StartsWith("maj") && !StartsWith("Maj") && TryTake("m")) || TryTake("-"))
		{
			builder.Minor = true;
		}
	}

	private bool ReadDiminishedQuality(ChordBuilder builder)
	{
		if (TryTake("dim") || TryTake("°"))
		{
			builder.Diminished = true;
			return !builder.Minor;
		}

		if (!TryTake("ø"))
		{
			return true;
		}

		builder.Diminished = true;
		builder.HalfDiminished = true;
		return !builder.Minor;
	}

	private void ReadAugmentedQuality(ChordBuilder builder)
	{
		if (TryTake("aug") || (Peek('+') && !IsDigit(PeekNext()) && TryTake("+")))
		{
			builder.Augmented = true;
		}
	}

	private void ReadMajorMarker(ChordBuilder builder)
	{
		if (TryTakeMajorWord())
		{
			builder.MajorMarker = true;
			builder.DeltaMarker = text[index - 1] == 'Δ';
		}
	}

	private static bool HasValidQualityCombination(ChordBuilder builder) =>
		!(builder.Diminished && builder.Augmented)
		&& !(builder.Minor && builder.Diminished)
		&& !(builder.MajorMarker && (builder.Diminished || builder.Augmented) && builder.Minor);

	private bool TryReadExtension(ChordBuilder builder)
	{
		if (TryReadSixthExtension(builder) || TryReadPowerExtension(builder))
		{
			return true;
		}

		return TryReadNumberExtension(builder);
	}

	private bool TryReadSixthExtension(ChordBuilder builder)
	{
		if (StartsWith("(b6)") || StartsWith("(♭6)"))
		{
			index++;
			if (!TryTakeEither("b6", "♭6") || !TryTake(")"))
			{
				return false;
			}

			builder.Sixth = SixthType.Flat;
			return true;
		}

		if (TryTakeEither("b6", "♭6"))
		{
			builder.Sixth = SixthType.Flat;
			return true;
		}

		if (!TryTake("6"))
		{
			return false;
		}

		builder.Sixth = SixthType.Natural;
		if (TryTake("/9") || TryTake("9"))
		{
			if (index < text.Length && IsDigit(text[index]))
			{
				return false;
			}

			builder.ExtendedNumber = 6;
			builder.AddedTensions |= ChordTensions.Nine;
		}

		return true;
	}

	private bool TryReadPowerExtension(ChordBuilder builder)
	{
		if (!TryTake("5"))
		{
			return false;
		}

		builder.Power = true;
		return true;
	}

	private bool TryReadNumberExtension(ChordBuilder builder)
	{
		if (!TryReadNumber(out int number))
		{
			return true;
		}

		builder.ExtendedNumber = number;
		bool majorAfterNumber = TryTake("M");
		if (majorAfterNumber)
		{
			builder.MajorMarker = true;
		}

		if (number == 7 && !majorAfterNumber && TryReadLegacyNumber(out int legacyNumber))
		{
			builder.ExtendedNumber = legacyNumber;
		}

		return true;
	}

	private bool TryReadSus(ChordBuilder builder)
	{
		if (TryTake("sus2"))
		{
			builder.Sus = ChordQuality.Sus2;
			return true;
		}

		if (TryTake("sus4") || TryTake("sus"))
		{
			builder.Sus = ChordQuality.Sus4;
			return true;
		}

		return false;
	}

	private bool TryReadModifiers(ChordBuilder builder)
	{
		while (index < text.Length && text[index] != '/')
		{
			if (TryReadModifierGroup(builder))
			{
				continue;
			}

			if (!TryReadModifier(builder))
			{
				return false;
			}
		}

		return true;
	}

	private bool TryReadModifierGroup(ChordBuilder builder)
	{
		int start = index;
		if (!TryTake("("))
		{
			return false;
		}

		bool any = false;
		while (!Peek(')'))
		{
			if (!TryReadModifier(builder))
			{
				index = start;
				return false;
			}

			any = true;
			if (TryTake(",") && Peek(')'))
			{
				index = start;
				return false;
			}
		}

		if (!any)
		{
			index = start;
			return false;
		}

		return TryTake(")");
	}

	private bool TryReadModifier(ChordBuilder builder)
	{
		int start = index;
		if (TryReadModifierGroup(builder))
		{
			return true;
		}

		if (builder.ExtendedNumber > 0 && Peek('+') && !IsDigit(PeekNext()))
		{
			if (builder.Minor || builder.Diminished || builder.Augmented || builder.MajorMarker)
			{
				return false;
			}

			index++;
			builder.Augmented = true;
			return true;
		}

		if (TryTakeEither("b6", "♭6"))
		{
			if (builder.SetSixth(SixthType.Flat))
			{
				return true;
			}

			index = start;
			return false;
		}

		bool parsed = TryReadAlteration(builder)
			|| TryReadAdd(builder)
			|| TryReadOmit(builder);
		if (!parsed)
		{
			index = start;
		}

		return parsed;
	}

	private bool TryReadAlteration(ChordBuilder builder)
	{
		bool flat = TryTakeEither("b", "♭") || TryTake("-");
		if (!flat)
		{
			bool sharp = TryTakeEither("#", "♯") || (Peek('+') && TryTake("+"));
			if (!sharp)
			{
				return false;
			}

			if (TryTake("5"))
			{
				return builder.SetFifthAlteration(1);
			}

			if (TryTake("9"))
			{
				return builder.SetTension(ChordTensions.SharpNine, ChordTensions.FlatNine | ChordTensions.Nine);
			}

			if (TryTake("11"))
			{
				return builder.SetTension(ChordTensions.SharpEleven, ChordTensions.Eleven);
			}

			return false;
		}

		if (TryTake("5"))
		{
			return builder.SetFifthAlteration(-1);
		}

		if (TryTake("9"))
		{
			return builder.SetTension(ChordTensions.FlatNine, ChordTensions.SharpNine | ChordTensions.Nine);
		}

		if (TryTake("13"))
		{
			return builder.SetTension(ChordTensions.FlatThirteen, ChordTensions.Thirteen);
		}

		return false;
	}

	private bool TryReadAdd(ChordBuilder builder)
	{
		if (!TryTake("add"))
		{
			return false;
		}

		if (TryTake("b6"))
		{
			return builder.SetSixth(SixthType.Flat);
		}

		if (TryTake("6"))
		{
			return builder.SetSixth(SixthType.Natural);
		}

		if (TryTake("9"))
		{
			return builder.SetTension(ChordTensions.Nine, ChordTensions.FlatNine | ChordTensions.SharpNine);
		}

		if (TryTake("11"))
		{
			return builder.SetTension(ChordTensions.Eleven, ChordTensions.SharpEleven);
		}

		if (TryTake("13"))
		{
			return builder.SetTension(ChordTensions.Thirteen, ChordTensions.FlatThirteen);
		}

		return false;
	}

	private bool TryReadOmit(ChordBuilder builder)
	{
		if (!TryTake("no3"))
		{
			if (!TryTake("no5"))
			{
				return false;
			}

			if (builder.Omissions.HasFlag(ChordOmissions.Fifth))
			{
				return false;
			}

			builder.Omissions |= ChordOmissions.Fifth;
			return true;
		}

		if (builder.Omissions.HasFlag(ChordOmissions.Third))
		{
			return false;
		}

		builder.Omissions |= ChordOmissions.Third;
		return true;
	}

	private bool TryTakeMajorWord() =>
		TryTake("maj") || TryTake("Maj") || TryTake("M") || TryTake("Δ");

	private bool TryReadNumber(out int number)
	{
		if (TryTake("13"))
		{
			number = 13;
			return true;
		}

		if (TryTake("11"))
		{
			number = 11;
			return true;
		}

		if (TryTake("9"))
		{
			number = 9;
			return true;
		}

		if (TryTake("7"))
		{
			number = 7;
			return true;
		}

		number = 0;
		return false;
	}

	private bool TryReadLegacyNumber(out int number)
	{
		if (TryTake("13"))
		{
			number = 13;
			return true;
		}

		if (TryTake("11"))
		{
			number = 11;
			return true;
		}

		if (TryTake("9"))
		{
			number = 9;
			return true;
		}

		number = 0;
		return false;
	}

	private bool TryTakeEither(string ascii, string unicode) => TryTake(ascii) || TryTake(unicode);

	private bool TryTake(string token)
	{
		if (index + token.Length > text.Length
			|| string.CompareOrdinal(text, index, token, 0, token.Length) != 0)
		{
			return false;
		}

		index += token.Length;
		return true;
	}

	private bool StartsWith(string token) =>
		index + token.Length <= text.Length
		&& string.CompareOrdinal(text, index, token, 0, token.Length) == 0;

	private bool Peek(char value) => index < text.Length && text[index] == value;

	private char PeekNext() => index + 1 < text.Length ? text[index + 1] : '\0';

	private static bool IsDigit(char value) => value is >= '0' and <= '9';

	private sealed class ChordBuilder
	{
		internal bool Minor { get; set; }
		internal bool Diminished { get; set; }
		internal bool HalfDiminished { get; set; }
		internal bool Augmented { get; set; }
		internal bool MajorMarker { get; set; }
		internal bool DeltaMarker { get; set; }
		internal bool Power { get; set; }
		internal ChordQuality? Sus { get; set; }
		internal int ExtendedNumber { get; set; }
		internal int FifthAlteration { get; private set; }
		internal SixthType Sixth { get; set; }
		internal ChordTensions AddedTensions { get; set; }
		internal ChordOmissions Omissions { get; set; }

		internal bool SetFifthAlteration(int alteration)
		{
			if (FifthAlteration != 0)
			{
				return false;
			}

			FifthAlteration = alteration;
			return true;
		}

		internal bool SetTension(ChordTensions tension, ChordTensions conflicts)
		{
			if ((AddedTensions & (tension | conflicts)) != 0)
			{
				return false;
			}

			AddedTensions |= tension;
			return true;
		}

		internal bool SetSixth(SixthType sixth)
		{
			if (Sixth != SixthType.None)
			{
				return false;
			}

			Sixth = sixth;
			return true;
		}

		internal bool TryBuild(PitchClass root, PitchClass? bass, out Chord? chord)
		{
			chord = null;
			if (!HasValidShape() || HasRepeatedExtensionTension())
			{
				return false;
			}

			ChordQuality quality = DetermineQuality();
			SeventhType seventh = DetermineSeventh(quality);
			ChordTensions tensions = ApplyExtensionStack(ref seventh);
			if (ExtendedNumber == 13 && tensions.HasFlag(ChordTensions.FlatThirteen))
			{
				return false;
			}

			chord = new Chord
			{
				Root = root,
				Quality = quality,
				Seventh = seventh,
				Sixth = Sixth,
				Tensions = tensions,
				Omissions = Omissions,
				Bass = bass,
			};
			return true;
		}

		private bool HasValidShape() =>
			!HasInvalidSuspension()
			&& !HasInvalidMajorMarker()
			&& !HasInvalidPower()
			&& !(Diminished && FifthAlteration != 0)
			&& !(FifthAlteration < 0 && Augmented);

		private bool HasInvalidSuspension() =>
			Sus is not null && (Minor || Diminished || Augmented || MajorMarker || Power || FifthAlteration != 0);

		private bool HasInvalidMajorMarker() =>
			(MajorMarker && !DeltaMarker && ExtendedNumber == 0 && (Minor || Diminished || Augmented))
			|| (DeltaMarker && ExtendedNumber == 6)
			|| (MajorMarker && ExtendedNumber == 6 && (Minor || Diminished || Augmented));

		private bool HasInvalidPower() =>
			Power && (Minor || Diminished || Augmented || MajorMarker || ExtendedNumber != 0
				|| Sixth != SixthType.None || AddedTensions != ChordTensions.None
				|| Omissions != ChordOmissions.None || FifthAlteration != 0);

		private bool HasRepeatedExtensionTension() =>
			(ExtendedNumber >= 9 && AddedTensions.HasFlag(ChordTensions.Nine))
			|| (ExtendedNumber >= 11 && AddedTensions.HasFlag(ChordTensions.Eleven))
			|| (ExtendedNumber >= 13 && AddedTensions.HasFlag(ChordTensions.Thirteen));

		private ChordQuality DetermineQuality()
		{
			if (Sus is not null)
			{
				return Sus.Value;
			}

			if (Power)
			{
				return ChordQuality.Power;
			}

			if (Diminished || (Minor && FifthAlteration < 0))
			{
				return ChordQuality.Diminished;
			}

			if (FifthAlteration > 0 || Augmented)
			{
				return Minor ? ChordQuality.MinorSharpFive : ChordQuality.Augmented;
			}

			if (FifthAlteration < 0)
			{
				return ChordQuality.MajorFlatFive;
			}

			return Minor ? ChordQuality.Minor : ChordQuality.Major;
		}

		private SeventhType DetermineSeventh(ChordQuality quality)
		{
			if (ExtendedNumber is 0 or 6)
			{
				if (HalfDiminished)
				{
					return SeventhType.Dominant;
				}

				return DeltaMarker ? SeventhType.Major : SeventhType.None;
			}

			if (MajorMarker)
			{
				return SeventhType.Major;
			}

			return quality == ChordQuality.Diminished && Diminished && !HalfDiminished
				? SeventhType.Diminished
				: SeventhType.Dominant;
		}

		private ChordTensions ApplyExtensionStack(ref SeventhType seventh)
		{
			ChordTensions tensions = AddedTensions;
			if (ExtendedNumber < 9)
			{
				return tensions;
			}

			if (!tensions.HasFlag(ChordTensions.FlatNine) && !tensions.HasFlag(ChordTensions.SharpNine))
			{
				tensions |= ChordTensions.Nine;
			}

			if (ExtendedNumber >= 11 && !tensions.HasFlag(ChordTensions.SharpEleven))
			{
				tensions |= ChordTensions.Eleven;
			}

			if (ExtendedNumber >= 13)
			{
				tensions |= ChordTensions.Thirteen;
			}

			if (seventh == SeventhType.None)
			{
				seventh = SeventhType.Dominant;
			}

			return tensions;
		}
	}
}
