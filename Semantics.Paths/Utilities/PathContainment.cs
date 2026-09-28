// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using System.IO;
#if !NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

/// <summary>
/// The containment rule shared by <see cref="AbsoluteDirectoryPath.IsChildOf"/>,
/// <see cref="AbsoluteDirectoryPath.IsParentOf"/> and <see cref="AbsoluteFilePath.IsChildOf"/>.
/// </summary>
internal static class PathContainment
{
	/// <summary>
	/// Gets the comparison used to match a parent path against a child path.
	/// </summary>
	/// <remarks>
	/// Windows filesystems ignore case, so there <c>C:\Docs\a</c> is inside <c>C:\docs</c>. Everywhere
	/// else, macOS included, paths are compared ordinally, the same way path equality and hashing
	/// already are. APFS is case-insensitive by default but can be formatted case-sensitive, so on
	/// macOS the ordinal comparison is the one that cannot report a sibling directory as a child.
	/// </remarks>
	internal static StringComparison Comparison { get; } =
#if NET5_0_OR_GREATER
		OperatingSystem.IsWindows()
#else
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
			? StringComparison.OrdinalIgnoreCase
			: StringComparison.Ordinal;

	/// <summary>
	/// Determines whether <paramref name="childFullPath"/> is strictly inside <paramref name="parentFullPath"/>.
	/// </summary>
	/// <param name="childFullPath">The full path of the candidate child.</param>
	/// <param name="parentFullPath">The full path of the candidate parent directory.</param>
	/// <returns>
	/// <see langword="true"/> if the child starts with the parent and continues past a directory
	/// boundary; <see langword="false"/> for the same path, a sibling sharing a name prefix, or an
	/// unrelated path.
	/// </returns>
	internal static bool IsStrictlyInside(string childFullPath, string parentFullPath)
	{
		// A path is never inside itself, however its case is spelled, so the child must be longer.
		if (childFullPath.Length <= parentFullPath.Length
			|| !childFullPath.StartsWith(parentFullPath, Comparison))
		{
			return false;
		}

		// A root ("/", "C:\") already ends in a separator, so anything longer than it is inside it.
		// Otherwise the match must end at a separator, so that "/home2" is not inside "/home".
		return IsSeparator(parentFullPath[parentFullPath.Length - 1])
			|| IsSeparator(childFullPath[parentFullPath.Length]);
	}

	private static bool IsSeparator(char c) =>
		c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
}
