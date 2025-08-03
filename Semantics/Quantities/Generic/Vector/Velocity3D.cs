// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

/// <summary>
/// Represents a generic 3D velocity vector with compile-time dimensional safety.
/// Base class for concrete velocity vector implementations.
/// </summary>
/// <typeparam name="T">The scalar type used in the vector components.</typeparam>
public abstract record Velocity3D<T>
	where T : struct, System.Numerics.INumber<T>
{
	/// <summary>Gets the physical dimension of velocity [L T⁻¹].</summary>
	public virtual PhysicalDimension Dimension => PhysicalDimensions.Velocity;
}