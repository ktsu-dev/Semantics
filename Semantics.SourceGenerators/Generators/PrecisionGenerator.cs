// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;
using Semantics.SourceGenerators.Templates;

/// <summary>
/// Source generator that creates the StorageTypes.cs file from JSON metadata.
/// </summary>
[Generator]
public class PrecisionGenerator : GeneratorBase<PrecisionMetadata>
{
	public PrecisionGenerator() : base("precision.json") { }

	protected override void Generate(SourceProductionContext context, PrecisionMetadata metadata, CodeBlocker codeBlocker)
	{
		if (metadata.StorageTypes == null || metadata.StorageTypes.Count == 0)
		{
			return;
		}

		SourceFileTemplate sourceFileTemplate = new()
		{
			FileName = "StorageTypes.g.cs",
			Namespace = "ktsu.Semantics",
			Usings =
			[
				"System",
				"System.Collections.Generic",
			],
		};

		ClassTemplate storageClass = new()
		{
			Comments =
			[
				"/// <summary>",
				"/// Available storage types for numeric values in the Semantics library.",
				"/// </summary>",
			],
			Keywords = ["public", "static", "class"],
			Name = "StorageTypes",
		};

		// Generate Type fields for each storage type
		foreach (string storageType in metadata.StorageTypes.OrderBy(t => t))
		{
			storageClass.Members.Add(new FieldTemplate()
			{
				Comments = [$"/// <summary>The {storageType} storage type.</summary>"],
				Keywords = ["public", "static", "readonly", "Type"],
				Name = storageType.ToUpperInvariant(),
				DefaultValue = $"typeof({storageType})",
			});
		}

		// Generate All property as a field
		string allTypes = string.Join(", ", metadata.StorageTypes.OrderBy(t => t).Select(t => t.ToUpperInvariant()));
		storageClass.Members.Add(new FieldTemplate()
		{
			Comments = ["/// <summary>Gets all available storage types.</summary>"],
			Keywords = ["public", "static", "readonly", "IReadOnlyList<Type>"],
			Name = "All",
			DefaultValue = $"new List<Type> {{ {allTypes} }}",
		});

		// Generate Names property as a field
		string allNames = string.Join(", ", metadata.StorageTypes.OrderBy(t => t).Select(t => $"\"{t}\""));
		storageClass.Members.Add(new FieldTemplate()
		{
			Comments = ["/// <summary>Gets the names of all available storage types.</summary>"],
			Keywords = ["public", "static", "readonly", "IReadOnlyList<string>"],
			Name = "Names",
			DefaultValue = $"new List<string> {{ {allNames} }}",
		});

		sourceFileTemplate.Classes.Add(storageClass);

		WriteSourceFileTo(codeBlocker, sourceFileTemplate);
		context.AddSource(sourceFileTemplate.FileName, codeBlocker.ToString());
	}
}
