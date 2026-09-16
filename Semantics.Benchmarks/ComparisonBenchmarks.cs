// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;

/// <summary>
/// Measures comparing and equating quantities, by each of the two routes available.
/// </summary>
/// <remarks>
/// A quantity carries two comparisons, and they do not cost the same. The operators are the
/// storage type's own, on a struct, and should allocate nothing. <see cref="CompareToInterface"/>
/// and <see cref="EqualsInterface"/> go through <c>PhysicalQuantityCore</c>, which takes an
/// <c>IPhysicalQuantity&lt;T&gt;</c> — an interface, so the argument is boxed at the call, and the
/// dimensions are checked before the values are. That is a deliberate difference rather than an
/// oversight: the interface route is the one that can refuse to compare a length with a mass, and
/// this class is what says what it costs.
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class ComparisonBenchmarks<T>
	where T : struct, INumber<T>
{
	// Assigned in GlobalSetup before anything is measured. Initialised here because a
	// quantity was a class before 4.0, where an unassigned field is a null reference the
	// compiler rejects; from 4.0 it is a record struct and this is simply its default.
	private Length<T> left = default!;
	private Length<T> right = default!;

	/// <summary>
	/// Prepares the operands.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		left = Length<T>.Create(Operands.Of<T>("1234.5678901234567890123456"));
		right = Length<T>.Create(Operands.Of<T>("1234.5678901234567890123457"));
	}

	/// <summary>Compares with the less-than operator.</summary>
	/// <returns>Whether the left sorts before the right.</returns>
	[Benchmark]
	public bool LessThan() => left < right;

	/// <summary>Compares through the dimension-checking interface.</summary>
	/// <returns>The comparison.</returns>
	[Benchmark]
	public int CompareToInterface() => left.CompareTo(right);

	/// <summary>Equates through the record's generated equality.</summary>
	/// <returns>Whether the two are equal.</returns>
	[Benchmark]
	public bool EqualsRecord() => left.Equals(right);

	/// <summary>Equates through the dimension-checking interface.</summary>
	/// <returns>Whether the two are equal.</returns>
	[Benchmark]
	public bool EqualsInterface() => left.Equals((IPhysicalQuantity<T>)right);
}
