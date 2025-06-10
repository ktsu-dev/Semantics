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
}
