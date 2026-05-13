// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Common surface for every physical unit. Carries the unit's name, symbol,
/// the <see cref="DimensionInfo"/> it belongs to, and the affine conversion
/// (<c>base = value × ToBaseFactor + ToBaseOffset</c>) that maps values in
/// this unit to the SI base unit of the dimension.
/// </summary>
/// <remarks>
/// <para>
/// For dimensional compile-time safety, generated quantity types do not accept
/// the raw <see cref="IUnit"/> on <c>In(...)</c>. Each dimension has its own
/// marker interface (e.g. <c>ILengthUnit : IUnit</c>) which only its units
/// implement, and the generated <c>In(I&lt;Dim&gt;Unit)</c> overload accepts
/// only that family. So <c>length.In(Units.Kilogram)</c> fails to compile.
/// </para>
/// <para>
/// <c>ToBase</c> / <c>FromBase</c> are default-implemented; concrete units only
/// have to provide <see cref="ToBaseFactor"/> and <see cref="ToBaseOffset"/>.
/// </para>
/// </remarks>
public interface IUnit
{
	/// <summary>Gets the full name of the unit (e.g. <c>"Kilometer"</c>).</summary>
	string Name { get; }

	/// <summary>Gets the unit's symbol/abbreviation (e.g. <c>"km"</c>).</summary>
	string Symbol { get; }

	/// <summary>Gets the unit system this unit belongs to.</summary>
	UnitSystem System { get; }

	/// <summary>Gets the dimension this unit measures.</summary>
	DimensionInfo Dimension { get; }

	/// <summary>Gets the multiplication factor used in the to-base affine conversion.</summary>
	double ToBaseFactor { get; }

	/// <summary>Gets the additive offset used in the to-base affine conversion.</summary>
	double ToBaseOffset { get; }

	/// <summary>Converts a value expressed in this unit to the dimension's SI base unit.</summary>
	T ToBase<T>(T value) where T : struct, INumber<T>
		=> (value * T.CreateChecked(ToBaseFactor)) + T.CreateChecked(ToBaseOffset);

	/// <summary>Converts a value expressed in the dimension's SI base unit to this unit.</summary>
	T FromBase<T>(T baseValue) where T : struct, INumber<T>
		=> (baseValue - T.CreateChecked(ToBaseOffset)) / T.CreateChecked(ToBaseFactor);
}
