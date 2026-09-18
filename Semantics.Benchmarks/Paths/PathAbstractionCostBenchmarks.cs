// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using System.IO;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures what the path types cost over <see cref="Path"/> doing the same work.
/// </summary>
/// <remarks>
/// <para>
/// This pairing needs none of the care the string one does. <see cref="Path"/> is a real API doing
/// the real work, so each category is the same operation twice and the ratio is a straight answer.
/// </para>
/// <para>
/// What the semantic side adds is a validated wrapper around the result: every one of these
/// operations returns a path type rather than a string, which means a creation, which means the
/// reflection machinery and the validator. The ratio is therefore expected well above 1.00
/// throughout, and the number is the point rather than a disappointment — it is what a caller pays
/// for a result that cannot be silently passed where a different kind of path belongs.
/// </para>
/// <para>
/// The loops exist for the reason they do everywhere in this suite: a single call over an
/// unchanging operand is loop-invariant, the JIT hoists it, and a ratio between two hoisted
/// methods means nothing.
/// </para>
/// <para>
/// <b>Read <see cref="SemanticCreate"/>'s ratio differently from the other three.</b> It is the
/// largest by a wide margin, and that is a fact about its baseline rather than about creation. The
/// semantic side costs about the same in all four categories, because all four are dominated by the
/// same reflection and validation; what differs is what each is measured against.
/// <see cref="BareFileName"/>, <see cref="BareAsAbsolute"/> and <see cref="BareAsRelative"/> each do
/// real string work, while <see cref="BareCreate"/> is a single boolean check costing a few
/// nanoseconds. Dividing a roughly constant numerator by a much smaller denominator is most of the
/// difference between 1,810 and the double- and triple-digit ratios above it.
/// </para>
/// <para>
/// <b>And <see cref="BareCreate"/> is deliberately narrower than the validator it stands in for.</b>
/// <c>IsAbsolutePathAttribute</c> asks
/// <c>Path.IsPathFullyQualified(value + Path.DirectorySeparatorChar)</c>, concatenating and
/// allocating first; the baseline here asks the question without the separator. That is not an
/// oversight, and it is worth roughly a fourteenfold difference in the ratio on its own, so it is
/// worth saying why: a baseline in this class is <i>the code a caller would otherwise write</i>, and
/// a caller checking whether a path is absolute writes the plain check. Appending a separator is the
/// library's own way of handling edge cases, so it belongs on the library's side of the comparison,
/// which is exactly what the ratio is meant to report.
/// </para>
/// </remarks>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class PathAbstractionCostBenchmarks
{
	/// <summary>Operations per invocation.</summary>
	private const int Operations = 64;

	private string absoluteFileText = "";
	private string relativeFileText = "";
	private string baseDirectoryText = "";
	private AbsoluteFilePath absoluteFile = null!;
	private RelativeFilePath relativeFile = null!;
	private AbsoluteDirectoryPath baseDirectory = null!;

	/// <summary>Prepares both sides of every pair, holding the same values.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFileText = PathSpecimens.AbsoluteFile;
		relativeFileText = PathSpecimens.RelativeFile;
		baseDirectoryText = PathSpecimens.AbsoluteDirectory;

		absoluteFile = AbsoluteFilePath.Create(absoluteFileText);
		relativeFile = RelativeFilePath.Create(relativeFileText);
		baseDirectory = AbsoluteDirectoryPath.Create(baseDirectoryText);
	}

	/// <summary>Extracting a file name with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("FileName")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareFileName()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetFileName(absoluteFileText).Length;
		}

		return accumulator;
	}

	/// <summary>Extracting it through the path type, which validates the result.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("FileName")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticFileName()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += absoluteFile.FileName.Length;
		}

		return accumulator;
	}

	/// <summary>Resolving a relative path with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsAbsolute")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareAsAbsolute()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetFullPath(relativeFileText, baseDirectoryText).Length;
		}

		return accumulator;
	}

	/// <summary>Resolving it through the path type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsAbsolute")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticAsAbsolute()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += relativeFile.AsAbsolute(baseDirectory).Length;
		}

		return accumulator;
	}

	/// <summary>Relativizing with the runtime's own helper.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsRelative")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareAsRelative()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += Path.GetRelativePath(baseDirectoryText, absoluteFileText).Length;
		}

		return accumulator;
	}

	/// <summary>Relativizing through the path type.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("AsRelative")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticAsRelative()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += absoluteFile.AsRelative(baseDirectory).Length;
		}

		return accumulator;
	}

	/// <summary>
	/// Checking a path is rooted by hand, which is what creation validates — modulo the separator the
	/// validator appends before asking.
	/// </summary>
	/// <returns>The count of rooted paths, which is every iteration.</returns>
	[BenchmarkCategory("Create")]
	[Benchmark(Baseline = true, OperationsPerInvoke = Operations)]
	public int BareCreate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			if (Path.IsPathFullyQualified(absoluteFileText))
			{
				accumulator++;
			}
		}

		return accumulator;
	}

	/// <summary>Building the path type, which asks the same question and keeps the answer.</summary>
	/// <returns>The accumulated length.</returns>
	[BenchmarkCategory("Create")]
	[Benchmark(OperationsPerInvoke = Operations)]
	public int SemanticCreate()
	{
		int accumulator = 0;

		for (int i = 0; i < Operations; i++)
		{
			accumulator += AbsoluteFilePath.Create(absoluteFileText).Length;
		}

		return accumulator;
	}
}
