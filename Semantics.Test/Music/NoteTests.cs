// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test.Music;

using ktsu.Semantics.Music;

[TestClass]
public class NoteTests
{
	[TestMethod]
	public void ToStringEncodesPitchDurationVelocity()
	{
		Note n = Note.Create(Pitch.Parse("C4"), Duration.Quarter, Velocity.Create(80));
		Assert.AreEqual("C4:1/4:v80", n.ToString());
	}

	[TestMethod]
	public void RoundTrip()
	{
		Note n = Note.Create(Pitch.Parse("F#3"), Duration.Eighth, Velocity.Create(100));
		Assert.AreEqual(n, Note.Parse(n.ToString()));
	}

	[TestMethod]
	public void TryParseFailsOnMissingVelocityMarker()
	{
		Assert.IsFalse(Note.TryParse("C4:1/4:80", out Note? result));
		Assert.IsNull(result);
	}
}
