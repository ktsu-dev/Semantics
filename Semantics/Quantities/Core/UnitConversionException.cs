// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Exception thrown when unit conversion fails.
/// </summary>
/// <remarks>
/// Initializes a new instance of the UnitConversionException class.
/// </remarks>
/// <param name="source">The source unit.</param>
/// <param name="target">The target unit.</param>
/// <param name="reason">The reason for the conversion failure.</param>
public class UnitConversionException(IUnit source, IUnit target, string reason) : Exception($"Cannot convert from {source.Symbol} to {target.Symbol}: {reason}")
{
	/// <summary>Gets the source unit.</summary>
	public IUnit SourceUnit { get; } = source;

	/// <summary>Gets the target unit.</summary>
	public IUnit TargetUnit { get; } = target;
	/// <inheritdoc/>
	public UnitConversionException()
	{
	}
	public UnitConversionException(string message) : base(message)
	{
	}
}
