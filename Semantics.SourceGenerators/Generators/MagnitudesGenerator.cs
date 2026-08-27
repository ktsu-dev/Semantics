// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.CodeGen;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the MetricMagnitudes.cs file from JSON metadata.
/// </summary>
[Generator]
public class MagnitudesGenerator : SemanticsGenerator<MagnitudesMetadata>
{
	public MagnitudesGenerator() : base("magnitudes.json") { }

	protected override void Generate(SourceProductionContext context, MagnitudesMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.Magnitudes.Count == 0)
		{
			return;
		}

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "MetricMagnitudes.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
		};

		ClassTemplate magnitudesClass = new()
		{
			Comments =
			[
				Emit.SummaryOpen,
				"/// Metric magnitude constants for unit scaling.",
				Emit.SummaryClose,
			],
			Keywords = [Emit.Public, Emit.Static, "class"],
			Name = "MetricMagnitudes",
		};

		foreach (MagnitudeDefinition magnitude in metadata.Magnitudes)
		{
			string valueString = magnitude.Exponent switch
			{
				0 => "1.0",
				_ => $"1e{magnitude.Exponent}",
			};

			magnitudesClass.Members.Add(new FieldTemplate()
			{
				Comments = [$"/// <summary>{magnitude.Name} magnitude ({magnitude.Symbol}): 10^{magnitude.Exponent}</summary>"],
				Keywords = [Emit.Public, "const", "double"],
				Name = magnitude.Name,
				DefaultValue = valueString,
			});
		}

		sourceFileTemplate.Classes.Add(magnitudesClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		GeneratedSource.Add(context, sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
