// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// How a quantity is bounded below.
/// </summary>
internal enum Magnitude
{
	/// <summary>A signed value: a component of a vector, or a scale that runs both ways.</summary>
	Signed,

	/// <summary>A magnitude, which cannot be negative.</summary>
	NonNegative,

	/// <summary>A magnitude for which zero is unphysical too, like a wavelength or a half-life.</summary>
	Positive,
}

/// <summary>
/// One generated quantity class.
/// </summary>
/// <param name="Name">What it is called, which is what <c>dimensions.json</c> calls it.</param>
/// <param name="Description">What it is, for the generated documentation comment.</param>
/// <param name="Dimension">Its exponents.</param>
/// <param name="Refines">The base it widens into, or null when it is the base.</param>
/// <param name="Magnitude">How it is bounded below.</param>
internal sealed record QuantityType(
	string Name,
	string Description,
	DimensionVector Dimension,
	string? Refines,
	Magnitude Magnitude);

/// <summary>
/// What combining two quantities produces.
/// </summary>
internal enum RelationshipKind
{
	/// <summary>The left operand multiplied by the right.</summary>
	Product,

	/// <summary>The left operand divided by the right.</summary>
	Quotient,
}

/// <summary>
/// One generated operator: two quantities in, a named quantity out.
/// </summary>
internal sealed record QuantityRelationship(string Left, string Right, string Result, RelationshipKind Kind)
{
	/// <summary>Gets the operator as C++ spells it.</summary>
	internal string Symbol => Kind == RelationshipKind.Product ? "*" : "/";

	/// <inheritdoc />
	public override string ToString() => $"{Left} {Symbol} {Right} -> {Result}";
}

/// <summary>
/// Something the metadata says that the generator will not emit, and why.
/// </summary>
/// <param name="Subject">What was refused, named the way the metadata names it.</param>
/// <param name="Reason">Why, in terms a person editing the metadata can act on.</param>
internal sealed record VocabularyIssue(string Subject, string Reason)
{
	/// <inheritdoc />
	public override string ToString() => $"{Subject}: {Reason}";
}

/// <summary>
/// The metadata, resolved into what the C++ projection needs, with everything it cannot honour
/// separated out rather than silently dropped.
/// </summary>
/// <remarks>
/// The separation is the point. A relationship is a claim -- <c>Force * Length -&gt; Torque</c> --
/// and the exponents are what check it. A claim the exponents contradict is not emitted, because
/// the generated operator builds its result out of <c>Quantity</c> arithmetic and would simply
/// fail to compile; refusing it by name, with the two dimensions written out, is the same house
/// style the rest of this stack uses for something it cannot express.
/// <para>
/// That check earns its keep immediately: on the metadata as it stands it refuses four
/// relationships, one of which (<c>Sensitivity * Pressure -&gt; ElectricPotential</c>) was already
/// wrong before angle existed and had never been noticed, because nothing had ever multiplied the
/// exponents out.
/// </para>
/// </remarks>
internal sealed class QuantityVocabulary
{
	private QuantityVocabulary(
		IReadOnlyList<QuantityType> types,
		IReadOnlyList<QuantityRelationship> relationships,
		IReadOnlyList<VocabularyIssue> refused)
	{
		Types = types;
		Relationships = relationships;
		Refused = refused;
	}

	/// <summary>Gets every class to generate, in the order the metadata declares them.</summary>
	internal IReadOnlyList<QuantityType> Types { get; }

	/// <summary>Gets every operator to generate.</summary>
	internal IReadOnlyList<QuantityRelationship> Relationships { get; }

	/// <summary>Gets what the metadata asked for and did not get.</summary>
	internal IReadOnlyList<VocabularyIssue> Refused { get; }

	/// <summary>
	/// Resolves the metadata.
	/// </summary>
	/// <param name="metadata">The deserialised <c>dimensions.json</c>.</param>
	/// <returns>The vocabulary, and everything refused.</returns>
	internal static QuantityVocabulary FromMetadata(QuantityMetadata metadata)
	{
		List<QuantityType> types = [];
		List<VocabularyIssue> refused = [];

		Dictionary<string, DimensionVector> byDimensionName = [];
		Dictionary<string, string> baseTypeOf = [];

		foreach (MetadataDimension dimension in metadata.PhysicalDimensions)
		{
			DimensionVector exponents = DimensionVector.FromFormula(dimension.DimensionalFormula);
			byDimensionName[dimension.Name] = exponents;

			// Only the magnitude form is projected so far. The vector forms are distinct classes
			// too, and they are the next thing this generator grows; they need componentwise
			// operations, which have their own rules, so they are deliberately not half-done here.
			MetadataForm? magnitude = dimension.Quantities.Vector0;
			if (magnitude is null || string.IsNullOrEmpty(magnitude.Base))
			{
				refused.Add(new VocabularyIssue(dimension.Name, "has no vector0 form, so it has no magnitude type to generate."));
				continue;
			}

			baseTypeOf[dimension.Name] = magnitude.Base;
			types.Add(new QuantityType(
				magnitude.Base,
				$"The magnitude of {Article(dimension.Name)} {Spaced(dimension.Name)}.",
				exponents,
				Refines: null,
				Magnitude.NonNegative));

			foreach (MetadataOverload overload in magnitude.Overloads)
			{
				types.Add(new QuantityType(
					overload.Name,
					overload.Description,
					exponents,
					Refines: magnitude.Base,
					overload.PhysicalConstraints is null ? Magnitude.NonNegative : Magnitude.Positive));
			}
		}

		List<QuantityRelationship> relationships =
			[.. ResolveRelationships(metadata, byDimensionName, baseTypeOf, refused)];

		return new QuantityVocabulary(
			new ReadOnlyCollection<QuantityType>(types),
			new ReadOnlyCollection<QuantityRelationship>(relationships),
			new ReadOnlyCollection<VocabularyIssue>(refused));
	}

	private static IEnumerable<QuantityRelationship> ResolveRelationships(
		QuantityMetadata metadata,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, string> baseTypeOf,
		List<VocabularyIssue> refused)
	{
		foreach (MetadataDimension dimension in metadata.PhysicalDimensions)
		{
			// A dot or a cross product is a statement about vector forms rather than magnitudes,
			// so neither belongs here: `dot` on two magnitudes is just their product, and a cross
			// product of two magnitudes is not defined at all.
			foreach (QuantityRelationship? resolved in dimension.Integrals
				.Select(relationship => Resolve(dimension, relationship, RelationshipKind.Product, byDimensionName, baseTypeOf, refused))
				.Concat(dimension.Derivatives
					.Select(relationship => Resolve(dimension, relationship, RelationshipKind.Quotient, byDimensionName, baseTypeOf, refused))))
			{
				if (resolved is not null)
				{
					yield return resolved;
				}
			}
		}
	}

	private static QuantityRelationship? Resolve(
		MetadataDimension dimension,
		MetadataRelationship relationship,
		RelationshipKind kind,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, string> baseTypeOf,
		List<VocabularyIssue> refused)
	{
		string subject = $"{dimension.Name} {(kind == RelationshipKind.Product ? "*" : "/")} {relationship.Other} -> {relationship.Result}";

		foreach (string named in (string[])[relationship.Other, relationship.Result])
		{
			if (!byDimensionName.ContainsKey(named))
			{
				// The same gap SEM001 reports on the .NET side, seen from here.
				refused.Add(new VocabularyIssue(subject, $"names '{named}', which dimensions.json does not declare."));
				return null;
			}
		}

		DimensionVector left = byDimensionName[dimension.Name];
		DimensionVector right = byDimensionName[relationship.Other];
		DimensionVector result = byDimensionName[relationship.Result];

		DimensionVector combined = kind == RelationshipKind.Product ? left + right : left - right;
		if (!combined.Equals(result))
		{
			refused.Add(new VocabularyIssue(
				subject,
				$"is not dimensionally true: {left} {(kind == RelationshipKind.Product ? "*" : "/")} {right} is {combined}, and {relationship.Result} is {result}."));
			return null;
		}

		return new QuantityRelationship(
			baseTypeOf[dimension.Name],
			baseTypeOf[relationship.Other],
			baseTypeOf[relationship.Result],
			kind);
	}

	/// <summary>
	/// Splits a PascalCase dimension name for prose, so a comment reads "an angular velocity"
	/// rather than "an AngularVelocity".
	/// </summary>
	private static string Spaced(string name) =>
		string.Concat(name.Select((character, index) =>
			index > 0 && char.IsUpper(character) && !char.IsUpper(name[index - 1])
				? $" {char.ToLowerInvariant(character)}"
				: $"{(index == 0 ? char.ToLowerInvariant(character) : character)}"));

	private static string Article(string name) => "AEIOU".Contains(name[0]) ? "an" : "a";
}
