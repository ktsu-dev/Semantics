// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;

using ktsu.CodeBlocker;

internal class FieldTemplate : MemberTemplate
{
	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);

		if (!string.IsNullOrEmpty(DefaultValue))
		{
			codeBlocker.Write(" = ");
			if (DefaultValueIsQuoted)
			{
				codeBlocker.Write($"\"{DefaultValue}\"");
			}
			else
			{
				codeBlocker.Write(DefaultValue);
			}
		}
		codeBlocker.WriteLine(";");
	}
}
