// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for directory paths (paths to directories)
/// </summary>
public interface IDirectoryPath : IPath
{
	/// <summary>
	/// Converts this directory path to an absolute directory path representation.
	/// For relative paths, this resolves against the current working directory.
	/// For absolute paths, this returns the path itself.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path to this directory.</returns>
	/// <remarks>
	/// This method provides a consistent way to obtain an absolute path regardless of whether
	/// the underlying implementation is relative or absolute. For relative paths, the conversion
	/// uses the current working directory as the base.
	/// </remarks>
	public AbsoluteDirectoryPath AsAbsolute();

	/// <summary>
	/// Gets the files and directories contained in this directory as semantic path types.
	/// Files are returned as the appropriate file path type, and directories as the appropriate directory path type.
	/// </summary>
	/// <value>
	/// A collection of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty collection if the directory doesn't exist or cannot be accessed.
	/// </value>
	public IEnumerable<IPath> Contents { get; }

	/// <summary>
	/// Asynchronously enumerates the files and directories contained in this directory as semantic path types.
	/// This is more efficient for large directories as it streams results instead of loading everything into memory.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
	/// <returns>
	/// An async enumerable of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty enumerable if the directory doesn't exist or cannot be accessed.
	/// </returns>
	public IAsyncEnumerable<IPath> GetContentsAsync(CancellationToken cancellationToken = default);
}
