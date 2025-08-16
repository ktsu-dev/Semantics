// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;
using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;

internal class SourceFileTemplate : TemplateBase
{
	public string FileName { get; set; } = string.Empty;
	public string Namespace { get; set; } = string.Empty;
	public List<string> Usings { get; set; } = [];
	public List<ClassTemplate> Classes { get; set; } = [];
}

internal static class SourceFileTemplateExtensions
{
	public static CodeBlocker AddSourceFile(this CodeBlocker codeBlocker, SourceFileTemplate template)
	{
		codeBlocker.AddTemplate(template);
		codeBlocker.AddNamespace(template.Namespace);
		codeBlocker.AddUsings(template.Usings);
		codeBlocker.AddClasses(template.Classes);
		return codeBlocker;
	}

	public static CodeBlocker AddNamespace(this CodeBlocker codeBlocker, string namespaceName)
	{
		if (!string.IsNullOrEmpty(namespaceName))
		{
			codeBlocker.WriteLine($"namespace {namespaceName};");
			codeBlocker.NewLine();
		}

		return codeBlocker;
	}

	public static CodeBlocker AddUsings(this CodeBlocker codeBlocker, IEnumerable<string> usings)
	{
		foreach (string usingDirective in usings)
		{
			codeBlocker.WriteLine($"using {usingDirective};");
		}

		if (usings.Any())
		{
			codeBlocker.NewLine();
		}

		return codeBlocker;
	}

	public static CodeBlocker AddClasses(this CodeBlocker codeBlocker, IEnumerable<ClassTemplate> classes)
	{
		foreach (ClassTemplate classTemplate in classes)
		{
			codeBlocker.AddClass(classTemplate);
			codeBlocker.NewLine();
		}

		return codeBlocker;
	}
}
