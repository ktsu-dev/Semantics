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
}
