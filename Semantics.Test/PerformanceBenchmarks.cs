// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Performance benchmarks for common physics quantity operations.
/// These tests measure performance characteristics but don't fail on timing.
/// </summary>
[TestClass]
public class PerformanceBenchmarks
{
	private const int IterationCount = 100000;
	private const int WarmupIterations = 1000;

	/// <summary>
	/// Benchmarks unit conversion performance across different domains.
	/// </summary>
	[TestMethod]
	public void BenchmarkUnitConversions()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var temp = Temperature<double>.FromCelsius(25.0);
			var _ = temp.ToFahrenheit();
		}

		var stopwatch = Stopwatch.StartNew();

		// Benchmark different unit conversions
		for (int i = 0; i < IterationCount; i++)
		{
			// Temperature conversions
			var temp = Temperature<double>.FromCelsius(25.0 + i * 0.001);
			var fahrenheit = temp.ToFahrenheit();
			var kelvin = temp.ToKelvin();

			// Length conversions
			var length = Length<double>.FromMeters(1.0 + i * 0.0001);
			var feet = length.ToFeet();
			var inches = length.ToInches();

			// Energy conversions
			var energy = Energy<double>.FromJoules(1000.0 + i * 0.01);
			var calories = energy.ToCalories();
			var kwh = energy.ToKilowattHours();
		}

		stopwatch.Stop();
		double operationsPerSecond = (IterationCount * 6) / stopwatch.Elapsed.TotalSeconds; // 6 conversions per iteration

		Console.WriteLine($"Unit conversions: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 100000, "Unit conversions should be fast");
	}

	/// <summary>
	/// Benchmarks arithmetic operations between quantities.
	/// </summary>
	[TestMethod]
	public void BenchmarkArithmeticOperations()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var force1 = Force<double>.FromNewtons(100.0);
			var force2 = Force<double>.FromNewtons(50.0);
			var _ = force1 + force2;
		}

		var stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Force arithmetic
			var force1 = Force<double>.FromNewtons(100.0 + i * 0.001);
			var force2 = Force<double>.FromNewtons(50.0 + i * 0.0005);
			var forceSum = force1 + force2;
			var forceDiff = force1 - force2;

			// Energy arithmetic
			var energy1 = Energy<double>.FromJoules(1000.0 + i * 0.01);
			var energy2 = Energy<double>.FromJoules(500.0 + i * 0.005);
			var energySum = energy1 + energy2;
			var energyDiff = energy1 - energy2;

			// Temperature arithmetic
			var temp1 = Temperature<double>.FromKelvin(300.0 + i * 0.001);
			var temp2 = Temperature<double>.FromKelvin(273.15);
			var tempDiff = temp1 - temp2;
		}

		stopwatch.Stop();
		double operationsPerSecond = (IterationCount * 7) / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Arithmetic operations: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 500000, "Arithmetic operations should be very fast");
	}

	/// <summary>
	/// Benchmarks physics relationship calculations.
	/// </summary>
	[TestMethod]
	public void BenchmarkPhysicsRelationships()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var mass = Mass<double>.FromKilograms(10.0);
			var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
			var _ = mass * acceleration;
		}

		var stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// F = ma
			var mass = Mass<double>.FromKilograms(10.0 + i * 0.0001);
			var acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8 + i * 0.00001);
			var force = mass * acceleration;

			// P = VI
			var voltage = ElectricPotential<double>.FromVolts(12.0 + i * 0.0001);
			var current = ElectricCurrent<double>.FromAmperes(2.0 + i * 0.00001);
			var power = voltage * current;

			// E = mc² (simplified)
			var energy = Energy<double>.FromKineticEnergy(mass, Velocity<double>.FromMetersPerSecond(10.0));

			// λ = v/f
			var soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);
			var frequency = Frequency<double>.FromHertz(1000.0 + i * 0.1);
			var wavelength = soundSpeed / frequency;
		}

		stopwatch.Stop();
		double operationsPerSecond = (IterationCount * 4) / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Physics relationships: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 200000, "Physics relationships should be efficient");
	}

	/// <summary>
	/// Benchmarks quantity creation from different input types.
	/// </summary>
	[TestMethod]
	public void BenchmarkQuantityCreation()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var _ = Temperature<double>.FromCelsius(25.0);
		}

		var stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Different creation methods
			var temp1 = Temperature<double>.FromCelsius(25.0 + i * 0.001);
			var temp2 = Temperature<double>.FromKelvin(298.15 + i * 0.001);
			var temp3 = Temperature<double>.FromFahrenheit(77.0 + i * 0.002);

			var force1 = Force<double>.FromNewtons(100.0 + i * 0.001);
			var force2 = Force<double>.FromPounds(22.48 + i * 0.0002);

			var energy1 = Energy<double>.FromJoules(1000.0 + i * 0.01);
			var energy2 = Energy<double>.FromCalories(239.0 + i * 0.002);
			var energy3 = Energy<double>.FromKilowattHours(0.000278 + i * 0.000001);
		}

		stopwatch.Stop();
		double operationsPerSecond = (IterationCount * 8) / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Quantity creation: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 300000, "Quantity creation should be fast");
	}

	/// <summary>
	/// Benchmarks constant access performance.
	/// </summary>
	[TestMethod]
	public void BenchmarkConstantAccess()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var _ = PhysicalConstants.Generic.SpeedOfLight<double>();
		}

		var stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Access various constants
			var c = PhysicalConstants.Generic.SpeedOfLight<double>();
			var g = PhysicalConstants.Generic.StandardGravity<double>();
			var R = PhysicalConstants.Generic.GasConstant<double>();
			var Na = PhysicalConstants.Generic.AvogadroNumber<double>();
			var h = PhysicalConstants.Generic.PlanckConstant<double>();
		}

		stopwatch.Stop();
		double operationsPerSecond = (IterationCount * 5) / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Constant access: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 1000000, "Constant access should be very fast");
	}

	/// <summary>
	/// Benchmarks memory allocation patterns for quantity operations.
	/// </summary>
	[TestMethod]
	public void BenchmarkMemoryAllocation()
	{
		// Force garbage collection before test
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		long startMemory = GC.GetTotalMemory(false);

		// Perform operations that might allocate memory
		for (int i = 0; i < IterationCount; i++)
		{
			var temp = Temperature<double>.FromCelsius(25.0 + i * 0.001);
			var force = Force<double>.FromNewtons(100.0 + i * 0.001);
			var energy = force * Length<double>.FromMeters(1.0);
			var power = energy / Time<double>.FromSeconds(1.0);

			// ToString operations which might allocate
			var tempStr = temp.ToString();
			var forceStr = force.ToString();
		}

		long endMemory = GC.GetTotalMemory(false);
		long memoryUsed = endMemory - startMemory;

		Console.WriteLine($"Memory used: {memoryUsed / 1024.0:F1} KB for {IterationCount} operations");
		Console.WriteLine($"Average per operation: {(double)memoryUsed / IterationCount:F1} bytes");

		// This is informational - memory usage will vary
		Assert.IsTrue(memoryUsed < 10 * 1024 * 1024, "Memory usage should be reasonable (< 10MB)");
	}

	/// <summary>
	/// Benchmarks cross-domain calculations (ideal gas law).
	/// </summary>
	[TestMethod]
	public void BenchmarkCrossDomainCalculations()
	{
		// Warmup
		for (int i = 0; i < WarmupIterations; i++)
		{
			var p = Pressure<double>.FromPascals(101325.0);
			var v = Volume<double>.FromCubicMeters(0.0224);
			var t = Temperature<double>.FromKelvin(273.15);
			var r = PhysicalConstants.Generic.GasConstant<double>();
			var _ = (p.Value * v.Value) / (r * t.Value);
		}

		var stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Ideal gas law calculation combining multiple domains
			var pressure = Pressure<double>.FromPascals(101325.0 + i * 0.1);     // Mechanical
			var volume = Volume<double>.FromCubicMeters(0.0224 + i * 0.000001);  // Mechanical
			var temperature = Temperature<double>.FromKelvin(273.15 + i * 0.001); // Thermal
			var gasConstant = PhysicalConstants.Generic.GasConstant<double>();     // Chemical

			// PV = nRT calculation
			var moles = (pressure.Value * volume.Value) / (gasConstant * temperature.Value);

			// Convert to amount of substance
			var amount = AmountOfSubstance<double>.FromMoles(moles);
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Cross-domain calculations: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 100000, "Cross-domain calculations should be efficient");
	}
}
