// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.CodeGen;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ktsu.CodeBlocker;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ktsu.CodeBlocker.Templates;

/// <summary>
/// Base class for a generator driven by one or more JSON metadata files supplied as
/// <c>AdditionalFiles</c>.
/// </summary>
/// <remarks>
/// Nothing here is specific to any one generator: it finds the declared metadata files, hands them
/// to <see cref="Generate"/>, and reports a diagnostic for anything missing or malformed. Declaring
/// more than one file is the normal case rather than a reason to reimplement
/// <see cref="Initialize"/>.
/// </remarks>
public abstract class GeneratorBase : IIncrementalGenerator
{
	/// <summary>
	/// Gets the names of the metadata files this generator reads, without their directories.
	/// </summary>
	protected abstract IReadOnlyList<string> MetadataFileNames { get; }

	/// <summary>
	/// Gets the catalogue this generator's diagnostics are allocated from.
	/// </summary>
	protected abstract DiagnosticCatalog Diagnostics { get; }

	/// <summary>
	/// Gets the descriptor reported when a declared metadata file is not in the compilation.
	/// </summary>
	protected abstract DiagnosticDescriptor MetadataFileMissing { get; }

	/// <summary>
	/// Gets the descriptor reported when a metadata file cannot be parsed. Its message format takes
	/// the file name and the reason.
	/// </summary>
	protected abstract DiagnosticDescriptor MetadataParseFailed { get; }

	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IReadOnlyList<string> wanted = MetadataFileNames;

		IncrementalValueProvider<ImmutableArray<MetadataFile>> metadataFiles = context.AdditionalTextsProvider
			.Where(file => wanted.Any(name => IsNamed(file.Path, name)))
			.Select((file, cancellationToken) =>
			{
				SourceText? sourceText = file.GetText(cancellationToken);
				return new MetadataFile(NameOf(file.Path), sourceText?.ToString() ?? string.Empty, sourceText, file.Path);
			})
			.Where(file => file.Text.Length > 0)
			.Collect();

		context.RegisterSourceOutput(metadataFiles, (productionContext, files) =>
		{
			// A duplicate name means the same metadata reached the compilation twice; the first
			// wins, which is what the old EndsWith-plus-FirstOrDefault matching did implicitly.
			Dictionary<string, MetadataFile> byName = files
				.GroupBy(file => file.FileName, StringComparer.Ordinal)
				.ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

			// Previously a missing file produced no output and no explanation.
			foreach (string name in wanted.Where(name => !byName.ContainsKey(name)))
			{
				productionContext.Report(MetadataFileMissing, name);
			}

			Generate(productionContext, new MetadataSet(byName));
		});
	}

	/// <summary>
	/// Emits this generator's sources.
	/// </summary>
	/// <param name="context">The source production context to add sources to.</param>
	/// <param name="metadata">The metadata files this generator declared.</param>
	protected abstract void Generate(SourceProductionContext context, MetadataSet metadata);

	/// <summary>
	/// Creates a <see cref="CodeBlocker"/> configured the way generated sources are written.
	/// </summary>
	/// <returns>A new <see cref="CodeBlocker"/>.</returns>
	/// <remarks>
	/// The line terminator is pinned rather than inherited from the host. Generated output is
	/// committed and verified by CI, so it has to be byte-identical wherever it was produced —
	/// which previously meant rewriting every line ending after the fact, because
	/// <see cref="System.CodeDom.Compiler.IndentedTextWriter"/> uses
	/// <see cref="System.Environment.NewLine"/>.
	/// <para>
	/// LF, matching <c>* text=auto eol=lf</c> in <c>.gitattributes</c> and
	/// <c>end_of_line = lf</c> in <c>.editorconfig</c>. The pass this replaces rewrote everything
	/// to CRLF on the grounds that those files asked for CRLF, which stopped being true at the LF
	/// migration; git had been quietly normalising the difference away on every commit ever since.
	/// </para>
	/// </remarks>
	protected static CodeBlocker CreateCodeBlocker() =>
		CodeBlocker.Create(CodeBlocker.DefaultIndentString, NewLines.Lf);

	/// <summary>
	/// Writes the header every generated file starts with.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	/// <param name="copyright">
	/// The copyright line written above the generated-file marker, without its <c>//</c> prefix.
	/// <see langword="null"/> or empty writes only the marker.
	/// </param>
	/// <remarks>
	/// A parameter rather than the literal this used to hard-code, so a consuming repository can
	/// keep the header in step with its own file header template from one place.
	/// </remarks>
	/// <exception cref="ArgumentNullException"><paramref name="codeBlocker"/> is <see langword="null"/>.</exception>
	protected static void WriteFileHeader(CodeBlocker codeBlocker, string? copyright)
	{
		if (codeBlocker is null)
		{
			throw new ArgumentNullException(nameof(codeBlocker));
		}

		if (!string.IsNullOrEmpty(copyright))
		{
			codeBlocker.WriteLine($"// {copyright}");
		}

		codeBlocker.WriteLine("// <auto-generated />");
		codeBlocker.NewLine();
	}

	/// <summary>
	/// Writes a whole source file, header included.
	/// </summary>
	/// <param name="codeBlocker">The <see cref="CodeBlocker"/> to write to.</param>
	/// <param name="sourceFileTemplate">The file to write.</param>
	/// <param name="copyright">The copyright line, as for <see cref="WriteFileHeader"/>.</param>
	protected static void WriteSourceFile(CodeBlocker codeBlocker, SourceFileTemplate sourceFileTemplate, string? copyright)
	{
		WriteFileHeader(codeBlocker, copyright);
		codeBlocker.AddSourceFile(sourceFileTemplate);
	}

	/// <summary>
	/// Whether a path names the given file.
	/// </summary>
	/// <param name="path">The additional file's path.</param>
	/// <param name="fileName">The file name to match.</param>
	/// <returns>True when the path's last segment is exactly <paramref name="fileName"/>.</returns>
	/// <remarks>
	/// Matched on the whole last segment rather than with <c>EndsWith</c>, which also matched
	/// anything whose name merely ended with the wanted one.
	/// </remarks>
	private static bool IsNamed(string path, string fileName) =>
		string.Equals(NameOf(path), fileName, StringComparison.Ordinal);

	/// <summary>
	/// The last segment of a path, handling either directory separator so the generator behaves the
	/// same wherever it runs.
	/// </summary>
	/// <param name="path">The path to take the name of.</param>
	/// <returns>The path's last segment.</returns>
	private static string NameOf(string path)
	{
		int separator = path.LastIndexOfAny(['/', '\\']);
		return separator < 0 ? path : path.Substring(separator + 1);
	}
}

/// <summary>
/// Base class for a generator driven by exactly one JSON metadata file.
/// </summary>
/// <typeparam name="T">The shape the metadata file deserializes into.</typeparam>
/// <param name="metadataFileName">The metadata file's name, without its directory.</param>
public abstract class GeneratorBase<T>(string metadataFileName) : GeneratorBase
	where T : class
{
	/// <inheritdoc/>
	protected sealed override IReadOnlyList<string> MetadataFileNames => [metadataFileName];

	/// <inheritdoc/>
	protected sealed override void Generate(SourceProductionContext context, MetadataSet metadata)
	{
		T? deserialized = metadata[metadataFileName]?.Deserialize<T>(context, MetadataParseFailed);
		if (deserialized is null)
		{
			return;
		}

		using CodeBlocker codeBlocker = CreateCodeBlocker();
		Generate(context, deserialized, codeBlocker);
	}

	/// <summary>
	/// Emits this generator's sources from its metadata.
	/// </summary>
	/// <param name="context">The source production context to add sources to.</param>
	/// <param name="metadata">The deserialized metadata.</param>
	/// <param name="codeBlocker">A <see cref="CodeBlocker"/> to build the output in.</param>
	protected abstract void Generate(SourceProductionContext context, T metadata, CodeBlocker codeBlocker);
}
