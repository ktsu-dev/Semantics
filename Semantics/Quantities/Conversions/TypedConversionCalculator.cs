// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Numerics;

/// <summary>
/// Interface for strongly-typed conversion calculator.
/// </summary>
/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
public interface ITypedConversionCalculator<TQuantity>
	where TQuantity : PhysicalQuantity<TQuantity>, new()
{
	/// <summary>
	/// Converts a value from the specified SI unit to the base quantity.
	/// </summary>
	/// <typeparam name="TNumber">The type of the input number.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <param name="unit">The SI unit to convert from.</param>
	/// <returns>A quantity representing the value in the base unit.</returns>
	public TQuantity FromSIUnit<TNumber>(TNumber value, SIUnit unit) where TNumber : INumber<TNumber>;

	/// <summary>
	/// Converts a value from the specified imperial unit to the base quantity.
	/// </summary>
	/// <typeparam name="TNumber">The type of the input number.</typeparam>
	/// <param name="value">The value to convert.</param>
	/// <param name="unit">The imperial unit to convert from.</param>
	/// <returns>A quantity representing the value in the base unit.</returns>
	public TQuantity FromImperialUnit<TNumber>(TNumber value, ImperialUnit unit) where TNumber : INumber<TNumber>;

	/// <summary>
	/// Converts a quantity to the specified SI unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the output number.</typeparam>
	/// <param name="quantity">The quantity to convert.</param>
	/// <param name="unit">The SI unit to convert to.</param>
	/// <returns>The value in the specified unit.</returns>
	public TNumber ToSIUnit<TNumber>(TQuantity quantity, SIUnit unit) where TNumber : INumber<TNumber>;

	/// <summary>
	/// Converts a quantity to the specified imperial unit.
	/// </summary>
	/// <typeparam name="TNumber">The type of the output number.</typeparam>
	/// <param name="quantity">The quantity to convert.</param>
	/// <param name="unit">The imperial unit to convert to.</param>
	/// <returns>The value in the specified unit.</returns>
	public TNumber ToImperialUnit<TNumber>(TQuantity quantity, ImperialUnit unit) where TNumber : INumber<TNumber>;

	/// <summary>
	/// Gets the base SI unit for this quantity type.
	/// </summary>
	/// <returns>The base SI unit.</returns>
	public SIUnit GetBaseSIUnit();

	/// <summary>
	/// Gets all available SI units for this quantity type.
	/// </summary>
	/// <returns>An enumerable of available SI units.</returns>
	public IEnumerable<SIUnit> GetAvailableSIUnits();

	/// <summary>
	/// Gets all available imperial units for this quantity type.
	/// </summary>
	/// <returns>An enumerable of available imperial units.</returns>
	public IEnumerable<ImperialUnit> GetAvailableImperialUnits();
}

/// <summary>
/// Base implementation of strongly-typed conversion calculator.
/// </summary>
/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
public abstract class TypedConversionCalculator<TQuantity> : ITypedConversionCalculator<TQuantity>
	where TQuantity : PhysicalQuantity<TQuantity>, new()
{
	private readonly SIUnit _baseSIUnit;
	private readonly List<SIUnit> _availableSIUnits = [];
	private readonly List<ImperialUnit> _availableImperialUnits = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="TypedConversionCalculator{TQuantity}"/> class.
	/// </summary>
	/// <param name="baseSIUnit">The base SI unit for this quantity type.</param>
	/// <param name="availableSIUnits">The available SI units for conversion.</param>
	/// <param name="availableImperialUnits">The available imperial units for conversion.</param>
	protected TypedConversionCalculator(SIUnit baseSIUnit, IEnumerable<SIUnit> availableSIUnits, IEnumerable<ImperialUnit> availableImperialUnits)
	{
		ArgumentNullException.ThrowIfNull(baseSIUnit);
		ArgumentNullException.ThrowIfNull(availableSIUnits);
		ArgumentNullException.ThrowIfNull(availableImperialUnits);

		_baseSIUnit = baseSIUnit;
		_availableSIUnits.Add(baseSIUnit);
		_availableSIUnits.AddRange(availableSIUnits.Where(u => u != baseSIUnit));
		_availableImperialUnits.AddRange(availableImperialUnits);
	}

	/// <inheritdoc/>
	public virtual TQuantity FromSIUnit<TNumber>(TNumber value, SIUnit unit) where TNumber : INumber<TNumber>
	{
		ArgumentNullException.ThrowIfNull(unit);
		double doubleValue = Convert.ToDouble(value);

		return unit == _baseSIUnit
			? PhysicalQuantity<TQuantity>.Create(doubleValue)
			: unit is DerivedSIUnit derivedUnit && derivedUnit.BaseUnit == _baseSIUnit
			? PhysicalQuantity<TQuantity>.Create(derivedUnit.ToBase(doubleValue))
			: throw new ArgumentException($"Unit '{unit}' is not supported for {typeof(TQuantity).Name}", nameof(unit));
	}

	/// <inheritdoc/>
	public virtual TQuantity FromImperialUnit<TNumber>(TNumber value, ImperialUnit unit) where TNumber : INumber<TNumber>
	{
		ArgumentNullException.ThrowIfNull(unit);

		if (unit.SIBaseUnit != _baseSIUnit)
		{
			throw new ArgumentException($"Imperial unit '{unit}' does not convert to base SI unit '{_baseSIUnit}' for {typeof(TQuantity).Name}", nameof(unit));
		}

		double doubleValue = Convert.ToDouble(value);
		double siValue = unit.ToSI(doubleValue);
		return PhysicalQuantity<TQuantity>.Create(siValue);
	}

	/// <inheritdoc/>
	public virtual TNumber ToSIUnit<TNumber>(TQuantity quantity, SIUnit unit) where TNumber : INumber<TNumber>
	{
		ArgumentNullException.ThrowIfNull(quantity);
		ArgumentNullException.ThrowIfNull(unit);

		return unit == _baseSIUnit
			? TNumber.CreateChecked(quantity.Quantity)
			: unit is DerivedSIUnit derivedUnit && derivedUnit.BaseUnit == _baseSIUnit
			? TNumber.CreateChecked(derivedUnit.FromBase(quantity.Quantity))
			: throw new ArgumentException($"Unit '{unit}' is not supported for {typeof(TQuantity).Name}", nameof(unit));
	}

	/// <inheritdoc/>
	public virtual TNumber ToImperialUnit<TNumber>(TQuantity quantity, ImperialUnit unit) where TNumber : INumber<TNumber>
	{
		ArgumentNullException.ThrowIfNull(quantity);
		ArgumentNullException.ThrowIfNull(unit);

		if (unit.SIBaseUnit != _baseSIUnit)
		{
			throw new ArgumentException($"Imperial unit '{unit}' does not convert to base SI unit '{_baseSIUnit}' for {typeof(TQuantity).Name}", nameof(unit));
		}

		double convertedValue = unit.FromSI(quantity.Quantity);
		return TNumber.CreateChecked(convertedValue);
	}

	/// <inheritdoc/>
	public virtual SIUnit GetBaseSIUnit() => _baseSIUnit;

	/// <inheritdoc/>
	public virtual IEnumerable<SIUnit> GetAvailableSIUnits() => _availableSIUnits.AsReadOnly();

	/// <inheritdoc/>
	public virtual IEnumerable<ImperialUnit> GetAvailableImperialUnits() => _availableImperialUnits.AsReadOnly();
}
