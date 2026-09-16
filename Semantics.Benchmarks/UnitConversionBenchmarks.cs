// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using System.Numerics;

using BenchmarkDotNet.Attributes;

using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;

/// <summary>
/// Measures reading a quantity back out in a unit.
/// </summary>
/// <remarks>
/// <para>
/// This is the other half of the boundary <see cref="ConstructionBenchmarks{T}"/> measures: a
/// value goes in through a factory and comes back out through <c>In(unit)</c>, and between the two
/// it is held in the SI base unit. Anything that uses the library at all crosses this pair, which
/// is why both are measured rather than only the arithmetic between them.
/// </para>
/// <para>
/// <see cref="InCelsius"/> is the affine case. Every other conversion here is a multiplication;
/// a temperature also carries an offset, so it reads <c>ToBaseOffsetAs</c> as well and is the one
/// unit family where the second half of the affine conversion is not free.
/// </para>
/// </remarks>
/// <typeparam name="T">The storage type.</typeparam>
[MemoryDiagnoser]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(PreciseNumber))]
public class UnitConversionBenchmarks<T>
	where T : struct, INumber<T>
{
	private static readonly Meter Meter = new();
	private static readonly Kilometer Kilometer = new();
	private static readonly NauticalMile NauticalMile = new();
	private static readonly Celsius Celsius = new();

	// Assigned in GlobalSetup before anything is measured. Initialised here because a
	// quantity was a class before 4.0, where an unassigned field is a null reference the
	// compiler rejects; from 4.0 it is a record struct and this is simply its default.
	private Length<T> length = default!;
	private Temperature<T> temperature = default!;

	/// <summary>
	/// Prepares the operands.
	/// </summary>
	[GlobalSetup]
	public void Setup()
	{
		length = Length<T>.Create(Operands.Of<T>("1234.5678901234567890123456"));
		temperature = Temperature<T>.Create(Operands.Of<T>("293.15"));
	}

	/// <summary>Reads the value back in the base unit, where the factor is one.</summary>
	/// <returns>The value in metres.</returns>
	[Benchmark]
	public T InMeter() => length.In(Meter);

	/// <summary>Reads the value back through a metric magnitude.</summary>
	/// <returns>The value in kilometres.</returns>
	[Benchmark]
	public T InKilometer() => length.In(Kilometer);

	/// <summary>Reads the value back through a conversion constant.</summary>
	/// <returns>The value in nautical miles.</returns>
	[Benchmark]
	public T InNauticalMile() => length.In(NauticalMile);

	/// <summary>Reads a temperature back through a conversion carrying an offset.</summary>
	/// <returns>The value in degrees Celsius.</returns>
	[Benchmark]
	public T InCelsius() => temperature.In(Celsius);
}
