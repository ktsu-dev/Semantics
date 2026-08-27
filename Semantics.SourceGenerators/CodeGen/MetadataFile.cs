// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.CodeGen;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// One metadata file a generator was asked for, with the text it was found to contain.
/// </summary>
/// <param name="fileName">The file's name, without its directory.</param>
/// <param name="text">The file's contents.</param>
/// <param name="sourceText">The underlying <see cref="SourceText"/>, used to build locations.</param>
/// <param name="path">The file's full path, used to build locations.</param>
public sealed class MetadataFile(string fileName, string text, SourceText? sourceText, string path)
{
	/// <summary>Gets the file's name, without its directory.</summary>
	public string FileName { get; } = fileName;

	/// <summary>Gets the file's contents.</summary>
	public string Text { get; } = text;

	/// <summary>
	/// Finds the first occurrence of <paramref name="needle"/> in the file and returns a location
	/// covering it.
	/// </summary>
	/// <param name="needle">The text to find — typically the offending name from the metadata.</param>
	/// <returns>
	/// A location in the metadata file, or <see cref="Location.None"/> when the text is not found.
	/// </returns>
	/// <remarks>
	/// A diagnostic reported at <see cref="Location.None"/> tells the reader which name is wrong but
	/// not where it is written, which for a file the size of <c>dimensions.json</c> is most of the
	/// work. Matching on the name itself is approximate — the first occurrence wins, and a name that
	/// appears in several entries points at the first — but it is navigable, which nothing was
	/// before.
	/// </remarks>
	public Location FindLocation(string needle)
	{
		if (sourceText is null || string.IsNullOrEmpty(needle))
		{
			return Location.None;
		}

		int index = Text.IndexOf(needle, StringComparison.Ordinal);
		return index < 0
			? Location.None
			: Location.Create(path, new TextSpan(index, needle.Length), sourceText.Lines.GetLinePositionSpan(new TextSpan(index, needle.Length)));
	}

	/// <summary>
	/// Deserializes the file into <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The metadata shape to deserialize into.</typeparam>
	/// <param name="context">The source production context, used to report a parse failure.</param>
	/// <param name="parseFailed">The descriptor reported when the file cannot be parsed.</param>
	/// <returns>The deserialized metadata, or <see langword="null"/> when parsing failed.</returns>
	/// <remarks>
	/// A parse failure is always reported. It used to be swallowed on the path that loaded a second
	/// metadata file, so a malformed <c>units.json</c> produced no diagnostic at all and the
	/// generator silently emitted identity conversions.
	/// </remarks>
	public T? Deserialize<T>(SourceProductionContext context, DiagnosticDescriptor parseFailed)
		where T : class
	{
		try
		{
			JsonSerializerOptions options = new()
			{
				PropertyNameCaseInsensitive = true
			};

			T? metadata = JsonSerializer.Deserialize<T>(Text, options);
			if (metadata is not null)
			{
				return metadata;
			}

			context.Report(parseFailed, FileName, "the document deserialized to null");
			return null;
		}
		catch (JsonException ex)
		{
			context.Report(parseFailed, FileName, ex.Message);
			return null;
		}
	}
}

/// <summary>
/// The metadata files a generator asked for, keyed by file name.
/// </summary>
public sealed class MetadataSet(IReadOnlyDictionary<string, MetadataFile> files)
{
	/// <summary>
	/// Gets the named metadata file.
	/// </summary>
	/// <param name="fileName">The file name the generator declared.</param>
	/// <returns>The file, or <see langword="null"/> when it was not supplied to the compilation.</returns>
	public MetadataFile? this[string fileName] =>
		files.TryGetValue(fileName, out MetadataFile? file) ? file : null;
}
