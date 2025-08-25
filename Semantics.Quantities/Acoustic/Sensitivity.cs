// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Represents a sensitivity quantity with compile-time dimensional safety.
/// Sensitivity measures the efficiency of electroacoustic transducers.
/// </summary>
public sealed record Sensitivity<T> : PhysicalQuantity<Sensitivity<T>, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the physical dimension of sensitivity [1].</summary>
	public override PhysicalDimension Dimension => PhysicalDimensions.Sensitivity;

	/// <summary>
	/// Initializes a new instance of the Sensitivity class.
	/// </summary>
	public Sensitivity() : base() { }

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/W).
	/// </summary>
	/// <param name="dbSplPerWatt">The sensitivity in dB SPL/W.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity<T> FromDbSplPerWatt(T dbSplPerWatt) => Create(dbSplPerWatt);

	/// <summary>
	/// Creates a new Sensitivity from a value in dB (SPL/V).
	/// </summary>
	/// <param name="dbSplPerVolt">The sensitivity in dB SPL/V.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity<T> FromDbSplPerVolt(T dbSplPerVolt) => Create(dbSplPerVolt);

	/// <summary>
	/// Creates a new Sensitivity from a value in mV/Pa (microphone sensitivity).
	/// </summary>
	/// <param name="mvPerPa">The sensitivity in mV/Pa.</param>
	/// <returns>A new Sensitivity instance.</returns>
	public static Sensitivity<T> FromMvPerPa(T mvPerPa)
	{
		// Convert mV/Pa to dB re 1 V/Pa
		// dB = 20 * log10(mV/Pa / 1000)
		double dbValue = 20.0 * Math.Log10(double.CreateChecked(mvPerPa) / 1000.0);
		return Create(T.CreateChecked(dbValue));
	}

	/// <summary>
	/// Converts sensitivity to mV/Pa (for microphones).
	/// </summary>
	/// <returns>The sensitivity in mV/Pa.</returns>
	public T ToMvPerPa()
	{
		// Inverse conversion from dB re 1 V/Pa to mV/Pa
		double mvPerPa = 1000.0 * Math.Pow(10.0, double.CreateChecked(Value) / 20.0);
		return T.CreateChecked(mvPerPa);
	}

	/// <summary>
	/// Gets the sensitivity efficiency category.
	/// </summary>
	/// <returns>A string describing the efficiency level.</returns>
	public string GetEfficiencyCategory() => double.CreateChecked(Value) switch
	{
		< 80 => "Very Low Efficiency",
		< 85 => "Low Efficiency",
		< 90 => "Moderate Efficiency",
		< 95 => "High Efficiency",
		< 100 => "Very High Efficiency",
		_ => "Exceptional Efficiency"
	};

	/// <summary>
	/// Estimates power consumption for a target SPL.
	/// </summary>
	/// <param name="targetSpl">The target sound pressure level.</param>
	/// <returns>Estimated power consumption in watts.</returns>
	public T EstimatePowerConsumption(SoundPressureLevel<T> targetSpl)
	{
		ArgumentNullException.ThrowIfNull(targetSpl);

		// Power (dB) = Target SPL - Sensitivity
		// Power (W) = 10^(Power(dB)/10)
		T powerDb = targetSpl.Value - Value;
		double powerWatts = Math.Pow(10.0, double.CreateChecked(powerDb) / 10.0);
		return T.CreateChecked(powerWatts);
	}

	/// <summary>
	/// Calculates the maximum SPL at 1 meter for a given power input.
	/// </summary>
	/// <param name="inputPower">The input power.</param>
	/// <returns>The maximum SPL at 1 meter.</returns>
	public SoundPressureLevel<T> MaximumSplAt1m(Power<T> inputPower)
	{
		ArgumentNullException.ThrowIfNull(inputPower);

		// SPL = Sensitivity + 10*log10(Power)
		T powerDb = T.CreateChecked(10.0 * Math.Log10(double.CreateChecked(inputPower.Value)));
		T maxSpl = Value + powerDb;
		return SoundPressureLevel<T>.FromDecibels(maxSpl);
	}

	/// <summary>
	/// Gets the typical application based on sensitivity value.
	/// </summary>
	/// <returns>A string describing typical applications.</returns>
	public string GetTypicalApplication() => double.CreateChecked(Value) switch
	{
		< 82 => "High-power PA systems, subwoofers",
		< 87 => "Home audio, bookshelf speakers",
		< 92 => "Car audio, portable speakers",
		< 97 => "Headphones, efficient speakers",
		_ => "Horn-loaded, compression drivers"
	};
}
