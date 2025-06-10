// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides interned common strings for path operations to reduce memory allocations.
/// </summary>
internal static class InternedPathStrings
{
	/// <summary>
	/// Interned directory separator character as string.
	/// </summary>
	public static readonly string DirectorySeparator = string.Intern(Path.DirectorySeparatorChar.ToString());

	/// <summary>
	/// Interned alternative directory separator character as string.
	/// </summary>
	public static readonly string AltDirectorySeparator = string.Intern(Path.AltDirectorySeparatorChar.ToString());

	/// <summary>
	/// Interned empty string.
	/// </summary>
	public static readonly string Empty = string.Intern(string.Empty);

	/// <summary>
	/// Interned common path roots.
	/// </summary>
	public static readonly string WindowsRoot = string.Intern(@"C:\");
	public static readonly string UnixRoot = string.Intern("/");
	public static readonly string WindowsUncRoot = string.Intern(@"\\");

	/// <summary>
	/// Interns a string if it matches common path patterns, otherwise returns the original string.
	/// </summary>
	/// <param name="value">The string to potentially intern.</param>
	/// <returns>An interned string if it matches common patterns, otherwise the original string.</returns>
	public static string InternIfCommon(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Empty;
		}

		// Intern common separators
		if (value == Path.DirectorySeparatorChar.ToString())
		{
			return DirectorySeparator;
		}
		if (value == Path.AltDirectorySeparatorChar.ToString())
		{
			return AltDirectorySeparator;
		}

		// Intern common roots
		return value.Length <= 4
			? value switch
			{
				@"C:\" => WindowsRoot,
				"/" => UnixRoot,
				@"\\" => WindowsUncRoot,
				_ => value
			}
			: value;
	}
}

/// <summary>
/// Provides span-based path parsing utilities to reduce allocations.
/// </summary>
internal static class SpanPathUtilities
{
	/// <summary>
	/// Gets the directory name from a path span without allocating intermediate strings.
	/// </summary>
	/// <param name="path">The path span to parse.</param>
	/// <returns>The directory name span, or empty span if no directory.</returns>
	public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path)
	{
		if (path.IsEmpty)
		{
			return default;
		}

		// Find the last directory separator
		int lastSeparatorIndex = -1;
		for (int i = path.Length - 1; i >= 0; i--)
		{
			if (path[i] == Path.DirectorySeparatorChar || path[i] == Path.AltDirectorySeparatorChar)
			{
				lastSeparatorIndex = i;
				break;
			}
		}

		return lastSeparatorIndex >= 0 ? path[..lastSeparatorIndex] : default;
	}

	/// <summary>
	/// Gets the filename from a path span without allocating intermediate strings.
	/// </summary>
	/// <param name="path">The path span to parse.</param>
	/// <returns>The filename span, or empty span if no filename.</returns>
	public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
	{
		if (path.IsEmpty)
		{
			return default;
		}

		// Find the last directory separator
		int lastSeparatorIndex = -1;
		for (int i = path.Length - 1; i >= 0; i--)
		{
			if (path[i] == Path.DirectorySeparatorChar || path[i] == Path.AltDirectorySeparatorChar)
			{
				lastSeparatorIndex = i;
				break;
			}
		}

		return lastSeparatorIndex >= 0 && lastSeparatorIndex < path.Length - 1
			? path[(lastSeparatorIndex + 1)..]
			: path;
	}

	/// <summary>
	/// Determines whether a path span ends with a directory separator.
	/// </summary>
	/// <param name="path">The path span to check.</param>
	/// <returns><see langword="true"/> if the path ends with a directory separator; otherwise, <see langword="false"/>.</returns>
	public static bool EndsWithDirectorySeparator(ReadOnlySpan<char> path)
	{
		return !path.IsEmpty &&
			   (path[^1] == Path.DirectorySeparatorChar || path[^1] == Path.AltDirectorySeparatorChar);
	}
}

/// <summary>
/// Provides pooled StringBuilder for efficient path operations.
/// </summary>
internal static class PooledStringBuilder
{
	[ThreadStatic]
	private static StringBuilder? t_cachedInstance;

	/// <summary>
	/// Gets a pooled StringBuilder instance.
	/// </summary>
	/// <returns>A StringBuilder instance that should be returned to the pool after use.</returns>
	public static StringBuilder Get()
	{
		StringBuilder? sb = t_cachedInstance;
		if (sb != null)
		{
			// Clear but don't reset capacity
			sb.Clear();
			t_cachedInstance = null;
			return sb;
		}
		return new StringBuilder();
	}

	/// <summary>
	/// Returns a StringBuilder to the pool.
	/// </summary>
	/// <param name="sb">The StringBuilder to return to the pool.</param>
	public static void Return(StringBuilder sb)
	{
		if (sb.Capacity <= 360) // Reasonable size limit
		{
			t_cachedInstance = sb;
		}
	}

	/// <summary>
	/// Combines multiple path components efficiently using pooled StringBuilder.
	/// </summary>
	/// <param name="components">The path components to combine.</param>
	/// <returns>The combined path string.</returns>
	public static string CombinePaths(params ReadOnlySpan<string> components)
	{
		if (components.Length == 0)
		{
			return InternedPathStrings.Empty;
		}
		if (components.Length == 1)
		{
			return components[0];
		}

		StringBuilder sb = Get();
		try
		{
			sb.Append(components[0]);
			for (int i = 1; i < components.Length; i++)
			{
				if (!SpanPathUtilities.EndsWithDirectorySeparator(sb.ToString().AsSpan()))
				{
					sb.Append(Path.DirectorySeparatorChar);
				}
				sb.Append(components[i]);
			}
			return sb.ToString();
		}
		finally
		{
			Return(sb);
		}
	}
}

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
			return _cachedDirectoryPath ??= AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>(
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
	/// <returns>A new <see cref="AbsoluteFilePath"/> with the changed extension.</returns>
	public AbsoluteFilePath ChangeExtension(FileExtension newExtension)
	{
		ArgumentNullException.ThrowIfNull(newExtension);
		string newPath = Path.ChangeExtension(WeakString, newExtension.WeakString);
		return FromString<AbsoluteFilePath>(newPath);
	}

	/// <summary>
	/// Removes the file extension from this path.
	/// </summary>
	/// <returns>A new <see cref="AbsoluteFilePath"/> without an extension.</returns>
	public AbsoluteFilePath RemoveExtension()
	{
		string pathWithoutExtension = Path.ChangeExtension(WeakString, null) ?? "";
		return FromString<AbsoluteFilePath>(pathWithoutExtension);
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
	/// Converts this interface to its concrete <see cref="AbsoluteFilePath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> instance with the same path value.</returns>
	public AbsoluteFilePath AsAbsoluteFilePath() => this;
}

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

/// <summary>
/// Represents a directory path (path to a directory)
/// </summary>
[IsPath, IsDirectoryPath]
public sealed record DirectoryPath : SemanticDirectoryPath<DirectoryPath>, IDirectoryPath
{
	/// <summary>
	/// Gets the parent directory of this directory path.
	/// </summary>
	/// <value>A <see cref="DirectoryPath"/> representing the parent directory, or an empty path if this is a root directory.</value>
	public DirectoryPath Parent
	{
		get
		{
			string? parentPath = Path.GetDirectoryName(WeakString);
			return FromString<DirectoryPath>(parentPath ?? "");
		}
	}

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="FileName"/> representing just the directory name.</value>
	public FileName Name => FileName.FromString<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Asynchronously enumerates the files and directories contained in this directory as semantic path types.
	/// This is more efficient for large directories as it streams results instead of loading everything into memory.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
	/// <returns>
	/// An async enumerable of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty enumerable if the directory doesn't exist or cannot be accessed.
	/// </returns>
	public async IAsyncEnumerable<IPath> GetContentsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		string directoryPath = WeakString;
		if (!Directory.Exists(directoryPath))
		{
			yield break;
		}

		// Use Task.Run to avoid blocking the caller while enumerating
		IEnumerable<string> entries = await Task.Run(() => Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.TopDirectoryOnly), cancellationToken).ConfigureAwait(false);

		foreach (string item in entries)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (Directory.Exists(item))
			{
				yield return FromString<DirectoryPath>(item);
			}
			else if (File.Exists(item))
			{
				yield return FilePath.FromString<FilePath>(item);
			}
		}
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
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return FromString<DirectoryPath>(combinedPath);
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
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = Path.Combine(left.WeakString, right.WeakString);
		return FilePath.FromString<FilePath>(combinedPath);
	}
}

/// <summary>
/// Represents an absolute directory path
/// </summary>
[IsPath, IsAbsolutePath, IsDirectoryPath]
public sealed record AbsoluteDirectoryPath : SemanticDirectoryPath<AbsoluteDirectoryPath>, IAbsoluteDirectoryPath
{
	// Cache for expensive parent directory computation
	private AbsoluteDirectoryPath? _cachedParent;

	/// <summary>
	/// Gets the parent directory of this absolute directory path.
	/// </summary>
	/// <value>An <see cref="AbsoluteDirectoryPath"/> representing the parent directory, or an empty path if this is a root directory.</value>
	public AbsoluteDirectoryPath Parent
	{
		get
		{
			return _cachedParent ??= FromString<AbsoluteDirectoryPath>(
				InternedPathStrings.InternIfCommon(Path.GetDirectoryName(WeakString) ?? InternedPathStrings.Empty));
		}
	}

	// Cache for directory name
	private FileName? _cachedName;

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="FileName"/> representing just the directory name.</value>
	public FileName Name => _cachedName ??= FileName.FromString<FileName>(Path.GetFileName(WeakString) ?? "");

	// Cache for depth calculation
	private int? _cachedDepth;

	/// <summary>
	/// Gets the depth of this directory path (number of directory separators).
	/// For example, "a/b/c" has depth 2, "a" has depth 0.
	/// </summary>
	/// <value>The number of directory separators in the path.</value>
	public int Depth => _cachedDepth ??= CalculateDepth(WeakString);

	/// <summary>
	/// Calculates directory depth using span semantics for optimal performance.
	/// </summary>
	/// <param name="path">The path to analyze.</param>
	/// <returns>The directory depth.</returns>
	private static int CalculateDepth(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return 0;
		}

		ReadOnlySpan<char> span = path.AsSpan();
		int depth = 0;
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] == Path.DirectorySeparatorChar || span[i] == Path.AltDirectorySeparatorChar)
			{
				depth++;
			}
		}
		return depth;
	}

	// Cache for root check
	private bool? _cachedIsRoot;

	/// <summary>
	/// Determines whether this directory is a root directory.
	/// </summary>
	/// <value><c>true</c> if this is a root directory; otherwise, <c>false</c>.</value>
	public bool IsRoot => _cachedIsRoot ??= Path.GetPathRoot(WeakString) == WeakString;

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

	/// <summary>
	/// Combines an absolute directory path with a relative directory path using the '/' operator.
	/// </summary>
	/// <param name="left">The base absolute directory path.</param>
	/// <param name="right">The relative directory path to append.</param>
	/// <returns>A new <see cref="AbsoluteDirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static AbsoluteDirectoryPath operator /(AbsoluteDirectoryPath left, RelativeDirectoryPath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths([left.WeakString, right.WeakString]);
		return FromString<AbsoluteDirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines an absolute directory path with a relative file path using the '/' operator.
	/// </summary>
	/// <param name="left">The base absolute directory path.</param>
	/// <param name="right">The relative file path to append.</param>
	/// <returns>A new <see cref="AbsoluteFilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static AbsoluteFilePath operator /(AbsoluteDirectoryPath left, RelativeFilePath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths([left.WeakString, right.WeakString]);
		return AbsoluteFilePath.FromString<AbsoluteFilePath>(combinedPath);
	}

	/// <summary>
	/// Determines whether this directory is a child of the specified parent path using efficient span comparison.
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
	/// Determines whether this directory is a parent of the specified child path using efficient span comparison.
	/// </summary>
	/// <param name="childPath">The potential child path to check against.</param>
	/// <returns><see langword="true"/> if this path is a parent of the child path; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method uses span-based comparison for better performance than string concatenation.
	/// It normalizes both paths before comparison to handle different separator styles.
	/// </remarks>
	public bool IsParentOf(AbsoluteDirectoryPath childPath)
	{
		ArgumentNullException.ThrowIfNull(childPath);
		return childPath.IsChildOf(this);
	}

	/// <summary>
	/// Gets all parent directories from this directory up to the root.
	/// </summary>
	/// <returns>An enumerable of <see cref="AbsoluteDirectoryPath"/> representing all parent directories.</returns>
	public IEnumerable<AbsoluteDirectoryPath> GetAncestors()
	{
		AbsoluteDirectoryPath current = Parent;
		while (!string.IsNullOrEmpty(current.WeakString) && current != this)
		{
			yield return current;
			AbsoluteDirectoryPath next = current.Parent;
			if (next == current)
			{
				break; // Prevent infinite loop at root
			}
			current = next;
		}
	}

	/// <summary>
	/// Creates a relative directory path from this directory to another absolute directory.
	/// </summary>
	/// <param name="targetDirectory">The target directory.</param>
	/// <returns>A <see cref="RelativeDirectoryPath"/> from this directory to the target.</returns>
	public RelativeDirectoryPath GetRelativePathTo(AbsoluteDirectoryPath targetDirectory)
	{
		ArgumentNullException.ThrowIfNull(targetDirectory);
		// Use Path.GetRelativePath to compute the relative path
		string relativePath = Path.GetRelativePath(WeakString, targetDirectory.WeakString);
		return RelativeDirectoryPath.FromString<RelativeDirectoryPath>(relativePath);
	}

	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> instance with the same path value.</returns>
	public AbsoluteDirectoryPath AsAbsoluteDirectoryPath() => this;

	/// <summary>
	/// Asynchronously enumerates the files and directories contained in this directory as semantic path types.
	/// This is more efficient for large directories as it streams results instead of loading everything into memory.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
	/// <returns>
	/// An async enumerable of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty enumerable if the directory doesn't exist or cannot be accessed.
	/// </returns>
	public async IAsyncEnumerable<IPath> GetContentsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		string directoryPath = WeakString;
		if (!Directory.Exists(directoryPath))
		{
			yield break;
		}

		// Use Task.Run to avoid blocking the caller while enumerating
		IEnumerable<string> entries = await Task.Run(() => Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.TopDirectoryOnly), cancellationToken).ConfigureAwait(false);

		foreach (string item in entries)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (Directory.Exists(item))
			{
				yield return CreateDirectoryPath(item);
			}
			else if (File.Exists(item))
			{
				yield return CreateFilePath(item);
			}
		}
	}
}

/// <summary>
/// Represents a relative directory path
/// </summary>
[IsPath, IsRelativePath, IsDirectoryPath]
public sealed record RelativeDirectoryPath : SemanticDirectoryPath<RelativeDirectoryPath>, IRelativeDirectoryPath
{
	// Cache for expensive parent directory computation
	private RelativeDirectoryPath? _cachedParent;

	/// <summary>
	/// Gets the parent directory of this relative directory path.
	/// </summary>
	/// <value>A <see cref="RelativeDirectoryPath"/> representing the parent directory, or an empty path if this is a root-relative directory.</value>
	public RelativeDirectoryPath Parent => _cachedParent ??= FromString<RelativeDirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");

	// Cache for directory name
	private FileName? _cachedName;

	/// <summary>
	/// Gets the name of this directory (the last component of the path).
	/// </summary>
	/// <value>A <see cref="FileName"/> representing just the directory name.</value>
	public FileName Name => _cachedName ??= FileName.FromString<FileName>(Path.GetFileName(WeakString) ?? "");

	// Cache for depth calculation
	private int? _cachedDepth;

	/// <summary>
	/// Gets the depth of this relative directory path (number of directory levels).
	/// </summary>
	/// <value>The depth of the directory path.</value>
	public int Depth => _cachedDepth ??= CalculateDepth(WeakString);

	/// <summary>
	/// Calculates directory depth using span semantics for optimal performance.
	/// </summary>
	/// <param name="path">The path to analyze.</param>
	/// <returns>The directory depth.</returns>
	private static int CalculateDepth(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return 0;
		}

		ReadOnlySpan<char> span = path.AsSpan();
		int depth = 0;
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i] == Path.DirectorySeparatorChar || span[i] == Path.AltDirectorySeparatorChar)
			{
				depth++;
			}
		}
		return depth;
	}

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

	/// <summary>
	/// Combines a relative directory path with another relative directory path using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The relative directory path to append.</param>
	/// <returns>A new <see cref="RelativeDirectoryPath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeDirectoryPath operator /(RelativeDirectoryPath left, RelativeDirectoryPath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths([left.WeakString, right.WeakString]);
		return FromString<RelativeDirectoryPath>(combinedPath);
	}

	/// <summary>
	/// Combines a relative directory path with a relative file path using the '/' operator.
	/// </summary>
	/// <param name="left">The base relative directory path.</param>
	/// <param name="right">The relative file path to append.</param>
	/// <returns>A new <see cref="RelativeFilePath"/> representing the combined path.</returns>
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Path combination is the semantic meaning, not mathematical division")]
	public static RelativeFilePath operator /(RelativeDirectoryPath left, RelativeFilePath right)
	{
		ArgumentNullException.ThrowIfNull(left);
		ArgumentNullException.ThrowIfNull(right);

		string combinedPath = PooledStringBuilder.CombinePaths([left.WeakString, right.WeakString]);
		return RelativeFilePath.FromString<RelativeFilePath>(combinedPath);
	}

	/// <summary>
	/// Converts this relative directory path to an absolute directory path using the specified base directory.
	/// </summary>
	/// <param name="baseDirectory">The base directory to resolve this relative path against.</param>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute path.</returns>
	public AbsoluteDirectoryPath ToAbsolute(AbsoluteDirectoryPath baseDirectory)
	{
		ArgumentNullException.ThrowIfNull(baseDirectory);
		string absolutePath = Path.GetFullPath(Path.Combine(baseDirectory.WeakString, WeakString));
		return AbsoluteDirectoryPath.FromString<AbsoluteDirectoryPath>(absolutePath);
	}

	/// <summary>
	/// Normalizes this relative path by resolving any "." and ".." components.
	/// </summary>
	/// <returns>A new <see cref="RelativeDirectoryPath"/> with normalized path components.</returns>
	public new RelativeDirectoryPath Normalize()
	{
		string path = WeakString;
		if (string.IsNullOrEmpty(path))
		{
			return this;
		}

		// Use Path.GetFullPath with a dummy base to normalize relative paths
		string dummyBase = OperatingSystem.IsWindows() ? "C:\\" : "/";
		string fullPath = Path.GetFullPath(Path.Combine(dummyBase, path));
		string normalized = Path.GetRelativePath(dummyBase, fullPath);

		return FromString<RelativeDirectoryPath>(normalized);
	}

	/// <summary>
	/// Converts this interface to its concrete <see cref="RelativeDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeDirectoryPath"/> instance with the same path value.</returns>
	public RelativeDirectoryPath AsRelativeDirectoryPath() => this;

	/// <summary>
	/// Asynchronously enumerates the files and directories contained in this directory as semantic path types.
	/// This is more efficient for large directories as it streams results instead of loading everything into memory.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token to cancel the enumeration.</param>
	/// <returns>
	/// An async enumerable of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty enumerable if the directory doesn't exist or cannot be accessed.
	/// </returns>
	public async IAsyncEnumerable<IPath> GetContentsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		string directoryPath = WeakString;
		if (!Directory.Exists(directoryPath))
		{
			yield break;
		}

		// Use Task.Run to avoid blocking the caller while enumerating
		IEnumerable<string> entries = await Task.Run(() => Directory.EnumerateFileSystemEntries(directoryPath, "*", SearchOption.TopDirectoryOnly), cancellationToken).ConfigureAwait(false);

		foreach (string item in entries)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (Directory.Exists(item))
			{
				yield return CreateDirectoryPath(item);
			}
			else if (File.Exists(item))
			{
				yield return CreateFilePath(item);
			}
		}
	}
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
