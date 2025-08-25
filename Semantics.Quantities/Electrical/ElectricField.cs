// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric field quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricField<T> : PhysicalQuantity<ElectricField<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricfield [M L T⁻³ I⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricField;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricField{T}"/> class.
	/// </summary>
	public ElectricField() : base() { }

	/// <summary>
	/// Creates a new ElectricField from a value in volts per meter.
	/// </summary>
	/// <param name="voltsPerMeter">The value in volts per meter.</param>
	/// <returns>A new ElectricField instance.</returns>
	public static ElectricField<T> FromVoltsPerMeter(T voltsPerMeter) => Create(voltsPerMeter);

	/// <summary>
	/// Divides electric potential by length to create electric field.
	/// </summary>
	/// <param name="potential">The electric potential.</param>
	/// <param name="length">The length.</param>
	/// <returns>The resulting electric field.</returns>
	public static ElectricField<T> Divide(ElectricPotential<T> potential, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(potential);
		ArgumentNullException.ThrowIfNull(length);
		return Create(potential.Value / length.Value);
	}

	/// <summary>
	/// Multiplies electric field by length to get electric potential.
	/// </summary>
	/// <param name="field">The electric field.</param>
	/// <param name="length">The length.</param>
	/// <returns>The resulting electric potential.</returns>
	public static ElectricPotential<T> Multiply(ElectricField<T> field, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(field);
		ArgumentNullException.ThrowIfNull(length);
		return ElectricPotential<T>.Create(field.Value * length.Value);
	}
}
