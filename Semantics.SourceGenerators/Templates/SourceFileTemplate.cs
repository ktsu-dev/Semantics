// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;
using System.Collections.Generic;
using ktsu.CodeBlocker;

internal class SourceFileTemplate : TemplateBase
{
	public string FileName { get; set; } = string.Empty;
	public string Namespace { get; set; } = string.Empty;
	public List<string> Usings { get; set; } = [];
	public List<ClassTemplate> Classes { get; set; } = [];

	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);
		if (!string.IsNullOrEmpty(Namespace))
		{
			codeBlocker.WriteLine($"namespace {Namespace};");
			codeBlocker.NewLine();
		}
		foreach (string usingDirective in Usings)
		{
			codeBlocker.WriteLine($"using {usingDirective};");
		}

		if (Usings.Count > 0)
		{
			codeBlocker.NewLine();
		}

		foreach (ClassTemplate classTemplate in Classes)
		{
			classTemplate.WriteTo(codeBlocker);
			codeBlocker.NewLine();
		}
	}
}
