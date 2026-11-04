// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Constants that are right and survive their siblings (#386, #392), logarithmic conversions with no
/// infinity to land on (#352), strictly positive overloads that stay positive (#354), and division by zero
/// that throws on every generated path (#480).
/// </summary>
[TestClass]
public sealed class PhysicsBugfixTests
{
	[TestMethod]
	public void AbsoluteZeroInCelsiusIsZeroKelvin()
	{
		double celsius = PhysicalConstants.Generic.AbsoluteZeroInCelsius<double>();
		Assert.AreEqual(-273.15, celsius, 1e-12);
		Assert.AreEqual(0.0, Temperature<double>.FromCelsius(celsius).Value, 1e-9);
	}

	[TestMethod]
	public void WaterIonProductIsKw()
	{
		Assert.AreEqual(1e-14, PhysicalConstants.Generic.WaterIonProduct<double>(), 1e-28);
	}

	[TestMethod]
	public void MolarVolumeStpIsInCubicMetresPerMole()
	{
		double expected = PhysicalConstants.Generic.GasConstant<double>() * 273.15 / 101325.0;
		Assert.AreEqual(expected, PhysicalConstants.Generic.MolarVolumeSTP<double>(), 1e-12);
	}

	[TestMethod]
	public void IntegerStorageReadsTheConstantsItCanHold()
	{
		Assert.AreEqual(299792458L, PhysicalConstants.Fundamental.SpeedOfLight<long>());
		Assert.AreEqual(101325, PhysicalConstants.ClassicalMechanics.StandardAtmosphericPressure<int>());
		Assert.AreEqual(7, PH<int>.Neutral.Value);
	}

	[TestMethod]
	public void AConstantTheStorageTypeCannotHoldFailsAlone()
	{
		Assert.ThrowsExactly<OverflowException>(() => PhysicalConstants.Fundamental.AvogadroNumber<long>());
		Assert.ThrowsExactly<ArithmeticException>(() => PhysicalConstants.ClassicalMechanics.StandardGravity<int>());
		Assert.AreEqual(299792458L, PhysicalConstants.Fundamental.SpeedOfLight<long>());
	}

	[TestMethod]
	public void DecimalRefusesAConstantItWouldRoundAway()
	{
		Assert.ThrowsExactly<ArithmeticException>(() => PhysicalConstants.Fundamental.PlanckConstant<decimal>());
		Assert.ThrowsExactly<ArithmeticException>(() => PhysicalConstants.NuclearPhysics.AtomicMassUnit<decimal>());
		Assert.AreEqual(299792458m, PhysicalConstants.Fundamental.SpeedOfLight<decimal>());
	}

	[TestMethod]
	public void ALinearZeroWithNoInfinityToHoldThrowsArgumentOutOfRange()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Gain<decimal>.Silence.ToDecibels());
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Gain<int>.Silence.ToDecibels());
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SoundPressureLevel<decimal>.FromSoundPressure(SoundPressure<decimal>.Zero));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PH<decimal>.FromHydrogenConcentration(Concentration<decimal>.Zero));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Semitones<decimal>.FromFrequencyRatio(Ratio<decimal>.Zero));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Decibels<decimal>.FromAmplitude(0m));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Decibels<decimal>.FromPower(0m));
	}

	[TestMethod]
	public void ALinearZeroIsNegativeInfinityWhereTheTypeHoldsIt()
	{
		Assert.AreEqual(double.NegativeInfinity, Gain<double>.Silence.ToDecibels().Value);
		Assert.AreEqual(float.NegativeInfinity, Gain<float>.Silence.ToDecibels().Value);
	}

	[TestMethod]
	public void ALevelTooHighForTheLinearTypeThrowsOverflow() =>
		Assert.ThrowsExactly<OverflowException>(() => Decibels<decimal>.Create(600m).ToAmplitude());

	[TestMethod]
	public void StrictlyPositiveOverloadsDoNotReachZero()
	{
		Wavelength<double> w = Wavelength<double>.FromMeter(1);
		Period<double> p = Period<double>.FromSecond(1);
		HalfLife<double> h = HalfLife<double>.FromSecond(1);

		Assert.ThrowsExactly<ArgumentException>(() => w * 0.0);
		Assert.ThrowsExactly<ArgumentException>(() => 0.0 * w);
		Assert.ThrowsExactly<ArgumentException>(() => w - w);
		Assert.ThrowsExactly<ArgumentException>(() => w / double.PositiveInfinity);
		Assert.ThrowsExactly<ArgumentException>(() => Wavelength<double>.From(Length<double>.Zero));
		Assert.ThrowsExactly<ArgumentException>(() => (Wavelength<double>)Length<double>.Zero);
		Assert.ThrowsExactly<ArgumentException>(() => p - p);
		Assert.ThrowsExactly<ArgumentException>(() => p * 0.0);
		Assert.ThrowsExactly<ArgumentException>(() => h - h);
		Assert.ThrowsExactly<ArgumentException>(() => HalfLife<double>.From(Duration<double>.Zero));

		Assert.AreEqual(2.0, (w * 2.0).Value);
		Assert.AreEqual(1.0, (Wavelength<double>.FromMeter(3) - Wavelength<double>.FromMeter(2)).Value);
	}

	[TestMethod]
	public void UnconstrainedOverloadsStillAcceptZero()
	{
		Assert.AreEqual(0.0, (Distance<double>.FromMeter(1) * 0.0).Value);
		Assert.AreEqual(0.0, Distance<double>.From(Length<double>.Zero).Value);
		Assert.AreEqual(0.0, (Length<double>.FromMeter(1) * 0.0).Value);
	}

	[TestMethod]
	public void DivisionByZeroThrowsOnEveryPath()
	{
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity1D<double>.FromMeterPerSecond(1) / 0.0);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<double>.FromMeterPerSecond(1, 2, 3) / 0.0);
		Assert.ThrowsExactly<DivideByZeroException>(() => Mass<double>.FromKilogram(1) / Volume<double>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Mass<double>.FromKilogram(0) / Volume<double>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity1D<double>.FromMeterPerSecond(1) / Duration<double>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<double>.FromMeterPerSecond(1, 2, 3) / Duration<double>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<double>.Zero.Normalize());
	}

	[TestMethod]
	public void DivisionByZeroThrowsTheSameWayForDecimal()
	{
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<decimal>.FromMeterPerSecond(1, 2, 3) / 0m);
		Assert.ThrowsExactly<DivideByZeroException>(() => Mass<decimal>.FromKilogram(1) / Volume<decimal>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<decimal>.FromMeterPerSecond(1, 2, 3) / Duration<decimal>.Zero);
		Assert.ThrowsExactly<DivideByZeroException>(() => Velocity3D<decimal>.Zero.Normalize());
	}
}
