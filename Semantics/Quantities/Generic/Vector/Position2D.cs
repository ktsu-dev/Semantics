// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

/// <summary>
/// Represents a generic 2D position vector with compile-time dimensional safety.
/// Base class for concrete position vector implementations.
/// </summary>
/// <typeparam name="T">The scalar type used in the vector components.</typeparam>
public abstract record Position2D<T>
	where T : struct, System.Numerics.INumber<T>
{
	/// <summary>Gets the physical dimension of position [L].</summary>
	public virtual PhysicalDimension Dimension => PhysicalDimensions.Length;
}