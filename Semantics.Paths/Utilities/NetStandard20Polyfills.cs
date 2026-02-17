// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using System.IO;

/// <summary>
/// Provides path utility methods with netstandard2.0 polyfill support.
/// </summary>
internal static class PathHelper
{
	/// <summary>
	/// Returns a value that indicates whether a file path is fully qualified.
	/// </summary>
	/// <param name="path">A file path.</param>
	/// <returns>true if the path is fully qualified; otherwise, false.</returns>
	public static bool IsPathFullyQualified(string path)
	{
#if NETSTANDARD2_0
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		if (path.Length >= 2 && path[1] == ':' && ((path[0] >= 'A' && path[0] <= 'Z') || (path[0] >= 'a' && path[0] <= 'z')))
		{
			return true;
		}

		if (path.Length >= 2 && path[0] == '\\' && path[1] == '\\')
		{
			return true;
		}

		return path.Length >= 1 && path[0] == '/';
#else
		return Path.IsPathFullyQualified(path);
#endif
	}

	/// <summary>
	/// Returns a relative path from one path to another.
	/// </summary>
	/// <param name="relativeTo">The source path the result should be relative to.</param>
	/// <param name="path">The destination path.</param>
	/// <returns>The relative path.</returns>
	public static string GetRelativePath(string relativeTo, string path)
	{
#if NETSTANDARD2_0
		relativeTo = Path.GetFullPath(relativeTo);
		path = Path.GetFullPath(path);

		if (!relativeTo.EndsWith(Path.DirectorySeparatorChar.ToString(), System.StringComparison.Ordinal) &&
			!relativeTo.EndsWith(Path.AltDirectorySeparatorChar.ToString(), System.StringComparison.Ordinal))
		{
			relativeTo += Path.DirectorySeparatorChar;
		}

		System.Uri relativeToUri = new(relativeTo);
		System.Uri pathUri = new(path);
		System.Uri relativeUri = relativeToUri.MakeRelativeUri(pathUri);
		string relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());
		return relativePath.Replace('/', Path.DirectorySeparatorChar);
#else
		return Path.GetRelativePath(relativeTo, path);
#endif
	}

	/// <summary>
	/// Returns the absolute path for the specified path string, resolving against a base path.
	/// </summary>
	/// <param name="path">The file or directory for which to obtain absolute path information.</param>
	/// <param name="basePath">The base path to resolve against.</param>
	/// <returns>The fully qualified location of path.</returns>
	public static string GetFullPath(string path, string basePath)
	{
#if NETSTANDARD2_0
		if (Path.IsPathRooted(path))
		{
			return Path.GetFullPath(path);
		}

		return Path.GetFullPath(Path.Combine(basePath, path));
#else
		return Path.GetFullPath(path, basePath);
#endif
	}
}

#if NETSTANDARD2_0

/// <summary>
/// String extension methods for netstandard2.0 compatibility.
/// </summary>
internal static class StringPolyfill
{
	/// <summary>
	/// Returns a new string in which all occurrences of a specified string are replaced with another,
	/// using the provided comparison type.
	/// </summary>
	public static string Replace(this string source, string oldValue, string newValue, System.StringComparison comparisonType)
	{
		if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(oldValue))
		{
			return source;
		}

		System.Text.StringBuilder result = new();
		int startIndex = 0;

		while (true)
		{
			int index = source.IndexOf(oldValue, startIndex, comparisonType);
			if (index < 0)
			{
				result.Append(source, startIndex, source.Length - startIndex);
				break;
			}

			result.Append(source, startIndex, index - startIndex);
			result.Append(newValue);
			startIndex = index + oldValue.Length;
		}

		return result.ToString();
	}
}

#endif
