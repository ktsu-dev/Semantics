// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Represents activation energy with a specific unit of measurement.
/// Activation energy is the minimum energy required for a reaction to occur.
/// </summary>
/// <typeparam name="T">The numeric type for the activation energy value.</typeparam>
public record ActivationEnergy<T> : PhysicalQuantity<ActivationEnergy<T>, T>
	where T : struct, INumber<T>, IFloatingPoint<T>
{
	/// <summary>Gets the physical dimension of activation energy [M L² T⁻² N⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ActivationEnergy;

	/// <summary>Initializes a new instance of the <see cref="ActivationEnergy{T}"/> class.</summary>
	public ActivationEnergy() : base() { }

	/// <summary>Calculates activation energy from rate constants at two temperatures using Arrhenius equation.</summary>
	/// <param name="rateConstant1">Rate constant at temperature 1.</param>
	/// <param name="rateConstant2">Rate constant at temperature 2.</param>
	/// <param name="temperature1">Temperature 1.</param>
	/// <param name="temperature2">Temperature 2.</param>
	/// <returns>The activation energy.</returns>
	public static ActivationEnergy<T> FromArrheniusPlot(RateConstant<T> rateConstant1, RateConstant<T> rateConstant2,
		Temperature<T> temperature1, Temperature<T> temperature2)
	{
		ArgumentNullException.ThrowIfNull(rateConstant1);
		ArgumentNullException.ThrowIfNull(rateConstant2);
		ArgumentNullException.ThrowIfNull(temperature1);
		ArgumentNullException.ThrowIfNull(temperature2);

		T k1 = rateConstant1.In(Units.PerSecond);
		T k2 = rateConstant2.In(Units.PerSecond);
		T t1 = temperature1.In(Units.Kelvin);
		T t2 = temperature2.In(Units.Kelvin);
		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();

		T lnRatio = T.CreateChecked(Math.Log(double.CreateChecked(k2 / k1)));
		T tempDifference = (T.One / t1) - (T.One / t2);
		T ea = -gasConstant * lnRatio / tempDifference;
		return Create(ea);
	}

	/// <summary>Common activation energies for various processes.</summary>
	public static class CommonValues
	{
		/// <summary>Water self-ionization: ~55 kJ/mol.</summary>
		public static ActivationEnergy<T> WaterSelfIonization => Create(T.CreateChecked(55000));

		/// <summary>DNA denaturation: ~150 kJ/mol.</summary>
		public static ActivationEnergy<T> DNADenaturation => Create(T.CreateChecked(150000));

		/// <summary>Protein denaturation: ~200 kJ/mol.</summary>
		public static ActivationEnergy<T> ProteinDenaturation => Create(T.CreateChecked(200000));

		/// <summary>Ester hydrolysis: ~70 kJ/mol.</summary>
		public static ActivationEnergy<T> EsterHydrolysis => Create(T.CreateChecked(70000));

		/// <summary>Simple nucleophilic substitution: ~80 kJ/mol.</summary>
		public static ActivationEnergy<T> NucleophilicSubstitution => Create(T.CreateChecked(80000));

		/// <summary>Enzyme-catalyzed reaction: ~50 kJ/mol (lowered from uncatalyzed).</summary>
		public static ActivationEnergy<T> EnzymeCatalyzed => Create(T.CreateChecked(50000));
	}

	/// <summary>Calculates the ratio of rate constants at two temperatures.</summary>
	/// <param name="temperature1">Initial temperature.</param>
	/// <param name="temperature2">Final temperature.</param>
	/// <returns>The ratio k2/k1.</returns>
	public T CalculateRateRatio(Temperature<T> temperature1, Temperature<T> temperature2)
	{
		ArgumentNullException.ThrowIfNull(temperature1);
		ArgumentNullException.ThrowIfNull(temperature2);

		T ea = In(Units.JoulesPerMole);
		T t1 = temperature1.In(Units.Kelvin);
		T t2 = temperature2.In(Units.Kelvin);
		T gasConstant = PhysicalConstants.Generic.GasConstant<T>();

		T exponent = ea / gasConstant * ((T.One / t1) - (T.One / t2));
		return T.CreateChecked(Math.Exp(double.CreateChecked(exponent)));
	}
}
