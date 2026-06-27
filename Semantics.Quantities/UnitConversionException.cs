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
	/// <summary>Initializes a new instance of the <see cref="UnitConversionException"/> class.</summary>
	public UnitConversionException() { }

	/// <summary>Initializes a new instance of the <see cref="UnitConversionException"/> class with a message.</summary>
	/// <param name="message">The message that describes the conversion error.</param>
	public UnitConversionException(string message) : base(message) { }

	/// <summary>Initializes a new instance of the <see cref="UnitConversionException"/> class with a message and inner exception.</summary>
	/// <param name="message">The message that describes the conversion error.</param>
	/// <param name="inner">The exception that caused this exception.</param>
	public UnitConversionException(string message, Exception inner) : base(message, inner) { }
}
