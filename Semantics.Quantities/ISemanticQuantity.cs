// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.Semantics;

using System.Numerics;

/// <summary>
/// Base interface for all semantic quantities.
/// </summary>
/// <typeparam name="T">The storage type for the quantity value.</typeparam>
public interface ISemanticQuantity<T> where T : struct, INumber<T>
{
	/// <summary>Gets the quantity value.</summary>
	public T Quantity { get; }
}
