// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Vocabulary;

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
	/// <summary>Gets the operator's symbol, or the function's name when it has no symbol.</summary>
	/// <remarks>
	/// A dot and a cross product are calls rather than operators in every target that has them,
	/// which is why they are named here rather than spelled.
	/// </remarks>
	internal string Symbol => Kind switch
	{
		RelationshipKind.Product => "*",
		RelationshipKind.Quotient => "/",
		RelationshipKind.Cross => "cross",
		_ => "dot",
	};

	/// <summary>Gets a value indicating whether this is spelled as an operator rather than a call.</summary>
	internal bool IsOperator => Kind is RelationshipKind.Product or RelationshipKind.Quotient;

	/// <inheritdoc />
	public override string ToString() =>
		IsOperator ? $"{Left} {Symbol} {Right} -> {Result}" : $"{Symbol}({Left}, {Right}) -> {Result}";
}

/// <summary>
/// Why something the metadata asked for was refused.
/// </summary>
/// <remarks>
/// Carried so a consumer can report only what it does not already diagnose for itself. The C#
/// generator has had its own diagnostics for an unknown dimension (SEM001) and a missing vector
/// form (SEM003) since before this was shared, so it reports the two kinds that are genuinely new
/// to it and leaves those alone; the C++ projection prints all of them, having no other channel.
/// </remarks>
internal enum VocabularyIssueKind
{
	/// <summary>A relationship names a dimension the metadata does not declare.</summary>
	UnknownDimension,

	/// <summary>A dimension declares no magnitude form, so its other forms have nothing to measure against.</summary>
	NoMagnitudeForm,

	/// <summary>The exponents contradict the declared result.</summary>
	NotDimensionallyTrue,

	/// <summary>The value is signed and the declared result is a magnitude, which cannot hold one.</summary>
	SignedResultInMagnitudeForm,

	/// <summary>A relationship names a vector form a participant does not declare.</summary>
	MissingVectorForm,
}

/// <summary>
/// Something the metadata says that will not be emitted, and why.
/// </summary>
/// <param name="Kind">Which kind of problem it is, so a consumer can report only what it needs to.</param>
/// <param name="Subject">What was refused, named the way the metadata names it.</param>
/// <param name="Reason">Why, in terms a person editing the metadata can act on.</param>
internal sealed record VocabularyIssue(VocabularyIssueKind Kind, string Subject, string Reason)
{
	/// <inheritdoc />
	public override string ToString() => $"{Subject}: {Reason}";
}

/// <summary>
/// The metadata, resolved into the quantities and operators it describes, with everything it
/// cannot honour separated out rather than silently dropped.
/// </summary>
/// <remarks>
/// The separation is the point. A relationship is a claim -- <c>Force * Length -&gt; Torque</c> --
/// and the exponents are what check it. A claim the exponents contradict is refused by name, with
/// the two dimensions written out, because the alternative is an operator that compiles and
/// computes the wrong physics.
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
/// <para>
/// This is shared source rather than a project of its own, compiled into both the C# source
/// generator and the C++ projection. See the README beside it for why, and for what that costs.
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
	/// <param name="dimensions">The dimensions, as each reader projects them.</param>
	/// <returns>The vocabulary, and everything refused.</returns>
	internal static QuantityVocabulary FromDimensions(IReadOnlyList<DimensionDeclaration> dimensions)
	{
		List<QuantityType> types = [];
		List<VocabularyIssue> refused = [];

		Dictionary<string, DimensionVector> byDimensionName = [];
		Dictionary<string, DimensionDeclaration> declarations = [];

		foreach (DimensionDeclaration dimension in dimensions)
		{
			DimensionVector exponents = DimensionVector.FromFormula(dimension.Formula);
			byDimensionName[dimension.Name] = exponents;
			declarations[dimension.Name] = dimension;

			FormDeclaration? magnitude = dimension.Form(0);
			if (magnitude is null || string.IsNullOrEmpty(magnitude.Base))
			{
				// Every other form reports its length through this one, so a dimension without it
				// has nothing for the vector forms to answer with either.
				refused.Add(new VocabularyIssue(
					VocabularyIssueKind.NoMagnitudeForm,
					dimension.Name,
					"has no vector0 form, so it has no magnitude type to generate."));
				continue;
			}

			for (int form = 0; form < DimensionDeclaration.FormCount; form++)
			{
				types.AddRange(Declared(dimension, form, exponents, magnitude.Base));
			}
		}

		List<QuantityRelationship> relationships =
			[.. ResolveRelationships(dimensions, byDimensionName, declarations, refused)];

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
		DimensionDeclaration dimension,
		int form,
		DimensionVector exponents,
		string magnitudeType)
	{
		FormDeclaration? declared = dimension.Form(form);
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

		foreach (OverloadDeclaration overload in declared.Overloads)
		{
			yield return new QuantityType(
				overload.Name,
				overload.Description,
				exponents,
				Refines: declared.Base,
				BoundOf(form, overload),
				form,
				magnitudeType);
		}
	}

	/// <summary>
	/// How far down an overload is bounded.
	/// </summary>
	/// <param name="form">How many components the form has.</param>
	/// <param name="overload">The overload being declared.</param>
	/// <returns>Its lower bound.</returns>
	/// <remarks>
	/// Only the magnitude form is bounded below at all, which is the whole reason the forms are
	/// separate types: a component of a vector is signed whatever the overload asked for, so a
	/// stricter floor declared on one applies to the magnitude and to nothing else.
	/// </remarks>
	private static Magnitude BoundOf(int form, OverloadDeclaration overload)
	{
		if (form != 0)
		{
			return Magnitude.Signed;
		}

		return overload.IsStrictlyPositive ? Magnitude.Positive : Magnitude.NonNegative;
	}

	private static string Describe(string dimension, int form) => form switch
	{
		0 => $"The magnitude of {Article(dimension)} {Spaced(dimension)}.",
		1 => $"A signed {Spaced(dimension)} along one axis.",
		_ => $"{CapitalisedArticle(dimension)} {Spaced(dimension)} in {form.ToString(CultureInfo.InvariantCulture)} dimensions.",
	};

	private static IEnumerable<QuantityRelationship> ResolveRelationships(
		IReadOnlyList<DimensionDeclaration> dimensions,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, DimensionDeclaration> declarations,
		List<VocabularyIssue> refused)
	{
		foreach (DimensionDeclaration dimension in dimensions)
		{
			IEnumerable<(RelationshipDeclaration Declared, RelationshipKind Kind)> declared =
			[
				.. dimension.Integrals.Select(relationship => (relationship, RelationshipKind.Product)),
				.. dimension.Derivatives.Select(relationship => (relationship, RelationshipKind.Quotient)),
				.. dimension.DotProducts.Select(relationship => (relationship, RelationshipKind.Dot)),
				.. dimension.CrossProducts.Select(relationship => (relationship, RelationshipKind.Cross)),
			];

			foreach ((RelationshipDeclaration relationship, RelationshipKind kind) in declared)
			{
				foreach (QuantityRelationship resolved in
					Resolve(dimension, relationship, kind, byDimensionName, declarations, refused))
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
		DimensionDeclaration dimension,
		RelationshipDeclaration relationship,
		RelationshipKind kind,
		IReadOnlyDictionary<string, DimensionVector> byDimensionName,
		IReadOnlyDictionary<string, DimensionDeclaration> declarations,
		List<VocabularyIssue> refused)
	{
		string subject = Subject(dimension.Name, relationship, kind);

		foreach (string named in (string[])[relationship.Other, relationship.Result])
		{
			if (!byDimensionName.ContainsKey(named))
			{
				// The same gap SEM001 reports.
				refused.Add(new VocabularyIssue(
					VocabularyIssueKind.UnknownDimension,
					subject,
					$"names '{named}', which dimensions.json does not declare."));
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
				VocabularyIssueKind.NotDimensionallyTrue,
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
				VocabularyIssueKind.SignedResultInMagnitudeForm,
				subject,
				$"reduces to a signed value -- two vectors that oppose each other give a negative one -- and '{relationship.Result}' declares only a magnitude form, which cannot be negative. A vector1 form on it is what would let this be generated."));
			return [];
		}

		return At(subject, dimension.Name, relationship, kind, declarations, refused);
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
	/// <c>SEM003</c> makes.
	/// </para>
	/// </remarks>
	private static List<QuantityRelationship> At(
		string subject,
		string self,
		RelationshipDeclaration relationship,
		RelationshipKind kind,
		IReadOnlyDictionary<string, DimensionDeclaration> declarations,
		List<VocabularyIssue> refused)
	{
		bool crossed = kind == RelationshipKind.Cross;
		IReadOnlyList<int> wanted = Wanted(relationship, crossed);

		List<QuantityRelationship> emitted = [];

		foreach (int form in wanted)
		{
			// The right operand of a cross product has the same shape as the left; of a product or
			// a quotient it is the magnitude the vector is scaled by.
			string? leftType = Base(declarations, self, form);
			string? rightType = Base(declarations, relationship.Other, crossed ? form : 0);
			string? resultType = Base(declarations, relationship.Result, form);

			if (leftType is null || rightType is null || resultType is null)
			{
				if (relationship.Forms.Count > 0)
				{
					// The same gap SEM003 reports: a form asked for by name that one of the
					// participants does not have.
					refused.Add(new VocabularyIssue(
						VocabularyIssueKind.MissingVectorForm,
						subject,
						$"is declared at vector{form.ToString(CultureInfo.InvariantCulture)}, which {Missing(declarations, form, self, relationship, crossed)} does not declare."));
				}

				continue;
			}

			emitted.Add(new QuantityRelationship(leftType, rightType, resultType, kind, form));
		}

		return emitted;
	}

	/// <summary>
	/// The forms a relationship asks to be emitted at.
	/// </summary>
	/// <param name="relationship">The declared relationship.</param>
	/// <param name="crossed">Whether it is a cross product.</param>
	/// <returns>The forms, in the order they should be walked.</returns>
	/// <remarks>
	/// An explicit list is taken as it stands. With none, a cross product defaults to three
	/// components and nothing else, because that is where a cross product exists; everything else
	/// defaults to every form and lets the participants decide which of them it reaches.
	/// </remarks>
	private static IReadOnlyList<int> Wanted(RelationshipDeclaration relationship, bool crossed)
	{
		if (relationship.Forms.Count > 0)
		{
			return [.. relationship.Forms];
		}

		return crossed ? [3] : [.. Enumerable.Range(0, DimensionDeclaration.FormCount)];
	}

	private static string Missing(
		IReadOnlyDictionary<string, DimensionDeclaration> declarations,
		int form,
		string self,
		RelationshipDeclaration relationship,
		bool crossed)
	{
		IEnumerable<string> participants = crossed
			? [self, relationship.Other, relationship.Result]
			: [self, relationship.Result];

		return string.Join(" and ", participants.Where(named => Base(declarations, named, form) is null));
	}

	private static string? Base(IReadOnlyDictionary<string, DimensionDeclaration> declarations, string dimension, int form)
	{
		string? name = declarations.TryGetValue(dimension, out DimensionDeclaration? declared)
			? declared.Form(form)?.Base
			: null;
		return string.IsNullOrEmpty(name) ? null : name;
	}

	private static string Subject(string self, RelationshipDeclaration relationship, RelationshipKind kind) => kind switch
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

	/// <summary>
	/// Whether a dimension's name takes "an" rather than "a".
	/// </summary>
	/// <remarks>
	/// A pattern over the character rather than a lookup in a string, which is what keeps this
	/// inside netstandard2.0's surface: <c>string.Contains(char)</c> arrived later, and the
	/// <c>IndexOf</c> that netstandard2.0 does have is what CA2249 objects to. Dimension names are
	/// PascalCase, so only the upper-case vowels can appear first.
	/// </remarks>
	private static bool StartsWithVowel(string name) =>
		name.Length > 0 && name[0] is 'A' or 'E' or 'I' or 'O' or 'U';

	private static string Article(string name) => StartsWithVowel(name) ? "an" : "a";

	private static string CapitalisedArticle(string name) => StartsWithVowel(name) ? "An" : "A";
}
