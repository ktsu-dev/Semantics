// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

/// <summary>
/// Represents a generic 4D spacetime force vector (four-force) with compile-time dimensional safety.
/// Base class for concrete force vector implementations.
/// </summary>
/// <typeparam name="T">The scalar type used in the vector components.</typeparam>
public abstract record Force4D<T>
	where T : struct, System.Numerics.INumber<T>
{
	/// <summary>Gets the physical dimension of force [M L T⁻²].</summary>
	public virtual PhysicalDimension Dimension => PhysicalDimensions.Force;
}
