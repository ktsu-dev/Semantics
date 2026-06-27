// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Unified metadata model for JSON-driven metric magnitudes generation
/// </summary>
public class MagnitudesMetadata
{
	public List<MagnitudeDefinition> Magnitudes { get; set; } = [];
}

/// <summary>
/// Unified definition of a single metric magnitude for code generation.
/// Supports both simple extension generation and full metadata generation.
/// </summary>
public class MagnitudeDefinition
{
	public string Name { get; set; } = string.Empty;
	public string Symbol { get; set; } = string.Empty;
	public int Exponent { get; set; }
}
