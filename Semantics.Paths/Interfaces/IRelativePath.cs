// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

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
