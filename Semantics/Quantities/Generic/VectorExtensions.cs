// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Generic;

using System.Numerics;

/// <summary>
/// Extension methods for creating vector quantities from numeric values and System.Numerics vectors.
/// Provides convenient factory methods for 2D, 3D, and 4D vector quantities.
/// </summary>
public static class VectorExtensions
{
	#region 2D Vector Extensions

	/// <summary>
	/// Creates a Position2D from X and Y coordinates in meters.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Float.Position2D MetersPosition2D(this (float x, float y) coordinates)
		=> Float.Position2D.FromMeters(coordinates.x, coordinates.y);

	/// <summary>
	/// Creates a Position2D from X and Y coordinates in meters.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Double.Position2D MetersPosition2D(this (double x, double y) coordinates)
		=> Double.Position2D.FromMeters(coordinates.x, coordinates.y);

	/// <summary>
	/// Creates a Displacement2D from X and Y components in meters.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in meters.</param>
	/// <param name="y">The Y component in meters.</param>
	/// <returns>A new Displacement2D instance.</returns>
	public static Float.Displacement2D MetersDisplacement2D(this (float x, float y) components)
		=> Float.Displacement2D.FromMeters(components.x, components.y);

	/// <summary>
	/// Creates a Displacement2D from X and Y components in meters.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in meters.</param>
	/// <param name="y">The Y component in meters.</param>
	/// <returns>A new Displacement2D instance.</returns>
	public static Double.Displacement2D MetersDisplacement2D(this (double x, double y) components)
		=> Double.Displacement2D.FromMeters(components.x, components.y);

	/// <summary>
	/// Creates a Velocity2D from X and Y components in meters per second.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s.</param>
	/// <param name="y">The Y component in m/s.</param>
	/// <returns>A new Velocity2D instance.</returns>
	public static Float.Velocity2D MetersPerSecondVelocity2D(this (float x, float y) components)
		=> Float.Velocity2D.FromMetersPerSecond(components.x, components.y);

	/// <summary>
	/// Creates a Velocity2D from X and Y components in meters per second.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s.</param>
	/// <param name="y">The Y component in m/s.</param>
	/// <returns>A new Velocity2D instance.</returns>
	public static Double.Velocity2D MetersPerSecondVelocity2D(this (double x, double y) components)
		=> Double.Velocity2D.FromMetersPerSecond(components.x, components.y);

	/// <summary>
	/// Creates a Force2D from X and Y components in newtons.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in N.</param>
	/// <param name="y">The Y component in N.</param>
	/// <returns>A new Force2D instance.</returns>
	public static Float.Force2D NewtonsForce2D(this (float x, float y) components)
		=> Float.Force2D.FromNewtons(components.x, components.y);

	/// <summary>
	/// Creates a Force2D from X and Y components in newtons.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in N.</param>
	/// <param name="y">The Y component in N.</param>
	/// <returns>A new Force2D instance.</returns>
	public static Double.Force2D NewtonsForce2D(this (double x, double y) components)
		=> Double.Force2D.FromNewtons(components.x, components.y);

	#endregion

	#region 3D Vector Extensions

	/// <summary>
	/// Creates a Position3D from X, Y, and Z coordinates in meters.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <param name="z">The Z coordinate in meters.</param>
	/// <returns>A new Position3D instance.</returns>
	public static Float.Position3D MetersPosition3D(this (float x, float y, float z) coordinates)
		=> Float.Position3D.FromMeters(coordinates.x, coordinates.y, coordinates.z);

	/// <summary>
	/// Creates a Position3D from X, Y, and Z coordinates in meters.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <param name="z">The Z coordinate in meters.</param>
	/// <returns>A new Position3D instance.</returns>
	public static Double.Position3D MetersPosition3D(this (double x, double y, double z) coordinates)
		=> Double.Position3D.FromMeters(coordinates.x, coordinates.y, coordinates.z);

	/// <summary>
	/// Creates a Displacement3D from X, Y, and Z components in meters.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in meters.</param>
	/// <param name="y">The Y component in meters.</param>
	/// <param name="z">The Z component in meters.</param>
	/// <returns>A new Displacement3D instance.</returns>
	public static Float.Displacement3D MetersDisplacement3D(this (float x, float y, float z) components)
		=> Float.Displacement3D.FromMeters(components.x, components.y, components.z);

	/// <summary>
	/// Creates a Displacement3D from X, Y, and Z components in meters.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in meters.</param>
	/// <param name="y">The Y component in meters.</param>
	/// <param name="z">The Z component in meters.</param>
	/// <returns>A new Displacement3D instance.</returns>
	public static Double.Displacement3D MetersDisplacement3D(this (double x, double y, double z) components)
		=> Double.Displacement3D.FromMeters(components.x, components.y, components.z);

	/// <summary>
	/// Creates a Velocity3D from X, Y, and Z components in meters per second.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s.</param>
	/// <param name="y">The Y component in m/s.</param>
	/// <param name="z">The Z component in m/s.</param>
	/// <returns>A new Velocity3D instance.</returns>
	public static Float.Velocity3D MetersPerSecondVelocity3D(this (float x, float y, float z) components)
		=> Float.Velocity3D.FromMetersPerSecond(components.x, components.y, components.z);

	/// <summary>
	/// Creates a Velocity3D from X, Y, and Z components in meters per second.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s.</param>
	/// <param name="y">The Y component in m/s.</param>
	/// <param name="z">The Z component in m/s.</param>
	/// <returns>A new Velocity3D instance.</returns>
	public static Double.Velocity3D MetersPerSecondVelocity3D(this (double x, double y, double z) components)
		=> Double.Velocity3D.FromMetersPerSecond(components.x, components.y, components.z);

	/// <summary>
	/// Creates an Acceleration3D from X, Y, and Z components in meters per second squared.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s².</param>
	/// <param name="y">The Y component in m/s².</param>
	/// <param name="z">The Z component in m/s².</param>
	/// <returns>A new Acceleration3D instance.</returns>
	public static Float.Acceleration3D MetersPerSecondSquaredAcceleration3D(this (float x, float y, float z) components)
		=> Float.Acceleration3D.FromMetersPerSecondSquared(components.x, components.y, components.z);

	/// <summary>
	/// Creates an Acceleration3D from X, Y, and Z components in meters per second squared.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in m/s².</param>
	/// <param name="y">The Y component in m/s².</param>
	/// <param name="z">The Z component in m/s².</param>
	/// <returns>A new Acceleration3D instance.</returns>
	public static Double.Acceleration3D MetersPerSecondSquaredAcceleration3D(this (double x, double y, double z) components)
		=> Double.Acceleration3D.FromMetersPerSecondSquared(components.x, components.y, components.z);

	/// <summary>
	/// Creates a Force3D from X, Y, and Z components in newtons.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in N.</param>
	/// <param name="y">The Y component in N.</param>
	/// <param name="z">The Z component in N.</param>
	/// <returns>A new Force3D instance.</returns>
	public static Float.Force3D NewtonsForce3D(this (float x, float y, float z) components)
		=> Float.Force3D.FromNewtons(components.x, components.y, components.z);

	/// <summary>
	/// Creates a Force3D from X, Y, and Z components in newtons.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="x">The X component in N.</param>
	/// <param name="y">The Y component in N.</param>
	/// <param name="z">The Z component in N.</param>
	/// <returns>A new Force3D instance.</returns>
	public static Double.Force3D NewtonsForce3D(this (double x, double y, double z) components)
		=> Double.Force3D.FromNewtons(components.x, components.y, components.z);

	#endregion

	#region 4D Vector Extensions

	/// <summary>
	/// Creates a Position4D from spacetime coordinates.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <param name="z">The Z coordinate in meters.</param>
	/// <param name="t">The time coordinate in seconds.</param>
	/// <returns>A new Position4D instance.</returns>
	public static Float.Position4D SpacetimePosition4D(this (float x, float y, float z, float t) coordinates)
		=> Float.Position4D.FromSpacetime(coordinates.x, coordinates.y, coordinates.z, coordinates.t);

	/// <summary>
	/// Creates a Position4D from spacetime coordinates.
	/// </summary>
	/// <param name="coordinates"></param>
	/// <param name="x">The X coordinate in meters.</param>
	/// <param name="y">The Y coordinate in meters.</param>
	/// <param name="z">The Z coordinate in meters.</param>
	/// <param name="t">The time coordinate in seconds.</param>
	/// <returns>A new Position4D instance.</returns>
	public static Double.Position4D SpacetimePosition4D(this (double x, double y, double z, double t) coordinates)
		=> Double.Position4D.FromSpacetime(coordinates.x, coordinates.y, coordinates.z, coordinates.t);

	/// <summary>
	/// Creates a Displacement4D from spacetime displacement.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="dx">The X displacement in meters.</param>
	/// <param name="dy">The Y displacement in meters.</param>
	/// <param name="dz">The Z displacement in meters.</param>
	/// <param name="dt">The time interval in seconds.</param>
	/// <returns>A new Displacement4D instance.</returns>
	public static Float.Displacement4D SpacetimeDisplacement4D(this (float dx, float dy, float dz, float dt) components)
		=> Float.Displacement4D.FromSpacetime(components.dx, components.dy, components.dz, components.dt);

	/// <summary>
	/// Creates a Displacement4D from spacetime displacement.
	/// </summary>
	/// <param name="components"></param>
	/// <param name="dx">The X displacement in meters.</param>
	/// <param name="dy">The Y displacement in meters.</param>
	/// <param name="dz">The Z displacement in meters.</param>
	/// <param name="dt">The time interval in seconds.</param>
	/// <returns>A new Displacement4D instance.</returns>
	public static Double.Displacement4D SpacetimeDisplacement4D(this (double dx, double dy, double dz, double dt) components)
		=> Double.Displacement4D.FromSpacetime(components.dx, components.dy, components.dz, components.dt);

	#endregion

	#region System.Numerics Integration

	/// <summary>
	/// Creates a Position2D from a System.Numerics.Vector2 in meters.
	/// </summary>
	/// <param name="vector">The Vector2 representing position in meters.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Float.Position2D MetersPosition2D(this Vector2 vector)
		=> Float.Position2D.FromMeters(vector);

	/// <summary>
	/// Creates a Velocity2D from a System.Numerics.Vector2 in meters per second.
	/// </summary>
	/// <param name="vector">The Vector2 representing velocity in m/s.</param>
	/// <returns>A new Velocity2D instance.</returns>
	public static Float.Velocity2D MetersPerSecondVelocity2D(this Vector2 vector)
		=> Float.Velocity2D.FromMetersPerSecond(vector);

	/// <summary>
	/// Creates a Position3D from a System.Numerics.Vector3 in meters.
	/// </summary>
	/// <param name="vector">The Vector3 representing position in meters.</param>
	/// <returns>A new Position3D instance.</returns>
	public static Float.Position3D MetersPosition3D(this Vector3 vector)
		=> Float.Position3D.FromMeters(vector);

	/// <summary>
	/// Creates a Velocity3D from a System.Numerics.Vector3 in meters per second.
	/// </summary>
	/// <param name="vector">The Vector3 representing velocity in m/s.</param>
	/// <returns>A new Velocity3D instance.</returns>
	public static Float.Velocity3D MetersPerSecondVelocity3D(this Vector3 vector)
		=> Float.Velocity3D.FromMetersPerSecond(vector);

	/// <summary>
	/// Creates a Force3D from a System.Numerics.Vector3 in newtons.
	/// </summary>
	/// <param name="vector">The Vector3 representing force in N.</param>
	/// <returns>A new Force3D instance.</returns>
	public static Float.Force3D NewtonsForce3D(this Vector3 vector)
		=> Float.Force3D.FromNewtons(vector);

	#endregion

	#region Convenience Methods

	/// <summary>
	/// Creates a 2D position vector from polar coordinates.
	/// </summary>
	/// <param name="polar"></param>
	/// <param name="radius">The radius in meters.</param>
	/// <param name="angle">The angle in radians.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Float.Position2D PolarPosition2D(this (float radius, float angle) polar)
	{
		float x = polar.radius * MathF.Cos(polar.angle);
		float y = polar.radius * MathF.Sin(polar.angle);
		return Float.Position2D.FromMeters(x, y);
	}

	/// <summary>
	/// Creates a 2D position vector from polar coordinates.
	/// </summary>
	/// <param name="polar"></param>
	/// <param name="radius">The radius in meters.</param>
	/// <param name="angle">The angle in radians.</param>
	/// <returns>A new Position2D instance.</returns>
	public static Double.Position2D PolarPosition2D(this (double radius, double angle) polar)
	{
		double x = polar.radius * Math.Cos(polar.angle);
		double y = polar.radius * Math.Sin(polar.angle);
		return Double.Position2D.FromMeters(x, y);
	}

	/// <summary>
	/// Creates a 3D position vector from spherical coordinates.
	/// </summary>
	/// <param name="spherical"></param>
	/// <param name="radius">The radius in meters.</param>
	/// <param name="theta">The polar angle in radians (0 to π).</param>
	/// <param name="phi">The azimuthal angle in radians (0 to 2π).</param>
	/// <returns>A new Position3D instance.</returns>
	public static Float.Position3D SphericalPosition3D(this (float radius, float theta, float phi) spherical)
	{
		float x = spherical.radius * MathF.Sin(spherical.theta) * MathF.Cos(spherical.phi);
		float y = spherical.radius * MathF.Sin(spherical.theta) * MathF.Sin(spherical.phi);
		float z = spherical.radius * MathF.Cos(spherical.theta);
		return Float.Position3D.FromMeters(x, y, z);
	}

	/// <summary>
	/// Creates a 3D position vector from spherical coordinates.
	/// </summary>
	/// <param name="spherical"></param>
	/// <param name="radius">The radius in meters.</param>
	/// <param name="theta">The polar angle in radians (0 to π).</param>
	/// <param name="phi">The azimuthal angle in radians (0 to 2π).</param>
	/// <returns>A new Position3D instance.</returns>
	public static Double.Position3D SphericalPosition3D(this (double radius, double theta, double phi) spherical)
	{
		double x = spherical.radius * Math.Sin(spherical.theta) * Math.Cos(spherical.phi);
		double y = spherical.radius * Math.Sin(spherical.theta) * Math.Sin(spherical.phi);
		double z = spherical.radius * Math.Cos(spherical.theta);
		return Double.Position3D.FromMeters(x, y, z);
	}

	#endregion
}
