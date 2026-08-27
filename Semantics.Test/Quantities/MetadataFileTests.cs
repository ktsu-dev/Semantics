// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using global::Semantics.SourceGenerators.CodeGen;

/// <summary>
/// Covers <see cref="MetadataFile.FindLocation(string, string)"/> directly.
/// </summary>
/// <remarks>
/// The generator tests reach the happy path of both overloads, but not what happens when a search
/// comes up empty — and the fallbacks are the whole reason the scoped overload is safe to use. A
/// metadata layout it does not recognise has to degrade to the unscoped search and then to
/// <see cref="Location.None"/>, never throw and never point somewhere arbitrary.
/// </remarks>
[TestClass]
public class MetadataFileTests
{
	private const string Document =
		"""
		{
		  "physicalDimensions": [
		    { "name": "Length", "symbol": "L", "integrals": [ { "other": "Time" } ] },
		    { "name": "Time", "symbol": "T", "integrals": [ { "other": "Length" } ] }
		  ]
		}
		""";

	private static MetadataFile File(string text = Document) =>
		new("dimensions.json", text, SourceText.From(text), "/metadata/dimensions.json");

	private static string Covered(Location location) =>
		Document.Substring(location.SourceSpan.Start, location.SourceSpan.Length);

	[TestMethod]
	public void TheScopedSearchStartsAtItsAnchorRatherThanAtTheTopOfTheFile()
	{
		// "Length" appears first as this dimension's own name, and again as Time's integral. The
		// second one is the one an SEM003 about Time is talking about.
		Location location = File().FindLocation("\"name\": \"Time\"", "\"other\": \"Length\"");

		Assert.AreEqual("\"other\": \"Length\"", Covered(location));
		Assert.IsTrue(
			location.SourceSpan.Start > Document.IndexOf("\"name\": \"Time\"", StringComparison.Ordinal),
			"The scoped search must land after its anchor, not before it.");
	}

	[TestMethod]
	public void AMissingAnchorFallsBackToAnUnscopedSearch()
	{
		Location location = File().FindLocation("\"name\": \"Nonexistent\"", "symbol");

		Assert.AreEqual(
			Document.IndexOf("symbol", StringComparison.Ordinal),
			location.SourceSpan.Start,
			"An unrecognised anchor should degrade to the unscoped search, not to nothing.");
	}

	[TestMethod]
	public void ANeedleThatOnlyAppearsBeforeTheAnchorFallsBackToAnUnscopedSearch()
	{
		// "symbol": "L" is in the first entry only, before the Time anchor.
		Location location = File().FindLocation("\"name\": \"Time\"", "\"symbol\": \"L\"");

		Assert.AreEqual("\"symbol\": \"L\"", Covered(location));
	}

	[TestMethod]
	public void AnEmptyAnchorIsAnUnscopedSearch()
	{
		Location location = File().FindLocation("", "symbol");

		Assert.AreEqual(Document.IndexOf("symbol", StringComparison.Ordinal), location.SourceSpan.Start);
	}

	[TestMethod]
	public void AnEmptyNeedleHasNoLocation()
	{
		Assert.AreEqual(Location.None, File().FindLocation("\"name\": \"Time\"", ""));
	}

	[TestMethod]
	public void ANeedleThatIsNowhereInTheFileHasNoLocation()
	{
		Assert.AreEqual(Location.None, File().FindLocation("\"name\": \"Time\"", "nowhere"));
		Assert.AreEqual(Location.None, File().FindLocation("nowhere"));
	}

	/// <remarks>
	/// An <c>AdditionalText</c> whose <c>GetText</c> returned null. There is nothing to build a
	/// location against, so both overloads have to say so rather than throw.
	/// </remarks>
	[TestMethod]
	public void AFileWithNoSourceTextHasNoLocations()
	{
		MetadataFile file = new("dimensions.json", Document, null, "/metadata/dimensions.json");

		Assert.AreEqual(Location.None, file.FindLocation("symbol"));
		Assert.AreEqual(Location.None, file.FindLocation("\"name\": \"Time\"", "symbol"));
	}
}
