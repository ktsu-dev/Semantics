// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

using System;

/// <summary>Chord tones intentionally omitted from a voicing (reduced voicings).</summary>
[Flags]
public enum ChordOmissions
{
	/// <summary>Nothing omitted.</summary>
	None = 0,

	/// <summary>The third is omitted (e.g. m7no3 → open root/fifth/seventh).</summary>
	Third = 1,

	/// <summary>The fifth is omitted (e.g. 7no5, add9no5).</summary>
	Fifth = 2,
}
