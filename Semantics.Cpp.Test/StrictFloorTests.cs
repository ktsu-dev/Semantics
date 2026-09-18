// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp.Test;

using System;

using ktsu.Semantics.Cpp;
using ktsu.Semantics.Vocabulary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers which declared constraint opts a V0 overload into the stricter floor, which is the one
/// rule about <c>physicalConstraints</c> that both projections of <c>dimensions.json</c> read.
/// </summary>
/// <remarks>
/// The rule is that <c>minExclusive: "0"</c> <em>specifically</em> opts in, and it is the
/// documented one: CLAUDE.md's design decision #4 says the strict guard is what that value asks
/// for. A reader that tested whether a constraints object was present instead answered the same
/// for the three sites the metadata declares today -- <c>Wavelength</c>, <c>Period</c> and
/// <c>HalfLife</c>, all <c>{ "minExclusive": "0" }</c> -- and would have answered differently for
/// the first constraint of any other kind, giving one declared quantity two domains depending on
/// which language it was generated into. That is ktsu-dev/Semantics#218.
/// <para>
/// So these are driven by a metadata document written for the purpose rather than by the real one:
/// the case that separates the two rules is a constraint the real file does not declare yet, and
/// the point is to fail on the day it does.
/// </para>
/// </remarks>
[TestClass]
public sealed class StrictFloorTests
{
	/// <summary>
	/// One dimension, whose magnitude form carries an overload per way of declaring -- or not
	/// declaring -- a floor. Only <c>ZeroFloor</c> asks for the strict one.
	/// </summary>
	private const string Metadata = """
		{
		  "physicalDimensions": [
		    {
		      "name": "Length",
		      "dimensionalFormula": { "length": 1 },
		      "quantities": {
		        "vector0": {
		          "base": "Length",
		          "overloads": [
		            { "name": "NoConstraints", "description": "Declares no constraints at all." },
		            { "name": "EmptyConstraints", "description": "Declares a constraints object with no floor in it.", "physicalConstraints": { } },
		            { "name": "OtherFloor", "description": "Declares a floor that is not zero.", "physicalConstraints": { "minExclusive": "1" } },
		            { "name": "ZeroFloor", "description": "Declares the floor that opts into the strict guard.", "physicalConstraints": { "minExclusive": "0" } }
		          ]
		        }
		      }
		    }
		  ]
		}
		""";

	private static CppQuantityOutput Output { get; } = new CppQuantityGenerator(
		new CppQuantityOptions { Namespace = "holo" }).Generate(QuantityMetadata.Parse(Metadata));

	/// <summary>
	/// The value the rule is written around gets the strict comparison, which is the behaviour the
	/// three real constraint sites rely on.
	/// </summary>
	[TestMethod]
	public void GuardsAZeroFloorStrictly() =>
		Assert.Contains("assert(value.count() > 0", Output.Files["ZeroFloor.hpp"], StringComparison.Ordinal);

	/// <summary>
	/// A constraints object with no floor in it is not an opt-in. <c>MinExclusive</c> defaults to
	/// empty, so a reader testing the object for null rather than reading its value turns the strict
	/// guard on here -- and the C# generator, which reads the value, leaves it off.
	/// </summary>
	[TestMethod]
	public void DoesNotGuardEmptyConstraintsStrictly() =>
		Assert.Contains(
			"assert(value.count() >= 0",
			Output.Files["EmptyConstraints.hpp"],
			StringComparison.Ordinal);

	/// <summary>
	/// Neither is a floor of some other value. Nothing about <c>minExclusive: "1"</c> says a
	/// quantity may not be zero -- it says considerably more than that -- and no guard for it is
	/// emitted, so the strict comparison would be an assertion the metadata never asked for.
	/// </summary>
	[TestMethod]
	public void DoesNotGuardANonZeroFloorStrictly() =>
		Assert.Contains("assert(value.count() >= 0", Output.Files["OtherFloor.hpp"], StringComparison.Ordinal);

	/// <summary>
	/// An overload with no constraints at all keeps its magnitude form's own floor, as does the base
	/// it refines. This is the case the two rules always agreed about.
	/// </summary>
	[TestMethod]
	public void GuardsAnUnconstrainedOverloadAsAMagnitude()
	{
		Assert.Contains("assert(value.count() >= 0", Output.Files["NoConstraints.hpp"], StringComparison.Ordinal);
		Assert.Contains("assert(value.count() >= 0", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// The rule itself, stated once and asked by both readers. Testing it here rather than through
	/// each projection is the point of it being one function: the C# generator and this one cannot
	/// answer differently, because there is no longer a second answer for them to hold.
	/// </summary>
	[TestMethod]
	[DataRow("0", true, "the documented opt-in")]
	[DataRow(null, false, "no constraints object at all")]
	[DataRow("", false, "a constraints object carrying no floor")]
	[DataRow("1", false, "a floor of some other value")]
	[DataRow("0.0", false, "a floor that is zero but is not spelled the way the rule names it")]
	[DataRow(" 0", false, "a floor that would only match if it were trimmed first")]
	public void ReadsTheStrictFloorOffTheValue(string? minExclusive, bool expected, string because) =>
		Assert.AreEqual(
			expected,
			OverloadDeclaration.IsStrictFloor(minExclusive),
			$"minExclusive {minExclusive ?? "(null)"} is {because}.");
}
