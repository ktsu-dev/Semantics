// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using ktsu.SourceGeneratorToolkit;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

/// <summary>
/// Source generator that creates the ConversionConstants.cs file from JSON metadata.
/// </summary>
/// <remarks>
/// <para>
/// Each factor is emitted twice. The <see langword="double"/> constant backs the public
/// <c>IUnit.ToBaseFactor</c> and <c>ToBaseOffset</c> properties. The nested <c>Values&lt;T&gt;</c>
/// holder materialises the same literal into each storage type once, the way
/// <see cref="PhysicalConstantsGenerator"/> does for constants, and that is what the generated
/// factories and <c>In(unit)</c> read. Before the holder existed they reached every storage type
/// through <c>T.CreateChecked(double)</c>, so a <see cref="decimal"/> quantity converted with 15
/// significant digits of a 17-digit literal.
/// </para>
/// <para>
/// Each factor in the holder is a property over a nullable parsed value. A storage type that does not
/// parse it, such as an integer, converts the <see langword="double"/> at each read, so a factor too
/// large for the type throws <see cref="System.OverflowException"/> from the factory that uses it and
/// leaves the others working, as before 5.2.0. Converting in the static initializer instead made one
/// overflow, <c>CurieToBecquerels</c> into <see cref="int"/>, fail every factor for the type.
/// </para>
/// <para>
/// A value is a decimal literal or an exact fraction of two, <c>"5/9"</c>, that a <see langword="double"/>
/// can hold. Anything else is reported as SEM009 and generates no constant.
/// </para>
/// </remarks>
[Generator]
public class ConversionsGenerator : SemanticsGenerator<ConversionsMetadata>
{
	/// <summary>
	/// Prefix of the private field holding each factor parsed into the storage type.
	/// </summary>
	private const string ParsedPrefix = "Parsed";

	/// <summary>
	/// Name of the nested holder that caches each factor materialised into a storage type.
	/// </summary>
	private const string HolderName = "Values";

	/// <summary>
	/// Constraint carried by the holder, matching the one on every generated quantity.
	/// </summary>
	private const string NumericConstraint = "where T : struct, INumber<T>";

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
			Usings =
			{
				"System.Numerics",
			},
		};

		ClassTemplate constantsClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Conversion constants used by generated unit definitions.",
				"/// Values sourced from conversions.json metadata.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords =
			{
				Emit.Internal,
				Emit.Static,
			},
			Name = "ConversionConstants",
		};

		ClassTemplate holderClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Caches each conversion constant materialised into <typeparamref name=\"T\"/> at that type's own precision.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords =
			{
				Emit.Internal,
				Emit.Static,
			},
			Name = $"{HolderName}<T>",
			Constraints = {NumericConstraint},
		};

		foreach (ConversionCategory category in metadata.Conversions)
		{
			foreach (ConversionFactor factor in category.Factors)
			{
				ConversionValue? value = ConversionValue.Parse(factor.Value);
				if (value is null)
				{
					context.Report(SemanticsDiagnostics.InvalidConversionFactor, factor.Name, factor.Value);
					continue;
				}

				constantsClass.Members.Add(new FieldTemplate()
				{
					Comments =
					{
						$"/// <summary>{factor.Description}</summary>",
					},
					Keywords =
					{
						Emit.Internal,
						"const",
						"double",
					},
					Name = factor.Name,
					DefaultValue = value.DoubleExpression,
				});

				// Qualified, because inside the holder the bare name is the property being declared.
				holderClass.Members.Add(new FieldTemplate()
				{
					Comments =
					{
						$"/// <summary>{factor.Description}</summary>",
					},
					Keywords =
					{
						Emit.Internal,
						Emit.Static,
						"T",
					},
					Name = $"{factor.Name} => {ParsedPrefix}{factor.Name} ?? T.CreateChecked(ConversionConstants.{factor.Name})",
				});

				holderClass.Members.Add(new FieldTemplate()
				{
					Comments =
					{
						$"/// <summary>{factor.Name} parsed into <typeparamref name=\"T\"/>, or <see langword=\"null\"/> when the <see langword=\"double\"/> is converted at each read.</summary>",
					},
					Keywords =
					{
						"private",
						Emit.Static,
						"readonly",
						"T?",
					},
					Name = $"{ParsedPrefix}{factor.Name}",
					DefaultValue = value.StorageExpression,
				});
			}
		}

		if (holderClass.Members.Count > 0)
		{
			constantsClass.NestedClasses.Add(holderClass);
		}

		sourceFileTemplate.Classes.Add(constantsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);

		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
