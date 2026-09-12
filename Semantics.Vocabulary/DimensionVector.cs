// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Vocabulary;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

/// <summary>
/// The eight exponents a dimension is, and the arithmetic over them.
/// </summary>
/// <remarks>
/// This exists so a generator can answer one question the metadata does not: whether a declared
/// relationship is dimensionally true. <c>Force * Length -&gt; Torque</c> is a claim, and the
/// exponents are what check it.
/// <para>
/// Eight axes rather than SI's seven, because angle is the one quantity this system is stricter
/// about than SI: without it an angle and a ratio are one type, and an angular velocity and a
/// frequency are one type.
/// </para>
/// <para>
/// Nothing here uses <c>System.HashCode</c>, a range expression or any other API that arrived after
/// netstandard2.0. This file is compiled into a Roslyn component as well as into an ordinary
/// library, and the component has the older surface.
/// </para>
/// </remarks>
internal readonly record struct DimensionVector
{
	/// <summary>
	/// The axis names, in the order a dimension's exponents are written.
	/// </summary>
	/// <remarks>
	/// These are the keys <c>dimensions.json</c> writes, except <c>angle</c>, which the metadata
	/// did not have until the C++ projection needed it. The order is load-bearing wherever a
	/// dimension is written as its non-zero prefix, which only means anything if everyone agrees
	/// where each axis sits.
	/// </remarks>
	internal static readonly string[] Axes =
	[
		"length", "mass", "time", "angle", "electricCurrent", "temperature", "amountOfSubstance", "luminousIntensity",
	];

	private readonly int[] exponents;

	private DimensionVector(int[] exponents) => this.exponents = exponents;

	/// <summary>
	/// Reads a dimension out of a metadata entry's <c>dimensionalFormula</c>.
	/// </summary>
	/// <param name="formula">The axis-to-exponent map, which names only its non-zero axes.</param>
	/// <returns>The exponents, with every axis the map omits at zero.</returns>
	internal static DimensionVector FromFormula(IReadOnlyDictionary<string, int> formula)
	{
		int[] values = new int[Axes.Length];
		for (int axis = 0; axis < Axes.Length; axis++)
		{
			values[axis] = formula.TryGetValue(Axes[axis], out int exponent) ? exponent : 0;
		}

		return new DimensionVector(values);
	}

	/// <summary>Gets the exponent on one axis.</summary>
	internal int this[int axis] => exponents is null ? 0 : exponents[axis];

	/// <summary>Adds two dimensions, which is what multiplying two quantities does.</summary>
	public static DimensionVector operator +(DimensionVector left, DimensionVector right) =>
		Combine(left, right, static (a, b) => a + b);

	/// <summary>Subtracts two dimensions, which is what dividing two quantities does.</summary>
	public static DimensionVector operator -(DimensionVector left, DimensionVector right) =>
		Combine(left, right, static (a, b) => a - b);

	private static DimensionVector Combine(DimensionVector left, DimensionVector right, System.Func<int, int, int> how)
	{
		int[] values = new int[Axes.Length];
		for (int axis = 0; axis < Axes.Length; axis++)
		{
			values[axis] = how(left[axis], right[axis]);
		}

		return new DimensionVector(values);
	}

	/// <inheritdoc />
	public bool Equals(DimensionVector other)
	{
		for (int axis = 0; axis < Axes.Length; axis++)
		{
			if (this[axis] != other[axis])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc />
	/// <remarks>
	/// Hand-rolled rather than <c>System.HashCode</c>, which netstandard2.0 does not have. The
	/// multiplier is the usual one; what matters is only that two vectors that compare equal agree
	/// here, which they do because both walk the same eight axes.
	/// </remarks>
	public override int GetHashCode()
	{
		int hash = 17;
		for (int axis = 0; axis < Axes.Length; axis++)
		{
			hash = (hash * 31) + this[axis];
		}

		return hash;
	}

	/// <summary>
	/// Writes the dimension the way a physicist would, for a comment or a diagnostic.
	/// </summary>
	/// <returns>Something like <c>M L T⁻²</c>, or <c>1</c> when there is nothing to say.</returns>
	public override string ToString()
	{
		string[] symbols = ["L", "M", "T", "A", "I", "Θ", "N", "J"];
		List<string> parts = [];
		for (int axis = 0; axis < Axes.Length; axis++)
		{
			int exponent = this[axis];
			if (exponent == 0)
			{
				continue;
			}

			parts.Add(exponent == 1 ? symbols[axis] : $"{symbols[axis]}{Superscript(exponent)}");
		}

		return parts.Count == 0 ? "1" : string.Join(" ", parts);
	}

	private static string Superscript(int exponent)
	{
		string digits = System.Math.Abs(exponent).ToString(CultureInfo.InvariantCulture);
		string raised = string.Concat(digits.Select(digit => "⁰¹²³⁴⁵⁶⁷⁸⁹"[digit - '0']));
		return exponent < 0 ? $"⁻{raised}" : raised;
	}
}
