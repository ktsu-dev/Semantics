// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using global::ktsu.Semantics.Vocabulary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators.Models;

/// <summary>
/// Pins the relationships in <c>dimensions.json</c> that cannot be kept, to exactly the set that is
/// documented and accepted.
/// </summary>
/// <remarks>
/// <c>SEM008</c> reports these, and <c>Semantics.Quantities.csproj</c> suppresses it, because
/// ktsu.Sdk builds warnings as errors and all four are outstanding for reasons that are not
/// spelling: three are the <c>r x F</c> versus <c>tau . theta</c> contradiction and no assignment of
/// angle exponents satisfies both, and one needs a <c>vector1</c> form on <c>Energy</c>.
/// <para>
/// There were five. <c>Sensitivity * Pressure -> ElectricPotential</c> was the one unrelated to
/// angle, and it is fixed: the dimension said amperes per newton while the unit beside it said
/// <c>VoltPerPascal</c> and the relationship agreed with the unit, so the formula was the one thing
/// that was wrong.
/// </para>
/// <para>
/// A suppression with no floor under it would swallow a sixth, which is what this exists to stop.
/// The assertion is on the exact set rather than on a count, so a relationship that stops being
/// refused fails here too — that would mean somebody fixed one, and the list and the suppression
/// should shrink with it.
/// </para>
/// <para>
/// Driven by the real production metadata, read through the same model the generator reads it with
/// and projected through the same adapter, so this is the generator's own view rather than a
/// restatement of it.
/// </para>
/// </remarks>
[TestClass]
public sealed class UnkeepableRelationshipTests
{
	/// <summary>
	/// Every relationship the metadata declares that the physics will not support, as
	/// <c>CLAUDE.md</c> lists them.
	/// </summary>
	private static readonly string[] Expected =
	[
		"MomentOfInertia * AngularAcceleration -> Torque",
		"MomentOfInertia * AngularVelocity -> AngularMomentum",
		"Torque * AngularDisplacement -> Energy",
		"dot(Force, Length) -> Energy",
	];

	private static readonly JsonSerializerOptions ReaderOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	private static QuantityVocabulary Vocabulary()
	{
		// Join rather than Combine: Combine discards everything before a rooted segment, which is
		// what the quality analyzer objects to, and Join has no such rule. The same call in
		// Semantics.Cpp.Test is already written this way.
		string json = File.ReadAllText(
			Path.Join(AppContext.BaseDirectory, "GeneratorMetadata", "dimensions.json"));

		DimensionsMetadata metadata = JsonSerializer.Deserialize<DimensionsMetadata>(json, ReaderOptions)
			?? throw new InvalidOperationException("dimensions.json is empty.");

		return QuantityVocabulary.FromDimensions(metadata.ToDeclarations());
	}

	/// <summary>
	/// The unkeepable relationships are exactly the four that are documented.
	/// </summary>
	[TestMethod]
	public void TheMetadataDeclaresExactlyTheKnownUnkeepableRelationships()
	{
		string[] unkeepable =
		[
			.. Vocabulary().Refused
				.Where(issue => issue.Kind is VocabularyIssueKind.NotDimensionallyTrue
					or VocabularyIssueKind.SignedResultInMagnitudeForm)
				.Select(issue => issue.Subject)
				.OrderBy(subject => subject, StringComparer.Ordinal),
		];

		Assert.AreSequenceEqual(
			Expected,
			unkeepable,
			"dimensions.json's unkeepable relationships changed. A new one means SEM008 is suppressed "
				+ "over a real problem; one fewer means somebody fixed it, so shrink this list and the "
				+ "NoWarn in Semantics.Quantities.csproj with it.");
	}

	/// <summary>
	/// The relationship that used to be refused is emitted, and means what its unit says.
	/// </summary>
	/// <remarks>
	/// The check that caught it is what says it is fixed: a sensitivity in volts per pascal times a
	/// pressure is a potential, and nothing refuses it now. Asserted through the vocabulary rather
	/// than by multiplying two quantities, because the refusal was never about a value - it was
	/// about whether the exponents on either side of the claim agreed.
	/// </remarks>
	[TestMethod]
	public void TheSensitivityRelationshipIsNoLongerRefused()
	{
		Assert.DoesNotContain(
			"Sensitivity * Pressure -> ElectricPotential",
			Vocabulary().Refused.Select(refused => refused.Subject).ToArray());
	}

	/// <summary>
	/// The refusals the C# generator already has diagnostics for are not counted among these, so
	/// SEM008 never says what SEM001 or SEM003 has already said.
	/// </summary>
	[TestMethod]
	public void TheKindsWithTheirOwnDiagnosticsAreKeptSeparate()
	{
		VocabularyIssueKind[] kinds = [.. Vocabulary().Refused.Select(issue => issue.Kind)];

		Assert.DoesNotContain(
			VocabularyIssueKind.UnknownDimension,
			kinds,
			"an unknown dimension is SEM001's to report, and the metadata should have none.");

		Assert.DoesNotContain(
			VocabularyIssueKind.NoMagnitudeForm,
			kinds,
			"every dimension should declare a magnitude form for the other forms to measure against.");
	}
}
