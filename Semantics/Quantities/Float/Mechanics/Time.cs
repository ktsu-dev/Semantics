// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates

namespace ktsu.Semantics.Float;

/// <summary>
/// Represents a time quantity with float precision.
/// </summary>
public sealed record Time : Generic.Time<float>
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
	public static new Time FromSeconds(float seconds) => new() { Quantity = seconds };

}
