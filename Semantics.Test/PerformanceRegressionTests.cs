// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Performance regression tests to monitor library performance over time.
/// These tests establish performance baselines and detect regressions.
/// </summary>
[TestClass]
public class PerformanceRegressionTests
{
	private const int LargeIterationCount = 1000000;
	private const int MediumIterationCount = 100000;
	private const int WarmupIterations = 10000;

	/// <summary>
	/// Performance baseline for basic quantity creation across all domains.
	/// Target: > 1M operations/second per domain.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_QuantityCreation()
	{
		Dictionary<string, double> results = [];

		// Test each domain's basic quantity creation
		Dictionary<string, Action> domainTests = new()
		{
			["Mechanical"] = () => { Force<double> _ = Force<double>.FromNewtons(100.0); },
			["Electrical"] = () => { ElectricCurrent<double> _ = ElectricCurrent<double>.FromAmperes(5.0); },
			["Thermal"] = () => { Temperature<double> _ = Temperature<double>.FromKelvin(300.0); },
			["Chemical"] = () => { AmountOfSubstance<double> _ = AmountOfSubstance<double>.FromMoles(1.0); },
			["Acoustic"] = () => { Frequency<double> _ = Frequency<double>.FromHertz(1000.0); },
			["Optical"] = () => { LuminousIntensity<double> _ = LuminousIntensity<double>.FromCandelas(100.0); },
			["Nuclear"] = () => { RadioactiveActivity<double> _ = RadioactiveActivity<double>.FromBecquerels(1000.0); },
			["FluidDynamics"] = () => { ReynoldsNumber<double> _ = ReynoldsNumber<double>.Create(10000.0); }
		};

		foreach (KeyValuePair<string, Action> domain in domainTests)
		{
			// Warmup
			for (int i = 0; i < WarmupIterations; i++)
			{
				domain.Value();
			}

			// Benchmark
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < LargeIterationCount; i++)
			{
				domain.Value();
			}
			stopwatch.Stop();

			double operationsPerSecond = LargeIterationCount / stopwatch.Elapsed.TotalSeconds;
			results[domain.Key] = operationsPerSecond;

			Console.WriteLine($"{domain.Key} creation: {operationsPerSecond:F0} ops/sec");

			// Performance regression test - should be > 1M ops/sec
			Assert.IsTrue(operationsPerSecond > 1000000,
				$"{domain.Key} quantity creation performance regression: {operationsPerSecond:F0} ops/sec < 1M ops/sec");
		}

		// Overall performance should be consistent across domains (within 50% variance)
		double averagePerformance = results.Values.Average();
		foreach (KeyValuePair<string, double> result in results)
		{
			double variance = Math.Abs(result.Value - averagePerformance) / averagePerformance;
			Assert.IsTrue(variance < 0.5,
				$"{result.Key} performance variance too high: {variance:P1} from average {averagePerformance:F0}");
		}
	}

	/// <summary>
	/// Performance baseline for unit conversions.
	/// Target: > 500K conversions/second.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_UnitConversions()
	{
		var conversions = new[]
		{
			new { name = "Temperature K→C", action = new Action(() => {
				Temperature<double> temp = Temperature<double>.FromKelvin(300.0);
				double _ = temp.ToCelsius();
			})},
			new { name = "Temperature K→F", action = new Action(() => {
				Temperature<double> temp = Temperature<double>.FromKelvin(300.0);
				double _ = temp.ToFahrenheit();
			})},
			new { name = "Mass kg→lb", action = new Action(() => {
				Mass<double> mass = Mass<double>.FromKilograms(1.0);
				double _ = mass.Value * 2.20462; // Convert to pounds
			})}
		};

		foreach (var conversion in conversions)
		{
			double performance = MeasurePerformance(conversion.action, MediumIterationCount);
			Console.WriteLine($"{conversion.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 500000,
				$"Conversion performance regression: {conversion.name} = {performance:F0} ops/sec < 500K ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for arithmetic operations between quantities.
	/// Target: > 2M operations/second for basic arithmetic.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_ArithmeticOperations()
	{
		var arithmeticTests = new[]
		{
			new { name = "Force Addition", action = new Action(() => {
				Force<double> f1 = Force<double>.FromNewtons(100.0);
				Force<double> f2 = Force<double>.FromNewtons(50.0);
				Force<double> _ = f1 + f2;
			})},
			new { name = "Energy Subtraction", action = new Action(() => {
				Energy<double> e1 = Energy<double>.FromJoules(1000.0);
				Energy<double> e2 = Energy<double>.FromJoules(300.0);
				Energy<double> _ = e1 - e2;
			})},
			new { name = "Temperature Difference", action = new Action(() => {
				Temperature<double> t1 = Temperature<double>.FromKelvin(350.0);
				Temperature<double> t2 = Temperature<double>.FromKelvin(300.0);
				Temperature<double> _ = t1 - t2;
			})},
			new { name = "Scalar Multiplication", action = new Action(() => {
				Mass<double> mass = Mass<double>.FromKilograms(10.0);
				Mass<double> _ = mass * 2.5;
			})},
			new { name = "Scalar Division", action = new Action(() => {
				Length<double> distance = Length<double>.FromMeters(100.0);
				Length<double> _ = distance / 3.0;
			})}
		};

		foreach (var test in arithmeticTests)
		{
			double performance = MeasurePerformance(test.action, LargeIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 2000000,
				$"Arithmetic operation performance regression: {test.name} = {performance:F0} ops/sec < 2M ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for physics relationship calculations.
	/// Target: > 200K operations/second for complex physics calculations.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_PhysicsRelationships()
	{
		var physicsTests = new[]
		{
			new { name = "Newton's 2nd Law", action = new Action(() => {
				Mass<double> mass = Mass<double>.FromKilograms(10.0);
				Acceleration<double> acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
				Force<double> _ = mass * acceleration; // F = ma
			})},
			new { name = "Ohm's Law", action = new Action(() => {
				ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(5.0);
				ElectricResistance<double> resistance = ElectricResistance<double>.FromOhms(10.0);
				ElectricPotential<double> _ = current * resistance; // V = IR
			})},
			new { name = "Momentum Calculation", action = new Action(() => {
				Mass<double> mass = Mass<double>.FromKilograms(2.0);
				Velocity<double> velocity = Velocity<double>.FromMetersPerSecond(15.0);
				Momentum<double> _ = mass * velocity; // p = mv
			})},
			new { name = "Power Calculation", action = new Action(() => {
				ElectricPotential<double> voltage = ElectricPotential<double>.FromVolts(12.0);
				ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(3.0);
				Power<double> _ = voltage * current; // P = VI
			})},
			new { name = "Density Calculation", action = new Action(() => {
				Mass<double> mass = Mass<double>.FromKilograms(5.0);
				Volume<double> volume = Volume<double>.FromCubicMeters(0.002);
				Density<double> _ = mass / volume; // ρ = m/V
			})}
		};

		foreach (var test in physicsTests)
		{
			double performance = MeasurePerformance(test.action, MediumIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 200000,
				$"Physics relationship performance regression: {test.name} = {performance:F0} ops/sec < 200K ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for physical constant access.
	/// Target: > 5M operations/second for constant access.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_ConstantAccess()
	{
		var constantTests = new[]
		{
			new { name = "Speed of Light", action = new Action(() => {
				double _ = PhysicalConstants.Generic.SpeedOfLight<double>();
			})},
			new { name = "Gas Constant", action = new Action(() => {
				double _ = PhysicalConstants.Generic.GasConstant<double>();
			})},
			new { name = "Standard Gravity", action = new Action(() => {
				double _ = PhysicalConstants.Generic.StandardGravity<double>();
			})},
			new { name = "Avogadro Number", action = new Action(() => {
				double _ = PhysicalConstants.Generic.AvogadroNumber<double>();
			})}
		};

		foreach (var test in constantTests)
		{
			double performance = MeasurePerformance(test.action, LargeIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 5000000,
				$"Constant access performance regression: {test.name} = {performance:F0} ops/sec < 5M ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for cross-domain calculations.
	/// Target: > 50K operations/second for multi-domain scenarios.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_CrossDomainCalculations()
	{
		var crossDomainTests = new[]
		{
			new { name = "Thermal-Mechanical", action = new Action(() => {
				Temperature<double> temperature = Temperature<double>.FromKelvin(300.0);
				Pressure<double> pressure = Pressure<double>.FromPascals(PhysicalConstants.Generic.StandardAtmosphericPressure<double>());
				double tempRatio = temperature.Value / PhysicalConstants.Generic.StandardTemperature<double>();
				double pressRatio = pressure.Value / PhysicalConstants.Generic.StandardAtmosphericPressure<double>();
				double _ = tempRatio * pressRatio;
			})},
			new { name = "Electrical-Thermal", action = new Action(() => {
				ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(10.0);
				ElectricResistance<double> resistance = ElectricResistance<double>.FromOhms(5.0);
				Time<double> time = Time<double>.FromSeconds(1.0);
				ElectricPotential<double> voltage = current * resistance;
				Power<double> power = voltage * current;
				Energy<double> _ = Energy<double>.Create(power.Value * time.Value);
			})},
			new { name = "Chemical-Thermal", action = new Action(() => {
				AmountOfSubstance<double> amount = AmountOfSubstance<double>.FromMoles(1.0);
				Temperature<double> temperature = Temperature<double>.FromKelvin(298.15);
				double gasConstant = PhysicalConstants.Generic.GasConstant<double>();
				double _ = gasConstant * temperature.Value * amount.Value;
			})}
		};

		foreach (var test in crossDomainTests)
		{
			double performance = MeasurePerformance(test.action, MediumIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 50000,
				$"Cross-domain performance regression: {test.name} = {performance:F0} ops/sec < 50K ops/sec");
		}
	}

	private static double MeasurePerformance(Action action, int iterations)
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			action();
		}

		// Measure
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < iterations; i++)
		{
			action();
		}
		stopwatch.Stop();

		return iterations / stopwatch.Elapsed.TotalSeconds;
	}
}
