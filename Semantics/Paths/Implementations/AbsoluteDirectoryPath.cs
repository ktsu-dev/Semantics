// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents an absolute directory path
/// </summary>
[IsPath, IsAbsolutePath, IsDirectoryPath]
public sealed record AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
{
	// Cache for expensive parent directory computation
	private AbsoluteDirectoryPath? _cachedParent;

	/// <summary>
	/// Gets the parent directory of this absolute directory path.
	/// </summary>
	/// <value>An <see cref="AbsoluteDirectoryPath"/> representing the parent directory, or an empty path if this is a root directory.</value>
	public AbsoluteDirectoryPath Parent
	{
		get
		{
			return _cachedParent ??= Create<AbsoluteDirectoryPath>(
				InternedPathStrings.InternIfCommon(Path.GetDirectoryName(WeakString) ?? InternedPathStrings.Empty));
		}
	}

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
	/// Gets the depth of this directory path (number of directory separators).
	/// For example, "a/b/c" has depth 2, "a" has depth 0.
	/// </summary>
	/// <value>The number of directory separators in the path.</value>
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

	// Cache for root check
	private bool? _cachedIsRoot;

	/// <summary>
	/// Determines whether this directory is a root directory.
	/// </summary>
	/// <value><c>true</c> if this is a root directory; otherwise, <c>false</c>.</value>
	public bool IsRoot => _cachedIsRoot ??= Path.GetPathRoot(WeakString) == WeakString;

	/// <summary>
	/// Creates an absolute file path for files in this directory.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>An <see cref="AbsoluteFilePath"/> object.</returns>
	protected override IFilePath CreateFilePath(string filePath) =>
		AbsoluteFilePath.Create<AbsoluteFilePath>(filePath);

	/// <summary>
	/// Creates an absolute directory path for subdirectories in this directory.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> object.</returns>
	protected override IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		Create<AbsoluteDirectoryPath>(directoryPath);

	/// <summary>
	/// Combines an absolute directory path with a relative directory path using the '/' operator.
	/// </summary>
	/// <param name="left">The base absolute directory path.</param>
	/// <param name="right">The relative directory path to append.</param>
	/// <returns>A new <see cref="AbsoluteDirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static AbsoluteDirectoryPath operator /(AbsoluteDirectoryPath left, RelativeDirectoryPath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return Create<AbsoluteDirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines an absolute directory path with a relative file path using the '/' operator.
	/// </summary>
	/// <param name="left">The base absolute directory path.</param>
	/// <param name="right">The relative file path to append.</param>
	/// <returns>A new <see cref="AbsoluteFilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static AbsoluteFilePath operator /(AbsoluteDirectoryPath left, RelativeFilePath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths(left.WeakString, right.WeakString);
		return AbsoluteFilePath.Create<AbsoluteFilePath>(combinedPath);
	}

	/// <summary>
	/// Determines whether this directory is a child of the specified parent path using efficient span comparison.
	/// </summary>
	/// <param name="parentPath">The potential parent path to check against.</param>
	/// <returns><see langword="true"/> if this path is a child of the parent path; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method uses span-based comparison for better performance than string concatenation.
	/// It normalizes both paths before comparison to handle different separator styles.
	/// </remarks>
	public bool IsChildOf(AbsoluteDirectoryPath parentPath)
	{
		ArgumentNullException.ThrowIfNull(parentPath);

		// Get normalized paths using span semantics for comparison
		ReadOnlySpan<char> thisPathSpan = Path.GetFullPath(WeakString).AsSpan();
		ReadOnlySpan<char> parentPathSpan = Path.GetFullPath(parentPath.WeakString).AsSpan();

		// A path cannot be a child of itself
		if (thisPathSpan.SequenceEqual(parentPathSpan))
		{
			return false;
		}

		// Check if this path starts with the parent path followed by a separator
		if (!thisPathSpan.StartsWith(parentPathSpan, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		// Ensure there's a separator after the parent path (not just a prefix match)
		int nextIndex = parentPathSpan.Length;
		return nextIndex < thisPathSpan.Length &&
			   (thisPathSpan[nextIndex] == Path.DirectorySeparatorChar ||
				thisPathSpan[nextIndex] == Path.AltDirectorySeparatorChar);
	}

	/// <summary>
	/// Determines whether this directory is a parent of the specified child path using efficient span comparison.
	/// </summary>
	/// <param name="childPath">The potential child path to check against.</param>
	/// <returns><see langword="true"/> if this path is a parent of the child path; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method uses span-based comparison for better performance than string concatenation.
	/// It normalizes both paths before comparison to handle different separator styles.
	/// </remarks>
	public bool IsParentOf(AbsoluteDirectoryPath childPath)
	{
		ArgumentNullException.ThrowIfNull(childPath);
		return childPath.IsChildOf(this);
	}

	/// <summary>
	/// Gets all parent directories from this directory up to the root.
	/// </summary>
	/// <returns>An enumerable of <see cref="AbsoluteDirectoryPath"/> representing all parent directories.</returns>
	public IEnumerable<AbsoluteDirectoryPath> GetAncestors()
	{
		AbsoluteDirectoryPath current = Parent;
		while (!string.IsNullOrEmpty(current.WeakString) && current != this)
		{
			yield return current;
			AbsoluteDirectoryPath next = current.Parent;
			if (next == current)
			{
				break; // Prevent infinite loop at root
			}
			current = next;
		}
	}

	/// <summary>
	/// Creates a relative directory path from this directory to another absolute directory.
	/// </summary>
	/// <param name="targetDirectory">The target directory.</param>
	/// <returns>A <see cref="RelativeDirectoryPath"/> from this directory to the target.</returns>
	public RelativeDirectoryPath GetRelativePathTo(AbsoluteDirectoryPath targetDirectory)
	{
		ArgumentNullException.ThrowIfNull(targetDirectory);
		// Use Path.GetRelativePath to compute the relative path
		string relativePath = Path.GetRelativePath(WeakString, targetDirectory.WeakString);
		return RelativeDirectoryPath.Create<RelativeDirectoryPath>(relativePath);
	}

	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> instance with the same path value.</returns>
	public AbsoluteDirectoryPath AsAbsoluteDirectoryPath() => this;

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
