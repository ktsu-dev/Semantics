// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an electric conductivity quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ElectricConductivity<T> : PhysicalQuantity<ElectricConductivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of electricconductivity [M⁻¹ L⁻³ T³ I²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ElectricConductivity;

	/// <summary>
	/// Initializes a new instance of the <see cref="ElectricConductivity{T}"/> class.
	/// </summary>
	public ElectricConductivity() : base() { }

	/// <summary>
	/// Creates a new ElectricConductivity from a value in siemens per meter.
	/// </summary>
	/// <param name="siemensPerMeter">The value in siemens per meter.</param>
	/// <returns>A new ElectricConductivity instance.</returns>
	public static ElectricConductivity<T> FromSiemensPerMeter(T siemensPerMeter) => Create(siemensPerMeter);

	/// <summary>
	/// Divides electric current density by electric field to get conductivity.
	/// </summary>
	/// <param name="currentDensity">The electric current density (amperes per square meter).</param>
	/// <param name="electricField">The electric field.</param>
	/// <returns>The resulting electric conductivity.</returns>
	public static ElectricConductivity<T> Divide(T currentDensity, ElectricField<T> electricField)
	{
		Ensure.NotNull(electricField);
		return Create(currentDensity / electricField.Value);
	}
}
