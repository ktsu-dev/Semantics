// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the MetricMagnitudes.cs file from JSON metadata.
/// </summary>
[Generator]
public class MagnitudesGenerator : GeneratorBase<MagnitudesMetadata>
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
			Namespace = "ktsu.Semantics",
		};

		ClassTemplate magnitudesClass = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Metric magnitude constants for unit scaling.",
				"/// </summary>",
			],
			Keywords = ["public", "static", "class"],
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
				Keywords = ["public", "const", "double"],
				Name = magnitude.Name,
				DefaultValue = valueString,
			});
		}

		sourceFileTemplate.Classes.Add(magnitudesClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
