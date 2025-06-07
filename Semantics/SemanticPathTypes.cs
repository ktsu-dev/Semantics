// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents any path
/// </summary>
[IsPath]
public sealed record Path : SemanticPath<Path>
{
}

/// <summary>
/// Represents an absolute (fully qualified) path
/// </summary>
[IsPath, IsAbsolutePath]
public sealed record AbsolutePath : SemanticAbsolutePath<AbsolutePath>
{
}

/// <summary>
/// Represents a relative (not fully qualified) path
/// </summary>
[IsPath, IsRelativePath]
public sealed record RelativePath : SemanticRelativePath<RelativePath>
{
}

/// <summary>
/// Represents a file path (path to a file)
/// </summary>
[IsPath, IsFilePath]
public sealed record FilePath : SemanticFilePath<FilePath>
{
}

/// <summary>
/// Represents an absolute file path
/// </summary>
[IsPath, IsAbsolutePath, IsFilePath]
public sealed record AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>
{
}

/// <summary>
/// Represents a relative file path
/// </summary>
[IsPath, IsRelativePath, IsFilePath]
public sealed record RelativeFilePath : SemanticFilePath<RelativeFilePath>
{
}

/// <summary>
/// Represents a directory path (path to a directory)
/// </summary>
[IsPath, IsDirectoryPath]
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>
{
}

/// <summary>
/// Represents an absolute directory path
/// </summary>
[IsPath, IsAbsolutePath, IsDirectoryPath]
public sealed record AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>
{
}

/// <summary>
/// Represents a relative directory path
/// </summary>
[IsPath, IsRelativePath, IsDirectoryPath]
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>
{
}

/// <summary>
/// Represents a filename (without directory path)
/// </summary>
[IsFileName]
public sealed record FileName : SemanticString<FileName>
{
}

/// <summary>
/// Represents a file extension (starts with a period)
/// </summary>
[IsExtension]
public sealed record FileExtension : SemanticString<FileExtension>
{
}
