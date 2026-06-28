// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A timed musical event (a sounding note or a rest) with a rhythmic duration.</summary>
public interface IMusicalEvent
{
	/// <summary>Gets the rhythmic duration of the event.</summary>
	public Duration Duration { get; }
}
