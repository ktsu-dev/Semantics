// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a reaction rate quantity with double precision.
/// </summary>
public sealed record ReactionRate
{
	/// <summary>Gets the underlying generic reaction rate instance.</summary>
	public Generic.ReactionRate<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReactionRate"/> class.
	/// </summary>
	public ReactionRate() { }

	/// <summary>
	/// Creates a new ReactionRate from a value in molar per second.
	/// </summary>
	/// <param name="molarPerSecond">The value in molar per second.</param>
	/// <returns>A new ReactionRate instance.</returns>
	public static ReactionRate FromMolarPerSecond(double molarPerSecond) => new() { Value = Generic.ReactionRate<double>.Create(molarPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
