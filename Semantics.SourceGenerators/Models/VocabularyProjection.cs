// Copyright (c) 2023-2026 ktsu-dev contributors

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;
using System.Linq;

using ktsu.Semantics.Vocabulary;

/// <summary>
/// Projects this project's reader of <c>dimensions.json</c> onto what the shared vocabulary takes.
/// </summary>
/// <remarks>
/// <see cref="DimensionsMetadata"/> carries more than the physics — symbols, units, the per-overload
/// relationship map, and the validation the SEM diagnostics are built on — and none of that is the
/// vocabulary's business. This says which field is which and nothing else; every rule about what the
/// metadata means lives in <see cref="QuantityVocabulary"/>, shared with the C++ projection.
/// </remarks>
internal static class VocabularyProjection
{
	/// <summary>
	/// Projects the whole document.
	/// </summary>
	/// <param name="metadata">The deserialised <c>dimensions.json</c>.</param>
	/// <returns>The dimensions, in the order the file declares them.</returns>
	internal static IReadOnlyList<DimensionDeclaration> ToDeclarations(this DimensionsMetadata metadata) =>
		[.. metadata.PhysicalDimensions.Select(Declaration)];

	private static DimensionDeclaration Declaration(PhysicalDimension dimension) => new(
		dimension.Name,
		dimension.DimensionalFormula,
		[
			Form(dimension.Quantities.Vector0),
			Form(dimension.Quantities.Vector1),
			Form(dimension.Quantities.Vector2),
			Form(dimension.Quantities.Vector3),
			Form(dimension.Quantities.Vector4),
		],
		[.. dimension.Integrals.Select(Relationship)],
		[.. dimension.Derivatives.Select(Relationship)],
		[.. dimension.DotProducts.Select(Relationship)],
		[.. dimension.CrossProducts.Select(Relationship)]);

	private static FormDeclaration? Form(VectorFormDefinition? form) =>
		form is null ? null : new FormDeclaration(form.Base, [.. form.Overloads.Select(Overload)]);

	// The constraint is carried as the flag the vocabulary reads rather than as its value: what the
	// vocabulary needs is whether a stricter floor is declared. The value itself stays here, where
	// the Vector0Guards.EnsurePositive call is emitted from — and the flag is read off that value
	// rather than off the presence of the object holding it, so a constraint of some other kind,
	// when one is added, does not silently turn the strict floor on.
	private static OverloadDeclaration Overload(OverloadDefinition overload) =>
		new(overload.Name, overload.Description, overload.PhysicalConstraints?.MinExclusive == "0");

	private static RelationshipDeclaration Relationship(RelationshipDefinition relationship) =>
		new(relationship.Other, relationship.Result, [.. relationship.Forms]);
}
