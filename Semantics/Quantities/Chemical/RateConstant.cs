// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a rate constant with a specific unit of measurement.
/// Rate constant determines the speed of a chemical reaction.
/// </summary>
/// <typeparam name="T">The numeric type for the rate constant value.</typeparam>
public sealed record RateConstant<T> : PhysicalQuantity<RateConstant<T>, T>
	where T : struct, INumber<T>, IFloatingPoint<T>
{
	/// <summary>Gets the physical dimension of rate constant [T⁻¹] for first-order reactions.</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.RateConstant;

	/// <summary>Initializes a new instance of the <see cref="RateConstant{T}"/> class.</summary>
	public RateConstant() : base() { }

	/// <summary>Calculates rate constant from Arrhenius equation: k = A * e^(-Ea/RT).</summary>
	/// <param name="preExponentialFactor">Pre-exponential factor A.</param>
	/// <param name="activationEnergy">Activation energy.</param>
	/// <param name="temperature">Temperature.</param>
	/// <returns>The rate constant.</returns>
	public static RateConstant<T> FromArrheniusEquation(T preExponentialFactor,
		ActivationEnergy<T> activationEnergy, Temperature<T> temperature)
	{
		ArgumentNullException.ThrowIfNull(activationEnergy);
		ArgumentNullException.ThrowIfNull(temperature);

		T ea = activationEnergy.In(Units.JoulesPerMole);
		T temp = temperature.In(Units.Kelvin);
		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();

		T exponent = -ea / (gasConstant * temp);
		T k = preExponentialFactor * T.CreateChecked(Math.Exp(double.CreateChecked(exponent)));
		return Create(k);
	}

	/// <summary>Calculates temperature dependence ratio: k2/k1 = exp((Ea/R) * (1/T1 - 1/T2)).</summary>
	/// <param name="temperature1">Initial temperature.</param>
	/// <param name="temperature2">Final temperature.</param>
	/// <param name="activationEnergy">Activation energy.</param>
	/// <returns>The rate constant at temperature2.</returns>
	public RateConstant<T> AtTemperature(Temperature<T> temperature1, Temperature<T> temperature2,
		ActivationEnergy<T> activationEnergy)
	{
		ArgumentNullException.ThrowIfNull(temperature1);
		ArgumentNullException.ThrowIfNull(temperature2);
		ArgumentNullException.ThrowIfNull(activationEnergy);

		T k1 = In(Units.PerSecond);
		T t1 = temperature1.In(Units.Kelvin);
		T t2 = temperature2.In(Units.Kelvin);
		T ea = activationEnergy.In(Units.JoulesPerMole);
		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();

		T exponent = ea / gasConstant * ((T.One / t1) - (T.One / t2));
		T ratio = T.CreateChecked(Math.Exp(double.CreateChecked(exponent)));
		T k2 = k1 * ratio;
		return Create(k2);
	}

	/// <summary>Common rate constant values for reference reactions.</summary>
	public static class CommonValues
	{
		/// <summary>Typical enzyme turnover (kcat): ~1000 s⁻¹.</summary>
		public static RateConstant<T> TypicalEnzyme => Create(T.CreateChecked(1000));

		/// <summary>Fast protein folding: ~10⁶ s⁻¹.</summary>
		public static RateConstant<T> FastProteinFolding => Create(T.CreateChecked(1e6));

		/// <summary>DNA replication: ~1000 s⁻¹.</summary>
		public static RateConstant<T> DNAReplication => Create(T.CreateChecked(1000));

		/// <summary>Slow metabolic process: ~0.01 s⁻¹.</summary>
		public static RateConstant<T> SlowMetabolic => Create(T.CreateChecked(0.01));
	}
}
