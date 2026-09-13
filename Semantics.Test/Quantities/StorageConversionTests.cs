// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

	[TestMethod]
	public void AMetricMagnitudeIsExact()
	{
		AssertValue("0.01", Length<T>.FromCentimeter(T.One).Value, terminates: true);
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
