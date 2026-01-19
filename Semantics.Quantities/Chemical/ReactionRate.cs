// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents reaction rate with a specific unit of measurement.
/// Reaction rate measures the speed of a chemical reaction.
/// </summary>
/// <typeparam name="T">The numeric type for the reaction rate value.</typeparam>
public sealed record ReactionRate<T> : PhysicalQuantity<ReactionRate<T>, T>
	where T : struct, INumber<T>, IFloatingPoint<T>
{
	/// <summary>Gets the physical dimension of reaction rate [N L⁻³ T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ReactionRate;

	/// <summary>Initializes a new instance of the <see cref="ReactionRate{T}"/> class.</summary>
	public ReactionRate() : base() { }

	/// <summary>Calculates reaction rate from concentration change over time.</summary>
	/// <param name="concentrationChange">Change in concentration.</param>
	/// <param name="timeInterval">Time interval for the change.</param>
	/// <returns>The reaction rate.</returns>
	public static ReactionRate<T> FromConcentrationChange(Concentration<T> concentrationChange, Time<T> timeInterval)
	{
		Ensure.NotNull(concentrationChange);
		Ensure.NotNull(timeInterval);

		T deltaConcentration = concentrationChange.In(Units.Molar);
		T deltaTime = timeInterval.In(Units.Second);
		T rate = deltaConcentration / deltaTime;
		return Create(rate);
	}

	/// <summary>Calculates reaction rate using rate law: rate = k[A]^m[B]^n.</summary>
	/// <param name="rateConstant">Rate constant.</param>
	/// <param name="concentrationA">Concentration of reactant A.</param>
	/// <param name="orderA">Reaction order for A.</param>
	/// <param name="concentrationB">Concentration of reactant B.</param>
	/// <param name="orderB">Reaction order for B.</param>
	/// <returns>The reaction rate.</returns>
	public static ReactionRate<T> FromRateLaw(RateConstant<T> rateConstant,
		Concentration<T> concentrationA, T orderA,
		Concentration<T> concentrationB, T orderB)
	{
		Ensure.NotNull(rateConstant);
		Ensure.NotNull(concentrationA);
		Ensure.NotNull(concentrationB);

		T k = rateConstant.In(Units.PerSecond);
		T cA = concentrationA.In(Units.Molar);
		T cB = concentrationB.In(Units.Molar);

		T rateValue = k * T.CreateChecked(Math.Pow(double.CreateChecked(cA), double.CreateChecked(orderA))) * T.CreateChecked(Math.Pow(double.CreateChecked(cB), double.CreateChecked(orderB)));
		return Create(rateValue);
	}

	/// <summary>Calculates rate constant from initial rate and concentrations.</summary>
	/// <param name="concentrationA">Initial concentration of A.</param>
	/// <param name="concentrationB">Initial concentration of B.</param>
	/// <param name="orderA">Reaction order for A.</param>
	/// <param name="orderB">Reaction order for B.</param>
	/// <returns>The rate constant.</returns>
	public RateConstant<T> CalculateRateConstant(Concentration<T> concentrationA, Concentration<T> concentrationB,
		T orderA, T orderB)
	{
		Ensure.NotNull(concentrationA);
		Ensure.NotNull(concentrationB);

		T rate = In(Units.MolesPerSecond);
		T cA = concentrationA.In(Units.Molar);
		T cB = concentrationB.In(Units.Molar);

		T denominator = T.CreateChecked(Math.Pow(double.CreateChecked(cA), double.CreateChecked(orderA))) * T.CreateChecked(Math.Pow(double.CreateChecked(cB), double.CreateChecked(orderB)));
		T k = rate / denominator;
		return RateConstant<T>.Create(k);
	}

	/// <summary>Calculates half-life for first-order reaction.</summary>
	/// <param name="rateConstant">First-order rate constant.</param>
	/// <returns>The half-life time.</returns>
	public static Time<T> CalculateHalfLife(RateConstant<T> rateConstant)
	{
		Ensure.NotNull(rateConstant);

		T k = rateConstant.In(Units.PerSecond);
		T ln2 = PhysicalConstants.Generic.Ln2<T>();
		T halfLife = ln2 / k;
		return Time<T>.Create(halfLife);
	}
}
