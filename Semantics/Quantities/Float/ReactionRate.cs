// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a reaction rate quantity with float precision.
/// </summary>
public sealed record ReactionRate
{
	/// <summary>Gets the underlying generic reaction rate instance.</summary>
	public Generic.ReactionRate<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ReactionRate"/> class.
	/// </summary>
	public ReactionRate() { }

	/// <summary>
	/// Creates a new ReactionRate from a value in molar per second.
	/// </summary>
	/// <param name="molarPerSecond">The value in molar per second.</param>
	/// <returns>A new ReactionRate instance.</returns>
	public static new ReactionRate FromMolarPerSecond(float molarPerSecond) => new() { Value = Generic.ReactionRate<float>.Create(molarPerSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
