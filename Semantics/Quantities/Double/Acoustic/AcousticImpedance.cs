// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable CA1716 // Identifiers should not match keywords

namespace ktsu.Semantics.Double;

/// <summary>
/// Represents an acoustic impedance quantity with double precision.
/// </summary>
public sealed record AcousticImpedance : Generic.AcousticImpedance<double>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AcousticImpedance"/> class.
	/// </summary>
	public AcousticImpedance() : base() { }

	/// <summary>
	/// Creates a new AcousticImpedance from a value in pascal-seconds per meter.
	/// </summary>
	/// <param name="pascalSecondsPerMeter">The value in pascal-seconds per meter.</param>
	/// <returns>A new AcousticImpedance instance.</returns>
	public static new AcousticImpedance FromPascalSecondsPerMeter(double pascalSecondsPerMeter) => new() { Value = pascalSecondsPerMeter };

	/// <summary>
	/// Creates a new AcousticImpedance from a value in rayls.
	/// </summary>
	/// <param name="rayls">The value in rayls (Pa·s/m).</param>
	/// <returns>A new AcousticImpedance instance.</returns>
	public static new AcousticImpedance FromRayls(double rayls) => new() { Value = rayls };
}
