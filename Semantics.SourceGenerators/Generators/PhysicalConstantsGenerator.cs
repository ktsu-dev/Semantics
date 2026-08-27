// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.CodeGen;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the PhysicalConstants.cs file from JSON metadata.
/// </summary>
/// <remarks>
/// Every constant is emitted as a generic accessor that materialises the metadata literal directly
/// into <c>T</c> via <c>T.Parse</c>. Parsing the literal once per closed generic type — rather than
/// storing it in an intermediate arbitrary-precision type and converting per call — keeps the
/// quantities package dependency-free and is also the more accurate route: an intermediate
/// significand/exponent representation rounds twice and loses the tail of the long CODATA literals.
/// </remarks>
[Generator]
public class PhysicalConstantsGenerator : SemanticsGenerator<DomainsMetadata>
{
	/// <summary>
	/// Name of the private nested holder that caches the parsed value of each constant per closed
	/// generic type. A static field on a generic type is initialised once per <c>T</c>, so callers
	/// pay the parse exactly once regardless of how hot the accessor is.
	/// </summary>
	private const string HolderName = "Values";

	/// <summary>
	/// Constraint carried by every generic accessor and by the holder that caches its value.
	/// </summary>
	private const string NumericConstraint = "where T : struct, INumber<T>";

	/// <summary>
	/// Styles accepted when parsing a constant literal. <see cref="System.Globalization.NumberStyles.Float"/>
	/// covers the exponent forms used in the metadata (for example <c>6.62607015e-34</c>); the default
	/// <c>Parse(string, IFormatProvider)</c> overload resolves to <c>NumberStyles.Number</c> for some
	/// numeric types, which rejects an exponent, so the style is always passed explicitly.
	/// </summary>
	private const string ParseStyles = "NumberStyles.Float";

	public PhysicalConstantsGenerator() : base("domains.json") { }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Description lowercasing for XML docs")]
	protected override void Generate(SourceProductionContext context, DomainsMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.Domains == null || metadata.Domains.Count == 0)
		{
			return;
		}

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "PhysicalConstants.g.cs",
			Namespace = "ktsu.Semantics.Quantities",
			Usings =
			[
				"System.Globalization",
				"System.Numerics",
			],
		};

		ClassTemplate constantsClass = new()
		{
			Comments =
			[
				Emit.SummaryOpen,
				"/// Provides fundamental physical constants used throughout the Semantics library.",
				"/// All values are based on the 2019 redefinition of SI base units and CODATA 2018 values.",
				Emit.SummaryClose,
			],
			Keywords = [Emit.Public, Emit.Static, "class"],
			Name = "PhysicalConstants",
		};

		// Generate nested class per domain
		foreach (Domain domain in metadata.Domains.OrderBy(d => d.Name))
		{
			if (domain.Constants == null || domain.Constants.Count == 0)
			{
				continue;
			}

			ClassTemplate domainClass = new()
			{
				Comments =
				[
					Emit.SummaryOpen,
					$"/// {domain.Description}",
					Emit.SummaryClose,
				],
				Keywords = [Emit.Public, Emit.Static, "class"],
				Name = domain.Name,
			};

			// The holder caches one parsed value per constant per closed generic type.
			ClassTemplate holderClass = new()
			{
				Comments =
				[
					Emit.SummaryOpen,
					$"/// Caches the {domain.Name} constants materialised into <typeparamref name=\"T\"/>.",
					Emit.SummaryClose,
				],
				Keywords = ["private", Emit.Static, "class"],
				Name = $"{HolderName}<T>",
				Constraints = [NumericConstraint],
			};

			foreach (ConstantDefinition constant in domain.Constants.OrderBy(c => c.Name))
			{
				domainClass.Members.Add(new MethodTemplate()
				{
					Comments = [$"/// <summary>{constant.Description}</summary>"],
					Keywords = [Emit.Public, Emit.Static, "T"],
					Name = $"{constant.Name}<T>",
					BodyFactory = (body) => body.Write($" {NumericConstraint} => {HolderName}<T>.{constant.Name};"),
				});

				holderClass.Members.Add(new FieldTemplate()
				{
					Comments = [$"/// <summary>{constant.Description}</summary>"],
					Keywords = ["internal", Emit.Static, "readonly", "T"],
					Name = constant.Name,
					DefaultValue = $"T.Parse(\"{constant.Value}\", {ParseStyles}, CultureInfo.InvariantCulture)",
				});
			}

			domainClass.NestedClasses.Add(holderClass);
			constantsClass.NestedClasses.Add(domainClass);
		}

		// Collect all constants for the Generic helper class
		List<ConstantDefinition> allConstants = [.. metadata.Domains
			.Where(d => d.Constants != null && d.Constants.Count > 0)
			.SelectMany(d => d.Constants)];

		if (allConstants.Count != 0)
		{
			ClassTemplate genericClass = new()
			{
				Comments =
				[
					Emit.SummaryOpen,
					"/// Helper methods to get constants as generic numeric types.",
					Emit.SummaryClose,
				],
				Keywords = [Emit.Public, Emit.Static, "class"],
				Name = "Generic",
			};

			foreach (ConstantDefinition constant in allConstants.OrderBy(c => c.Name))
			{
				// Find which domain this constant belongs to
				string domainName = metadata.Domains
					.First(d => d.Constants != null && d.Constants.Any(c => c.Name == constant.Name))
					.Name;

				genericClass.Members.Add(new MethodTemplate()
				{
					Comments = [$"/// <summary>Gets {constant.Description.ToLowerInvariant()} as type T.</summary>"],
					Keywords = [Emit.Public, Emit.Static, "T"],
					Name = $"{constant.Name}<T>",
					BodyFactory = (body) => body.Write($" {NumericConstraint} => {domainName}.{constant.Name}<T>();"),
				});
			}

			constantsClass.NestedClasses.Add(genericClass);
		}

		sourceFileTemplate.Classes.Add(constantsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		GeneratedSource.Add(context, sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
