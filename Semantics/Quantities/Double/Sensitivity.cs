// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sensitivity quantity with double precision.
/// </summary>
public sealed record Sensitivity
{
	/// <summary>Gets the underlying generic sensitivity instance.</summary>
	public Generic.Sensitivity<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Sensitivity"/> class.
	/// </summary>
	public Sensitivity() { }

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/W).
	/// </summary>
	/// <param name="dbSplPerWatt">The sensitivity in dB SPL/W.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity FromDbSplPerWatt(double dbSplPerWatt) => new() { Value = Generic.Sensitivity<double>.FromDbSplPerWatt(dbSplPerWatt) };

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/V).
	/// </summary>
	/// <param name="dbSplPerVolt">The sensitivity in dB SPL/V.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity FromDbSplPerVolt(double dbSplPerVolt) => new() { Value = Generic.Sensitivity<double>.FromDbSplPerVolt(dbSplPerVolt) };

	/// <summary>
	/// Creates a new Sensitivity from a value in mV/Pa (microphone sensitivity).
	/// </summary>
	/// <param name="mvPerPa">The sensitivity in mV/Pa.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity FromMvPerPa(double mvPerPa) => new() { Value = Generic.Sensitivity<double>.FromMvPerPa(mvPerPa) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
