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
/// <para>
/// The public <see langword="double"/> constants are unchanged. Alongside them an internal
/// <c>Values&lt;T&gt;</c> holder parses each power of ten into each storage type once, which is what
/// the generated factories multiply by. A <see cref="double"/> such as <c>1e-2</c> is not exactly a
/// hundredth, and converting it gave a <see cref="decimal"/> quantity its rounding.
/// </para>
/// <para>
/// Each magnitude is a property over a nullable parsed value. A storage type that does not parse it,
/// such as an integer, converts the <see langword="double"/> at each read, so a magnitude too large
/// for the type throws <see cref="System.OverflowException"/> where it is used and leaves the others
/// working. Converting in the static initializer instead made one overflow, 10^12 into
/// <see cref="int"/>, fail every magnitude for the type.
/// </para>
/// </remarks>
[Generator]
public class MagnitudesGenerator : SemanticsGenerator<MagnitudesMetadata>
{
	/// <summary>
	/// Prefix of the private field holding each value parsed into the storage type.
	/// </summary>
	private const string ParsedPrefix = "Parsed";

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

			// Qualified, because inside the holder the bare name is the property being declared.
			holderClass.Members.Add(new FieldTemplate()
			{
				Comments = {comment},
				Keywords = {"internal", Emit.Static, "T"},
				Name = $"{magnitude.Name} => {ParsedPrefix}{magnitude.Name} ?? T.CreateChecked(MetricMagnitudes.{magnitude.Name})",
			});

			holderClass.Members.Add(new FieldTemplate()
			{
				Comments = {$"/// <summary>{magnitude.Name} parsed into <typeparamref name=\"T\"/>, or <see langword=\"null\"/> when the <see langword=\"double\"/> is converted at each read.</summary>"},
				Keywords = {"private", Emit.Static, "readonly", "T?"},
				Name = $"{ParsedPrefix}{magnitude.Name}",
				DefaultValue = $"StorageLiteral.Parse<T>(\"{valueString}\")",
			});
		}

		magnitudesClass.NestedClasses.Add(holderClass);
		sourceFileTemplate.Classes.Add(magnitudesClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
