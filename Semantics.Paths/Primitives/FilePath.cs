// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a file path (path to a file)
/// </summary>
[IsPath, IsFilePath]
public sealed record FilePath : SemanticFilePath<FilePath>, IFilePath
{
	/// <summary>
	/// Converts this file path to an absolute file path representation.
	/// Converts this path to absolute using the current working directory if it's relative.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path to this file.</returns>
	public AbsoluteFilePath AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsoluteFilePath.Create<AbsoluteFilePath>(absolutePath);
	}

	/// <summary>
	/// Converts this file path to a relative file path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to make this path relative to.</param>
	/// <returns>A <see cref="RelativeFilePath"/> representing the relative path from the base directory.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public RelativeFilePath AsRelative(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string absolutePath = Path.GetFullPath(WeakString);
		string relativePath = Path.GetRelativePath(baseDirectory.WeakString, absolutePath);
		return RelativeFilePath.Create<RelativeFilePath>(relativePath);
	}
}
