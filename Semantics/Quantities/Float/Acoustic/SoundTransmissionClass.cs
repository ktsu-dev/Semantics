// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound transmission class quantity with float precision.
/// </summary>
public sealed record SoundTransmissionClass
{
	/// <summary>Gets the underlying generic sound transmission class instance.</summary>
	public Generic.SoundTransmissionClass<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundTransmissionClass"/> class.
	/// </summary>
	public SoundTransmissionClass() { }

	/// <summary>
	/// Creates a new SoundTransmissionClass from a rating value.
	/// </summary>
	/// <param name="rating">The STC rating (typically 15-65).</param>
	/// <returns>A new SoundTransmissionClass instance.</returns>
	public static SoundTransmissionClass FromRating(float rating) => new() { Value = Generic.SoundTransmissionClass<float>.FromRating(rating) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
