// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ArrangementTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	[TestMethod]
	public void Arrangement_OrdersSectionsAndSumsBars()
	{
		Section verse = Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / / | Am / / / | F / / / | G / / /"));
		Section chorus = Section.Create(SectionType.Chorus, Progression.Parse("4/4  F / / / | G / / / | C / / / | C / / /"));
		Arrangement arrangement = Arrangement.Create(CMajor, [verse, chorus, verse]);
		Assert.HasCount(3, arrangement.Sections);
		Assert.AreEqual(12.0, arrangement.TotalBars, 1e-9);
		Assert.AreEqual(SectionType.Chorus, arrangement.Sections[1].Type);
	}

	[TestMethod]
	public void Arrangement_RejectsEmpty()
	{
		Section[] empty = [];
		_ = Assert.ThrowsExactly<ArgumentException>(() => Arrangement.Create(CMajor, empty));
	}

	[TestMethod]
	public void Arrangement_RejectsNullKey() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(
			() => Arrangement.Create(null!, [Section.Create(SectionType.Verse, Progression.Parse("4/4  C / / /"))]));
}
