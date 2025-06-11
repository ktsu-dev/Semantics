// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Conversions;

using System.Collections.Concurrent;

/// <summary>
/// Registry for managing conversion calculators for different quantity types.
/// </summary>
public static class ConversionRegistry
{
	private static readonly ConcurrentDictionary<Type, object> _calculators = new();

	/// <summary>
	/// Gets the conversion calculator for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>The conversion calculator for the specified quantity type.</returns>
	/// <exception cref="NotSupportedException">Thrown when no calculator is registered for the specified quantity type.</exception>
	public static IConversionCalculator<TQuantity> GetCalculator<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		Type quantityType = typeof(TQuantity);

		if (_calculators.TryGetValue(quantityType, out object? calculator))
		{
			return (IConversionCalculator<TQuantity>)calculator;
		}

		// Try to create default calculators for known types
		IConversionCalculator<TQuantity>? newCalculator = CreateDefaultCalculator<TQuantity>();
		if (newCalculator != null)
		{
			_calculators.TryAdd(quantityType, newCalculator);
			return newCalculator;
		}

		throw new NotSupportedException($"No conversion calculator registered for type {quantityType.Name}");
	}

	/// <summary>
	/// Registers a conversion calculator for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <param name="calculator">The conversion calculator to register.</param>
	public static void RegisterCalculator<TQuantity>(IConversionCalculator<TQuantity> calculator)
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		ArgumentNullException.ThrowIfNull(calculator);
		_calculators.AddOrUpdate(typeof(TQuantity), calculator, (_, _) => calculator);
	}

	/// <summary>
	/// Checks if a conversion calculator is registered for the specified quantity type.
	/// </summary>
	/// <typeparam name="TQuantity">The type of the physical quantity.</typeparam>
	/// <returns>True if a calculator is registered, otherwise false.</returns>
	public static bool IsRegistered<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new() => _calculators.ContainsKey(typeof(TQuantity));

	/// <summary>
	/// Clears all registered calculators.
	/// </summary>
	public static void Clear() => _calculators.Clear();

	private static IConversionCalculator<TQuantity>? CreateDefaultCalculator<TQuantity>()
		where TQuantity : PhysicalQuantity<TQuantity>, new()
	{
		Type quantityType = typeof(TQuantity);

		return quantityType.Name switch
		{
			nameof(Length) => (IConversionCalculator<TQuantity>)(object)LengthConversionCalculator.Instance,
			nameof(Mass) => (IConversionCalculator<TQuantity>)(object)MassConversionCalculator.Instance,
			nameof(Time) => (IConversionCalculator<TQuantity>)(object)TimeConversionCalculator.Instance,
			nameof(Temperature) => (IConversionCalculator<TQuantity>)(object)TemperatureConversionCalculator.Instance,
			nameof(Angle) => (IConversionCalculator<TQuantity>)(object)AngleConversionCalculator.Instance,
			nameof(Energy) => (IConversionCalculator<TQuantity>)(object)EnergyConversionCalculator.Instance,
			nameof(Velocity) => (IConversionCalculator<TQuantity>)(object)VelocityConversionCalculator.Instance,
			nameof(Force) => (IConversionCalculator<TQuantity>)(object)ForceConversionCalculator.Instance,
			nameof(Power) => (IConversionCalculator<TQuantity>)(object)PowerConversionCalculator.Instance,
			nameof(Acceleration) => (IConversionCalculator<TQuantity>)(object)AccelerationConversionCalculator.Instance,
			nameof(Volume) => (IConversionCalculator<TQuantity>)(object)VolumeConversionCalculator.Instance,
			nameof(Area) => (IConversionCalculator<TQuantity>)(object)AreaConversionCalculator.Instance,
			nameof(Pressure) => (IConversionCalculator<TQuantity>)(object)PressureConversionCalculator.Instance,
			_ => null
		};
	}

	static ConversionRegistry()
	{
		// Register default calculators
		RegisterCalculator(LengthConversionCalculator.Instance);
		RegisterCalculator(MassConversionCalculator.Instance);
		RegisterCalculator(TimeConversionCalculator.Instance);
		RegisterCalculator(TemperatureConversionCalculator.Instance);
		RegisterCalculator(AngleConversionCalculator.Instance);
		RegisterCalculator(EnergyConversionCalculator.Instance);
		RegisterCalculator(VelocityConversionCalculator.Instance);
		RegisterCalculator(ForceConversionCalculator.Instance);
		RegisterCalculator(PowerConversionCalculator.Instance);
		RegisterCalculator(AccelerationConversionCalculator.Instance);
		RegisterCalculator(VolumeConversionCalculator.Instance);
		RegisterCalculator(AreaConversionCalculator.Instance);
		RegisterCalculator(PressureConversionCalculator.Instance);
	}
}
