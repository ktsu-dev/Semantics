// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata structure for conversions generation.
/// </summary>
public class ConversionsMetadata
{
	public List<ConversionCategory> Conversions { get; set; } = [];
}

/// <summary>
/// Category of conversion factors.
/// </summary>
public class ConversionCategory
{
	public string Category { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<ConversionFactor> Factors { get; set; } = [];
}

/// <summary>
/// Definition of a single conversion factor for code generation.
/// </summary>
public class ConversionFactor
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}
