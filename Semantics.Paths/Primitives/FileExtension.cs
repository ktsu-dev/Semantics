// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a file extension (starts with a period)
/// </summary>
[IsFileExtension]
public sealed record FileExtension : SemanticString<FileExtension>, IFileExtension
{
}
