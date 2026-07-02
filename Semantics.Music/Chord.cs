// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

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
	public ChordTension Tensions { get; init; } = ChordTension.None;

	/// <summary>Gets the chord tones intentionally omitted from the voicing.</summary>
	public ChordOmission Omissions { get; init; } = ChordOmission.None;

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
		if (symbol.Length == 0)
		{
			throw new FormatException("Chord symbol is empty.");
		}

		// Split off a slash bass first.
		PitchClass? bass = null;
		string head = symbol;
		int slash = symbol.IndexOf('/');
		if (slash >= 0)
		{
			int bassIndex = 0;
			bass = ParseRoot(symbol[(slash + 1)..], ref bassIndex);
			head = symbol[..slash];
		}

		int index = 0;
		PitchClass root = ParseRoot(head, ref index);
		string body = new([.. head[index..].Where(c => c is not ('(' or ')'))]);

		ChordModifiers modifiers = ConsumeModifiers(ref body);
		ChordQuality quality = DetermineQuality(body, modifiers.FifthAlteration);
		SeventhType seventh = DetermineSeventh(body, quality);

		SixthType sixth = modifiers.Sixth;
		if (sixth == SixthType.None && body.Contains('6'))
		{
			sixth = SixthType.Natural;
		}

		ChordTension tensions = modifiers.Tensions;
		ApplyExtensions(ref body, modifiers.HasAdd9, ref seventh, ref tensions);

		return new Chord
		{
			Root = root,
			Quality = quality,
			Seventh = seventh,
			Sixth = sixth,
			Tensions = tensions,
			Omissions = modifiers.Omissions,
			Bass = bass,
		};
	}

	private static PitchClass ParseRoot(string symbol, ref int index)
	{
		if (index >= symbol.Length)
		{
			throw new FormatException($"Missing chord root in '{symbol}'.");
		}

		int letterPc = char.ToUpperInvariant(symbol[index]) switch
		{
			'C' => 0,
			'D' => 2,
			'E' => 4,
			'F' => 5,
			'G' => 7,
			'A' => 9,
			'B' => 11,
			_ => throw new FormatException($"Invalid chord root in '{symbol}'."),
		};
		index++;

		int accidental = 0;
		while (index < symbol.Length && (symbol[index] is '#' or 'b' or '♯' or '♭'))
		{
			accidental += symbol[index] is '#' or '♯' ? 1 : -1;
			index++;
		}

		return PitchClass.Create(letterPc + accidental);
	}

	/// <summary>The fifth alteration and the upper-structure modifiers consumed from a chord body.</summary>
	private readonly record struct ChordModifiers(
		ChordOmission Omissions,
		SixthType Sixth,
		ChordTension Tensions,
		int FifthAlteration,
		bool HasAdd9);

	private static ChordModifiers ConsumeModifiers(ref string body)
	{
		ChordOmission omissions = ChordOmission.None;
		if (Take(ref body, "no5"))
		{
			omissions |= ChordOmission.Fifth;
		}

		if (Take(ref body, "no3"))
		{
			omissions |= ChordOmission.Third;
		}

		// Flat sixth before the bare "6".
		SixthType sixth = (Take(ref body, "b6") || Take(ref body, "♭6")) ? SixthType.Flat : SixthType.None;

		// Altered tensions: consume multi-character tokens before the bare numbers.
		ChordTension tensions = ChordTension.None;
		if (Take(ref body, "#11") || Take(ref body, "♯11"))
		{
			tensions |= ChordTension.SharpEleven;
		}

		if (Take(ref body, "b13") || Take(ref body, "♭13"))
		{
			tensions |= ChordTension.FlatThirteen;
		}

		if (Take(ref body, "b9") || Take(ref body, "♭9"))
		{
			tensions |= ChordTension.FlatNine;
		}

		if (Take(ref body, "#9") || Take(ref body, "♯9"))
		{
			tensions |= ChordTension.SharpNine;
		}

		int fifthAlteration = 0;
		if (Take(ref body, "#5") || Take(ref body, "♯5"))
		{
			fifthAlteration = 1;
		}

		if (Take(ref body, "b5") || Take(ref body, "♭5"))
		{
			fifthAlteration = -1;
		}

		// "add9" must be consumed before the bare "9" logic so it does not imply a seventh.
		bool hasAdd9 = Take(ref body, "add9");
		if (hasAdd9)
		{
			tensions |= ChordTension.Nine;
		}

		return new ChordModifiers(omissions, sixth, tensions, fifthAlteration, hasAdd9);
	}

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

	private static void ApplyExtensions(ref string body, bool hasAdd9, ref SeventhType seventh, ref ChordTension tensions)
	{
		// Bare extension numbers (9/11/13) imply a dominant seventh and stack the lower tensions.
		if (hasAdd9)
		{
			return;
		}

		if (Take(ref body, "13"))
		{
			tensions |= ChordTension.Nine | ChordTension.Eleven | ChordTension.Thirteen;
		}
		else if (Take(ref body, "11"))
		{
			tensions |= ChordTension.Nine | ChordTension.Eleven;
		}
		else if (Take(ref body, "9"))
		{
			if (!tensions.HasFlag(ChordTension.FlatNine) && !tensions.HasFlag(ChordTension.SharpNine))
			{
				tensions |= ChordTension.Nine;
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

		if (Quality != ChordQuality.Power && !Omissions.HasFlag(ChordOmission.Third))
		{
			_ = offsets.Add(Quality switch
			{
				ChordQuality.Sus2 => 2,
				ChordQuality.Sus4 => 5,
				ChordQuality.Minor or ChordQuality.Diminished => 3,
				_ => 4,
			});
		}

		if (!Omissions.HasFlag(ChordOmission.Fifth))
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

		AddTension(offsets, ChordTension.FlatNine, 13);
		AddTension(offsets, ChordTension.Nine, 14);
		AddTension(offsets, ChordTension.SharpNine, 15);
		AddTension(offsets, ChordTension.Eleven, 17);
		AddTension(offsets, ChordTension.SharpEleven, 18);
		AddTension(offsets, ChordTension.FlatThirteen, 20);
		AddTension(offsets, ChordTension.Thirteen, 21);

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

	private void AddTension(SortedSet<int> offsets, ChordTension flag, int semitones)
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
