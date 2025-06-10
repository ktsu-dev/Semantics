// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for absolute file paths
/// </summary>
public interface IAbsoluteFilePath : IFilePath, IAbsolutePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="AbsoluteFilePath"/> implementation.
	/// </summary>
	/// <returns>An <see cref="AbsoluteFilePath"/> instance with the same path value.</returns>
	public AbsoluteFilePath AsAbsoluteFilePath();
}
