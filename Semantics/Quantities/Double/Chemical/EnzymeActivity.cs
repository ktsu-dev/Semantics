// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an enzyme activity quantity with double precision.
/// </summary>
public sealed record EnzymeActivity : Generic.EnzymeActivity<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EnzymeActivity"/> class.
	/// </summary>
	public EnzymeActivity() : base() { }

	/// <summary>
	/// Creates a new EnzymeActivity from a value in katal.
	/// </summary>
	/// <param name="katal">The value in katal.</param>
	/// <returns>A new EnzymeActivity instance.</returns>
	public static new EnzymeActivity FromKatal(double katal) => new() { Value = katal };
}
