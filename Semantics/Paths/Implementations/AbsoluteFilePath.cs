// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an absolute file path
/// </summary>
[IsPath, IsAbsolutePath, IsFilePath]
public sealed record AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
{
	// Cache for expensive directory path computation
	private AbsoluteDirectoryPath? _cachedDirectoryPath;

	/// <summary>
	/// Gets the directory portion of this absolute file path as an <see cref="AbsoluteDirectoryPath"/>.
	/// </summary>
	/// <value>An <see cref="AbsoluteDirectoryPath"/> representing the directory containing this file.</value>
	public AbsoluteDirectoryPath AbsoluteDirectoryPath
	{
		get
		{
			return _cachedDirectoryPath ??= AbsoluteDirectoryPath.Create<AbsoluteDirectoryPath>(
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
	/// <returns>A new <see cref="AbsoluteFilePath"/> with the changed extension.</returns>
	public AbsoluteFilePath ChangeExtension(FileExtension newExtension)
	{
		ArgumentNullException.ThrowIfNull(newExtension);
		string newPath = Path.ChangeExtension(WeakString, newExtension.WeakString);
		return Create<AbsoluteFilePath>(newPath);
	}

	/// <summary>
	/// Removes the file extension from this path.
	/// </summary>
	/// <returns>A new <see cref="AbsoluteFilePath"/> without an extension.</returns>
	public AbsoluteFilePath RemoveExtension()
	{
		string pathWithoutExtension = Path.ChangeExtension(WeakString, null) ?? "";
		return Create<AbsoluteFilePath>(pathWithoutExtension);
	}

	/// <summary>
	/// Determines whether this path is a child of the specified parent path using efficient span comparison.
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
	/// Converts this file path to an absolute file path representation.
	/// Since this is already an absolute path, returns itself.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path to this file.</returns>
	public AbsoluteFilePath AsAbsolute() => this;

	/// <summary>
	/// Explicitly implements IAbsolutePath.AsAbsolute() to return the base AbsolutePath type.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing this absolute path.</returns>
	AbsolutePath IAbsolutePath.AsAbsolute() => AbsolutePath.Create<AbsolutePath>(WeakString);

	/// <summary>
	/// Converts this absolute file path to a relative file path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to make this path relative to.</param>
	/// <returns>A <see cref="RelativeFilePath"/> representing the relative path from the base directory.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="baseDirectory"/> is <see langword="null"/>.</exception>
	public RelativeFilePath AsRelative(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string relativePath = Path.GetRelativePath(baseDirectory.WeakString, WeakString);
		return RelativeFilePath.Create<RelativeFilePath>(relativePath);
	}
}
