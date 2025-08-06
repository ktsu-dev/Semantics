// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;
using System.Collections.Generic;
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

	public virtual void WriteTo(CodeBlocker codeBlocker)
	{
		WriteCommentsTo(codeBlocker);
		WriteAttributesTo(codeBlocker);
		WriteKeywordsTo(codeBlocker);
	}

	private void WriteCommentsTo(CodeBlocker codeBlocker)
	{
		foreach (string comment in Comments)
		{
			codeBlocker.WriteLine(comment);
		}
	}

	private void WriteAttributesTo(CodeBlocker codeBlocker)
	{
		foreach (string attribute in Attributes)
		{
			codeBlocker.Write($"[{attribute}] ");
		}
	}

	private void WriteKeywordsTo(CodeBlocker codeBlocker)
	{
		if (Keywords.Count > 0)
		{
			codeBlocker.Write(string.Join(" ", Keywords) + " ");
		}
	}
}
