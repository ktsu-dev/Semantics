// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents a time quantity with double precision.
/// </summary>
public sealed record Time : Generic.Time<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Time"/> class.
	/// </summary>
	public Time() : base() { }

	/// <summary>
	/// Creates a new Time from a value in seconds.
	/// </summary>
	/// <param name="seconds">The value in seconds.</param>
	/// <returns>A new Time instance.</returns>
	public static new Time FromSeconds(double seconds) => new() { Value = seconds };
}
