// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class SectionTests
{
	[TestMethod]
	public void Section_ExposesTypeProgressionAndBars()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"), "Verse 1");
		Assert.AreEqual(SectionType.Verse, verse.Type);
		Assert.AreEqual("Verse 1", verse.Label);
		Assert.AreEqual(4.0, verse.Bars, 1e-9);
	}

	[TestMethod]
	public void IsSameStructure_IgnoresLabelAndKey()
	{
		Section a = Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"), "Verse 1");
		Section b = Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"), "Verse 2");
		Assert.IsTrue(a.IsSameStructure(b));
	}

	[TestMethod]
	public void IsSameStructure_FalseForDifferentTypeOrChords()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"));
		Section chorus = Section.Create(SectionType.Chorus, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"));
		Section other = Section.Create(SectionType.Verse, Progression.Parse("4/4  F / / / | G / / / | C / / / | C / / /"));
		Assert.IsFalse(verse.IsSameStructure(chorus));
		Assert.IsFalse(verse.IsSameStructure(other));
	}

	[TestMethod]
	public void Section_RejectsNullProgression() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => Section.Create(SectionType.Verse, null!));
}
