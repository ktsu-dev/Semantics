// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;
using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;

internal abstract class TemplateBase
{
	public string Name { get; set; } = string.Empty;
	public string Type { get; set; } = string.Empty;
	public string DefaultValue { get; set; } = string.Empty;
	public bool DefaultValueIsQuoted { get; set; } = false;
	public List<string> Attributes { get; set; } = [];
	public List<string> Keywords { get; set; } = [];
	public List<string> Comments { get; set; } = [];
}

internal static class TemplateBaseExtensions
{
	public static CodeBlocker AddTemplate(this CodeBlocker codeBlocker, TemplateBase template)
	{
		codeBlocker.AddComments(template.Comments);
		codeBlocker.AddAttributes(template.Attributes);
		codeBlocker.AddKeywords(template.Keywords);
		return codeBlocker;
	}

	public static CodeBlocker AddComments(this CodeBlocker codeBlocker, IEnumerable<string> comments)
	{
		foreach (string comment in comments)
		{
			codeBlocker.WriteLine(comment);
		}
		return codeBlocker;
	}

	public static CodeBlocker AddAttributes(this CodeBlocker codeBlocker, IEnumerable<string> attributes)
	{
		foreach (string attribute in attributes)
		{
			codeBlocker.Write($"[{attribute}] ");
		}
		return codeBlocker;
	}

	public static CodeBlocker AddKeywords(this CodeBlocker codeBlocker, IEnumerable<string> keywords)
	{
		if (keywords.Any())
		{
			codeBlocker.Write(string.Join(" ", keywords) + " ");
		}
		return codeBlocker;
	}
}
