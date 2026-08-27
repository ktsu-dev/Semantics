// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.CodeGen;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

/// <summary>
/// Source generator that creates the PhysicalDimensions.cs file from JSON metadata.
/// </summary>
[Generator]
public class DimensionsGenerator : SemanticsGenerator<DimensionsMetadata>
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
			{
				"System.Collections.Generic",
			},
		};

		// Generate DimensionInfo record
		ClassTemplate dimensionInfoRecord = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Dimension information record.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Record,
			Keywords = {Emit.Public},
			Name = "DimensionInfo(string Name, string Symbol, Dictionary<string, int> DimensionalFormula, List<string> Quantities)",
		};
		sourceFileTemplate.Classes.Add(dimensionInfoRecord);

		// Generate PhysicalDimensions static class
		ClassTemplate dimensionsClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Static registry of physical dimensions.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
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
			foreach (VectorFormDefinition form in forms.OfType<VectorFormDefinition>())
			{
				quantityNames.Add(form.Base);
				quantityNames.AddRange(form.Overloads.Select(overload => overload.Name));
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
				Comments = {$"/// <summary>{description}</summary>"},
				Keywords = {Emit.Public, Emit.Static, "readonly", "DimensionInfo"},
				Name = dimension.Name,
				DefaultValue = $"new(\"{dimension.Name}\", \"{dimension.Symbol}\", {formulaInit}, {quantitiesInit})",
			});
		}

		// Generate the All property
		string allDimensions = string.Join(", ", sortedDimensions.Select(d => d.Name));
		dimensionsClass.Members.Add(new FieldTemplate()
		{
			Comments = {"/// <summary>Gets a frozen collection of all standard physical dimensions.</summary>"},
			Keywords = {Emit.Public, Emit.Static, "IReadOnlySet<DimensionInfo>"},
			Name = "All",
			DefaultValue = $"new HashSet<DimensionInfo>([ {allDimensions} ])",
		});

		sourceFileTemplate.Classes.Add(dimensionsClass);

		// Emit per-dimension marker interfaces (I{Dim}Unit : IUnit) so generated
		// quantity types can accept dimensionally-compatible units only.
		foreach (string dimensionName in sortedDimensions.Select(dimension => dimension.Name))
		{
			sourceFileTemplate.Classes.Add(new ClassTemplate
			{
				Comments =
				{
					Emit.SummaryOpen,
					$"/// Marker interface implemented by every unit of the <c>{dimensionName}</c> dimension.",
					"/// Generated quantities use this to make <c>In(...)</c> dimensionally type-safe at compile time.",
					Emit.SummaryClose,
				},
				Kind = TypeKind.Interface,
				Keywords = {Emit.Public},
				Name = $"I{dimensionName}Unit",
				Interfaces = {"IUnit"},
			});
		}

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
