// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a frequency quantity with double precision.
/// </summary>
public sealed record Frequency
{
	/// <summary>Gets the underlying generic frequency instance.</summary>
	public Generic.Frequency<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Frequency"/> class.
	/// </summary>
	public Frequency() { }

	/// <summary>
	/// Creates a new Frequency from a value in hertz.
	/// </summary>
	/// <param name="hertz">The value in hertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency FromHertz(double hertz) => new() { Value = Generic.Frequency<double>.FromHertz(hertz) };

	/// <summary>
	/// Creates a new Frequency from a value in kilohertz.
	/// </summary>
	/// <param name="kilohertz">The value in kilohertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency FromKilohertz(double kilohertz) => new() { Value = Generic.Frequency<double>.FromKilohertz(kilohertz) };

	/// <summary>
	/// Creates a new Frequency from a value in megahertz.
	/// </summary>
	/// <param name="megahertz">The value in megahertz.</param>
	/// <returns>A new Frequency instance.</returns>
	public static Frequency FromMegahertz(double megahertz) => new() { Value = Generic.Frequency<double>.FromMegahertz(megahertz) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
