// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using ktsu.PreciseNumber;
using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Runs <see cref="StorageConversionTests{T}"/> over <see cref="double"/>.
/// </summary>
[TestClass]
public sealed class DoubleStorageConversionTests() : StorageConversionTests<double>(tolerance: "1e-14", decimalExact: false);

/// <summary>
/// Runs <see cref="StorageConversionTests{T}"/> over <see cref="decimal"/>, where every terminating answer is exact.
/// </summary>
[TestClass]
public sealed class DecimalStorageConversionTests() : StorageConversionTests<decimal>(tolerance: "1e-25", decimalExact: true);

/// <summary>
/// Runs <see cref="StorageConversionTests{T}"/> over <see cref="PreciseNumber"/>, the storage type the
/// ktsu.Semantics.Quantities.Precise alias package binds to.
/// </summary>
/// <remarks>
/// The tolerance is ten orders of magnitude tighter than <see cref="decimal"/>'s, so a factor that
/// reached the type through a <see cref="double"/> — wrong from about the sixteenth digit — fails here
/// by an enormous margin. It is not tighter still because the limit is the expected literals rather
/// than the storage type: the knot is a repeating fraction written to 38 decimal places, and
/// PreciseNumber answers it with 50 correct digits, so the residual being measured is the truncation
/// of the reference value. Lengthening that literal is what would buy a tighter bound.
/// </remarks>
[TestClass]
public sealed class PreciseNumberStorageConversionTests()
	: StorageConversionTests<PreciseNumber>(tolerance: "1e-35", decimalExact: true);

/// <summary>
/// The same unit conversions, physics relationships and vector lengths, run over each storage type.
/// </summary>
/// <typeparam name="T">The storage type under test.</typeparam>
/// <param name="tolerance">
/// The largest relative error accepted where the answer does not terminate in <typeparamref name="T"/>,
/// as a literal. For <see cref="decimal"/> it is far below the 15 significant digits a factor kept when
/// it reached the type through <see cref="double"/>, so a regression to that route fails.
/// </param>
/// <param name="decimalExact">
/// Whether <typeparamref name="T"/> represents a terminating decimal exactly, as <see cref="decimal"/>
/// does and <see cref="double"/> does not. Where it does, a terminating answer is asserted exactly.
/// </param>
/// <remarks>
/// Adding a storage type is one line: a sealed derived class like the two above.
/// </remarks>
public abstract class StorageConversionTests<T>(string tolerance, bool decimalExact)
	where T : struct, INumber<T>
{
	private readonly T relativeTolerance = Of(tolerance);

	[TestMethod]
	public void AFootIsTwelveInches()
	{
		Length<T> foot = Length<T>.FromFoot(T.One);

		AssertValue("0.3048", foot.Value, terminates: true);
		AssertValue("12", foot.In(Units.Inch), terminates: true);
	}

	[TestMethod]
	public void KilometersPerHourConvertAndRoundTrip()
	{
		Speed<T> speed = Speed<T>.FromKilometerPerHour(Of("36"));

		AssertValue("10", speed.Value, terminates: false);
		AssertValue("36", speed.In(Units.KilometerPerHour), terminates: false);
		AssertValue("10", speed.In(Units.MeterPerSecond), terminates: false);
	}

	[TestMethod]
	public void AKnotIsTheRepeatingFractionOfAMeterPerSecond()
	{
		Speed<T> knot = Speed<T>.FromKnot(T.One);

		AssertValue("0.51444444444444444444444444444444444444", knot.Value, terminates: false);
		AssertValue("1", knot.In(Units.Knot), terminates: false);
	}

	[TestMethod]
	public void FahrenheitConvertsToKelvinAndCelsius()
	{
		Temperature<T> boiling = Temperature<T>.FromFahrenheit(Of("212"));

		AssertValue("373.15", boiling.Value, terminates: false);
		AssertValue("100", boiling.In(Units.Celsius), terminates: false);
		AssertValue("212", boiling.In(Units.Fahrenheit), terminates: false);
	}

	[TestMethod]
	public void AnglesUsePiToThePrecisionOfTheStorageType()
	{
		const string Pi = "3.14159265358979323846264338327950288419716939937510582097494459";
		const string Tau = "6.28318530717958647692528676655900576839433879875021164194988918";

		AssertValue(Pi, Angle<T>.FromDegree(Of("180")).Value, terminates: false);
		AssertValue(Tau, Angle<T>.FromRevolution(T.One).Value, terminates: false);
		AssertValue(Pi, Angle<T>.FromGradian(Of("200")).Value, terminates: false);
		AssertValue(Pi, AngularSpeed<T>.FromRevolutionPerMinute(Of("30")).Value, terminates: false);
	}

	[TestMethod]
	public void PressureAndPowerUseTheirExactDefinitions()
	{
		AssertValue("745.69987158227022", Power<T>.FromHorsepower(T.One).Value, terminates: true);
		AssertValue("6894.757293168361336722673445346890693781387562775", Pressure<T>.FromPsi(T.One).Value, terminates: false);
		AssertValue("101325", Pressure<T>.FromTorr(Of("760")).Value, terminates: false);
	}

	/// <summary>
	/// A metric magnitude combined with a factor of more significant digits than a <see cref="double"/>
	/// conversion keeps is exact.
	/// </summary>
	/// <remarks>
	/// A power of ten alone cannot tell the two routes apart, because converting <c>1e-2</c> from
	/// <see cref="double"/> to <see cref="decimal"/> already gives exactly 0.01. The 17 significant digits
	/// of these factors are what the old route rounded to 15.
	/// </remarks>
	[TestMethod]
	public void AMetricMagnitudeCombinedWithALongFactorIsExact()
	{
		AssertValue("1355.8179483314004", TorqueMagnitude<T>.FromPoundFoot(Of("1000")).Value, terminates: true);
		AssertValue("0.00000074569987158227022", Power<T>.FromHorsepower(Of("0.000001")).In(Units.Kilowatt), terminates: true);
		AssertValue("36000", Length<T>.FromKilometer(Of("36")).Value, terminates: true);
	}

	[TestMethod]
	public void DividingALengthByADurationGivesASpeed()
	{
		Speed<T> speed = Length<T>.FromKilometer(Of("36")) / Duration<T>.FromHour(T.One);

		AssertValue("10", speed.Value, terminates: true);
	}

	[TestMethod]
	public void AThreeFourFiveVectorHasLengthFive()
	{
		Displacement3D<T> vector = new() { X = Of("3"), Y = Of("4"), Z = T.Zero };

		Assert.AreEqual(Of("5"), vector.Length());
		Assert.AreEqual(Of("5"), vector.Magnitude().Value);
	}

	[TestMethod]
	public void TheDistanceBetweenTwoPointsIsExactWhenTheRootIs()
	{
		Displacement3D<T> from = new() { X = Of("1"), Y = Of("2"), Z = Of("3") };
		Displacement3D<T> to = new() { X = Of("4"), Y = Of("6"), Z = Of("3") };

		Assert.AreEqual(Of("5"), from.Distance(to));
	}

	[TestMethod]
	public void AnIrrationalLengthHasThePrecisionOfTheStorageType()
	{
		Displacement2D<T> diagonal = new() { X = T.One, Y = T.One };

		AssertValue("1.41421356237309504880168872420969807856967187537694807317667973799", diagonal.Length(), terminates: false);
	}

	/// <summary>
	/// A vector factory converts every component, so the ×1000 a kilometre-native caller used to
	/// write by hand now lives in the type. Issue #237.
	/// </summary>
	[TestMethod]
	public void AVectorFactoryConvertsEveryComponent()
	{
		Position3D<T> orbit = Position3D<T>.FromKilometer(Of("6778"), Of("-1.5"), T.Zero);

		AssertValue("6778000", orbit.X, terminates: true);
		AssertValue("-1500", orbit.Y, terminates: true);
		AssertValue("0", orbit.Z, terminates: true);
	}

	/// <summary>
	/// The vector factories read the same <c>Values&lt;T&gt;</c> holder as the scalar ones, so a
	/// non-terminating factor lands at the storage type's own precision rather than arriving
	/// through <see cref="double"/>. The knot is the sharpest case the catalogue has.
	/// </summary>
	[TestMethod]
	public void AVectorFactoryAgreesWithTheScalarFactoryForTheSameUnit()
	{
		Velocity3D<T> velocity = Velocity3D<T>.FromKnot(T.One, T.One, T.One);
		Speed<T> speed = Speed<T>.FromKnot(T.One);

		AssertValue("0.51444444444444444444444444444444444444", velocity.X, terminates: false);
		Assert.AreEqual(speed.Value, velocity.X);
		Assert.AreEqual(speed.Value, velocity.Y);
		Assert.AreEqual(speed.Value, velocity.Z);
	}

	/// <summary>
	/// The reader answers in the caller's unit again, as bare components rather than the vector
	/// type — which it could not be, the result no longer being in base units.
	/// </summary>
	[TestMethod]
	public void TheVectorReaderRoundTripsThroughItsUnit()
	{
		(T x, T y, T z) = Position3D<T>.FromKilometer(Of("36"), Of("-4"), T.Zero).In(Units.Kilometer);

		AssertValue("36", x, terminates: true);
		AssertValue("-4", y, terminates: true);
		AssertValue("0", z, terminates: true);
	}

	/// <summary>
	/// Vector components are signed by construction, so the factories carry no
	/// <c>Vector0Guards</c>: a position with a negative X is ordinary, and the V0 non-negativity
	/// rule must not leak into the vector forms (#237, decision 1).
	/// </summary>
	/// <remarks>
	/// The paired scalar call is what makes this an assertion rather than a coincidence. The same
	/// unit and the same negative magnitude does throw on the V0, so a guard added to the vector
	/// factories would be caught here rather than quietly narrowing what they accept.
	/// </remarks>
	[TestMethod]
	public void AVectorFactoryAcceptsNegativeComponentsWhereTheMagnitudeFormRefusesThem()
	{
		Position3D<T> behind = Position3D<T>.FromKilometer(Of("-1"), Of("-2"), Of("-3"));

		AssertValue("-1000", behind.X, terminates: true);
		AssertValue("-3000", behind.Z, terminates: true);

		Assert.ThrowsExactly<ArgumentException>(() => Length<T>.FromKilometer(Of("-1")));
	}

	/// <summary>
	/// Parses a literal into <typeparamref name="T"/>.
	/// </summary>
	/// <param name="literal">The literal.</param>
	/// <returns>The nearest value of <typeparamref name="T"/>.</returns>
	protected static T Of(string literal) => T.Parse(literal, NumberStyles.Float, CultureInfo.InvariantCulture);

	/// <summary>
	/// Asserts a result against its exact value.
	/// </summary>
	/// <param name="expected">The exact answer as a literal, written with more digits than any storage type holds where it does not terminate.</param>
	/// <param name="actual">The result.</param>
	/// <param name="terminates">Whether the exact answer is a terminating decimal the storage type has the digits for, and is therefore compared exactly when <c>decimalExact</c> is set.</param>
	private void AssertValue(string expected, T actual, bool terminates)
	{
		T target = Of(expected);

		if (terminates && decimalExact)
		{
			Assert.AreEqual(target, actual, $"Expected exactly {expected}.");
			return;
		}

		T allowed = relativeTolerance * T.Max(T.One, T.Abs(target));
		Assert.IsLessThanOrEqualTo(allowed, T.Abs(actual - target), $"Expected {expected} to within {tolerance} relative, got {actual}.");
	}
}

/// <summary>
/// The storage-type factor path and the public <see cref="double"/> properties describe the same conversion.
/// </summary>
[TestClass]
public sealed class UnitFactorConsistencyTests
{
	/// <summary>
	/// For <see cref="double"/> storage, every generated unit hands out exactly the factor and offset its
	/// <see cref="IUnit.ToBaseFactor"/> and <see cref="IUnit.ToBaseOffset"/> report, so the two routes
	/// cannot drift apart.
	/// </summary>
	[TestMethod]
	public void EveryUnitGivesDoubleStorageWhatItsPropertiesReport()
	{
		List<IUnit> units =
		[
			.. typeof(Units)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Select(static field => field.GetValue(null))
				.OfType<IUnit>(),
		];

		Assert.IsGreaterThan(100, units.Count, "The catalogue scan found too few units to mean anything.");

		List<string> mismatched =
		[
			.. units
				.Where(static unit => unit.ToBaseFactorAs<double>().CompareTo(unit.ToBaseFactor) != 0
					|| unit.ToBaseOffsetAs<double>().CompareTo(unit.ToBaseOffset) != 0)
				.Select(static unit => unit.Name),
		];

		Assert.IsEmpty(mismatched, $"These units convert double storage differently from their properties: {string.Join(", ", mismatched)}");
	}
}
