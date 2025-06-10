// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a relative file path
/// </summary>
[IsPath, IsRelativePath, IsFilePath]
public sealed record RelativeFilePath : SemanticFilePath<RelativeFilePath>, IRelativeFilePath
{
	// Cache for expensive directory path computation
	private RelativeDirectoryPath? _cachedDirectoryPath;

	/// <summary>
	/// Gets the directory portion of this relative file path as a <see cref="RelativeDirectoryPath"/>.
	/// </summary>
	/// <value>A <see cref="RelativeDirectoryPath"/> representing the directory containing this file.</value>
	public RelativeDirectoryPath RelativeDirectoryPath
	{
		get
		{
			return _cachedDirectoryPath ??= RelativeDirectoryPath.FromString<RelativeDirectoryPath>(
				Path.GetDirectoryName(WeakString) ?? "");
		}
	}

	// Cache for filename without extension
	private FileName? _cachedFileNameWithoutExtension;

	/// <summary>
	/// Gets the filename without extension.
	/// </summary>
	/// <value>A <see cref="FileName"/> representing the filename without its extension.</value>
	public FileName FileNameWithoutExtension
	{
		get
		{
			return _cachedFileNameWithoutExtension ??= FileName.FromString<FileName>(
				Path.GetFileNameWithoutExtension(WeakString) ?? "");
		}
	}

	/// <summary>
	/// Changes the file extension of this path.
	/// </summary>
	/// <param name="newExtension">The new file extension (including the dot).</param>
	/// <returns>A new <see cref="RelativeFilePath"/> with the changed extension.</returns>
	public RelativeFilePath ChangeExtension(FileExtension newExtension)
	{
		ArgumentNullException.ThrowIfNull(newExtension);
		string newPath = Path.ChangeExtension(WeakString, newExtension.WeakString);
		return FromString<RelativeFilePath>(newPath);
	}

	/// <summary>
	/// Removes the file extension from this path.
	/// </summary>
	/// <returns>A new <see cref="RelativeFilePath"/> without an extension.</returns>
	public RelativeFilePath RemoveExtension()
	{
		string pathWithoutExtension = Path.ChangeExtension(WeakString, null) ?? "";
		return FromString<RelativeFilePath>(pathWithoutExtension);
	}

	/// <summary>
	/// Converts this relative file path to an absolute file path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to resolve this relative path against.</param>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path.</returns>
	public AbsoluteFilePath ToAbsolute(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string absolutePath = Path.GetFullPath(Path.Combine(baseDirectory.WeakString, WeakString));
		return AbsoluteFilePath.FromString<AbsoluteFilePath>(absolutePath);
	}

	/// <summary>
	/// Converts this interface to its concrete <see cref="RelativeFilePath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeFilePath"/> instance with the same path value.</returns>
	public RelativeFilePath AsRelativeFilePath() => this;
}
