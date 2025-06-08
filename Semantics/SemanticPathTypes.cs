// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Base interface for all path types
/// </summary>
public interface IPath
{
}

/// <summary>
/// Interface for absolute (fully qualified) paths
/// </summary>
public interface IAbsolutePath : IPath
{
}

/// <summary>
/// Interface for relative (not fully qualified) paths
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
}

/// <summary>
/// Interface for absolute file paths
/// </summary>
public interface IAbsoluteFilePath : IFilePath, IAbsolutePath
{
}

/// <summary>
/// Interface for relative file paths
/// </summary>
public interface IRelativeFilePath : IFilePath, IRelativePath
{
}

/// <summary>
/// Interface for absolute directory paths
/// </summary>
public interface IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
{
}

/// <summary>
/// Interface for relative directory paths
/// </summary>
public interface IRelativeDirectoryPath : IDirectoryPath, IRelativePath
{
}

/// <summary>
/// Interface for filenames (without directory path)
/// </summary>
public interface IFileName
{
}

/// <summary>
/// Interface for file extensions (starts with a period)
/// </summary>
public interface IFileExtension
{
}

/// <summary>
/// Represents an absolute (fully qualified) path
/// </summary>
[IsPath, IsAbsolutePath]
public sealed record AbsolutePath : SemanticAbsolutePath<AbsolutePath>, IAbsolutePath
{
}

/// <summary>
/// Represents a relative (not fully qualified) path
/// </summary>
[IsPath, IsRelativePath]
public sealed record RelativePath : SemanticRelativePath<RelativePath>, IRelativePath
{
}

/// <summary>
/// Represents a file path (path to a file)
/// </summary>
[IsPath, IsFilePath]
public sealed record FilePath : SemanticFilePath<FilePath>, IFilePath
{
}

/// <summary>
/// Represents an absolute file path
/// </summary>
[IsPath, IsAbsolutePath, IsFilePath]
public sealed record AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
{
}

/// <summary>
/// Represents a relative file path
/// </summary>
[IsPath, IsRelativePath, IsFilePath]
public sealed record RelativeFilePath : SemanticFilePath<RelativeFilePath>, IRelativeFilePath
{
}

/// <summary>
/// Represents a directory path (path to a directory)
/// </summary>
[IsPath, IsDirectoryPath]
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
{
}

/// <summary>
/// Represents an absolute directory path
/// </summary>
[IsPath, IsAbsolutePath, IsDirectoryPath]
public sealed record AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
{
	/// <summary>
	/// Creates an absolute file path for files in this directory.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>An <see cref="AbsoluteFilePath"/> object.</returns>
	protected override IFilePath CreateFilePath(string filePath) =>
		AbsoluteFilePath.FromString<AbsoluteFilePath>(filePath);

	/// <summary>
	/// Creates an absolute directory path for subdirectories in this directory.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> object.</returns>
	protected override IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		FromString<AbsoluteDirectoryPath>(directoryPath);
}

/// <summary>
/// Represents a relative directory path
/// </summary>
[IsPath, IsRelativePath, IsDirectoryPath]
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath
{
	/// <summary>
	/// Creates a relative file path for files in this directory.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>A <see cref="RelativeFilePath"/> object.</returns>
	protected override IFilePath CreateFilePath(string filePath) =>
		RelativeFilePath.FromString<RelativeFilePath>(filePath);

	/// <summary>
	/// Creates a relative directory path for subdirectories in this directory.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>A <see cref="RelativeDirectoryPath"/> object.</returns>
	protected override IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		FromString<RelativeDirectoryPath>(directoryPath);
}

/// <summary>
/// Represents a filename (without directory path)
/// </summary>
[IsFileName]
public sealed record FileName : SemanticString<FileName>, IFileName
{
}

/// <summary>
/// Represents a file extension (starts with a period)
/// </summary>
[IsExtension]
public sealed record FileExtension : SemanticString<FileExtension>, IFileExtension
{
}
