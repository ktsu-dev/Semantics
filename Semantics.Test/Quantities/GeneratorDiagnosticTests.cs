// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators;

/// <summary>
/// Proves each generator diagnostic fires on the input it is meant to catch.
/// </summary>
/// <remarks>
/// The existing generator tests only asserted that the real metadata produces <em>no</em>
/// diagnostics. That leaves the diagnostics themselves untested: SEM001 through SEM005 could stop
/// firing entirely and every test would still pass, which for a set of warnings whose whole job is
/// to catch typos in a large JSON file is the wrong way round.
/// </remarks>
[TestClass]
public class GeneratorDiagnosticTests
{
	private static string MetadataDirectory => Path.Combine(AppContext.BaseDirectory, "GeneratorMetadata");

	private static GeneratorHarness Harness => new(MetadataDirectory);

	/// <summary>
	/// A dimensions document small enough to reason about, with one hook for the test to break.
	/// </summary>
	/// <param name="relationships">Relationship JSON to splice into the Length dimension.</param>
	/// <param name="availableUnits">The units Length declares.</param>
	/// <returns>The document.</returns>
	private static string DimensionsDocument(string relationships = "", string availableUnits = "\"Meter\"") =>
		$$"""
		{
		  "physicalDimensions": [
		    {
		      "name": "Length",
		      "symbol": "L",
		      "dimensionalFormula": { "length": 1 },
		      "availableUnits": [ {{availableUnits}} ],
		      "quantities": { "vector0": { "base": "Length" }, "vector3": { "base": "Displacement3D" } }{{relationships}}
		    }
		  ]
		}
		""";

	/// <summary>
	/// Two dimensions, so a relationship's <c>other</c> and <c>result</c> can name different things
	/// and a diagnostic about one can be told from a diagnostic about the other.
	/// </summary>
	/// <param name="relationships">Relationship JSON to splice into the Length dimension.</param>
	/// <returns>The document.</returns>
	/// <remarks>
	/// Time deliberately declares only <c>vector0</c>. Length declares <c>vector0</c> and
	/// <c>vector3</c>, so a relationship between them at form 3 is honourable for Length and not for
	/// Time — which is what SEM003 is for.
	/// </remarks>
	private static string TwoDimensionsDocument(string relationships) =>
		$$"""
		{
		  "physicalDimensions": [
		    {
		      "name": "Length",
		      "symbol": "L",
		      "dimensionalFormula": { "length": 1 },
		      "availableUnits": [ "Meter" ],
		      "quantities": { "vector0": { "base": "Length" }, "vector3": { "base": "Displacement3D" } }{{relationships}}
		    },
		    {
		      "name": "Time",
		      "symbol": "T",
		      "dimensionalFormula": { "time": 1 },
		      "availableUnits": [ "Second" ],
		      "quantities": { "vector0": { "base": "Duration" } }
		    }
		  ]
		}
		""";

	private static IReadOnlyList<Diagnostic> Run(string generatorMetadata, IIncrementalGenerator generator, string fileName) =>
		[.. Harness.Run(generator, new Dictionary<string, string> { [fileName] = generatorMetadata }).Diagnostics];

	private static void AssertReports(IReadOnlyList<Diagnostic> diagnostics, string id)
	{
		Assert.IsTrue(
			diagnostics.Any(diagnostic => diagnostic.Id == id),
			$"Expected {id}. Got: {(diagnostics.Count == 0 ? "no diagnostics" : string.Join("; ", diagnostics.Select(d => $"{d.Id}: {d.GetMessage()}")))}");
	}

	/// <summary>
	/// Asserts that a diagnostic points at a specific piece of text in the metadata it was reported
	/// against.
	/// </summary>
	/// <param name="metadata">The metadata document the generator was run over.</param>
	/// <param name="diagnostics">Everything the generator reported.</param>
	/// <param name="id">The diagnostic to look for.</param>
	/// <param name="expected">The text the location should cover.</param>
	/// <remarks>
	/// Asserting only that the location is not <see cref="Location.None"/> would pass for a location
	/// pointing at the wrong entry, which is the failure mode that actually matters: every name in a
	/// relationship is spelled correctly somewhere else in the file, so an unscoped search lands
	/// plausibly and uselessly far from the mistake. Reading the covered text back proves it landed
	/// on the right one.
	/// </remarks>
	private static void AssertPointsAt(string metadata, IReadOnlyList<Diagnostic> diagnostics, string id, string expected)
	{
		AssertReports(diagnostics, id);
		Diagnostic diagnostic = diagnostics.First(candidate => candidate.Id == id);

		Assert.AreNotEqual(
			Location.None,
			diagnostic.Location,
			$"{id} is only actionable if it says where in the metadata the problem is.");
		Assert.EndsWith("dimensions.json", diagnostic.Location.GetLineSpan().Path);

		TextSpan span = diagnostic.Location.SourceSpan;
		Assert.AreEqual(
			expected,
			metadata.Substring(span.Start, span.Length),
			$"{id} pointed at the wrong place in the metadata.");
	}

	[TestMethod]
	public void Sem001_IsReportedForARelationshipNamingAnUnknownDimension()
	{
		string metadata = DimensionsDocument(
			relationships: ",\n      \"integrals\": [ { \"other\": \"Tiem\", \"result\": \"Length\" } ]");

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM001");
	}

	[TestMethod]
	public void Sem001_PointsAtTheMisspelledNameRatherThanAtNothing()
	{
		string metadata = DimensionsDocument(
			relationships: ",\n      \"integrals\": [ { \"other\": \"Tiem\", \"result\": \"Length\" } ]");

		AssertPointsAt(metadata, Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM001", "Tiem");
	}

	/// <summary>
	/// SEM001 fires for a bad name in any relationship kind, in either field.
	/// </summary>
	/// <param name="kind">The relationship array to put the bad name in.</param>
	/// <param name="field">Which of <c>other</c>/<c>result</c> is wrong.</param>
	/// <remarks>
	/// Each of these is a separate report site with its own field-path message, and only
	/// <c>integrals.other</c> had a test. A rename that dropped one of the other seven would not
	/// have failed anything.
	/// </remarks>
	[TestMethod]
	[DataRow("integrals", "other")]
	[DataRow("integrals", "result")]
	[DataRow("derivatives", "other")]
	[DataRow("derivatives", "result")]
	[DataRow("dotProducts", "other")]
	[DataRow("dotProducts", "result")]
	[DataRow("crossProducts", "other")]
	[DataRow("crossProducts", "result")]
	public void Sem001_IsReportedForAnUnknownNameInAnyRelationshipField(string kind, string field)
	{
		string other = field == "other" ? "Nonexistent" : "Time";
		string result = field == "result" ? "Nonexistent" : "Time";
		string metadata = TwoDimensionsDocument(
			$",\n      \"{kind}\": [ {{ \"other\": \"{other}\", \"result\": \"{result}\" }} ]");

		IReadOnlyList<Diagnostic> diagnostics = Run(metadata, new QuantitiesGenerator(), "dimensions.json");

		AssertPointsAt(metadata, diagnostics, "SEM001", "Nonexistent");
		Assert.Contains(
			$"{kind}[{other} -> {result}].{field}",
			diagnostics.First(candidate => candidate.Id == "SEM001").GetMessage(),
			"The message should name the field path the bad name is written at.");
	}

	[TestMethod]
	public void Sem002_IsReportedForADimensionMissingItsSymbol()
	{
		string metadata =
			"""
			{
			  "physicalDimensions": [
			    {
			      "name": "Length",
			      "availableUnits": [ "Meter" ],
			      "quantities": { "vector0": { "base": "Length" } }
			    }
			  ]
			}
			""";

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM002");
	}

	[TestMethod]
	public void Sem003_IsReportedWhenARelationshipRequestsAnUndeclaredForm()
	{
		// Length declares vector0 and vector3; asking for the cross product at V2 cannot be honoured.
		string metadata = DimensionsDocument(
			relationships: ",\n      \"crossProducts\": [ { \"other\": \"Length\", \"result\": \"Length\", \"forms\": [ 2 ] } ]");

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM003");
	}

	[TestMethod]
	public void Sem003_PointsAtTheRelationshipRatherThanAtNothing()
	{
		string metadata = DimensionsDocument(
			relationships: ",\n      \"crossProducts\": [ { \"other\": \"Length\", \"result\": \"Length\", \"forms\": [ 2 ] } ]");

		// Not the bare name: "Length" is spelled correctly and appears several times before the
		// relationship that is wrong. The location has to be the relationship's own "other".
		AssertPointsAt(
			metadata,
			Run(metadata, new QuantitiesGenerator(), "dimensions.json"),
			"SEM003",
			"\"other\": \"Length\"");
	}

	/// <remarks>
	/// The self branch is covered above. This is the second participant: Length has a vector3 and
	/// Time does not, so the cross product cannot be honoured at form 3 — and the diagnostic has to
	/// name Time rather than Length.
	/// </remarks>
	[TestMethod]
	public void Sem003_NamesTheOtherParticipantWhenItIsTheOneMissingTheForm()
	{
		string metadata = TwoDimensionsDocument(
			",\n      \"crossProducts\": [ { \"other\": \"Time\", \"result\": \"Length\", \"forms\": [ 3 ] } ]");

		IReadOnlyList<Diagnostic> diagnostics = Run(metadata, new QuantitiesGenerator(), "dimensions.json");

		AssertReports(diagnostics, "SEM003");
		Assert.Contains("Time", diagnostics.First(candidate => candidate.Id == "SEM003").GetMessage());
	}

	/// <remarks>
	/// The third participant. A cross product also needs its <em>result</em> to have the form —
	/// Force x Length -> Torque at V2 fails because Torque has no V2, not because either operand
	/// is missing one.
	/// </remarks>
	[TestMethod]
	public void Sem003_NamesTheResultWhenItIsTheOneMissingTheForm()
	{
		string metadata = TwoDimensionsDocument(
			",\n      \"crossProducts\": [ { \"other\": \"Length\", \"result\": \"Time\", \"forms\": [ 3 ] } ]");

		IReadOnlyList<Diagnostic> diagnostics = Run(metadata, new QuantitiesGenerator(), "dimensions.json");

		AssertReports(diagnostics, "SEM003");
		Assert.Contains("Time", diagnostics.First(candidate => candidate.Id == "SEM003").GetMessage());
	}

	[TestMethod]
	public void Sem004_IsReportedForAUnitThatUnitsJsonDoesNotDeclare()
	{
		string metadata = DimensionsDocument(availableUnits: "\"Meter\", \"Kilometres\"");

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM004");
	}

	[TestMethod]
	public void Sem004_PointsAtWhereTheUnitIsWrittenRatherThanAtNothing()
	{
		string metadata = DimensionsDocument(availableUnits: "\"Meter\", \"Kilometres\"");

		AssertPointsAt(metadata, Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM004", "Kilometres");
	}

	[TestMethod]
	public void Sem005_IsReportedForADuplicateLogarithmicScale()
	{
		string metadata =
			"""
			{
			  "logarithmicScales": [
			    { "name": "Decibels", "description": "A.", "base": 10, "multiplier": 20, "reference": 1 },
			    { "name": "Decibels", "description": "B.", "base": 10, "multiplier": 20, "reference": 1 }
			  ]
			}
			""";

		AssertReports(Run(metadata, new LogarithmicScalesGenerator(), "logarithmic.json"), "SEM005");
	}

	[TestMethod]
	public void Sem006_IsReportedWhenAGeneratorsSecondMetadataFileIsMissing()
	{
		// QuantitiesGenerator needs units.json for its non-base-unit conversion factors. Without
		// SEM006 its absence produced no output and no explanation.
		GeneratorRunResult result = Harness.RunWithOnly(new QuantitiesGenerator(), "dimensions.json");

		AssertReports([.. result.Diagnostics], "SEM006");
	}

	[TestMethod]
	public void Sem007_IsReportedForASecondMetadataFileThatIsMalformed()
	{
		// This path used to swallow the JsonException and carry on with an empty unit set, so a
		// malformed units.json silently produced factories with no scale factor.
		IReadOnlyList<Diagnostic> diagnostics = Run("{ not valid json", new QuantitiesGenerator(), "units.json");

		AssertReports(diagnostics, "SEM007");
	}

	[TestMethod]
	public void TheRealMetadataReportsNothing()
	{
		List<IIncrementalGenerator> generators =
		[
			new ConversionsGenerator(),
			new DimensionsGenerator(),
			new LogarithmicScalesGenerator(),
			new MagnitudesGenerator(),
			new PhysicalConstantsGenerator(),
			new PrecisionGenerator(),
			new QuantitiesGenerator(),
			new UnitsGenerator(),
		];

		foreach (IIncrementalGenerator generator in generators)
		{
			GeneratorRunResult result = Harness.Run(generator);
			Assert.IsEmpty(
				result.Diagnostics,
				$"{generator.GetType().Name}: {string.Join("; ", result.Diagnostics.Select(d => $"{d.Id}: {d.GetMessage()}"))}");
		}
	}
}
