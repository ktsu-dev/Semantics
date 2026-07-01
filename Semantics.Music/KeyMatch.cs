// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>A candidate key paired with its diatonic-fit score for a progression.</summary>
public sealed record KeyMatch
{
	/// <summary>Gets the candidate key.</summary>
	public Key Key { get; private init; } = Key.Create(PitchClass.Create(0), Mode.Major);

	/// <summary>Gets the quality-weighted diatonic-fit score in 0..1 (1.0 = every chord is diatonic with matching triad quality).</summary>
	public double Score { get; private init; }

	/// <summary>Creates a key match.</summary>
	/// <param name="key">The candidate key.</param>
	/// <param name="score">The fit score.</param>
	/// <returns>A new key match.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
	public static KeyMatch Create(Key key, double score)
	{
		Ensure.NotNull(key);
		return new() { Key = key, Score = score };
	}
}
