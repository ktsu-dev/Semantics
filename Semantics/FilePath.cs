// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Represents a file path (path to a file)
/// </summary>
[IsPath, IsFilePath]
public sealed record FilePath : SemanticFilePath<FilePath>, IFilePath
{
}
