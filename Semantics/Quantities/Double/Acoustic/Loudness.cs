// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a loudness quantity with double precision.
/// </summary>
public sealed record Loudness
{
	/// <summary>Gets the underlying generic loudness instance.</summary>
	public Generic.Loudness<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Loudness"/> class.
	/// </summary>
	public Loudness() { }

	/// <summary>
	/// Creates a new Loudness from a value in sones.
	/// </summary>
	/// <param name="sones">The loudness in sones.</param>
	/// <returns>A new Loudness instance.</returns>
	public static Loudness FromSones(double sones) => new() { Value = Generic.Loudness<double>.FromSones(sones) };

	/// <summary>
	/// Creates a new Loudness from a value in phons (loudness level).
	/// </summary>
	/// <param name="phons">The loudness level in phons.</param>
	/// <returns>A new Loudness instance.</returns>
	public static Loudness FromPhons(double phons) => new() { Value = Generic.Loudness<double>.FromPhons(phons) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
