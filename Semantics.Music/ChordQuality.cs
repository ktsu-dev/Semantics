// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>The triad quality of a chord.</summary>
public enum ChordQuality
{
	/// <summary>Major triad (major third, perfect fifth).</summary>
	Major,

	/// <summary>Minor triad (minor third, perfect fifth).</summary>
	Minor,

	/// <summary>Diminished triad (minor third, diminished fifth).</summary>
	Diminished,

	/// <summary>Augmented triad (major third, augmented fifth).</summary>
	Augmented,

	/// <summary>Suspended second (major second replaces the third).</summary>
	Sus2,

	/// <summary>Suspended fourth (perfect fourth replaces the third).</summary>
	Sus4,

	/// <summary>Power chord (root and perfect fifth only, no third).</summary>
	Power,
}
