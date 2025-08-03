// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a rate constant quantity with float precision.
/// </summary>
public sealed record RateConstant
{
	/// <summary>Gets the underlying generic rate constant instance.</summary>
	public Generic.RateConstant<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="RateConstant"/> class.
	/// </summary>
	public RateConstant() { }

	/// <summary>
	/// Creates a new RateConstant from a value in per second.
	/// </summary>
	/// <param name="perSecond">The value in per second.</param>
	/// <returns>A new RateConstant instance.</returns>
	public static new RateConstant FromPerSecond(float perSecond) => new() { Value = Generic.RateConstant<float>.Create(perSecond) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
