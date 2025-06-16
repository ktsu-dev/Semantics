// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for relative paths
/// </summary>
public interface IRelativePath : IPath
{
	/// <summary>
	/// Converts this relative path to its absolute representation using the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing the absolute form of this relative path.</returns>
	public AbsolutePath AsAbsolute();
}
