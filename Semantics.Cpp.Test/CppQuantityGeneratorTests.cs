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
		QuantityMetadata.Parse(File.ReadAllText(Path.Join(AppContext.BaseDirectory, "Metadata", "dimensions.json")));

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
	/// A class per dimension, per vector form and per named overload, plus the prelude and the two
	/// roll-ups.
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
	/// and -1. The fourth is a signed value in a magnitude form.
	/// <para>
	/// A fifth used to be here: <c>Sensitivity * Pressure -&gt; ElectricPotential</c>, which was a
	/// plain metadata bug rather than anything about angle, and is fixed. This check is what found
	/// it, so it is asserted the other way round in
	/// <c>UnkeepableRelationshipTests.TheSensitivityRelationshipIsNoLongerRefused</c>.
	/// </para>
	/// </remarks>
	[TestMethod]
	public void RefusesARelationshipTheExponentsContradict()
	{
		IReadOnlyList<string> refused = Output.Refused;

		Assert.IsTrue(
			refused.Any(issue => issue.Contains("Torque * AngularDisplacement", StringComparison.Ordinal)),
			$"expected the rotational contradiction to be caught; got: {string.Join(" | ", refused)}");

		Assert.IsTrue(
			refused.All(issue => issue.Contains("is not dimensionally true", StringComparison.Ordinal)
				|| issue.Contains("does not declare", StringComparison.Ordinal)
				|| issue.Contains("reduces to a signed value", StringComparison.Ordinal)),
			$"every refusal should say which of the three things went wrong; got: {string.Join(" | ", refused)}");
	}

	/// <summary>
	/// A vector form is a class of its own with as many components as it has dimensions, not an
	/// alias for the magnitude and not an array.
	/// </summary>
	[TestMethod]
	public void GeneratesAClassPerVectorForm()
	{
		Assert.Contains("Displacement1D.hpp", Output.Files.Keys);
		Assert.Contains("Displacement2D.hpp", Output.Files.Keys);
		Assert.Contains("Displacement3D.hpp", Output.Files.Keys);
		Assert.Contains("Displacement4D.hpp", Output.Files.Keys);

		string displacement = Output.Files["Displacement3D.hpp"];

		Assert.Contains("explicit constexpr Displacement3D(component x, component y, component z)", displacement, StringComparison.Ordinal);
		Assert.Contains("component x_{};", displacement, StringComparison.Ordinal);
		Assert.Contains("component z_{};", displacement, StringComparison.Ordinal);
		Assert.DoesNotContain("component w_{};", displacement, StringComparison.Ordinal);
	}

	/// <summary>
	/// An overload of a vector form is a distinct class too, and widens and narrows across every
	/// component rather than only the first.
	/// </summary>
	[TestMethod]
	public void WidensAndNarrowsAVectorAcrossAllOfItsComponents()
	{
		string position = Output.Files["Position3D.hpp"];

		Assert.Contains("return Displacement3D{ x_, y_, z_ };", position, StringComparison.Ordinal);
		Assert.Contains("return Position3D{ value.x(), value.y(), value.z() };", position, StringComparison.Ordinal);
	}

	/// <summary>
	/// Rule four of the four the zero-cost measurement produced: a componentwise operation is
	/// expanded at compile time rather than looped over an index.
	/// </summary>
	/// <remarks>
	/// A loop over a runtime subscript is what took the same spike from 1.01 to 4.51 on MSVC. It
	/// costs this generator nothing to obey, because the components are in hand while the class is
	/// being written -- so what is asserted here is that the expansion is in the text, with no
	/// index anywhere for a compiler to have an opinion about.
	/// </remarks>
	[TestMethod]
	public void ExpandsAComponentwiseOperationRatherThanLoopingOverIt()
	{
		string displacement = Output.Files["Displacement3D.hpp"];

		Assert.Contains(
			"return Displacement3D{ lhs.x_ + rhs.x_, lhs.y_ + rhs.y_, lhs.z_ + rhs.z_ };",
			displacement,
			StringComparison.Ordinal);

		Assert.DoesNotContain("for(", displacement, StringComparison.Ordinal);
		Assert.DoesNotContain("operator[]", displacement, StringComparison.Ordinal);
	}

	/// <summary>
	/// Arithmetic belongs to the signed forms and stops there.
	/// </summary>
	/// <remarks>
	/// Not an oversight. <c>Length - Length</c> has a question in it that the vector forms do not:
	/// what it means when the answer would be negative. The .NET side settled that as the absolute
	/// difference, and settling it here is a decision about the magnitude form rather than
	/// something to smuggle in alongside the vectors.
	/// </remarks>
	[TestMethod]
	public void GivesArithmeticToTheSignedFormsOnly()
	{
		Assert.Contains("operator+(Displacement1D lhs, Displacement1D rhs)", Output.Files["Displacement1D.hpp"], StringComparison.Ordinal);
		Assert.Contains("operator+(Displacement3D lhs, Displacement3D rhs)", Output.Files["Displacement3D.hpp"], StringComparison.Ordinal);
		Assert.DoesNotContain("operator+(Length lhs, Length rhs)", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// A vector compares for equality and for nothing else: there is no reading in which one
	/// displacement is less than another.
	/// </summary>
	[TestMethod]
	public void OrdersAScalarAndOnlyEquatesAVector()
	{
		Assert.Contains("operator<=>(Length, Length)", Output.Files["Length.hpp"], StringComparison.Ordinal);
		Assert.Contains("operator<=>(Displacement1D, Displacement1D)", Output.Files["Displacement1D.hpp"], StringComparison.Ordinal);

		string displacement = Output.Files["Displacement3D.hpp"];

		Assert.Contains("operator==(Displacement3D, Displacement3D)", displacement, StringComparison.Ordinal);
		Assert.DoesNotContain("operator<=>", displacement, StringComparison.Ordinal);
	}

	/// <summary>
	/// Every signed form answers its size with the magnitude form of the same dimension, which is
	/// the one place the signed half of the vocabulary reaches back into the unsigned half.
	/// </summary>
	/// <remarks>
	/// The dimension works out rather than being arranged: the sum of the squares of the
	/// components has twice a component's dimension, and <c>sqrt</c> halves it again. The
	/// magnitude form's constructor takes exactly that, so the two would not compile together if
	/// the generator had this wrong -- which is what
	/// <c>GeneratedCppCompilesTests</c> then actually checks.
	/// </remarks>
	[TestMethod]
	public void AnswersItsSizeWithTheMagnitudeForm()
	{
		Assert.Contains("Length magnitude() const", Output.Files["Displacement1D.hpp"], StringComparison.Ordinal);
		Assert.Contains("return Length{ abs(value_) };", Output.Files["Displacement1D.hpp"], StringComparison.Ordinal);

		string displacement = Output.Files["Displacement3D.hpp"];

		Assert.Contains("Length magnitude() const", displacement, StringComparison.Ordinal);
		Assert.Contains("return Length{ sqrt(magnitude_squared()) };", displacement, StringComparison.Ordinal);

		// A length squared is an area, and the structural layer is what says so without having to
		// choose between Area and NuclearCrossSection, which are the same exponents.
		Assert.Contains("constexpr Quantity<Dimension<2>> magnitude_squared() const", displacement, StringComparison.Ordinal);

		Assert.DoesNotContain("magnitude()", Output.Files["Length.hpp"], StringComparison.Ordinal);
	}

	/// <summary>
	/// A relationship reaches the vector forms by carrying its form on the left operand and the
	/// result, with the right operand staying a magnitude.
	/// </summary>
	/// <remarks>
	/// There is no reading in which the duration in <c>Velocity3D * Duration</c> has three
	/// components, which is why the form propagates along one side rather than all three. It is
	/// the same rule the .NET generator follows.
	/// </remarks>
	[TestMethod]
	public void CarriesARelationshipToEveryFormItsParticipantsShare()
	{
		string relationships = Output.Files["relationships.hpp"];

		Assert.Contains("Length operator*(Speed lhs, Duration rhs)", relationships, StringComparison.Ordinal);
		Assert.Contains("Displacement1D operator*(Velocity1D lhs, Duration rhs)", relationships, StringComparison.Ordinal);
		Assert.Contains("Displacement3D operator*(Velocity3D lhs, Duration rhs)", relationships, StringComparison.Ordinal);

		Assert.Contains(
			"return Displacement3D{ lhs.x() * rhs.value(), lhs.y() * rhs.value(), lhs.z() * rhs.value() };",
			relationships,
			StringComparison.Ordinal);
	}

	/// <summary>
	/// A cross product is emitted at three components and nowhere else.
	/// </summary>
	/// <remarks>
	/// That is the definition rather than a limitation: the cross product exists in 3D and 7D and
	/// nowhere else, and the metadata says so with <c>forms: [3]</c>. It is a named call rather
	/// than an operator because C++ has no symbol for it.
	/// </remarks>
	[TestMethod]
	public void GeneratesACrossProductAtThreeComponentsOnly()
	{
		string relationships = Output.Files["relationships.hpp"];

		// r x F, not F x r: the operands are in the order the declaring dimension puts them in, and
		// the two differ by a sign that no exponent can tell apart.
		Assert.Contains("Torque3D cross(Displacement3D lhs, Force3D rhs)", relationships, StringComparison.Ordinal);
		Assert.Contains(
			"return Torque3D{ lhs.y() * rhs.z() - lhs.z() * rhs.y(), lhs.z() * rhs.x() - lhs.x() * rhs.z(), lhs.x() * rhs.y() - lhs.y() * rhs.x() };",
			relationships,
			StringComparison.Ordinal);

		Assert.DoesNotContain("cross(Displacement2D", relationships, StringComparison.Ordinal);
		Assert.DoesNotContain("cross(Length", relationships, StringComparison.Ordinal);
	}

	/// <summary>
	/// The second kind of refusal, which the vector forms are what surfaced: a claim the exponents
	/// agree with and the sign does not.
	/// </summary>
	/// <remarks>
	/// A force opposing a displacement does negative work, so <c>dot</c> answers with a signed
	/// value; the metadata names <c>Energy</c> for the result, and a magnitude form cannot be
	/// negative. Emitting it would produce a type that fails its own assertion on a perfectly
	/// ordinary input, so it is refused with the fix named -- a <c>vector1</c> form on
	/// <c>Energy</c> -- rather than generated.
	/// </remarks>
	[TestMethod]
	public void RefusesADotProductThatWouldLandInAMagnitude()
	{
		IReadOnlyList<string> refused = Output.Refused;

		Assert.IsTrue(
			refused.Any(issue => issue.Contains("dot(Force, Length) -> Energy", StringComparison.Ordinal)
				&& issue.Contains("vector1", StringComparison.Ordinal)),
			$"the refusal should name the relationship and what would fix it; got: {string.Join(" | ", refused)}");

		Assert.DoesNotContain("dot(", Output.Files["relationships.hpp"], StringComparison.Ordinal);
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
