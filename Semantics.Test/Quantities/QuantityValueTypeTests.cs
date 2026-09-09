// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ktsu.Semantics.Quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Locks in that a quantity is a value type, and that arithmetic on one allocates nothing.
/// </summary>
/// <remarks>
/// Quantities used to be records deriving from an abstract <c>PhysicalQuantity</c>, whose
/// <c>Create</c> was <c>new TQuantity() with { Quantity = value }</c> — two heap allocations,
/// 48 bytes, for every operator and every unit factory. That is invisible in a unit test and
/// ruinous in a per-frame loop, so the property is asserted here rather than left to be
/// rediscovered.
/// </remarks>
[TestClass]
public sealed class QuantityValueTypeTests
{
	/// <summary>
	/// Every generated quantity is a struct. This is what makes the allocation test below
	/// possible, and what lets a consumer lay a quantity out inline in a component.
	/// </summary>
	[TestMethod]
	public void EveryGeneratedQuantityIsAValueType()
	{
		List<Type> quantities = [.. CollectGeneratedQuantityTypes(typeof(Mass<>).Assembly)];

		// Sanity: a filter that silently matched nothing would pass vacuously.
		Assert.IsGreaterThan(
			50,
			quantities.Count,
			$"Expected to find many generated quantity types (got {quantities.Count}). The filter likely needs updating.");

		List<string> referenceTypes = [.. quantities.Where(static t => !t.IsValueType).Select(static t => t.Name)];

		Assert.IsTrue(
			referenceTypes.Count == 0,
			$"These generated quantities are reference types and will allocate on every operation:\n  " +
			string.Join("\n  ", referenceTypes));
	}

	/// <summary>
	/// Arithmetic, unit factories and cross-dimensional operators allocate nothing.
	/// </summary>
	/// <remarks>
	/// Measured rather than inferred from the type's shape: a struct returned through a shared
	/// generic path can still be boxed, so the byte counter is what actually settles it. The
	/// warm-up is what keeps tiered compilation out of the measurement.
	/// </remarks>
	[TestMethod]
	public void QuantityArithmeticDoesNotAllocate()
	{
		Length<double> a = Length<double>.FromMeter(3.0);
		Duration<double> t = Duration<double>.FromSecond(1.5);

		Assert.AreEqual(0L, MeasureAllocation(() => (a + a).Value), "addition allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => (a - a).Value), "subtraction allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => (a * 2.0).Value), "scaling allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => Length<double>.FromMeter(3.0).Value), "a base-unit factory allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => Length<double>.FromFoot(3.0).Value), "a converting factory allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => (a / t).Value), "a cross-dimensional operator allocated");
	}

	/// <summary>
	/// A vector quantity is a value type too, and its arithmetic allocates nothing.
	/// </summary>
	[TestMethod]
	public void VectorQuantityArithmeticDoesNotAllocate()
	{
		Velocity3D<double> v = new() { X = 1.0, Y = 2.0, Z = 3.0 };

		Assert.AreEqual(0L, MeasureAllocation(() => (v + v).X), "vector addition allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => (v * 2.0).Y), "vector scaling allocated");
		Assert.AreEqual(0L, MeasureAllocation(() => v.Magnitude().Value), "Magnitude() allocated");
	}

	/// <summary>
	/// The stored value still renders as the bare number, not as a record's <c>{ Quantity = 2.5 }</c>.
	/// </summary>
	[TestMethod]
	public void ToStringIsTheBareValue()
		=> Assert.AreEqual("2.5", Length<double>.FromMeter(2.5).ToString());

	/// <summary>
	/// <c>default</c> of a quantity is its zero, and equals <c>Zero</c>.
	/// </summary>
	[TestMethod]
	public void DefaultIsZero()
	{
		Assert.AreEqual(0.0, default(Mass<double>).Value);
		Assert.AreEqual(Mass<double>.Zero, default);
	}

	/// <summary>
	/// The one behaviour a value type gives up: <c>default</c> does not run a factory, so a
	/// quantity whose constraint excludes zero can be reached at zero through it.
	/// </summary>
	/// <remarks>
	/// This affects only the V0 overloads declaring <c>physicalConstraints.minExclusive: "0"</c>
	/// — <c>Wavelength</c>, <c>Period</c> and <c>HalfLife</c>. Every other quantity's invariant
	/// is non-negativity, which zero already satisfies, so <c>default</c> is a legal value for
	/// them. The test exists to state the gap rather than to endorse it: a caller that must
	/// reject an unset value should ask, and <c>Zero</c> is the value to compare against.
	/// </remarks>
	[TestMethod]
	public void DefaultBypassesAStrictlyPositiveConstructionGuard()
	{
		Assert.ThrowsExactly<ArgumentException>(static () => Wavelength<double>.FromMeter(0.0));
		Assert.AreEqual(0.0, default(Wavelength<double>).Value);
	}

	/// <summary>
	/// Runs <paramref name="body"/> enough times for tiering to settle, then reports the bytes
	/// it allocates per iteration over a measured run.
	/// </summary>
	/// <param name="body">The expression under measurement, returning its result.</param>
	/// <returns>Bytes allocated per iteration, rounded down.</returns>
	private static long MeasureAllocation(Func<double> body)
	{
		const int WarmUp = 10_000;
		const int Iterations = 100_000;

		double accumulated = 0.0;
		for (int i = 0; i < WarmUp; i++)
		{
			accumulated += body();
		}

		// No GC.Collect first: this counter is the thread's cumulative allocation total, which
		// a collection does not reset. Collecting would measure nothing and cost seconds.
		long before = GC.GetAllocatedBytesForCurrentThread();
		for (int i = 0; i < Iterations; i++)
		{
			accumulated += body();
		}

		long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

		// Reading the accumulated result is what stops the measured expression being optimised
		// away — without it the loop could be removed and every measurement would read zero.
		Assert.IsTrue(double.IsFinite(accumulated), "the measured expression produced a non-finite result");

		return allocated / Iterations;
	}

	private static IEnumerable<Type> CollectGeneratedQuantityTypes(Assembly assembly)
	{
		foreach (Type type in assembly.GetTypes())
		{
			if (type.Namespace != "ktsu.Semantics.Quantities" || !type.IsGenericTypeDefinition)
			{
				continue;
			}

			// Generated quantity types implement one of IVector0..IVector4 (closed over TSelf, T).
			if (type.GetInterfaces().Any(static i => i.IsGenericType && i.Name.StartsWith("IVector", StringComparison.Ordinal)))
			{
				yield return type;
			}
		}
	}
}
