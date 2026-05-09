// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class AcousticOperatorTests
{
	[TestMethod]
	public void Frequency_From_Speed_DividedBy_Wavelength()
	{
		SoundSpeed<double> speed = SoundSpeed<double>.FromMetersPerSecond(343.0);
		Wavelength<double> wavelength = Wavelength<double>.Create(0.343);

		Frequency<double> f1 = speed / wavelength; // operator on Wavelength
		Frequency<double> f2 = Frequency<double>.Create(speed.Value / wavelength.Value);

		Assert.AreEqual(f2.Value, f1.Value, 1e-12);
	}
}

