// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>A pitch alteration. The underlying value is the semitone offset.</summary>
public enum Accidental
{
	/// <summary>Double flat (−2 semitones).</summary>
	DoubleFlat = -2,

	/// <summary>Flat (−1 semitone).</summary>
	Flat = -1,

	/// <summary>Natural (no alteration).</summary>
	Natural = 0,

	/// <summary>Sharp (+1 semitone).</summary>
	Sharp = 1,

	/// <summary>Double sharp (+2 semitones).</summary>
	DoubleSharp = 2,
}
