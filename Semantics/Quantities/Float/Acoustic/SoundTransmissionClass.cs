// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a sound transmission class quantity with float precision.
/// </summary>
public sealed record SoundTransmissionClass : Generic.SoundTransmissionClass<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SoundTransmissionClass"/> class.
	/// </summary>
	public SoundTransmissionClass() : base() { }

	/// <summary>
	/// Creates a new SoundTransmissionClass from a rating value.
	/// </summary>
	/// <param name="rating">The STC rating (typically 15-65).</param>
	/// <returns>A new SoundTransmissionClass instance.</returns>
	public static new SoundTransmissionClass FromRating(float rating) => new() { Quantity = rating };
}
