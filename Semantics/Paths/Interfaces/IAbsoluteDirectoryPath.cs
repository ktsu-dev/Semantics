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
	/// Converts this absolute directory path to its concrete absolute representation.
	/// Since this is already an absolute directory path, returns itself as an <see cref="AbsoluteDirectoryPath"/>.
	/// </summary>
	/// <returns>An <see cref="AbsoluteDirectoryPath"/> representing this absolute directory path.</returns>
	public new AbsoluteDirectoryPath AsAbsolute();
}
