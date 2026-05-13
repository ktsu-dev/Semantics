// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

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
public class UnitsGenerator : GeneratorBase<UnitsMetadata>
{
	public UnitsGenerator() : base("units.json") { }

	private sealed class CombinedMetadata
	{
		public UnitsMetadata Units { get; }
		public DimensionsMetadata Dimensions { get; }

		public CombinedMetadata(UnitsMetadata units, DimensionsMetadata dimensions)
		{
			Units = units;
			Dimensions = dimensions;
		}
	}

	public override void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<UnitsMetadata?> unitsProvider = LoadJson<UnitsMetadata>(context, "units.json");
		IncrementalValueProvider<DimensionsMetadata?> dimensionsProvider = LoadJson<DimensionsMetadata>(context, "dimensions.json");
		IncrementalValueProvider<CombinedMetadata?> combined = unitsProvider.Combine(dimensionsProvider).Select(static (pair, _) =>
			pair.Left == null ? null : new CombinedMetadata(pair.Left, pair.Right ?? new DimensionsMetadata()));

		context.RegisterSourceOutput(combined, (ctx, metadata) =>
		{
			if (metadata == null)
			{
				return;
			}

			using CodeBlocker codeBlocker = CodeBlocker.Create();
			GenerateInner(ctx, metadata.Units, metadata.Dimensions, codeBlocker);
		});
	}

	private static IncrementalValueProvider<TMeta?> LoadJson<TMeta>(IncrementalGeneratorInitializationContext context, string filename)
		where TMeta : class
	{
		return context.AdditionalTextsProvider
			.Where(file => file.Path.EndsWith(filename, StringComparison.InvariantCulture))
			.Select((file, ct) => file.GetText(ct)?.ToString() ?? "")
			.Where(content => !string.IsNullOrEmpty(content))
			.Select((content, _) =>
			{
				try
				{
					return JsonSerializer.Deserialize<TMeta>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				}
				catch (JsonException)
				{
					return null;
				}
			})
			.Where(m => m != null)
			.Collect()
			.Select((arr, _) => arr.FirstOrDefault());
	}

	protected override void Generate(SourceProductionContext context, UnitsMetadata metadata, CodeBlocker codeBlocker)
		=> GenerateInner(context, metadata, new DimensionsMetadata(), codeBlocker);

	private static void GenerateInner(SourceProductionContext context, UnitsMetadata units, DimensionsMetadata dimensions, CodeBlocker codeBlocker)
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

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "Units.g.cs",
			Namespace = "ktsu.Semantics.Quantities.Units",
			Usings =
			[
				"ktsu.Semantics.Quantities",
				"static ktsu.Semantics.Quantities.Units.ConversionConstants",
			],
		};

		List<string> catalogueUnitNames = [];

		foreach (UnitCategory category in units.UnitCategories)
		{
			foreach (UnitDefinition unit in category.Units)
			{
				List<string> dims = unitToDimensions.TryGetValue(unit.Name, out List<string>? d) ? d : [];

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

				ClassTemplate unitClass = new()
				{
					Comments =
					[
						"/// <summary>",
						$"/// {unit.Description}",
						"/// </summary>",
					],
					Keywords = ["public", "sealed", "record"],
					Name = unit.Name,
					Interfaces = interfaces,
					Members =
					[
						new ConstructorTemplate()
						{
							Comments = ["/// <summary>Initializes a new instance of the unit.</summary>"],
							Keywords = ["public"],
							Name = unit.Name,
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the full name of the unit.</summary>"],
							Keywords = ["public", "string"],
							Name = $"Name => \"{unit.Name}\"",
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the symbol/abbreviation of the unit.</summary>"],
							Keywords = ["public", "string"],
							Name = $"Symbol => \"{unit.Symbol}\"",
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the unit system this unit belongs to.</summary>"],
							Keywords = ["public", "UnitSystem"],
							Name = $"System => UnitSystem.{unit.System}",
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the physical dimension this unit measures.</summary>"],
							Keywords = ["public", "DimensionInfo"],
							Name = $"Dimension => {dimensionExpr}",
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the multiplication factor used in the to-base affine conversion.</summary>"],
							Keywords = ["public", "double"],
							Name = $"ToBaseFactor => {factorExpr}",
						},
						new FieldTemplate()
						{
							Comments = ["/// <summary>Gets the additive offset used in the to-base affine conversion.</summary>"],
							Keywords = ["public", "double"],
							Name = $"ToBaseOffset => {offsetExpr}",
						},
					],
				};

				sourceFileTemplate.Classes.Add(unitClass);
				catalogueUnitNames.Add(unit.Name);
			}
		}

		// Emit the static Units catalogue with one singleton per unit.
		ClassTemplate unitsCatalogue = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Static catalogue exposing one singleton per declared unit. Generated quantity",
				"/// types accept these on their typed <c>In(...)</c> methods.",
				"/// </summary>",
			],
			Keywords = ["public", "static", "class"],
			Name = "Units",
		};

		foreach (string unitName in catalogueUnitNames.OrderBy(n => n, StringComparer.Ordinal))
		{
			unitsCatalogue.Members.Add(new FieldTemplate()
			{
				Comments = [$"/// <summary>Singleton <c>{unitName}</c> instance.</summary>"],
				Keywords = ["public", "static", "readonly", unitName],
				Name = unitName,
				DefaultValue = $"new {unitName}()",
			});
		}

		sourceFileTemplate.Classes.Add(unitsCatalogue);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
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
