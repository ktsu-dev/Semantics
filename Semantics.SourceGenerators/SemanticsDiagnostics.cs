// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Semantics.SourceGenerators.CodeGen;

/// <summary>
/// Every diagnostic this repository's generators report, allocated from one catalogue so the
/// identifiers stay consecutive and the category stays consistent.
/// </summary>
/// <remarks>
/// Documented in <c>CLAUDE.md</c> and <c>docs/physics-generator.md</c>; keep those in step when
/// adding one, and add the new identifier to <c>AnalyzerReleases.Unshipped.md</c> —
/// <c>AnalyzerReleaseTrackingTests</c> checks that it is there.
/// </remarks>
public static class SemanticsDiagnostics
{
	/// <summary>The catalogue every descriptor below is allocated from.</summary>
	public static DiagnosticCatalog Catalog { get; } = new("SEM", "Semantics.SourceGenerators");

	/// <summary>Gets every descriptor this repository's generators can report.</summary>
	public static IReadOnlyList<DiagnosticDescriptor> All => Catalog.Descriptors;

	/// <summary>SEM001: a relationship names a dimension that does not exist.</summary>
	public static DiagnosticDescriptor UnknownDimensionReference { get; } = Catalog.Warning(
		1,
		"Unknown dimension reference in physics relationship",
		"Dimension '{0}' references unknown dimension '{1}' in {2}; the operator will not be generated. Check spelling and that the referenced dimension exists in dimensions.json.");

	/// <summary>SEM002: dimensions.json failed schema-level validation.</summary>
	public static DiagnosticDescriptor MetadataValidationFailed { get; } = Catalog.Warning(
		2,
		"dimensions.json metadata validation failed",
		"dimensions.json validation issue: {0}");

	/// <summary>SEM003: a relationship requires a vector form a participant does not declare.</summary>
	public static DiagnosticDescriptor RelationshipFormMissing { get; } = Catalog.Warning(
		3,
		"Relationship requires a vector form not declared on a participating dimension",
		"Relationship in dimension '{0}' ({1}) explicitly requests form V{2}, but '{3}' does not declare that form. The operator will not be generated.");

	/// <summary>SEM004: dimensions.json names a unit that units.json does not declare.</summary>
	public static DiagnosticDescriptor UnknownUnitReference { get; } = Catalog.Warning(
		4,
		"dimensions.json references a unit not declared in units.json",
		"Unit '{0}' (referenced by dimension '{1}'.availableUnits) is not declared in units.json; the generated From{0} factory will use an identity conversion. Add the unit to units.json or fix the spelling.");

	/// <summary>SEM005: logarithmic.json failed schema-level validation.</summary>
	public static DiagnosticDescriptor InvalidScaleDefinition { get; } = Catalog.Warning(
		5,
		"logarithmic.json scale definition is invalid",
		"logarithmic.json validation issue: {0}");

	/// <summary>
	/// SEM006: a metadata file a generator declared is not in the compilation.
	/// </summary>
	/// <remarks>
	/// Previously the generator produced no output and no explanation, which is indistinguishable
	/// from a generator that simply had nothing to emit.
	/// </remarks>
	public static DiagnosticDescriptor MetadataFileMissing { get; } = Catalog.Warning(
		6,
		"A metadata file is missing from the compilation",
		"Metadata file '{0}' was not supplied as an AdditionalFile; the generator produced nothing. Check the AdditionalFiles item group in the consuming project.");

	/// <summary>
	/// SEM007: a metadata file could not be parsed.
	/// </summary>
	/// <remarks>
	/// Replaces the base generator's <c>CONV001</c> in category <c>SourceGenerator</c>, a leftover
	/// from when the base served only <c>ConversionsGenerator</c>. It also covers the path that used
	/// to swallow the exception and return null, where a malformed <c>units.json</c> produced no
	/// diagnostic and the generator silently emitted identity conversions.
	/// </remarks>
	public static DiagnosticDescriptor MetadataParseFailed { get; } = Catalog.Error(
		7,
		"A metadata file could not be parsed",
		"Metadata file '{0}' could not be parsed: {1}");
}
