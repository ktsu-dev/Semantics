// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Base interface for all path types (both absolute and relative, files and directories)
/// </summary>
public interface IPath
{
}

/// <summary>
/// Interface for absolute paths (paths with a root)
/// </summary>
public interface IAbsolutePath : IPath
{
}

/// <summary>
/// Interface for relative paths (paths without a root)
/// </summary>
public interface IRelativePath : IPath
{
}

/// <summary>
/// Interface for file paths (paths to files)
/// </summary>
public interface IFilePath : IPath
{
}

/// <summary>
/// Interface for directory paths (paths to directories)
/// </summary>
public interface IDirectoryPath : IPath
{
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

/// <summary>
/// Interface for absolute file paths
/// </summary>
public interface IAbsoluteFilePath : IFilePath, IAbsolutePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteFilePath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> instance with the same path value.</returns>
	public AbsoluteFilePath AsAbsoluteFilePath();
}

/// <summary>
/// Interface for relative file paths
/// </summary>
public interface IRelativeFilePath : IFilePath, IRelativePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="RelativeFilePath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeFilePath"/> instance with the same path value.</returns>
	public RelativeFilePath AsRelativeFilePath();
}

/// <summary>
/// Interface for absolute directory paths
/// </summary>
public interface IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> instance with the same path value.</returns>
	public AbsoluteDirectoryPath AsAbsoluteDirectoryPath();
}

/// <summary>
/// Interface for relative directory paths
/// </summary>
public interface IRelativeDirectoryPath : IDirectoryPath, IRelativePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="RelativeDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeDirectoryPath"/> instance with the same path value.</returns>
	public RelativeDirectoryPath AsRelativeDirectoryPath();
}

/// <summary>
/// Interface for filename types
/// </summary>
public interface IFileName
{
}

/// <summary>
/// Interface for file extension types
/// </summary>
public interface IFileExtension
{
}
