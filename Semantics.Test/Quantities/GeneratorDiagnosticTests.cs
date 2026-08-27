// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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

	private static IReadOnlyList<Diagnostic> Run(string generatorMetadata, IIncrementalGenerator generator, string fileName) =>
		[.. Harness.Run(generator, new Dictionary<string, string> { [fileName] = generatorMetadata }).Diagnostics];

	private static void AssertReports(IReadOnlyList<Diagnostic> diagnostics, string id)
	{
		Assert.IsTrue(
			diagnostics.Any(diagnostic => diagnostic.Id == id),
			$"Expected {id}. Got: {(diagnostics.Count == 0 ? "no diagnostics" : string.Join("; ", diagnostics.Select(d => $"{d.Id}: {d.GetMessage()}")))}");
	}

	[TestMethod]
	public void Sem001_IsReportedForARelationshipNamingAnUnknownDimension()
	{
		string metadata = DimensionsDocument(
			relationships: ",\n      \"integrals\": [ { \"other\": \"Tiem\", \"result\": \"Length\" } ]");

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM001");
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
	public void Sem004_IsReportedForAUnitThatUnitsJsonDoesNotDeclare()
	{
		string metadata = DimensionsDocument(availableUnits: "\"Meter\", \"Kilometres\"");

		AssertReports(Run(metadata, new QuantitiesGenerator(), "dimensions.json"), "SEM004");
	}

	[TestMethod]
	public void Sem004_PointsAtWhereTheUnitIsWrittenRatherThanAtNothing()
	{
		string metadata = DimensionsDocument(availableUnits: "\"Meter\", \"Kilometres\"");

		Diagnostic diagnostic = Run(metadata, new QuantitiesGenerator(), "dimensions.json")
			.First(candidate => candidate.Id == "SEM004");

		Assert.AreNotEqual(
			Location.None,
			diagnostic.Location,
			"A warning about a name in a large JSON file is only actionable if it says where the name is.");
		Assert.EndsWith("dimensions.json", diagnostic.Location.GetLineSpan().Path);
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
