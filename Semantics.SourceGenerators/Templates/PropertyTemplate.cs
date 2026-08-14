// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.Templates;

using System;
using ktsu.CodeBlocker;

internal class PropertyTemplate : MemberTemplate
{
	// Compared by reference in WriteTo to detect auto-property shorthand, so these must stay
	// single fixed instances; readonly enforces that without changing the comparison.
	public static readonly Action<CodeBlocker> AutoGet = (sw) => sw.Write("get;");
	public static readonly Action<CodeBlocker> AutoSet = (sw) => sw.Write("set;");
	public static readonly Action<CodeBlocker> AutoInit = (sw) => sw.Write("init;");

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
			return;
		}

		if (CanUseShorthand)
		{
			WriteShorthand(codeBlocker);
			return;
		}

		WriteFullAccessors(codeBlocker);
	}

	/// <summary>
	/// Gets a value indicating whether both accessors are auto (or absent), in which case the
	/// whole property fits on one line as <c>{ get; set; }</c>.
	/// </summary>
	private bool CanUseShorthand =>
		(GetterFactory == AutoGet || GetterFactory is null)
		&& (SetterFactory is null || SetterFactory == AutoSet || SetterFactory == AutoInit);

	private void WriteShorthand(CodeBlocker codeBlocker)
	{
		codeBlocker.Write(" { ");
		GetterFactory?.Invoke(codeBlocker);
		if (GetterFactory is not null && SetterFactory is not null)
		{
			codeBlocker.Write(" ");
		}

		SetterFactory?.Invoke(codeBlocker);
		codeBlocker.WriteLine(" }");
	}

	/// <summary>
	/// Writes a braced accessor list. Used when either accessor has a custom body.
	/// </summary>
	private void WriteFullAccessors(CodeBlocker codeBlocker)
	{
		codeBlocker.NewLine();
		codeBlocker.WriteLine("{");
		codeBlocker.NewLine();

		WriteAccessor(codeBlocker, GetterFactory, GetterFactory == AutoGet);
		WriteAccessor(codeBlocker, SetterFactory, SetterFactory == AutoSet || SetterFactory == AutoInit);

		codeBlocker.WriteLine("}");
	}

	private static void WriteAccessor(CodeBlocker codeBlocker, Action<CodeBlocker>? factory, bool isAuto)
	{
		if (factory is null)
		{
			return;
		}

		factory(codeBlocker);
		if (isAuto)
		{
			// An auto accessor writes only "get;"/"set;", so it needs its own line break.
			codeBlocker.NewLine();
		}
	}
}
