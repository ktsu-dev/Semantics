// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators;

using System;
using Microsoft.CodeAnalysis;

/// <summary>
/// Helpers for emitting generated sources with host-independent content.
/// </summary>
internal static class GeneratedSource
{
	/// <summary>
	/// The line ending emitted for all generated sources.
	/// </summary>
	/// <remarks>
	/// CRLF matches <c>end_of_line = crlf</c> in <c>.editorconfig</c> and <c>*.cs text eol=crlf</c> in
	/// <c>.gitattributes</c>.
	/// </remarks>
	private const string LineEnding = "\r\n";

	/// <summary>
	/// Adds a generated source file, normalizing its line endings so the output does not depend on the
	/// operating system the generator runs on.
	/// </summary>
	/// <param name="context">The source production context to add the source to.</param>
	/// <param name="hintName">The name of the generated file.</param>
	/// <param name="source">The generated source text.</param>
	/// <remarks>
	/// <c>ktsu.CodeBlocker</c> writes through <see cref="System.CodeDom.Compiler.IndentedTextWriter"/>, which
	/// uses <see cref="Environment.NewLine"/> — so the same metadata produced CRLF output on Windows and LF
	/// output on Linux. Generator output under <c>Semantics.Quantities/Generated/</c> is committed to the
	/// repository and verified by CI, so it has to be byte-identical regardless of who builds it.
	/// </remarks>
	internal static void Add(SourceProductionContext context, string hintName, string source) =>
		context.AddSource(hintName, NormalizeLineEndings(source));

	/// <summary>
	/// Rewrites every line ending in the given text to <see cref="LineEnding"/>.
	/// </summary>
	/// <param name="source">The text to normalize.</param>
	/// <returns>The text with normalized line endings.</returns>
	internal static string NormalizeLineEndings(string source) =>
		source.Replace(LineEnding, "\n").Replace("\n", LineEnding);
}
