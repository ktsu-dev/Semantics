// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using ktsu.CodeBlocker.Templates;

/// <summary>
/// Helpers for filling a template's collections from a sequence.
/// </summary>
/// <remarks>
/// The template model exposes its collections as read-only properties, which is the right shape for
/// a public API — but it means a caller holding a prebuilt list cannot assign it in an object
/// initializer. These keep those call sites to one expression instead of forcing the template out
/// into a local and a loop.
/// </remarks>
internal static class TemplateExtensions
{
	/// <summary>
	/// Adds every comment in <paramref name="comments"/> to the template.
	/// </summary>
	/// <typeparam name="T">The template type.</typeparam>
	/// <param name="template">The template to add to.</param>
	/// <param name="comments">The comment lines, each written verbatim.</param>
	/// <returns>The same template, for chaining.</returns>
	internal static T WithComments<T>(this T template, IEnumerable<string> comments)
		where T : TemplateBase
	{
		foreach (string comment in comments)
		{
			template.Comments.Add(comment);
		}

		return template;
	}

	/// <summary>
	/// Adds every interface in <paramref name="interfaces"/> to the type.
	/// </summary>
	/// <param name="template">The type to add to.</param>
	/// <param name="interfaces">The interface names.</param>
	/// <returns>The same template, for chaining.</returns>
	internal static ClassTemplate WithInterfaces(this ClassTemplate template, IEnumerable<string> interfaces)
	{
		foreach (string name in interfaces)
		{
			template.Interfaces.Add(name);
		}

		return template;
	}
}
