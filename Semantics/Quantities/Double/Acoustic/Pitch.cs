// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a pitch quantity with double precision.
/// </summary>
public sealed record Pitch : Generic.Pitch<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Pitch"/> class.
	/// </summary>
	public Pitch() : base() { }

	/// <summary>
	/// Creates a new Pitch from a frequency value in Hz.
	/// </summary>
	/// <param name="hertz">The frequency in Hz.</param>
	/// <returns>A new Pitch instance.</returns>
	public static new Pitch FromHertz(double hertz) => new() { Quantity = hertz };

	/// <summary>
	/// Creates a new Pitch from a value in mels (perceptual pitch scale).
	/// </summary>
	/// <param name="mels">The pitch in mels.</param>
	/// <returns>A new Pitch instance.</returns>
	public static new Pitch FromMels(double mels) => new() { Quantity = mels };

	/// <summary>
	/// Creates a new Pitch from a value in barks (critical band scale).
	/// </summary>
	/// <param name="barks">The pitch in barks.</param>
	/// <returns>A new Pitch instance.</returns>
	public static new Pitch FromBarks(double barks) => new() { Quantity = barks };
}
