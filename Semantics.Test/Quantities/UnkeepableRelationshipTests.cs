// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Collections.Generic;
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
/// ktsu.Sdk builds warnings as errors and all five are outstanding for reasons that are not
/// spelling: three are the <c>r x F</c> versus <c>tau . theta</c> contradiction and no assignment of
/// angle exponents satisfies both, one is a metadata bug whose fix is a physics call, and one needs
/// a <c>vector1</c> form on <c>Energy</c>.
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
		"Sensitivity * Pressure -> ElectricPotential",
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
		string json = File.ReadAllText(
			Path.Combine(AppContext.BaseDirectory, "GeneratorMetadata", "dimensions.json"));

		DimensionsMetadata metadata = JsonSerializer.Deserialize<DimensionsMetadata>(json, ReaderOptions)
			?? throw new InvalidOperationException("dimensions.json is empty.");

		return QuantityVocabulary.FromDimensions(metadata.ToDeclarations());
	}

	/// <summary>
	/// The unkeepable relationships are exactly the five that are documented.
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
	/// The pre-existing metadata bug is among them, named rather than merely counted.
	/// </summary>
	/// <remarks>
	/// <c>Sensitivity</c> is declared as A/Pa while the relationship treats it as V/Pa. It is the
	/// one refusal unrelated to angle, and the one that had been shipping as a working C# operator
	/// computing the wrong physics before the C# side gained this check.
	/// </remarks>
	[TestMethod]
	public void TheSensitivityBugIsCaughtWithBothDimensionsWrittenOut()
	{
		VocabularyIssue issue = Vocabulary().Refused.Single(refused =>
			refused.Subject == "Sensitivity * Pressure -> ElectricPotential");

		Assert.AreEqual(VocabularyIssueKind.NotDimensionallyTrue, issue.Kind);
		StringAssert.Contains(issue.Reason, "L⁻² I", StringComparison.Ordinal);
		StringAssert.Contains(issue.Reason, "L² M T⁻³ I⁻¹", StringComparison.Ordinal);
	}

	/// <summary>
	/// The refusals the C# generator already has diagnostics for are not counted among these, so
	/// SEM008 never says what SEM001 or SEM003 has already said.
	/// </summary>
	[TestMethod]
	public void TheKindsWithTheirOwnDiagnosticsAreKeptSeparate()
	{
		IReadOnlyList<VocabularyIssue> refused = Vocabulary().Refused;

		Assert.IsFalse(
			refused.Any(issue => issue.Kind is VocabularyIssueKind.UnknownDimension),
			"an unknown dimension is SEM001's to report, and the metadata should have none.");

		Assert.IsFalse(
			refused.Any(issue => issue.Kind is VocabularyIssueKind.NoMagnitudeForm),
			"every dimension should declare a magnitude form for the other forms to measure against.");
	}
}
