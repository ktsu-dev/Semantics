// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Numerics;

/// <summary>
/// Base implementation of conversion calculator that handles common conversion logic.
/// </summary>
/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
public abstract class BaseConversionCalculator<TQuantity> : IConversionCalculator<TQuantity>
	where TQuantity : PhysicalQuantity<TQuantity>, new()
{
	private readonly Dictionary<string, ConversionDefinition> _conversions = new(StringComparer.OrdinalIgnoreCase);
	private readonly string _baseUnit;

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseConversionCalculator{TQuantity}"/> class.
	/// </summary>
	/// <param name="baseUnit">The base SI unit for this quantity type.</param>
	/// <param name="conversions">The available conversion definitions.</param>
	protected BaseConversionCalculator(string baseUnit, IEnumerable<ConversionDefinition> conversions) // TODO: baseUnit should be a SIUnit
	{
		ArgumentNullException.ThrowIfNull(conversions);
		_baseUnit = baseUnit;

		// Add base unit with factor 1 and no offset
		_conversions[baseUnit] = new ConversionDefinition(baseUnit, 1.0, 0.0);

		// Add all other conversions
		foreach (ConversionDefinition conversion in conversions)
		{
			_conversions[conversion.Unit] = conversion;
		}
	}

	/// <inheritdoc/>
	public virtual TQuantity FromUnit<TNumber>(TNumber value, string unit)
		where TNumber : INumber<TNumber>
	{
		if (!_conversions.TryGetValue(unit, out ConversionDefinition? conversion))
		{
			throw new ArgumentException($"Unknown unit '{unit}' for {typeof(TQuantity).Name}", nameof(unit));
		}

		double doubleValue = Convert.ToDouble(value);
		double baseValue = conversion.ToBase(doubleValue);
		return PhysicalQuantity<TQuantity>.Create(baseValue);
	}

	/// <inheritdoc/>
	public virtual TNumber ToUnit<TNumber>(TQuantity quantity, string unit)
		where TNumber : INumber<TNumber>
	{
		ArgumentNullException.ThrowIfNull(quantity);

		if (!_conversions.TryGetValue(unit, out ConversionDefinition? conversion))
		{
			throw new ArgumentException($"Unknown unit '{unit}' for {typeof(TQuantity).Name}", nameof(unit));
		}

		double convertedValue = conversion.FromBase(quantity.Quantity);
		return TNumber.CreateChecked(convertedValue);
	}

	/// <inheritdoc/>
	public virtual IEnumerable<string> GetAvailableUnits() => _conversions.Keys.OrderBy(unit => unit);

	/// <inheritdoc/>
	public virtual string GetBaseUnit() => _baseUnit;

	/// <summary>
	/// Gets the conversion definition for the specified unit.
	/// </summary>
	/// <param name="unit">The unit name.</param>
	/// <returns>The conversion definition if found, otherwise null.</returns>
	protected ConversionDefinition? GetConversionDefinition(string unit)
	{
		_conversions.TryGetValue(unit, out ConversionDefinition? conversion);
		return conversion;
	}
}
