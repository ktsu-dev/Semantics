// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Vocabulary;

using System.Collections.Generic;

/// <summary>
/// What <see cref="QuantityVocabulary"/> needs to know about one dimension.
/// </summary>
/// <remarks>
/// Deliberately not either side's metadata model. <c>Semantics.SourceGenerators</c> reads
/// <c>dimensions.json</c> into a shape that also carries units, symbols and the validation its
/// diagnostics need; <c>Semantics.Cpp</c> reads the same file into a smaller shape of its own. Both
/// of those are readers, and a reader is allowed to be its own business — what is not each reader's
/// own business is the physics, which is this.
/// <para>
/// So the two keep their models and each projects onto these, which costs an adapter apiece and
/// leaves the exponents, the forms and the relationships described once. A field only one side
/// reads never has to appear here at all.
/// </para>
/// </remarks>
/// <param name="Name">What the dimension is called, as <c>dimensions.json</c> names it.</param>
/// <param name="Formula">The exponents, keyed by axis, naming only the axes that are not zero.</param>
/// <param name="Forms">
/// The five vector forms, indexed by how many components each has. An entry is null where the
/// dimension does not declare that form.
/// </param>
/// <param name="Integrals">What this dimension multiplied by another produces.</param>
/// <param name="Derivatives">What this dimension divided by another produces.</param>
/// <param name="DotProducts">What this dimension dotted with another produces.</param>
/// <param name="CrossProducts">What this dimension crossed with another produces.</param>
internal sealed record DimensionDeclaration(
	string Name,
	IReadOnlyDictionary<string, int> Formula,
	IReadOnlyList<FormDeclaration?> Forms,
	IReadOnlyList<RelationshipDeclaration> Integrals,
	IReadOnlyList<RelationshipDeclaration> Derivatives,
	IReadOnlyList<RelationshipDeclaration> DotProducts,
	IReadOnlyList<RelationshipDeclaration> CrossProducts)
{
	/// <summary>The number of forms a dimension can declare, counting the magnitude.</summary>
	internal const int FormCount = 5;

	/// <summary>
	/// Gets one form by how many components it has.
	/// </summary>
	/// <param name="form">The component count, from zero to four.</param>
	/// <returns>The form, or null when the dimension does not declare it.</returns>
	internal FormDeclaration? Form(int form) =>
		form >= 0 && form < Forms.Count ? Forms[form] : null;
}

/// <summary>One vector form: a base type and the names that refine it.</summary>
/// <param name="Base">What the base type is called.</param>
/// <param name="Overloads">The named refinements of that base.</param>
internal sealed record FormDeclaration(string Base, IReadOnlyList<OverloadDeclaration> Overloads);

/// <summary>A named refinement of a base quantity.</summary>
/// <param name="Name">What it is called.</param>
/// <param name="Description">What it is, for the generated documentation comment.</param>
/// <param name="IsStrictlyPositive">
/// Whether it opts into a floor stricter than the magnitude form's own — a wavelength, a period
/// and a half-life are quantities for which zero is unphysical rather than merely small.
/// </param>
/// <remarks>
/// A flag rather than the constraint itself, because the constraint's value is the only one either
/// side declares and what the vocabulary needs from it is whether it is there.
/// </remarks>
internal sealed record OverloadDeclaration(string Name, string Description, bool IsStrictlyPositive);

/// <summary>One declared relationship between dimensions.</summary>
/// <param name="Other">The dimension on the other side of the operator.</param>
/// <param name="Result">The dimension the operator produces.</param>
/// <param name="Forms">
/// The vector forms this relationship is declared at, or empty to mean every form the participants
/// share. A constraint rather than a request: a cross product is declared at <c>[3]</c> because it
/// is only defined in three dimensions.
/// </param>
internal sealed record RelationshipDeclaration(string Other, string Result, IReadOnlyList<int> Forms);
