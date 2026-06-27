// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;

using ktsu.CodeBlocker;

internal abstract class MemberTemplate : TemplateBase
{
	public override void WriteTo(CodeBlocker codeBlocker)
	{
		base.WriteTo(codeBlocker);
		string typeAndName = $"{Type} {Name}".Trim();
		codeBlocker.Write(typeAndName);
	}

	internal static int MemberSortOrder(MemberTemplate memberTemplate)
	{
		return memberTemplate switch
		{
			FieldTemplate => 0,
			PropertyTemplate => 1,
			MethodTemplate => 2,
			_ => 3
		};
	}
}
