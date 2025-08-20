// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for relative file paths
/// </summary>
public interface IRelativeFilePath : IFilePath, IRelativePath
{
	/// <summary>
	/// Converts this relative file path to its absolute representation using the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing the absolute form of this relative file path.</returns>
	public new AbsoluteFilePath AsAbsolute();
}
