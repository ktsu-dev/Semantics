// Copyright (c) 2023-2026 ktsu-dev contributors

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
	/// An augmented-sixth chord on the lowered sixth degree: Italian ("It+6", the shape of a dominant
	/// seventh with no fifth), French ("Fr+6", a dominant seventh with a flat fifth) or German ("Ger+6",
	/// a dominant seventh, reported only when it resolves to the dominant or to a cadential six-four,
	/// since otherwise it is indistinguishable from the dominant seventh it sounds like).
	/// </summary>
	AugmentedSixth,

	/// <summary>A chromatic chord with no more specific classification.</summary>
	Chromatic,
}
