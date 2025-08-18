// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a relative (not fully qualified) path
/// </summary>
[IsPath, IsRelativePath]
public sealed record RelativePath : SemanticRelativePath<RelativePath>, IRelativePath
{
	/// <summary>
	/// Converts this relative path to its absolute representation using the current working directory.
	/// </summary>
	/// <returns>An <see cref="AbsolutePath"/> representing the absolute form of this relative path.</returns>
	public AbsolutePath AsAbsolute()
	{
		string absolutePath = Path.GetFullPath(WeakString);
		return AbsolutePath.Create<AbsolutePath>(absolutePath);
	}
}
