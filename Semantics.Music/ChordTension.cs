// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

using System;

/// <summary>Upper-structure tensions and alterations present in a chord.</summary>
[Flags]
public enum ChordTension
{
	/// <summary>No tensions.</summary>
	None = 0,

	/// <summary>Flat ninth (13 semitones above the root).</summary>
	FlatNine = 1,

	/// <summary>Natural ninth (14 semitones).</summary>
	Nine = 2,

	/// <summary>Sharp ninth (15 semitones).</summary>
	SharpNine = 4,

	/// <summary>Eleventh (17 semitones).</summary>
	Eleven = 8,

	/// <summary>Sharp eleventh (18 semitones).</summary>
	SharpEleven = 16,

	/// <summary>Flat thirteenth (20 semitones).</summary>
	FlatThirteen = 32,

	/// <summary>Natural thirteenth (21 semitones).</summary>
	Thirteen = 64,
}
