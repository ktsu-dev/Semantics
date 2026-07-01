// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A recognized named musical form.</summary>
public enum NamedForm
{
	/// <summary>32-bar AABA song form (recognized by the AABA section pattern).</summary>
	ThirtyTwoBarAABA,

	/// <summary>12-bar blues (recognized by a section's I-IV-V progression template).</summary>
	TwelveBarBlues,

	/// <summary>Verse-chorus form.</summary>
	VerseChorus,

	/// <summary>Binary form (AB).</summary>
	Binary,

	/// <summary>Ternary form (ABA).</summary>
	Ternary,

	/// <summary>Rondo form (ABACA / ABACABA).</summary>
	Rondo,

	/// <summary>Strophic form (a single repeated section).</summary>
	Strophic,

	/// <summary>Through-composed (no section repeats).</summary>
	ThroughComposed,

	/// <summary>An unrecognized pattern.</summary>
	Unknown,
}
