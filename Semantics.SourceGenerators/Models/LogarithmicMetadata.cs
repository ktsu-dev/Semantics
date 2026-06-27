// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace Semantics.SourceGenerators.Models;

using System.Collections.Generic;

/// <summary>
/// Metadata model for logarithmic-scale quantity generation (decibel levels,
/// pitch intervals, pH). Logarithmic scales don't obey linear arithmetic, so
/// they are generated as standalone record structs that convert to and from
/// their linear generated counterparts rather than as physical dimensions.
/// </summary>
public class LogarithmicMetadata
{
	public List<LogarithmicScaleDefinition> LogarithmicScales { get; set; } = [];
}

/// <summary>
/// Definition of a single logarithmic scale type.
/// </summary>
public class LogarithmicScaleDefinition
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string? Remarks { get; set; }

	/// <summary>Display format with <c>{0}</c> as the value placeholder, e.g. <c>"{0} dB SPL"</c>.</summary>
	public string DisplayFormat { get; set; } = "{0}";

	/// <summary>Name of the raw-scalar factory method (<c>Create</c>, <c>FromDecibels</c>, …).</summary>
	public string ScalarFactory { get; set; } = "Create";

	/// <summary>Whether to emit log-space addition and subtraction operators.</summary>
	public bool Arithmetic { get; set; }

	public List<LogarithmicConversionDefinition> Conversions { get; set; } = [];
}

/// <summary>
/// A conversion between the logarithmic scale and a linear generated quantity:
/// <c>scale = multiplier · log_base(linear / reference)</c> and its inverse.
/// </summary>
public class LogarithmicConversionDefinition
{
	/// <summary>The linear generated quantity type name (e.g. <c>SoundPressure</c>).</summary>
	public string Linear { get; set; } = string.Empty;

	/// <summary>The log-space multiplier (20 for field quantities, 10 for power quantities, 1200 for cents, …).</summary>
	public string Multiplier { get; set; } = "10";

	/// <summary>The logarithm base; defaults to 10.</summary>
	public string LogBase { get; set; } = "10";

	/// <summary>The reference value the linear quantity is divided by; defaults to 1.</summary>
	public LogarithmicReferenceDefinition? Reference { get; set; }

	/// <summary>Factory method name override; defaults to <c>From{Linear}</c>.</summary>
	public string? FromName { get; set; }

	/// <summary>Conversion method name override; defaults to <c>To{Linear}</c>.</summary>
	public string? ToName { get; set; }

	public string? FromSummary { get; set; }
	public string? ToSummary { get; set; }
}

/// <summary>
/// The reference for a logarithmic conversion: either a named constant on
/// <c>PhysicalConstants.Generic</c> or a literal value.
/// </summary>
public class LogarithmicReferenceDefinition
{
	/// <summary>Name of a <c>PhysicalConstants.Generic</c> accessor (e.g. <c>ReferenceSoundPressure</c>).</summary>
	public string? Constant { get; set; }

	/// <summary>A literal reference value (e.g. <c>"1000"</c>).</summary>
	public string? Value { get; set; }
}
