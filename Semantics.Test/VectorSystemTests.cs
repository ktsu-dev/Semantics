// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Test;

using ktsu.Semantics.Double;
using ktsu.Semantics.Float;
using ktsu.Semantics.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

/// <summary>
/// Tests for the sophisticated vector system including interfaces, types, and 2D quantities.
/// </summary>
[TestClass]
public class VectorSystemTests
{
	private const float FloatTolerance = 1e-6f;
	private const double DoubleTolerance = 1e-10;

	[TestClass]
	public class VectorInterfaceTests
	{
		[TestMethod]
		public void Vector2f_ImplementsIVector2Interface()
		{
			// Arrange & Act
			var vector = new ktsu.Semantics.Float.Vector2f(3.0f, 4.0f);

			// Assert
			Assert.IsTrue(vector is IVector2<ktsu.Semantics.Float.Vector2f, float>);
			Assert.AreEqual(3.0f, vector.X, FloatTolerance);
			Assert.AreEqual(4.0f, vector.Y, FloatTolerance);
		}

		[TestMethod]
		public void Vector2d_ImplementsIVector2Interface()
		{
			// Arrange & Act
			var vector = new ktsu.Semantics.Double.Vector2d(3.14159265359, 2.71828182846);

			// Assert
			Assert.IsTrue(vector is IVector2<ktsu.Semantics.Double.Vector2d, double>);
			Assert.AreEqual(3.14159265359, vector.X, DoubleTolerance);
			Assert.AreEqual(2.71828182846, vector.Y, DoubleTolerance);
		}

		[TestMethod]
		public void Vector3f_ImplementsIVector3Interface()
		{
			// Arrange & Act
			var vector = new ktsu.Semantics.Float.Vector3f(1.0f, 2.0f, 3.0f);

			// Assert
			Assert.IsTrue(vector is IVector3<ktsu.Semantics.Float.Vector3f, float>);
			Assert.AreEqual(1.0f, vector.X, FloatTolerance);
			Assert.AreEqual(2.0f, vector.Y, FloatTolerance);
			Assert.AreEqual(3.0f, vector.Z, FloatTolerance);
		}

		[TestMethod]
		public void Vector3d_ImplementsIVector3Interface()
		{
			// Arrange & Act
			var vector = new ktsu.Semantics.Double.Vector3d(1.23456, 2.34567, 3.45678);

			// Assert
			Assert.IsTrue(vector is IVector3<ktsu.Semantics.Double.Vector3d, double>);
			Assert.AreEqual(1.23456, vector.X, DoubleTolerance);
			Assert.AreEqual(2.34567, vector.Y, DoubleTolerance);
			Assert.AreEqual(3.45678, vector.Z, DoubleTolerance);
		}
	}

	[TestClass]
	public class Vector2fTests
	{
		[TestMethod]
		public void Vector2f_StaticProperties_ShouldHaveCorrectValues()
		{
			// Assert
			Assert.AreEqual(0.0f, ktsu.Semantics.Float.Vector2f.Zero.X, FloatTolerance);
			Assert.AreEqual(0.0f, ktsu.Semantics.Float.Vector2f.Zero.Y, FloatTolerance);

			Assert.AreEqual(1.0f, ktsu.Semantics.Float.Vector2f.One.X, FloatTolerance);
			Assert.AreEqual(1.0f, ktsu.Semantics.Float.Vector2f.One.Y, FloatTolerance);

			Assert.AreEqual(1.0f, ktsu.Semantics.Float.Vector2f.UnitX.X, FloatTolerance);
			Assert.AreEqual(0.0f, ktsu.Semantics.Float.Vector2f.UnitX.Y, FloatTolerance);

			Assert.AreEqual(0.0f, ktsu.Semantics.Float.Vector2f.UnitY.X, FloatTolerance);
			Assert.AreEqual(1.0f, ktsu.Semantics.Float.Vector2f.UnitY.Y, FloatTolerance);
		}

		[TestMethod]
		public void Vector2f_Length_ShouldCalculateCorrectly()
		{
			// Arrange
			var vector = new ktsu.Semantics.Float.Vector2f(3.0f, 4.0f);

			// Act & Assert
			Assert.AreEqual(5.0f, vector.Length(), FloatTolerance);
		}

		[TestMethod]
		public void Vector2f_Normalize_ShouldCreateUnitVector()
		{
			// Arrange
			var vector = new ktsu.Semantics.Float.Vector2f(3.0f, 4.0f);

			// Act
			var normalized = vector.Normalize();

			// Assert
			Assert.AreEqual(1.0f, normalized.Length(), FloatTolerance);
			Assert.AreEqual(0.6f, normalized.X, FloatTolerance);
			Assert.AreEqual(0.8f, normalized.Y, FloatTolerance);
		}

		[TestMethod]
		public void Vector2f_DotProduct_ShouldCalculateCorrectly()
		{
			// Arrange
			var vector1 = new ktsu.Semantics.Float.Vector2f(2.0f, 3.0f);
			var vector2 = new ktsu.Semantics.Float.Vector2f(4.0f, 5.0f);

			// Act
			float dotProduct = vector1.Dot(vector2);

			// Assert
			Assert.AreEqual(23.0f, dotProduct, FloatTolerance); // 2*4 + 3*5 = 23
		}

		[TestMethod]
		public void Vector2f_ArithmeticOperations_ShouldWork()
		{
			// Arrange
			var vector1 = new ktsu.Semantics.Float.Vector2f(2.0f, 3.0f);
			var vector2 = new ktsu.Semantics.Float.Vector2f(1.0f, 4.0f);

			// Act & Assert
			var sum = vector1 + vector2;
			Assert.AreEqual(3.0f, sum.X, FloatTolerance);
			Assert.AreEqual(7.0f, sum.Y, FloatTolerance);

			var difference = vector1 - vector2;
			Assert.AreEqual(1.0f, difference.X, FloatTolerance);
			Assert.AreEqual(-1.0f, difference.Y, FloatTolerance);

			var scaled = vector1 * 2.0f;
			Assert.AreEqual(4.0f, scaled.X, FloatTolerance);
			Assert.AreEqual(6.0f, scaled.Y, FloatTolerance);
		}
	}

	[TestClass]
	public class Vector2dTests
	{
		[TestMethod]
		public void Vector2d_StaticProperties_ShouldHaveCorrectValues()
		{
			// Assert
			Assert.AreEqual(0.0, ktsu.Semantics.Double.Vector2d.Zero.X, DoubleTolerance);
			Assert.AreEqual(0.0, ktsu.Semantics.Double.Vector2d.Zero.Y, DoubleTolerance);

			Assert.AreEqual(1.0, ktsu.Semantics.Double.Vector2d.One.X, DoubleTolerance);
			Assert.AreEqual(1.0, ktsu.Semantics.Double.Vector2d.One.Y, DoubleTolerance);
		}

		[TestMethod]
		public void Vector2d_HighPrecisionOperations_ShouldMaintainPrecision()
		{
			// Arrange - Use high precision values
			var vector1 = new ktsu.Semantics.Double.Vector2d(Math.PI, Math.E);
			var vector2 = new ktsu.Semantics.Double.Vector2d(Math.Sqrt(2), Math.Sqrt(3));

			// Act
			double dotProduct = vector1.Dot(vector2);
			var sum = vector1 + vector2;

			// Assert
			double expectedDot = Math.PI * Math.Sqrt(2) + Math.E * Math.Sqrt(3);
			Assert.AreEqual(expectedDot, dotProduct, DoubleTolerance);
			
			Assert.AreEqual(Math.PI + Math.Sqrt(2), sum.X, DoubleTolerance);
			Assert.AreEqual(Math.E + Math.Sqrt(3), sum.Y, DoubleTolerance);
		}
	}

	[TestClass]
	public class Vector2D_QuantitiesFloatTests
	{
		[TestMethod]
		public void Position2D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var position = ktsu.Semantics.Float.Position2D.FromMeters(10.5f, 20.3f);

			// Assert
			Assert.AreEqual(10.5f, position.X, FloatTolerance);
			Assert.AreEqual(20.3f, position.Y, FloatTolerance);
			Assert.IsTrue(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Displacement2D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var displacement = ktsu.Semantics.Float.Displacement2D.FromMeters(5.0f, -3.0f);

			// Assert
			Assert.AreEqual(5.0f, displacement.X, FloatTolerance);
			Assert.AreEqual(-3.0f, displacement.Y, FloatTolerance);
			Assert.AreEqual(Math.Sqrt(34), displacement.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Velocity2D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var velocity = ktsu.Semantics.Float.Velocity2D.FromMetersPerSecond(15.0f, 8.0f);

			// Assert
			Assert.AreEqual(15.0f, velocity.X, FloatTolerance);
			Assert.AreEqual(8.0f, velocity.Y, FloatTolerance);
			Assert.AreEqual(17.0f, velocity.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Acceleration2D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var acceleration = ktsu.Semantics.Float.Acceleration2D.FromMetersPerSecondSquared(9.8f, 0.0f);

			// Assert
			Assert.AreEqual(9.8f, acceleration.X, FloatTolerance);
			Assert.AreEqual(0.0f, acceleration.Y, FloatTolerance);
			Assert.AreEqual(9.8f, acceleration.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Force2D_Float_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var force = ktsu.Semantics.Float.Force2D.FromNewtons(100.0f, 50.0f);

			// Assert
			Assert.AreEqual(100.0f, force.X, FloatTolerance);
			Assert.AreEqual(50.0f, force.Y, FloatTolerance);
			Assert.AreEqual(Math.Sqrt(12500), force.Magnitude, FloatTolerance);
		}

		[TestMethod]
		public void Vector2D_Float_ArithmeticOperations_ShouldWork()
		{
			// Arrange
			var position1 = ktsu.Semantics.Float.Position2D.FromMeters(10.0f, 20.0f);
			var displacement = ktsu.Semantics.Float.Displacement2D.FromMeters(5.0f, -3.0f);

			// Act
			var position2 = position1 + displacement;

			// Assert
			Assert.AreEqual(15.0f, position2.X, FloatTolerance);
			Assert.AreEqual(17.0f, position2.Y, FloatTolerance);
		}
	}

	[TestClass]
	public class Vector2D_QuantitiesDoubleTests
	{
		[TestMethod]
		public void Position2D_Double_CreateAndManipulate_ShouldWork()
		{
			// Arrange & Act
			var position = ktsu.Semantics.Double.Position2D.FromMeters(10.123456789, 20.987654321);

			// Assert
			Assert.AreEqual(10.123456789, position.X, DoubleTolerance);
			Assert.AreEqual(20.987654321, position.Y, DoubleTolerance);
			Assert.IsTrue(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector2D_Double_HighPrecision_ShouldMaintainAccuracy()
		{
			// Arrange - Use high precision values that would lose accuracy in float
			var position1 = ktsu.Semantics.Double.Position2D.FromMeters(Math.PI * 1e10, Math.E * 1e10);
			var displacement = ktsu.Semantics.Double.Displacement2D.FromMeters(Math.Sqrt(2), Math.Sqrt(3));

			// Act
			var position2 = position1 + displacement;

			// Assert
			Assert.AreEqual(Math.PI * 1e10 + Math.Sqrt(2), position2.X, DoubleTolerance);
			Assert.AreEqual(Math.E * 1e10 + Math.Sqrt(3), position2.Y, DoubleTolerance);
		}

		[TestMethod]
		public void Vector2D_Double_Distance_ShouldCalculateCorrectly()
		{
			// Arrange
			var position1 = ktsu.Semantics.Double.Position2D.FromMeters(0.0, 0.0);
			var position2 = ktsu.Semantics.Double.Position2D.FromMeters(3.0, 4.0);

			// Act
			double distance = position1.Distance(position2);

			// Assert
			Assert.AreEqual(5.0, distance, DoubleTolerance);
		}

		[TestMethod]
		public void Vector2D_Double_DotProduct_ShouldCalculateCorrectly()
		{
			// Arrange
			var velocity1 = ktsu.Semantics.Double.Velocity2D.FromMetersPerSecond(2.0, 3.0);
			var velocity2 = ktsu.Semantics.Double.Velocity2D.FromMetersPerSecond(4.0, 5.0);

			// Act
			double dotProduct = velocity1.Dot(velocity2);

			// Assert
			Assert.AreEqual(23.0, dotProduct, DoubleTolerance); // 2*4 + 3*5 = 23
		}
	}

	[TestClass]
	public class VectorQuantityValidationTests
	{
		[TestMethod]
		public void Vector2D_NaNValues_ShouldBeInvalid()
		{
			// Arrange
			var vector = new ktsu.Semantics.Float.Vector2f(float.NaN, 5.0f);
			var position = ktsu.Semantics.Float.Position2D.Create(vector);

			// Assert
			Assert.IsFalse(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector2D_InfiniteValues_ShouldBeInvalid()
		{
			// Arrange
			var vector = new ktsu.Semantics.Float.Vector2f(float.PositiveInfinity, 5.0f);
			var position = ktsu.Semantics.Float.Position2D.Create(vector);

			// Assert
			Assert.IsFalse(position.IsPhysicallyValid);
		}

		[TestMethod]
		public void Vector2D_ValidValues_ShouldBeValid()
		{
			// Arrange
			var vector = new ktsu.Semantics.Float.Vector2f(10.0f, 20.0f);
			var position = ktsu.Semantics.Float.Position2D.Create(vector);

			// Assert
			Assert.IsTrue(position.IsPhysicallyValid);
		}
	}

	[TestClass]
	public class VectorUnitTests
	{
		[TestMethod]
		public void Vector2D_Unit_ShouldReturnNormalizedVector()
		{
			// Arrange
			var velocity = ktsu.Semantics.Float.Velocity2D.FromMetersPerSecond(3.0f, 4.0f);

			// Act
			var unitVelocity = velocity.Unit();

			// Assert
			Assert.AreEqual(1.0f, unitVelocity.Magnitude, FloatTolerance);
			Assert.AreEqual(0.6f, unitVelocity.X, FloatTolerance);
			Assert.AreEqual(0.8f, unitVelocity.Y, FloatTolerance);
		}

		[TestMethod]
		public void Vector2D_ZeroMagnitude_Unit_ShouldReturnZero()
		{
			// Arrange
			var zeroVelocity = ktsu.Semantics.Float.Velocity2D.FromMetersPerSecond(0.0f, 0.0f);

			// Act
			var unitVelocity = zeroVelocity.Unit();

			// Assert
			Assert.AreEqual(0.0f, unitVelocity.Magnitude, FloatTolerance);
			Assert.AreEqual(0.0f, unitVelocity.X, FloatTolerance);
			Assert.AreEqual(0.0f, unitVelocity.Y, FloatTolerance);
		}
	}
}