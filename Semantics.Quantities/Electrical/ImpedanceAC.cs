// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents an AC impedance quantity with compile-time dimensional safety.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public sealed record ImpedanceAC<T> : PhysicalQuantity<ImpedanceAC<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of impedanceac [M L² T⁻³ I⁻²].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ImpedanceAC;

	/// <summary>
	/// Initializes a new instance of the <see cref="ImpedanceAC{T}"/> class.
	/// </summary>
	public ImpedanceAC() : base() { }

	/// <summary>
	/// Creates a new ImpedanceAC from a value in ohms.
	/// </summary>
	/// <param name="ohms">The value in ohms.</param>
	/// <returns>A new ImpedanceAC instance.</returns>
	public static ImpedanceAC<T> FromOhms(T ohms) => Create(ohms);

	/// <summary>
	/// Divides electric potential by electric current to create impedance.
	/// </summary>
	/// <param name="potential">The electric potential (AC voltage).</param>
	/// <param name="current">The electric current (AC current).</param>
	/// <returns>The resulting AC impedance.</returns>
	public static ImpedanceAC<T> Divide(ElectricPotential<T> potential, ElectricCurrent<T> current)
	{
		Ensure.NotNull(potential);
		Ensure.NotNull(current);
		return Create(potential.Value / current.Value);
	}

	/// <summary>
	/// Multiplies impedance by current to get potential.
	/// </summary>
	/// <param name="impedance">The AC impedance.</param>
	/// <param name="current">The electric current.</param>
	/// <returns>The resulting electric potential.</returns>
	public static ElectricPotential<T> Multiply(ImpedanceAC<T> impedance, ElectricCurrent<T> current)
	{
		Ensure.NotNull(impedance);
		Ensure.NotNull(current);
		return ElectricPotential<T>.Create(impedance.Value * current.Value);
	}
}
