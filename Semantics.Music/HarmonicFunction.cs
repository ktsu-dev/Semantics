// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>The functional role a chord plays within a key.</summary>
public enum HarmonicFunction
{
	/// <summary>Tonic function (scale degrees I, iii, vi).</summary>
	Tonic,

	/// <summary>Predominant / subdominant function (scale degrees ii, IV).</summary>
	Predominant,

	/// <summary>Dominant function (scale degrees V, vii).</summary>
	Dominant,

	/// <summary>A chromatic (non-diatonic) chord with no diatonic function.</summary>
	Chromatic,
}
