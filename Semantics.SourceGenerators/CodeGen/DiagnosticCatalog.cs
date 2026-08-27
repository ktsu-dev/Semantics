// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.CodeGen;

using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis;

/// <summary>
/// Declares and reports a generator's diagnostics under one identifier prefix and category.
/// </summary>
/// <remarks>
/// Every generator that reads metadata needs the same handful of diagnostics, and hand-rolling a
/// <see cref="DiagnosticDescriptor"/> plus a private reporting helper per generator is how the
/// identifiers drift: the base generator used to report parse failures as <c>CONV001</c> in category
/// <c>SourceGenerator</c> while everything derived from it used <c>SEM00x</c> in its own category.
/// Allocating them from one catalogue keeps the scheme consistent and gives
/// <c>AnalyzerReleaseTrackingTests</c> something to enumerate, so a descriptor that is missing from
/// <c>AnalyzerReleases.Unshipped.md</c> fails a test rather than RS2008 at build time.
/// <para>
/// Nothing in this type is specific to any one generator; it is part of the reusable layer.
/// </para>
/// </remarks>
/// <param name="idPrefix">The identifier prefix, for example <c>SEM</c>.</param>
/// <param name="category">The category reported on every descriptor.</param>
public sealed class DiagnosticCatalog(string idPrefix, string category)
{
	private readonly List<DiagnosticDescriptor> descriptors = [];

	/// <summary>Gets the category every descriptor in this catalogue is reported under.</summary>
	public string Category { get; } = category;

	/// <summary>
	/// Gets every descriptor allocated from this catalogue, in allocation order.
	/// </summary>
	public IReadOnlyList<DiagnosticDescriptor> Descriptors => descriptors;

	/// <summary>
	/// Allocates a warning descriptor.
	/// </summary>
	/// <param name="number">The numeric part of the identifier, formatted to three digits.</param>
	/// <param name="title">The diagnostic title.</param>
	/// <param name="messageFormat">The message format string.</param>
	/// <returns>The descriptor, also recorded in <see cref="Descriptors"/>.</returns>
	public DiagnosticDescriptor Warning(int number, string title, string messageFormat) =>
		Add(number, title, messageFormat, DiagnosticSeverity.Warning);

	/// <summary>
	/// Allocates an error descriptor.
	/// </summary>
	/// <param name="number">The numeric part of the identifier, formatted to three digits.</param>
	/// <param name="title">The diagnostic title.</param>
	/// <param name="messageFormat">The message format string.</param>
	/// <returns>The descriptor, also recorded in <see cref="Descriptors"/>.</returns>
	public DiagnosticDescriptor Error(int number, string title, string messageFormat) =>
		Add(number, title, messageFormat, DiagnosticSeverity.Error);

	private DiagnosticDescriptor Add(int number, string title, string messageFormat, DiagnosticSeverity severity)
	{
		DiagnosticDescriptor descriptor = new(
			id: idPrefix + number.ToString("D3", CultureInfo.InvariantCulture),
			title: title,
			messageFormat: messageFormat,
			category: Category,
			defaultSeverity: severity,
			isEnabledByDefault: true);

		descriptors.Add(descriptor);
		return descriptor;
	}
}

/// <summary>
/// Reporting helpers that keep a diagnostic to one call at the site that found the problem.
/// </summary>
public static class DiagnosticReporting
{
	/// <summary>
	/// Reports a diagnostic with no source location.
	/// </summary>
	/// <param name="context">The source production context to report to.</param>
	/// <param name="descriptor">The descriptor to report.</param>
	/// <param name="messageArgs">Arguments for the descriptor's message format.</param>
	public static void Report(
		this SourceProductionContext context,
		DiagnosticDescriptor descriptor,
		params object?[] messageArgs) =>
		context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, messageArgs));

	/// <summary>
	/// Reports a diagnostic pointing at a position in a metadata file.
	/// </summary>
	/// <param name="context">The source production context to report to.</param>
	/// <param name="descriptor">The descriptor to report.</param>
	/// <param name="location">Where in the metadata the problem is, or <see langword="null"/>.</param>
	/// <param name="messageArgs">Arguments for the descriptor's message format.</param>
	public static void ReportAt(
		this SourceProductionContext context,
		DiagnosticDescriptor descriptor,
		Location? location,
		params object?[] messageArgs) =>
		context.ReportDiagnostic(Diagnostic.Create(descriptor, location ?? Location.None, messageArgs));
}
