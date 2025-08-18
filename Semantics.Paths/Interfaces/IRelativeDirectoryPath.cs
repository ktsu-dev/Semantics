// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for relative directory paths
/// </summary>
public interface IRelativeDirectoryPath : IDirectoryPath, IRelativePath
{
	/// <summary>
	/// Converts this relative directory path to its absolute representation using the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing the absolute form of this relative directory path.</returns>
	public new AbsoluteDirectoryPath AsAbsolute();
}
