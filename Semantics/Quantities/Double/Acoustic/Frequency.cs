// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a frequency quantity with double precision.
/// </summary>
public sealed record Frequency : Generic.Frequency<double>
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
	public static new Frequency FromHertz(double hertz) => new() { Quantity = hertz };

	/// <summary>
	/// Creates a new Frequency from a value in kilohertz.
	/// </summary>
	/// <param name="kilohertz">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static new Frequency FromKilohertz(double kilohertz) => new() { Quantity = kilohertz };

	/// <summary>
	/// Creates a new Frequency from a value in megahertz.
	/// </summary>
	/// <param name="megahertz">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static new Frequency FromMegahertz(double megahertz) => new() { Quantity = megahertz };
}
