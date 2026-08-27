// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.CodeGen;
using ktsu.CodeBlocker.Templates;

/// <summary>
/// The settings every generator in this repository shares, in one place.
/// </summary>
internal static class SemanticsGeneratorSettings
{
	/// <summary>
	/// The copyright line written above the generated-file marker.
	/// </summary>
	/// <remarks>
	/// Must match <c>file_header_template</c> in <c>.editorconfig</c>, which ktsu.Sdk syncs from
	/// <c>COPYRIGHT.md</c> on every build. Generated output is committed, so a drift shows up as a
	/// diff rather than a build error — <c>SourceGeneratorTests</c> asserts the emitted header to
	/// turn that into a failing test instead.
	/// </remarks>
	internal const string Copyright = "Copyright (c) 2023-2026 ktsu-dev contributors";
}

/// <summary>
/// Base class for a generator in this repository driven by one metadata file.
/// </summary>
/// <typeparam name="T">The shape the metadata file deserializes into.</typeparam>
/// <param name="metadataFileName">The metadata file's name.</param>
public abstract class SemanticsGenerator<T>(string metadataFileName) : GeneratorBase<T>(metadataFileName)
	where T : class
{
	/// <inheritdoc/>
	protected sealed override DiagnosticCatalog Diagnostics => SemanticsDiagnostics.Catalog;

	/// <inheritdoc/>
	protected sealed override DiagnosticDescriptor MetadataFileMissing => SemanticsDiagnostics.MetadataFileMissing;

	/// <inheritdoc/>
	protected sealed override DiagnosticDescriptor MetadataParseFailed => SemanticsDiagnostics.MetadataParseFailed;


	/// <summary>
	/// Writes the header every generated file in this repository starts with.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	protected static void WriteHeaderTo(CodeBlocker codeBlocker) =>
		WriteFileHeader(codeBlocker, SemanticsGeneratorSettings.Copyright);

	/// <summary>
	/// Writes a whole source file, header included.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	/// <param name="sourceFileTemplate">The file to write.</param>
	protected static void WriteSourceFileTo(CodeBlocker codeBlocker, SourceFileTemplate sourceFileTemplate) =>
		WriteSourceFile(codeBlocker, sourceFileTemplate, SemanticsGeneratorSettings.Copyright);
}

/// <summary>
/// Base class for a generator in this repository driven by more than one metadata file.
/// </summary>
public abstract class SemanticsMultiFileGenerator : GeneratorBase
{
	/// <inheritdoc/>
	protected sealed override DiagnosticCatalog Diagnostics => SemanticsDiagnostics.Catalog;

	/// <inheritdoc/>
	protected sealed override DiagnosticDescriptor MetadataFileMissing => SemanticsDiagnostics.MetadataFileMissing;

	/// <inheritdoc/>
	protected sealed override DiagnosticDescriptor MetadataParseFailed => SemanticsDiagnostics.MetadataParseFailed;


	/// <summary>
	/// Writes the header every generated file in this repository starts with.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	protected static void WriteHeaderTo(CodeBlocker codeBlocker) =>
		WriteFileHeader(codeBlocker, SemanticsGeneratorSettings.Copyright);

	/// <summary>
	/// Writes a whole source file, header included.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	/// <param name="sourceFileTemplate">The file to write.</param>
	protected static void WriteSourceFileTo(CodeBlocker codeBlocker, SourceFileTemplate sourceFileTemplate) =>
		WriteSourceFile(codeBlocker, sourceFileTemplate, SemanticsGeneratorSettings.Copyright);
}
