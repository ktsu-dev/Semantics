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
			Temperature<double> temp = Temperature<double>.FromCelsius(25.0);
			double _ = temp.ToFahrenheit();
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		// Benchmark different unit conversions
		for (int i = 0; i < IterationCount; i++)
		{
			// Temperature conversions
			Temperature<double> temp = Temperature<double>.FromCelsius(25.0 + (i * 0.001));
			double fahrenheit = temp.ToFahrenheit();
			double kelvin = temp.ToKelvin();

			// Length conversions
			Length<double> length = Length<double>.FromMeters(1.0 + (i * 0.0001));
			double feet = length.Value * 3.28084; // Convert to feet
			double inches = length.Value * 39.3701; // Convert to inches

			// Energy conversions
			Energy<double> energy = Energy<double>.FromJoules(1000.0 + (i * 0.01));
			double calories = energy.Value / 4184.0; // Convert to calories
			double kwh = energy.Value / 3600000.0; // Convert to kWh
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount * 6 / stopwatch.Elapsed.TotalSeconds; // 6 conversions per iteration

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
			Force<double> force1 = Force<double>.FromNewtons(100.0);
			Force<double> force2 = Force<double>.FromNewtons(50.0);
			Force<double> _ = force1 + force2;
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Force arithmetic
			Force<double> force1 = Force<double>.FromNewtons(100.0 + (i * 0.001));
			Force<double> force2 = Force<double>.FromNewtons(50.0 + (i * 0.0005));
			Force<double> forceSum = force1 + force2;
			Force<double> forceDiff = force1 - force2;

			// Energy arithmetic
			Energy<double> energy1 = Energy<double>.FromJoules(1000.0 + (i * 0.01));
			Energy<double> energy2 = Energy<double>.FromJoules(500.0 + (i * 0.005));
			Energy<double> energySum = energy1 + energy2;
			Energy<double> energyDiff = energy1 - energy2;

			// Temperature arithmetic
			Temperature<double> temp1 = Temperature<double>.FromKelvin(300.0 + (i * 0.001));
			Temperature<double> temp2 = Temperature<double>.FromKelvin(273.15);
			Temperature<double> tempDiff = temp1 - temp2;
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount * 7 / stopwatch.Elapsed.TotalSeconds;

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
			Mass<double> mass = Mass<double>.FromKilograms(10.0);
			Acceleration<double> acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8);
			Force<double> _ = mass * acceleration;
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// F = ma
			Mass<double> mass = Mass<double>.FromKilograms(10.0 + (i * 0.0001));
			Acceleration<double> acceleration = Acceleration<double>.FromMetersPerSecondSquared(9.8 + (i * 0.00001));
			Force<double> force = mass * acceleration;

			// P = VI
			ElectricPotential<double> voltage = ElectricPotential<double>.FromVolts(12.0 + (i * 0.0001));
			ElectricCurrent<double> current = ElectricCurrent<double>.FromAmperes(2.0 + (i * 0.00001));
			Power<double> power = voltage * current;

			// E = mc² (simplified)
			Energy<double> energy = Energy<double>.FromKineticEnergy(mass, Velocity<double>.FromMetersPerSecond(10.0));

			// λ = v/f
			SoundSpeed<double> soundSpeed = SoundSpeed<double>.FromMetersPerSecond(343.0);
			Frequency<double> frequency = Frequency<double>.FromHertz(1000.0 + (i * 0.1));
			Wavelength<double> wavelength = soundSpeed / frequency;
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount * 4 / stopwatch.Elapsed.TotalSeconds;

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
			Temperature<double> _ = Temperature<double>.FromCelsius(25.0);
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Different creation methods
			Temperature<double> temp1 = Temperature<double>.FromCelsius(25.0 + (i * 0.001));
			Temperature<double> temp2 = Temperature<double>.FromKelvin(298.15 + (i * 0.001));
			Temperature<double> temp3 = Temperature<double>.FromFahrenheit(77.0 + (i * 0.002));

			Force<double> force1 = Force<double>.FromNewtons(100.0 + (i * 0.001));
			Force<double> force2 = Force<double>.FromNewtons((22.48 + (i * 0.0002)) * 4.448); // Convert pounds to newtons

			Energy<double> energy1 = Energy<double>.FromJoules(1000.0 + (i * 0.01));
			Energy<double> energy2 = Energy<double>.FromJoules((239.0 + (i * 0.002)) * 4184.0); // Convert calories to joules
			Energy<double> energy3 = Energy<double>.FromJoules((0.000278 + (i * 0.000001)) * 3600000.0); // Convert kWh to joules
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount * 8 / stopwatch.Elapsed.TotalSeconds;

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
			double _ = PhysicalConstants.Generic.SpeedOfLight<double>();
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Access various constants
			double c = PhysicalConstants.Generic.SpeedOfLight<double>();
			double g = PhysicalConstants.Generic.StandardGravity<double>();
			double R = PhysicalConstants.Generic.GasConstant<double>();
			double Na = PhysicalConstants.Generic.AvogadroNumber<double>();
			double h = PhysicalConstants.Generic.PlanckConstant<double>();

			// Use the values to prevent compiler warnings
			_ = c + g + R + Na + h;
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount * 5 / stopwatch.Elapsed.TotalSeconds;

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
			Temperature<double> temp = Temperature<double>.FromCelsius(25.0 + (i * 0.001));
			Force<double> force = Force<double>.FromNewtons(100.0 + (i * 0.001));
			Energy<double> energy = force * Length<double>.FromMeters(1.0);
			Power<double> power = energy / Time<double>.FromSeconds(1.0);

			// ToString operations which might allocate
			string tempStr = temp.ToString();
			string forceStr = force.ToString();
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
			Pressure<double> p = Pressure<double>.FromPascals(PhysicalConstants.Generic.StandardAtmosphericPressure<double>());
			Volume<double> v = Volume<double>.FromCubicMeters(0.0224);
			Temperature<double> t = Temperature<double>.FromKelvin(PhysicalConstants.Generic.StandardTemperature<double>());
			double r = PhysicalConstants.Generic.GasConstant<double>();
			double _ = p.Value * v.Value / (r * t.Value);
		}

		Stopwatch stopwatch = Stopwatch.StartNew();

		for (int i = 0; i < IterationCount; i++)
		{
			// Ideal gas law calculation combining multiple domains
			Pressure<double> pressure = Pressure<double>.FromPascals(PhysicalConstants.Generic.StandardAtmosphericPressure<double>() + (i * 0.1));     // Mechanical
			Volume<double> volume = Volume<double>.FromCubicMeters(0.0224 + (i * 0.000001));  // Mechanical
			Temperature<double> temperature = Temperature<double>.FromKelvin(PhysicalConstants.Generic.StandardTemperature<double>() + (i * 0.001)); // Thermal
			double gasConstant = PhysicalConstants.Generic.GasConstant<double>();     // Chemical

			// PV = nRT calculation
			double moles = pressure.Value * volume.Value / (gasConstant * temperature.Value);

			// Convert to amount of substance
			AmountOfSubstance<double> amount = AmountOfSubstance<double>.FromMoles(moles);
		}

		stopwatch.Stop();
		double operationsPerSecond = IterationCount / stopwatch.Elapsed.TotalSeconds;

		Console.WriteLine($"Cross-domain calculations: {operationsPerSecond:F0} operations/second");
		Assert.IsTrue(operationsPerSecond > 100000, "Cross-domain calculations should be efficient");
	}
}
