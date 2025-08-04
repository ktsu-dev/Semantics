// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a reaction rate quantity with double precision.
/// </summary>
public sealed record ReactionRate : Generic.ReactionRate<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReactionRate"/> class.
	/// </summary>
	public ReactionRate() : base() { }

	/// <summary>
	/// Creates a new ReactionRate from a value in molar per second.
	/// </summary>
	/// <param name="molarPerSecond">The value in molar per second.</param>
	/// <returns>A new ReactionRate instance.</returns>
	public static ReactionRate FromMolarPerSecond(double molarPerSecond) => new() { Value = molarPerSecond };
}
