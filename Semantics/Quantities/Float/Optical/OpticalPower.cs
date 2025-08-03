// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an optical power quantity with float precision.
/// </summary>
public sealed record OpticalPower
{
	/// <summary>Gets the underlying generic optical power instance.</summary>
	public Generic.OpticalPower<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="OpticalPower"/> class.
	/// </summary>
	public OpticalPower() { }

	/// <summary>
	/// Creates a new OpticalPower from a value in diopters.
	/// </summary>
	/// <param name="diopters">The value in diopters.</param>
	/// <returns>A new OpticalPower instance.</returns>
	public static OpticalPower FromDiopters(float diopters) => new() { Value = Generic.OpticalPower<float>.FromDiopters(diopters) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
