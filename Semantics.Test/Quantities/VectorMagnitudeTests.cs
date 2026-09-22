// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the two vector members that answer with the dimension's magnitude form rather than with
/// a bare <c>T</c> — <c>Magnitude()</c> and <c>DistanceTo()</c>. Issue #238.
/// </summary>
/// <remarks>
/// <para>
/// Every assertion here is written through an <em>explicitly typed local</em> rather than
/// <c>var</c>. That is the point of most of these tests: the interesting claim is the static return
/// type, so the compiler checks it and the runtime assertion is the lesser half. A regression that
/// changed <c>Velocity3D.Magnitude()</c> back to <c>T</c>, or to the wrong V0, would stop this file
/// compiling rather than failing an assertion.
/// </para>
/// <para>
/// <c>LengthSquared()</c> and <c>Length()</c> deliberately still return <c>T</c> and are not
/// retested here; they are unchanged, and the squared one has no dimension to answer with.
/// </para>
/// </remarks>
[TestClass]
public sealed class VectorMagnitudeTests
{
	private const double Tolerance = 1e-10;

	// ---------------------------------------- Magnitude returns the V0 base

	/// <summary>
	/// The case a naive generator rule would break on: the V0 base of <c>Velocity</c> is called
	/// <c>Speed</c>, not <c>Velocity</c>, so a rule reading the dimension name rather than
	/// <c>quantities.vector0.base</c> would emit a type that does not exist.
	/// </summary>
	[TestMethod]
	public void Velocity3D_Magnitude_Is_Speed_Not_A_Bare_Storage_Value()
	{
		Velocity3D<double> v = new() { X = 3.0, Y = 4.0, Z = 0.0 };

		Speed<double> speed = v.Magnitude();

		Assert.AreEqual(5.0, speed.Value, Tolerance);
	}

	/// <summary>
	/// The second name-differs case, and a different shape of name: <c>Acceleration</c>'s V0 base
	/// is <c>AccelerationMagnitude</c>.
	/// </summary>
	[TestMethod]
	public void Acceleration3D_Magnitude_Is_AccelerationMagnitude()
	{
		Acceleration3D<double> a = new() { X = 0.0, Y = 6.0, Z = 8.0 };

		AccelerationMagnitude<double> magnitude = a.Magnitude();

		Assert.AreEqual(10.0, magnitude.Value, Tolerance);
	}

	/// <summary>
	/// <c>Displacement3D</c> is the name-collision case. Its magnitude form is called
	/// <c>Length</c>, and the vector already has a <c>Length()</c> method, so the generated return
	/// type has to be fully qualified. This test is what proves the two are distinguishable from a
	/// caller's side: one answers <c>T</c>, the other answers <c>Length&lt;T&gt;</c>.
	/// </summary>
	[TestMethod]
	public void Displacement3D_Length_And_Magnitude_Agree_But_Differ_In_Type()
	{
		Displacement3D<double> d = new() { X = 1.0, Y = 2.0, Z = 2.0 };

		double bare = d.Length();
		Length<double> typed = d.Magnitude();

		Assert.AreEqual(3.0, bare, Tolerance);
		Assert.AreEqual(bare, typed.Value, Tolerance);
	}

	/// <summary>
	/// A vector overload gets the members too, and answers with the V0 <em>base</em> rather than
	/// with an overload of it — <c>Length</c>, not <c>Distance</c> — because the base is what every
	/// overload widens from. A caller who wants <c>Distance</c> narrows explicitly.
	/// </summary>
	[TestMethod]
	public void Position3D_Magnitude_Answers_With_The_V0_Base()
	{
		Position3D<double> p = new() { X = 0.0, Y = 3.0, Z = 4.0 };

		Length<double> magnitude = p.Magnitude();

		Assert.AreEqual(5.0, magnitude.Value, Tolerance);
	}

	/// <summary>Two and four components get the same treatment as three.</summary>
	[TestMethod]
	public void Vector2_And_Vector4_Also_Answer_With_Their_V0_Base()
	{
		Velocity2D<double> v2 = new() { X = 3.0, Y = 4.0 };
		Velocity4D<double> v4 = new() { X = 1.0, Y = 1.0, Z = 1.0, W = 1.0 };

		Speed<double> fromTwo = v2.Magnitude();
		Speed<double> fromFour = v4.Magnitude();

		Assert.AreEqual(5.0, fromTwo.Value, Tolerance);
		Assert.AreEqual(2.0, fromFour.Value, Tolerance);
	}

	// -------------------------------------------------- Agreement and zero

	/// <summary>
	/// <c>Magnitude()</c> is the V0 constructed from <c>Length()</c>, and nothing more.
	/// </summary>
	[TestMethod]
	public void Magnitude_Equals_The_V0_Constructed_From_Length()
	{
		Velocity3D<double> v = new() { X = -2.0, Y = 5.0, Z = 14.0 };

		Speed<double> magnitude = v.Magnitude();

		Assert.AreEqual(Speed<double>.Create(v.Length()).Value, magnitude.Value, Tolerance);
	}

	/// <summary>The magnitude of a zero vector is the V0's zero.</summary>
	[TestMethod]
	public void Magnitude_Of_A_Zero_Vector_Is_The_V0_Zero()
	{
		Speed<double> magnitude = Velocity3D<double>.Zero.Magnitude();

		Assert.AreEqual(Speed<double>.Zero.Value, magnitude.Value, Tolerance);
	}

	/// <summary>
	/// <c>Magnitude()</c> uses <c>Create</c> rather than a <c>From{Unit}</c> factory, so it does
	/// not run <c>Vector0Guards</c>. Negative components must therefore still produce a
	/// non-negative magnitude by arithmetic alone, rather than by a guard throwing.
	/// </summary>
	[TestMethod]
	public void Magnitude_Of_All_Negative_Components_Is_Non_Negative_Without_A_Guard()
	{
		Velocity3D<double> v = new() { X = -3.0, Y = -4.0, Z = 0.0 };

		Speed<double> magnitude = v.Magnitude();

		Assert.AreEqual(5.0, magnitude.Value, Tolerance);
	}

	// ------------------------------------------------------------ DistanceTo

	/// <summary>
	/// <c>DistanceTo</c> is <c>Distance</c> with its dimension kept, so it agrees with the bare
	/// member exactly.
	/// </summary>
	[TestMethod]
	public void DistanceTo_Agrees_With_Distance_But_Keeps_The_Dimension()
	{
		Displacement3D<double> a = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		Displacement3D<double> b = new() { X = 4.0, Y = 6.0, Z = 3.0 };

		double bare = a.Distance(b);
		Length<double> typed = a.DistanceTo(b);

		Assert.AreEqual(5.0, bare, Tolerance);
		Assert.AreEqual(bare, typed.Value, Tolerance);
	}

	/// <summary>
	/// For a signed form, where subtraction is defined, the distance between two vectors is the
	/// magnitude of their difference. This is the invariant that would catch a
	/// <c>DistanceTo</c> wired to the wrong helper.
	/// </summary>
	[TestMethod]
	public void DistanceTo_Is_The_Magnitude_Of_The_Difference()
	{
		Displacement3D<double> a = new() { X = 1.0, Y = 2.0, Z = 3.0 };
		Displacement3D<double> b = new() { X = 4.0, Y = 6.0, Z = 3.0 };

		Length<double> distance = a.DistanceTo(b);
		Length<double> ofDifference = (a - b).Magnitude();

		Assert.AreEqual(ofDifference.Value, distance.Value, Tolerance);
	}

	/// <summary>The distance from a vector to itself is zero.</summary>
	[TestMethod]
	public void DistanceTo_Self_Is_Zero()
	{
		Velocity3D<double> v = new() { X = 7.0, Y = -1.0, Z = 4.0 };

		Speed<double> distance = v.DistanceTo(v);

		Assert.AreEqual(0.0, distance.Value, Tolerance);
	}
}
