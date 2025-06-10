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
	/// Converts this interface to its concrete <see cref="RelativeDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeDirectoryPath"/> instance with the same path value.</returns>
	public RelativeDirectoryPath AsRelativeDirectoryPath();
}
