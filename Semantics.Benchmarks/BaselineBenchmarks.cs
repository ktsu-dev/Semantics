// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using BenchmarkDotNet.Attributes;

/// <summary>
/// Measures a fixed workload that touches none of this library, so that timings taken on
/// different machines can be compared.
/// </summary>
/// <remarks>
/// Every release is benchmarked in its own CI job, and a job lands on whichever shared runner is
/// free — an x86-64-v3 or v4 host, at whatever clock its neighbours leave it. That difference is
/// routinely larger than the changes a release makes, so a chart of raw times across releases
/// mostly plots the runner.
/// <para>
/// This benchmark is the fixed point that makes the rest comparable. It is integer arithmetic over
/// a value the JIT cannot fold away, chosen because it has no allocation, no library code, and no
/// dependence on anything that changes between versions — so its measured time is a reading of the
/// machine and nothing else. Dividing a benchmark's time by this one's, taken in the same job,
/// cancels most of the difference between hosts. `scripts/benchmark-history.cs` records it on
/// every entry and plots the ratio rather than the nanoseconds.
/// </para>
/// <para>
/// It follows that this method's body must never change. Editing it silently rescales every
/// comparison drawn against history recorded before the edit.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class BaselineBenchmarks
{
	// Read from a field rather than written as a literal, so that the loop cannot be constant
	// folded into its own answer at JIT time.
	private ulong seed;

	/// <summary>
	/// Sets the starting value.
	/// </summary>
	[GlobalSetup]
	public void Setup() => seed = 0xcbf29ce484222325;

	/// <summary>
	/// Mixes a counter with a multiply-xor-shift step, the way a non-cryptographic hash does.
	/// </summary>
	/// <returns>The accumulated value, returned so that nothing here is dead code.</returns>
	[Benchmark]
	public ulong ReferenceWork()
	{
		ulong accumulator = seed;

		for (int i = 0; i < 256; i++)
		{
			accumulator = (accumulator ^ (ulong)i) * 0x100000001b3;
			accumulator ^= accumulator >> 29;
		}

		return accumulator;
	}
}
