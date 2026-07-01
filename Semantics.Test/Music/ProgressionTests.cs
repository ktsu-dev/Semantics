// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ProgressionTests
{
	[TestMethod]
	public void ChordEvent_BundlesChordAndDuration_AndIsMusicalEvent()
	{
		ChordEvent chordEvent = ChordEvent.Create(Chord.Parse("Cmaj7"), Duration.Half);
		Assert.AreEqual(0, chordEvent.Chord.Root.Value);
		Assert.AreEqual(Duration.Half, chordEvent.Duration);
		static Duration DurationOf(IMusicalEvent musicalEvent) => musicalEvent.Duration;
		Assert.AreEqual(Duration.Half, DurationOf(chordEvent));
	}

	[TestMethod]
	public void ChordEvent_RejectsNullChord() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => ChordEvent.Create(null!, Duration.Half));

	[TestMethod]
	public void Progression_TotalBars_SumsHarmonicRhythmAgainstTimeSignature()
	{
		Progression progression = Progression.Create(
		[
			ChordEvent.Create(Chord.Parse("C"), Duration.Whole),
			ChordEvent.Create(Chord.Parse("G"), Duration.Whole),
		]);
		Assert.AreEqual(2.0, progression.TotalBars, 1e-9);
		Assert.AreEqual(2, progression.Chords.Count);
	}

	[TestMethod]
	public void Progression_Create_FromChordsWithUniformRhythm()
	{
		Progression progression = Progression.Create(
			[Chord.Parse("Dm7"), Chord.Parse("G7"), Chord.Parse("Cmaj7")], Duration.Whole);
		Assert.AreEqual(3.0, progression.TotalBars, 1e-9);
	}

	[TestMethod]
	public void Progression_RejectsEmpty()
	{
		ChordEvent[] empty = [];
		_ = Assert.ThrowsExactly<ArgumentException>(() => Progression.Create(empty));
	}
}
