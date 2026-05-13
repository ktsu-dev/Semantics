// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System;

/// <summary>
/// Thrown when a unit conversion cannot be performed — typically because the
/// target unit's dimension does not match the source quantity's dimension.
/// </summary>
/// <remarks>
/// In the typed compile-time path (<see cref="IPhysicalQuantity{T}"/> → <c>In(I&lt;Dim&gt;Unit)</c>),
/// dimension mismatches fail at compile time. This exception remains for runtime
/// scenarios where a quantity is converted via the untyped <see cref="IUnit"/> surface.
/// </remarks>
public sealed class UnitConversionException : ArgumentException
{
	public UnitConversionException() { }

	public UnitConversionException(string message) : base(message) { }

	public UnitConversionException(string message, Exception inner) : base(message, inner) { }
}
