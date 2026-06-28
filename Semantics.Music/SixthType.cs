// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The added sixth present in a chord, if any.</summary>
public enum SixthType
{
	/// <summary>No added sixth.</summary>
	None,

	/// <summary>A major (natural) sixth, 9 semitones above the root (e.g. C6, Cm6).</summary>
	Natural,

	/// <summary>A minor (flat) sixth, 8 semitones above the root (e.g. Cmb6).</summary>
	Flat,
}
