// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System;
using System.IO;

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
	public static string GetRelativePath(string relativeTo, string path)
	{
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
		return Path.GetRelativePath(relativeTo, path);
#else
		// Fallback implementation for netstandard2.0
		relativeTo = Path.GetFullPath(relativeTo);
		path = Path.GetFullPath(path);

		Uri fromUri = new(AppendDirectorySeparatorChar(relativeTo));
		Uri toUri = new(AppendDirectorySeparatorChar(path));

		if (fromUri.Scheme != toUri.Scheme)
		{
			return path;
		}

		Uri relativeUri = fromUri.MakeRelativeUri(toUri);
		string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

		if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
		{
			relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		return relativePath;
#endif
	}

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
	private static string AppendDirectorySeparatorChar(string path)
	{
		if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
		{
			return path + Path.DirectorySeparatorChar;
		}

		return path;
	}

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
