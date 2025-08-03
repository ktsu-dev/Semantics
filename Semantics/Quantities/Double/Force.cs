// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a force quantity with double precision.
/// </summary>
public sealed record Force
{
	/// <summary>Gets the underlying generic force instance.</summary>
	public Generic.Force<double> Value { get; init; } = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="Force"/> class.
	/// </summary>
	public Force() { }

	/// <summary>
	/// Creates a new Force from a value in newtons.
	/// </summary>
	/// <param name="newtons">The value in newtons.</param>
	/// <returns>A new Force instance.</returns>
	public static Force FromNewtons(double newtons) => new() { Value = Generic.Force<double>.FromNewtons(newtons) };

	/// <summary>Returns a string representation of this quantity.</summary>
	public override string ToString() => Value.ToString();
}
