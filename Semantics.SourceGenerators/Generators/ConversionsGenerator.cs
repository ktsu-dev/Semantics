// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.Models;

[Generator]
public sealed class ConversionsGenerator : GeneratorBase<ConversionsMetadata>
{
	public ConversionsGenerator() : base("conversions.json") { }

	protected override void Generate(SourceProductionContext context, ConversionsMetadata metadata, CodeBlocker codeBlocker)
	{
        if (metadata.Conversions.Count == 0)
        {
            return;
        }
        var code = codeBlocker
            .AddNamespace("Semantics.Conversions")
            .AddUsing("System")
            .AddUsing("System.Diagnostics.CodeAnalysis")
            .AddUsing("Semantics.Conversions")
            .AddClass("Conversions", "public static partial class Conversions")
            .BeginClass()
            .AddSummary("Provides conversion methods for various types.")
            ;
        foreach (var conversion in metadata.Conversions)
        {
            code.AddMethod(conversion);
        }
        code.EndClass();
        context.AddSource("Conversions.g.cs", code.ToString());
    }
}
