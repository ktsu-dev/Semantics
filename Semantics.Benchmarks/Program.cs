// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks;

using BenchmarkDotNet.Running;

/// <summary>
/// Entry point for the benchmark suite.
/// </summary>
internal static class Program
{
	/// <summary>
	/// Runs the benchmarks named on the command line, or prompts for a selection when none are.
	/// </summary>
	/// <param name="args">Command line arguments, forwarded to BenchmarkDotNet.</param>
	internal static void Main(string[] args) =>
		_ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, BenchmarkConfig.Create());
}
