// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a file extension (starts with a period)
/// </summary>
[IsFileExtension]
public sealed record FileExtension : SemanticString<FileExtension>, IFileExtension
{
}
