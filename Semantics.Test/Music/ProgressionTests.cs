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
#pragma warning disable CA1859 // Intentionally testing interface contract via IMusicalEvent reference
		IMusicalEvent asEvent = chordEvent;
#pragma warning restore CA1859
		Assert.AreEqual(Duration.Half, asEvent.Duration);
	}

	[TestMethod]
	public void ChordEvent_RejectsNullChord() =>
		_ = Assert.ThrowsExactly<ArgumentNullException>(() => ChordEvent.Create(null!, Duration.Half));
}
