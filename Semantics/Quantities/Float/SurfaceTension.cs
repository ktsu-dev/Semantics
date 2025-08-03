// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a surface tension quantity with float precision.
/// </summary>
public sealed record SurfaceTension
{
	/// <summary>Gets the underlying generic surface tension instance.</summary>
	public Generic.SurfaceTension<float> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="SurfaceTension"/> class.
	/// </summary>
	public SurfaceTension() { }

	/// <summary>
	/// Creates a new SurfaceTension from a value in newtons per meter.
	/// </summary>
	/// <param name="newtonsPerMeter">The value in newtons per meter.</param>
	/// <returns>A new SurfaceTension instance.</returns>
	public static new SurfaceTension FromNewtonsPerMeter(float newtonsPerMeter) => new() { Value = Generic.SurfaceTension<float>.Create(newtonsPerMeter) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
