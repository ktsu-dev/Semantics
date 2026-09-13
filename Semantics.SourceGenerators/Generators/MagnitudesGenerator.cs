// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using ktsu.SourceGeneratorToolkit;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

/// <summary>
/// Source generator that creates the MetricMagnitudes.cs file from JSON metadata.
/// </summary>
/// <remarks>
/// The public <see langword="double"/> constants are unchanged. Alongside them an internal
/// <c>Values&lt;T&gt;</c> holder parses each power of ten into each storage type once, which is what
/// the generated factories multiply by. A <see cref="double"/> such as <c>1e-2</c> is not exactly a
/// hundredth, and converting it gave a <see cref="decimal"/> quantity its rounding.
/// </remarks>
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
			Usings =
			{
				"System.Numerics",
			},
		};

		ClassTemplate magnitudesClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Metric magnitude constants for unit scaling.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
			Name = "MetricMagnitudes",
		};

		ClassTemplate holderClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Caches each magnitude materialised into <typeparamref name=\"T\"/> at that type's own precision.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords = {"internal", Emit.Static},
			Name = "Values<T>",
			Constraints = {"where T : struct, INumber<T>"},
		};

		foreach (MagnitudeDefinition magnitude in metadata.Magnitudes)
		{
			string valueString = magnitude.Exponent switch
			{
				0 => "1.0",
				_ => $"1e{magnitude.Exponent}",
			};

			string comment = $"/// <summary>{magnitude.Name} magnitude ({magnitude.Symbol}): 10^{magnitude.Exponent}</summary>";

			magnitudesClass.Members.Add(new FieldTemplate()
			{
				Comments = {comment},
				Keywords = {Emit.Public, "const", "double"},
				Name = magnitude.Name,
				DefaultValue = valueString,
			});

			// Qualified, because inside the holder the bare name is the field being declared.
			holderClass.Members.Add(new FieldTemplate()
			{
				Comments = {comment},
				Keywords = {"internal", Emit.Static, "readonly", "T"},
				Name = magnitude.Name,
				DefaultValue = $"StorageLiteral.Parse<T>(\"{valueString}\", MetricMagnitudes.{magnitude.Name})",
			});
		}

		magnitudesClass.NestedClasses.Add(holderClass);
		sourceFileTemplate.Classes.Add(magnitudesClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
