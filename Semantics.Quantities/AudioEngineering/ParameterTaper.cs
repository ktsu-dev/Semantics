// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

/// <summary>
/// Describes how a <see cref="NormalizedParameter{T}"/> maps a host-normalized position in
/// <c>[0, 1]</c> onto its real value range.
/// </summary>
public enum ParameterTaper
{
	/// <summary>
	/// A straight linear mapping, optionally bent by a power-curve skew. With a skew of one the mapping
	/// is purely linear; a skew greater than one gives more resolution near the minimum (an exponential
	/// feel), and a skew less than one gives more resolution near the maximum.
	/// </summary>
	Linear,

	/// <summary>
	/// A logarithmic (constant-ratio) mapping where equal movements in the normalized position produce
	/// equal frequency/gain ratios. This is the natural taper for frequency and gain controls. Requires
	/// the range minimum and maximum to share the same sign and be non-zero.
	/// </summary>
	Logarithmic,
}
