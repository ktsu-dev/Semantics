// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Represents a relative file path
/// </summary>
[IsRelativePath]
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
			return _cachedDirectoryPath ??= RelativeDirectoryPath.Create<RelativeDirectoryPath>(
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
			return _cachedFileNameWithoutExtension ??= FileName.Create<FileName>(
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
		Ensure.NotNull(newExtension);

		string newPath = Path.ChangeExtension(WeakString, newExtension.WeakString);
		return Create<RelativeFilePath>(newPath);
	}

	/// <summary>
	/// Removes the file extension from this path.
	/// </summary>
	/// <returns>A new <see cref="RelativeFilePath"/> without an extension.</returns>
	public RelativeFilePath RemoveExtension()
	{
		string pathWithoutExtension = Path.ChangeExtension(WeakString, null) ?? "";
		return Create<RelativeFilePath>(pathWithoutExtension);
	}

	/// <summary>
	/// Converts this relative file path to an absolute file path representation.
	/// This resolves the relative path against the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path to this file.</returns>
	public AbsoluteFilePath AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsoluteFilePath.Create<AbsoluteFilePath>(absolutePath);
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
	/// Converts this relative file path to an absolute file path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to resolve this relative path against.</param>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path to this file.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public AbsoluteFilePath AsAbsolute(AbsoluteDirectoryPath baseDirectory)
	{
		Ensure.NotNull(baseDirectory);
#if NETSTANDARD2_0
		string absolutePath = PathPolyfill.GetFullPath(WeakString, baseDirectory.WeakString);
#else
		string absolutePath = Path.GetFullPath(WeakString, baseDirectory.WeakString);
#endif
		return AbsoluteFilePath.Create<AbsoluteFilePath>(absolutePath);
	}

	/// <summary>
	/// Converts this relative file path to a relative file path using the specified base directory.
	/// Since this is already a relative path, returns itself.
	/// </summary>
	/// <param name="baseDirectory">The base directory (ignored since this is already relative).</param>
	/// <returns>This <see cref="RelativeFilePath"/> instance.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public RelativeFilePath AsRelative(AbsoluteDirectoryPath baseDirectory)
	{
		Ensure.NotNull(baseDirectory);
		return this;
	}
}
