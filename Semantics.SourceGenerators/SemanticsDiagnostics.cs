// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators;

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ktsu.SourceGeneratorToolkit;

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

	/// <summary>
	/// SEM008: a relationship's declared result does not follow from its operands.
	/// </summary>
	/// <remarks>
	/// A relationship is a claim, and until this existed nothing on the C# side checked it. The
	/// names were checked (SEM001) and the forms were checked (SEM003), and then the operator was
	/// emitted — so <c>Sensitivity * Pressure -&gt; ElectricPotential</c>, whose exponents are off
	/// by <c>M L⁴ T⁻⁵ I⁻²</c>, shipped for several versions as a working operator computing the
	/// wrong physics. The C++ projection had the check and refused it by name; the two now share
	/// it, and share what is done about it.
	/// <para>
	/// No operator is generated. That is what changed when the shared vocabulary went from
	/// checking emission to driving it: a relationship it refuses is simply not among the ones
	/// there are to write, in any of the directions C# spells a product in. Reporting it and
	/// emitting it anyway was the earlier, narrower step — it made the claim audible without
	/// breaking a shipped package, and the package has since had a major version to break in.
	/// </para>
	/// <para>
	/// A warning rather than an error, because the metadata's bug is a physics call rather than a
	/// spelling one and a build that cannot complete is no way to ask for one.
	/// </para>
	/// </remarks>
	public static DiagnosticDescriptor RelationshipNotDimensionallyTrue { get; } = Catalog.Warning(
		8,
		"Physics relationship does not follow from the dimensions of its operands",
		"Relationship {0} {1} No operator is generated for it; fix dimensions.json.");

	/// <summary>
	/// SEM009: a conversion factor's value is neither a decimal literal nor a fraction of two, or a
	/// <see cref="double"/> cannot hold it.
	/// </summary>
	/// <remarks>
	/// A factor is emitted as a C# <see cref="double"/> constant and parsed into every storage type, so a
	/// value that is neither form would otherwise surface as a compile error in generated code, far from
	/// the line in <c>conversions.json</c> that caused it. So would a literal beyond the range of
	/// <see cref="double"/> (CS0594), and a non-zero value that rounds to zero or a quotient that
	/// overflows would compile into a factor that is silently wrong. An error rather than a warning,
	/// because every unit using the factor fails to compile without its constant.
	/// </remarks>
	public static DiagnosticDescriptor InvalidConversionFactor { get; } = Catalog.Error(
		9,
		"conversions.json factor value is malformed",
		"Conversion factor '{0}' has value '{1}', which is not a decimal literal such as \"0.3048\" or a fraction of two such as \"5/9\" with a non-zero denominator, that a double can hold without overflowing or rounding to zero. No constant is generated for it. Fix conversions.json.");

	/// <summary>
	/// SEM010: a dimension declaring a vector form has an offset unit in its
	/// <c>availableUnits</c>, so that vector form gets no per-unit surface.
	/// </summary>
	/// <remarks>
	/// An additive offset is meaningless applied componentwise — adding 273.15 to each component
	/// of a displacement is not a unit change — so the <c>From{Unit}</c> factories and the
	/// <c>In(unit)</c> reader are not emitted for that vector type, rather than being emitted
	/// quietly wrong (#237, decision 2). The scalar forms keep their factories: the offset is
	/// correct for a V0 or V1, and only the componentwise reading of it is not.
	/// <para>
	/// Defensive rather than observed. No dimension declaring a vector form has an offset unit
	/// today, and this is what keeps that true instead of letting the combination appear silently.
	/// A warning rather than an error, because the vector type itself still generates and every
	/// other member on it is unaffected.
	/// </para>
	/// </remarks>
	public static DiagnosticDescriptor OffsetUnitOnVectorForm { get; } = Catalog.Warning(
		10,
		"Vector form cannot express a unit with an additive offset",
		"Vector type '{0}' belongs to dimension '{1}', whose availableUnits include offset unit(s) {2}. An offset conversion is meaningless applied componentwise, so no From{{Unit}} factories and no In(unit) reader are generated for '{0}'. Give the offset unit its own dimension, or drop the vector form.");
}
