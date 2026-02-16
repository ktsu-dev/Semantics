// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;
nusing ktsu.Semantics.Strings;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a relative directory path
/// </summary>
[IsPath, IsRelativePath, IsDirectoryPath]
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath
{
	// Cache for expensive parent directory computation
	private RelativeDirectoryPath? _cachedParent;

	/// <summary>
	/// Gets the parent directory of this relative directory path.
	/// </summary>
	/// <value>A <see cref="RelativeDirectoryPath"/> representing the parent directory, or an empty path if this is a root-relative directory.</value>
	public RelativeDirectoryPath Parent => _cachedParent ??= Create<RelativeDirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");

	// Cache for directory name
	private FileName? _cachedName;

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="FileName"/> representing just the directory name.</value>
	public FileName Name => _cachedName ??= FileName.Create<FileName>(Path.GetFileName(WeakString) ?? "");

	// Cache for depth calculation
	private int? _cachedDepth;

	/// <summary>
	/// Gets the depth of this relative directory path (number of directory levels).
	/// </summary>
	/// <value>The depth of the directory path.</value>
	public int Depth => _cachedDepth ??= CalculateDepth(WeakString);

	/// <summary>
	/// Calculates directory depth using span semantics for optimal performance.
	/// </summary>
	/// <param name="path">The path to analyze.</param>
	/// <returns>The directory depth.</returns>
	private static int CalculateDepth(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return 0;
		}

		ReadOnlySpan<char> span = path.AsSpan();
		int depth = 0;
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] == Path.DirectorySeparatorChar || span[i] == Path.AltDirectorySeparatorChar)
			{
				depth++;
			}
		}
		return depth;
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
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

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
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return RelativeFilePath.Create<RelativeFilePath>(combinedPath);
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
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

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
	/// Explicitly implements IRelativePath.AsAbsolute() to return the base AbsolutePath type.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing this absolute path.</returns>
	AbsolutePath IRelativePath.AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsolutePath.Create<AbsolutePath>(absolutePath);
	}

	/// <summary>
	/// Converts this relative directory path to an absolute directory path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to resolve this relative path against.</param>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path to this directory.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public AbsoluteDirectoryPath AsAbsolute(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string absolutePath = Path.GetFullPath(WeakString, baseDirectory.WeakString);
		return AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(absolutePath);
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
		ArgumentNullException.ThrowIfNull(baseDirectory);
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

		// Use Path.GetFullPath with a dummy base to normalize relative paths
		string dummyBase = OperatingSystem.IsWindows() ? "C:\\" : "/";
		string fullPath = Path.GetFullPath(Path.Combine(dummyBase, path));
		string normalized = Path.GetRelativePath(dummyBase, fullPath);

		return Create<RelativeDirectoryPath>(normalized);
	}

	/// <summary>
	/// Asynchronously enumerates the files and directories contained in this directory as semantic path types.
	/// This is more efficient for large directories as it streams results instead of loading everything into memory.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
	/// <returns>
	/// An async enumerable of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty enumerable if the directory doesn't exist or cannot be accessed.
	/// </returns>
	public async IAsyncEnumerable<IPath> GetContentsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		string directoryPath = WeakString;
		if (!Directory.Exists(directoryPath))
		{
			yield break;
		}

		// Use Task.Run to avoid blocking the caller while enumerating
		IEnumerable<string> entries = await Task.Run(() => Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.TopDirectoryOnly), cancellationToken).ConfigureAwait(false);

		foreach (string item in entries)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (Directory.Exists(item))
			{
				yield return CreateDirectoryPath(item);
			}
			else if (File.Exists(item))
			{
				yield return CreateFilePath(item);
			}
		}
	}
}
