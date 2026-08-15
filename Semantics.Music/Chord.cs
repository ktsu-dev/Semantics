// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// A chord parsed from a symbol such as "Cmaj7", "Dm7", "E7b9", "Cm7b5", "Cmmaj7", "C6", or "C/G".
/// </summary>
public sealed record Chord
{
	/// <summary>Gets the chord root.</summary>
	public PitchClass Root { get; init; } = PitchClass.Create(0);

	/// <summary>Gets the triad quality.</summary>
	public ChordQuality Quality { get; init; } = ChordQuality.Major;

	/// <summary>Gets the seventh, if any.</summary>
	public SeventhType Seventh { get; init; } = SeventhType.None;

	/// <summary>Gets the added sixth, if any.</summary>
	public SixthType Sixth { get; init; } = SixthType.None;

	/// <summary>Gets the upper-structure tensions and alterations.</summary>
	public ChordTensions Tensions { get; init; } = ChordTensions.None;

	/// <summary>Gets the chord tones intentionally omitted from the voicing.</summary>
	public ChordOmissions Omissions { get; init; } = ChordOmissions.None;

	/// <summary>Gets the slash-chord bass, if any (otherwise the root sounds in the bass).</summary>
	public PitchClass? Bass { get; init; }

	/// <summary>Parses a chord symbol.</summary>
	/// <param name="symbol">The chord symbol.</param>
	/// <returns>The parsed chord.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="symbol"/> is null.</exception>
	/// <exception cref="FormatException">Thrown when the symbol cannot be parsed.</exception>
	public static Chord Parse(string symbol)
	{
		Ensure.NotNull(symbol);
		return TryParse(symbol, out Chord? result)
			? result
			: throw new FormatException($"Invalid chord symbol '{symbol}'.");
	}

	/// <summary>Tries to parse a chord symbol.</summary>
	/// <param name="symbol">The text to parse.</param>
	/// <param name="result">The parsed chord, or null on failure.</param>
	/// <returns><see langword="true"/> when parsing succeeds.</returns>
	public static bool TryParse(string? symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Chord? result)
	{
		result = null;
		if (symbol is null || symbol.Length == 0)
		{
			return false;
		}

		if (!TryReadRoot(symbol, out PitchClass? bass, out string head, out int index, out PitchClass? root))
		{
			return false;
		}

		string body = new([.. head[index..].Where(c => c is not ('(' or ')'))]);
		ChordModifiers modifiers = ConsumeModifiers(ref body);
		ChordQuality quality = DetermineQuality(body, modifiers.FifthAlteration);
		SeventhType seventh = DetermineSeventh(body, quality);

		SixthType sixth = modifiers.Sixth;
		if (sixth == SixthType.None && body.Contains('6'))
		{
			sixth = SixthType.Natural;
		}

		ChordTensions tensions = modifiers.Tensions;
		ApplyExtensions(ref body, modifiers.HasAdd9, ref seventh, ref tensions);

		result = new Chord
		{
			Root = root,
			Quality = quality,
			Seventh = seventh,
			Sixth = sixth,
			Tensions = tensions,
			Omissions = modifiers.Omissions,
			Bass = bass,
		};
		return true;
	}

	private static bool TryReadRoot(string symbol, out PitchClass? bass, out string head, out int index, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PitchClass? root)
	{
		bass = null;
		head = symbol;
		index = 0;
		root = null;

		int slash = symbol.IndexOf('/');
		if (slash >= 0)
		{
			int bassIndex = 0;
			if (!TryParseRoot(symbol[(slash + 1)..], ref bassIndex, out PitchClass? parsedBass))
			{
				return false;
			}

			bass = parsedBass;
			head = symbol[..slash];
		}

		return TryParseRoot(head, ref index, out root);
	}

	private static bool TryParseRoot(string symbol, ref int index, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PitchClass? root)
	{
		root = null;
		if (index >= symbol.Length || !Notation.TryReadNoteLetter(symbol[index], out NoteLetter letter))
		{
			return false;
		}

		index++;
		int accidental = Notation.ReadAccidentalOffset(symbol, ref index);
		root = PitchClass.Create((int)letter + accidental);
		return true;
	}

	/// <summary>The fifth alteration and the upper-structure modifiers consumed from a chord body.</summary>
	private readonly record struct ChordModifiers(
		ChordOmissions Omissions,
		SixthType Sixth,
		ChordTensions Tensions,
		int FifthAlteration,
		bool HasAdd9);

	/// <summary>
	/// Consumes the modifier tokens from a chord body, in the order they must be taken: the
	/// omissions, the flat sixth (before any bare "6"), the altered tensions (multi-character
	/// tokens before bare numbers), the fifth alteration, and finally "add9".
	/// </summary>
	private static ChordModifiers ConsumeModifiers(ref string body)
	{
		ChordOmissions omissions = ConsumeOmissions(ref body);

		// Flat sixth before the bare "6".
		SixthType sixth = TakeEither(ref body, "b6", "♭6") ? SixthType.Flat : SixthType.None;

		ChordTensions tensions = ConsumeTensions(ref body);
		int fifthAlteration = ConsumeFifthAlteration(ref body);

		// "add9" must be consumed before the bare "9" logic so it does not imply a seventh.
		bool hasAdd9 = Take(ref body, "add9");
		if (hasAdd9)
		{
			tensions |= ChordTensions.Nine;
		}

		return new ChordModifiers(omissions, sixth, tensions, fifthAlteration, hasAdd9);
	}

	private static ChordOmissions ConsumeOmissions(ref string body)
	{
		ChordOmissions omissions = ChordOmissions.None;
		if (Take(ref body, "no5"))
		{
			omissions |= ChordOmissions.Fifth;
		}

		if (Take(ref body, "no3"))
		{
			omissions |= ChordOmissions.Third;
		}

		return omissions;
	}

	/// <summary>Consumes the altered tensions, taking multi-character tokens before bare numbers.</summary>
	private static ChordTensions ConsumeTensions(ref string body)
	{
		ChordTensions tensions = ChordTensions.None;
		if (TakeEither(ref body, "#11", "♯11"))
		{
			tensions |= ChordTensions.SharpEleven;
		}

		if (TakeEither(ref body, "b13", "♭13"))
		{
			tensions |= ChordTensions.FlatThirteen;
		}

		if (TakeEither(ref body, "b9", "♭9"))
		{
			tensions |= ChordTensions.FlatNine;
		}

		if (TakeEither(ref body, "#9", "♯9"))
		{
			tensions |= ChordTensions.SharpNine;
		}

		return tensions;
	}

	/// <summary>Consumes the fifth alteration, returning +1 for a sharp fifth, -1 for a flat fifth, 0 for neither.</summary>
	private static int ConsumeFifthAlteration(ref string body)
	{
		int fifthAlteration = 0;
		if (TakeEither(ref body, "#5", "♯5"))
		{
			fifthAlteration = 1;
		}

		if (TakeEither(ref body, "b5", "♭5"))
		{
			fifthAlteration = -1;
		}

		return fifthAlteration;
	}

	/// <summary>
	/// Takes the ASCII spelling of a token, falling back to its Unicode spelling. Only one is
	/// ever consumed — the fallback is not attempted once the ASCII form matches.
	/// </summary>
	private static bool TakeEither(ref string body, string asciiToken, string unicodeToken) =>
		Take(ref body, asciiToken) || Take(ref body, unicodeToken);

	private static ChordQuality DetermineQuality(string body, int fifthAlteration)
	{
		if (body.Contains("sus2", StringComparison.Ordinal))
		{
			return ChordQuality.Sus2;
		}

		if (body.Contains("sus", StringComparison.Ordinal))
		{
			return ChordQuality.Sus4;
		}

		if (body == "5")
		{
			return ChordQuality.Power;
		}

		if (body.Contains("dim", StringComparison.Ordinal) || body.Contains('°'))
		{
			return ChordQuality.Diminished;
		}

		if (body.Contains("aug", StringComparison.Ordinal) || body.Contains('+'))
		{
			return ChordQuality.Augmented;
		}

		if (fifthAlteration < 0)
		{
			// A lowered fifth with a (typically minor) third: a diminished/half-diminished colour.
			return ChordQuality.Diminished;
		}

		if (fifthAlteration > 0)
		{
			// A raised fifth with a (typically major) third: an augmented colour.
			return ChordQuality.Augmented;
		}

		return IsMinor(body) ? ChordQuality.Minor : ChordQuality.Major;
	}

	private static bool IsMinor(string body) =>
		body.Contains("min", StringComparison.Ordinal)
		|| body.Contains('-')
		|| (body.StartsWith('m') && !body.StartsWith("maj", StringComparison.Ordinal));

	private static SeventhType DetermineSeventh(string body, ChordQuality quality)
	{
		bool hasSeven = body.Contains('7');
		bool hasMaj7 = body.Contains("maj", StringComparison.Ordinal) || body.Contains("M7", StringComparison.Ordinal) || body.Contains('Δ');

		if (quality == ChordQuality.Diminished && body.Contains("dim", StringComparison.Ordinal) && hasSeven)
		{
			return SeventhType.Diminished;
		}

		if (hasMaj7)
		{
			return SeventhType.Major;
		}

		return hasSeven ? SeventhType.Dominant : SeventhType.None;
	}

	private static void ApplyExtensions(ref string body, bool hasAdd9, ref SeventhType seventh, ref ChordTensions tensions)
	{
		// Bare extension numbers (9/11/13) imply a dominant seventh and stack the lower tensions.
		if (hasAdd9)
		{
			return;
		}

		if (Take(ref body, "13"))
		{
			tensions |= ChordTensions.Nine | ChordTensions.Eleven | ChordTensions.Thirteen;
		}
		else if (Take(ref body, "11"))
		{
			tensions |= ChordTensions.Nine | ChordTensions.Eleven;
		}
		else if (Take(ref body, "9"))
		{
			if (!tensions.HasFlag(ChordTensions.FlatNine) && !tensions.HasFlag(ChordTensions.SharpNine))
			{
				tensions |= ChordTensions.Nine;
			}
		}
		else
		{
			return;
		}

		seventh = seventh == SeventhType.None ? SeventhType.Dominant : seventh;
	}

	/// <summary>Returns the chord's semitone offsets above the root, ascending and de-duplicated.</summary>
	/// <returns>Sorted offsets: 0 (root), the third, the fifth, any seventh, any sixth, then tensions.</returns>
	public IReadOnlyList<int> ChordTones()
	{
		SortedSet<int> offsets = [0];

		if (Quality != ChordQuality.Power && !Omissions.HasFlag(ChordOmissions.Third))
		{
			_ = offsets.Add(Quality switch
			{
				ChordQuality.Sus2 => 2,
				ChordQuality.Sus4 => 5,
				ChordQuality.Minor or ChordQuality.Diminished => 3,
				_ => 4,
			});
		}

		if (!Omissions.HasFlag(ChordOmissions.Fifth))
		{
			_ = offsets.Add(Quality switch
			{
				ChordQuality.Diminished => 6,
				ChordQuality.Augmented => 8,
				_ => 7,
			});
		}

		if (Seventh != SeventhType.None)
		{
			_ = offsets.Add(Seventh switch
			{
				SeventhType.Diminished => 9,
				SeventhType.Dominant => 10,
				_ => 11,
			});
		}

		if (Sixth == SixthType.Natural)
		{
			_ = offsets.Add(9);
		}
		else if (Sixth == SixthType.Flat)
		{
			_ = offsets.Add(8);
		}

		AddTension(offsets, ChordTensions.FlatNine, 13);
		AddTension(offsets, ChordTensions.Nine, 14);
		AddTension(offsets, ChordTensions.SharpNine, 15);
		AddTension(offsets, ChordTensions.Eleven, 17);
		AddTension(offsets, ChordTensions.SharpEleven, 18);
		AddTension(offsets, ChordTensions.FlatThirteen, 20);
		AddTension(offsets, ChordTensions.Thirteen, 21);

		return [.. offsets];
	}

	/// <summary>Returns the chord transposed by a number of semitones (root and any slash bass move together).</summary>
	/// <param name="semitones">The signed semitone offset.</param>
	/// <returns>The transposed chord, preserving quality, seventh, sixth, tensions, and omissions.</returns>
	public Chord Transpose(int semitones) => this with
	{
		Root = PitchClass.Create(Root.Value + semitones),
		Bass = Bass is null ? null : PitchClass.Create(Bass.Value + semitones),
	};

	/// <summary>Voices the chord in root position with the root at the given octave.</summary>
	/// <param name="octave">The octave for the root (e.g. 4 places the root at C4 for a C chord).</param>
	/// <returns>The pitches, lowest first; a slash bass (if any) sounds one octave below the root.</returns>
	public IReadOnlyList<Pitch> Voice(int octave) => Voice(octave, 0);

	/// <summary>Voices the chord at the given octave and inversion.</summary>
	/// <param name="octave">The octave for the root (e.g. 4 places the root at C4 for a C chord).</param>
	/// <param name="inversion">
	/// The inversion: 0 root position, 1 first inversion, and so on. Each step raises the next-lowest
	/// chord tone by an octave; the value wraps modulo the number of chord tones.
	/// </param>
	/// <returns>The pitches, lowest first; a slash bass (if any) sounds one octave below the root.</returns>
	public IReadOnlyList<Pitch> Voice(int octave, int inversion)
	{
		List<int> tones = [.. ChordTones()];
		int count = tones.Count;
		if (count > 0)
		{
			int rotation = ((inversion % count) + count) % count;
			for (int i = 0; i < rotation; i++)
			{
				tones[i] += 12;
			}

			tones.Sort();
		}

		Pitch rootPitch = Pitch.Parse(Root.Name + octave.ToString(CultureInfo.InvariantCulture));
		List<Pitch> pitches = [.. tones.Select(offset => rootPitch.Transpose(offset))];

		if (Bass is not null)
		{
			Pitch bassPitch = Pitch.Parse(Bass.Name + (octave - 1).ToString(CultureInfo.InvariantCulture));
			pitches.Insert(0, bassPitch);
		}

		return pitches;
	}

	/// <summary>Returns the canonical chord symbol. The formatter is the inverse of <see cref="Parse"/> over the parseable corpus.</summary>
	/// <returns>The canonical chord symbol (e.g. "Cmaj7", "C/G").</returns>
	public override string ToString()
	{
		System.Text.StringBuilder sb = new();
		_ = sb.Append(Root.Name);
		AppendQualityAndSeventh(sb);
		AppendSixth(sb);
		AppendTensions(sb);
		AppendOmissions(sb);
		if (Bass is not null)
		{
			_ = sb.Append('/').Append(Bass.Name);
		}

		return sb.ToString();
	}

	private void AppendQualityAndSeventh(System.Text.StringBuilder sb)
	{
		switch (Quality)
		{
			case ChordQuality.Sus2:
				_ = sb.Append("sus2");
				AppendPlainSeventh(sb);
				break;
			case ChordQuality.Sus4:
				_ = sb.Append("sus4");
				AppendPlainSeventh(sb);
				break;
			case ChordQuality.Power:
				_ = sb.Append('5');
				break;
			case ChordQuality.Augmented:
				_ = sb.Append("aug");
				AppendPlainSeventh(sb);
				break;
			case ChordQuality.Minor:
				_ = sb.Append('m');
				AppendPlainSeventh(sb);
				break;
			case ChordQuality.Diminished:
				AppendDiminished(sb);
				break;
			default:
				AppendPlainSeventh(sb);
				break;
		}
	}

	private void AppendPlainSeventh(System.Text.StringBuilder sb) => _ = sb.Append(Seventh switch
	{
		SeventhType.Major => "maj7",
		SeventhType.Dominant => "7",
		SeventhType.Diminished => "7",
		_ => "",
	});

	private void AppendDiminished(System.Text.StringBuilder sb) => _ = Seventh switch
	{
		SeventhType.Diminished => sb.Append("dim7"),
		SeventhType.Dominant => sb.Append("m7b5"),
		SeventhType.Major => sb.Append("dimmaj7"),
		_ => sb.Append("dim"),
	};

	private void AppendSixth(System.Text.StringBuilder sb) => _ = Sixth switch
	{
		SixthType.Natural => sb.Append('6'),
		SixthType.Flat => sb.Append("b6"),
		_ => sb,
	};

	private void AppendTensions(System.Text.StringBuilder sb)
	{
		// Natural extension stack: 13 implies 9+11+13, 11 implies 9+11. A bare 9 with no
		// seventh must be written "add9" so it does not imply a dominant seventh on reparse.
		bool hasSeventh = Seventh != SeventhType.None;
		if (Tensions.HasFlag(ChordTensions.Thirteen))
		{
			_ = sb.Append("13");
		}
		else if (Tensions.HasFlag(ChordTensions.Eleven))
		{
			_ = sb.Append("11");
		}
		else if (Tensions.HasFlag(ChordTensions.Nine))
		{
			_ = sb.Append(hasSeventh ? "9" : "add9");
		}

		if (Tensions.HasFlag(ChordTensions.FlatNine))
		{
			_ = sb.Append("b9");
		}

		if (Tensions.HasFlag(ChordTensions.SharpNine))
		{
			_ = sb.Append("#9");
		}

		if (Tensions.HasFlag(ChordTensions.SharpEleven))
		{
			_ = sb.Append("#11");
		}

		if (Tensions.HasFlag(ChordTensions.FlatThirteen))
		{
			_ = sb.Append("b13");
		}
	}

	private void AppendOmissions(System.Text.StringBuilder sb)
	{
		if (Omissions.HasFlag(ChordOmissions.Third))
		{
			_ = sb.Append("no3");
		}

		if (Omissions.HasFlag(ChordOmissions.Fifth))
		{
			_ = sb.Append("no5");
		}
	}

	private void AddTension(SortedSet<int> offsets, ChordTensions flag, int semitones)
	{
		if (Tensions.HasFlag(flag))
		{
			_ = offsets.Add(semitones);
		}
	}

	private static bool Take(ref string body, string token)
	{
		int at = body.IndexOf(token, StringComparison.Ordinal);
		if (at < 0)
		{
			return false;
		}

		body = body.Remove(at, token.Length);
		return true;
	}
}
