// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Music;

/// <summary>A harmonic cadence type, classified by the scale-degree motion into the final chord.</summary>
public enum Cadence
{
	/// <summary>Authentic cadence: V to I.</summary>
	Authentic,

	/// <summary>Plagal cadence: IV to I.</summary>
	Plagal,

	/// <summary>Half cadence: any chord arriving on V.</summary>
	Half,

	/// <summary>Deceptive cadence: V to vi.</summary>
	Deceptive,
}
