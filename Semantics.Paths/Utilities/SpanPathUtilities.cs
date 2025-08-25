// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

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
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	public static ReadOnlySpan<char> GetDirectoryName(ReadOnlySpan<char> path)
#else
	public static string GetDirectoryName(string path)
#endif
	{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
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
#else
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
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

		return lastSeparatorIndex >= 0 ? path.Substring(0, lastSeparatorIndex) : string.Empty;
#endif
	}

	/// <summary>
	/// Gets the filename from a path span without allocating intermediate strings.
	/// </summary>
	/// <param name="path">The path span to parse.</param>
	/// <returns>The filename span, or empty span if no filename.</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
#else
	public static string GetFileName(string path)
#endif
	{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
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

		if (lastSeparatorIndex >= 0)
		{
			// If separator is at the end, return empty span
			if (lastSeparatorIndex == path.Length - 1)
			{
				return default;
			}
			// Otherwise return the part after the separator
			return path[(lastSeparatorIndex + 1)..];
		}

		// No separator found, return the whole path
		return path;
#else
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
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

		if (lastSeparatorIndex >= 0)
		{
			// If separator is at the end, return empty string
			if (lastSeparatorIndex == path.Length - 1)
			{
				return string.Empty;
			}
			// Otherwise return the part after the separator
			return path.Substring(lastSeparatorIndex + 1);
		}

		// No separator found, return the whole path
		return path;
#endif
	}

	/// <summary>
	/// Determines whether a path span ends with a directory separator.
	/// </summary>
	/// <param name="path">The path span to check.</param>
	/// <returns><see langword="true"/> if the path ends with a directory separator; otherwise, <see langword="false"/>.</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
	public static bool EndsWithDirectorySeparator(ReadOnlySpan<char> path)
#else
	public static bool EndsWithDirectorySeparator(string path)
#endif
	{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
		return !path.IsEmpty &&
			   (path[^1] == Path.DirectorySeparatorChar || path[^1] == Path.AltDirectorySeparatorChar);
#else
		return !string.IsNullOrEmpty(path) &&
			   (path[path.Length - 1] == Path.DirectorySeparatorChar || path[path.Length - 1] == Path.AltDirectorySeparatorChar);
#endif
	}
}
