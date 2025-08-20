// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for absolute file paths
/// </summary>
public interface IAbsoluteFilePath : IFilePath, IAbsolutePath
{
	/// <summary>
	/// Converts this absolute file path to its concrete absolute representation.
	/// Since this is already an absolute file path, returns itself as an <see cref="AbsoluteFilePath"/>.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> representing this absolute file path.</returns>
	public new AbsoluteFilePath AsAbsolute();
}
