// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the PhysicalConstants.cs file from JSON metadata.
/// </summary>
[Generator]
public class PhysicalConstantsGenerator : GeneratorBase<DomainsMetadata>
{
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
				"ktsu.PreciseNumber",
			],
		};

		ClassTemplate constantsClass = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Provides fundamental physical constants used throughout the Semantics library.",
				"/// All values are based on the 2019 redefinition of SI base units and CODATA 2018 values.",
				"/// </summary>",
			],
			Keywords = ["public", "static", "class"],
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
					"/// <summary>",
					$"/// {domain.Description}",
					"/// </summary>",
				],
				Keywords = ["public", "static", "class"],
				Name = domain.Name,
			};

			foreach (ConstantDefinition constant in domain.Constants.OrderBy(c => c.Name))
			{
				domainClass.Members.Add(new FieldTemplate()
				{
					Comments = [$"/// <summary>{constant.Description}</summary>"],
					Keywords = ["public", "static", "readonly", "PreciseNumber"],
					Name = constant.Name,
					DefaultValue = $"PreciseNumber.Parse(\"{constant.Value}\", CultureInfo.InvariantCulture)",
				});
			}

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
					"/// <summary>",
					"/// Helper methods to get constants as generic numeric types.",
					"/// </summary>",
				],
				Keywords = ["public", "static", "class"],
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
					Keywords = ["public", "static", "T"],
					Name = $"{constant.Name}<T>",
					BodyFactory = (body) =>
					{
						body.Write($" where T : struct, INumber<T> => T.CreateChecked({domainName}.{constant.Name});");
					},
				});
			}

			constantsClass.NestedClasses.Add(genericClass);
		}

		sourceFileTemplate.Classes.Add(constantsClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
