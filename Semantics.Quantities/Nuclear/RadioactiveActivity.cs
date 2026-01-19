// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents radioactive activity with a specific unit of measurement.
/// Radioactive activity is the number of radioactive decays per unit time.
/// It is measured in becquerels (Bq) in the SI system.
/// </summary>
/// <typeparam name="T">The numeric type for the radioactive activity value.</typeparam>
public sealed record RadioactiveActivity<T> : PhysicalQuantity<RadioactiveActivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of radioactive activity [T⁻¹].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.RadioactiveActivity;

	/// <summary>Initializes a new instance of the <see cref="RadioactiveActivity{T}"/> class.</summary>
	public RadioactiveActivity() : base() { }

	/// <summary>Creates a new radioactive activity from a value in becquerels.</summary>
	/// <param name="becquerels">The radioactive activity in becquerels.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static RadioactiveActivity<T> FromBecquerels(T becquerels) => Create(becquerels);

	/// <summary>Creates a new radioactive activity from a value in curies.</summary>
	/// <param name="curies">The radioactive activity in curies.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static RadioactiveActivity<T> FromCuries(T curies)
	{
		T becquerels = curies * T.CreateChecked(3.7e10);
		return Create(becquerels);
	}

	/// <summary>Creates a new radioactive activity from a value in kilobecquerels.</summary>
	/// <param name="kilobecquerels">The radioactive activity in kilobecquerels.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static RadioactiveActivity<T> FromKilobecquerels(T kilobecquerels)
	{
		T becquerels = kilobecquerels * T.CreateChecked(1000);
		return Create(becquerels);
	}

	/// <summary>Creates a new radioactive activity from a value in megabecquerels.</summary>
	/// <param name="megabecquerels">The radioactive activity in megabecquerels.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static RadioactiveActivity<T> FromMegabecquerels(T megabecquerels)
	{
		T becquerels = megabecquerels * T.CreateChecked(1e6);
		return Create(becquerels);
	}

	/// <summary>Creates a new radioactive activity from a value in millicuries.</summary>
	/// <param name="millicuries">The radioactive activity in millicuries.</param>
	/// <returns>A new RadioactiveActivity instance.</returns>
	public static RadioactiveActivity<T> FromMillicuries(T millicuries)
	{
		T becquerels = millicuries * T.CreateChecked(3.7e7);
		return Create(becquerels);
	}

	/// <summary>Calculates the number of atoms from activity and decay constant.</summary>
	/// <param name="decayConstant">The decay constant (λ) in s⁻¹.</param>
	/// <returns>The number of radioactive atoms.</returns>
	/// <remarks>
	/// Uses the relationship: A = λN
	/// where A is activity, λ is decay constant, and N is number of atoms.
	/// </remarks>
	public T CalculateNumberOfAtoms(T decayConstant)
	{
		if (decayConstant == T.Zero)
		{
			throw new ArgumentException("Decay constant cannot be zero.", nameof(decayConstant));
		}

		T activity = In(Units.Becquerel);
		return activity / decayConstant;
	}

	/// <summary>Calculates the half-life from activity and initial activity.</summary>
	/// <param name="initialActivity">The initial activity.</param>
	/// <param name="timeElapsed">The time elapsed since initial measurement.</param>
	/// <returns>The half-life.</returns>
	/// <remarks>
	/// Uses the exponential decay law: A(t) = A₀ × e^(-λt)
	/// Solving for half-life: t₁/₂ = ln(2) / λ
	/// </remarks>
	public Time<T> CalculateHalfLife(RadioactiveActivity<T> initialActivity, Time<T> timeElapsed)
	{
		Ensure.NotNull(initialActivity);
		Ensure.NotNull(timeElapsed);

		T currentActivity = In(Units.Becquerel);
		T initialActivityValue = initialActivity.In(Units.Becquerel);
		T timeSeconds = timeElapsed.In(Units.Second);

		if (currentActivity >= initialActivityValue)
		{
			throw new ArgumentException("Current activity must be less than initial activity for decay calculation.");
		}

		T ratio = currentActivity / initialActivityValue;
		T decayConstant = -T.CreateChecked(Math.Log(double.CreateChecked(ratio))) / timeSeconds;
		T halfLife = T.CreateChecked(Math.Log(2.0)) / decayConstant;

		return Time<T>.Create(halfLife);
	}

	/// <summary>Calculates the activity after a given time period.</summary>
	/// <param name="halfLife">The half-life of the radioactive material.</param>
	/// <param name="timeElapsed">The time elapsed.</param>
	/// <returns>The activity after the specified time.</returns>
	/// <remarks>
	/// Uses the exponential decay law: A(t) = A₀ × (1/2)^(t/t₁/₂)
	/// where A(t) is activity at time t, A₀ is initial activity, and t₁/₂ is half-life.
	/// </remarks>
	public RadioactiveActivity<T> CalculateActivityAfterTime(Time<T> halfLife, Time<T> timeElapsed)
	{
		Ensure.NotNull(halfLife);
		Ensure.NotNull(timeElapsed);

		T currentActivity = In(Units.Becquerel);
		T halfLifeSeconds = halfLife.In(Units.Second);
		T timeSeconds = timeElapsed.In(Units.Second);

		T exponent = timeSeconds / halfLifeSeconds;
		T decayFactor = T.CreateChecked(Math.Pow(0.5, double.CreateChecked(exponent)));
		T futureActivity = currentActivity * decayFactor;

		return Create(futureActivity);
	}
}
