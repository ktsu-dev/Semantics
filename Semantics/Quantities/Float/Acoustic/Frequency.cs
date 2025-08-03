// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a frequency quantity with float precision.
/// </summary>
public sealed record Frequency : Generic.Frequency<float>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Frequency"/> class.
	/// </summary>
	public Frequency() : base() { }

	/// <summary>
	/// Creates a new Frequency from a value in hertz.
	/// </summary>
	/// <param name="hertz">The value in hertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static new Frequency FromHertz(float hertz) => new() { Value = hertz };

	/// <summary>
	/// Creates a new Frequency from a value in kilohertz.
	/// </summary>
	/// <param name="kilohertz">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static new Frequency FromKilohertz(float kilohertz) => new() { Value = kilohertz * 1000f };

	/// <summary>
	/// Creates a new Frequency from a value in megahertz.
	/// </summary>
	/// <param name="megahertz">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static new Frequency FromMegahertz(float megahertz) => new() { Value = megahertz * 1000000f };
}
