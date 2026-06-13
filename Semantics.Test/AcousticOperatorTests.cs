// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class AcousticOperatorTests
{
	[TestMethod]
	public void Frequency_From_Speed_DividedBy_Wavelength()
	{
		// Semantic overloads widen implicitly to their base, so the
		// Speed / Length => Frequency operator applies to acoustic types.
		Speed<double> speed = SoundSpeed<double>.FromMeterPerSecond(343.0);
		Wavelength<double> wavelength = Wavelength<double>.Create(0.343);

		Frequency<double> f1 = speed / wavelength;
		Frequency<double> f2 = Frequency<double>.Create(speed.Value / wavelength.Value);

		Assert.AreEqual(f2.Value, f1.Value, 1e-12);
	}

	[TestMethod]
	public void Wavelength_From_Speed_DividedBy_Frequency()
	{
		Speed<double> speed = SoundSpeed<double>.FromMeterPerSecond(343.0);
		Frequency<double> frequency = Frequency<double>.FromHertz(1000.0);

		Length<double> wavelength = speed / frequency;

		Assert.AreEqual(0.343, wavelength.Value, 1e-12);
	}

	[TestMethod]
	public void Speed_From_Frequency_MultipliedBy_Wavelength()
	{
		Frequency<double> frequency = Frequency<double>.FromHertz(1000.0);
		Wavelength<double> wavelength = Wavelength<double>.Create(0.343);

		Speed<double> speed = frequency * wavelength;

		Assert.AreEqual(343.0, speed.Value, 1e-12);
	}
}
