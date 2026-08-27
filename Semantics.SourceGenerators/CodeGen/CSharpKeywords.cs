// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.CodeGen;

/// <summary>
/// C# vocabulary a generator emits, named so a typo in a keyword is a compile error rather than
/// malformed generated source.
/// </summary>
/// <remarks>
/// Nothing here is specific to any one generator; it is part of the reusable layer. The XML
/// documentation delimiters that used to live alongside these are gone: documentation is written
/// through the template model's <c>DocComment</c>, which owns the tags and escapes their content.
/// </remarks>
internal static class CSharpKeywords
{
	/// <summary>The <c>public</c> modifier.</summary>
	internal const string Public = "public";

	/// <summary>The <c>static</c> modifier.</summary>
	internal const string Static = "static";
}
