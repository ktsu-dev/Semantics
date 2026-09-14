// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Integer storage converts exactly as it did before factors were materialised per storage type.
/// </summary>
/// <remarks>
/// <para>
/// Before 5.2.0 every generated factory multiplied by <c>T.CreateChecked(double)</c> at the call, so
/// a factor that did not fit an integer threw <see cref="OverflowException"/> from that one factory
/// and every other factory kept working. 5.2.0 converted every factor for a storage type in one static
/// initializer, so a single factor too large for <see cref="int"/> made every conversion for
/// <see cref="int"/> throw <see cref="TypeInitializationException"/>.
/// </para>
/// <para>
/// Each expected value is the expression the 5.1 generated code evaluated, with the <see cref="double"/>
/// constants it declared, so these tests compare against that code rather than against a restatement of it.
/// </para>
/// </remarks>
[TestClass]
public sealed class IntegerStorageConversionTests
{
	// The constants as the 5.1 ConversionConstants.g.cs declared them.
	private const double FeetToMeters = 0.3048;
	private const double MileToMeters = 1609.344;
	private const double HourToSeconds = 3600;
	private const double CelsiusToKelvinOffset = 273.15;
	private const double FahrenheitScale = 0.5555555555555556;
	private const double FahrenheitToKelvinOffset = 255.37222222222223;
	private const double CurieToBecquerels = 3.7e10;

	/// <summary>
	/// A factor too large for the storage type throws <see cref="OverflowException"/> from its own factory,
	/// and the factories around it still convert.
	/// </summary>
	[TestMethod]
	public void AnOverflowingFactorFailsOnlyItsOwnFactory()
	{
		Assert.ThrowsExactly<OverflowException>(static () => int.CreateChecked(CurieToBecquerels));
		Assert.ThrowsExactly<OverflowException>(static () => RadioactiveActivity<int>.FromCurie(1));
		Assert.ThrowsExactly<OverflowException>(static () => RadioactiveActivity<int>.FromCurie(1));

		Assert.AreEqual(1 * int.CreateChecked(MetricMagnitudes.Kilo), Length<int>.FromKilometer(1).Value);
		Assert.AreEqual(5000 * int.CreateChecked(MileToMeters), Length<int>.FromMile(5000).Value);
		Assert.AreEqual(2 * int.CreateChecked(HourToSeconds), Duration<int>.FromHour(2).Value);
	}

	/// <summary>
	/// A metric magnitude too large for the storage type fails when it is read, and the smaller ones still read.
	/// </summary>
	[TestMethod]
	public void AnOverflowingMagnitudeFailsOnlyWhereItIsRead()
	{
		Assert.ThrowsExactly<OverflowException>(static () => MetricMagnitudes.Values<int>.Tera);
		Assert.ThrowsExactly<OverflowException>(static () => MetricMagnitudes.Values<long>.Yotta);

		Assert.AreEqual(1000, MetricMagnitudes.Values<int>.Kilo);
		Assert.AreEqual(1000000000000L, MetricMagnitudes.Values<long>.Tera);
		Assert.AreEqual(1 * long.CreateChecked(MetricMagnitudes.Kilo), Length<long>.FromKilometer(1L).Value);
	}

	/// <summary>
	/// <see cref="int"/> factories give what the 5.1 expressions gave, including the truncation of a fractional factor.
	/// </summary>
	[TestMethod]
	public void IntFactoriesMatchTheExpressionsTheyReplaced()
	{
		Assert.AreEqual(1000, Length<int>.FromKilometer(1).Value);
		Assert.AreEqual(36 * int.CreateChecked(MetricMagnitudes.Kilo), Length<int>.FromKilometer(36).Value);
		Assert.AreEqual(250 * int.CreateChecked(MetricMagnitudes.Centi), Length<int>.FromCentimeter(250).Value);
		Assert.AreEqual(10 * int.CreateChecked(FeetToMeters), Length<int>.FromFoot(10).Value);
		Assert.AreEqual(20 + int.CreateChecked(CelsiusToKelvinOffset), Temperature<int>.FromCelsius(20).Value);
		Assert.AreEqual(
			(212 * int.CreateChecked(FahrenheitScale)) + int.CreateChecked(FahrenheitToKelvinOffset),
			Temperature<int>.FromFahrenheit(212).Value);
	}

	/// <summary>
	/// <see cref="long"/> factories give what the 5.1 expressions gave, including a factor too large for <see cref="int"/>.
	/// </summary>
	[TestMethod]
	public void LongFactoriesMatchTheExpressionsTheyReplaced()
	{
		Assert.AreEqual(36L * long.CreateChecked(MetricMagnitudes.Kilo), Length<long>.FromKilometer(36L).Value);
		Assert.AreEqual(10L * long.CreateChecked(FeetToMeters), Length<long>.FromFoot(10L).Value);
		Assert.AreEqual(3L * long.CreateChecked(CurieToBecquerels), RadioactiveActivity<long>.FromCurie(3L).Value);
		Assert.AreEqual(
			(212L * long.CreateChecked(FahrenheitScale)) + long.CreateChecked(FahrenheitToKelvinOffset),
			Temperature<long>.FromFahrenheit(212L).Value);
	}

	/// <summary>
	/// <c>In(unit)</c> on integer storage matches the 5.1 default <c>IUnit.FromBase</c>, which divided by the
	/// truncated factor, including the <see cref="DivideByZeroException"/> a fractional scale truncated to zero gave.
	/// </summary>
	[TestMethod]
	public void InMatchesTheDefaultFromBaseItReplaced()
	{
		Assert.AreEqual((5000 - int.CreateChecked(0d)) / int.CreateChecked(MetricMagnitudes.Kilo), Length<int>.FromMeter(5000).In(Units.Kilometer));
		Assert.AreEqual((7200L - long.CreateChecked(0d)) / long.CreateChecked(HourToSeconds), Duration<long>.FromSecond(7200L).In(Units.Hour));
		Assert.ThrowsExactly<DivideByZeroException>(static () => Temperature<int>.FromKelvin(300).In(Units.Fahrenheit));
	}
}
