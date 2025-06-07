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
	public bool IsDirectory => Directory.Exists(ToString());

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
	public bool IsFile => File.Exists(ToString());

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
		canonical = canonical.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

		// Remove trailing directory separator (except for root paths)
		string separator = new([System.IO.Path.DirectorySeparatorChar]);
		if (canonical.EndsWith(separator) && canonical.Length > separator.Length)
		{
			// Check if this is a Windows root path (e.g., "C:\")
			bool isWindowsRoot = OperatingSystem.IsWindows()
				&& canonical.Length == 3
				&& char.IsLetter(canonical[0])
				&& canonical[1] == ':'
				&& canonical[2] == System.IO.Path.DirectorySeparatorChar;

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

		FileInfo fromInfo = new(System.IO.Path.GetFullPath(from.ToString()));
		FileInfo toInfo = new(System.IO.Path.GetFullPath(to.ToString()));

		// Use unix-style separators because they work on windows too
		const string separator = "/";
		const string altSeparator = "\\";

		string fromPath = System.IO.Path.GetFullPath(fromInfo.FullName)
			.Replace(altSeparator, separator, StringComparison.Ordinal);
		string toPath = System.IO.Path.GetFullPath(toInfo.FullName)
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
			string value = ToString();
			string[] parts = value.Split('.');
			string ext = parts[^1];
			return ext == value
				? FileExtension.FromString<FileExtension>("")
				: FileExtension.FromString<FileExtension>("." + ext);
		}
	}

	/// <summary>
	/// Gets all trailing period-delimited segments including the leading period, or empty if no extensions
	/// </summary>
	public FileExtension FullFileExtension
	{
		get
		{
			string[] parts = ToString().Split('.', 2);
			return parts.Length > 1
				? FileExtension.FromString<FileExtension>("." + parts[1])
				: FileExtension.FromString<FileExtension>("");
		}
	}

	/// <summary>
	/// Gets the filename portion of the path
	/// </summary>
	public FileName FileName => FileName.FromString<FileName>(System.IO.Path.GetFileName(ToString()));

	/// <summary>
	/// Gets the directory portion of the path
	/// </summary>
	public DirectoryPath DirectoryPath => DirectoryPath.FromString<DirectoryPath>(System.IO.Path.GetDirectoryName(ToString()) ?? "");
}

/// <summary>
/// Base class for directory paths (paths that represent directories)
/// </summary>
[IsPath, IsDirectoryPath]
public abstract record SemanticDirectoryPath<TDerived> : SemanticPath<TDerived>
	where TDerived : SemanticDirectoryPath<TDerived>
{
}
