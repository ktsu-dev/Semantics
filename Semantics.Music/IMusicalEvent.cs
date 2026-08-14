// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>A timed musical event (a sounding note or a rest) with a rhythmic duration.</summary>
public interface IMusicalEvent
{
	/// <summary>Gets the rhythmic duration of the event.</summary>
	public Duration Duration { get; }
}
