// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents Reynolds number with a specific unit of measurement.
/// Reynolds number is a dimensionless quantity that characterizes fluid flow regimes.
/// It indicates whether flow is laminar or turbulent.
/// </summary>
/// <typeparam name="T">The numeric type for the Reynolds number value.</typeparam>
public sealed record ReynoldsNumber<T> : PhysicalQuantity<ReynoldsNumber<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of Reynolds number [1] (dimensionless).</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.ReynoldsNumber;

	/// <summary>Initializes a new instance of the <see cref="ReynoldsNumber{T}"/> class.</summary>
	public ReynoldsNumber() : base() { }

	/// <summary>Creates a new Reynolds number from a dimensionless value.</summary>
	/// <param name="value">The dimensionless Reynolds number value.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	public static ReynoldsNumber<T> FromValue(T value) => Create(value);

	/// <summary>Creates a new Reynolds number from velocity, characteristic length, and kinematic viscosity.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <param name="kinematicViscosity">The kinematic viscosity.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	/// <remarks>
	/// Uses the relationship: Re = (v × L) / ν
	/// where Re is Reynolds number, v is velocity, L is characteristic length, and ν is kinematic viscosity.
	/// </remarks>
	public static ReynoldsNumber<T> FromVelocityLengthAndKinematicViscosity(Velocity<T> velocity, Length<T> characteristicLength, KinematicViscosity<T> kinematicViscosity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(characteristicLength);
		ArgumentNullException.ThrowIfNull(kinematicViscosity);

		T v = velocity.In(Units.MetersPerSecond);
		T length = characteristicLength.In(Units.Meter);
		T nu = kinematicViscosity.In(Units.Meter);

		if (nu == T.Zero)
		{
			throw new ArgumentException("Kinematic viscosity cannot be zero.", nameof(kinematicViscosity));
		}

		T reynoldsNumber = v * length / nu;
		return Create(reynoldsNumber);
	}

	/// <summary>Creates a new Reynolds number from velocity, characteristic length, dynamic viscosity, and density.</summary>
	/// <param name="velocity">The flow velocity.</param>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <param name="dynamicViscosity">The dynamic viscosity.</param>
	/// <param name="density">The fluid density.</param>
	/// <returns>A new ReynoldsNumber instance.</returns>
	/// <remarks>
	/// Uses the relationship: Re = (ρ × v × L) / μ
	/// where Re is Reynolds number, ρ is density, v is velocity, L is characteristic length, and μ is dynamic viscosity.
	/// </remarks>
	public static ReynoldsNumber<T> FromVelocityLengthDynamicViscosityAndDensity(Velocity<T> velocity, Length<T> characteristicLength, DynamicViscosity<T> dynamicViscosity, Density<T> density)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(characteristicLength);
		ArgumentNullException.ThrowIfNull(dynamicViscosity);
		ArgumentNullException.ThrowIfNull(density);

		T v = velocity.In(Units.MetersPerSecond);
		T length = characteristicLength.In(Units.Meter);
		T mu = dynamicViscosity.In(Units.Pascal);
		T rho = density.In(Units.Kilogram);

		if (mu == T.Zero)
		{
			throw new ArgumentException("Dynamic viscosity cannot be zero.", nameof(dynamicViscosity));
		}

		T reynoldsNumber = rho * v * length / mu;
		return Create(reynoldsNumber);
	}

	/// <summary>Calculates the critical velocity for transition from laminar to turbulent flow.</summary>
	/// <param name="characteristicLength">The characteristic length.</param>
	/// <param name="kinematicViscosity">The kinematic viscosity.</param>
	/// <param name="criticalReynoldsNumber">The critical Reynolds number (default: 2300 for pipe flow).</param>
	/// <returns>The critical velocity.</returns>
	/// <remarks>
	/// Uses the relationship: v_critical = (Re_critical × ν) / L
	/// where v_critical is critical velocity, Re_critical is critical Reynolds number, ν is kinematic viscosity, and L is characteristic length.
	/// </remarks>
	public static Velocity<T> CalculateCriticalVelocity(Length<T> characteristicLength, KinematicViscosity<T> kinematicViscosity, T? criticalReynoldsNumber = null)
	{
		ArgumentNullException.ThrowIfNull(characteristicLength);
		ArgumentNullException.ThrowIfNull(kinematicViscosity);

		T reCritical = criticalReynoldsNumber ?? T.CreateChecked(2300); // Default for pipe flow
		T length = characteristicLength.In(Units.Meter);
		T nu = kinematicViscosity.In(Units.Meter);

		if (length == T.Zero)
		{
			throw new ArgumentException("Characteristic length cannot be zero.", nameof(characteristicLength));
		}

		T criticalVelocity = reCritical * nu / length;
		return Velocity<T>.Create(criticalVelocity);
	}

	/// <summary>Determines if the flow is laminar.</summary>
	/// <param name="laminarThreshold">The Reynolds number threshold for laminar flow (default: 2300).</param>
	/// <returns>True if the flow is laminar (Re &lt; threshold).</returns>
	public bool IsLaminarFlow(T? laminarThreshold = null)
	{
		T threshold = laminarThreshold ?? T.CreateChecked(2300);
		T re = In(Units.Radian);
		return re < threshold;
	}

	/// <summary>Determines if the flow is turbulent.</summary>
	/// <param name="turbulentThreshold">The Reynolds number threshold for turbulent flow (default: 4000).</param>
	/// <returns>True if the flow is turbulent (Re &gt; threshold).</returns>
	public bool IsTurbulentFlow(T? turbulentThreshold = null)
	{
		T threshold = turbulentThreshold ?? T.CreateChecked(4000);
		T re = In(Units.Radian);
		return re > threshold;
	}

	/// <summary>Determines if the flow is in the transition region.</summary>
	/// <param name="laminarThreshold">The laminar threshold (default: 2300).</param>
	/// <param name="turbulentThreshold">The turbulent threshold (default: 4000).</param>
	/// <returns>True if the flow is in the transition region.</returns>
	public bool IsTransitionFlow(T? laminarThreshold = null, T? turbulentThreshold = null)
	{
		T lowerThreshold = laminarThreshold ?? T.CreateChecked(2300);
		T upperThreshold = turbulentThreshold ?? T.CreateChecked(4000);
		T re = In(Units.Radian);
		return re >= lowerThreshold && re <= upperThreshold;
	}

	/// <summary>Gets the flow regime classification.</summary>
	/// <param name="laminarThreshold">The laminar threshold (default: 2300).</param>
	/// <param name="turbulentThreshold">The turbulent threshold (default: 4000).</param>
	/// <returns>A string describing the flow regime.</returns>
	public string GetFlowRegime(T? laminarThreshold = null, T? turbulentThreshold = null)
	{
		return IsLaminarFlow(laminarThreshold) ? "Laminar" : IsTurbulentFlow(turbulentThreshold) ? "Turbulent" : "Transitional";
	}

	/// <summary>Calculates the friction factor for pipe flow using appropriate correlations.</summary>
	/// <param name="relativeRoughness">The relative roughness (ε/D) for the pipe (default: 0 for smooth pipes).</param>
	/// <returns>The Darcy friction factor.</returns>
	/// <remarks>
	/// Uses different correlations based on flow regime:
	/// - Laminar: f = 64/Re
	/// - Turbulent smooth: Blasius equation f = 0.316/Re^0.25 (for Re &lt; 100,000)
	/// - Turbulent rough: Colebrook-White equation approximation
	/// </remarks>
	public T CalculateFrictionFactor(T? relativeRoughness = null)
	{
		T re = In(Units.Radian);
		T roughness = relativeRoughness ?? T.Zero;

		if (IsLaminarFlow())
		{
			// Laminar flow: f = 64/Re
			return T.CreateChecked(64) / re;
		}
		else if (IsTurbulentFlow())
		{
			if (roughness == T.Zero)
			{
				// Smooth pipe turbulent flow: Blasius equation
				if (re < T.CreateChecked(100000))
				{
					return T.CreateChecked(0.316) / T.CreateChecked(Math.Pow(double.CreateChecked(re), 0.25));
				}
				else
				{
					// Prandtl equation for smooth pipes
					T logRe = T.CreateChecked(Math.Log10(double.CreateChecked(re)));
					return T.One / T.CreateChecked(Math.Pow((2.0 * double.CreateChecked(logRe)) - 0.8, 2));
				}
			}
			else
			{
				// Rough pipe: simplified Colebrook-White approximation
				T term1 = roughness / T.CreateChecked(3.7);
				T term2 = T.CreateChecked(2.51) / re;
				T logTerm = T.CreateChecked(Math.Log10(double.CreateChecked(term1 + term2)));
				return T.CreateChecked(0.25) / (logTerm * logTerm);
			}
		}
		else
		{
			// Transition region: linear interpolation between laminar and turbulent
			T laminarF = T.CreateChecked(64) / re;
			T turbulentF = T.CreateChecked(0.316) / T.CreateChecked(Math.Pow(double.CreateChecked(re), 0.25));
			T factor = (re - T.CreateChecked(2300)) / T.CreateChecked(1700); // interpolation factor
			return laminarF + (factor * (turbulentF - laminarF));
		}
	}

	/// <summary>Determines if this Reynolds number is typical for flow around a sphere.</summary>
	/// <returns>True if the value is in the typical range for sphere flow (0.1 to 200,000).</returns>
	public bool IsTypicalSphereFlow()
	{
		T re = In(Units.Radian);
		T lowerBound = T.CreateChecked(0.1);
		T upperBound = T.CreateChecked(200000);
		return re >= lowerBound && re <= upperBound;
	}

	/// <summary>Determines if this Reynolds number is typical for pipe flow.</summary>
	/// <returns>True if the value is in the typical range for pipe flow (1 to 1,000,000).</returns>
	public bool IsTypicalPipeFlow()
	{
		T re = In(Units.Radian);
		T lowerBound = T.CreateChecked(1);
		T upperBound = T.CreateChecked(1000000);
		return re >= lowerBound && re <= upperBound;
	}

	/// <summary>
	/// Calculates Reynolds number from density, velocity, length, and dynamic viscosity (Re = ρvL/μ).
	/// </summary>
	/// <param name="density">The fluid density.</param>
	/// <param name="velocity">The characteristic velocity.</param>
	/// <param name="length">The characteristic length.</param>
	/// <param name="dynamicViscosity">The dynamic viscosity.</param>
	/// <returns>The resulting Reynolds number.</returns>
	public static ReynoldsNumber<T> FromFluidProperties(Density<T> density, Velocity<T> velocity, Length<T> length, DynamicViscosity<T> dynamicViscosity)
	{
		ArgumentNullException.ThrowIfNull(density);
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(length);
		ArgumentNullException.ThrowIfNull(dynamicViscosity);

		T reynoldsValue = density.Value * velocity.Value * length.Value / dynamicViscosity.Value;

		return Create(reynoldsValue);
	}

	/// <summary>
	/// Calculates Reynolds number from velocity, length, and kinematic viscosity (Re = vL/ν).
	/// </summary>
	/// <param name="velocity">The characteristic velocity.</param>
	/// <param name="length">The characteristic length.</param>
	/// <param name="kinematicViscosity">The kinematic viscosity.</param>
	/// <returns>The resulting Reynolds number.</returns>
	public static ReynoldsNumber<T> FromKinematicProperties(Velocity<T> velocity, Length<T> length, KinematicViscosity<T> kinematicViscosity)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(length);
		ArgumentNullException.ThrowIfNull(kinematicViscosity);

		T reynoldsValue = velocity.Value * length.Value / kinematicViscosity.Value;

		return Create(reynoldsValue);
	}

	/// <summary>
	/// Calculates Reynolds number for air flow at standard conditions.
	/// </summary>
	/// <param name="velocity">The air velocity.</param>
	/// <param name="length">The characteristic length.</param>
	/// <returns>The resulting Reynolds number for standard air.</returns>
	public static ReynoldsNumber<T> ForStandardAir(Velocity<T> velocity, Length<T> length)
	{
		ArgumentNullException.ThrowIfNull(velocity);
		ArgumentNullException.ThrowIfNull(length);

		// Standard air properties at 20°C, 1 atm
		T airDensity = PhysicalConstants.Generic.StandardAirDensity<T>();
		T airViscosity = T.CreateChecked(1.825e-5); // Pa·s for air at 20°C

		T reynoldsValue = airDensity * velocity.Value * length.Value / airViscosity;

		return Create(reynoldsValue);
	}

	/// <summary>
	/// Determines the flow regime based on Reynolds number for pipe flow.
	/// </summary>
	/// <returns>A description of the flow regime.</returns>
	public string GetPipeFlowRegime()
	{
		double re = double.CreateChecked(Value);
		return re switch
		{
			< 2300 => "Laminar",
			< 4000 => "Transitional",
			_ => "Turbulent"
		};
	}
}
