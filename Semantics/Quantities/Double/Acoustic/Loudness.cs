// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a loudness quantity with double precision.
/// </summary>
public sealed record Loudness : Generic.Loudness<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Loudness"/> class.
	/// </summary>
	public Loudness() : base() { }

	/// <summary>
	/// Creates a new Loudness from a value in sones.
	/// </summary>
	/// <param name="sones">The loudness in sones.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromSones(double sones) => new() { Quantity = sones };

	/// <summary>
	/// Creates a new Loudness from a value in phons (loudness level).
	/// </summary>
	/// <param name="phons">The loudness level in phons.</param>
	/// <returns>A new Loudness instance.</returns>
	public static new Loudness FromPhons(double phons) => new() { Quantity = phons };
}
