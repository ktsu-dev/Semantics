// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for file paths (paths to files)
/// </summary>
public interface IFilePath : IPath
{
	/// <summary>
	/// Converts this file path to an absolute file path representation.
	/// For relative paths, this resolves against the current working directory.
	/// For absolute paths, this returns the path itself.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute path to this file.</returns>
	/// <remarks>
	/// This method provides a consistent way to obtain an absolute path regardless of whether
	/// the underlying implementation is relative or absolute. For relative paths, the conversion
	/// uses the current working directory as the base.
	/// </remarks>
	public AbsoluteFilePath AsAbsolute();
}
