// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators;
using global::ktsu.SourceGeneratorToolkit.Testing;

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

	/// <summary>
	/// Three dimensions that make a real cross product: <c>Length x Force -&gt; Torque</c>, whose
	/// exponents work out — <c>L</c> times <c>L M T⁻²</c> is <c>L² M T⁻²</c>.
	/// </summary>
	/// <param name="length">The vector forms Length declares.</param>
	/// <param name="force">The vector forms Force declares.</param>
	/// <param name="torque">The vector forms Torque declares.</param>
	/// <returns>The document, with the cross product declared on Length at form 3.</returns>
	/// <remarks>
	/// The physics has to be right for this to test what its name says. A relationship the
	/// exponents contradict is refused for that reason and never reaches the question of which
	/// forms its participants declare, so a fixture that multiplied two lengths into a length
	/// would report SEM008 and never SEM003, whatever forms it asked for.
	/// <para>
	/// Each participant's forms are a parameter because which one is short of the form is the
	/// whole of what these tests vary: the relationship names three dimensions and the diagnostic
	/// has to name the right one of them.
	/// </para>
	/// </remarks>
	private static string CrossProductDocument(string length, string force, string torque) =>
		$$"""
		{
		  "physicalDimensions": [
		    {
		      "name": "Length",
		      "symbol": "L",
		      "dimensionalFormula": { "length": 1 },
		      "availableUnits": [ "Meter" ],
		      "quantities": { {{length}} },
		      "crossProducts": [ { "other": "Force", "result": "Torque", "forms": [ 3 ] } ]
		    },
		    {
		      "name": "Force",
		      "symbol": "F",
		      "dimensionalFormula": { "length": 1, "mass": 1, "time": -2 },
		      "availableUnits": [ "Newton" ],
		      "quantities": { {{force}} }
		    },
		    {
		      "name": "Torque",
		      "symbol": "M",
		      "dimensionalFormula": { "length": 2, "mass": 1, "time": -2 },
		      "availableUnits": [ "NewtonMeter" ],
		      "quantities": { {{torque}} }
		    }
		  ]
		}
		""";

	/// <summary>A dimension that declares a magnitude form and nothing else.</summary>
	private static string MagnitudeOnly(string name) => $$"""

		      "vector0": { "base": "{{name}}" }
		""";

	/// <summary>A dimension that declares a magnitude form and a three-component one.</summary>
	private static string WithVector3(string name) => $$"""

		      "vector0": { "base": "{{name}}" }, "vector3": { "base": "{{name}}3D" }
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

	/// <summary>
	/// Asserts that SEM003 was reported and that it names the participant that is short of the
	/// form, rather than one of the other two.
	/// </summary>
	/// <param name="diagnostics">Everything the generator reported.</param>
	/// <param name="expected">The dimension that does not declare the form.</param>
	/// <remarks>
	/// On the quoted clause rather than on the bare name, because all three participants are named
	/// in the message either way — the field path spells the relationship out — so asserting that
	/// the name appears somewhere would pass for a diagnostic blaming the wrong one.
	/// </remarks>
	private static void AssertNamesTheDimensionMissingTheForm(IReadOnlyList<Diagnostic> diagnostics, string expected)
	{
		AssertReports(diagnostics, "SEM003");

		Assert.Contains(
			$"but '{expected}' does not declare that form",
			diagnostics.First(candidate => candidate.Id == "SEM003").GetMessage(),
			"SEM003 named the wrong participant.");
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

	/// <remarks>
	/// The first of the three participants. Length declares a magnitude form and nothing else, so
	/// the cross product it declares at V3 cannot be honoured by the dimension declaring it.
	/// </remarks>
	[TestMethod]
	public void Sem003_IsReportedWhenARelationshipRequestsAnUndeclaredForm()
	{
		string metadata = CrossProductDocument(
			MagnitudeOnly("Length"),
			WithVector3("Force"),
			WithVector3("Torque"));

		AssertNamesTheDimensionMissingTheForm(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "Length");
	}

	[TestMethod]
	public void Sem003_PointsAtTheRelationshipRatherThanAtNothing()
	{
		string metadata = CrossProductDocument(
			MagnitudeOnly("Length"),
			WithVector3("Force"),
			WithVector3("Torque"));

		// Not the bare name: "Force" is spelled correctly and appears as a dimension of its own
		// further down the file. The location has to be the relationship's own "other".
		AssertPointsAt(
			metadata,
			Run(metadata, new QuantitiesGenerator(), "dimensions.json"),
			"SEM003",
			"\"other\": \"Force\"");
	}

	/// <remarks>
	/// The second participant: Length has a vector3 and Force does not, so the cross product
	/// cannot be honoured at form 3 — and the diagnostic has to name Force rather than Length.
	/// </remarks>
	[TestMethod]
	public void Sem003_NamesTheOtherParticipantWhenItIsTheOneMissingTheForm()
	{
		string metadata = CrossProductDocument(
			WithVector3("Length"),
			MagnitudeOnly("Force"),
			WithVector3("Torque"));

		AssertNamesTheDimensionMissingTheForm(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "Force");
	}

	/// <remarks>
	/// The third participant. A cross product also needs its <em>result</em> to have the form:
	/// Length x Force -> Torque at V3 fails when Torque has no V3, not because either operand is
	/// missing one.
	/// </remarks>
	[TestMethod]
	public void Sem003_NamesTheResultWhenItIsTheOneMissingTheForm()
	{
		string metadata = CrossProductDocument(
			WithVector3("Length"),
			WithVector3("Force"),
			MagnitudeOnly("Torque"));

		AssertNamesTheDimensionMissingTheForm(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "Torque");
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

	/// <summary>
	/// The real metadata reports nothing except the relationships it is already known to get wrong.
	/// </summary>
	/// <remarks>
	/// This asserted nothing at all until the dimensional check moved into the shared vocabulary and
	/// the C# generator started running it. Five relationships fail it, all five documented in
	/// <c>CLAUDE.md</c> and none of them fixable by spelling, so SEM008 is excluded here rather than
	/// the invariant being abandoned: everything else must still be silent.
	/// <para>
	/// Which five is not this test's business but
	/// <see cref="UnkeepableRelationshipTests"/>'s, which pins them exactly. Excluding the
	/// identifier here without that would let a sixth through.
	/// </para>
	/// </remarks>
	[TestMethod]
	public void TheRealMetadataReportsNothingUnexpected()
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
			IEnumerable<Diagnostic> unexpected =
				result.Diagnostics.Where(diagnostic => diagnostic.Id != "SEM008");

			Assert.IsEmpty(
				unexpected,
				$"{generator.GetType().Name}: {string.Join("; ", unexpected.Select(d => $"{d.Id}: {d.GetMessage()}"))}");
		}
	}
}
