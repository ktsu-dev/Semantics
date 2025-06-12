// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// A bootstrap unit used during initialization to break circular dependencies.
/// This unit is replaced with the actual unit after the Units class is initialized.
/// </summary>
/// <param name="name">The name of the unit.</param>
/// <param name="symbol">The symbol of the unit.</param>
internal sealed class BootstrapUnit(string name, string symbol) : IUnit
{
	/// <inheritdoc/>
	public string Name { get; } = name;

	/// <inheritdoc/>
	public string Symbol { get; } = symbol;

	/// <inheritdoc/>
	public PhysicalDimension Dimension => default; // Will be set properly later

	/// <inheritdoc/>
	public UnitSystem System => UnitSystem.SIDerived;

	/// <inheritdoc/>
	public double ToBaseFactor => 1.0;

	/// <inheritdoc/>
	public double ToBaseOffset => 0.0;

	/// <inheritdoc/>
	public bool Equals(IUnit? other) => ReferenceEquals(this, other);

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is IUnit other && Equals(other);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Name, Symbol);
}
