// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;

/// <summary>
/// Measures the generated physics relationships.
/// </summary>
/// <remarks>
/// <para>
/// Every value is held in its SI base unit, so a relationship operator is the storage type's own
/// arithmetic and a struct initialiser — there is no conversion in the middle of an equation. That
/// is the claim this class exists to keep honest: an operator should cost what the same arithmetic
/// costs on bare <typeparamref name="T"/>, and <see cref="Add"/> is here as the comparison,
/// being the one operation that changes no dimension.
/// </para>
/// <para>
/// For <see cref="double"/> and <see cref="float"/> that claim comes back as a non-answer, and the
/// non-answer is the point. The operands do not change between iterations, so a product of two of
/// them is loop-invariant and the JIT hoists it clean out; BenchmarkDotNet then reports a
/// ZeroMeasurement warning — the method is indistinguishable from an empty one. Nothing here can
/// fix that without measuring the fix instead of the operator: an array of operands adds a load,
/// a mutated field adds a store, and either would swamp the single instruction being asked about.
/// Read those two rows as "below what the harness resolves", not as a number, and read the
/// <see cref="decimal"/> and <c>PreciseNumber</c> rows, which are far enough above the floor to
/// mean something. The release chart draws none of these for the same reason.
/// </para>
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class OperatorBenchmarks<T>
	where T : struct, INumber<T>
{
	// Assigned in GlobalSetup before anything is measured. Initialised here because a
	// quantity was a class before 4.0, where an unassigned field is a null reference the
	// compiler rejects; from 4.0 it is a record struct and this is simply its default.
	private Length<T> length = default!;
	private Length<T> otherLength = default!;
	private Force1D<T> force = default!;
	private Duration<T> duration = default!;
	private Velocity3D<T> velocity = default!;

	/// <summary>
	/// Prepares the operands.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		length = Length<T>.Create(Operands.Of<T>("1234.5678901234567890123456"));
		otherLength = Length<T>.Create(Operands.Of<T>("8765.4321098765432109876543"));
		force = Force1D<T>.Create(Operands.Of<T>("-98.76543210987654321"));
		duration = Duration<T>.Create(Operands.Of<T>("12.3456789012345678"));
		velocity = new()
		{
			X = Operands.Of<T>("3.14159265358979323846"),
			Y = Operands.Of<T>("-2.71828182845904523536"),
			Z = Operands.Of<T>("1.41421356237309504880"),
		};
	}

	/// <summary>Adds two quantities of the same dimension.</summary>
	/// <returns>The sum.</returns>
	[Benchmark]
	public Length<T> Add() => length + otherLength;

	/// <summary>Multiplies two lengths, which lands on a different dimension.</summary>
	/// <returns>The area.</returns>
	[Benchmark]
	public Area<T> LengthTimesLength() => length * otherLength;

	/// <summary>Multiplies a signed scalar quantity by a duration.</summary>
	/// <returns>The momentum.</returns>
	[Benchmark]
	public Momentum1D<T> ForceTimesDuration() => force * duration;

	/// <summary>Multiplies a three-component quantity by a duration, componentwise.</summary>
	/// <returns>The displacement.</returns>
	[Benchmark]
	public Displacement3D<T> VelocityTimesDuration() => velocity * duration;

	/// <summary>Divides one quantity by another of the same dimension, giving a bare ratio.</summary>
	/// <returns>The ratio.</returns>
	[Benchmark]
	public T LengthOverLength() => length / otherLength;
}
