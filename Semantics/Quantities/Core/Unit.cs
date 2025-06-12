// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Unified implementation for all physical units.
/// </summary>
/// <remarks>
/// Initializes a new instance of the Unit struct.
/// </remarks>
/// <param name="name">The full name of the unit.</param>
/// <param name="symbol">The symbol/abbreviation of the unit.</param>
/// <param name="dimension">The physical dimension this unit measures.</param>
/// <param name="toBaseFactor">The multiplication factor to convert to the base unit.</param>
/// <param name="toBaseOffset">The offset to add when converting to the base unit (default: 0.0).</param>
/// <param name="system">The unit system this unit belongs to (default: SI_Derived).</param>
public readonly struct Unit(
	string name,
	string symbol,
	PhysicalDimension dimension,
	UnitSystem system,
	double toBaseFactor = 1.0,
	double toBaseOffset = 0.0) : IUnit, IEquatable<Unit>
{
	/// <summary>Gets the full name of the unit.</summary>
	public string Name { get; } = string.Intern(name);

	/// <summary>Gets the symbol/abbreviation of the unit.</summary>
	public string Symbol { get; } = string.Intern(symbol);

	/// <summary>Gets the physical dimension this unit measures.</summary>
	public PhysicalDimension Dimension { get; } = dimension;

	/// <summary>Gets the multiplication factor to convert to the base unit.</summary>
	public double ToBaseFactor { get; } = toBaseFactor;

	/// <summary>Gets the offset to add when converting to the base unit (0.0 for linear units).</summary>
	public double ToBaseOffset { get; } = toBaseOffset;

	/// <summary>Gets the unit system this unit belongs to.</summary>
	public UnitSystem System { get; } = system;

	/// <summary>
	/// Determines whether this unit is equal to another unit.
	/// </summary>
	/// <param name="other">The other unit to compare.</param>
	/// <returns>True if the units are equal, false otherwise.</returns>
	public bool Equals(Unit other) =>
		ReferenceEquals(Name, other.Name) &&
		ReferenceEquals(Symbol, other.Symbol) &&
		Dimension.Equals(other.Dimension) &&
		Math.Abs(ToBaseFactor - other.ToBaseFactor) < 1e-10 &&
		Math.Abs(ToBaseOffset - other.ToBaseOffset) < 1e-10 &&
		System == other.System;

	/// <summary>
	/// Determines whether this unit is equal to another object.
	/// </summary>
	/// <param name="obj">The object to compare.</param>
	/// <returns>True if the objects are equal, false otherwise.</returns>
	public override bool Equals(object? obj) => obj is Unit other && Equals(other);

	/// <summary>
	/// Gets the hash code for this unit.
	/// </summary>
	/// <returns>A hash code for this unit.</returns>
	public override int GetHashCode() => HashCode.Combine(Name, Symbol, Dimension, ToBaseFactor, ToBaseOffset, System);

	/// <summary>
	/// Determines whether two units are equal.
	/// </summary>
	/// <param name="left">The first unit.</param>
	/// <param name="right">The second unit.</param>
	/// <returns>True if the units are equal, false otherwise.</returns>
	public static bool operator ==(Unit left, Unit right) => left.Equals(right);

	/// <summary>
	/// Determines whether two units are not equal.
	/// </summary>
	/// <param name="left">The first unit.</param>
	/// <param name="right">The second unit.</param>
	/// <returns>True if the units are not equal, false otherwise.</returns>
	public static bool operator !=(Unit left, Unit right) => !left.Equals(right);

	/// <summary>
	/// Returns a string representation of this unit.
	/// </summary>
	/// <returns>The unit's symbol.</returns>
	public override string ToString() => Symbol;
}
