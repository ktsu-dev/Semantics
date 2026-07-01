// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class FormTests
{
	private static Key CMajor => Key.Create(PitchClass.Create(0), Mode.Major);

	private static Section Section(SectionType type, string chords) =>
		ktsu.Semantics.Music.Section.Create(type, Progression.Parse(chords));

	[TestMethod]
	public void Of_ExtractsAABAPattern_AndNamesThirtyTwoBarForm()
	{
		Section a = Section(SectionType.Verse, "C | Am | F | G");
		Section b = Section(SectionType.Bridge, "F | G | Em | Am");
		Arrangement arrangement = Arrangement.Create(CMajor, [a, a, b, a]);
		Form form = arrangement.Form;
		Assert.AreEqual("AABA", form.Pattern);
		Assert.AreEqual(NamedForm.ThirtyTwoBarAABA, form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTernaryAndBinary()
	{
		Section a = Section(SectionType.Verse, "C | G");
		Section b = Section(SectionType.Chorus, "F | C");
		Assert.AreEqual(NamedForm.Ternary, Arrangement.Create(CMajor, [a, b, a]).Form.Name);
		Assert.AreEqual(NamedForm.Binary, Arrangement.Create(CMajor, [a, b]).Form.Name);
	}

	[TestMethod]
	public void Of_RecognizesTwelveBarBlues()
	{
		Section blues = Section(SectionType.Verse, "C7 | C7 | C7 | C7 | F7 | F7 | C7 | C7 | G7 | F7 | C7 | C7");
		Arrangement arrangement = Arrangement.Create(CMajor, [blues]);
		Assert.AreEqual(NamedForm.TwelveBarBlues, arrangement.Form.Name);
	}

	[TestMethod]
	public void FromPattern_AppliesLetterRecognition()
	{
		Form form = Form.FromPattern("ABACA");
		Assert.AreEqual(NamedForm.Rondo, form.Name);
		Assert.AreEqual(5, form.Letters.Count);
	}

	[TestMethod]
	public void FromPattern_RejectsNonLetters() =>
		_ = Assert.ThrowsExactly<FormatException>(() => Form.FromPattern("A1B"));
}
