// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Issue #456: a semantic overload on the left of a relationship operator reaches its base's operator.
/// Each of these failed to compile with CS0019 before, so compiling is half of what is tested.
/// </summary>
[TestClass]
public sealed class OverloadRelationshipOperatorTests
{
	private const double Tolerance = 1e-12;

	[TestMethod]
	public void DistanceOverDuration_IsASpeed()
	{
		Speed<double> speed = Distance<double>.FromMeter(100) / Duration<double>.FromSecond(10);
		Assert.AreEqual(10.0, speed.Value, Tolerance);
	}

	[TestMethod]
	public void WidthTimesWidth_IsAnArea()
	{
		Width<double> width = Width<double>.FromMeter(2);
		Area<double> area = width * width;
		Assert.AreEqual(4.0, area.Value, Tolerance);
	}

	[TestMethod]
	public void WidthTimesHeight_IsAnArea()
	{
		Area<double> area = Width<double>.FromMeter(2) * Height<double>.FromMeter(3);
		Assert.AreEqual(6.0, area.Value, Tolerance);
	}

	[TestMethod]
	public void WeightTimesHeight_IsAnEnergy()
	{
		Energy<double> energy = Weight<double>.FromNewton(9.8) * Height<double>.FromMeter(10);
		Assert.AreEqual(98.0, energy.Value, 1e-9);
	}

	[TestMethod]
	public void OperandOrder_NoLongerDecidesWhetherItCompiles()
	{
		Width<double> width = Width<double>.FromMeter(2);
		ForceMagnitude<double> force = ForceMagnitude<double>.FromNewton(3);

		Energy<double> leftFirst = width * force;
		Energy<double> rightFirst = force * width;

		Assert.AreEqual(rightFirst.Value, leftFirst.Value, Tolerance);
		Assert.AreEqual(6.0, leftFirst.Value, Tolerance);
	}

	[TestMethod]
	public void WindVelocityTimesDuration_IsADisplacement()
	{
		Displacement3D<double> displacement = WindVelocity3D<double>.FromMeterPerSecond(1, 2, 3) * Duration<double>.FromSecond(2);

		Assert.AreEqual(2.0, displacement.X, Tolerance);
		Assert.AreEqual(4.0, displacement.Y, Tolerance);
		Assert.AreEqual(6.0, displacement.Z, Tolerance);
	}
}
