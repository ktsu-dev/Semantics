// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an angular acceleration quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record AngularAcceleration<T> : PhysicalQuantity<AngularAcceleration<T>, T>
	where T : struct, INumber<T>
{
	/// <inheritdoc/>
	public override PhysicalDimension Dimension => PhysicalDimensions.AngularAcceleration;

	/// <summary>
	/// Initializes a new instance of the AngularAcceleration class.
	/// </summary>
	public AngularAcceleration() : base() { }

	/// <summary>
	/// Creates a new AngularAcceleration from a value in radians per second squared.
	/// </summary>
	/// <param name="radiansPerSecondSquared">The value in radians per second squared.</param>
	/// <returns>A new AngularAcceleration instance.</returns>
	public static AngularAcceleration<T> FromRadiansPerSecondSquared(T radiansPerSecondSquared) => Create(radiansPerSecondSquared);
}
