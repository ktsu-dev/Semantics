// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

/// <summary>
/// Exception thrown when unit conversion fails.
/// </summary>
public class UnitConversionException : Exception
{
	/// <summary>Gets the source unit.</summary>
	public IUnit? SourceUnit { get; }

	/// <summary>Gets the target unit.</summary>
	public IUnit? TargetUnit { get; }

	/// <summary>
	/// Initializes a new instance of the UnitConversionException class.
	/// </summary>
	public UnitConversionException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the UnitConversionException class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UnitConversionException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the UnitConversionException class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
	public UnitConversionException(string message, Exception innerException) : base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the UnitConversionException class with source and target units and reason.
	/// </summary>
	/// <param name="source">The source unit.</param>
	/// <param name="target">The target unit.</param>
	/// <param name="reason">The reason for the conversion failure.</param>
	public UnitConversionException(IUnit source, IUnit target, string reason) : base($"Cannot convert from {source?.Symbol} to {target?.Symbol}: {reason}")
	{
		SourceUnit = source;
		TargetUnit = target;
	}
}
