// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Semantics.SourceGenerators.Models;

[Generator]
public sealed class ConversionsGenerator : GeneratorBase<ConversionsMetadata>
{
	public ConversionsGenerator() : base("conversions.json") { }

	protected override string Generate(SourceProductionContext context, ConversionsMetadata metadata)
	{
		StringBuilder builder = new();

		AddHeader(builder);
		
		builder.AppendLine("namespace ktsu.Semantics;");
		builder.AppendLine();
		builder.AppendLine("using System.Globalization;");
		builder.AppendLine("using ktsu.PreciseNumber;");
		builder.AppendLine();
		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// Static registry of conversion factors used by the units system.");
		builder.AppendLine("/// </summary>");
		builder.AppendLine("public static class ConversionFactors");
		builder.AppendLine("{");

		// Generate each category
		foreach (ConversionCategory category in metadata.Conversions.OrderBy(c => c.Category))
		{
			if (category.Factors.Count != 0)
			{
				builder.AppendLine();
				builder.AppendLine($"\t// === {category.Category.ToUpperInvariant()} ===");
				builder.AppendLine();

				foreach (ConversionFactor factor in category.Factors.OrderBy(f => f.Name))
				{
					GenerateConversionFactor(builder, factor);
				}
			}
		}

		builder.AppendLine("}");
		return builder.ToString();
	}

	private static void GenerateConversionFactor(StringBuilder builder, ConversionFactor factor)
	{
		// Generate XML documentation
		builder.AppendLine($"\t/// <summary>{factor.Description}</summary>");

		// Parse the high-precision value and generate the property declaration
		builder.AppendLine($"\tpublic static readonly PreciseNumber {factor.Name} = PreciseNumber.Parse(\"{factor.Value}\", CultureInfo.InvariantCulture);");
		builder.AppendLine();
	}
}
