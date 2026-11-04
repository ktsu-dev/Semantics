// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

/// <summary>
/// Represents a relative directory path
/// </summary>
[IsRelativePath]
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath
{
	/// <summary>
	/// Determines whether this path and <paramref name="other"/> are the same path.
	/// </summary>
	/// <param name="other">The path to compare with, or <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if both are relative directory paths with the same text; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// Declared explicitly, rather than left to the record's generated equality, because that would compare the
	/// private memoization fields below as well — so reading a derived property would change both equality and the
	/// hash code while leaving the text untouched, moving the instance out of its own hash bucket. A path's identity
	/// is its text. See <see href="https://github.com/ktsu-dev/Semantics/issues/265"/>.
	/// </remarks>
	public bool Equals(RelativeDirectoryPath? other) => base.Equals(other);

	/// <inheritdoc cref="Equals(RelativeDirectoryPath)"/>
	public override int GetHashCode() => base.GetHashCode();

	// Cache for expensive parent directory computation. Excluded from equality by the members above.
	private RelativeDirectoryPath? _cachedParent;

	/// <summary>
	/// Gets the parent directory of this relative directory path.
	/// </summary>
	/// <value>
	/// A <see cref="RelativeDirectoryPath"/> naming the directory that contains this one: the path with its last
	/// segment removed, or <c>.</c> (the starting directory, as <see cref="Normalize"/> writes it)
	/// for a single-segment path. Paths are normalized lexically first, so the
	/// parent of <c>a/..</c> is <c>..</c>, and a path that names the starting directory or one above it climbs
	/// further instead: the parent of <c>.</c> (or the empty path) is <c>..</c>, and the parent of <c>..</c> is
	/// <c>../..</c>.
	/// </value>
	public RelativeDirectoryPath Parent => _cachedParent ??= ComputeParent();

	private RelativeDirectoryPath ComputeParent()
	{
		string normalized = Normalize().WeakString;
		string separator = Path.DirectorySeparatorChar.ToString();

		// Nothing to drop: the path names the starting directory, so its parent is one level up.
		if (normalized.Length == 0 || normalized == ".")
		{
			return Create<RelativeDirectoryPath>("..");
		}

		// Normalize leaves ".." only as a leading run, so the last segment decides whether there is a
		// named directory to drop or the path only climbs and the parent must climb one level further.
		string[] segments = normalized.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
		if (segments[^1] == "..")
		{
			return Create<RelativeDirectoryPath>(normalized + separator + "..");
		}

		// One segment leaves the starting directory, which is "." rather than "": the empty path is
		// rejected by Path.GetFullPath and reads as missing to Directory.Exists.
		return segments.Length == 1
			? Create<RelativeDirectoryPath>(".")
			: Create<RelativeDirectoryPath>(string.Join(separator, segments, 0, segments.Length - 1));
	}

	// Cache for directory name
	private DirectoryName? _cachedName;

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="DirectoryName"/> representing just the directory name.</value>
	public DirectoryName Name => _cachedName ??= DirectoryName.Create<DirectoryName>(Path.GetFileName(WeakString) ?? "");

	// Cache for depth calculation
	private int? _cachedDepth;

	/// <summary>
	/// Gets the depth of this relative directory path: the number of levels below its first segment.
	/// </summary>
	/// <value>
	/// 0 for <c>a</c>, 1 for <c>a/b</c>, 2 for <c>a/b/c</c>. The path is normalized first, the same way as
	/// <see cref="Normalize"/>, so <c>./a</c> has the same depth as <c>a</c>, a trailing separator does not add a
	/// level, and <c>a/..</c>, <c>.</c> and the empty path, which all name the starting directory, have depth 0.
	/// </value>
	public int Depth => _cachedDepth ??= CalculateDepth();

	/// <summary>
	/// Counts the segments of the normalized path, less the first.
	/// </summary>
	/// <returns>The directory depth.</returns>
	private int CalculateDepth()
	{
		string normalized = Normalize().WeakString;
		if (normalized.Length == 0 || normalized == ".")
		{
			return 0;
		}

		return SpanPathUtilities.CountSegments(normalized.AsSpan()) - 1;
	}

	/// <summary>
	/// Creates a relative file path for files in this directory.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>A <see cref="RelativeFilePath"/> object.</returns>
	protected override IFilePath CreateFilePath(string filePath) =>
		RelativeFilePath.Create<RelativeFilePath>(filePath);

	/// <summary>
	/// Creates a relative directory path for subdirectories in this directory.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>A <see cref="RelativeDirectoryPath"/> object.</returns>
	protected override IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		Create<RelativeDirectoryPath>(directoryPath);

	/// <summary>
	/// Combines a relative directory path with another relative directory path using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The relative directory path to append.</param>
	/// <returns>A new <see cref="RelativeDirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeDirectoryPath operator /(RelativeDirectoryPath left, RelativeDirectoryPath right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return Create<RelativeDirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines a relative directory path with a relative file path using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The relative file path to append.</param>
	/// <returns>A new <see cref="RelativeFilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeFilePath operator /(RelativeDirectoryPath left, RelativeFilePath right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return RelativeFilePath.Create<RelativeFilePath>(combinedPath);
	}

	/// <summary>
	/// Combines a relative directory path with a directory name using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The directory name to append.</param>
	/// <returns>A new <see cref="RelativeDirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeDirectoryPath operator /(RelativeDirectoryPath left, DirectoryName right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return Create<RelativeDirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines a relative directory path with a file name using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The file name to append.</param>
	/// <returns>A new <see cref="RelativeFilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeFilePath operator /(RelativeDirectoryPath left, FileName right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return RelativeFilePath.Create<RelativeFilePath>(combinedPath);
	}

	/// <summary>
	/// Converts this relative directory path to an absolute directory path representation.
	/// This resolves the relative path against the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path to this directory.</returns>
	public AbsoluteDirectoryPath AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(absolutePath);
	}

	/// <summary>
	/// Converts this relative directory path to an absolute directory path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to resolve this relative path against.</param>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path to this directory.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public AbsoluteDirectoryPath AsAbsolute(AbsoluteDirectoryPath baseDirectory)
	{
		Ensure.NotNull(baseDirectory);
#if NETSTANDARD2_0
		string absolutePath = PathPolyfill.GetFullPath(WeakString, baseDirectory.WeakString);
#else
		string absolutePath = Path.GetFullPath(WeakString, baseDirectory.WeakString);
#endif
		return AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(absolutePath);
	}

	/// <summary>
	/// Explicitly implements IRelativePath.AsAbsolute() to return the base AbsolutePath type.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing this absolute path.</returns>
	AbsolutePath IRelativePath.AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsolutePath.Create<AbsolutePath>(absolutePath);
	}

	/// <summary>
	/// Converts this relative directory path to a relative directory path using the specified base directory.
	/// Since this is already a relative path, returns itself.
	/// </summary>
	/// <param name="baseDirectory">The base directory (ignored since this is already relative).</param>
	/// <returns>This <see cref="RelativeDirectoryPath"/> instance.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public RelativeDirectoryPath AsRelative(AbsoluteDirectoryPath baseDirectory)
	{
		Ensure.NotNull(baseDirectory);

		return this;
	}

	/// <summary>
	/// Normalizes this relative path by resolving any "." and ".." components.
	/// </summary>
	/// <returns>A new <see cref="RelativeDirectoryPath"/> with normalized path components.</returns>
	public new RelativeDirectoryPath Normalize()
	{
		string path = WeakString;
		if (string.IsNullOrEmpty(path))
		{
			return this;
		}

		// Resolve segments lexically. Resolving against a dummy root would discard every ".." that
		// climbs above the starting point, changing which directory the path names.
		List<string> segments = [];
		foreach (string segment in path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries))
		{
			if (segment == ".")
			{
				continue;
			}

			if (segment == ".." && segments.Count > 0 && segments[^1] != "..")
			{
				segments.RemoveAt(segments.Count - 1);
				continue;
			}

			segments.Add(segment);
		}

		string normalized = segments.Count == 0 ? "." : string.Join(Path.DirectorySeparatorChar.ToString(), segments);

		return Create<RelativeDirectoryPath>(normalized);
	}
}
