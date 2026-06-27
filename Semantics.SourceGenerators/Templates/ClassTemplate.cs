// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Templates;

using System.Collections.Generic;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class ClassTemplate : TemplateBase
{

	public string BaseClass { get; set; } = string.Empty;
	public List<string> Interfaces { get; set; } = [];
	public List<string> Constraints { get; set; } = [];
	public List<MemberTemplate> Members { get; set; } = [];
	public List<ClassTemplate> NestedClasses { get; set; } = [];

	public override void WriteTo(CodeBlocker codeBlocker)
	{
		// specify class/struct/ record etc as keywords
		base.WriteTo(codeBlocker);

		codeBlocker.Write($"{Name}");

		WriteBaseClassAndInterfacesTo(codeBlocker);
		WriteConstraintsTo(codeBlocker);
		WriteMembersTo(codeBlocker);
	}

	private void WriteBaseClassAndInterfacesTo(CodeBlocker codeBlocker)
	{
		bool hasBaseClass = !string.IsNullOrEmpty(BaseClass);
		bool hasInterfaces = Interfaces.Count > 0;
		bool hasBaseClassOrInterfaces = hasBaseClass || hasInterfaces;
		if (hasBaseClassOrInterfaces)
		{
			codeBlocker.Write($" : ");
		}

		List<string> baseAndInterfaces = [];
		if (hasBaseClass)
		{
			baseAndInterfaces.Add(BaseClass);
		}

		baseAndInterfaces.AddRange(Interfaces);

		codeBlocker.Write(string.Join(", ", baseAndInterfaces));

		if (hasBaseClassOrInterfaces)
		{
			codeBlocker.NewLine();
		}
	}

	public void WriteConstraintsTo(CodeBlocker codeBlocker)
	{
		foreach (string constraint in Constraints)
		{
			codeBlocker.WriteLine($"\t{constraint}");
		}
	}

	public void WriteMembersTo(CodeBlocker codeBlocker)
	{
		if (Members.Count == 0 && NestedClasses.Count == 0)
		{
			codeBlocker.WriteLine("{ }");
			return;
		}

		IEnumerable<MemberTemplate> sortedMembers = Members.OrderBy(MemberTemplate.MemberSortOrder);

		using (new ScopeWithTrailingSemicolon(codeBlocker))
		{
			foreach (MemberTemplate member in sortedMembers)
			{
				member.WriteTo(codeBlocker);
				codeBlocker.NewLine();
			}

			foreach (ClassTemplate nestedClass in NestedClasses)
			{
				codeBlocker.NewLine();
				nestedClass.WriteTo(codeBlocker);
			}
		}
	}
}

internal static class ClassTemplateExtensions
{
	public static CodeBlocker AddClass(this CodeBlocker codeBlocker, ClassTemplate classTemplate)
	{
		classTemplate.WriteTo(codeBlocker);

		return codeBlocker;
	}

	public static CodeBlocker AddClassName(this CodeBlocker codeBlocker, ClassTemplate classTemplate)
	{
		codeBlocker.Write(classTemplate.Name);
		return codeBlocker;
	}

	public static CodeBlocker AddInheritance(this CodeBlocker codeBlocker, ClassTemplate classTemplate)
	{
		bool hasBaseClass = !string.IsNullOrEmpty(classTemplate.BaseClass);
		bool hasInterfaces = classTemplate.Interfaces.Count > 0;
		bool hasBaseClassOrInterfaces = hasBaseClass || hasInterfaces;

		if (hasBaseClassOrInterfaces)
		{
			codeBlocker.Write($" : ");
		}

		List<string> baseAndInterfaces = [];
		if (hasBaseClass)
		{
			baseAndInterfaces.Add(classTemplate.BaseClass);
		}

		baseAndInterfaces.AddRange(classTemplate.Interfaces);

		codeBlocker.Write(string.Join(", ", baseAndInterfaces));

		if (hasBaseClassOrInterfaces)
		{
			codeBlocker.NewLine();
		}

		if (string.IsNullOrEmpty(classTemplate.BaseClass) && classTemplate.Interfaces.Count == 0)
		{
			return codeBlocker;
		}
		codeBlocker.Write(" : ");
		if (!string.IsNullOrEmpty(classTemplate.BaseClass))
		{
			codeBlocker.Write(classTemplate.BaseClass);
		}
		if (classTemplate.Interfaces.Count > 0)
		{
			if (!string.IsNullOrEmpty(classTemplate.BaseClass))
			{
				codeBlocker.Write(", ");
			}
			codeBlocker.Write(string.Join(", ", classTemplate.Interfaces));
		}
		return codeBlocker;
	}
}
