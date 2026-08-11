// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.Templates;

using ktsu.CodeBlocker;

internal class ParameterTemplate : TemplateBase
{
	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);

		codeBlocker.Write($"{Type} {Name}");
		if (!string.IsNullOrEmpty(DefaultValue))
		{
			codeBlocker.Write($" = ");
			if (DefaultValueIsQuoted)
			{
				codeBlocker.Write($"\"{DefaultValue}\"");
			}
			else
			{
				codeBlocker.Write(DefaultValue);
			}
		}
	}
}
