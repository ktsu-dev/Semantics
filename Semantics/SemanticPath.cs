// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Base semantic path class that provides path-specific functionality and validation.
/// </summary>
/// <typeparam name="TDerived">The derived path type using CRTP (Curiously Recurring Template Pattern).</typeparam>
/// <remarks>
/// This abstract base class provides common functionality for all path-based semantic strings:
/// <list type="bullet">
/// <item><description>File system existence checking</description></item>
/// <item><description>Path canonicalization (directory separator normalization)</description></item>
/// <item><description>Type-safe path operations</description></item>
/// <item><description>Implicit string conversion for transparent usage (inherited from SemanticString)</description></item>
/// </list>
/// All path types are automatically validated using the <see cref="IsPathAttribute"/> to ensure
/// they contain valid path characters and reasonable lengths.
/// </remarks>
[IsPath]
public abstract record SemanticPath<TDerived> : SemanticString<TDerived>
	where TDerived : SemanticPath<TDerived>
{

	/// <summary>
	/// Gets a value indicating whether this path exists on the filesystem as either a file or directory.
	/// </summary>
	/// <value>
	/// <see langword="true"/> if the path exists as either a file or directory; otherwise, <see langword="false"/>.
	/// </value>
	/// <remarks>
	/// This property combines the results of <see cref="IsDirectory"/> and <see cref="IsFile"/>
	/// to provide a convenient way to check for any type of filesystem existence.
	/// </remarks>
	public bool Exists => IsDirectory || IsFile;

	/// <summary>
	/// Gets a value indicating whether this path exists as a directory on the filesystem.
	/// </summary>
	/// <value>
	/// <see langword="true"/> if the path exists and is a directory; otherwise, <see langword="false"/>.
	/// </value>
	/// <remarks>
	/// This property uses <see cref="Directory.Exists(string)"/> to check for directory existence.
	/// Returns <see langword="false"/> if the path exists but is a file, or if the path doesn't exist at all.
	/// </remarks>
	public bool IsDirectory => Directory.Exists(WeakString);

	/// <summary>
	/// Gets a value indicating whether this path exists as a file on the filesystem.
	/// </summary>
	/// <value>
	/// <see langword="true"/> if the path exists and is a file; otherwise, <see langword="false"/>.
	/// </value>
	/// <remarks>
	/// This property uses <see cref="File.Exists(string)"/> to check for file existence.
	/// Returns <see langword="false"/> if the path exists but is a directory, or if the path doesn't exist at all.
	/// </remarks>
	public bool IsFile => File.Exists(WeakString);

	/// <summary>
	/// Normalizes the path by standardizing directory separators and removing trailing separators.
	/// </summary>
	/// <param name="input">The input path string to canonicalize.</param>
	/// <returns>The canonicalized path string.</returns>
	/// <remarks>
	/// This method performs the following canonicalization steps:
	/// <list type="number">
	/// <item><description>Calls the base canonicalization method</description></item>
	/// <item><description>Replaces alternative directory separators with the platform's preferred separator</description></item>
	/// <item><description>Removes trailing directory separators (except for root paths)</description></item>
	/// </list>
	/// For example, on Windows:
	/// <list type="bullet">
	/// <item><description><c>"path/to/file"</c> becomes <c>"path\to\file"</c></description></item>
	/// <item><description><c>"path\to\dir\"</c> becomes <c>"path\to\dir"</c></description></item>
	/// <item><description><c>"C:\"</c> remains <c>"C:\"</c> (root path exception)</description></item>
	/// </list>
	/// </remarks>
	protected override string MakeCanonical(string input)
	{
		string canonical = base.MakeCanonical(input);

		// Normalize directory separators to the current platform's preferred separator
		canonical = canonical.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

		// Remove trailing directory separator (except for root paths)
		string separator = new([Path.DirectorySeparatorChar]);
		if (canonical.EndsWith(separator) && canonical.Length > separator.Length)
		{
			// Check if this is a Windows root path (e.g., "C:\")
			bool isWindowsRoot = OperatingSystem.IsWindows()
				&& canonical.Length == 3
				&& char.IsLetter(canonical[0])
				&& canonical[1] == ':'
				&& canonical[2] == Path.DirectorySeparatorChar;

			// Check if this is a Unix root path (e.g., "/")
			bool isUnixRoot = !OperatingSystem.IsWindows() && canonical == separator;

			// Only remove trailing separator if it's not a root path
			if (!isWindowsRoot && !isUnixRoot)
			{
				canonical = canonical[..^separator.Length];
			}
		}

		return canonical;
	}
}

/// <summary>
/// Base class for absolute paths (fully qualified paths)
/// </summary>
[IsPath, IsAbsolutePath]
public abstract record SemanticAbsolutePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticAbsolutePath<TDerived>
{
}

/// <summary>
/// Base class for relative paths (not fully qualified)
/// </summary>
[IsPath, IsRelativePath]
public abstract record SemanticRelativePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticRelativePath<TDerived>
{
	/// <summary>
	/// Creates a relative path from an absolute path to another absolute path
	/// </summary>
	public static TRelativePath Make<TRelativePath, TFromPath, TToPath>(TFromPath from, TToPath to)
		where TRelativePath : SemanticRelativePath<TRelativePath>
		where TFromPath : SemanticPath<TFromPath>
		where TToPath : SemanticPath<TToPath>
	{
		ArgumentNullException.ThrowIfNull(from);
		ArgumentNullException.ThrowIfNull(to);

		FileInfo fromInfo = new(Path.GetFullPath(from.WeakString));
		FileInfo toInfo = new(Path.GetFullPath(to.WeakString));

		// Use unix-style separators because they work on windows too
		const string separator = "/";
		const string altSeparator = "\\";

		string fromPath = Path.GetFullPath(fromInfo.FullName)
			.Replace(altSeparator, separator, StringComparison.Ordinal);
		string toPath = Path.GetFullPath(toInfo.FullName)
			.Replace(altSeparator, separator, StringComparison.Ordinal);

		// Handle directory paths - ensure they end with separator
		bool fromIsDirectory = IsDirectoryPath(from);
		bool toIsDirectory = IsDirectoryPath(to);

		if (fromIsDirectory && !fromPath.EndsWith(separator, StringComparison.Ordinal))
		{
			fromPath += separator;
		}

		if (toIsDirectory && !toPath.EndsWith(separator, StringComparison.Ordinal))
		{
			toPath += separator;
		}

		Uri fromUri = new(fromPath);
		Uri toUri = new(toPath);

		Uri relativeUri = fromUri.MakeRelativeUri(toUri);
		string relativePath = Uri.UnescapeDataString(relativeUri.ToString());
		relativePath = relativePath.Replace(altSeparator, separator, StringComparison.Ordinal);

		return FromString<TRelativePath>(relativePath);
	}

	/// <summary>
	/// Determines whether the specified path type represents a directory path based on its validation attributes.
	/// </summary>
	/// <typeparam name="T">The type of semantic path to check.</typeparam>
	/// <param name="path">The path instance to check.</param>
	/// <returns><see langword="true"/> if the path type has the <see cref="IsDirectoryPathAttribute"/>; otherwise, <see langword="false"/>.</returns>
	private static bool IsDirectoryPath<T>(T path) where T : SemanticPath<T>
	{
		// Check if it's a directory-specific type based on validation attributes
		Type type = path.GetType();
		return type.GetCustomAttributes(typeof(IsDirectoryPathAttribute), true).Length > 0;
	}
}

/// <summary>
/// Base class for file paths (paths that represent files)
/// </summary>
[IsPath, IsFilePath]
public abstract record SemanticFilePath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticFilePath<TDerived>
{
	/// <summary>
	/// Gets the file extension including the leading period, or empty if no extension
	/// </summary>
	public FileExtension FileExtension
	{
		get
		{
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the last dot
			int lastDotIndex = span.LastIndexOf('.');
			if (lastDotIndex == -1 || lastDotIndex == span.Length - 1)
			{
				// No extension or trailing dot
				return FileExtension.FromString<FileExtension>("");
			}

			// Return extension including the dot
			ReadOnlySpan<char> extension = span[lastDotIndex..];
			return FileExtension.FromString<FileExtension>(extension.ToString());
		}
	}

	/// <summary>
	/// Gets all trailing period-delimited segments including the leading period, or empty if no extensions
	/// </summary>
	public FileExtension FullFileExtension
	{
		get
		{
			ReadOnlySpan<char> span = WeakString.AsSpan();

			// Find the first dot
			int firstDotIndex = span.IndexOf('.');
			if (firstDotIndex == -1)
			{
				// No extension
				return FileExtension.FromString<FileExtension>("");
			}

			// Return everything from the first dot onward
			ReadOnlySpan<char> fullExtension = span[firstDotIndex..];
			return FileExtension.FromString<FileExtension>(fullExtension.ToString());
		}
	}

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.FromString<FileName>(Path.GetFileName(WeakString));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	public DirectoryPath DirectoryPath => DirectoryPath.FromString<DirectoryPath>(Path.GetDirectoryName(WeakString) ?? "");
}

/// <summary>
/// Base class for directory paths (paths that represent directories)
/// </summary>
[IsPath, IsDirectoryPath]
public abstract record SemanticDirectoryPath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticDirectoryPath<TDerived>
{
	/// <summary>
	/// Gets the files and directories contained in this directory as semantic path types.
	/// Files are returned as the appropriate file path type, and directories as the appropriate directory path type.
	/// </summary>
	/// <value>
	/// A collection of <see cref="IPath"/> objects representing the contents of the directory.
	/// Returns an empty collection if the directory doesn't exist or cannot be accessed.
	/// </value>
	/// <remarks>
	/// The returned types depend on the current directory type:
	/// <list type="bullet">
	/// <item><description><see cref="AbsoluteDirectoryPath"/> returns <see cref="AbsoluteFilePath"/> and <see cref="AbsoluteDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="RelativeDirectoryPath"/> returns <see cref="RelativeFilePath"/> and <see cref="RelativeDirectoryPath"/> objects</description></item>
	/// <item><description><see cref="DirectoryPath"/> returns <see cref="FilePath"/> and <see cref="DirectoryPath"/> objects</description></item>
	/// </list>
	/// </remarks>
	public virtual IEnumerable<IPath> Contents
	{
		get
		{
			string directoryPath = WeakString;
			if (!Directory.Exists(directoryPath))
			{
				return [];
			}

			try
			{
				List<IPath> contents = [];

				// Get all files and directories
				string[] entries = Directory.GetFileSystemEntries(directoryPath);

				foreach (string entry in entries)
				{
					if (File.Exists(entry))
					{
						// It's a file - create appropriate file path type
						contents.Add(CreateFilePath(entry));
					}
					else if (Directory.Exists(entry))
					{
						// It's a directory - create appropriate directory path type
						contents.Add(CreateDirectoryPath(entry));
					}
				}

				return contents;
			}
			catch (UnauthorizedAccessException)
			{
				// Return empty collection if access denied
				return [];
			}
			catch (DirectoryNotFoundException)
			{
				// Return empty collection if directory not found
				return [];
			}
		}
	}

	/// <summary>
	/// Creates an appropriate file path type based on the current directory path type.
	/// </summary>
	/// <param name="filePath">The file path to wrap.</param>
	/// <returns>An <see cref="IFilePath"/> of the appropriate type.</returns>
	protected virtual IFilePath CreateFilePath(string filePath) =>
		FilePath.FromString<FilePath>(filePath);

	/// <summary>
	/// Creates an appropriate directory path type based on the current directory path type.
	/// </summary>
	/// <param name="directoryPath">The directory path to wrap.</param>
	/// <returns>An <see cref="IDirectoryPath"/> of the appropriate type.</returns>
	protected virtual IDirectoryPath CreateDirectoryPath(string directoryPath) =>
		DirectoryPath.FromString<DirectoryPath>(directoryPath);
}
