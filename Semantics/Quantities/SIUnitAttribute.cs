// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Reflection;

/// <summary>
/// An attribute to define the SI unit for a class.
/// </summary>
/// <remarks>
/// This attribute is intended to be applied to classes that represent physical quantities
/// and their associated SI units.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SIUnitAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SIUnitAttribute"/> class with a strongly-typed SI unit.
	/// </summary>
	/// <param name="unit">The SI unit for the physical quantity.</param>
	public SIUnitAttribute(SIUnit unit) => Unit = unit;

	/// <summary>
	/// Initializes a new instance of the <see cref="SIUnitAttribute"/> class using a predefined unit from SIUnits class.
	/// </summary>
	/// <param name="unitType">The type containing the static unit property (typically typeof(SIUnits)).</param>
	/// <param name="unitPropertyName">The name of the static property that returns the SIUnit.</param>
	public SIUnitAttribute(Type unitType, string unitPropertyName)
	{
		ArgumentNullException.ThrowIfNull(unitType);
		ArgumentNullException.ThrowIfNull(unitPropertyName);

		PropertyInfo? property = unitType.GetProperty(unitPropertyName, BindingFlags.Public | BindingFlags.Static);
		if (property?.GetValue(null) is not SIUnit unit)
		{
			throw new ArgumentException($"Property '{unitPropertyName}' on type '{unitType.Name}' does not return a valid SIUnit.", nameof(unitPropertyName));
		}

		Unit = unit;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SIUnitAttribute"/> class with string parameters for backward compatibility.
	/// </summary>
	/// <param name="symbol">The symbol of the SI unit.</param>
	/// <param name="singular">The singular name of the SI unit.</param>
	/// <param name="plural">The plural name of the SI unit.</param>
	[Obsolete("Use SIUnitAttribute(SIUnit unit) constructor with predefined units from SIUnits class instead.")]
	public SIUnitAttribute(string symbol, string singular, string plural) => Unit = new SIUnit(symbol, singular, plural);

	/// <summary>
	/// Gets the SI unit associated with the physical quantity.
	/// </summary>
	public SIUnit Unit { get; }

	/// <summary>
	/// Gets the symbol of the SI unit (e.g., "m" for meters).
	/// </summary>
	public string Symbol => Unit.Symbol;

	/// <summary>
	/// Gets the singular name of the SI unit (e.g., "meter").
	/// </summary>
	public string Singular => Unit.Singular;

	/// <summary>
	/// Gets the plural name of the SI unit (e.g., "meters").
	/// </summary>
	public string Plural => Unit.Plural;
	public Type UnitType { get; }
}
