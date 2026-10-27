// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using ktsu.SourceGeneratorToolkit;
using ktsu.CodeBlocker.Templates;
using TypeKind = ktsu.CodeBlocker.Templates.TypeKind;

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
	/// generic type. A static field on a generic type exists once per <c>T</c>, so callers pay the
	/// parse once per constant regardless of how hot the accessor is.
	/// </summary>
	private const string HolderName = "Values";

	/// <summary>
	/// Constraint carried by every generic accessor and by the holder that caches its value.
	/// </summary>
	private const string NumericConstraint = "where T : struct, INumber<T>";

	/// <summary>
	/// Prefix of the private field caching each constant once it has been materialised. Each constant is
	/// materialised on its own, through <c>StorageLiteral.Constant</c>, when it is first read.
	/// </summary>
	private const string CachePrefix = "cached";

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
			{
				"System.Numerics",
			},
		};

		ClassTemplate constantsClass = new()
		{
			Comments =
			{
				Emit.SummaryOpen,
				"/// Provides fundamental physical constants used throughout the Semantics library.",
				"/// All values are based on the 2019 redefinition of SI base units and CODATA 2018 values.",
				Emit.SummaryClose,
			},
			Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
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
				{
					Emit.SummaryOpen,
					$"/// {domain.Description}",
					Emit.SummaryClose,
				},
				Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
				Name = domain.Name,
			};

			// The holder caches one parsed value per constant per closed generic type.
			ClassTemplate holderClass = new()
			{
				Comments =
				{
					Emit.SummaryOpen,
					$"/// Caches the {domain.Name} constants materialised into <typeparamref name=\"T\"/>.",
					Emit.SummaryClose,
				},
				Kind = TypeKind.Class,
			Keywords = {"private", Emit.Static},
				Name = $"{HolderName}<T>",
				Constraints = {NumericConstraint},
			};

			foreach (ConstantDefinition constant in domain.Constants.OrderBy(c => c.Name))
			{
				domainClass.Members.Add(new MethodTemplate()
				{
					Comments = {$"/// <summary>{constant.Description}</summary>"},
					Keywords = {Emit.Public, Emit.Static, "T"},
					Name = $"{constant.Name}<T>",
					BodyFactory = (body) => body.Write($" {NumericConstraint} => {HolderName}<T>.{constant.Name};"),
				});

				// One lazily filled field per constant, so a constant T cannot hold throws from its own accessor
				// rather than failing the shared static initializer for every constant of the domain.
				holderClass.Members.Add(new FieldTemplate()
				{
					Comments = {$"/// <summary>{constant.Description}</summary>"},
					Keywords = {Emit.Internal, Emit.Static, "T"},
					Name = $"{constant.Name} => {CachePrefix}{constant.Name} ??= StorageLiteral.Constant<T>(\"{constant.Value}\", \"{constant.Name}\")",
				});

				holderClass.Members.Add(new FieldTemplate()
				{
					Comments = {$"/// <summary>{constant.Name} once materialised into <typeparamref name=\"T\"/>.</summary>"},
					Keywords = {"private", Emit.Static, "T?"},
					Name = $"{CachePrefix}{constant.Name}",
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
				{
					Emit.SummaryOpen,
					"/// Helper methods to get constants as generic numeric types.",
					Emit.SummaryClose,
				},
				Kind = TypeKind.Class,
			Keywords = {Emit.Public, Emit.Static},
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
					Comments = {$"/// <summary>Gets {constant.Description.ToLowerInvariant()} as type T.</summary>"},
					Keywords = {Emit.Public, Emit.Static, "T"},
					Name = $"{constant.Name}<T>",
					BodyFactory = (body) => body.Write($" {NumericConstraint} => {domainName}.{constant.Name}<T>();"),
				});
			}

			constantsClass.NestedClasses.Add(genericClass);
		}

		sourceFileTemplate.Classes.Add(constantsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
