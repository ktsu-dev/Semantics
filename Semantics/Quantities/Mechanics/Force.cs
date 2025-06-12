// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a force quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Force<T> : PhysicalQuantity<Force<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Force;

	/// <inheritdoc/>
	public override IUnit BaseUnit => Units.Newton;

	/// <summary>
	/// Initializes a new instance of the Force class.
	/// </summary>
	public Force() : base() { }

	/// <summary>
	/// Creates a new Force from a value in newtons.
	/// </summary>
	/// <param name="newtons">The value in newtons.</param>
	/// <returns>A new Force instance.</returns>
	public static Force<T> FromNewtons(T newtons) => Create(newtons);
}
