// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Paths;

using ktsu.Semantics.Strings;

/// <summary>
/// Represents a filename (without directory path)
/// </summary>
[IsFileName]
public sealed record FileName : SemanticString<FileName>, IFileName
{
}
