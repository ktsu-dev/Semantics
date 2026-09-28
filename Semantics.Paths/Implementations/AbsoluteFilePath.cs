// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Represents an absolute file path
/// </summary>
[IsAbsolutePath]
public sealed record AbsoluteFilePath : SemanticFilePath<AbsoluteFilePath>, IAbsoluteFilePath
{
	/// <summary>
	/// Determines whether this path and <paramref name="other"/> are the same path.
	/// </summary>
	/// <param name="other">The path to compare with, or <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if both are absolute file paths with the same text; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// Declared explicitly, rather than left to the record's generated equality, because that would compare the
	/// private memoization fields below as well — so reading a derived property would change both equality and the
	/// hash code while leaving the text untouched, moving the instance out of its own hash bucket. A path's identity
	/// is its text. See <see href="https://github.com/ktsu-dev/Semantics/issues/265"/>.
	/// </remarks>
	public bool Equals(AbsoluteFilePath? other) => base.Equals(other);

	/// <inheritdoc cref="Equals(AbsoluteFilePath)"/>
	public override int GetHashCode() => base.GetHashCode();

	// Cache for expensive directory path computation. Excluded from equality by the members above.
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
		Ensure.NotNull(newExtension);

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
	/// Determines whether this file is inside the specified parent directory.
	/// </summary>
	/// <param name="parentPath">The potential parent path to check against.</param>
	/// <returns><see langword="true"/> if this path is a child of the parent path; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// Both paths are normalized before comparison to handle different separator styles. Case is
	/// ignored on Windows and significant on every other platform. Every file below a filesystem
	/// root is a child of that root.
	/// </remarks>
	public bool IsChildOf(AbsoluteDirectoryPath parentPath)
	{
		Ensure.NotNull(parentPath);

		return PathContainment.IsStrictlyInside(Path.GetFullPath(WeakString), Path.GetFullPath(parentPath.WeakString));
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
		Ensure.NotNull(baseDirectory);
#if NETSTANDARD2_0
		string relativePath = PathPolyfill.GetRelativePath(baseDirectory.WeakString, WeakString);
#else
		string relativePath = Path.GetRelativePath(baseDirectory.WeakString, WeakString);
#endif
		return RelativeFilePath.Create<RelativeFilePath>(relativePath);
	}
}
