// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;

using System;
using ktsu.CodeBlocker;

internal class PropertyTemplate : MemberTemplate
{
	public static Action<CodeBlocker> AutoGet = (sw) => sw.Write("get;");
	public static Action<CodeBlocker> AutoSet = (sw) => sw.Write("set;");
	public static Action<CodeBlocker> AutoInit = (sw) => sw.Write("init;");

	public Action<CodeBlocker>? GetterFactory { get; set; }
	public Action<CodeBlocker>? SetterFactory { get; set; }
	public string? SetterBody { get; set; }
	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);

		if (GetterFactory is null && SetterFactory is null)
		{
			// If both are null, we assume it's an abstract property and terminate with a semicolon.
			codeBlocker.WriteLine(";");
		}
		else if ((GetterFactory == AutoGet || GetterFactory is null) && (SetterFactory is null || SetterFactory == AutoSet || SetterFactory == AutoInit))
		{
			// both properties are auto or null, we can use the shorthand syntax.
			codeBlocker.Write(" { ");
			GetterFactory?.Invoke(codeBlocker);
			if (GetterFactory is not null && SetterFactory is not null)
			{
				codeBlocker.Write(" ");
			}
			SetterFactory?.Invoke(codeBlocker);
			codeBlocker.WriteLine(" }");
			return;
		}
		else
		{
			// either one or both properties are custom, we need to write them out in full.
			codeBlocker.NewLine();
			codeBlocker.WriteLine("{");
			codeBlocker.NewLine();

			if (GetterFactory == AutoGet)
			{
				// If the getter is auto, we can use the shorthand syntax.
				GetterFactory(codeBlocker);
				codeBlocker.NewLine();
			}
			else if (GetterFactory is not null)
			{
				GetterFactory(codeBlocker);
			}

			if (SetterFactory == AutoSet || SetterFactory == AutoInit)
			{
				// If the setter is auto, we can use the shorthand syntax.
				SetterFactory(codeBlocker);
				codeBlocker.NewLine();
			}
			else if (SetterFactory is not null)
			{
				SetterFactory(codeBlocker);
			}

			codeBlocker.WriteLine("}");
		}
	}
}
