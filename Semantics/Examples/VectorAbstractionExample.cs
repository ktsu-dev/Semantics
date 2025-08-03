// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Examples;

using System.Numerics;
using DoubleTypes = Double;
using FloatTypes = Float;

/// <summary>
/// Example demonstrating the vector abstraction system.
/// </summary>
public static class VectorAbstractionExample
{
	/// <summary>
	/// Demonstrates float-precision vector operations.
	/// </summary>
	public static void DemonstrateFloatVectors()
	{
		// Float precision position and displacement
		FloatTypes.Position2D startPosition = FloatTypes.Position2D.FromMeters(10.0f, 20.0f);
		FloatTypes.Displacement2D displacement = FloatTypes.Displacement2D.FromMeters(5.0f, -3.0f);
		FloatTypes.Position2D endPosition = startPosition + displacement;

		Console.WriteLine($"Start: {startPosition}");
		Console.WriteLine($"Displacement: {displacement}");
		Console.WriteLine($"End: {endPosition}");
		Console.WriteLine($"Distance: {startPosition.Distance(endPosition)}");

		// Velocity and force vectors
		FloatTypes.Velocity2D velocity = FloatTypes.Velocity2D.FromMetersPerSecond(2.5f, 1.8f);
		FloatTypes.Force2D force = FloatTypes.Force2D.FromNewtons(100.0f, -50.0f);

		Console.WriteLine($"Velocity: {velocity} (magnitude: {velocity.Magnitude})");
		Console.WriteLine($"Force: {force} (magnitude: {force.Magnitude})");

		// Vector arithmetic
		FloatTypes.Velocity2D combinedVelocity = velocity + FloatTypes.Velocity2D.FromMetersPerSecond(1.0f, 0.5f);
		Console.WriteLine($"Combined velocity: {combinedVelocity}");
	}

	/// <summary>
	/// Demonstrates double-precision vector operations.
	/// </summary>
	public static void DemonstrateDoubleVectors()
	{
		// Double precision position
		DoubleTypes.Position2D precisePosition1 = DoubleTypes.Position2D.FromMeters(10.123456789, 20.987654321);
		DoubleTypes.Position2D precisePosition2 = DoubleTypes.Position2D.FromMeters(10.123456790, 20.987654320);

		Console.WriteLine($"Position 1: {precisePosition1}");
		Console.WriteLine($"Position 2: {precisePosition2}");
		Console.WriteLine($"Precise distance: {precisePosition1.Distance(precisePosition2)}");

		// Custom vector operations
		DoubleTypes.Vector2d vector = new(3.0, 4.0);
		Console.WriteLine($"Vector: {vector}");
		Console.WriteLine($"Length: {vector.Length()}");
		Console.WriteLine($"Normalized: {vector.Normalize()}");
	}

	/// <summary>
	/// Demonstrates interoperability with System.Numerics.
	/// </summary>
	public static void DemonstrateSystemNumericsInterop()
	{
		// Seamless conversion with System.Numerics
		Vector2 systemVector = new(5.0f, 12.0f);
		FloatTypes.Position2D position = FloatTypes.Position2D.FromMeters(systemVector);

		Console.WriteLine($"From System.Numerics Vector2: {position}");
		Console.WriteLine($"Magnitude: {position.Magnitude}");

		// Back to System.Numerics
		Vector2 backToSystem = position.InMeters();
		Console.WriteLine($"Back to Vector2: {backToSystem}");
	}
}
