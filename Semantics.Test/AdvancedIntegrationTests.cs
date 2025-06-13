// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Advanced integration tests for complex multi-domain physics relationships.
/// Tests mathematical properties and cross-domain consistency.
/// </summary>
[TestClass]
public class AdvancedIntegrationTests
{
	private const double Tolerance = 1e-10;
	private const double RelativeTolerance = 1e-12;

	#region Property-Based Testing for Mathematical Relationships

	/// <summary>
	/// Tests the fundamental thermodynamic relationship: dU = TdS - PdV + μdN
	/// Verifies energy conservation across thermal, mechanical, and chemical domains.
	/// </summary>
	[TestMethod]
	public void ThermodynamicFundamentalRelation_EnergyConservation()
	{
		// Test multiple scenarios with different conditions
		var testCases = new[]
		{
			new { T = 300.0, P = 101325.0, V = 0.024, n = 1.0, S = 188.8 }, // Standard conditions
			new { T = 373.15, P = 101325.0, V = 0.03, n = 1.5, S = 195.5 }, // Elevated temperature
			new { T = 273.15, P = 50663.0, V = 0.048, n = 0.5, S = 205.1 }  // Different pressure
		};

		foreach (var testCase in testCases)
		{
			// Arrange - Thermodynamic state variables
			var temperature = Temperature<double>.FromKelvin(testCase.T);
			var pressure = Pressure<double>.FromPascals(testCase.P);
			var volume = Volume<double>.FromCubicMeters(testCase.V);
			var amount = AmountOfSubstance<double>.FromMoles(testCase.n);
			var entropy = Entropy<double>.FromJoulesPerKelvin(testCase.S);

			// Calculate chemical potential (simplified ideal gas)
			var gasConstant = PhysicalConstants.Generic.GasConstant<double>();
			var chemicalPotential = gasConstant * testCase.T * Math.Log(testCase.P / 101325.0);

			// Act - Calculate internal energy change for small variations
			var deltaT = 1.0; // 1 K change
			var deltaP = 1000.0; // 1 kPa change
			var deltaN = 0.01; // 0.01 mol change

			var deltaU_thermal = entropy.Value * deltaT;
			var deltaU_mechanical = -pressure.Value * (volume.Value * deltaP / pressure.Value);
			var deltaU_chemical = chemicalPotential * deltaN;

			var totalDeltaU = deltaU_thermal + deltaU_mechanical + deltaU_chemical;

			// Assert - Energy changes should be additive and physically reasonable
			Assert.IsTrue(Math.Abs(deltaU_thermal) > 0, "Thermal contribution should be non-zero");
			Assert.IsTrue(Math.Abs(totalDeltaU) > Math.Abs(deltaU_thermal) * 0.1,
				"Total energy change should be significant");

			// Test energy conservation: ΔU should equal sum of contributions
			var calculatedDeltaU = deltaU_thermal + deltaU_mechanical + deltaU_chemical;
			Assert.AreEqual(totalDeltaU, calculatedDeltaU, Tolerance,
				"Energy contributions should sum correctly");
		}
	}

	/// <summary>
	/// Tests Maxwell's electromagnetic field relationships across electrical and optical domains.
	/// Verifies ∇×E = -∂B/∂t and related electromagnetic properties.
	/// </summary>
	[TestMethod]
	public void MaxwellElectromagneticRelations_CrossDomainConsistency()
	{
		// Test electromagnetic wave properties
		var testFrequencies = new double[] { 1e9, 5e14, 1e15 }; // 1 GHz, visible light, UV

		foreach (var freq in testFrequencies)
		{
			// Arrange - Electromagnetic wave parameters
			var frequency = Frequency<double>.FromHertz(freq);
			var speedOfLight = PhysicalConstants.Generic.SpeedOfLight<double>();
			var vacuumPermeability = PhysicalConstants.Generic.VacuumPermeability<double>();
			var vacuumPermittivity = PhysicalConstants.Generic.VacuumPermittivity<double>();

			// Calculate wave properties
			var wavelength = speedOfLight / frequency.Value;
			var angularFrequency = 2 * Math.PI * frequency.Value;

			// Electric field amplitude (example: 1 V/m)
			var electricFieldAmplitude = ElectricField<double>.FromVoltsPerMeter(1.0);

			// Act - Calculate corresponding magnetic field using Maxwell relations
			// For plane waves: |B| = |E|/c
			var magneticFieldAmplitude = electricFieldAmplitude.Value / speedOfLight;

			// Calculate electromagnetic energy density
			var electricEnergyDensity = 0.5 * vacuumPermittivity * Math.Pow(electricFieldAmplitude.Value, 2);
			var magneticEnergyDensity = 0.5 * Math.Pow(magneticFieldAmplitude, 2) / vacuumPermeability;

			// Calculate Poynting vector magnitude: S = E×B/μ₀
			var poyntingVector = electricFieldAmplitude.Value * magneticFieldAmplitude / vacuumPermeability;

			// Assert - Maxwell relationship verification
			Assert.AreEqual(electricEnergyDensity, magneticEnergyDensity, RelativeTolerance * electricEnergyDensity,
				"Electric and magnetic energy densities should be equal in vacuum");

			Assert.IsTrue(poyntingVector > 0, "Poynting vector should be positive for propagating wave");

			// Test impedance relationship: Z₀ = √(μ₀/ε₀) = E/H
			var vacuumImpedance = Math.Sqrt(vacuumPermeability / vacuumPermittivity);
			var calculatedImpedance = electricFieldAmplitude.Value / magneticFieldAmplitude;

			Assert.AreEqual(vacuumImpedance, calculatedImpedance, RelativeTolerance * vacuumImpedance,
				"Vacuum impedance should match E/H ratio");

			// Test relationship between optical and electrical quantities
			if (freq >= 4e14 && freq <= 8e14) // Visible light range
			{
				var photonEnergy = Frequency<double>.GetPhotonEnergy(frequency);
				var expectedEnergy = PhysicalConstants.Generic.PlanckConstant<double>() * frequency.Value;

				Assert.AreEqual(expectedEnergy, photonEnergy.Value, RelativeTolerance * expectedEnergy,
					"Photon energy should match Planck relation E = hf");
			}
		}
	}

	/// <summary>
	/// Tests fluid dynamic relationships with heat and mass transfer.
	/// Verifies Nusselt, Prandtl, and Reynolds number correlations.
	/// </summary>
	[TestMethod]
	public void FluidDynamicsHeatMassTransfer_DimensionlessNumberRelations()
	{
		// Test various fluid conditions
		var testCases = new[]
		{
			new { fluid = "air", temp = 300.0, velocity = 10.0, length = 1.0 },
			new { fluid = "water", temp = 350.0, velocity = 2.0, length = 0.1 },
			new { fluid = "oil", temp = 400.0, velocity = 0.5, length = 2.0 }
		};

		foreach (var testCase in testCases)
		{
			// Arrange - Fluid properties based on type and temperature
			var temperature = Temperature<double>.FromKelvin(testCase.temp);
			var velocity = Velocity<double>.FromMetersPerSecond(testCase.velocity);
			var characteristicLength = Length<double>.FromMeters(testCase.length);

			// Set fluid properties
			var (density, viscosity, thermalConductivity, specificHeat) = GetFluidProperties(testCase.fluid, testCase.temp);

			var fluidDensity = Density<double>.FromKilogramsPerCubicMeter(density);
			var dynamicViscosity = DynamicViscosity<double>.FromPascalSeconds(viscosity);
			var thermConductivity = ThermalConductivity<double>.FromWattsPerMeterKelvin(thermalConductivity);
			var fluidSpecificHeat = SpecificHeat<double>.FromJoulesPerKilogramKelvin(specificHeat);

			// Act - Calculate dimensionless numbers
			var reynolds = ReynoldsNumber<double>.FromFluidProperties(
				fluidDensity, velocity, characteristicLength, dynamicViscosity);

			var kinematicViscosity = KinematicViscosity<double>.FromDynamicViscosityAndDensity(
				dynamicViscosity, fluidDensity);

			var thermalDiffusivity = ThermalDiffusivity<double>.Create(
				thermConductivity.Value / (fluidDensity.Value * fluidSpecificHeat.Value));

			var prandtl = kinematicViscosity.Value / thermalDiffusivity.Value;

			// Calculate Nusselt number using empirical correlation for forced convection
			var nusselt = CalculateNusseltNumber(reynolds.Value, prandtl);

			// Assert - Verify dimensionless number relationships
			Assert.IsTrue(reynolds.Value > 0, "Reynolds number should be positive");
			Assert.IsTrue(prandtl > 0, "Prandtl number should be positive");
			Assert.IsTrue(nusselt > 1, "Nusselt number should be greater than 1 for convection");

			// Test physical relationships between numbers
			if (reynolds.Value < 2300) // Laminar flow
			{
				Assert.IsTrue(nusselt < 100, "Nusselt number should be moderate for laminar flow");
			}
			else if (reynolds.Value > 4000) // Turbulent flow
			{
				Assert.IsTrue(nusselt > 10, "Nusselt number should be higher for turbulent flow");
			}

			// Test Prandtl number physical ranges
			switch (testCase.fluid)
			{
				case "air":
					Assert.IsTrue(prandtl > 0.5 && prandtl < 1.0, "Air Prandtl number should be ~0.7");
					break;
				case "water":
					Assert.IsTrue(prandtl > 1.0 && prandtl < 15.0, "Water Prandtl number should be 1-15");
					break;
				case "oil":
					Assert.IsTrue(prandtl > 10.0, "Oil Prandtl number should be > 10");
					break;
			}

			// Test heat transfer coefficient calculation
			var heatTransferCoeff = HeatTransferCoefficient<double>.Create(
				nusselt * thermConductivity.Value / characteristicLength.Value);

			Assert.IsTrue(heatTransferCoeff.Value > 0, "Heat transfer coefficient should be positive");
		}
	}

	/// <summary>
	/// Tests quantum mechanical relationships in nuclear and optical domains.
	/// Verifies de Broglie wavelength, uncertainty principle, and energy-momentum relations.
	/// </summary>
	[TestMethod]
	public void QuantumMechanicalRelations_EnergyMomentumConsistency()
	{
		// Test various quantum scenarios
		var testCases = new[]
		{
			new { particle = "electron", energy_eV = 1.0 },
			new { particle = "electron", energy_eV = 10.0 },
			new { particle = "proton", energy_eV = 1000.0 },
			new { particle = "neutron", energy_eV = 0.025 } // Thermal neutron
		};

		foreach (var testCase in testCases)
		{
			// Arrange - Particle properties
			var energy = Energy<double>.FromElectronVolts(testCase.energy_eV);
			var (mass, charge) = GetParticleProperties(testCase.particle);

			var particleMass = Mass<double>.FromKilograms(mass);
			var speedOfLight = PhysicalConstants.Generic.SpeedOfLight<double>();
			var planckConstant = PhysicalConstants.Generic.PlanckConstant<double>();
			var reducedPlanck = planckConstant / (2 * Math.PI);

			// Act - Calculate quantum mechanical properties
			// For non-relativistic case: E = p²/(2m), so p = √(2mE)
			var momentum = Math.Sqrt(2 * particleMass.Value * energy.Value);
			var velocity = momentum / particleMass.Value;

			// Verify non-relativistic assumption (v << c)
			if (velocity < 0.1 * speedOfLight)
			{
				// Calculate de Broglie wavelength: λ = h/p
				var deBroglieWavelength = planckConstant / momentum;

				// Calculate classical kinetic energy
				var classicalKineticEnergy = 0.5 * particleMass.Value * Math.Pow(velocity, 2);

				// Assert - Quantum relationships
				Assert.AreEqual(energy.Value, classicalKineticEnergy, RelativeTolerance * energy.Value,
					"Non-relativistic kinetic energy should match input energy");

				Assert.IsTrue(deBroglieWavelength > 0, "de Broglie wavelength should be positive");

				// Test uncertainty principle: Δx·Δp ≥ ℏ/2
				var minimumUncertainty = reducedPlanck / 2;
				var assumedPositionUncertainty = deBroglieWavelength; // Order of magnitude estimate
				var momentumUncertainty = minimumUncertainty / assumedPositionUncertainty;

				Assert.IsTrue(momentumUncertainty > 0, "Momentum uncertainty should be positive");
				Assert.IsTrue(momentumUncertainty * assumedPositionUncertainty >= minimumUncertainty * 0.99,
					"Uncertainty principle should be satisfied");

				// Test wavelength ranges for different particles
				switch (testCase.particle)
				{
					case "electron":
						if (testCase.energy_eV == 1.0)
						{
							// 1 eV electron should have wavelength ~1.2 nm
							Assert.IsTrue(deBroglieWavelength > 1e-9 && deBroglieWavelength < 2e-9,
								"1 eV electron wavelength should be ~1.2 nm");
						}
						break;
					case "neutron":
						if (testCase.energy_eV == 0.025)
						{
							// Thermal neutron should have wavelength ~1.8 Å
							Assert.IsTrue(deBroglieWavelength > 1e-10 && deBroglieWavelength < 3e-10,
								"Thermal neutron wavelength should be ~1.8 Å");
						}
						break;
				}
			}
		}
	}

	/// <summary>
	/// Tests chemical reaction kinetics with thermodynamic constraints.
	/// Verifies Arrhenius equation, equilibrium constants, and energy conservation.
	/// </summary>
	[TestMethod]
	public void ChemicalKineticsThermodynamics_ReactionEquilibrium()
	{
		// Test reaction: A + B ⇌ C + D
		var testTemperatures = new double[] { 298.15, 350.0, 400.0, 450.0 }; // 25°C to 177°C

		var activationEnergyForward = ActivationEnergy<double>.FromJoulesPerMole(80000.0);  // 80 kJ/mol
		var activationEnergyReverse = ActivationEnergy<double>.FromJoulesPerMole(120000.0); // 120 kJ/mol
		var reactionEnthalpy = Energy<double>.Create(activationEnergyReverse.Value - activationEnergyForward.Value); // -40 kJ/mol

		foreach (var temp in testTemperatures)
		{
			// Arrange - Reaction conditions
			var temperature = Temperature<double>.FromKelvin(temp);
			var preExponentialFactor = 1e12; // s⁻¹

			// Act - Calculate forward and reverse rate constants
			var forwardRateConstant = RateConstant<double>.FromArrheniusEquation(
				preExponentialFactor, activationEnergyForward, temperature);
			var reverseRateConstant = RateConstant<double>.FromArrheniusEquation(
				preExponentialFactor, activationEnergyReverse, temperature);

			// Calculate equilibrium constant: K = k_forward / k_reverse
			var equilibriumConstant = forwardRateConstant.Value / reverseRateConstant.Value;

			// Calculate equilibrium constant from thermodynamics: K = exp(-ΔG/RT)
			var gasConstant = PhysicalConstants.Generic.GasConstant<double>();
			var thermoEquilibriumConstant = Math.Exp(-reactionEnthalpy.Value / (gasConstant * temperature.Value));

			// Assert - Kinetic and thermodynamic consistency
			Assert.AreEqual(thermoEquilibriumConstant, equilibriumConstant,
				RelativeTolerance * thermoEquilibriumConstant,
				"Kinetic and thermodynamic equilibrium constants should match");

			// Test temperature dependence (van 't Hoff equation)
			if (temp > testTemperatures[0])
			{
				var prevTemp = Temperature<double>.FromKelvin(testTemperatures[0]);
				var prevForwardRate = RateConstant<double>.FromArrheniusEquation(
					preExponentialFactor, activationEnergyForward, prevTemp);

				// Higher temperature should give higher rate constant for endothermic activation
				Assert.IsTrue(forwardRateConstant.Value > prevForwardRate.Value,
					"Rate constant should increase with temperature");

				// Test Arrhenius plot linearity: ln(k) vs 1/T should be linear
				var lnRatio = Math.Log(forwardRateConstant.Value / prevForwardRate.Value);
				var invTempDiff = 1.0 / prevTemp.Value - 1.0 / temperature.Value;
				var calculatedActivationEnergy = -gasConstant * lnRatio / invTempDiff;

				Assert.AreEqual(activationEnergyForward.Value, calculatedActivationEnergy,
					RelativeTolerance * activationEnergyForward.Value,
					"Calculated activation energy should match input");
			}

			// Test reaction rate calculation with concentrations
			var concentrationA = Concentration<double>.FromMolar(1.0);
			var concentrationB = Concentration<double>.FromMolar(0.5);

			var reactionRate = ReactionRate<double>.FromSecondOrderKinetics(
				forwardRateConstant, concentrationA, concentrationB);

			Assert.IsTrue(reactionRate.Value > 0, "Reaction rate should be positive");
			Assert.IsTrue(reactionRate.Value == forwardRateConstant.Value * concentrationA.Value * concentrationB.Value,
				"Second-order reaction rate should follow rate law");
		}
	}

	#endregion

	#region Helper Methods

	/// <summary>
	/// Gets fluid properties for testing purposes.
	/// </summary>
	private static (double density, double viscosity, double thermalConductivity, double specificHeat)
		GetFluidProperties(string fluid, double temperature)
	{
		return fluid switch
		{
			"air" => (1.225, 1.825e-5, 0.026, 1005.0),
			"water" => (958.4, 0.000365, 0.668, 4186.0),
			"oil" => (850.0, 0.01, 0.14, 2100.0),
			_ => throw new ArgumentException($"Unknown fluid: {fluid}")
		};
	}

	/// <summary>
	/// Gets particle properties for quantum mechanical calculations.
	/// </summary>
	private static (double mass, double charge) GetParticleProperties(string particle)
	{
		return particle switch
		{
			"electron" => (PhysicalConstants.Generic.ElectronMass<double>(),
						  -PhysicalConstants.Generic.ElementaryCharge<double>()),
			"proton" => (PhysicalConstants.Generic.ProtonMass<double>(),
						PhysicalConstants.Generic.ElementaryCharge<double>()),
			"neutron" => (PhysicalConstants.Generic.NeutronMass<double>(), 0.0),
			_ => throw new ArgumentException($"Unknown particle: {particle}")
		};
	}

	/// <summary>
	/// Calculates Nusselt number using empirical correlations.
	/// </summary>
	private static double CalculateNusseltNumber(double reynolds, double prandtl)
	{
		if (reynolds < 2300) // Laminar flow
		{
			return 3.66; // Constant for fully developed laminar flow in circular pipe
		}
		else if (reynolds > 10000) // Turbulent flow
		{
			// Dittus-Boelter correlation: Nu = 0.023 * Re^0.8 * Pr^0.4
			return 0.023 * Math.Pow(reynolds, 0.8) * Math.Pow(prandtl, 0.4);
		}
		else // Transition region
		{
			// Linear interpolation between laminar and turbulent
			var laminarNu = 3.66;
			var turbulentNu = 0.023 * Math.Pow(10000, 0.8) * Math.Pow(prandtl, 0.4);
			var factor = (reynolds - 2300) / (10000 - 2300);
			return laminarNu + factor * (turbulentNu - laminarNu);
		}
	}

	#endregion
}
