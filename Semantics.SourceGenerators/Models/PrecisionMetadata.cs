// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata structure for precision types generation.
/// </summary>
public class PrecisionMetadata
{
	public List<string> StorageTypes { get; set; } = [];
}
