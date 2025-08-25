// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class SoundSpeedTests
{
	private const double Tolerance = 1e-10;

	[TestMethod]
	public void FromFeetPerSecond_ShouldConvertToMetersPerSecond()
	{
		// 1 ft/s == 0.3048 m/s
		SoundSpeed<double> fromFps = SoundSpeed<double>.FromFeetPerSecond(1.0);
		Assert.AreEqual(0.3048, fromFps.Value, Tolerance);
	}

	[TestMethod]
	public void Multiply_Wavelength_And_Frequency_ShouldYieldSpeed()
	{
		Wavelength<double> wavelength = Wavelength<double>.Create(0.343); // 343 m/s at 1kHz
		Frequency<double> frequency = Frequency<double>.Create(1000.0);

		SoundSpeed<double> speed1 = SoundSpeed<double>.Multiply(wavelength, frequency);
		SoundSpeed<double> speed2 = wavelength * frequency; // operator path
		SoundSpeed<double> speed3 = frequency * wavelength; // commutative operator

		Assert.AreEqual(343.0, speed1.Value, 1e-9);
		Assert.AreEqual(speed1.Value, speed2.Value, Tolerance);
		Assert.AreEqual(speed1.Value, speed3.Value, Tolerance);
	}
}

