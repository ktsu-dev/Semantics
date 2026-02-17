// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Root metadata structure for physical dimensions.
/// </summary>
public class DimensionsMetadata
{
	public List<PhysicalDimension> PhysicalDimensions { get; set; } = [];
}

/// <summary>
/// Definition of a physical dimension with vector form quantities and relationships.
/// Relationships reference dimension names and are resolved to concrete types at each vector form.
/// </summary>
public class PhysicalDimension
{
	public string Name { get; set; } = string.Empty;
	public string Symbol { get; set; } = string.Empty;
	public Dictionary<string, int> DimensionalFormula { get; set; } = [];
	public List<string> AvailableUnits { get; set; } = [];
	public VectorFormsMap Quantities { get; set; } = new();
	public List<RelationshipDefinition> Integrals { get; set; } = [];
	public List<RelationshipDefinition> Derivatives { get; set; } = [];
	public List<RelationshipDefinition> DotProducts { get; set; } = [];
	public List<RelationshipDefinition> CrossProducts { get; set; } = [];
}

/// <summary>
/// Map of vector forms for a physical dimension.
/// Each entry is nullable; absent entries mean that vector form is not available.
/// </summary>
public class VectorFormsMap
{
	public VectorFormDefinition? Vector0 { get; set; }
	public VectorFormDefinition? Vector1 { get; set; }
	public VectorFormDefinition? Vector2 { get; set; }
	public VectorFormDefinition? Vector3 { get; set; }
	public VectorFormDefinition? Vector4 { get; set; }
}

/// <summary>
/// Definition of a single vector form within a dimension.
/// </summary>
public class VectorFormDefinition
{
	/// <summary>Gets or sets the base type name for this vector form.</summary>
	public string Base { get; set; } = string.Empty;

	/// <summary>Gets or sets optional semantic overloads for this vector form.</summary>
	public List<OverloadDefinition> Overloads { get; set; } = [];
}

/// <summary>
/// Definition of a semantic overload (typed alias) for a base quantity type.
/// </summary>
public class OverloadDefinition
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public Dictionary<string, string> Relationships { get; set; } = [];
}

/// <summary>
/// Definition of a mathematical relationship between dimensions.
/// </summary>
public class RelationshipDefinition
{
	public string Other { get; set; } = string.Empty;
	public string Result { get; set; } = string.Empty;
}
