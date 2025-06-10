// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Interface for relative file paths
/// </summary>
public interface IRelativeFilePath : IFilePath, IRelativePath
{
	/// <summary>
	/// Converts this interface to its concrete <see cref="RelativeFilePath"/> implementation.
	/// </summary>
	/// <returns>A <see cref="RelativeFilePath"/> instance with the same path value.</returns>
	public RelativeFilePath AsRelativeFilePath();
}
