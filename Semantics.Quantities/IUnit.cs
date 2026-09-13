// Copyright (c) 2023-2026 ktsu-dev contributors

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
/// <para>
/// The conversion itself reads <see cref="ToBaseFactorAs{T}"/> and <see cref="ToBaseOffsetAs{T}"/>,
/// not the <see cref="double"/> properties. Their default implementations convert those properties,
/// so a unit written outside this library behaves as it always did. The generated units override
/// them with the factor materialised directly into the storage type, which is what gives a
/// <see cref="decimal"/> quantity all 28 of its digits.
/// </para>
/// </remarks>
public interface IUnit
{
	/// <summary>Gets the full name of the unit (e.g. <c>"Kilometer"</c>).</summary>
	public string Name { get; }

	/// <summary>Gets the unit's symbol/abbreviation (e.g. <c>"km"</c>).</summary>
	public string Symbol { get; }

	/// <summary>Gets the unit system this unit belongs to.</summary>
	public UnitSystem System { get; }

	/// <summary>Gets the dimension this unit measures.</summary>
	public DimensionInfo Dimension { get; }

	/// <summary>Gets the multiplication factor used in the to-base affine conversion.</summary>
	public double ToBaseFactor { get; }

	/// <summary>Gets the additive offset used in the to-base affine conversion.</summary>
	public double ToBaseOffset { get; }

	/// <summary>
	/// Gets the multiplication factor used in the to-base affine conversion, at the precision of a storage type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns>The factor as <typeparamref name="T"/>. By default, <see cref="ToBaseFactor"/> converted with <c>T.CreateChecked</c>.</returns>
	public T ToBaseFactorAs<T>() where T : struct, INumber<T>
		=> T.CreateChecked(ToBaseFactor);

	/// <summary>
	/// Gets the additive offset used in the to-base affine conversion, at the precision of a storage type.
	/// </summary>
	/// <typeparam name="T">The numeric storage type.</typeparam>
	/// <returns>The offset as <typeparamref name="T"/>. By default, <see cref="ToBaseOffset"/> converted with <c>T.CreateChecked</c>.</returns>
	public T ToBaseOffsetAs<T>() where T : struct, INumber<T>
		=> T.CreateChecked(ToBaseOffset);

	/// <summary>Converts a value expressed in this unit to the dimension's SI base unit.</summary>
	public T ToBase<T>(T value) where T : struct, INumber<T>
		=> (value * ToBaseFactorAs<T>()) + ToBaseOffsetAs<T>();

	/// <summary>Converts a value expressed in the dimension's SI base unit to this unit.</summary>
	public T FromBase<T>(T baseValue) where T : struct, INumber<T>
		=> (baseValue - ToBaseOffsetAs<T>()) / ToBaseFactorAs<T>();
}
