// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The structural role of a section within a piece.</summary>
public enum SectionType
{
	/// <summary>An introduction.</summary>
	Intro,

	/// <summary>A verse.</summary>
	Verse,

	/// <summary>A pre-chorus leading into the chorus.</summary>
	PreChorus,

	/// <summary>A chorus / refrain hook.</summary>
	Chorus,

	/// <summary>A post-chorus following the chorus.</summary>
	PostChorus,

	/// <summary>A bridge providing contrast.</summary>
	Bridge,

	/// <summary>A solo section.</summary>
	Solo,

	/// <summary>An instrumental interlude.</summary>
	Interlude,

	/// <summary>A recurring refrain.</summary>
	Refrain,

	/// <summary>An outro / ending section.</summary>
	Outro,

	/// <summary>A coda / tail.</summary>
	Coda,

	/// <summary>Any other or unspecified role.</summary>
	Other,
}
