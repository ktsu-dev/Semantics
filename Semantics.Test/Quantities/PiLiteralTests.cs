// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Checks every long literal built on π in the metadata against π computed here, to its last digit.
/// </summary>
/// <remarks>
/// The literals were once derived from one another, and an error at the 98th significant digit of
/// <c>DegreeToRadians</c> spread into every factor taken from it. A comparison against a hardcoded π of
/// 63 digits could not see that, so π is computed from Machin's formula in integer arithmetic instead.
/// </remarks>
[TestClass]
public sealed class PiLiteralTests
{
	/// <summary>The number of decimal places π is computed to.</summary>
	private const int Places = 220;

	/// <summary>Extra places computed and then discarded, so truncation in the series cannot reach the places kept.</summary>
	private const int GuardPlaces = 20;

	/// <summary>The fewest significant digits a literal built on π must carry.</summary>
	private const int MinimumSignificantDigits = 100;

	private static string MetadataDirectory => Path.Combine(AppContext.BaseDirectory, "GeneratorMetadata");

	/// <summary>
	/// π scaled by 10 to the power of <see cref="Places"/>, correct to within a unit in its last place.
	/// </summary>
	private static readonly BigInteger ScaledPi = ComputeScaledPi();

	/// <summary>
	/// The Machin computation agrees with the first 50 decimal places of π, which is what makes it a
	/// trustworthy reference for the rest.
	/// </summary>
	[TestMethod]
	public void MachinFormulaReproducesTheKnownDigitsOfPi()
	{
		const string KnownDigits = "314159265358979323846264338327950288419716939937510";

		Assert.StartsWith(KnownDigits, ScaledPi.ToString(System.Globalization.CultureInfo.InvariantCulture));
	}

	/// <summary>
	/// A literal built on π matches π to its last written digit and carries at least
	/// <see cref="MinimumSignificantDigits"/> significant digits.
	/// </summary>
	/// <param name="fileName">The metadata file declaring the literal.</param>
	/// <param name="name">The factor or constant name.</param>
	/// <param name="inverse">Whether the value is <paramref name="numerator"/> / (π × <paramref name="denominator"/>) rather than π × <paramref name="numerator"/> / <paramref name="denominator"/>.</param>
	/// <param name="numerator">The integer multiplier.</param>
	/// <param name="denominator">The integer divisor.</param>
	[TestMethod]
	[DataRow("conversions.json", "DegreeToRadians", false, 1L, 180L)]
	[DataRow("conversions.json", "GradianToRadians", false, 1L, 200L)]
	[DataRow("conversions.json", "RevolutionToRadians", false, 2L, 1L)]
	[DataRow("conversions.json", "RevolutionPerMinuteToRadianPerSecond", false, 1L, 30L)]
	[DataRow("conversions.json", "FootLambertToCandelaPerSquareMeter", true, 100000000L, 9290304L)]
	[DataRow("domains.json", "TwoPi", false, 2L, 1L)]
	[DataRow("domains.json", "RadiansPerDegree", false, 1L, 180L)]
	[DataRow("domains.json", "DegreesPerRadian", true, 180L, 1L)]
	public void APiLiteralMatchesPiToItsLastDigit(string fileName, string name, bool inverse, long numerator, long denominator)
	{
		string literal = ReadLiteral(fileName, name);
		(BigInteger mantissa, int scale) = SplitLiteral(literal);
		string digits = mantissa.ToString(System.Globalization.CultureInfo.InvariantCulture);

		Assert.IsGreaterThanOrEqualTo(MinimumSignificantDigits, digits.Length, $"{name} has only {digits.Length} significant digits.");
		Assert.IsLessThan(Places - GuardPlaces, scale, $"{name} has more places than this test computes π to.");

		BigInteger placeValue = BigInteger.Pow(10, scale);
		BigInteger piPlaceValue = BigInteger.Pow(10, Places);
		BigInteger expected = inverse
			? RoundedQuotient(numerator * placeValue * piPlaceValue, ScaledPi * denominator)
			: RoundedQuotient(ScaledPi * numerator * placeValue, piPlaceValue * denominator);

		BigInteger error = BigInteger.Abs(mantissa - expected);
		Assert.IsLessThanOrEqualTo(BigInteger.One, error, $"{name} is wrong before its last digit: {literal}");
	}

	/// <summary>
	/// Computes π × 10^<see cref="Places"/> from π = 16 arctan(1/5) − 4 arctan(1/239).
	/// </summary>
	/// <returns>The scaled value.</returns>
	private static BigInteger ComputeScaledPi()
	{
		BigInteger unity = BigInteger.Pow(10, Places + GuardPlaces);
		BigInteger pi = (16 * ArctanOfReciprocal(5, unity)) - (4 * ArctanOfReciprocal(239, unity));
		return pi / BigInteger.Pow(10, GuardPlaces);
	}

	/// <summary>
	/// Sums the Taylor series of arctan(1/<paramref name="x"/>) in fixed point.
	/// </summary>
	/// <param name="x">The reciprocal of the argument.</param>
	/// <param name="unity">The fixed point representation of one.</param>
	/// <returns>arctan(1/<paramref name="x"/>) × <paramref name="unity"/>, truncated term by term.</returns>
	private static BigInteger ArctanOfReciprocal(int x, BigInteger unity)
	{
		BigInteger xSquared = new(x * x);
		BigInteger power = unity / x;
		BigInteger sum = power;
		bool subtract = true;
		int n = 3;

		while (!power.IsZero)
		{
			power /= xSquared;
			BigInteger term = power / n;
			sum = subtract ? sum - term : sum + term;
			subtract = !subtract;
			n += 2;
		}

		return sum;
	}

	/// <summary>
	/// Divides and rounds to the nearest integer, for positive operands.
	/// </summary>
	/// <param name="dividend">The dividend.</param>
	/// <param name="divisor">The divisor.</param>
	/// <returns>The rounded quotient.</returns>
	private static BigInteger RoundedQuotient(BigInteger dividend, BigInteger divisor)
		=> ((2 * dividend) + divisor) / (2 * divisor);

	/// <summary>
	/// Splits a plain decimal literal into its digits and the number of places after the point.
	/// </summary>
	/// <param name="literal">A literal such as <c>"0.0174"</c>, with no sign or exponent.</param>
	/// <returns>The significand without leading zeros, and the scale.</returns>
	private static (BigInteger Mantissa, int Scale) SplitLiteral(string literal)
	{
		int point = literal.IndexOf('.', StringComparison.Ordinal);
		string digits = point < 0 ? literal : literal.Remove(point, 1);
		int scale = point < 0 ? 0 : literal.Length - point - 1;
		return (BigInteger.Parse(digits, System.Globalization.CultureInfo.InvariantCulture), scale);
	}

	/// <summary>
	/// Reads the <c>value</c> of a named entry from <c>conversions.json</c> or <c>domains.json</c>.
	/// </summary>
	/// <param name="fileName">The metadata file.</param>
	/// <param name="name">The entry name.</param>
	/// <returns>The value as written.</returns>
	private static string ReadLiteral(string fileName, string name)
	{
		using JsonDocument document = JsonDocument.Parse(File.ReadAllText(Path.Combine(MetadataDirectory, fileName)));
		(string groups, string entries) = fileName == "domains.json" ? ("domains", "constants") : ("conversions", "factors");

		Dictionary<string, string> values = [];
		foreach (JsonElement group in document.RootElement.GetProperty(groups).EnumerateArray())
		{
			// Not every domain declares constants.
			if (!group.TryGetProperty(entries, out JsonElement members))
			{
				continue;
			}

			foreach (JsonElement entry in members.EnumerateArray())
			{
				values[entry.GetProperty("name").GetString()!] = entry.GetProperty("value").GetString()!;
			}
		}

		Assert.IsTrue(values.TryGetValue(name, out string? literal), $"{fileName} declares no {name}.");
		return literal!;
	}
}
