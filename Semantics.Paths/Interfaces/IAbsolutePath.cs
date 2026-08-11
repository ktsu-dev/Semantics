// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

/// <summary>
/// Interface for absolute (fully qualified) paths
/// </summary>
public interface IAbsolutePath : IPath
{
	/// <summary>
	/// Converts this absolute path to its concrete absolute representation.
	/// Since this is already an absolute path, returns the path as an <see cref="AbsolutePath"/>.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing this absolute path.</returns>
	public AbsolutePath AsAbsolute();
}
