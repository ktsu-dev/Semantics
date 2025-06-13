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
	private const int SmallIterationCount = 10000;
	private const int WarmupIterations = 10000;

	/// <summary>
	/// Performance baseline for basic quantity creation across all domains.
	/// Target: >1M operations/second per domain.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_QuantityCreation()
	{
		var results = new Dictionary<string, double>();

		// Test each domain's basic quantity creation
		var domainTests = new Dictionary<string, Action>
		{
			["Mechanical"] = () => { var _ = Force<double>.FromNewtons(100.0); },
			["Electrical"] = () => { var _ = ElectricCurrent<double>.FromAmperes(5.0); },
			["Thermal"] = () => { var _ = Temperature<double>.FromCelsius(25.0); },
			["Chemical"] = () => { var _ = Concentration<double>.FromMolar(1.0); },
			["Acoustic"] = () => { var _ = Frequency<double>.FromHertz(1000.0); },
			["Optical"] = () => { var _ = LuminousIntensity<double>.FromCandelas(100.0); },
			["Nuclear"] = () => { var _ = RadioactiveActivity<double>.FromBecquerels(1000.0); },
			["FluidDynamics"] = () => { var _ = ReynoldsNumber<double>.Create(10000.0); }
		};

		foreach (var domain in domainTests)
		{
			// Warmup
			for (int i = 0; i < WarmupIterations; i++)
			{
				domain.Value();
			}

			// Benchmark
			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < LargeIterationCount; i++)
			{
				domain.Value();
			}
			stopwatch.Stop();

			var operationsPerSecond = LargeIterationCount / stopwatch.Elapsed.TotalSeconds;
			results[domain.Key] = operationsPerSecond;

			Console.WriteLine($"{domain.Key} creation: {operationsPerSecond:F0} ops/sec");

			// Performance regression test - should be > 1M ops/sec
			Assert.IsTrue(operationsPerSecond > 1000000,
				$"{domain.Key} quantity creation performance regression: {operationsPerSecond:F0} ops/sec < 1M ops/sec");
		}

		// Overall performance should be consistent across domains (within 50% variance)
		var averagePerformance = results.Values.Average();
		foreach (var result in results)
		{
			var variance = Math.Abs(result.Value - averagePerformance) / averagePerformance;
			Assert.IsTrue(variance < 0.5,
				$"{result.Key} performance variance too high: {variance:P1} from average {averagePerformance:F0}");
		}
	}

	/// <summary>
	/// Performance baseline for unit conversions across different complexity levels.
	/// Target: >500K simple conversions/second, >100K complex conversions/second.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_UnitConversions()
	{
		// Simple conversions (same dimension, linear scaling)
		var simpleConversions = new[]
		{
			new { name = "Length m→km", action = new Action(() => {
				var length = Length<double>.FromMeters(1000.0);
				var _ = length.In(Units.Kilometer);
			})},
			new { name = "Mass kg→g", action = new Action(() => {
				var mass = Mass<double>.FromKilograms(1.0);
				var _ = mass.In(Units.Gram);
			})},
			new { name = "Energy J→kWh", action = new Action(() => {
				var energy = Energy<double>.FromJoules(3600000.0);
				var _ = energy.In(Units.KilowattHour);
			})}
		};

		// Complex conversions (non-linear, with offsets)
		var complexConversions = new[]
		{
			new { name = "Temperature C→F", action = new Action(() => {
				var temp = Temperature<double>.FromCelsius(25.0);
				var _ = temp.ToFahrenheit();
			})},
			new { name = "Temperature K→C", action = new Action(() => {
				var temp = Temperature<double>.FromKelvin(298.15);
				var _ = temp.ToCelsius();
			})},
			new { name = "Pressure Pa→psi", action = new Action(() => {
				var pressure = Pressure<double>.FromPascals(101325.0);
				var _ = pressure.In(Units.PoundsPerSquareInch);
			})}
		};

		// Test simple conversions
		foreach (var conversion in simpleConversions)
		{
			var performance = MeasurePerformance(conversion.action, MediumIterationCount);
			Console.WriteLine($"{conversion.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 500000,
				$"Simple conversion performance regression: {conversion.name} = {performance:F0} ops/sec < 500K ops/sec");
		}

		// Test complex conversions
		foreach (var conversion in complexConversions)
		{
			var performance = MeasurePerformance(conversion.action, MediumIterationCount);
			Console.WriteLine($"{conversion.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 100000,
				$"Complex conversion performance regression: {conversion.name} = {performance:F0} ops/sec < 100K ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for arithmetic operations between quantities.
	/// Target: >2M operations/second for basic arithmetic.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_ArithmeticOperations()
	{
		var arithmeticTests = new[]
		{
			new { name = "Force Addition", action = new Action(() => {
				var f1 = Force<double>.FromNewtons(100.0);
				var f2 = Force<double>.FromNewtons(50.0);
				var _ = f1 + f2;
			})},
			new { name = "Energy Subtraction", action = new Action(() => {
				var e1 = Energy<double>.FromJoules(1000.0);
				var e2 = Energy<double>.FromJoules(300.0);
				var _ = e1 - e2;
			})},
			new { name = "Temperature Difference", action = new Action(() => {
				var t1 = Temperature<double>.FromKelvin(350.0);
				var t2 = Temperature<double>.FromKelvin(300.0);
				var _ = t1 - t2;
			})},
			new { name = "Scalar Multiplication", action = new Action(() => {
				var mass = Mass<double>.FromKilograms(10.0);
				var _ = mass * 2.5;
			})},
			new { name = "Scalar Division", action = new Action(() => {
				var distance = Length<double>.FromMeters(100.0);
				var _ = distance / 3.0;
			})}
		};

		foreach (var test in arithmeticTests)
		{
			var performance = MeasurePerformance(test.action, LargeIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 2000000,
				$"Arithmetic operation performance regression: {test.name} = {performance:F0} ops/sec < 2M ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for physics relationship calculations.
	/// Target: >200K operations/second for complex physics calculations.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_PhysicsRelationships()
	{
		var physicsTests = new[]
		{
			new { name = "Newton's 2nd Law", action = new Action(() => {
				var mass = Mass<double>.FromKilograms(10.0);
				var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
				var _ = mass * acceleration; // F = ma
			})},
			new { name = "Ohm's Law", action = new Action(() => {
				var current = ElectricCurrent<double>.FromAmperes(5.0);
				var resistance = ElectricResistance<double>.FromOhms(10.0);
				var _ = current * resistance; // V = IR
			})},
			new { name = "Ideal Gas Law", action = new Action(() => {
				var pressure = Pressure<double>.FromPascals(101325.0);
				var volume = Volume<double>.FromCubicMeters(0.024);
				var temperature = Temperature<double>.FromKelvin(273.15);
				var _ = AmountOfSubstance<double>.FromIdealGasLaw(pressure, volume, temperature);
			})},
			new { name = "Kinetic Energy", action = new Action(() => {
				var mass = Mass<double>.FromKilograms(2.0);
				var velocity = Velocity<double>.FromMetersPerSecond(10.0);
				var _ = Energy<double>.FromKineticEnergy(mass, velocity);
			})},
			new { name = "Reynolds Number", action = new Action(() => {
				var density = Density<double>.FromKilogramsPerCubicMeter(1000.0);
				var velocity = Velocity<double>.FromMetersPerSecond(2.0);
				var length = Length<double>.FromMeters(0.1);
				var viscosity = DynamicViscosity<double>.FromPascalSeconds(0.001);
				var _ = ReynoldsNumber<double>.FromFluidProperties(density, velocity, length, viscosity);
			})},
			new { name = "Sound Wavelength", action = new Action(() => {
				var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);
				var frequency = Frequency<double>.FromHertz(1000.0);
				var _ = soundSpeed / frequency;
			})}
		};

		foreach (var test in physicsTests)
		{
			var performance = MeasurePerformance(test.action, MediumIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 200000,
				$"Physics relationship performance regression: {test.name} = {performance:F0} ops/sec < 200K ops/sec");
		}
	}

	/// <summary>
	/// Performance baseline for constant access patterns.
	/// Target: >5M operations/second for constant access.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_ConstantAccess()
	{
		var constantTests = new[]
		{
			new { name = "Speed of Light", action = new Action(() => {
				var _ = PhysicalConstants.Generic.SpeedOfLight<double>();
			})},
			new { name = "Planck Constant", action = new Action(() => {
				var _ = PhysicalConstants.Generic.PlanckConstant<double>();
			})},
			new { name = "Gas Constant", action = new Action(() => {
				var _ = PhysicalConstants.Generic.GasConstant<double>();
			})},
			new { name = "Avogadro Number", action = new Action(() => {
				var _ = PhysicalConstants.Generic.AvogadroNumber<double>();
			})},
			new { name = "Boltzmann Constant", action = new Action(() => {
				var _ = PhysicalConstants.Generic.BoltzmannConstant<double>();
			})}
		};

		foreach (var test in constantTests)
		{
			var performance = MeasurePerformance(test.action, LargeIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 5000000,
				$"Constant access performance regression: {test.name} = {performance:F0} ops/sec < 5M ops/sec");
		}
	}

	/// <summary>
	/// Memory allocation performance and garbage collection impact.
	/// Target: Minimal GC pressure, <1KB per 1000 operations.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_MemoryAllocation()
	{
		// Force GC before starting
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		var initialMemory = GC.GetTotalMemory(false);
		var initialGen0 = GC.CollectionCount(0);
		var initialGen1 = GC.CollectionCount(1);
		var initialGen2 = GC.CollectionCount(2);

		// Perform operations that should have minimal allocation
		var stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < SmallIterationCount; i++)
		{
			// Operations using value types (should not allocate)
			var temp = Temperature<double>.FromCelsius(25.0 + i * 0.01);
			var pressure = Pressure<double>.FromPascals(101325.0 + i);
			var volume = Volume<double>.FromCubicMeters(0.024 + i * 0.000001);

			// Basic arithmetic (should not allocate)
			var _ = temp.ToKelvin();
			var __ = pressure.In(Units.Pascal);
			var ___ = volume * 2.0;
		}
		stopwatch.Stop();

		var finalMemory = GC.GetTotalMemory(false);
		var finalGen0 = GC.CollectionCount(0);
		var finalGen1 = GC.CollectionCount(1);
		var finalGen2 = GC.CollectionCount(2);

		var allocatedMemory = finalMemory - initialMemory;
		var gen0Collections = finalGen0 - initialGen0;
		var gen1Collections = finalGen1 - initialGen1;
		var gen2Collections = finalGen2 - initialGen2;

		var operationsPerSecond = SmallIterationCount / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Memory allocation test:");
		Console.WriteLine($"  Operations: {SmallIterationCount:N0} in {stopwatch.Elapsed.TotalMilliseconds:F1} ms");
		Console.WriteLine($"  Performance: {operationsPerSecond:F0} ops/sec");
		Console.WriteLine($"  Memory allocated: {allocatedMemory:N0} bytes ({allocatedMemory / (double)SmallIterationCount:F3} bytes/op)");
		Console.WriteLine($"  GC collections: Gen0={gen0Collections}, Gen1={gen1Collections}, Gen2={gen2Collections}");

		// Performance assertions
		Assert.IsTrue(operationsPerSecond > 100000,
			$"Memory allocation test performance regression: {operationsPerSecond:F0} ops/sec < 100K ops/sec");

		// Memory allocation should be minimal (< 1KB per 1000 operations)
		var bytesPerOperation = allocatedMemory / (double)SmallIterationCount;
		Assert.IsTrue(bytesPerOperation < 1.0,
			$"Excessive memory allocation: {bytesPerOperation:F3} bytes/operation > 1.0 bytes/operation");

		// Should not trigger Gen1 or Gen2 collections for these simple operations
		Assert.AreEqual(0, gen1Collections, "Should not trigger Gen1 garbage collection");
		Assert.AreEqual(0, gen2Collections, "Should not trigger Gen2 garbage collection");
	}

	/// <summary>
	/// Cross-domain calculation performance with complex scenarios.
	/// Target: >50K operations/second for complex multi-domain calculations.
	/// </summary>
	[TestMethod]
	public void PerformanceBaseline_CrossDomainCalculations()
	{
		var crossDomainTests = new[]
		{
			new { name = "Electromagnetic Power", action = new Action(() => {
				var voltage = ElectricPotential<double>.FromVolts(230.0);
				var current = ElectricCurrent<double>.FromAmperes(10.0);
				var power = voltage * current; // Electrical
				var time = Time<double>.FromHours(1.0); // Mechanical
				var _ = power * time; // Energy calculation
			})},
			new { name = "Fluid Heat Transfer", action = new Action(() => {
				var density = Density<double>.FromKilogramsPerCubicMeter(1000.0); // Mechanical
				var velocity = Velocity<double>.FromMetersPerSecond(2.0); // Mechanical
				var diameter = Length<double>.FromMeters(0.1); // Mechanical
				var viscosity = DynamicViscosity<double>.FromPascalSeconds(0.001); // Chemical/Fluid
				var reynolds = ReynoldsNumber<double>.FromFluidProperties(density, velocity, diameter, viscosity); // Fluid
				var _ = reynolds.Value > 4000; // Flow regime determination
			})},
			new { name = "Acoustic Power", action = new Action(() => {
				var soundPressure = SoundPressure<double>.FromPascals(0.1); // Acoustic
				var frequency = Frequency<double>.FromHertz(1000.0); // Acoustic
				var density = Density<double>.FromKilogramsPerCubicMeter(1.225); // Mechanical
				var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0); // Acoustic
				var impedance = AcousticImpedance<double>.FromDensityAndSoundSpeed(density, soundSpeed);
				var _ = SoundIntensity<double>.FromPressureAndImpedance(soundPressure, impedance);
			})},
			new { name = "Nuclear Decay Energy", action = new Action(() => {
				var activity = RadioactiveActivity<double>.FromBecquerels(1000.0); // Nuclear
				var halfLife = Time<double>.FromSeconds(3600.0); // Mechanical
				var elapsedTime = Time<double>.FromSeconds(7200.0); // Mechanical
				var remaining = activity.CalculateActivityAfterTime(halfLife, elapsedTime); // Nuclear
				var _ = activity.Value - remaining.Value; // Decayed atoms
			})}
		};

		foreach (var test in crossDomainTests)
		{
			var performance = MeasurePerformance(test.action, SmallIterationCount);
			Console.WriteLine($"{test.name}: {performance:F0} ops/sec");

			Assert.IsTrue(performance > 50000,
				$"Cross-domain calculation performance regression: {test.name} = {performance:F0} ops/sec < 50K ops/sec");
		}
	}

	#region Helper Methods

	/// <summary>
	/// Measures the performance of an action with warmup and accurate timing.
	/// </summary>
	private static double MeasurePerformance(Action action, int iterations)
	{
		// Warmup
		for (int i = 0; i < Math.Min(WarmupIterations, iterations / 10); i++)
		{
			action();
		}

		// Force garbage collection before measurement
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		// Measure
		var stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < iterations; i++)
		{
			action();
		}
		stopwatch.Stop();

		return iterations / stopwatch.Elapsed.TotalSeconds;
	}

	#endregion
}
