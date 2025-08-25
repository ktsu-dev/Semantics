// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

#if !NET6_0_OR_GREATER
using System;
#endif
#if !NET5_0_OR_GREATER || NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

#if !NET6_0_OR_GREATER
/// <summary>
/// Polyfill for ArgumentNullException.ThrowIfNull for older .NET versions
/// </summary>
internal static class ArgumentNullExceptionPolyfill
{
	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.
	/// </summary>
	/// <param name="argument">The reference type argument to validate as non-null.</param>
	/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
	public static void ThrowIfNull(object? argument, string? paramName = null)
	{
		if (argument is null)
		{
			throw new ArgumentNullException(paramName);
		}
	}
}
#endif

#if !NET5_0_OR_GREATER
/// <summary>
/// Polyfill for OperatingSystem class for older .NET versions
/// </summary>
internal static class OperatingSystem
{
	/// <summary>
	/// Indicates whether the current application is running on Windows.
	/// </summary>
	/// <returns>true if the current application is running on Windows; otherwise, false.</returns>
	public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}
#endif

#if NETSTANDARD2_0
/// <summary>
/// Polyfill for Path methods not available in .NET Standard 2.0
/// </summary>
internal static class PathPolyfill
{
	/// <summary>
	/// Returns a relative path from one path to another.
	/// </summary>
	/// <param name="relativeTo">The source path the result should be relative to.</param>
	/// <param name="path">The destination path.</param>
	/// <returns>The relative path, or path if the paths don't share the same root.</returns>
	public static string GetRelativePath(string relativeTo, string path)
	{
		// Simplified implementation - in a real scenario you'd want more robust logic
		Uri relativeUri = new(Path.GetFullPath(relativeTo + Path.DirectorySeparatorChar));
		Uri pathUri = new(Path.GetFullPath(path));

		if (relativeUri.Scheme != pathUri.Scheme)
		{
			return path; // Different schemes, can't make relative
		}

		string relativeUriString = relativeUri.MakeRelativeUri(pathUri).ToString();
		return Uri.UnescapeDataString(relativeUriString).Replace('/', Path.DirectorySeparatorChar);
	}

	/// <summary>
	/// Gets a value that indicates whether the specified path string contains absolute or relative path information.
	/// </summary>
	/// <param name="path">The path to test.</param>
	/// <returns>true if path contains an absolute path; otherwise, false.</returns>
	public static bool IsPathFullyQualified(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return false;
		}

		if (Path.IsPathRooted(path))
		{
			// On Windows, check if it's a drive letter or UNC path
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return (path.Length >= 3 && char.IsLetter(path[0]) && path[1] == ':' && path[2] == Path.DirectorySeparatorChar)
					|| path.StartsWith(@"\\", StringComparison.Ordinal);
			}
			// On Unix-like systems, rooted paths are fully qualified
			return true;
		}

		return false;
	}

	/// <summary>
	/// Returns the absolute path for the specified path string, using the specified base path.
	/// </summary>
	/// <param name="path">The relative or absolute path.</param>
	/// <param name="basePath">The base path to use if path is relative.</param>
	/// <returns>The absolute path.</returns>
	public static string GetFullPath(string path, string basePath)
	{
		if (IsPathFullyQualified(path))
		{
			return Path.GetFullPath(path);
		}

		string combinedPath = Path.Combine(basePath, path);
		return Path.GetFullPath(combinedPath);
	}
}

/// <summary>
/// Polyfill for string methods not available in older .NET versions
/// </summary>
internal static class StringPolyfill
{
	/// <summary>
	/// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string, using the provided comparison type.
	/// </summary>
	/// <param name="str">The string to perform the replacement on.</param>
	/// <param name="oldValue">The string to be replaced.</param>
	/// <param name="newValue">The string to replace all occurrences of oldValue.</param>
	/// <param name="comparisonType">One of the enumeration values that determines how this method searches for oldValue.</param>
	/// <returns>A string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.</returns>
	public static string Replace(string str, string oldValue, string newValue, StringComparison comparisonType)
	{
		if (comparisonType == StringComparison.Ordinal)
		{
			return str.Replace(oldValue, newValue);
		}

		// For other comparison types, we need a more complex implementation
		string result = str;
		int index = 0;
		while ((index = result.IndexOf(oldValue, index, comparisonType)) >= 0)
		{
			result = result.Remove(index, oldValue.Length).Insert(index, newValue);
			index += newValue.Length;
		}
		return result;
	}
}
#endif
