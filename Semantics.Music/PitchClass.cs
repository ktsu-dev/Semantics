// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Music;

/// <summary>
/// Represents one of the twelve pitch classes (C..B), octave-folded to 0..11.
/// </summary>
public sealed record PitchClass
{
	private static readonly string[] Names =
		["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];

	/// <summary>Gets the pitch class value, 0 (C) through 11 (B).</summary>
	public int Value { get; private init; }

	/// <summary>Gets the sharp-spelled name of this pitch class.</summary>
	public string Name => Names[Value];

	/// <summary>Creates a pitch class, folding any integer into the range 0..11.</summary>
	/// <param name="value">Any integer; reduced modulo 12.</param>
	/// <returns>A pitch class in 0..11.</returns>
	public static PitchClass Create(int value) => new() { Value = ((value % 12) + 12) % 12 };
}
