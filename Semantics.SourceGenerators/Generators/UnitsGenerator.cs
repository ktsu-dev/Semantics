// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.CodeGen;
using Semantics.SourceGenerators.Models;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

/// <summary>
/// Source generator that creates the per-unit record types and the static <c>Units</c>
/// catalogue from units.json, cross-referenced with dimensions.json so each unit can
/// be tagged with its per-dimension marker interface.
/// </summary>
/// <remarks>
/// Each emitted unit implements <see cref="ktsu.Semantics.Quantities.IUnit"/> plus
/// the <c>I{Dim}Unit</c> marker(s) emitted by <see cref="DimensionsGenerator"/>, so
/// generated quantities can accept dimensionally-correct units only at compile time.
/// </remarks>
[Generator]
public class UnitsGenerator : SemanticsMultiFileGenerator
{
	/// <summary>
	/// Both metadata files, because each unit's declaration records which dimensions use it, and
	/// that mapping only exists in <c>dimensions.json</c>.
	/// </summary>
	/// <remarks>
	/// This generator carried its own copy of the same workaround <c>QuantitiesGenerator</c> did —
	/// an <c>Initialize</c> override, a private JSON loader, a combining type, and a dead shim for
	/// the single-file base's abstract contract. Two generators reimplementing the same thing is
	/// what made multi-file support belong in the base.
	/// </remarks>
	protected override IReadOnlyList<string> MetadataFileNames => ["units.json", "dimensions.json"];

	/// <inheritdoc/>
	protected override void Generate(SourceProductionContext context, MetadataSet metadata)
	{
		UnitsMetadata? units = metadata["units.json"]?.Deserialize<UnitsMetadata>(context, MetadataParseFailed);
		if (units is null)
		{
			return;
		}

		// A missing dimensions.json is reported as SEM006; an empty set still lets the unit
		// declarations themselves be emitted, just without their dimension cross-references.
		DimensionsMetadata dimensions =
			metadata["dimensions.json"]?.Deserialize<DimensionsMetadata>(context, MetadataParseFailed) ?? new DimensionsMetadata();

		using CodeBlocker codeBlocker = CreateCodeBlocker();
		GenerateInner(context, units, dimensions, codeBlocker);
	}

	private static void GenerateInner(SourceProductionContext context, UnitsMetadata units, DimensionsMetadata dimensions, CodeBlocker codeBlocker)
	{
		Dictionary<string, List<string>> unitToDimensions = BuildUnitToDimensionsMap(dimensions);

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "Units.g.cs",
			Namespace = "ktsu.Semantics.Quantities.Units",
			Usings =
			{
				"ktsu.Semantics.Quantities",
				"static ktsu.Semantics.Quantities.Units.ConversionConstants",
			},
		};

		List<string> catalogueUnitNames = [];

		foreach (UnitCategory category in units.UnitCategories)
		{
			foreach (UnitDefinition unit in category.Units)
			{
				List<string> dims = unitToDimensions.TryGetValue(unit.Name, out List<string>? d) ? d : [];

				sourceFileTemplate.Classes.Add(BuildUnitClass(unit, dims));
				catalogueUnitNames.Add(unit.Name);
			}
		}

		sourceFileTemplate.Classes.Add(BuildUnitsCatalogue(catalogueUnitNames));

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}

	/// <summary>
	/// Indexes every dimension that declares a given unit in its <c>availableUnits</c>, so each
	/// emitted unit type can implement the matching <c>I{Dimension}Unit</c> marker interfaces.
	/// </summary>
	private static Dictionary<string, List<string>> BuildUnitToDimensionsMap(DimensionsMetadata dimensions)
	{
		Dictionary<string, List<string>> unitToDimensions = [];
		foreach (PhysicalDimension dim in dimensions.PhysicalDimensions ?? [])
		{
			foreach (string unitName in dim.AvailableUnits ?? [])
			{
				if (!unitToDimensions.TryGetValue(unitName, out List<string>? list))
				{
					list = [];
					unitToDimensions[unitName] = list;
				}

				list.Add(dim.Name);
			}
		}

		return unitToDimensions;
	}

	/// <summary>
	/// Builds the sealed record for one unit, carrying its name, symbol, system, dimension, and
	/// the affine to-base conversion (factor plus offset).
	/// </summary>
	private static ClassTemplate BuildUnitClass(UnitDefinition unit, List<string> dims)
	{
		List<string> interfaces = ["IUnit"];
		foreach (string dimName in dims)
		{
			interfaces.Add($"I{dimName}Unit");
		}

		string factorExpr = BuildToBaseFactorExpression(unit);
		string offsetExpr = string.IsNullOrEmpty(unit.Offset) || unit.Offset == "0"
			? "0d"
			: unit.Offset;
		string dimensionExpr = dims.Count > 0
			? $"PhysicalDimensions.{dims[0]}"
			: "null!";

		return new ClassTemplate
		{
			Comments =
			{
				Emit.SummaryOpen,
				$"/// {unit.Description}",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Record,
			Keywords = {Emit.Public, "sealed"},
			Name = unit.Name,
			Members =
			{
				new ConstructorTemplate()
				{
					Comments = {"/// <summary>Initializes a new instance of the unit.</summary>"},
					Keywords = {Emit.Public},
					Name = unit.Name,
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the full name of the unit.</summary>"},
					Keywords = {Emit.Public, "string"},
					Name = $"Name => \"{unit.Name}\"",
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the symbol/abbreviation of the unit.</summary>"},
					Keywords = {Emit.Public, "string"},
					Name = $"Symbol => \"{unit.Symbol}\"",
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the unit system this unit belongs to.</summary>"},
					Keywords = {Emit.Public, "UnitSystem"},
					Name = $"System => UnitSystem.{unit.System}",
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the physical dimension this unit measures.</summary>"},
					Keywords = {Emit.Public, "DimensionInfo"},
					Name = $"Dimension => {dimensionExpr}",
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the multiplication factor used in the to-base affine conversion.</summary>"},
					Keywords = {Emit.Public, "double"},
					Name = $"ToBaseFactor => {factorExpr}",
				},
				new FieldTemplate()
				{
					Comments = {"/// <summary>Gets the additive offset used in the to-base affine conversion.</summary>"},
					Keywords = {Emit.Public, "double"},
					Name = $"ToBaseOffset => {offsetExpr}",
				},
			},
		}.WithInterfaces(interfaces);
	}

	/// <summary>
	/// Builds the static <c>Units</c> catalogue exposing one singleton per declared unit,
	/// ordered by name so the emitted file is stable across builds.
	/// </summary>
	private static ClassTemplate BuildUnitsCatalogue(List<string> catalogueUnitNames)
	{
		ClassTemplate unitsCatalogue = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Static catalogue exposing one singleton per declared unit. Generated quantity",
				"/// types accept these on their typed <c>In(...)</c> methods.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
			Name = "Units",
		};

		foreach (string unitName in catalogueUnitNames.OrderBy(n => n, StringComparer.Ordinal))
		{
			unitsCatalogue.Members.Add(new FieldTemplate()
			{
				Comments = {$"/// <summary>Singleton <c>{unitName}</c> instance.</summary>"},
				Keywords = {Emit.Public, Emit.Static, "readonly", unitName},
				Name = unitName,
				DefaultValue = $"new {unitName}()",
			});
		}

		return unitsCatalogue;
	}

	/// <summary>
	/// Builds the literal-double expression for the unit's to-base multiplication factor.
	/// Folds <c>Magnitude</c> (metric prefix) and <c>ConversionFactor</c> (named constant)
	/// together so the runtime <c>IUnit.ToBase</c> default implementation sees a single scalar.
	/// </summary>
	private static string BuildToBaseFactorExpression(UnitDefinition unit)
	{
		bool hasMagnitude = !string.IsNullOrEmpty(unit.Magnitude) && unit.Magnitude != "1";
		bool hasFactor = !string.IsNullOrEmpty(unit.ConversionFactor) && unit.ConversionFactor != "1";

		if (hasMagnitude && hasFactor)
		{
			return $"MetricMagnitudes.{unit.Magnitude} * {unit.ConversionFactor}";
		}

		if (hasMagnitude)
		{
			return $"MetricMagnitudes.{unit.Magnitude}";
		}

		if (hasFactor)
		{
			return $"{unit.ConversionFactor}";
		}

		return "1d";
	}
}
