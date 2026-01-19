// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a directory name
/// </summary>
[IsDirectoryName]
public sealed record DirectoryName : SemanticString<DirectoryName>, IDirectoryName
{
}
