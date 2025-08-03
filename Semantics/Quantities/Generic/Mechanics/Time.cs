// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents a time quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public record Time<T> : PhysicalQuantity<Time<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of time [T].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Time;

	/// <summary>
	/// Initializes a new instance of the <see cref="Time{T}"/> class.
	/// </summary>
	public Time() : base() { }

	/// <summary>
	/// Creates a new Time from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new Time instance.</returns>
	public static Time<T> FromSeconds(T seconds) => Create(seconds);
}
