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

	/// <summary>
	/// Validates the deserialised metadata, returning a list of human-readable issues.
	/// An empty list means the metadata is well-formed.
	/// </summary>
	/// <remarks>
	/// Per #60: catches malformed entries before they reach the generator emit
	/// pass, so problems surface as Roslyn diagnostics rather than a mid-emit crash
	/// or a silently dropped operator. Cross-dimension reference checks are still
	/// performed in the generator (#56 / SEM001) because they need the full
	/// dimension map.
	/// </remarks>
	public List<string> Validate()
	{
		List<string> issues = [];

		if (PhysicalDimensions.Count == 0)
		{
			issues.Add("dimensions.json contains no physicalDimensions entries.");
			return issues;
		}

		HashSet<string> seenDimensionNames = [];
		HashSet<string> seenTypeNames = [];

		foreach (PhysicalDimension dim in PhysicalDimensions)
		{
			string label = string.IsNullOrEmpty(dim.Name) ? "<unnamed>" : dim.Name;

			if (string.IsNullOrEmpty(dim.Name))
			{
				issues.Add("A physicalDimensions entry is missing 'name'.");
			}
			else if (!seenDimensionNames.Add(dim.Name))
			{
				issues.Add($"Dimension '{dim.Name}' is declared more than once.");
			}

			if (string.IsNullOrEmpty(dim.Symbol))
			{
				issues.Add($"Dimension '{label}' is missing 'symbol'.");
			}

			if (dim.AvailableUnits.Count == 0)
			{
				issues.Add($"Dimension '{label}' has an empty 'availableUnits' list.");
			}
			else
			{
				foreach (string unit in dim.AvailableUnits)
				{
					if (string.IsNullOrWhiteSpace(unit))
					{
						issues.Add($"Dimension '{label}' has a blank entry in 'availableUnits'.");
					}
				}
			}

			VectorFormDefinition?[] forms = [
				dim.Quantities.Vector0,
				dim.Quantities.Vector1,
				dim.Quantities.Vector2,
				dim.Quantities.Vector3,
				dim.Quantities.Vector4,
			];

			if (System.Array.TrueForAll(forms, f => f == null))
			{
				issues.Add($"Dimension '{label}' declares no vector forms (vector0..vector4).");
			}

			for (int i = 0; i < forms.Length; i++)
			{
				VectorFormDefinition? form = forms[i];
				if (form == null)
				{
					continue;
				}

				if (string.IsNullOrEmpty(form.Base))
				{
					issues.Add($"Dimension '{label}' vector{i} is missing 'base'.");
				}
				else if (!seenTypeNames.Add(form.Base))
				{
					issues.Add($"Type name '{form.Base}' (dimension '{label}' vector{i}) collides with another base or overload.");
				}

				foreach (OverloadDefinition overload in form.Overloads)
				{
					if (string.IsNullOrEmpty(overload.Name))
					{
						issues.Add($"Dimension '{label}' vector{i} has an overload missing 'name'.");
						continue;
					}

					if (!seenTypeNames.Add(overload.Name))
					{
						issues.Add($"Overload type name '{overload.Name}' (dimension '{label}' vector{i}) collides with another base or overload.");
					}
				}
			}
		}

		return issues;
	}
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

	/// <summary>
	/// Optional explicit list of vector forms (0..4) at which this relationship should
	/// emit operators. When empty, the generator uses sensible defaults from the
	/// relationship kind: <c>integrals</c>/<c>derivatives</c> default to all common forms,
	/// <c>dotProducts</c> to V1+, <c>crossProducts</c> to V3 only. When set, missing forms
	/// on either side surface as <c>SEM003</c> diagnostics instead of being silently dropped.
	/// </summary>
	public List<int> Forms { get; set; } = [];
}
