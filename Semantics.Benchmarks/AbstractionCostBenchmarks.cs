// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;

/// <summary>
/// Measures what the quantity types cost over doing the same arithmetic on the bare storage type.
/// </summary>
/// <remarks>
/// <para>
/// The claim this library makes is that a quantity is a <c>readonly record struct</c> holding one
/// value in the SI base unit, so an operator on it is the storage type's own arithmetic and a
/// struct initialiser, and the wrapper costs nothing once the JIT has inlined it. That is a claim
/// about a number, and the C++ projection has always been held to it against bare floats. This is
/// the same question asked on this side.
/// </para>
/// <para>
/// Each pair runs the identical arithmetic twice, once on <typeparamref name="T"/> and once on the
/// quantity types over it, and the bare one is the BenchmarkDotNet baseline — so the answer is read
/// off the <c>Ratio</c> column rather than by dividing two rows by hand. A ratio of 1.00 is the
/// claim being kept.
/// </para>
/// <para>
/// <b>Why these are loops rather than single operations.</b> A single operator over operands that
/// do not change between iterations is loop-invariant, and the JIT hoists it out: that is what
/// makes <see cref="OperatorBenchmarks{T}"/> report a ZeroMeasurement warning for
/// <see cref="double"/> and <see cref="float"/>, and it would make a ratio between two hoisted
/// methods meaningless. Here each iteration feeds the next, so there is nothing to hoist and both
/// sides of a pair are measurable for every storage type.
/// </para>
/// <para>
/// <b>What the loop costs, and which way it biases.</b> Both sides pay the same counter increment
/// and branch. It is a dependency chain, so on a superscalar core most of that overlaps the
/// arithmetic rather than adding to it, but whatever does not is added equally to numerator and
/// denominator and therefore pulls the ratio toward 1.00. So a ratio at 1.00 is the claim kept, and
/// a ratio above it is a floor on the real cost rather than the whole of it.
/// </para>
/// <para>
/// <b>Why the operands stay bounded.</b> <c>PreciseNumber</c> carries as many digits as the
/// arithmetic produces, so a chain that grows its operand measures that growth instead of the
/// operation. Both loops here accumulate rather than compound: the running value gains a step and
/// the accumulator takes a product, so neither runs away, and the comparison stays about the
/// wrapper for every storage type rather than only for the fixed-width ones.
/// </para>
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class AbstractionCostBenchmarks<T>
	where T : struct, INumber<T>
{
	/// <summary>
	/// Operations per invocation. Enough that the loop's own cost is a small share of the work,
	/// few enough that an arbitrary-precision storage type still finishes an iteration promptly.
	/// </summary>
	private const int Operations = 256;

	private T seed;
	private T step;
	private T other;

	// Assigned in GlobalSetup before anything is measured. Initialised here because a quantity
	// was a class before 4.0, where an unassigned field is a null reference the compiler rejects;
	// from 4.0 it is a record struct and this is simply its default. The backfill measures those
	// releases too, so the file has to compile against both shapes.
	private Length<T> seedLength = default!;
	private Length<T> stepLength = default!;
	private Length<T> otherLength = default!;

	/// <summary>
	/// Prepares the operands, the bare ones and the wrapped ones holding the same values.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		seed = Operands.Of<T>("1234.5678901234567890");
		step = Operands.Of<T>("0.0009765625");
		other = Operands.Of<T>("3.14159265358979323846");

		seedLength = Length<T>.Create(seed);
		stepLength = Length<T>.Create(step);
		otherLength = Length<T>.Create(other);
	}

	/// <summary>Adds along a chain, on the bare storage type.</summary>
	/// <returns>The accumulated value.</returns>
	[BenchmarkCategory("Add")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public T BareAdd()
	{
		T accumulator = seed;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += step;
		}

		return accumulator;
	}

	/// <summary>Adds along the same chain, on quantities of the same dimension.</summary>
	/// <returns>The accumulated quantity.</returns>
	[BenchmarkCategory("Add")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public Length<T> QuantityAdd()
	{
		Length<T> accumulator = seedLength;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += stepLength;
		}

		return accumulator;
	}

	/// <summary>Multiplies and accumulates, on the bare storage type.</summary>
	/// <returns>The accumulated value.</returns>
	[BenchmarkCategory("Multiply")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public T BareMultiply()
	{
		T accumulator = T.Zero;
		T value = seed;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += value * other;
			value += step;
		}

		return accumulator;
	}

	/// <summary>
	/// Multiplies and accumulates over the same values, through the generated physics relationship
	/// that takes two lengths to an area.
	/// </summary>
	/// <returns>The accumulated area.</returns>
	/// <remarks>
	/// The one to read. This is where a quantity does something a bare number cannot — the product
	/// lands on a different dimension, and the type system knows it — so if any of the vocabulary
	/// were going to cost something at run time rather than only at compile time, it would be here.
	/// </remarks>
	[BenchmarkCategory("Multiply")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public Area<T> QuantityMultiply()
	{
		Area<T> accumulator = Area<T>.Zero;
		Length<T> value = seedLength;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += value * otherLength;
			value += stepLength;
		}

		return accumulator;
	}
}
