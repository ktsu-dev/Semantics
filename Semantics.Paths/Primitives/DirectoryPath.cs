// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a directory path (path to a directory)
/// </summary>
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
{
	/// <summary>
	/// Gets the parent directory of this directory path.
	/// </summary>
	/// <value>
	/// A <see cref="DirectoryPath"/> naming the directory that contains this one, or an empty path when this path
	/// resolves to a root directory.
	/// </value>
	/// <remarks>
	/// <c>.</c> and <c>..</c> segments are resolved before the last segment is dropped, so the parent of
	/// <c>/home/user/..</c> is <c>/</c> rather than <c>/home/user</c>. A rooted path is resolved as
	/// <see cref="AbsoluteDirectoryPath.Parent"/> resolves it, and a relative one is normalized lexically as
	/// <see cref="RelativeDirectoryPath.Parent"/> normalizes it, so it is never resolved against the working
	/// directory: the parent of <c>a/..</c> is <c>..</c>, and the parent of <c>..</c> is <c>../..</c>.
	/// </remarks>
	public DirectoryPath Parent
	{
		get
		{
			if (Path.IsPathRooted(WeakString))
			{
				string? parent = Path.GetDirectoryName(Path.GetFullPath(WeakString));
				return Create<DirectoryPath>(parent ?? "");
			}

			return Create<DirectoryPath>(RelativeDirectoryPath.Create<RelativeDirectoryPath>(WeakString).Parent.WeakString);
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
		Ensure.NotNull(baseDirectory);

		string absolutePath = Path.GetFullPath(WeakString);
#if NETSTANDARD2_0
		string relativePath = PathPolyfill.GetRelativePath(baseDirectory.WeakString, absolutePath);
#else
		string relativePath = Path.GetRelativePath(baseDirectory.WeakString, absolutePath);
#endif
		return RelativeDirectoryPath.Create<RelativeDirectoryPath>(relativePath);
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
		Ensure.NotNull(left);
		Ensure.NotNull(right);

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
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return FilePath.Create<FilePath>(combinedPath);
	}

	/// <summary>
	/// Combines a directory path with a directory name using the '/' operator.
	/// </summary>
	/// <param name="left">The base directory path.</param>
	/// <param name="right">The directory name to append.</param>
	/// <returns>A new <see cref="DirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static DirectoryPath operator /(DirectoryPath left, DirectoryName right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return Create<DirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines a directory path with a file name using the '/' operator.
	/// </summary>
	/// <param name="left">The base directory path.</param>
	/// <param name="right">The file name to append.</param>
	/// <returns>A new <see cref="FilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static FilePath operator /(DirectoryPath left, FileName right)
	{
		Ensure.NotNull(left);
		Ensure.NotNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return FilePath.Create<FilePath>(combinedPath);
	}
}
