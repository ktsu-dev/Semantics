// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System.IO;
using System.Runtime.InteropServices;
using ktsu.Semantics.Strings;

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
#if NET5_0_OR_GREATER
			bool isWindowsRoot = OperatingSystem.IsWindows()
#else
			bool isWindowsRoot = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
				&& canonical.Length == 3
				&& char.IsLetter(canonical[0])
				&& canonical[1] == ':'
				&& canonical[2] == Path.DirectorySeparatorChar;

			// Check if this is a Unix root path (e.g., "/")
#if NET5_0_OR_GREATER
			bool isUnixRoot = !OperatingSystem.IsWindows() && canonical == separator;
#else
			bool isUnixRoot = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && canonical == separator;
#endif

			// Only remove trailing separator if it's not a root path
			if (!isWindowsRoot && !isUnixRoot)
			{
#if NETSTANDARD2_0
				canonical = canonical.Substring(0, canonical.Length - separator.Length);
#else
				canonical = canonical[..^separator.Length];
#endif
			}
		}

		return canonical;
	}
}
