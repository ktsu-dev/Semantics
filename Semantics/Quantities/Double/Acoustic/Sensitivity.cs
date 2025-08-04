// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a sensitivity quantity with double precision.
/// </summary>
public sealed record Sensitivity : Generic.Sensitivity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Sensitivity"/> class.
	/// </summary>
	public Sensitivity() : base() { }

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/W).
	/// </summary>
	/// <param name="dbSplPerWatt">The sensitivity in dB SPL/W.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static new Sensitivity FromDbSplPerWatt(double dbSplPerWatt) => new() { Quantity = dbSplPerWatt };

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/V).
	/// </summary>
	/// <param name="dbSplPerVolt">The sensitivity in dB SPL/V.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static new Sensitivity FromDbSplPerVolt(double dbSplPerVolt) => new() { Quantity = dbSplPerVolt };

	/// <summary>
	/// Creates a new Sensitivity from a value in mV/Pa (microphone sensitivity).
	/// </summary>
	/// <param name="mvPerPa">The sensitivity in mV/Pa.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static new Sensitivity FromMvPerPa(double mvPerPa) => new() { Quantity = mvPerPa };
}
