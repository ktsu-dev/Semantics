// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata structure for precision types generation.
/// </summary>
public class PrecisionMetadata
{
	public List<string> StorageTypes { get; set; } = [];
}
