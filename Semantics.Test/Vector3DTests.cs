// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;
using ktsu.Semantics.Float;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Tests for the 3D vector quantities.
/// </summary>
[TestClass]
public static class Vector3DTests
{
	private const float FloatTolerance = 1e-6f;
	private const double DoubleTolerance = 1e-10;

	[TestClass]
	public class Float3DVectorTests
	{
		[TestMethod]
		public void Position3D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Float.Position3D position = Float.Position3D.FromMeters(10.5f, 20.3f, 5.7f);

			// Assert
			Assert.AreEqual(10.5f, position.X, FloatTolerance);
			Assert.AreEqual(20.3f, position.Y, FloatTolerance);
			Assert.AreEqual(5.7f, position.Z, FloatTolerance);
			Assert.IsTrue(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Displacement3D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Float.Displacement3D displacement = Float.Displacement3D.FromMeters(5.0f, -3.0f, 2.0f);

			// Assert
			Assert.AreEqual(5.0f, displacement.X, FloatTolerance);
			Assert.AreEqual(-3.0f, displacement.Y, FloatTolerance);
			Assert.AreEqual(2.0f, displacement.Z, FloatTolerance);
			Assert.AreEqual(Math.Sqrt(38), displacement.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Velocity3D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Float.Velocity3D velocity = Float.Velocity3D.FromMetersPerSecond(15.0f, 8.0f, 6.0f);

			// Assert
			Assert.AreEqual(15.0f, velocity.X, FloatTolerance);
			Assert.AreEqual(8.0f, velocity.Y, FloatTolerance);
			Assert.AreEqual(6.0f, velocity.Z, FloatTolerance);
			Assert.AreEqual(19.0f, velocity.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Acceleration3D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Float.Acceleration3D acceleration = Float.Acceleration3D.FromMetersPerSecondSquared(9.8f, 0.0f, -2.5f);

			// Assert
			Assert.AreEqual(9.8f, acceleration.X, FloatTolerance);
			Assert.AreEqual(0.0f, acceleration.Y, FloatTolerance);
			Assert.AreEqual(-2.5f, acceleration.Z, FloatTolerance);
			Assert.AreEqual(Math.Sqrt(102.29), acceleration.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Force3D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Float.Force3D force = Float.Force3D.FromNewtons(100.0f, 50.0f, 25.0f);

			// Assert
			Assert.AreEqual(100.0f, force.X, FloatTolerance);
			Assert.AreEqual(50.0f, force.Y, FloatTolerance);
			Assert.AreEqual(25.0f, force.Z, FloatTolerance);
			Assert.AreEqual(Math.Sqrt(11875), force.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Vector3D_Float_ArithmeticOperations_ShouldWork()
		{
			// Arrange
			Float.Position3D position1 = Float.Position3D.FromMeters(10.0f, 20.0f, 30.0f);
			Float.Displacement3D displacement = Float.Displacement3D.FromMeters(5.0f, -3.0f, 2.0f);

			// Act
			Float.Position3D position2 = position1 + displacement;

			// Assert
			Assert.AreEqual(15.0f, position2.X, FloatTolerance);
			Assert.AreEqual(17.0f, position2.Y, FloatTolerance);
			Assert.AreEqual(32.0f, position2.Z, FloatTolerance);
		}

		[TestMethod]
		public void Vector3D_Float_CrossProduct_ShouldWork()
		{
			// Arrange
			Float.Force3D vector1 = Float.Force3D.FromNewtons(1.0f, 0.0f, 0.0f);
			Float.Force3D vector2 = Float.Force3D.FromNewtons(0.0f, 1.0f, 0.0f);

			// Act
			Float.Force3D crossProduct = vector1.Cross(vector2);

			// Assert
			Assert.AreEqual(0.0f, crossProduct.X, FloatTolerance);
			Assert.AreEqual(0.0f, crossProduct.Y, FloatTolerance);
			Assert.AreEqual(1.0f, crossProduct.Z, FloatTolerance);
		}

		[TestMethod]
		public void Vector3D_Float_DotProduct_ShouldWork()
		{
			// Arrange
			Float.Velocity3D velocity1 = Float.Velocity3D.FromMetersPerSecond(2.0f, 3.0f, 4.0f);
			Float.Velocity3D velocity2 = Float.Velocity3D.FromMetersPerSecond(5.0f, 6.0f, 7.0f);

			// Act
			float dotProduct = velocity1.Dot(velocity2);

			// Assert
			Assert.AreEqual(56.0f, dotProduct, FloatTolerance); // 2*5 + 3*6 + 4*7 = 56
		}
	}

	[TestClass]
	public class Double3DVectorTests
	{
		[TestMethod]
		public void Position3D_Double_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			Double.Position3D position = Double.Position3D.FromMeters(10.123456789, 20.987654321, 5.555555555);

			// Assert
			Assert.AreEqual(10.123456789, position.X, DoubleTolerance);
			Assert.AreEqual(20.987654321, position.Y, DoubleTolerance);
			Assert.AreEqual(5.555555555, position.Z, DoubleTolerance);
			Assert.IsTrue(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector3D_Double_HighPrecision_ShouldMaintainAccuracy()
		{
			// Arrange - Use high precision values that would lose accuracy in float
			Double.Position3D position1 = Double.Position3D.FromMeters(Math.PI * 1e10, Math.E * 1e10, Math.Sqrt(2) * 1e10);
			Double.Displacement3D displacement = Double.Displacement3D.FromMeters(Math.Sqrt(3), Math.Sqrt(5), Math.Sqrt(7));

			// Act
			Double.Position3D position2 = position1 + displacement;

			// Assert
			Assert.AreEqual((Math.PI * 1e10) + Math.Sqrt(3), position2.X, DoubleTolerance);
			Assert.AreEqual((Math.E * 1e10) + Math.Sqrt(5), position2.Y, DoubleTolerance);
			Assert.AreEqual((Math.Sqrt(2) * 1e10) + Math.Sqrt(7), position2.Z, DoubleTolerance);
		}

		[TestMethod]
		public void Vector3D_Double_Distance_ShouldCalculateCorrectly()
		{
			// Arrange
			Double.Position3D position1 = Double.Position3D.FromMeters(0.0, 0.0, 0.0);
			Double.Position3D position2 = Double.Position3D.FromMeters(3.0, 4.0, 12.0);

			// Act
			double distance = position1.Distance(position2);

			// Assert
			Assert.AreEqual(13.0, distance, DoubleTolerance); // sqrt(3^2 + 4^2 + 12^2) = 13
		}

		[TestMethod]
		public void Vector3D_Double_DotProduct_ShouldCalculateCorrectly()
		{
			// Arrange
			Double.Velocity3D velocity1 = Double.Velocity3D.FromMetersPerSecond(2.0, 3.0, 4.0);
			Double.Velocity3D velocity2 = Double.Velocity3D.FromMetersPerSecond(5.0, 6.0, 7.0);

			// Act
			double dotProduct = velocity1.Dot(velocity2);

			// Assert
			Assert.AreEqual(56.0, dotProduct, DoubleTolerance); // 2*5 + 3*6 + 4*7 = 56
		}

		[TestMethod]
		public void Vector3D_Double_CrossProduct_ShouldCalculateCorrectly()
		{
			// Arrange
			Double.Force3D force1 = Double.Force3D.FromNewtons(1.0, 2.0, 3.0);
			Double.Force3D force2 = Double.Force3D.FromNewtons(4.0, 5.0, 6.0);

			// Act
			Double.Force3D crossProduct = force1.Cross(force2);

			// Assert
			// Cross product of (1,2,3) x (4,5,6) = (2*6-3*5, 3*4-1*6, 1*5-2*4) = (-3, 6, -3)
			Assert.AreEqual(-3.0, crossProduct.X, DoubleTolerance);
			Assert.AreEqual(6.0, crossProduct.Y, DoubleTolerance);
			Assert.AreEqual(-3.0, crossProduct.Z, DoubleTolerance);
		}
	}

	[TestClass]
	public class Vector3D_ValidationTests
	{
		[TestMethod]
		public void Vector3D_NaNValues_ShouldBeInvalid()
		{
			// Arrange
			Float.Position3D position = Float.Position3D.Create(new Vector3f(float.NaN, 5.0f, 3.0f));

			// Assert
			Assert.IsFalse(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector3D_InfiniteValues_ShouldBeInvalid()
		{
			// Arrange
			Float.Position3D position = Float.Position3D.Create(new Vector3f(float.PositiveInfinity, 5.0f, 3.0f));

			// Assert
			Assert.IsFalse(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector3D_ValidValues_ShouldBeValid()
		{
			// Arrange
			Float.Position3D position = Float.Position3D.Create(new Vector3f(10.0f, 20.0f, 30.0f));

			// Assert
			Assert.IsTrue(position.IsPhysicallyValid);
		}
	}

	[TestClass]
	public class Vector3D_UnitTests
	{
		[TestMethod]
		public void Vector3D_Unit_ShouldReturnNormalizedVector()
		{
			// Arrange
			Float.Velocity3D velocity = Float.Velocity3D.FromMetersPerSecond(3.0f, 4.0f, 12.0f);

			// Act
			Float.Velocity3D unitVelocity = velocity.Unit();

			// Assert
			Assert.AreEqual(1.0f, unitVelocity.Magnitude, FloatTolerance);
			Assert.AreEqual(3.0f / 13.0f, unitVelocity.X, FloatTolerance);
			Assert.AreEqual(4.0f / 13.0f, unitVelocity.Y, FloatTolerance);
			Assert.AreEqual(12.0f / 13.0f, unitVelocity.Z, FloatTolerance);
		}

		[TestMethod]
		public void Vector3D_ZeroMagnitude_Unit_ShouldReturnZero()
		{
			// Arrange
			Float.Velocity3D zeroVelocity = Float.Velocity3D.FromMetersPerSecond(0.0f, 0.0f, 0.0f);

			// Act
			Float.Velocity3D unitVelocity = zeroVelocity.Unit();

			// Assert
			Assert.AreEqual(0.0f, unitVelocity.Magnitude, FloatTolerance);
			Assert.AreEqual(0.0f, unitVelocity.X, FloatTolerance);
			Assert.AreEqual(0.0f, unitVelocity.Y, FloatTolerance);
			Assert.AreEqual(0.0f, unitVelocity.Z, FloatTolerance);
		}
	}

	[TestClass]
	public class Vector3D_ConstantsTests
	{
		[TestMethod]
		public void Acceleration3D_StandardGravity_ShouldHaveCorrectValue()
		{
			// Arrange & Act
			Float.Acceleration3D gravity = Float.Acceleration3D.StandardGravity;

			// Assert
			Assert.AreEqual(0.0f, gravity.X, FloatTolerance);
			Assert.AreEqual(0.0f, gravity.Y, FloatTolerance);
			Assert.AreEqual(-9.80665f, gravity.Z, FloatTolerance);
		}

		[TestMethod]
		public void Position3D_Origin_ShouldHaveZeroValues()
		{
			// Arrange & Act
			Float.Position3D origin = Float.Position3D.Origin;

			// Assert
			Assert.AreEqual(0.0f, origin.X, FloatTolerance);
			Assert.AreEqual(0.0f, origin.Y, FloatTolerance);
			Assert.AreEqual(0.0f, origin.Z, FloatTolerance);
		}
	}
}
