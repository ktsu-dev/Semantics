// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>The seventh present in a chord, if any.</summary>
public enum SeventhType
{
	/// <summary>No seventh.</summary>
	None,

	/// <summary>Minor (dominant) seventh, 10 semitones above the root.</summary>
	Dominant,

	/// <summary>Major seventh, 11 semitones above the root.</summary>
	Major,

	/// <summary>Diminished seventh, 9 semitones above the root.</summary>
	Diminished,
}
