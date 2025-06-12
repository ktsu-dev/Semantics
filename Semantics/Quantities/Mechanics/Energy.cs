// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an energy quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record Energy<T> : PhysicalQuantity<Energy<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.Energy;

	/// <summary>
	/// Initializes a new instance of the Energy class.
	/// </summary>
	public Energy() : base() { }

	/// <summary>
	/// Creates a new Energy from a value in joules.
	/// </summary>
	/// <param name="joules">The value in joules.</param>
	/// <returns>A new Energy instance.</returns>
	public static Energy<T> FromJoules(T joules) => Create(joules);
}
