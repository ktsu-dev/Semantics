// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures building each of the path types from a well-formed string.
/// </summary>
/// <remarks>
/// A path type is a semantic string whose validator asks the runtime a question about the shape of
/// the value, so each row is the same reflection machinery
/// <c>StringCreationBenchmarks.Unvalidated</c> measures plus one such question. The absolute rows
/// ask <c>Path.IsPathFullyQualified</c> and the relative row asks its negation, so the pair is the
/// same work in both directions rather than two different validators.
/// </remarks>
[MemoryDiagnoser]
public class PathCreationBenchmarks
{
	private string absoluteFile = "";
	private string absoluteDirectory = "";
	private string relativeFile = "";
	private string fileName = "";

	/// <summary>Copies the inputs into fields.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFile = PathSpecimens.AbsoluteFile;
		absoluteDirectory = PathSpecimens.AbsoluteDirectory;
		relativeFile = PathSpecimens.RelativeFile;
		fileName = PathSpecimens.FileNameOnly;
	}

	/// <summary>Builds a fully qualified file path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public AbsoluteFilePath AbsoluteFilePath() =>
		Semantics.Paths.AbsoluteFilePath.Create(absoluteFile);

	/// <summary>Builds a relative file path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public RelativeFilePath RelativeFilePath() =>
		Semantics.Paths.RelativeFilePath.Create(relativeFile);

	/// <summary>Builds a fully qualified directory path.</summary>
	/// <returns>The created path.</returns>
	[Benchmark]
	public AbsoluteDirectoryPath AbsoluteDirectoryPath() =>
		Semantics.Paths.AbsoluteDirectoryPath.Create(absoluteDirectory);

	/// <summary>Builds a bare file name, whose validator checks for separators.</summary>
	/// <returns>The created file name.</returns>
	[Benchmark]
	public FileName FileNameType() => FileName.Create(fileName);
}
