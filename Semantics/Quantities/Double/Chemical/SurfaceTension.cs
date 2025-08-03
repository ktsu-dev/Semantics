// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a surface tension quantity with double precision.
/// </summary>
public sealed record SurfaceTension : Generic.SurfaceTension<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SurfaceTension"/> class.
	/// </summary>
	public SurfaceTension() : base() { }

	/// <summary>
	/// Creates a new SurfaceTension from a value in newtons per meter.
	/// </summary>
	/// <param name="newtonsPerMeter">The value in newtons per meter.</param>
	/// <returns>A new SurfaceTension instance.</returns>
	public static new SurfaceTension FromNewtonsPerMeter(double newtonsPerMeter) => new() { Value = newtonsPerMeter };
}
