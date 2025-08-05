// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata structure for dimensions generation.
/// </summary>
public class DimensionsMetadata
{
	public List<PhysicalDimension> PhysicalDimensions { get; set; } = [];
}

/// <summary>
/// Definition of a physical dimension containing multiple quantities.
/// </summary>
public class PhysicalDimension
{
	public string Name { get; set; } = string.Empty;
	public string Symbol { get; set; } = string.Empty;
	public Dictionary<string, int> DimensionalFormula { get; set; } = [];
	public List<QuantityDefinition> Quantities { get; set; } = [];
}

/// <summary>
/// Definition of a quantity within a physical dimension.
/// </summary>
public class QuantityDefinition
{
	public string Name { get; set; } = string.Empty;
	public List<string> AvailableUnits { get; set; } = [];
	public string Description { get; set; } = string.Empty;
	public bool Scalar { get; set; } = true;
	public bool Vectors { get; set; } = false;
	public List<RelationshipDefinition> Integrals { get; set; } = [];
	public List<RelationshipDefinition> Derivatives { get; set; } = [];
}

/// <summary>
/// Definition of a mathematical relationship between quantities.
/// </summary>
public class RelationshipDefinition
{
	public string Other { get; set; } = string.Empty;
	public string Result { get; set; } = string.Empty;
}
