// Copyright (c) 2023-2026 ktsu-dev contributors

namespace ktsu.Semantics.Test.Quantities;

using System;
using System.Collections.Generic;
using System.Linq;
using ktsu.Semantics.Quantities;
using ktsu.Semantics.Quantities.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Covers which dimension a unit reports when more than one claims it.
/// </summary>
/// <remarks>
/// A unit's marker interfaces carry every dimension whose <c>availableUnits</c> names it, so the
/// singular <see cref="IUnit.Dimension"/> is the only place that has to choose. It used to choose
/// the first declared, which is a choice by file position rather than by meaning, and
/// <c>Dimensionless</c> is the first entry in <c>dimensions.json</c>.
/// </remarks>
[TestClass]
public sealed class UnitDimensionTests
{
	/// <summary>
	/// A radian is an angle, not a ratio.
	/// </summary>
	/// <remarks>
	/// The whole of what the <c>angle</c> axis is for: without it an angle is the same thing as a
	/// ratio, and a unit reporting no exponents says exactly that however the axis is spelled
	/// elsewhere.
	/// </remarks>
	[TestMethod]
	public void AnAngularUnitReportsTheAngularDimension()
	{
		foreach (IUnit unit in (IUnit[])[new Radian(), new Degree(), new Gradian(), new Milliradian(), new Revolution()])
		{
			Assert.AreEqual("AngularDisplacement", unit.Dimension.Name, $"{unit.Name} reports the wrong dimension");
			Assert.AreEqual(1, unit.Dimension.DimensionalFormula["angle"], $"{unit.Name} is not one angle");
			Assert.HasCount(1, unit.Dimension.DimensionalFormula, $"{unit.Name} measures something besides an angle");
		}
	}

	/// <summary>
	/// A claim that says something beats one that says nothing, for every unit rather than the
	/// five that prompted it.
	/// </summary>
	/// <remarks>
	/// Stated over the assembly because the fix is a rule rather than a list: a unit added to both
	/// a real dimension and <c>Dimensionless</c> tomorrow is the same bug, and naming today's five
	/// would not catch it. Walks the compiled types rather than the metadata, since what a consumer
	/// reads is the property and not the JSON behind it.
	/// </remarks>
	[TestMethod]
	public void AUnitClaimedTwiceReportsTheDimensionWithExponents()
	{
		Dictionary<string, DimensionInfo> byName =
			PhysicalDimensions.All.ToDictionary(static dimension => dimension.Name, StringComparer.Ordinal);

		int claimedTwice = 0;

		foreach (IUnit unit in EveryUnit())
		{
			List<DimensionInfo> claims = [.. unit.GetType()
				.GetInterfaces()
				.Where(static marker => marker != typeof(IUnit) && marker.Name.StartsWith('I') && marker.Name.EndsWith("Unit", StringComparison.Ordinal))
				.Select(marker => marker.Name[1..^"Unit".Length])
				.Where(byName.ContainsKey)
				.Select(name => byName[name])];

			if (claims.Count < 2)
			{
				continue;
			}

			claimedTwice++;

			if (claims.Exists(static claim => claim.DimensionalFormula.Count > 0))
			{
				Assert.IsNotEmpty(
					unit.Dimension.DimensionalFormula,
					$"{unit.Name} is claimed by {string.Join(", ", claims.Select(static claim => claim.Name))} and reports the one with no exponents");
			}
		}

		Assert.IsGreaterThan(0, claimedTwice, "no unit is claimed twice, so this asserts nothing");
	}

	private static IEnumerable<IUnit> EveryUnit()
	{
		foreach (Type type in typeof(Meter).Assembly.GetTypes())
		{
			if (type.IsAbstract || !type.IsClass || !typeof(IUnit).IsAssignableFrom(type))
			{
				continue;
			}

			if (type.GetConstructor(Type.EmptyTypes) is null)
			{
				continue;
			}

			if (Activator.CreateInstance(type) is IUnit unit)
			{
				yield return unit;
			}
		}
	}
}
