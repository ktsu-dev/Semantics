// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Benchmarks.Paths;

using BenchmarkDotNet.Attributes;

using ktsu.Semantics.Paths;

/// <summary>
/// Measures the operations a path type offers over the string it holds.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two rows are here to be read against each other.</b>
/// <see cref="FileNameWithoutExtension"/> caches into a field on the path, so the second read and
/// every read after it is a field read. <see cref="FileName"/> does not: it builds a fresh
/// <c>FileName</c>, validation included, on every single read. Both are properties, both read like
/// field access at a call site, and they differ by the cost of a semantic string creation. Putting
/// them side by side is what makes that visible, and it is the kind of thing a release could
/// quietly change in either direction.
/// </para>
/// <para>
/// <b>Both conversions are measured in the direction that does work.</b>
/// <c>AbsoluteFilePath.AsAbsolute()</c> returns <c>this</c> and would measure nothing, so the
/// absolute direction is taken from a relative path and the relative direction from an absolute
/// one.
/// </para>
/// <para>
/// Nothing here touches the filesystem. <c>IsDirectory</c> and <c>IsFile</c> are excluded on
/// purpose: they call <c>Directory.Exists</c> and <c>File.Exists</c>, so they would measure the
/// disk and the state of the machine rather than this library.
/// </para>
/// </remarks>
[MemoryDiagnoser]
public class PathOperationBenchmarks
{
	private AbsoluteFilePath absoluteFile = null!;
	private RelativeFilePath relativeFile = null!;
	private AbsoluteDirectoryPath baseDirectory = null!;

	/// <summary>Builds the operands once, outside the measurement.</summary>
	[GlobalSetup]
	public void Setup()
	{
		absoluteFile = AbsoluteFilePath.Create(PathSpecimens.AbsoluteFile);
		relativeFile = RelativeFilePath.Create(PathSpecimens.RelativeFile);
		baseDirectory = AbsoluteDirectoryPath.Create(PathSpecimens.AbsoluteDirectory);
	}

	/// <summary>Reads the file name, which builds and validates a new one every time.</summary>
	/// <returns>The file name.</returns>
	[Benchmark]
	public FileName FileName() => absoluteFile.FileName;

	/// <summary>Reads the stem, which is cached into a field after the first read.</summary>
	/// <returns>The file name without its extension.</returns>
	[Benchmark]
	public FileName FileNameWithoutExtension() => absoluteFile.FileNameWithoutExtension;

	/// <summary>Reads the containing directory, which builds and validates a new path.</summary>
	/// <returns>The directory path.</returns>
	[Benchmark]
	public DirectoryPath DirectoryPath() => absoluteFile.DirectoryPath;

	/// <summary>Resolves a relative path against a base directory.</summary>
	/// <returns>The resolved absolute path.</returns>
	[Benchmark]
	public AbsoluteFilePath AsAbsolute() => relativeFile.AsAbsolute(baseDirectory);

	/// <summary>Expresses an absolute path relative to a base directory.</summary>
	/// <returns>The relative path.</returns>
	[Benchmark]
	public RelativeFilePath AsRelative() => absoluteFile.AsRelative(baseDirectory);

	/// <summary>Strips the extension, which rebuilds and revalidates the whole path.</summary>
	/// <returns>The path without its extension.</returns>
	[Benchmark]
	public AbsoluteFilePath RemoveExtension() => absoluteFile.RemoveExtension();
}
