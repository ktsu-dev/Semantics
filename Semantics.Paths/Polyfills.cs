// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Provides polyfill methods for Path class for older frameworks.
/// </summary>
internal static class PathPolyfill
{
	/// <summary>
	/// Returns the relative path from one path to another.
	/// </summary>
	/// <param name="relativeTo">The source path the result should be relative to.</param>
	/// <param name="path">The destination path.</param>
	/// <returns>The relative path, or path if the paths don't share the same root.</returns>
	public static string GetRelativePath(string relativeTo, string path) =>
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		Path.GetRelativePath(relativeTo, path);
#else
		GetRelativePathBySegments(relativeTo, path);
#endif

	/// <summary>
	/// Computes a relative path by comparing path segments, for targets without <see cref="Path"/>'s own
	/// GetRelativePath. Compiled on every target so it can be tested against that method.
	/// </summary>
	/// <remarks>
	/// A file-system path is not a URI: building one from a path decodes a literal <c>%2E</c> in a file
	/// name into <c>.</c>, so the result can name a different file, or climb out of the base directory.
	/// Comparing segments keeps every name exactly as written.
	/// </remarks>
	/// <param name="relativeTo">The directory the result should be relative to.</param>
	/// <param name="path">The destination path.</param>
	/// <returns>The relative path, <c>.</c> when both name the same directory, or <paramref name="path"/> when the roots differ.</returns>
	internal static string GetRelativePathBySegments(string relativeTo, string path)
	{
		relativeTo = Path.GetFullPath(relativeTo);
		path = Path.GetFullPath(path);

		// Matches Path.GetRelativePath: case-insensitive where the platform's file system usually is.
		StringComparison comparison =
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;

		string fromRoot = Path.GetPathRoot(relativeTo) ?? string.Empty;
		string toRoot = Path.GetPathRoot(path) ?? string.Empty;
		if (!string.Equals(TrimSeparators(fromRoot), TrimSeparators(toRoot), comparison))
		{
			return path;
		}

		char[] separators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
		string[] fromSegments = relativeTo.Substring(fromRoot.Length).Split(separators, StringSplitOptions.RemoveEmptyEntries);
		string[] toSegments = path.Substring(toRoot.Length).Split(separators, StringSplitOptions.RemoveEmptyEntries);

		int common = 0;
		while (common < fromSegments.Length && common < toSegments.Length
			&& string.Equals(fromSegments[common], toSegments[common], comparison))
		{
			common++;
		}

		List<string> result = [];
		for (int i = common; i < fromSegments.Length; i++)
		{
			result.Add("..");
		}

		for (int i = common; i < toSegments.Length; i++)
		{
			result.Add(toSegments[i]);
		}

		return result.Count == 0 ? "." : string.Join(Path.DirectorySeparatorChar.ToString(), result);
	}

	private static string TrimSeparators(string root) =>
		root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

	/// <summary>
	/// Returns a value that indicates whether a path is fully qualified.
	/// </summary>
	/// <param name="path">The path to check.</param>
	/// <returns>true if the path is fully qualified; otherwise, false.</returns>
	public static bool IsPathFullyQualified(string path)
	{
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		return Path.IsPathFullyQualified(path);
#else
		// Fallback implementation for netstandard2.0
		if (string.IsNullOrWhiteSpace(path))
		{
			return false;
		}

		if (path.Length < 2)
		{
			return false;
		}

		// Check for UNC paths (\\server\share)
		if (path.Length >= 2 && IsDirectorySeparator(path[0]) && IsDirectorySeparator(path[1]))
		{
			return true;
		}

		// Check for drive letter paths (C:\)
		if (path.Length >= 3 &&
			char.IsLetter(path[0]) &&
			path[1] == ':' &&
			IsDirectorySeparator(path[2]))
		{
			return true;
		}

		// Unix absolute paths start with /
		if (Path.DirectorySeparatorChar == '/' && path[0] == '/')
		{
			return true;
		}

		return false;
#endif
	}

	/// <summary>
	/// Gets the absolute path for the specified path string relative to the base path.
	/// </summary>
	/// <param name="path">The file or directory for which to obtain absolute path information.</param>
	/// <param name="basePath">The beginning of a fully qualified path.</param>
	/// <returns>The fully qualified location of path, such as "C:\MyFile.txt".</returns>
	public static string GetFullPath(string path, string basePath)
	{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		return Path.GetFullPath(path, basePath);
#else
		// Fallback implementation for netstandard2.0 and netcoreapp2.0
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Path cannot be empty.", nameof(path));
		}

		if (string.IsNullOrEmpty(basePath))
		{
			throw new ArgumentException("Base path cannot be empty.", nameof(basePath));
		}

		basePath = Path.GetFullPath(basePath);

		if (IsPathFullyQualified(path))
		{
			return Path.GetFullPath(path);
		}

		string combinedPath = Path.Combine(basePath, path);
		return Path.GetFullPath(combinedPath);
#endif
	}

#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
	private static bool IsDirectorySeparator(char c) =>
		c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
#endif
}

/// <summary>
/// Provides polyfill methods for String class for older frameworks.
/// </summary>
internal static class StringPolyfill
{
	/// <summary>
	/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string, using the provided comparison type.
	/// </summary>
	/// <param name="str">The string to perform the replacement on.</param>
	/// <param name="oldValue">The string to be replaced.</param>
	/// <param name="newValue">The string to replace all occurrences of oldValue.</param>
	/// <param name="comparisonType">One of the enumeration values that determines how oldValue is searched within this instance.</param>
	/// <returns>A string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.</returns>
	public static string Replace(string str, string oldValue, string newValue, StringComparison comparisonType)
	{
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		return str.Replace(oldValue, newValue, comparisonType);
#else
		// Fallback implementation for netstandard2.0
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}

		if (string.IsNullOrEmpty(oldValue))
		{
			throw new ArgumentException("Old value cannot be null or empty.", nameof(oldValue));
		}

		newValue ??= string.Empty;

		int index = str.IndexOf(oldValue, comparisonType);
		if (index < 0)
		{
			return str;
		}

		System.Text.StringBuilder result = new(str.Length);
		int lastIndex = 0;

		while (index >= 0)
		{
			result.Append(str, lastIndex, index - lastIndex);
			result.Append(newValue);
			lastIndex = index + oldValue.Length;
			index = str.IndexOf(oldValue, lastIndex, comparisonType);
		}

		result.Append(str, lastIndex, str.Length - lastIndex);
		return result.ToString();
#endif
	}
}
