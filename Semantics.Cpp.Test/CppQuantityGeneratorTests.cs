// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Cpp.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ktsu.Semantics.Cpp;

using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers the C++ projection of the quantity vocabulary, driven by the real metadata.
/// </summary>
[TestClass]
public sealed class CppQuantityGeneratorTests
{
	/// <summary>
	/// Generated once. Every test below reads the same output, because generating it is the
	/// expensive part and none of them change it.
	/// </summary>
	private static CppQuantityOutput Output { get; } = new CppQuantityGenerator(
		new CppQuantityOptions { Namespace = "holo" }).Generate(Metadata());

	private static QuantityMetadata Metadata() =>
		QuantityMetadata.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Metadata", "dimensions.json")));

	/// <summary>
	/// The metadata is read at all, which is the one thing every other test rests on.
	/// </summary>
	[TestMethod]
	public void ReadsTheMetadata()
	{
		QuantityMetadata metadata = Metadata();

		Assert.IsGreaterThan(60, metadata.PhysicalDimensions.Count);
		Assert.IsTrue(
			metadata.PhysicalDimensions.Any(dimension => string.Equals(dimension.Name, "Length", StringComparison.Ordinal)),
			"Length is the dimension everything else is checked against; the file should declare it.");
	}

	/// <summary>
	/// A class per dimension and per named overload, plus the prelude and the two roll-ups.
	/// </summary>
	[TestMethod]
	public void GeneratesAHeaderPerQuantity()
	{
		Assert.Contains("Length.hpp", Output.Files.Keys);
		Assert.Contains("Speed.hpp", Output.Files.Keys);
		Assert.Contains("Weight.hpp", Output.Files.Keys, "an overload is a type of its own, not an alias");
		Assert.Contains("dimension.hpp", Output.Files.Keys);
		Assert.Contains("quantity.hpp", Output.Files.Keys);
		Assert.Contains("relationships.hpp", Output.Files.Keys);
		Assert.Contains("quantities.hpp", Output.Files.Keys);
	}

	/// <summary>
	/// The target's namespace reaches the prelude as well as the generated classes: the prelude is
	/// substituted rather than fixed, which is the one thing about it that is not literal.
	/// </summary>
	[TestMethod]
	public void PutsEverythingInTheTargetsNamespace()
	{
		Assert.Contains("namespace holo", Output.Files["quantity.hpp"], StringComparison.Ordinal);
		Assert.Contains("namespace holo", Output.Files["Length.hpp"], StringComparison.Ordinal);
		Assert.DoesNotContain("@NAMESPACE@", Output.Files["dimension.hpp"], StringComparison.Ordinal);
		Assert.DoesNotContain("@BANNER@", Output.Files["dimension.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// A quantity is a distinct class over a <c>Quantity</c> of its exponents, and the exponents
	/// are written as the significant prefix rather than as eight numbers.
	/// </summary>
	[TestMethod]
	public void WritesADimensionAsItsSignificantPrefix()
	{
		Assert.Contains("using underlying = Quantity<Dimension<1>>;", Output.Files["Length.hpp"], StringComparison.Ordinal);

		// Velocity is L T⁻¹, so the prefix runs to the third axis and stops.
		Assert.Contains("using underlying = Quantity<Dimension<1, 0, -1>>;", Output.Files["Speed.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// The eighth axis is what the four angular dimensions are for, and this is the pair that
	/// justified adding it: an angular speed and a frequency are both per-second and must not be
	/// the same type.
	/// </summary>
	[TestMethod]
	public void SeparatesAnAngularSpeedFromAFrequency()
	{
		string angular = Output.Files["AngularSpeed.hpp"];
		string frequency = Output.Files["Frequency.hpp"];

		Assert.Contains("Quantity<Dimension<0, 0, -1, 1>>", angular, StringComparison.Ordinal);
		Assert.Contains("Quantity<Dimension<0, 0, -1>>", frequency, StringComparison.Ordinal);
	}

	/// <summary>
	/// An overload widens implicitly into its base and narrows back explicitly, which is the
	/// convention <c>ktsu.Schema</c> holds for a semantic type and this library holds for an
	/// overload. The two agreed about it independently.
	/// </summary>
	[TestMethod]
	public void WidensImplicitlyAndNarrowsExplicitly()
	{
		string weight = Output.Files["Weight.hpp"];

		Assert.Contains("operator ForceMagnitude()", weight, StringComparison.Ordinal);
		Assert.Contains("static constexpr Weight from(ForceMagnitude value)", weight, StringComparison.Ordinal);
		Assert.Contains("using refines = ForceMagnitude;", weight, StringComparison.Ordinal);
	}

	/// <summary>
	/// A magnitude cannot be negative, and the check is compiled out of the build the zero-cost
	/// claim is measured in.
	/// </summary>
	[TestMethod]
	public void GuardsAMagnitudeInDebugOnly()
	{
		Assert.Contains("assert(value.count() >= 0", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// Three overloads declare that zero is unphysical for them too, and they get the stricter
	/// comparison rather than the shared one.
	/// </summary>
	[TestMethod]
	public void GuardsAStrictlyPositiveQuantityMoreTightly()
	{
		Assert.Contains("assert(value.count() > 0", Output.Files["Wavelength.hpp"], StringComparison.Ordinal);
		Assert.Contains("assert(value.count() >= 0", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// Rule two of the four the zero-cost measurement produced: an accessor returns a reference.
	/// </summary>
	/// <remarks>
	/// Returning the component by value instead was one of the three differences that took the
	/// generated shape from 0.9896 to 1.4004 on MSVC while GCC and clang folded both away.
	/// </remarks>
	[TestMethod]
	public void ReturnsTheUnderlyingValueByReference()
	{
		Assert.Contains("const underlying& value() const", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// Rule three: the arithmetic stays in <c>Quantity</c> space rather than unwrapping to a
	/// number and rewrapping the result.
	/// </summary>
	/// <remarks>
	/// This is also what makes a declared relationship checkable. <c>value()</c> returns a
	/// <c>Quantity</c>, so its product carries the summed exponents and only converts to the
	/// declared result when they agree.
	/// </remarks>
	[TestMethod]
	public void KeepsRelationshipArithmeticInQuantitySpace()
	{
		string relationships = Output.Files["relationships.hpp"];

		Assert.Contains("lhs.value() * rhs.value()", relationships, StringComparison.Ordinal);
		Assert.DoesNotContain("lhs.count()", relationships, StringComparison.Ordinal);
	}

	/// <summary>
	/// The operators the metadata declares are generated as free functions.
	/// </summary>
	[TestMethod]
	public void GeneratesTheDeclaredRelationships()
	{
		string relationships = Output.Files["relationships.hpp"];

		// Speed integrated over time is a length, which is the relationship every other one is
		// checked by analogy with.
		Assert.Contains("Length operator*(Speed lhs, Duration rhs)", relationships, StringComparison.Ordinal);
	}

	/// <summary>
	/// A relationship the exponents contradict is refused by name rather than emitted as something
	/// that cannot compile.
	/// </summary>
	/// <remarks>
	/// Four are refused on the metadata as it stands. Three are the rotational cluster, where no
	/// assignment of angle exponents can satisfy both <c>Force x Length -&gt; Torque</c> and
	/// <c>Torque * AngularDisplacement -&gt; Energy</c> -- the two force the same exponent to be 0
	/// and -1. The fourth was already wrong before angle existed.
	/// </remarks>
	[TestMethod]
	public void RefusesARelationshipTheExponentsContradict()
	{
		IReadOnlyList<string> refused = Output.Refused;

		Assert.IsTrue(
			refused.Any(issue => issue.Contains("Sensitivity * Pressure", StringComparison.Ordinal)),
			$"expected the pre-existing metadata error to be caught; got: {string.Join(" | ", refused)}");

		Assert.IsTrue(
			refused.All(issue => issue.Contains("is not dimensionally true", StringComparison.Ordinal)
				|| issue.Contains("does not declare", StringComparison.Ordinal)),
			$"every refusal should say which of the two things went wrong; got: {string.Join(" | ", refused)}");
	}

	/// <summary>
	/// Nothing refused is also generated: a refusal has to mean the operator is absent, or it is
	/// only a log line.
	/// </summary>
	[TestMethod]
	public void DoesNotGenerateWhatItRefused()
	{
		string relationships = Output.Files["relationships.hpp"];

		Assert.DoesNotContain("ElectricPotential operator*(Sensitivity", relationships, StringComparison.Ordinal);
		Assert.DoesNotContain("Torque operator*(MomentOfInertia", relationships, StringComparison.Ordinal);
	}

	/// <summary>
	/// Every generated file says where it came from and that editing it is pointless.
	/// </summary>
	[TestMethod]
	public void SaysItIsGenerated()
	{
		foreach ((string name, string text) in Output.Files)
		{
			Assert.Contains("Do not edit", text, StringComparison.Ordinal, $"{name} should say it is generated");
		}
	}
}
