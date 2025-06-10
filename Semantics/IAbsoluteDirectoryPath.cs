// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for absolute directory paths
/// </summary>
public interface IAbsoluteDirectoryPath : IDirectoryPath, IAbsolutePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteDirectoryPath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> instance with the same path value.</returns>
	public AbsoluteDirectoryPath AsAbsoluteDirectoryPath();
}
