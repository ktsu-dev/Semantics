// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;

/// <summary>
/// Measures the componentwise vector operations, and the square root two of them need.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Dot"/> and <see cref="Cross"/> are written-out arithmetic and nothing else, so they
/// are the control. <see cref="Length"/> and <see cref="Distance"/> are the same arithmetic plus
/// <c>StorageMath.Sqrt</c>, and that is the whole point of the class: a binary floating point type
/// takes the <c>Math.Sqrt</c> round trip, and anything else is seeded from that root and refined
/// with Newton steps in its own arithmetic until it settles. The gap between the two rows is the
/// cost of those steps, and it is a per-storage-type answer rather than a single number.
/// </para>
/// <para>
/// <see cref="LengthSquared"/> is here to separate the two halves: it is <see cref="Length"/>
/// without the root, so the difference between them is the root alone.
/// </para>
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class VectorBenchmarks<T>
	where T : struct, INumber<T>
{
	// Assigned in GlobalSetup before anything is measured. Initialised here because a
	// quantity was a class before 4.0, where an unassigned field is a null reference the
	// compiler rejects; from 4.0 it is a record struct and this is simply its default.
	private Displacement3D<T> left = default!;
	private Displacement3D<T> right = default!;

	/// <summary>
	/// Prepares the operands.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		left = new()
		{
			X = Operands.Of<T>("3.14159265358979323846"),
			Y = Operands.Of<T>("-2.71828182845904523536"),
			Z = Operands.Of<T>("1.41421356237309504880"),
		};
		right = new()
		{
			X = Operands.Of<T>("1.61803398874989484820"),
			Y = Operands.Of<T>("0.57721566490153286060"),
			Z = Operands.Of<T>("-1.73205080756887729352"),
		};
	}

	/// <summary>Sums the squares of the components, without taking a root.</summary>
	/// <returns>The squared length.</returns>
	[Benchmark]
	public T LengthSquared() => left.LengthSquared();

	/// <summary>Takes the length, which is the squared length and a root.</summary>
	/// <returns>The length.</returns>
	[Benchmark]
	public T Length() => left.Length();

	/// <summary>Takes the distance between two vectors, which is a difference and a root.</summary>
	/// <returns>The distance.</returns>
	[Benchmark]
	public T Distance() => left.Distance(right);

	/// <summary>Takes the dot product, which is written-out arithmetic alone.</summary>
	/// <returns>The dot product.</returns>
	[Benchmark]
	public T Dot() => left.Dot(right);

	/// <summary>Takes the cross product, which is written-out arithmetic alone.</summary>
	/// <returns>The cross product.</returns>
	[Benchmark]
	public Displacement3D<T> Cross() => left.Cross(right);
}
