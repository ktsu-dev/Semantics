// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The category of a non-diatonic (chromatic) chord within a key.</summary>
public enum ChromaticKind
{
	/// <summary>A secondary dominant: a dominant-quality chord tonicizing a diatonic degree.</summary>
	SecondaryDominant,

	/// <summary>A borrowed chord: diatonic to the parallel mode of the same tonic (modal interchange).</summary>
	BorrowedChord,

	/// <summary>A Neapolitan: a major triad on the lowered second degree.</summary>
	Neapolitan,

	/// <summary>
	/// An augmented-sixth chord. Reserved: detection is not implemented because the chord model does
	/// not carry the interval spelling needed to identify Italian/French/German sixths reliably.
	/// </summary>
	AugmentedSixth,

	/// <summary>A chromatic chord with no more specific classification.</summary>
	Chromatic,
}
