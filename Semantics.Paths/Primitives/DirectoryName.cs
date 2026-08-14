// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a directory name
/// </summary>
[IsDirectoryName]
public sealed record DirectoryName : SemanticString<DirectoryName>, IDirectoryName
{
}
