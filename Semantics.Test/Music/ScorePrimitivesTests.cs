// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class ScorePrimitivesTests
{
	[TestMethod]
	public void Velocity_RejectsOutOfRange()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Velocity.Create(128));
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Velocity.Create(-1));
	}

	[TestMethod]
	public void Velocity_NamedDynamicsAreOrdered()
	{
		Assert.IsTrue(Velocity.Pianissimo.Value < Velocity.MezzoForte.Value);
		Assert.IsTrue(Velocity.MezzoForte.Value < Velocity.Fortissimo.Value);
	}

	[TestMethod]
	public void Tempo_ConvertsDurationToSeconds()
	{
		Tempo allegro = Tempo.Create(120); // quarter-note beat
		Assert.AreEqual(0.5, allegro.SecondsPerBeat, 1e-9);
		Assert.AreEqual(0.5, allegro.Seconds(Duration.Quarter), 1e-9);
		Assert.AreEqual(2.0, allegro.Seconds(Duration.Whole), 1e-9);
	}

	[TestMethod]
	public void Tempo_RespectsExplicitBeatUnit()
	{
		// 120 dotted-quarter beats per minute (compound time): a dotted quarter is one beat.
		Tempo compound = Tempo.Create(120, Duration.Quarter.Dotted());
		Assert.AreEqual(0.5, compound.Seconds(Duration.Quarter.Dotted()), 1e-9);
	}

	[TestMethod]
	public void Tempo_RejectsNonPositive()
	{
		_ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Tempo.Create(0));
	}

	[TestMethod]
	public void Note_BundlesPitchDurationVelocity_AndTimesInSeconds()
	{
		Note note = Note.Create(Pitch.Parse("A4"), Duration.Half, Velocity.Forte);
		Assert.AreEqual(69, note.Pitch.Midi);
		Assert.AreEqual(Duration.Half, note.Duration);
		Assert.AreEqual(96, note.Velocity.Value);
		Assert.AreEqual(1.0, note.Seconds(Tempo.Create(120)), 1e-9);
	}

	[TestMethod]
	public void Rest_TimesInSeconds()
	{
		Rest rest = Rest.Create(Duration.Eighth);
		Assert.AreEqual(0.25, rest.Seconds(Tempo.Create(120)), 1e-9);
	}

	[TestMethod]
	public void NoteAndRest_AreMusicalEvents()
	{
		static Duration DurationOf(IMusicalEvent musicalEvent) => musicalEvent.Duration;
		Assert.AreEqual(Duration.Half, DurationOf(Note.Create(Pitch.Create(60), Duration.Half, Velocity.Forte)));
		Assert.AreEqual(Duration.Eighth, DurationOf(Rest.Create(Duration.Eighth)));
	}
}
