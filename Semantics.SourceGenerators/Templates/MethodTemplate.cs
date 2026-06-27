// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;

using System;
using System.Collections.Generic;
using ktsu.CodeBlocker;

internal class MethodTemplate : MemberTemplate
{
	public List<ParameterTemplate> Parameters { get; set; } = [];
	public Action<CodeBlocker>? BodyFactory { get; set; }
	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);
		WriteParametersTo(codeBlocker);
		WriteBodyTo(codeBlocker);
	}

	private void WriteParametersTo(CodeBlocker codeBlocker)
	{
		List<string> parameterStrings = [];
		foreach (ParameterTemplate parameterTemplate in Parameters)
		{
			CodeBlocker parameterStringWriter = CodeBlocker.Create();
			parameterTemplate.WriteTo(parameterStringWriter);
			parameterStrings.Add(parameterStringWriter.ToString());
		}

		codeBlocker.Write("(");
		codeBlocker.Write(string.Join(", ", parameterStrings));
		codeBlocker.Write(")");
	}

	private void WriteBodyTo(CodeBlocker codeBlocker)
	{
		// If there is no body, we just append a semicolon. Abstract methods for example.
		if (BodyFactory is null)
		{
			codeBlocker.Write(";");
			return;
		}

		CodeBlocker bodyStringWriter = CodeBlocker.Create();
		BodyFactory(bodyStringWriter);
		string bodyString = bodyStringWriter.ToString();
		string[] bodyLines = bodyString.Split([Environment.NewLine], StringSplitOptions.None);
		int bodyLineCount = bodyLines.Length;

		// If the body is empty, we just append an empty block. Virtual base methods for example.
		if (bodyLineCount == 0)
		{
			codeBlocker.WriteLine(" { }");
			return;
		}

		if (bodyLineCount > 1)
		{
			// If the body has multiple lines, we need to write a new line before the opening brace.
			codeBlocker.NewLine();
		}

		// BodyFactory should provide the braces, or the expression body
		codeBlocker.Write(bodyString);
	}
}
