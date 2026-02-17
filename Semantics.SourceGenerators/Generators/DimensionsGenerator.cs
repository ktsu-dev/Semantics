// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the PhysicalDimensions.cs file from JSON metadata.
/// </summary>
[Generator]
public class DimensionsGenerator : GeneratorBase<DimensionsMetadata>
{
	public DimensionsGenerator() : base("dimensions.json") { }

	protected override void Generate(SourceProductionContext context, DimensionsMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.PhysicalDimensions == null || metadata.PhysicalDimensions.Count == 0)
		{
			return;
		}

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "PhysicalDimensions.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings =
			[
				"System.Collections.Generic",
			],
		};

		// Generate DimensionInfo record
		ClassTemplate dimensionInfoRecord = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Dimension information record.",
				"/// </summary>",
			],
			Keywords = ["public", "record"],
			Name = "DimensionInfo(string Name, string Symbol, Dictionary<string, int> DimensionalFormula, List<string> Quantities)",
		};
		sourceFileTemplate.Classes.Add(dimensionInfoRecord);

		// Generate PhysicalDimensions static class
		ClassTemplate dimensionsClass = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Static registry of physical dimensions.",
				"/// </summary>",
			],
			Keywords = ["public", "static", "class"],
			Name = "PhysicalDimensions",
		};

		IOrderedEnumerable<PhysicalDimension> sortedDimensions = metadata.PhysicalDimensions.OrderBy(d => d.Name);

		foreach (PhysicalDimension dimension in sortedDimensions)
		{
			string description = $"Physical dimension: {dimension.Name}";

			// Build dimensional formula initializer
			string formulaInit;
			if (dimension.DimensionalFormula.Count > 0)
			{
				IEnumerable<string> entries = dimension.DimensionalFormula.Select(kvp => $"[\"{kvp.Key}\"] = {kvp.Value}");
				formulaInit = $"new Dictionary<string, int> {{ {string.Join(", ", entries)} }}";
			}
			else
			{
				formulaInit = "new Dictionary<string, int>()";
			}

			// Collect all type names from vector forms (base types + overloads)
			List<string> quantityNames = [];
			VectorFormDefinition?[] forms = [dimension.Quantities.Vector0, dimension.Quantities.Vector1, dimension.Quantities.Vector2, dimension.Quantities.Vector3, dimension.Quantities.Vector4];
			foreach (VectorFormDefinition? form in forms)
			{
				if (form != null)
				{
					quantityNames.Add(form.Base);
					foreach (OverloadDefinition overload in form.Overloads)
					{
						quantityNames.Add(overload.Name);
					}
				}
			}

			// Build quantities list initializer
			string quantitiesInit;
			if (quantityNames.Count > 0)
			{
				IEnumerable<string> names = quantityNames.Select(n => $"\"{n}\"");
				quantitiesInit = $"new List<string> {{ {string.Join(", ", names)} }}";
			}
			else
			{
				quantitiesInit = "new List<string>()";
			}

			dimensionsClass.Members.Add(new FieldTemplate()
			{
				Comments = [$"/// <summary>{description}</summary>"],
				Keywords = ["public", "static", "readonly", "DimensionInfo"],
				Name = dimension.Name,
				DefaultValue = $"new(\"{dimension.Name}\", \"{dimension.Symbol}\", {formulaInit}, {quantitiesInit})",
			});
		}

		// Generate the All property
		string allDimensions = string.Join(", ", sortedDimensions.Select(d => d.Name));
		dimensionsClass.Members.Add(new FieldTemplate()
		{
			Comments = ["/// <summary>Gets a frozen collection of all standard physical dimensions.</summary>"],
			Keywords = ["public", "static", "IReadOnlySet<DimensionInfo>"],
			Name = "All",
			DefaultValue = $"new HashSet<DimensionInfo>([ {allDimensions} ])",
		});

		sourceFileTemplate.Classes.Add(dimensionsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
