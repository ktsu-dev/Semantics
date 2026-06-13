// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Bespoke members of <see cref="DirectionalityIndex{T}"/>; the logarithmic core
/// is generated from <c>logarithmic.json</c>.
/// </summary>
/// <typeparam name="T">The floating-point storage type.</typeparam>
public readonly partial record struct DirectionalityIndex<T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the index of an omnidirectional source (0 dB).</summary>
	public static DirectionalityIndex<T> Omnidirectional => new(T.Zero);
}
