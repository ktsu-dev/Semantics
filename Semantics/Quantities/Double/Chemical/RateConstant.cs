// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a rate constant quantity with double precision.
/// </summary>
public sealed record RateConstant : Generic.RateConstant<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RateConstant"/> class.
	/// </summary>
	public RateConstant() : base() { }

	/// <summary>
	/// Creates a new RateConstant from a value in per second.
	/// </summary>
	/// <param name="perSecond">The value in per second.</param>
	/// <returns>A new RateConstant instance.</returns>
	public static new RateConstant FromPerSecond(double perSecond) => new() { Value = perSecond };
}
