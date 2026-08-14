// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a filename (without directory path)
/// </summary>
[IsFileName]
public sealed record FileName : SemanticString<FileName>, IFileName
{
}
