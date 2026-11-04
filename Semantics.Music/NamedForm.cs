// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>A recognized named musical form.</summary>
public enum NamedForm
{
	/// <summary>32-bar AABA song form: the AABA section pattern, with every section eight bars long when the sections are known.</summary>
	ThirtyTwoBarAABA,

	/// <summary>12-bar blues (recognized by a section's I-IV-V progression template).</summary>
	TwelveBarBlues,

	/// <summary>Verse-chorus form: at least two verses and two choruses, each verse followed by a chorus (recognized from section types).</summary>
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
