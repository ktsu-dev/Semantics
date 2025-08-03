// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

/// <summary>
/// Represents a generic 2D acceleration vector with compile-time dimensional safety.
/// Base class for concrete acceleration vector implementations.
/// </summary>
/// <typeparam name="T">The scalar type used in the vector components.</typeparam>
public abstract record Acceleration2D<T>
	where T : struct, System.Numerics.INumber<T>
{
	/// <summary>Gets the physical dimension of acceleration [L T⁻²].</summary>
	public virtual PhysicalDimension Dimension => PhysicalDimensions.Acceleration;
}
