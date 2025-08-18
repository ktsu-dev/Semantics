// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents an absolute (fully qualified) path
/// </summary>
[IsPath, IsAbsolutePath]
public sealed record AbsolutePath : SemanticAbsolutePath<AbsolutePath>, IAbsolutePath
{
	/// <summary>
	/// Converts this absolute path to its concrete absolute representation.
	/// Since this is already an absolute path, returns itself.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing this absolute path.</returns>
	public AbsolutePath AsAbsolute() => this;
}
