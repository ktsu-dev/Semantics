// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;

/// <summary>
/// Measures building a quantity, through each of the three routes a unit factory can take.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Create"/> is the floor: a struct initialiser and nothing else. Every other route
/// here is that plus the work a unit costs, so the difference between them is the measurement.
/// </para>
/// <para>
/// <see cref="FromMeter"/> adds only the magnitude guard, because a metre already is the SI base
/// unit. <see cref="FromKilometer"/> multiplies by a metric magnitude, and
/// <see cref="FromNauticalMile"/> by a conversion constant; both read a <c>Values&lt;T&gt;</c>
/// property that materialises the factor into the storage type once per closed generic and holds
/// it. That holder is what 5.2.0 added, and it is why the storage-type axis is the interesting one
/// here: for <see cref="double"/> the factor was already a <see cref="double"/> and nothing
/// changed, while for <see cref="decimal"/> it went from a 15-digit conversion of a double to the
/// literal parsed at the type's own precision.
/// </para>
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class ConstructionBenchmarks<T>
	where T : struct, INumber<T>
{
	private T value;

	/// <summary>
	/// Prepares the operand.
	/// </summary>
	[GlobalSetup]
	public void Setup() => value = Operands.Of<T>("1234.5678901234567890123456");

	/// <summary>Builds a quantity from a value already in the SI base unit.</summary>
	/// <returns>The quantity.</returns>
	[Benchmark]
	public Length<T> Create() => Length<T>.Create(value);

	/// <summary>Builds a quantity through the base unit's factory, which is the guard alone.</summary>
	/// <returns>The quantity.</returns>
	[Benchmark]
	public Length<T> FromMeter() => Length<T>.FromMeter(value);

	/// <summary>Builds a quantity through a metric magnitude.</summary>
	/// <returns>The quantity.</returns>
	[Benchmark]
	public Length<T> FromKilometer() => Length<T>.FromKilometer(value);

	/// <summary>Builds a quantity through a conversion constant that is not a power of ten.</summary>
	/// <returns>The quantity.</returns>
	[Benchmark]
	public Length<T> FromNauticalMile() => Length<T>.FromNauticalMile(value);
}
