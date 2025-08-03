// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents an AC impedance quantity with float precision.
/// </summary>
public sealed record ImpedanceAC
{
	/// <summary>Gets the underlying generic AC impedance instance.</summary>
	public Generic.ImpedanceAC<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ImpedanceAC"/> class.
	/// </summary>
	public ImpedanceAC() { }

	/// <summary>
	/// Creates a new ImpedanceAC from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ImpedanceAC instance.</returns>
	public static new ImpedanceAC FromOhms(float ohms) => new() { Value = Generic.ImpedanceAC<float>.FromOhms(ohms) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
