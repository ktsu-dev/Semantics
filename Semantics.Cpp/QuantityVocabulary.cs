// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
/// <param name="Form">
/// How many components it has: zero for a magnitude, one for a signed scalar, and two to four for
/// a vector.
/// </param>
/// <param name="MagnitudeType">
/// The name of the same dimension's magnitude form, which is what <c>magnitude()</c> answers with.
/// </param>
internal sealed record QuantityType(
	string Name,
	string Description,
	DimensionVector Dimension,
	string? Refines,
	Magnitude Magnitude,
	int Form,
	string MagnitudeType)
{
	/// <summary>Gets a value indicating whether this holds several components rather than one.</summary>
	internal bool IsVector => Form >= 2;
}

/// <summary>
/// What combining two quantities produces.
/// </summary>
internal enum RelationshipKind
{
	/// <summary>The left operand multiplied by the right.</summary>
	Product,

	/// <summary>The left operand divided by the right.</summary>
	Quotient,

	/// <summary>Two vectors reduced to how much of one lies along the other.</summary>
	Dot,

	/// <summary>Two three-component vectors combined into the one perpendicular to both.</summary>
	Cross,
}

/// <summary>
/// One generated operator: two quantities in, a named quantity out.
/// </summary>
/// <param name="Left">The type on the left, with <paramref name="Form"/> components.</param>
/// <param name="Right">
/// The type on the right. For a product or a quotient this is always a magnitude, because scaling
/// a vector is the only way those reach the vector forms; for a cross product it has as many
/// components as the left.
/// </param>
/// <param name="Result">The type produced.</param>
/// <param name="Kind">How the two are combined.</param>
/// <param name="Form">How many components the left operand and the result have.</param>
internal sealed record QuantityRelationship(string Left, string Right, string Result, RelationshipKind Kind, int Form)
{
	/// <summary>Gets the operator as C++ spells it, or the function's name when it has no symbol.</summary>
	internal string Symbol => Kind switch
	{
		RelationshipKind.Product => "*",
		RelationshipKind.Quotient => "/",
		RelationshipKind.Cross => "cross",
		_ => "dot",
	};

	/// <summary>Gets a value indicating whether C++ spells this as an operator rather than a call.</summary>
	internal bool IsOperator => Kind is RelationshipKind.Product or RelationshipKind.Quotient;

	/// <inheritdoc />
	public override string ToString() =>
		IsOperator ? $"{Left} {Symbol} {Right} -> {Result}" : $"{Symbol}({Left}, {Right}) -> {Result}";
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
/// <para>
/// The exponents are not the only thing that can make a claim unkeepable, and the vector forms
/// brought the second kind: a magnitude cannot be negative, so a relationship whose value is
/// signed but whose declared result is a magnitude is refused for that reason instead. A dot
/// product is the case in the metadata. The message says the same two things either way -- what is
/// wrong, and what would fix it.
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
		Dictionary<string, MetadataForms> formsOf = [];

		foreach (MetadataDimension dimension in metadata.PhysicalDimensions)
		{
			DimensionVector exponents = DimensionVector.FromFormula(dimension.DimensionalFormula);
			byDimensionName[dimension.Name] = exponents;
			formsOf[dimension.Name] = dimension.Quantities;

			MetadataForm? magnitude = dimension.Quantities.Vector0;
			if (magnitude is null || string.IsNullOrEmpty(magnitude.Base))
			{
				// Every other form reports its length through this one, so a dimension without it
				// has nothing for the vector forms to answer with either.
				refused.Add(new VocabularyIssue(dimension.Name, "has no vector0 form, so it has no magnitude type to generate."));
				continue;
			}

			for (int form = 0; form < MetadataForms.Count; form++)
			{
				types.AddRange(Declared(dimension, form, exponents, magnitude.Base));
			}
		}

		List<QuantityRelationship> relationships =
			[.. ResolveRelationships(metadata, byDimensionName, formsOf, refused)];

		return new QuantityVocabulary(
			new ReadOnlyCollection<QuantityType>(types),
			new ReadOnlyCollection<QuantityRelationship>(relationships),
			new ReadOnlyCollection<VocabularyIssue>(refused));
	}

	/// <summary>
	/// One dimension's classes at one form: the base, and each name that refines it.
	/// </summary>
	/// <remarks>
	/// Only the magnitude form is bounded below, and that is the whole reason the forms are
	/// separate types rather than one type used several ways: a <c>Speed</c> cannot be negative and
	/// a component of a <c>Velocity3D</c> obviously can. A stricter floor declared on an overload
	/// belongs to the magnitude for the same reason -- a wavelength is never zero, but a component
	/// of a displacement along one axis routinely is.
	/// </remarks>
	private static IEnumerable<QuantityType> Declared(
		MetadataDimension dimension,
		int form,
		DimensionVector exponents,
		string magnitudeType)
	{
		MetadataForm? declared = dimension.Quantities[form];
		if (declared is null || string.IsNullOrEmpty(declared.Base))
		{
			yield break;
		}

		yield return new QuantityType(
			declared.Base,
			Describe(dimension.Name, form),
			exponents,
			Refines: null,
			form == 0 ? Magnitude.NonNegative : Magnitude.Signed,
			form,
			magnitudeType);

		foreach (MetadataOverload overload in declared.Overloads)
		{
			yield return new QuantityType(
				overload.Name,
				overload.Description,
				exponents,
				Refines: declared.Base,
				form != 0 ? Magnitude.Signed
					: overload.PhysicalConstraints is null ? Magnitude.NonNegative : Magnitude.Positive,
				form,
				magnitudeType);
		}
	}

	private static string Describe(string dimension, int form) => form switch
	{
		0 => $"The magnitude of {Article(dimension)} {Spaced(dimension)}.",
		1 => $"A signed {Spaced(dimension)} along one axis.",
		_ => $"{Capitalised(Article(dimension))} {Spaced(dimension)} in {form.ToString(CultureInfo.InvariantCulture)} dimensions.",
	};

	private static IEnumerable<QuantityRelationship> ResolveRelationships(
		QuantityMetadata metadata,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, MetadataForms> formsOf,
		List<VocabularyIssue> refused)
	{
		foreach (MetadataDimension dimension in metadata.PhysicalDimensions)
		{
			IEnumerable<(MetadataRelationship Declared, RelationshipKind Kind)> declared =
			[
				.. dimension.Integrals.Select(relationship => (relationship, RelationshipKind.Product)),
				.. dimension.Derivatives.Select(relationship => (relationship, RelationshipKind.Quotient)),
				.. dimension.DotProducts.Select(relationship => (relationship, RelationshipKind.Dot)),
				.. dimension.CrossProducts.Select(relationship => (relationship, RelationshipKind.Cross)),
			];

			foreach ((MetadataRelationship relationship, RelationshipKind kind) in declared)
			{
				foreach (QuantityRelationship resolved in
					Resolve(dimension, relationship, kind, byDimensionName, formsOf, refused))
				{
					yield return resolved;
				}
			}
		}
	}

	/// <summary>
	/// One declared relationship, at every form it reaches.
	/// </summary>
	/// <remarks>
	/// The dimensional check does not depend on the form -- the exponents of a <c>Velocity3D</c>
	/// are the exponents of a <c>Speed</c> -- so it happens once, before the forms are walked. A
	/// claim that is not dimensionally true is one refusal rather than five.
	/// </remarks>
	private static List<QuantityRelationship> Resolve(
		MetadataDimension dimension,
		MetadataRelationship relationship,
		RelationshipKind kind,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, MetadataForms> formsOf,
		List<VocabularyIssue> refused)
	{
		string subject = Subject(dimension.Name, relationship, kind);

		foreach (string named in (string[])[relationship.Other, relationship.Result])
		{
			if (!byDimensionName.ContainsKey(named))
			{
				// The same gap SEM001 reports on the .NET side, seen from here.
				refused.Add(new VocabularyIssue(subject, $"names '{named}', which dimensions.json does not declare."));
				return [];
			}
		}

		DimensionVector left = byDimensionName[dimension.Name];
		DimensionVector right = byDimensionName[relationship.Other];
		DimensionVector result = byDimensionName[relationship.Result];

		DimensionVector combined = kind == RelationshipKind.Quotient ? left - right : left + right;
		if (!combined.Equals(result))
		{
			refused.Add(new VocabularyIssue(
				subject,
				$"is not dimensionally true: {left} {(kind == RelationshipKind.Quotient ? "/" : "*")} {right} is {combined}, and {relationship.Result} is {result}."));
			return [];
		}

		// A dot product answers with a signed value -- a force opposing a displacement does
		// negative work -- and the metadata names a magnitude for its result, which cannot hold
		// one. No spelling fixes that: the result needs a signed form to land in.
		if (kind == RelationshipKind.Dot)
		{
			refused.Add(new VocabularyIssue(
				subject,
				$"reduces to a signed value -- two vectors that oppose each other give a negative one -- and '{relationship.Result}' declares only a magnitude form, which cannot be negative. A vector1 form on it is what would let this be generated."));
			return [];
		}

		return At(subject, dimension.Name, relationship, kind, formsOf, refused);
	}

	/// <summary>
	/// The forms one relationship is emitted at.
	/// </summary>
	/// <remarks>
	/// A product or a quotient carries its form on the left operand and the result, with the right
	/// operand always a magnitude: a <c>Velocity3D</c> times a <c>Duration</c> is a
	/// <c>Displacement3D</c>, and there is no reading in which the duration has three components.
	/// A cross product is the other shape -- three components on all three sides -- and is defined
	/// in three dimensions and nowhere else, which is why it defaults to that one form rather than
	/// to all of them.
	/// <para>
	/// When the metadata lists forms explicitly, a form a participant does not declare is refused
	/// by name; when it lists none, the relationship is emitted at whatever forms the participants
	/// share and saying nothing about the rest is the intended answer. That is the same split
	/// <c>SEM003</c> makes on the .NET side.
	/// </para>
	/// </remarks>
	private static List<QuantityRelationship> At(
		string subject,
		string self,
		MetadataRelationship relationship,
		RelationshipKind kind,
		IReadOnlyDictionary<string, MetadataForms> formsOf,
		List<VocabularyIssue> refused)
	{
		bool crossed = kind == RelationshipKind.Cross;
		IReadOnlyList<int> wanted = relationship.Forms.Count > 0
			? [.. relationship.Forms]
			: crossed ? [3] : [.. Enumerable.Range(0, MetadataForms.Count)];

		List<QuantityRelationship> emitted = [];

		foreach (int form in wanted)
		{
			// The right operand of a cross product has the same shape as the left; of a product or
			// a quotient it is the magnitude the vector is scaled by.
			string? leftType = Base(formsOf, self, form);
			string? rightType = Base(formsOf, relationship.Other, crossed ? form : 0);
			string? resultType = Base(formsOf, relationship.Result, form);

			if (leftType is null || rightType is null || resultType is null)
			{
				if (relationship.Forms.Count > 0)
				{
					// The same gap SEM003 reports on the .NET side: a form asked for by name that
					// one of the participants does not have.
					refused.Add(new VocabularyIssue(
						subject,
						$"is declared at vector{form.ToString(CultureInfo.InvariantCulture)}, which {Missing(formsOf, form, self, relationship, crossed)} does not declare."));
				}

				continue;
			}

			emitted.Add(new QuantityRelationship(leftType, rightType, resultType, kind, form));
		}

		return emitted;
	}

	private static string Missing(
		IReadOnlyDictionary<string, MetadataForms> formsOf,
		int form,
		string self,
		MetadataRelationship relationship,
		bool crossed)
	{
		IEnumerable<string> participants = crossed
			? [self, relationship.Other, relationship.Result]
			: [self, relationship.Result];

		return string.Join(" and ", participants.Where(named => Base(formsOf, named, form) is null));
	}

	private static string? Base(IReadOnlyDictionary<string, MetadataForms> formsOf, string dimension, int form)
	{
		string? name = formsOf.TryGetValue(dimension, out MetadataForms? forms) ? forms[form]?.Base : null;
		return string.IsNullOrEmpty(name) ? null : name;
	}

	private static string Subject(string self, MetadataRelationship relationship, RelationshipKind kind) => kind switch
	{
		RelationshipKind.Product => $"{self} * {relationship.Other} -> {relationship.Result}",
		RelationshipKind.Quotient => $"{self} / {relationship.Other} -> {relationship.Result}",
		RelationshipKind.Cross => $"cross({self}, {relationship.Other}) -> {relationship.Result}",
		_ => $"dot({self}, {relationship.Other}) -> {relationship.Result}",
	};

	/// <summary>
	/// Splits a PascalCase dimension name for prose, so a comment reads "an angular velocity"
	/// rather than "an AngularVelocity".
	/// </summary>
	private static string Spaced(string name) =>
		string.Concat(name.Select((character, index) =>
			index > 0 && char.IsUpper(character) && !char.IsUpper(name[index - 1])
				? $" {char.ToLowerInvariant(character)}"
				: $"{(index == 0 ? char.ToLowerInvariant(character) : character)}"));

	private static string Capitalised(string word) => $"{char.ToUpperInvariant(word[0])}{word[1..]}";

	private static string Article(string name) => "AEIOU".Contains(name[0]) ? "an" : "a";
}
