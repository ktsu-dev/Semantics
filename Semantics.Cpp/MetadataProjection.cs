// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System.Collections.Generic;
using System.Linq;

using ktsu.Semantics.Vocabulary;

/// <summary>
/// Projects this project's reader of <c>dimensions.json</c> onto what the shared vocabulary takes.
/// </summary>
/// <remarks>
/// The whole of the adapter <see cref="QuantityMetadata"/>'s remarks describe: the reader stays
/// this project's, and the physics underneath it is shared. Nothing here decides anything — every
/// rule about what the metadata means lives in <see cref="QuantityVocabulary"/>, and this only says
/// which field is which.
/// </remarks>
internal static class MetadataProjection
{
	/// <summary>
	/// Projects the whole document.
	/// </summary>
	/// <param name="metadata">The deserialised <c>dimensions.json</c>.</param>
	/// <returns>The dimensions, in the order the file declares them.</returns>
	internal static IReadOnlyList<DimensionDeclaration> ToDeclarations(this QuantityMetadata metadata) =>
		[.. metadata.PhysicalDimensions.Select(Declaration)];

	private static DimensionDeclaration Declaration(MetadataDimension dimension) => new(
		dimension.Name,
		dimension.DimensionalFormula,
		[.. Enumerable.Range(0, DimensionDeclaration.FormCount).Select(form => Form(dimension.Quantities[form]))],
		[.. dimension.Integrals.Select(Relationship)],
		[.. dimension.Derivatives.Select(Relationship)],
		[.. dimension.DotProducts.Select(Relationship)],
		[.. dimension.CrossProducts.Select(Relationship)]);

	private static FormDeclaration? Form(MetadataForm? form) =>
		form is null ? null : new FormDeclaration(form.Base, [.. form.Overloads.Select(Overload)]);

	// A constraint is carried as the flag the vocabulary reads rather than as its value: only a
	// strict-positive floor is declared anywhere, and what the vocabulary needs is whether it is
	// there. The value itself is the C# generator's business, which is where the guard is emitted.
	private static OverloadDeclaration Overload(MetadataOverload overload) =>
		new(overload.Name, overload.Description, overload.PhysicalConstraints is not null);

	private static RelationshipDeclaration Relationship(MetadataRelationship relationship) =>
		new(relationship.Other, relationship.Result, [.. relationship.Forms]);
}
