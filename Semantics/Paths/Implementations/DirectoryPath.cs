// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a directory path (path to a directory)
/// </summary>
[IsPath, IsDirectoryPath]
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
{
	/// <summary>
	/// Gets the parent directory of this directory path.
	/// </summary>
	/// <value>A <see cref="DirectoryPath"/> representing the parent directory, or an empty path if this is a root directory.</value>
	public DirectoryPath Parent
	{
		get
		{
			string? parentPath = Path.GetDirectoryName(WeakString);
			return Create<DirectoryPath>(parentPath ?? "");
		}
	}

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="FileName"/> representing just the directory name.</value>
	public FileName Name => FileName.Create<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Converts this directory path to an absolute directory path representation.
	/// Converts this path to absolute using the current working directory if it's relative.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path to this directory.</returns>
	public AbsoluteDirectoryPath AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(absolutePath);
	}

	/// <summary>
	/// Converts this directory path to a relative directory path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to make this path relative to.</param>
	/// <returns>A <see cref="RelativeDirectoryPath"/> representing the relative path from the base directory.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public RelativeDirectoryPath AsRelative(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string absolutePath = Path.GetFullPath(WeakString);
		string relativePath = Path.GetRelativePath(baseDirectory.WeakString, absolutePath);
		return RelativeDirectoryPath.Create<RelativeDirectoryPath>(relativePath);
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
				yield return Create<DirectoryPath>(item);
			}
			else if (File.Exists(item))
			{
				yield return FilePath.Create<FilePath>(item);
			}
		}
	}

	/// <summary>
	/// Combines a directory path with a relative directory path using the '/' operator.
	/// </summary>
	/// <param name="left">The base directory path.</param>
	/// <param name="right">The relative directory path to append.</param>
	/// <returns>A new <see cref="DirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static DirectoryPath operator /(DirectoryPath left, RelativeDirectoryPath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return Create<DirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines a directory path with a relative file path using the '/' operator.
	/// </summary>
	/// <param name="left">The base directory path.</param>
	/// <param name="right">The relative file path to append.</param>
	/// <returns>A new <see cref="FilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static FilePath operator /(DirectoryPath left, RelativeFilePath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return FilePath.Create<FilePath>(combinedPath);
	}
}
