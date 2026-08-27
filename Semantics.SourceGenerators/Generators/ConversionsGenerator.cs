// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.CodeGen;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the ConversionConstants.cs file from JSON metadata.
/// </summary>
[Generator]
public class ConversionsGenerator : SemanticsGenerator<ConversionsMetadata>
{
	public ConversionsGenerator() : base("conversions.json") { }

	protected override void Generate(SourceProductionContext context, ConversionsMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.Conversions.Count == 0)
		{
			return;
		}

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "ConversionConstants.g.cs",
			Namespace = "ktsu.Semantics.Quantities.Units",
		};

		ClassTemplate constantsClass = new()
		{
			Comments =
			[
				Emit.SummaryOpen,
				"/// Conversion constants used by generated unit definitions.",
				"/// Values sourced from conversions.json metadata.",
				Emit.SummaryClose,
			],
			Keywords =
			[
				"internal",
				Emit.Static,
				"class",
			],
			Name = "ConversionConstants",
		};

		foreach (ConversionCategory category in metadata.Conversions)
		{
			foreach (ConversionFactor factor in category.Factors)
			{
				constantsClass.Members.Add(new FieldTemplate()
				{
					Comments =
					[
						$"/// <summary>{factor.Description}</summary>",
					],
					Keywords =
					[
						"internal",
						"const",
						"double",
					],
					Name = factor.Name,
					DefaultValue = factor.Value,
				});
			}
		}

		sourceFileTemplate.Classes.Add(constantsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);

		GeneratedSource.Add(context, sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
