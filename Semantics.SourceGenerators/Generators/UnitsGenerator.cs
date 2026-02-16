// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the Units.cs file from JSON metadata.
/// </summary>
[Generator]
public class UnitsGenerator : GeneratorBase<UnitsMetadata>
{
	public UnitsGenerator() : base("units.json") { }

	protected override void Generate(SourceProductionContext context, UnitsMetadata metadata, CodeBlocker codeBlocker)
	{
		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "Units.g.cs",
			Namespace = "ktsu.Semantics.Units",
			Usings =
			[
				"static ktsu.Semantics.Units.ConversionConstants",
			],
		};

		foreach (UnitCategory category in metadata.UnitCategories)
		{
			foreach (UnitDefinition unit in category.Units)
			{
				sourceFileTemplate.Classes.Add(new()
				{
					Comments =
					[
						$"/// <summary>",
						$"/// {unit.Description}",
						"/// </summary>",
					],
					Keywords =
					[
						"public",
						"record",
						"struct",
					],
					Name = unit.Name,
					Interfaces =
					[
						"IUnit",
					],
					Members =
					[
						new ConstructorTemplate()
						{
							Comments =
							[
								"/// <summary>Initializes a new instance of the unit.</summary>",
							],
							Keywords =
							[
								"public",
							],
							Name = unit.Name,
						},
						new FieldTemplate()
						{
							Comments =
							[
								"/// <summary>Gets the full name of the unit.</summary>",
							],
							Keywords =
							[
								"public",
								"readonly",
							],
							Type = "string",
							Name = "Name",
							DefaultValue = unit.Name,
							DefaultValueIsQuoted = true,
						},
						new FieldTemplate()
						{
							Comments =
							[
								"/// <summary>Gets the symbol/abbreviation of the unit.</summary>",
							],
							Keywords =
							[
								"public",
								"readonly",
							],
							Type = "string",
							Name = "Symbol",
							DefaultValue = unit.Symbol,
							DefaultValueIsQuoted = true,
						},
						new FieldTemplate()
						{
							Comments =
							[
								"/// <summary>Gets the unit system this unit belongs to.</summary>",
							],
							Keywords =
							[
								"public",
								"readonly",
							],
							Type = "UnitSystem",
							Name = "System",
							DefaultValue = $"UnitSystem.{unit.System}",
						},
						new FieldTemplate()
						{
							Comments =
							[
								"/// <summary>Gets the multiplication factor to convert to the base unit.</summary>",
							],
							Keywords =
							[
								"public",
								"readonly",
							],
							Type = "double",
							Name = "ToBaseFactor",
							DefaultValue = unit.ConversionFactor,
						},
						new FieldTemplate()
						{
							Comments =
							[
								"/// <summary>Gets the offset to add when converting to the base unit (0.0 for linear units).</summary>",
							],
							Keywords =
							[
								"public",
								"readonly",
							],
							Type = "double",
							Name = "ToBaseOffset",
							DefaultValue = unit.Offset,
						},
					]
				});
			}
		}

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);

		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
