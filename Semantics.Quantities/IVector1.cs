// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics.Quantities;

using System.Numerics;

/// <summary>
/// Marker interface for Vector1 (signed one-dimensional) quantity types.
/// Vector1 quantities represent signed values along a single axis.
/// Arithmetic operators are inherited from <see cref="SemanticQuantity{TSelf, TStorage}"/>
/// rather than declared here to avoid ambiguity.
/// </summary>
/// <typeparam name="TSelf">The implementing quantity type.</typeparam>
/// <typeparam name="T">The numeric storage type.</typeparam>
public interface IVector1<TSelf, T>
	where TSelf : IVector1<TSelf, T>
	where T : struct, INumber<T>
{
	/// <summary>Gets the signed value along the single axis.</summary>
	public T Value { get; }

	/// <summary>Gets a quantity with value zero.</summary>
	public static abstract TSelf Zero { get; }
}
