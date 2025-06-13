// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sound absorption coefficient quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record SoundAbsorption<T> : PhysicalQuantity<SoundAbsorption<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of soundabsorption [dimensionless].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.SoundAbsorption;

	/// <summary>
	/// Initializes a new instance of the <see cref="SoundAbsorption{T}"/> class.
	/// </summary>
	public SoundAbsorption() : base() { }

	/// <summary>
	/// Creates a new SoundAbsorption from a dimensionless coefficient (0-1).
	/// </summary>
	/// <param name="coefficient">The absorption coefficient (0-1).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static SoundAbsorption<T> FromCoefficient(T coefficient) => Create(coefficient);

	/// <summary>
	/// Creates a new SoundAbsorption from a percentage value.
	/// </summary>
	/// <param name="percentage">The absorption percentage (0-100).</param>
	/// <returns>A new SoundAbsorption instance.</returns>
	public static SoundAbsorption<T> FromPercentage(T percentage) => Create(percentage / T.CreateChecked(100));

	/// <summary>
	/// Calculates the reflection coefficient (1 - absorption).
	/// </summary>
	/// <returns>The reflection coefficient.</returns>
	public T ReflectionCoefficient() => T.CreateChecked(1) - Value;

	/// <summary>
	/// Converts to percentage.
	/// </summary>
	/// <returns>The absorption as a percentage.</returns>
	public T ToPercentage() => Value * T.CreateChecked(100);
}
